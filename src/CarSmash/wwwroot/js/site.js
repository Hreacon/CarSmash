// Write your Javascript code.
$(document)
    .ready(function () {
        $("#card-form")
            .submit(function (event) {
                Stripe.setPublishableKey('pk_test_bNHd4oEUnVKkaeuqpGSdXvL4');
                Stripe.card.createToken({
                    number: $('#card-number').val(),
                    cvc: $('#card-cvc').val(),
                    exp_month: $('#card-expiry-month').val(),
                    exp_year: $('#card-expiry-year').val()
                }, stripeResponseHandler);
                event.preventDefault();

                function stripeResponseHandler(status, response) {

                    // Grab the form:
                    var $form = $('#card-form');

                    if (response.error) { // Problem!

                        // Show the errors on the form
                        console.log(response.error.message);
                        $form.find('button').prop('disabled', false); // Re-enable submission

                    } else { // Token was created!

                        // Get the token ID:
                        var token = response.id;
                        console.log(token);
                        // Insert the token into the form so it gets submitted to the server:
                        $form.append($('<input type="hidden" name="stripeToken" />').val(token));

                        // Submit the form:
                        $form.get(0).submit();

                    }
                }
            });
        $('select').material_select();
    });


