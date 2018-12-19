using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FluentAssertions;
using Domain.Models;
using SabreApiClient;

namespace SabreClientTest
{
    [TestClass]
    public class AuthenticationTests
    {
        IDictionary<string, string> Actions = new Dictionary<string, string>
        {
            {"CreateToken", "TokenCreateRQ"},
            {"CreateSession", "SessionCreateRQ"}
        };

        [TestMethod]
        public async Task GetToken()
        {
            var client = new Client("*", "*", "*", "DEFAULT");
            var sabreApiClient = new SabreApiClient.SabreApiClient();
            var session = await sabreApiClient.CreateAccessToken(client, "TokenCreateRQ");

            Assert.IsNotNull(session);
            Assert.IsNotNull(session.ConversationId);
            Assert.IsNotNull(session.Token);
            Assert.IsNotNull(session.MessageId);
            Assert.IsNotNull(session.TimeStamp);
        }

        [TestMethod]
        public void ParseResponse()
        {
            var responseProcessor = new ClientResponseProcessor();
            var session = responseProcessor.GetSessionFromResponse(GetXmlResponse());

            session.ConversationId.Should().Be("V1@280b16ec-5eac-46c0-893f-c88f8e8cb632@310b16ec-5dad-46c0-893f-c88f8e8cb643@780b16ec-5eac-46c0-893f-c88f8e8cb699");
            session.Token.Should().Be(@"Shared/IDL:IceSess\/SessMgr:1\.0.IDL/Common/!ICESMS\/ACPCRTC!ICESMSLB\/CRT.LB!1544695565786!2375!17");
            session.MessageId.Should().Be("888281363657680150");
            session.TimeStamp.Should().Be("2018-12-13T10:06:05");
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
    }
}
 