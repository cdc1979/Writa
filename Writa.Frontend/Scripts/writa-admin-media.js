$(document).ready(function () {

    $("#dz").dropzone(
        {
            url: "/writa/uploadimage",
            paramName: "file",
            uploadMultiple: true,
            maxFilesize: 10,
            acceptedFiles: "image/*",
            addRemoveLinks: true,
            success: function (f) {
                               
            }
        });
});

