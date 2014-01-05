function removeItem(array, item) {
    for (var i in array) {
        if (array[i] == item) {
            array.splice(i, 1);
            break;
        }
    }
}

$(document).ready(function () {
    var opts = {
        container: 'epiceditor',
        textarea: null,
        basePath: '/scripts/EpicEditor-v0.2.2/',
        clientSideStorage: true,
        localStorageName: 'epiceditor',
        useNativeFullscreen: true,
        parser: marked,
        file: {
            name: 'epiceditor',
            defaultContent: '',
            autoSave: 100
        },
        theme: {
            base: 'themes/base/epiceditor.css',
            preview: 'themes/preview/preview-dark.css',
            editor: 'themes/editor/epic-dark.css'
        },
        button: {
            preview: true,
            fullscreen: true,
            bar: "auto"
        },
        focusOnLoad: false,
        shortcut: {
            modifier: 18,
            fullscreen: 70,
            preview: 80
        },
        string: {
            togglePreview: 'Toggle Preview Mode',
            toggleEdit: 'Toggle Edit Mode',
            toggleFullscreen: 'Enter Fullscreen'
        },
        autogrow: { minHeight: 700, maxHeight: 700 }

    }

    var editor = new EpicEditor(opts).load(function () { /*alert("loaded!");*/ });

    var dataService = new breeze.DataService({
        serviceName: "/breeze/breeze",
        hasServerMetadata: false // don't ask the server for metadata
    });

    var manager = new breeze.EntityManager({ dataService: dataService });
    var query = new breeze.EntityQuery().from("GetPosts").orderByDesc("PostCreated");

    var viewmodel = {
        posts: ko.observableArray([]),
        myobj: ko.observable()
    }
    manager.executeQuery(query).then(function (data) {
        var g = data;
        viewmodel.posts(g.results);
        viewmodel.myobj = ko.viewmodel.fromModel(data.results[0]);
        editor.importFile('epiceditor', data.results[0].PostContent);
        ko.applyBindings(viewmodel);       
    }).fail(function (e) {
        alert(e);
    });
    

    viewmodel.postClick = function (obj) {
        $("[postid]").parent().parent().removeClass("success");
        $("[postid='" + obj.PostId + "']").parent().parent().addClass("success");
        var queryx = new breeze.EntityQuery().from("GetPost").withParameters({ postid: obj.PostId });
        manager.executeQuery(queryx).then(function (data) {
            ko.viewmodel.updateFromModel(viewmodel.myobj, data.results[0]);
            editor.importFile('epiceditor', data.results[0].PostContent);
        }).fail(function (e) {
            alert(e);
        });
    }
    $(document).on("click",".showsettings",function (e) {
        var btn = $(this);
        btn.button('loading');
        $.ajax({
            url: "/writa/editsettings/"+$(this).attr("postid"),
            type: "GET",
            cache: false,
            //contentType: 'application/json',
            success: function (msg) {
                $(".modal-body").html(msg);
                $('.selectpicker').selectpicker();
                btn.button('reset');
                $("#popdz").dropzone(
                        {
                            url: "/writa/uploadimage",
                            paramName: "file",
                            uploadMultiple: true,
                            maxFilesize: 10,
                            acceptedFiles: "image/*",
                            addRemoveLinks: true,
                            success: function (f) {
                                $("#PostThumbnail").val("/Images/"+f.name);
                                this.removeFile(f);
                            }
                        });

                $("#PostCreateDate").datepicker({format: 'mm/dd/yyyy'});
                $("#PostStartDate").datepicker({ format: 'mm/dd/yyyy' });
                $("#myModal").modal('show');
            }
        });

        e.preventDefault();
    });
    $("#updatepostbutton").click(function () {
        var title = $("#posttitle").val();
        var postid = $(this).attr("postid");
        var post = editor.exportFile("epiceditor", "text");
        //alert(postid);
        $.ajax({
            url: "/api/posts/update",
            type: "POST",
            //contentType: 'application/json',
            data: { postid: postid, postmarkdown: post, posttitle: title },
            success: function (msg) {
                toastr.success(msg);
                $("#myModal").modal('hide');
            }
        });
    });

    $(document).on("click", "#deletepostbutton", function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to delete this post?'))
        {
            var postid = $("#PostId").val();
            //alert(postid);
            var obj = { postid: postid };
            $.ajax({
                url: "/api/posts/deletepost",
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(obj),
                success: function (msg) {
                    toastr.success(msg);
                }
            });
        }
    });

    $(document).on("click", "#savesettingsform", function (e) {
        var formdata = $("#settingsform").serialize();
        //alert(formdata);
        var btn = $(this);
        btn.button('loading');
        $.ajax({
            url: "/api/posts/updatesettings",
            type: "POST",
            //contentType: 'application/json',
            data: formdata,
            success: function (msg) {
                btn.button('reset');
                if (msg.indexOf("Error") == 0) {
                    toastr.error(msg);
                }
                else {
                    toastr.success(msg);
                }
            }
        });
        e.preventDefault();
    });

    $(document).on("click", "#selectimagebutton", function (e) {
        $.ajax({
            url: "/writa/selectimage?classname=thumbnailclick",
            type: "GET",
            cache: false,
            //contentType: 'application/json',
            success: function (msg) {
                $("#myModal2Label").html("Click to insert image as thumbnail");
                $("#myModal2Body").html(msg);
                $("#myModal2").modal('show');
            }
        });

        e.preventDefault();
    });
    $(document).on("click", ".thumbnailclick", function () {
        $("#PostThumbnail").val("/Images/" + $(this).attr("filename"));
        $("#myModal2").modal('hide');
    });

    $(".imagebrowser").click(function (e) {

        $.ajax({
            url: "/writa/selectimage?classname=imageclick",
            type: "GET",
            cache: false,
            //contentType: 'application/json',
            success: function (msg) {
                $("#myModalLabel").html("Click to insert image code into page");
                $(".modal-body").html(msg);
                $("#myModal").modal('show');
            }
        });

        e.preventDefault();
    });

    $(document).on("click", ".imageclick", function () {
        var html = editor.exportFile("epiceditor", "text");
        html = html + '\n![image](/Images/' + $(this).attr("filename") + ')';
        toastr.success("Added " + $(this).attr("filename") + " to editor");
        editor.importFile('epiceditor', html);
    });

    //tagging
    $("#addtagbutton").click(function () {
        $.ajax({
            url: "/api/posts/tag",
            type: "POST",
            data: {postid: $(this).attr("postid"), tag: $("#addtag").val(), "delete": false},
            success: function (msg) {
                if (msg.indexOf("Deleted") == 0) {
                    toastr.error(msg);
                }
                else {
                    toastr.success(msg);
                    viewmodel.myobj.PostTags.push($("#addtag").val());
                }
            }
        });
    });
    $(document).on("click",".deletetag",function () {
        var txt = $(this).text();
        $.ajax({
            url: "/api/posts/tag",
            type: "POST",
            data: { postid: $("#postid").val(), tag: $(this).text(), "delete": true },
            success: function (msg) {
                if (msg.indexOf("Deleted") == 0) {
                    viewmodel.myobj.PostTags.remove(txt);
                    toastr.error(msg);
                }
                else {
                    toastr.success(msg);
                    viewmodel.myobj.PostTags.push($("#addtag").val());
                }
            }
        });
    });

});

