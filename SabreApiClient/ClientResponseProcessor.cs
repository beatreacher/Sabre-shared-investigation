using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SabreApiClient
{
    public class ClientResponseProcessor
    {
        public Session GetSessionFromResponse(string response)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);
            var nsmgr = GetXmlNamespaceManager(doc);
            string conversationId = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:ConversationId").InnerText;
            string token = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/wsse:Security/wsse:BinarySecurityToken").InnerText;
            string messageId = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:MessageData/eb:MessageId").InnerText;
            string timeStamp = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:MessageData/eb:Timestamp").InnerText;

            return new Session(token, conversationId, messageId, timeStamp);
        }

        private static XmlNamespaceManager GetXmlNamespaceManager(XmlDocument doc)
        {
            var xmlNamespaces = new Dictionary<string, string>
            {
                {"xsl", "http://www.w3.org/1999/XSL/Transform"},
                {"soap-env", "http://schemas.xmlsoap.org/soap/envelope/" },
                {"eb", "http://www.ebxml.org/namespaces/messageHeader" },
                {"wsse", "http://schemas.xmlsoap.org/ws/2002/12/secext" }
            };

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            foreach (var item in xmlNamespaces)
            {
                nsmgr.AddNamespace(item.Key, item.Value);
            }

            return nsmgr;
        }

        private static XmlNode GetNodeByPath(XmlDocument doc, XmlNamespaceManager nmgr, string path)
        {
            return doc.SelectSingleNode(path, nmgr);
        }
    }
}
