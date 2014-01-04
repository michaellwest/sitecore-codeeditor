using System;
using System.Web.UI.WebControls;
using Sitecore.Diagnostics;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.XamlSharp.Xaml;

namespace Sitecore.SharedSource.Shell.Applications.ContentEditor.Dialogs.EditCode
{
    public class EditCodePage : XamlMainControl
    {
        // Fields
        protected TextBox Code;

        // Methods
        protected virtual void Cancel_Click()
        {
            SheerResponse.CloseWindow();
        }

        protected virtual void OK_Click()
        {
            var text = Code.Text;
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
            base.OnLoad(e);
            if (AjaxScriptManager.Current.IsEvent) return;
            var handle = UrlHandle.Get();
            Code.Text = handle["code"];
        }
    }
}