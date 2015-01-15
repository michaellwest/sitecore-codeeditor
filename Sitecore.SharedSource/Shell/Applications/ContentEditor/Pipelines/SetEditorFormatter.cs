using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;

namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor.Pipelines
{
    public class SetEditorFormatter
    {
        public void Process(RenderContentEditorArgs args)
        {
            args.EditorFormatter = new EditorFormatter
            {
                Arguments = args
            };
        }
    }
}