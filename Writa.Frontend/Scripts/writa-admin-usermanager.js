$(document).ready(function () {
    var baseurl = $("#baseurl").val();
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
    
    $(".edituserbutton").click(function (e) {
        var content = "";
        var userid = $(this).attr("userid");
        $.ajax({
            url: baseurl + "writa/edituser",
            data: { userid: userid },
            async: false,
            type: "POST",
            success: function (msg) {
                content = msg;                
            }
        });

        bootbox.dialog({
            message: content,
            title: 'Edit User',
            buttons: {
                success: {
                    label: "Update User",
                    className: "btn-success",
                    callback: function () {
                        
                        var formdata = $("#edituserform").serialize();
                        alert(formdata);
                        if ($("#edituserform").valid()) {

                            $.ajax({
                                url: baseurl + "api/updateuser",
                                async: false,
                                dataType: 'json',
                                data: formdata,
                                type: "POST",
                                success: function (msg) {
                                    alert(msg);
                                },
                                error: function () { return false; }
                            });

                        }
                        else {
                            return false;
                        }

                    }
                },
                danger: {
                    label: "Cancel",
                    className: "btn-danger",
                    callback: function () {
                        
                        toastr.error("Cancelled");
                    }
                }
            }
        });
        //$('.selectpicker').selectpicker();
    });

    $("#adduserform").validate(validatetemplate);

    $(".changepasswordbutton").click(function () {
        var content = "";
        var userid = $(this).attr("userid");
        $.ajax({
            url: baseurl + "writa/changepassword",
            data: { userid: userid },
            async: false,
            type: "POST",
            success: function (msg) {
                content = msg;
            }
        });

        bootbox.dialog({
            message: content,
            title: 'Change Password',
            buttons: {
                success: {
                    label: "Save Changes",
                    className: "btn-success",
                    callback: function () {

                        var formdata = $("#changepasswordform").serialize();

                        if ($("#changepasswordform").valid()) {

                            $.ajax({
                                url: baseurl + "api/changepassword",
                                async: false,
                                dataType: 'json',
                                data: formdata,
                                type: "POST",
                                success: function (msg) {
                                    toastr.success("Users password was updated");
                                },
                                error: function () { return false; }
                            });

                        }
                        else {
                            return false;
                        }

                    }
                },
                danger: {
                    label: "Cancel",
                    className: "btn-danger",
                    callback: function () {
                        toastr.error("Cancelled");
                    }
                }
            }
        });
    });

    $("#adduserbutton").click(function (e) {

        var content = "";
        $.ajax({
            url: baseurl + "writa/adduser",
            async: false,
            type: "POST",
            success: function (msg) {
                content = msg;
            }
        });

        bootbox.dialog({
            message: content,
            title: 'Add New User',
            buttons: {
                success: {
                    label: "Create User",
                    className: "btn-success",
                    callback: function () {
                        var adddata = $("#adduserform").serialize();
                        
                        if ($("#adduserform").valid()) {

                            $.ajax({
                                url: baseurl + "api/adduser",
                                async: false,
                                dataType: 'json',
                                data:adddata,
                                type: "POST",
                                success: function (msg) {
                                    toastr.success(msg);
                                    window.location = baseurl + 'writa/usermanager';
                                },
                                error: function () { return false; }
                            });

                        }
                        else {
                            return false;
                        }
                        
                        
                    }
                },
                danger: {
                    label: "Cancel",
                    className: "btn-danger",
                    callback: function () {
                        toastr.error("Cancelled");
                    }
                }
            }
        });
        e.preventDefault();
    });

});

