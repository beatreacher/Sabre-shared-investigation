using Domain.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SabreApiClient
{
    public class SabreApiClient
    {
        private const string SabreUrl = @"https://sws3-crt.cert.sabre.com";

        public async Task<Session> CreateAccessToken(Client client, string methodName)
        {
            var request = GetRequest(client.UserName, client.Password, client.Organization, client.Domain, methodName, Guid.NewGuid().ToString());

            using (HttpClient httpClient = new HttpClient { BaseAddress = new Uri(SabreUrl) })
            {
                httpClient.DefaultRequestHeaders.Clear();
                HttpResponseMessage responseMessage = await httpClient.PostAsync("", new StringContent(request, Encoding.UTF8, "text/xml"));
                var response = await responseMessage.Content.ReadAsStringAsync();

                var responseProcessor = new ClientResponseProcessor();
                return responseProcessor.GetSessionFromResponse(response);
            }
        }

        private static string GetRequest(string userName, string password, string ipcc, string domain, string actionType, string conversationId)
        {
            return
                "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:eb=\"http://www.ebxml.org/namespaces/messageHeader\">" +
                    "<SOAP-ENV:Header>" +
                        "<eb:MessageHeader SOAP-ENV:mustUnderstand=\"1\" eb:version=\"1.0\">" +
                            "<eb:From><eb:PartyId>9aq99999</eb:PartyId></eb:From>" +
                            "<eb:To><eb:PartyId>123ws123</eb:PartyId></eb:To>" +
                            $"<eb:ConversationId>{conversationId}</eb:ConversationId>" +
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
    }
}
