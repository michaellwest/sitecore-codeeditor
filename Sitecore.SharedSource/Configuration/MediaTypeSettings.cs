using System;
using System.Linq;
using System.Xml;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.Data;
using Sitecore.SharedSource.Extensions;
using Sitecore.Xml;

namespace Sitecore.SharedSource.Configuration
{
    public class MediaTypeSettings
    {
        public MediaTypeSettings()
        {
            var configNode = Factory.GetConfigNode("mediaLibrary");
            ParseMediaTypes(configNode);
        }

        public SafeDictionary<string, MediaTypeConfig> MediaTypes { get; } = new SafeDictionary<string, MediaTypeConfig>(StringComparer.OrdinalIgnoreCase);

        private void ParseMediaTypes(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");
            var childNode = XmlUtil.GetChildNode("mediaTypes", configNode);
            if (childNode == null)
                return;
            var childNodes = XmlUtil.GetChildNodes("mediaType", childNode).Where(x=>x.Attributes?["name"] != null).ToList();
            if (!childNodes.Any()) return;

            foreach (var configNode1 in childNodes.Where(x=> x.Attributes?["enableCodeEditor"] != null && x.Attributes["enableCodeEditor"].Value.Is("true")))
            {
                ParseMediaType(configNode1);
            }
        }

        private void ParseMediaType(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");

            var attribute = XmlUtil.GetAttribute("extensions", configNode);
            if (attribute.Length == 0) return;

            var extensions = StringUtil.Split(attribute, ',', true);
            lock (MediaTypes.SyncRoot)
            {
                foreach (var extension in extensions)
                {
                    if (MediaTypes.Keys.Contains(extension))
                    {
                        FillConfiguration(MediaTypes[extension], configNode);
                    }
                    else
                    {
                        var config = new MediaTypeConfig(XmlUtil.GetAttribute("name", configNode, attribute), attribute);
                        FillConfiguration(config, configNode);
                        MediaTypes[extension] = config;
                    }
                }
            }
        }

        private static void FillConfiguration(MediaTypeConfig config, XmlNode configNode)
        {
            config.SharedTemplate = GetChildValue("sharedTemplate", configNode, config.SharedTemplate);
            config.VersionedTemplate = GetChildValue("versionedTemplate", configNode, config.VersionedTemplate);
        }

        private static string GetChildValue(string nodeName, XmlNode node, string oldValue)
        {
            Assert.ArgumentNotNull(nodeName, "nodeName");
            Assert.ArgumentNotNull(node, "node");

            var childNode = XmlUtil.GetChildNode(nodeName, node);
            if (childNode != null)
            {
                return XmlUtil.GetValue(childNode);
            }

            return !string.IsNullOrEmpty(oldValue) ? oldValue : string.Empty;
        }
    }
}