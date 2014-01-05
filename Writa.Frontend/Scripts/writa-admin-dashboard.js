$(document).ready(function () {



    $("#sitemapbutton").click(function (e) {

        $.ajax({
            url: "/api/tasks/sitemap",
            type: "POST",
            success: function (msg) {
                toastr.success("Sitemap has been updated!");
            }
        });

        e.preventDefault();
    });

    $("#showcasebutton").click(function (e) {
        var btn = $(this);
        btn.button('loading');
        $.ajax({
            url: "/api/tasks/showcase",
            type: "POST",
            success: function (msg) {
                btn.button('reset');
                $("#dashboardmodallabel").html("Showcase Submission Received");
                $("#dashboardmodalbody").html('<p><a href="http://writa.org">Writa.org</a> offers a blog showcase, which will automatically load and distribute new blog posts, and share your blog to our users.  Each submission is manually checked, to make sure you blogs content is appropriate for sharing, so please allow a few days for your blog to show up in the showcase.</p><hr/><div class="alert alert-success">' + msg + "</div>");
                $("#dashboardmodal").modal('show');
            }
        });

        e.preventDefault();
    });


    

    $(document).on("click", ".restorefile", function (e) {
        var btn = $(this);
        btn.button('loading');
        var filename = $(this).attr("filename");
        var obj = { filename: filename };
        if (confirm('Are you sure? This will overwrite all existing posts.')) {
            $.ajax({
                url: "/api/tasks/restore",
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(obj),
                success: function (msg) {
                    toastr.success("Your data has been restored.");
                    btn.button('reset');
                }
            });
        }
        e.preventDefault();
    });

    $("#restorebutton").click(function (e) {

        var btn = $(this);
        btn.button('loading');
        $.ajax({
            url: "/writa/restoredb",
            type: "POST",
            success: function (msg) {
                $("#dashboardmodallabel").html("Restore Data From File");
                $("#dashboardmodalbody").html(msg);
                $("#dashboardmodal").modal('show');
                btn.button('reset');
            }
        });

        e.preventDefault();
    });

    $("#backupbutton").click(function (e) {

        $.ajax({
            url: "/api/tasks/backup",
            type: "POST",
            success: function (msg) {
                toastr.success("Backup has been created (Check App_Data Folder)");
            }
        });

        e.preventDefault();
    });

});

