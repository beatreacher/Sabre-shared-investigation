using System;
using SabreApiClient.Helpers;
using System.Threading.Tasks;
using NLog;

using Domain.Models;

namespace SabreApiClient
{
    public class SessionManager
    {
        Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly SabreMapper SabreMapper = new SabreMapper();
        private readonly XmlResponseProcessor ResponseProcessor = new XmlResponseProcessor();

        public async Task<Session> CreateSession(Credentials credentials, string methodName)
        {
            try
            {
                _logger.Debug("CreateSession started");
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

                ResponseProcessor.CheckErrors(req, response);

                var session = new Session(
                    response.Security.BinarySecurityToken,
                    response.MessageHeader.ConversationId,
                    response.MessageHeader.MessageData.MessageId,
                    response.MessageHeader.MessageData.Timestamp,
                    response.MessageHeader.CPAId
                    );
                return session;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("CreateSession fineshed");
            }
        }

        public async Task<string> CloseSession(Session session)
        {
            try
            {
                _logger.Debug("CloseSession started");

                var header = SabreMapper.GetMessageHeader<SessionCloseRQ.MessageHeader>(session.ConversationId, "SessionCloseRQ", session.Organization);
                var security = new SessionCloseRQ.Security { BinarySecurityToken = session.Token };

                var source = new SessionCloseRQ.SessionCloseRQPOSSource { PseudoCityCode = session.Organization };
                var pos = new SessionCloseRQ.SessionCloseRQPOS { Source = source };
                var req = new SessionCloseRQ.SessionCloseRQ { POS = pos };

                var proxy = new SessionCloseRQ.SessionClosePortTypeClient("SessionClosePortType");
                var response = await proxy.SessionCloseRQAsync(header, security, req);
                ResponseProcessor.CheckErrors(req, response);

                return response.SessionCloseRS.status;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("CloseSession fineshed");
            }
        }
    }
}
