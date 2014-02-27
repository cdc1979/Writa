$(document).ready(function () {
    $("#privacymsg").click(function (e) {
        bootbox.dialog({
            message: "Privacymsg",
            title: 'Privacy Policy'
        });
    });
});