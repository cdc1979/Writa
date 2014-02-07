$(document).ready(function () {
    var baseurl = $("#baseurl").val();
    $("#dz").dropzone(
        {
            url: baseurl+"writa/uploadimage",
            paramName: "file",
            uploadMultiple: true,
            maxFilesize: 10,
            acceptedFiles: "image/*",
            addRemoveLinks: true,
            success: function (f) {
                               
            }
        });
});

