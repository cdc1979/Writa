$(document).ready(function () {
    $('.selectpicker').selectpicker();
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

    $("#savesettingsbutton").click(function (e) {

        e.preventDefault();

        $("#settingsform").validate(validatetemplate);

        var isvalid = $("#settingsform").valid();

        if (isvalid) {

            var formdata = $("#settingsform").serialize();

            $.ajax({
                url: "/api/settings/save",
                type: "POST",
                data: formdata,
                success: function (msg) {
                    toastr.success("Settings have been saved");
                }
            });
        }
        
    });

    

});

