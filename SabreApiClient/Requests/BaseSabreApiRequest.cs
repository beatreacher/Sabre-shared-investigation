using System;
using SabreApiClient.Helpers;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Domain.Models;

namespace SabreApiClient.Requests
{
    public abstract class BaseSabreApiRequest<TResponse, TMessageHeader, TSecurity, TRequest>
    {
        private readonly ILogger _logger;
        private readonly string _actionName;
        private readonly SabreMapper SabreMapper = new SabreMapper();

        public BaseSabreApiRequest(ILogger logger, string actionName)
        {
            _logger = logger;
            _actionName = actionName;
        }

        protected abstract Task<TResponse> SabreMethodCaller
        (
            TMessageHeader header,
            TSecurity security,
            TRequest request
        );

        public async Task<TResponse> CallSabreMethod(
            Session session,
            TRequest request
        )
        {
            try
            {
                _logger.Debug($"{_actionName} started");
                var header = SabreMapper.GetMessageHeader<TMessageHeader>(session.ConversationId, _actionName, session.Organization);
                var security = SabreMapper.GetSecurity<TSecurity>(session.Token);
                var response =  await SabreMethodCaller(header, security, request);
                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug($"{_actionName} finished");
            }
        }
    }
}
