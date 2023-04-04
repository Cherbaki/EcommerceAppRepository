$('document').ready(() => {

    let quantityIsValid = false;

    // Get the message container element
    var messageContainer = $('#message-container');

    // Show the message div for 5 seconds, then hide it
    setTimeout(function () {
        messageContainer.fadeOut();
    }, 3000);

    $('#QuantityMinus').click(() => {
        let currentQuantity = Number.parseInt($('#QuantityIFId').val());
        currentQuantity--;
        $('#QuantityIFId').val(currentQuantity);
        $('#QuantityValue').text(currentQuantity);
        ValidateQuantity();
    });

    $('#QuantityPlus').click(() => {
        let currentQuantity = Number.parseInt($('#QuantityIFId').val());
        currentQuantity++;
        $('#QuantityIFId').val(currentQuantity);
        $('#QuantityValue').text(currentQuantity);
        ValidateQuantity();
    });

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

        let ProductID = $('#ProductId').text();
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