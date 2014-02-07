$(document).ready(function () {
    var baseurl = $("#baseurl").val();
    var validatetemplate = {
        highlight: function (element) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        errorElement: 'span',
        errorClass: 'help-block',
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    };

    $("#ResetButton").click(function (e) {
        $("#resetform").validate(validatetemplate);

        var isvalid = $("#resetform").valid();
        if (isvalid) {

        }
        else {
            e.preventDefault();
        }
    });

    $("#LoginButton").click(function (e) {
        $("#loginform").validate(validatetemplate);

        var isvalid = $("#loginform").valid();
        if (isvalid) {

        }
        else {
            e.preventDefault();
        }
    });

    $("#RegisterAccountButton").click(function (e) {

        $("#registerform").validate(validatetemplate);

        var isvalid = $("#registerform").valid();
        if (isvalid) {

        }
        else {
            e.preventDefault();
        }

    });

});