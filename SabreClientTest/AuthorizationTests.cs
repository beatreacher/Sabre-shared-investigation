using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.Threading.Tasks;
//using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SabreClientTest
{
    [TestClass]
    public class AuthorizationTests
    {
        private const string EncodedType = "ISO-8859-1";
        private const string AuthPostData = "grant_type=";


        //[Fact]
        [TestMethod]
        public async Task GetToken()
        {
            string url = @"https://sws3-crt.cert.sabre.com";

            //string encodedclientId = System.Convert.ToBase64String(Encoding.GetEncoding(EncodedType).GetBytes("7971"));
            //string encodedpassword = System.Convert.ToBase64String(Encoding.GetEncoding(EncodedType).GetBytes("WS102513"));
            //CredentialCache credentials = new CredentialCache();
            //credentials.Add(new System.Uri(url), "Basic", new NetworkCredential(encodedclientId, encodedpassword));
            
            var sbb = GetRequest();
            try
            {
                using (HttpClient client = new HttpClient { BaseAddress = new Uri(url) })
                {
                    client.DefaultRequestHeaders.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypes.));
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("gzip,deflate"));

                    HttpResponseMessage responseMessage = await client.PostAsync("", new StringContent(sbb, Encoding.UTF8, "text/xml"));
                    //var resp = await responseMessage.Content.ReadAsStringAsync();
                    //var document = XDocument.Load(stream);

                    var response = await responseMessage.Content.ReadAsStringAsync();

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(response);
                    ProcessResponse(doc);
                    /*var binarySecurityToken = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "BinarySecurityToken");
                    var Action = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Action");

                    var userName = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Username");
                    var password = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Password");
                    var ipcc = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "Organization");
                    var conversationIds = document.Descendants().Where(x => x.Name.LocalName == "ConversationId");*/
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        public void ParseResponse()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(GetXmlResponse());
                ProcessResponse(doc);
            }
            catch (Exception e)
            {
                //throw;
            }

        }

        private string GetRequest()
        {
            var body = new StringBuilder();
            body.AppendLine("<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:eb=\"http://www.ebxml.org/namespaces/messageHeader\">");
            body.AppendLine("<SOAP-ENV:Header>");
            body.AppendLine("<eb:MessageHeader SOAP-ENV:mustUnderstand=\"1\" eb:version=\"1.0\">");
            body.AppendLine("<eb:From><eb:PartyId>9aq99999</eb:PartyId></eb:From>");
            body.AppendLine("<eb:To><eb:PartyId>123ws123</eb:PartyId></eb:To>");
            body.AppendLine("<eb:ConversationId>1234567</eb:ConversationId>");
            body.AppendLine("<eb:Action>SessionCreateRQ</eb:Action>");
            body.AppendLine("</eb:MessageHeader>");
            body.AppendLine("<wsse:Security xmlns:wsse=\"http://schemas.xmlsoap.org/ws/2002/12/secext\">");
            body.AppendLine("<wsse:UsernameToken>");
            body.AppendLine("<wsse:Username>7971</wsse:Username>");
            body.AppendLine("<wsse:Password>WS102513</wsse:Password>");
            body.AppendLine("<Organization>92RG</Organization>");
            body.AppendLine("<Domain>DEFAULT</Domain>");
            body.AppendLine("</wsse:UsernameToken>");
            body.AppendLine("</wsse:Security>");
            body.AppendLine("</SOAP-ENV:Header>");
            body.AppendLine("<SOAP-ENV:Body></SOAP-ENV:Body>");
            body.AppendLine("</SOAP-ENV:Envelope>");

            return body.ToString();
        }

        private void ProcessResponse(XmlDocument doc)
        {
            var cn = doc.DocumentElement.ChildNodes;
            var n1 = doc.DocumentElement.SelectNodes("soap-env:Envelope/soap-env:Header");
            var n2 = doc.DocumentElement.SelectNodes("//soap-env:Envelope/soap-env:Header");
            var n3 = doc.SelectNodes("soap-env:Envelope/soap-env:Header");
            var n4 = doc.SelectNodes("//soap-env:Envelope/soap-env:Header");

            var n5 = doc.SelectNodes("soap-env:Envelope/soap-env:Header/eb:MessageHeader/");
            var n6 = doc.SelectSingleNode("soap-env:Envelope/soap-env:Header/eb:MessageHeader/eb:ConversationId");
            var n7 = doc.SelectNodes("soap-env:Envelope/soap-env:Header/wsse:Security");
            var n8 = doc.SelectSingleNode("soap-env:Envelope/soap-env:Header/wsse:Security/wsse:BinarySecurityToken");


            string jsonText = JsonConvert.SerializeXmlNode(doc);
            int b = 1;
        }

        private string GetXmlResponse()
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
    }
}
 