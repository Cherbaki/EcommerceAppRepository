$('document').ready(() => {

    let quantityIsValid = false;

    $('#QuantityIFId').change(ValidateQuantity);
    $('#BuyButtonId').click(() => {
        if (!quantityIsValid)
            return;

        ValidateQuantity('RealBuyButtonId');
    });
    $('#AddToCartId').click(() => {
        if (!quantityIsValid)
            return;

        ValidateQuantity('RealAddToCartButtonId');
    });

    ValidateQuantity();

    function ValidateQuantity(callerId) {

        let ProductID = $('#ProductIdHeaderId').text();
        let specifiedQuantity = $('#QuantityIFId').val();

        $.ajax({
            type: 'GET',
            url: '/Product/ValidateQuantityAgainstStockQuantity',
            data: { productId: ProductID, givenQuantity: specifiedQuantity },
            success: function (response) {

                var quantityIFSpan = $('#QuantityIFSpanId');

                if (!response.success) {
                    $('#QuantityIFSpanId').text('There is not enough item in the stock');
                    quantityIsValid = false;
                }
                else { 
                    $('#QuantityIFSpanId').text('');
                    quantityIsValid = true;
                }

                if (callerId === 'RealBuyButtonId')
                    $('#RealBuyButtonId').click();
                else if (callerId === 'RealAddToCartButtonId')
                    $('#RealAddToCartButtonId').click();

            }
        });



    }

});