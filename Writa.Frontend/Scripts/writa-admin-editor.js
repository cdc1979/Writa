$(document).ready(function () {

    var converter = new Markdown.Converter();
    //   var html = converter.makeHtml(text);

    //var converter = new Markdown.converter();
    var editor = CodeMirror.fromTextArea($('#markdown')[0], {
        mode: "text/html",
        theme: "default",
        onCursorActivity: function (arg) {

            var linenumber = editor.getCursor().line;

            $('input:file').remove()
            //$("#preview").html(converter.toHTML(arg.getValue()));
            $("#preview").html(converter.makeHtml(arg.getValue()));
            

            $("a").each(function (index, obj) {
                var src = $(this).text();
                
                //alert(src.length);
                
                if (src == "image") {
                    $(this).parent().append('<div class="dropzone" style="width: 100%; height: 80px; padding: 20px; border: 1px solid #000;" id="dz' + index + '">Drop Image Here' + index + '</div>');
                    //$("div#dz" + index).dropzone({ , addedfile: function () {  } });
                    Dropzone.autoDiscover = false;
                    $("#dz" + index).dropzone(
                        {
                            url: "/writa/uploadimage",
                            paramName: "file",
                            uploadMultiple: false,
                            maxFilesize: 10,
                            acceptedFiles: "image/*",
                            success: function (f) {
                                
                                var count = editor.lineCount(), i;
                                for (i = 0; i < count; i++) {
                                    var g = editor.getLine(i + 1);
                                    if (g == "![image]()") {
                                        editor.setLine(i+1, "!["+f.name+"](/Images/"+f.name+")");
                                    }
                                    
                                }

                                $("#dz" + index).remove();
                            }
                        });

                }
            });
            
        }
    });

    $("#publishbutton").click(function (e) {
        
        var markdown = editor.getValue();
        var title = $("#posttitle").val();

        $.post('/writa/createpost', { PostTitle: title, PostMarkdown: markdown }
        ,
function (data) {

});

        e.preventDefault();
    });
    

});