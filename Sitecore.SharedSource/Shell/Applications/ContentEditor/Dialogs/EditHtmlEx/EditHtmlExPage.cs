using System;
using System.Web.UI.WebControls;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;
using Sitecore.Web.UI.XamlSharp.Xaml;

namespace Sitecore.SharedSource.Shell.Applications.ContentEditor.Dialogs.EditHtmlEx
{
    public class EditHtmlExPage : XamlMainControl
    {
        // Fields
        protected TextBox Html;
        protected Border RibbonPanel;

        // Methods
        protected virtual void Cancel_Click()
        {
            SheerResponse.CloseWindow();
        }

        protected virtual void OK_Click()
        {
            var text = Html.Text;
            if (string.IsNullOrEmpty(text))
            {
                text = "__#!$No value$!#__";
            }
            SheerResponse.SetDialogValue(text);
            SheerResponse.CloseWindow();
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            Assert.CanRunApplication("/sitecore/content/Applications/Content Editor/Dialogs/EditHtmlEx");
            base.OnLoad(e);
            if (!AjaxScriptManager.Current.IsEvent)
            {
                var handle = UrlHandle.Get();
                Html.Text = handle["html"];
                UpdateRibbon();
            }
        }

        private void UpdateRibbon()
        {
            var ctl = new Ribbon {ID = "Ribbon", CommandContext = new CommandContext()};
            var item =
                Sitecore.Context.Database.GetItem(
                    "/sitecore/content/Applications/Content Editor/Dialogs/EditHtmlEx/Ribbon");
            ctl.CommandContext.RibbonSourceUri = item.Uri;
            RibbonPanel.InnerHtml = HtmlUtil.RenderControl(ctl);
        }
    }
}