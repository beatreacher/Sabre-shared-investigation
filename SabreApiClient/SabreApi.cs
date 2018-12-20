using Domain.Models;
using SabreApiClient.RequestModelHelpers;
//using SabreApiClient.SessionCreator;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SabreApiClient
{
    public class SabreApi
    {
        private readonly SabreMapper SabreMapper = new SabreMapper();

        public async Task<Session> CreateSession(Credentials credentials, string methodName)
        {
            var header = SabreMapper.GetMessageHeader<SessionCreator.MessageHeader>(Guid.NewGuid().ToString(), methodName, credentials.Organization);
            var security = new SessionCreator.Security
            {
                UsernameToken = new SessionCreator.SecurityUsernameToken
                {
                    Username = credentials.UserName,
                    Password = credentials.Password,
                    Organization = credentials.Organization,
                    Domain = credentials.Domain
                }
            };

            var pos = new SessionCreator.SessionCreateRQPOS
            {
                Source = new SessionCreator.SessionCreateRQPOSSource { PseudoCityCode = credentials.Organization }
            };
            var req = new SessionCreator.SessionCreateRQ
            {
                POS = pos
            };

            var proxy = new SessionCreator.SessionCreatePortTypeClient("SessionCreatePortType");
            var response = await proxy.SessionCreateRQAsync(header, security, req);

            CheckErrors(req, response);

            var session = new Session(
                response.Security.BinarySecurityToken,
                response.MessageHeader.ConversationId,
                response.MessageHeader.MessageData.MessageId,
                response.MessageHeader.MessageData.Timestamp,
                response.MessageHeader.CPAId
                );


            return session;
        }

        public async Task<string> CloseSession(Session session)
        {
            var header = SabreMapper.GetMessageHeader<SessionCloseRQ.MessageHeader>(session.ConversationId, "SessionCloseRQ", session.Organization);
            var security = new SessionCloseRQ.Security { BinarySecurityToken = session.Token };

            var source = new SessionCloseRQ.SessionCloseRQPOSSource { PseudoCityCode = session.Organization };
            var pos = new SessionCloseRQ.SessionCloseRQPOS { Source = source };
            var req = new SessionCloseRQ.SessionCloseRQ {POS = pos};
        
            var proxy = new SessionCloseRQ.SessionClosePortTypeClient("SessionClosePortType");
            var response = await proxy.SessionCloseRQAsync(header, security, req);
            CheckErrors(req, response);

            return response.SessionCloseRS.status;
        }

        private string ToXmlString(object obj)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(stringwriter, obj);
            return stringwriter.ToString();
        }

        private void CheckErrors(object request, dynamic response)
        {
            try
            {
                if (request == null || response == null)
                {
                    return;
                }

                // Sabre Web Services version 1.x
                var t = (response as object).GetType().GetProperties().FirstOrDefault(x => x.Name == "Errors");
                if ((response as object).GetType().GetProperties().FirstOrDefault(x => x.Name == "Errors") != null)
                {
                    if (response.Errors != null)
                    {
                        var responseXml = this.ToXmlString(response);
                        var requestXml = this.ToXmlString(request);
                        throw new SabreException(response.Errors.Error.ErrorInfo.Message)
                        {
                            SabreRequest = requestXml,
                            SabreResponse = responseXml
                        };
                    }
                }

                // Sabre Web Services version 2.x
                if ((response as object).GetType().GetProperties().FirstOrDefault(x => x.Name == "ApplicationResults") != null)
                {
                    if (response.ApplicationResults.Error != null)
                    {
                        var responseXml = this.ToXmlString(response);
                        var requestXml = this.ToXmlString(request);

                        var errors = response.ApplicationResults.Error as dynamic[];
                        if (errors == null || errors.Length == 0)
                        {
                            return;
                        }

                        var sb = new StringBuilder();
                        foreach (var error in errors)
                        {
                            foreach (var result in error.SystemSpecificResults)
                            {
                                foreach (var message in result.Message)
                                {
                                    var messageContent = message.Value as string;
                                    sb.AppendLine(messageContent);
                                }
                            }
                        }
                        throw new SabreException(sb.ToString())
                        {
                            SabreRequest = requestXml,
                            SabreResponse = responseXml
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}