Dropzone.autoDiscover = false;

$(document).ready(function () {
    var baseurl = $("#baseurl").val();
    $("#poststartdate").datepicker({ format: 'mm/dd/yyyy' });
    var opts = {
        container: 'epiceditor',
        textarea: null,
        basePath: baseurl + 'scripts/EpicEditor-v0.2.2/',
        clientSideStorage: true,
        localStorageName: 'epiceditor',
        useNativeFullscreen: true,
        parser: marked,
        file: {
            name: 'addpost',
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

    editor.on('preview', function () {
        //console.log("preview");
    });

    var postid = $("#postid").val();
    if (postid.length > 5) {
        $.ajax({
            url: baseurl+"writa/getmarkdown",
            type: "POST",
            data: { postid: postid },
            success: function (msg) {
                editor.importFile('addpost', msg);
            }
        });
    }

    
    $("#dz").dropzone(
        {
            url: baseurl+"writa/uploadimage",
            paramName: "file",
            uploadMultiple: true,
            maxFilesize: 10,
            acceptedFiles: "image/*",
            addRemoveLinks: true,
            success: function (f) {

                //var iframe = document.getElementById("epiceditor-16161");
                //var sel = rangy.getIframeSelection();
                //alert(sel);
                this.removeFile(f);
                var html = editor.exportFile("addpost", "text");
                //myobj = JSON.parse(html);
                //alert(html);
                if (html.indexOf("![image]()") > 0) {
                    html = html.replace('![image]()', '![image](/Images/' + f.name + ')');
                }
                else {
                    html = html + '\n![image](/Images/' + f.name + ')';
                }
                editor.importFile('addpost', html);
            }
        });

    $("#publishbutton").click(function (e) {
        var btn = $(this);
        btn.button('loading');
       var markdown = editor.exportFile("addpost", "text");
       var title = $("#posttitle").val();
       var poststartdate = $("#poststartdate").val();
       //alert(title.length);
       if (title.length < 5) {
           toastr.error("Please enter at least 5 characters for your post title");
       }
       else {
           $.post(baseurl+'writa/createpost', { PostTitle: title, PostMarkdown: markdown, poststartdate: poststartdate }, function (data) {
               //alert(data);
               if (data == "Success") {
                   editor.remove("addpost");
                   toastr.success("Your post has been saved!");
                   editor.reflow();
                   $("#donemodal").modal('show');
                   btn.button('reset');
               }
               else {
                   toastr.error(data);
                   btn.button('reset');
               }
           });
       }
       e.preventDefault();
    });

    $(".imagebrowser").click(function (e) {

        $.ajax({
            url: baseurl + "writa/selectimage",
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
        var html = editor.exportFile("addpost", "text");
        html = html + '\n![image](/Images/' + $(this).attr("filename") + ')';
        toastr.success("Added " + $(this).attr("filename") + " to editor");
        editor.importFile('addpost', html);
    });

});