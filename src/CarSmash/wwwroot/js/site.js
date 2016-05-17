
function ajax(href, target) {
    // scalable ajax function. Use html attributes to get the target. Turn everything into an ajax call with a class and a data-target.
    // Also "breakproof". Site works with refreshing and even if there's no javascript.
    console.log("ajax");
    window.history.pushState('RealEstateGame', 'RealEstateGame', href);
    var tag = '';
    if (href.includes("#")) {
        tag = href.substring(href.indexOf("#"));
        href = href.substring(0, href.indexOf("#"));
    }
    if (href.includes("?")) {
        href = href + "&ajax=true";
    } else {
        href = href + "?ajax=true";
    }

    $.ajax({
        type: 'GET',
        url: href,
        success: function (result) {
            ajaxReturn(target, result, tag);
        }
    });
}
function ajaxPost(form, target) {
    // ajax post requests
    // get the href
    var href = $(form).attr('action');
    console.log("ajaxpost");
    // do the ajax call
    $.ajax({
        type: 'POST',
        url: href,
        data: $(form).serialize(),
        success: function (result) {
            ajaxReturn(target, result, 'no');
        }
    });
}
function ajaxReturn(target, result, tag) {
    console.log("post success");
    if (result.trim().substr(0, 1) == "<") {
        $(target).html(result);
        updatePlayer();
        ajaxInit();
        if (tag.length > 1 && tag != "no") {
            console.log('scroll');
            $(window).scrollTop($(tag).offset().top - 100);
        } else if (tag != "no") {
            $(window).scrollTop(0);
        }
    } else message(result);
}

function ajaxInit() {
    // initialize the ajax calls in place of the standard event handling
    console.log('ajax init');
    $("a.ajax")
    .click(function (event) {
        // remove the class ajax to prevent additional event handlers
        $(this).removeClass("ajax");
        ajax($(this).attr('href'), $(this).attr('data-target'));
        event.preventDefault();
        event.stopImmediatePropagation();
        return false;
    });
    $("button.ajax")
        .parent()
        .submit(function (event) {
            event.preventDefault();
            var button = $(this).find("button.ajax");
            button.removeClass("ajax");
            var flag = $("<input>").attr('name', 'ajax').attr('type', 'hidden').val('true');
            $(this).append(flag);
            console.log(this);
            ajaxPost(this, button.attr("data-target"));
            return false;
        });
}

// Write your Javascript code.
$(document)
    .ready(function () {
        ajaxInit();
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


