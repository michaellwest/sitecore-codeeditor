using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Text;
using Sitecore.Web;

namespace Sitecore.SharedSource.Data.Fields
{
    internal class LinkData
    {
        public string Alt { get; set; }
        public string Link { get; set; }
        public string Match { get; set; }
    }

    public class CodeTextField : CustomField
    {
        public CodeTextField(Field innerField)
            : base(innerField)
        {
            Assert.ArgumentNotNull(innerField, "innerField");
        }

        public CodeTextField(Field innerField, string runtimeValue)
            : base(innerField, runtimeValue)
        {
        }

        public static implicit operator CodeTextField(Field field)
        {
            return field != null ? new CodeTextField(field) : null;
        }

        private static void MatchLinks(string content, ItemLink itemLink, Action<LinkData> action)
        {
            // TODO: Match html links
            var matches = Regex.Matches(content, @"!?\[(?<alt>[^\[]+)\]\((?<link>[^\)]+)\)");
            foreach (Match match in matches)
            {
                if (!match.Success) continue;
                if (itemLink != null && match.Groups["link"].Value != itemLink.TargetPath) continue;

                action(new LinkData
                {
                    Alt = match.Groups["alt"].Value,
                    Link = match.Groups["link"].Value,
                    Match = match.Value
                });
            }
        }

        public override List<WebEditButton> GetWebEditButtons()
        {
            var list = new List<WebEditButton>();
            /*
            Item obj1 =
                Client.GetDatabaseNotNull("core")
                    .GetItem(StringUtil.GetString(InnerField.Source, Settings.HtmlEditor.DefaultProfile));
            if (obj1 == null)
                return list;
            Item obj2 = obj1.Children["WebEdit Buttons"];
            if (obj2 == null)
                return list;
            foreach (Item obj3 in obj2.Children)
            {
                WebEditButton webEditButton = new WebEditButton();
                if (obj3.TemplateID == Ribbon.Separator)
                {
                    webEditButton.IsDivider = true;
                }
                else
                {
                    webEditButton.Header = obj3["Header"];
                    webEditButton.Icon = obj3["Icon"];
                    webEditButton.Click = obj3["Click"];
                    webEditButton.Tooltip = obj3["Tooltip"];
                }
                if (UIUtil.SupportsInlineEditing() || webEditButton.Click.Contains("edithtml"))
                    list.Add(webEditButton);
            }
            */
            return list;
        }

        public override void Relink(ItemLink itemLink, Item newLink)
        {
            Assert.ArgumentNotNull(itemLink, "itemLink");
            Assert.ArgumentNotNull(newLink, "newLink");
            if (!InnerField.HasValue) return;

            var str = Value;
            MatchLinks(str, itemLink, linkData =>
            {
                var linkedItemId = GetLinkedItemId(linkData.Link);
                if (linkedItemId == (ID) null || linkedItemId != itemLink.TargetItemID) return;

                var shellOptions = MediaUrlOptions.GetShellOptions();
                var mediaUrl = MediaManager.GetMediaUrl(newLink, shellOptions);
                Value = str.Replace(linkData.Link, ReplaceUrlPath(linkData.Link, mediaUrl));
            });
        }

        public override void RemoveLink(ItemLink itemLink)
        {
            Assert.ArgumentNotNull(itemLink, "itemLink");

            if (!InnerField.HasValue) return;

            var str = Value;
            MatchLinks(str, itemLink, linkData => { Value = str.Replace(linkData.Match, ""); });
        }

        public override void ValidateLinks(LinksValidationResult result)
        {
            Assert.ArgumentNotNull(result, "result");
            var str = Value;
            if (string.IsNullOrEmpty(str))
                return;

            MatchLinks(str, null, linkData =>
            {
                // Add media links
                DynamicLink link;
                if (DynamicLink.TryParse(linkData.Link, out link))
                {
                    var targetItem = InnerField.Database.GetItem(link.ItemId);
                    AddLink(result, targetItem, linkData.Link);
                }
                else
                {
                    if (!Regex.IsMatch(linkData.Link, @"^http[s]?://"))
                        AddLink(result, null, linkData.Link);
                }

                // TODO: Add text links
            });
        }

        private static void AddLink(LinksValidationResult result, Item targetItem, string targetPath)
        {
            Assert.ArgumentNotNull(result, "result");
            Assert.ArgumentNotNull(targetPath, "targetPath");

            if (targetItem != null)
                result.AddValidLink(targetItem, targetPath);
            else
                result.AddBrokenLink(targetPath);
        }

        private ID GetLinkedItemId(string href)
        {
            Assert.ArgumentNotNull(href, "href");
            DynamicLink dynamicLink;
            try
            {
                dynamicLink = DynamicLink.Parse(href);
            }
            catch (InvalidLinkFormatException ex)
            {
                return null;
            }
            return dynamicLink.ItemId;
        }

        private static string ReplaceUrlPath(string oldHref, string newHref)
        {
            Assert.ArgumentNotNull(oldHref, "oldHref");
            Assert.ArgumentNotNull(newHref, "newHref");
            var urlString1 = new UrlString(newHref);
            var urlString2 = new UrlString(oldHref);
            foreach (string index in urlString2.Parameters)
                if (index != "_id" && index != "_site" && index != "_lang" && index != "_z")
                    urlString1[index] = urlString2[index];
            return urlString1.ToString();
        }
    }
}