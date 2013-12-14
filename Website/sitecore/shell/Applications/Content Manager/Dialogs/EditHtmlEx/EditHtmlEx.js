(function($){
    $('head').append("<style>div[id$=RibbonPanel],textarea[id$=Html] { display: none; } #CodeEditor { width: 100%; height: 100%; } </style>");

    function isHtml(source) {
            // <foo> - looks like html
            // <!--\nalert('foo!');\n--> - doesn't look like html

            var trimmed = source.replace(/^[ \t\n\r]+/, '');
            var comment_mark = '<' + '!-' + '-';
            return (trimmed && (trimmed.substring(0, 1) === '<' && trimmed.substring(0, 4) !== comment_mark));
    }
 
    $(function() {
        var html = $('textarea[id$=Html]');
        var ce = $("<div id='CodeEditor' />");
        html.after(ce);
 
        var codeeditor = ace.edit(ce[0]);
        codeeditor.setTheme("ace/theme/monokai");
        codeeditor.session.setMode("ace/mode/html");
        codeeditor.setShowPrintMargin(false);
 
        codeeditor.session.setValue(html.val().trim());
        codeeditor.session.on('change', function () {
                html.val(codeeditor.session.getValue());
        });

        ace.config.loadModule("ace/ext/emmet", function () {
            ace.require("ace/lib/net").loadScript("/sitecore/shell/Controls/Lib/ace/emmet-core/emmet.js", function () {
                codeeditor.setOption("enableEmmet", true);
            });             
        });
 
        ace.config.loadModule("ace/ext/language_tools", function (module) {
            codeeditor.setOptions({
                enableSnippets: true,
                enableBasicAutocompletion: true
            });
        });

        var codeeeditorcommands = [ {
            name: "format",
            bindKey: { win: "ctrl-shift-f", mac: "ctrl-command-enter", sender: 'codeeditor' },
            exec: function (env, args, request) {
                var source = codeeditor.session.getValue();
                if (source) {
                    if(isHtml(source)) {
                        var opts = {};
                        opts.indent_size = '4';
                        opts.indent_char = opts.indent_size == 1 ? '\t' : ' ';
                        opts.max_preserve_newlines = '5';
                        opts.preserve_newlines = opts.max_preserve_newlines !== -1;
                        opts.keep_array_indentation = false;
                        opts.break_chained_methods = false;
                        opts.indent_scripts = 'normal';
                        opts.brace_style = 'collapse';
                        opts.space_before_conditional = true;
                        opts.unescape_strings = false;
                        opts.wrap_line_length = '0';
                        opts.space_after_anon_function = true;
                        var output = html_beautify(source, opts);
                        codeeditor.session.setValue(output);
                        html.val(output);
                    }
                }
            },
            readOnly: true
        }];

        codeeditor.commands.addCommands(codeeeditorcommands);
    });
}(jQuery));