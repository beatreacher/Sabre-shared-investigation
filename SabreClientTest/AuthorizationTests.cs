using System;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SabreClientTest
{
    [TestClass]
    public class AuthorizationTests
    {
        IDictionary<string, string> Actions = new Dictionary<string, string>
        {
            {"CreateToken", "TokenCreateRQ"},
            {"CreateSession", "SessionCreateRQ"}
        };

        [TestMethod]
        public async Task GetToken()
        {
            string url = @"https://sws3-crt.cert.sabre.com";
            var sbb = GetRequest("7971", "WS102513", "92RG", "DEFAULT", "TokenCreateRQ");
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(url) })
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage responseMessage = await client.PostAsync("", new StringContent(sbb, Encoding.UTF8, "text/xml"));
                var response = await responseMessage.Content.ReadAsStringAsync();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                var nsmgr = GetXmlNamespaceManager(doc);
                string conversationId = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:ConversationId").InnerText;
                string token = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/wsse:Security/wsse:BinarySecurityToken").InnerText;
                string messageId = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:MessageData/eb:MessageId").InnerText;
                string timeStamp = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:MessageData/eb:Timestamp").InnerText;

                Assert.IsNotNull(conversationId);
                Assert.IsNotNull(token);
                Assert.IsNotNull(messageId);
                Assert.IsNotNull(timeStamp);
            }
        }

        [TestMethod]
        public void ParseResponse()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(GetXmlResponse());

            var nsmgr = GetXmlNamespaceManager(doc);
            string conversationId = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:ConversationId").InnerText;
            string token = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/wsse:Security/wsse:BinarySecurityToken").InnerText;
            string messageId = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:MessageData/eb:MessageId").InnerText;
            string timeStamp = GetNodeByPath(doc, nsmgr, "soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:MessageData/eb:Timestamp").InnerText;

            Assert.AreEqual("V1@280b16ec-5eac-46c0-893f-c88f8e8cb632@310b16ec-5dad-46c0-893f-c88f8e8cb643@780b16ec-5eac-46c0-893f-c88f8e8cb699", conversationId);
            Assert.AreEqual(@"Shared/IDL:IceSess\/SessMgr:1\.0.IDL/Common/!ICESMS\/ACPCRTC!ICESMSLB\/CRT.LB!1544695565786!2375!17", token);
            Assert.AreEqual("888281363657680150", messageId);
            Assert.AreEqual("2018-12-13T10:06:05", timeStamp);
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

        private static string GetRequest(string userName, string password, string ipcc, string domain, string actionType)
        {
            return
                "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:eb=\"http://www.ebxml.org/namespaces/messageHeader\">" +
                    "<SOAP-ENV:Header>" +
                        "<eb:MessageHeader SOAP-ENV:mustUnderstand=\"1\" eb:version=\"1.0\">" +
                            "<eb:From><eb:PartyId>9aq99999</eb:PartyId></eb:From>" +
                            "<eb:To><eb:PartyId>123ws123</eb:PartyId></eb:To>" +
                            "<eb:ConversationId>1234567</eb:ConversationId>" +
                            $"<eb:Action>{actionType}</eb:Action>" +
                        "</eb:MessageHeader>" +
                        "<wsse:Security xmlns:wsse=\"http://schemas.xmlsoap.org/ws/2002/12/secext\">" +
                            "<wsse:UsernameToken>" +
                                $"<wsse:Username>{userName}</wsse:Username>" +
                                $"<wsse:Password>{password}</wsse:Password>" +
                                $"<Organization>{ipcc}</Organization>" +
                                $"<Domain>{domain}</Domain>" +
                            "</wsse:UsernameToken>" +
                        "</wsse:Security>" +
                    "</SOAP-ENV:Header>" +
                    "<SOAP-ENV:Body></SOAP-ENV:Body>" +
                "</SOAP-ENV:Envelope>";
        }

        private static string GetXmlResponse()
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <soap-env:Envelope xmlns:soap-env=""http://schemas.xmlsoap.org/soap/envelope/"">
                        <soap-env:Header>
                            <eb:MessageHeader xmlns:eb=""http://www.ebxml.org/namespaces/messageHeader"" eb:version=""1.0"" soap-env:mustUnderstand=""1"">
                                <eb:From>
                                    <eb:PartyId eb:type=""URI""/>
                                </eb:From>
                                <eb:To>
                                    <eb:PartyId eb:type=""URI""/>
                                </eb:To>
                                <eb:CPAId>7TZA</eb:CPAId>
                                <eb:ConversationId>V1@280b16ec-5eac-46c0-893f-c88f8e8cb632@310b16ec-5dad-46c0-893f-c88f8e8cb643@780b16ec-5eac-46c0-893f-c88f8e8cb699</eb:ConversationId>
                                <eb:Service eb:type=""sabreXML"">Session</eb:Service>
                                <eb:Action>SessionCreateRS</eb:Action>
                                <eb:MessageData>
                                    <eb:MessageId>888281363657680150</eb:MessageId>
                                    <eb:Timestamp>2018-12-13T10:06:05</eb:Timestamp>
                                    <eb:RefToMessageId>mid:20001209-133003-2333@clientofsabre.com</eb:RefToMessageId>
                                </eb:MessageData>
                            </eb:MessageHeader>
                            <wsse:Security xmlns:wsse=""http://schemas.xmlsoap.org/ws/2002/12/secext"">
                                <wsse:BinarySecurityToken valueType=""String"" EncodingType=""wsse:Base64Binary"">Shared/IDL:IceSess\/SessMgr:1\.0.IDL/Common/!ICESMS\/ACPCRTC!ICESMSLB\/CRT.LB!1544695565786!2375!17</wsse:BinarySecurityToken>
                            </wsse:Security>
                        </soap-env:Header>
                        <soap-env:Body>
                            <SessionCreateRS xmlns=""http://www.opentravel.org/OTA/2002/11"" version=""1"" status=""Approved"">
                                <ConversationId>V1@280b16ec-5eac-46c0-893f-c88f8e8cb632@310b16ec-5dad-46c0-893f-c88f8e8cb643@780b16ec-5eac-46c0-893f-c88f8e8cb699</ConversationId>
                            </SessionCreateRS>
                        </soap-env:Body>
                    </soap-env:Envelope>";
        }

        //public async Task GetTokenDummy()
        //{
            //const string EncodedType = "ISO-8859-1";
            //const string AuthPostData = "grant_type=";


        //    string url = @"https://sws3-crt.cert.sabre.com";

        //    //string encodedclientId = System.Convert.ToBase64String(Encoding.GetEncoding(EncodedType).GetBytes("7971"));
        //    //string encodedpassword = System.Convert.ToBase64String(Encoding.GetEncoding(EncodedType).GetBytes("WS102513"));
        //    //CredentialCache credentials = new CredentialCache();
        //    //credentials.Add(new System.Uri(url), "Basic", new NetworkCredential(encodedclientId, encodedpassword));

        //    var sbb = GetRequest("7971", "WS102513", "92RG", "DEFAULT");
        //    try
        //    {
        //        using (HttpClient client = new HttpClient { BaseAddress = new Uri(url) })
        //        {
        //            client.DefaultRequestHeaders.Clear();
        //            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypes.));
        //            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("gzip,deflate"));

        //            HttpResponseMessage responseMessage = await client.PostAsync("", new StringContent(sbb, Encoding.UTF8, "text/xml"));
        //            //var resp = await responseMessage.Content.ReadAsStringAsync();
        //            //var document = XDocument.Load(stream);

        //            var response = await responseMessage.Content.ReadAsStringAsync();

        //            XmlDocument doc = new XmlDocument();
        //            doc.LoadXml(response);
        //            var nsmgr = GetXmlNamespaceManager(doc);
        //            ProcessResponse(doc, nsmgr);
        //            /*var binarySecurityToken = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "BinarySecurityToken");
        //            var Action = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Action");

        //            var userName = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Username");
        //            var password = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Password");
        //            var ipcc = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Organization");
        //            var conversationIds = document.Descendants().Where(x => x.Name.LocalName == "ConversationId");*/
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }
        //}

    }
}
 