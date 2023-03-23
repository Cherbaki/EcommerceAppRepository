using Ecommerce.Models;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Net;

namespace Ecommerce.Helpers
{
	public class PayPalHelpers
	{
		public static PayPalHttpClient Client(MyPayPalSetup setup)
		{
			PayPalEnvironment? environment = null;

			if (setup.Environment == "live")
				environment = new LiveEnvironment(setup.ClientId, setup.Secret);
			else
				environment = new SandboxEnvironment(setup.ClientId, setup.Secret);

			var client = new PayPalHttpClient(environment);

			return client;
		}

		public static async Task<PayPalHttp.HttpResponse> CreateOrder(MyPayPalSetup setup, List<MyItem> MyItems)
		{
			PayPalHttp.HttpResponse? response = null;


			var items = new List<Item>();
			double TotalTax = 0, TotalShipping = 0, ItemTotal = 0, TotalValue = 0;
			foreach (var item in MyItems)
			{
				//Parsing methods require ',' as decimal point
				TotalTax += double.Parse(item.TaxValue!) * int.Parse(item.Quantity!);
				TotalShipping += double.Parse(item.ShippingValue!.Replace('.', ',')) * int.Parse(item.Quantity!);
				ItemTotal += double.Parse(item.UnitAmountValue!.Replace('.', ',')) * int.Parse(item.Quantity!);


				//Paypal requires '.' as decimal point
				items.Add(new Item()
				{
					Quantity = item.Quantity,
					Name = item.Name,
					Sku = item.Sku,
					Tax = new PayPalCheckoutSdk.Orders.Money()
					{ CurrencyCode = item.CurrencyCode, Value = item.TaxValue!.Replace(',', '.') },
					UnitAmount = new PayPalCheckoutSdk.Orders.Money()
					{ CurrencyCode = item.CurrencyCode, Value = item.UnitAmountValue!.Replace(',', '.') }
				});
			}

			TotalValue = TotalTax + TotalShipping + ItemTotal;

			try
			{
				var order = new OrderRequest()
				{
					CheckoutPaymentIntent = "CAPTURE",
					PurchaseUnits = new List<PurchaseUnitRequest>()
					{
						new PurchaseUnitRequest()
						{
							Items = items,
							AmountWithBreakdown = new AmountWithBreakdown()
							{
								CurrencyCode = MyItems.ElementAt(0).CurrencyCode,
								AmountBreakdown = new AmountBreakdown()
								{
									TaxTotal = new PayPalCheckoutSdk.Orders.Money()
									{
										CurrencyCode = MyItems.ElementAt(0).CurrencyCode,
										Value = TotalTax.ToString("0.00").Replace(',', '.')
									},
									Shipping = new PayPalCheckoutSdk.Orders.Money() {
										CurrencyCode = MyItems.ElementAt(0).CurrencyCode,
										Value = TotalShipping.ToString("0.00").Replace(',', '.')
									},
									ItemTotal = new PayPalCheckoutSdk.Orders.Money()
									{
										CurrencyCode = MyItems.ElementAt(0).CurrencyCode,
										Value = ItemTotal.ToString("0.00").Replace(',', '.')
									}
								},
								Value = TotalValue.ToString("0.00").Replace(',', '.')//Total Value User will be paying
                            }
						}
					},
					ApplicationContext = new ApplicationContext()
					{
						ReturnUrl = setup.RedirectUrl + "?info=capture",
						CancelUrl = setup.RedirectUrl + "?info=cancel"
					}
				};

				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

				var request = new OrdersCreateRequest();
				request.Prefer("return=representation");
				request.RequestBody(order);
				PayPalHttpClient client = Client(setup);
				response = await client.Execute(request);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}

			return response;
		}

		public static async Task<PayPalHttp.HttpResponse?> CaptureOrder(MyPayPalSetup setup)
		{
			var request = new OrdersCaptureRequest(setup.PayerApprovedOrderId);
			request.RequestBody(new OrderActionRequest());
			var client = Client(setup);
			var response = await client.Execute(request);

			return response;
		}
	}

	public class MyPayPalSetup
	{
		public string? Environment { get; set; }
		public string? ClientId { get; set; }
		public string? Secret { get; set; }
		public string? RedirectUrl { get; set; }
		public string? ApproveUrl { get; set; }
		public string? PayerApprovedOrderId { get; set; }
	}

}
