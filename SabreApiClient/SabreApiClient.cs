using Domain.Models;
using Domain.Models.Customer;
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
            var request = GetCreateAccessRequest(client, methodName, Guid.NewGuid().ToString());

            using (HttpClient httpClient = new HttpClient { BaseAddress = new Uri(SabreUrl) })
            {
                httpClient.DefaultRequestHeaders.Clear();
                HttpResponseMessage responseMessage = await httpClient.PostAsync("", new StringContent(request, Encoding.UTF8, "text/xml"));
                var response = await responseMessage.Content.ReadAsStringAsync();

                var responseProcessor = new ClientResponseProcessor();
                return responseProcessor.GetSessionFromResponse(response);
            }
        }

        public async Task<string> GetTravelItineraryAddInfo(Client client, Session session, Customer customer, Agency agency)
        {
            var request = GetTravelItineraryAddInfoLLSRQRequest(client, session, customer, agency);

            using (HttpClient httpClient = new HttpClient { BaseAddress = new Uri(SabreUrl) })
            {
                httpClient.DefaultRequestHeaders.Clear();
                HttpResponseMessage responseMessage = await httpClient.PostAsync("", new StringContent(request, Encoding.UTF8, "text/xml"));
                var response = await responseMessage.Content.ReadAsStringAsync();

                return response;
            }
        }

        private static string GetCreateAccessRequest(Client client, string actionType, string conversationId)
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
                                $"<wsse:Username>{client.UserName}</wsse:Username>" +
                                $"<wsse:Password>{client.Password}</wsse:Password>" +
                                $"<Organization>{client.Organization}</Organization>" +
                                $"<Domain>{client.Domain}</Domain>" +
                            "</wsse:UsernameToken>" +
                        "</wsse:Security>" +
                    "</SOAP-ENV:Header>" +
                    "<SOAP-ENV:Body></SOAP-ENV:Body>" +
                "</SOAP-ENV:Envelope>";
        }

        private static string GetTravelItineraryAddInfoLLSRQRequest(Client client, Session session, Customer customer, Agency agency)
        {
            return
                "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:eb=\"http://www.ebxml.org/namespaces/messageHeader\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:xsd=\"http://www.w3.org/1999/XMLSchema\">"+
                   "<SOAP-ENV:Header>"+
                      "<eb:MessageHeader SOAP-ENV:mustUnderstand=\"1\" eb:version=\"2.0\">"+
                         "<eb:From><eb:PartyId type=\"urn:x12.org:IO5:01\">1212</eb:PartyId></eb:From>"+
                         "<eb:To><eb:PartyId type=\"urn:x12.org:IO5:01\">2323</eb:PartyId></eb:To>"+
                         $"<eb:CPAId>{client.Organization}</eb:CPAId>" +
                         $"<eb:ConversationId>{session.ConversationId}</eb:ConversationId>" +
                         "<eb:Service eb:type=\"OTA\">TravelItineraryAddInfoLLSRQ</eb:Service>" +
                         "<eb:Action>TravelItineraryAddInfoLLSRQ</eb:Action>" +
                         "<eb:MessageData>" +
                            "<eb:MessageId>1001</eb:MessageId>" +
                            $"<eb:Timestamp>{session.TimeStamp}</eb:Timestamp>" +
                            "<eb:TimeToLive>2013-06-06T23:59:59</eb:TimeToLive>" +
                         "</eb:MessageData>" +
                      "</eb:MessageHeader>" +
                      "<wsse:Security xmlns:wsse=\"http://schemas.xmlsoap.org/ws/2002/12/secext\" xmlns:wsu=\"http://schemas.xmlsoap.org/ws/2002/12/utility\">"+
                         $"<wsse:BinarySecurityToken valueType=\"String\" EncodingType=\"wsse:Base64Binary\">{session.Token}</wsse:BinarySecurityToken>" +
                      "</wsse:Security>" +
                   "</SOAP-ENV:Header>" +
                   "<SOAP-ENV:Body>" +
                      "<TravelItineraryAddInfoRQ Version=\"1.9.1\" xmlns=\"http://webservices.sabre.com/sabreXML/2003/07\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                         "<POS>" +
                           $"<Source PseudoCityCode=\"{client.Organization}\"/>" +
                         "</POS>" +
                         "<CustomerInfo>" +
                            $"<PersonName TravelerRefNumber=\"{customer.TravelerRefNumber}\">" +
                               $"<GivenName>{customer.Person.GivenName}</GivenName>" +
                               $"<Surname>{customer.Person.Surname}</Surname>" +
                               $"<NameReference Text=\"{customer.Person.NameReference}\"/>" +
                            "</PersonName>" +
                            $"<Telephone AreaCityCode=\"{customer.Phone.AreaCityCode}\" PhoneNumber=\"{customer.Phone.PhoneNumber}\" PhoneUseType=\"{customer.Phone.PhoneUseType}\" TravelerRefNumber=\"{customer.TravelerRefNumber}\"/>" +
                            $"<Email EmailAddress=\"{customer.Email}\" TravelerRefNumber=\"{customer.TravelerRefNumber}\"/>" +
                            $"<CustomerIdentifier Identifier=\"{customer.CustomerId}\"/>" +
                         "</CustomerInfo>" +
                         "<AgencyInfo>" +
                            "<Address>" +
                               $"<TPA_Extensions><AgencyName>{agency.Name}</AgencyName></TPA_Extensions>" +
                               $"<StreetNmbr>{agency.Address.Street}</StreetNmbr>" +
                               $"<CityName>{agency.Address.CityName}</CityName>" +
                               $"<PostalCode>{agency.Address.PostalCode}</PostalCode>" +
                               $"<StateCountyProv StateCode=\"{agency.Address.State}\"/>" +
                               $"<CountryName Code=\"{agency.Address.CountryName}\"/>" +
                            "</Address>" +
                            $"<Telephone AreaCityCode=\"{agency.Phone.AreaCityCode}\" PhoneNumber=\"{agency.Phone.PhoneNumber}\" PhoneUseType=\"{agency.Phone.PhoneUseType}\"/>" +
                            $"<Ticketing TicketType=\"{agency.TicketType}\"/>" +
                         "</AgencyInfo>" +
                      "</TravelItineraryAddInfoRQ>" +
                   "</SOAP-ENV:Body>" +
                "</SOAP-ENV:Envelope> ";
        }
    }
}
