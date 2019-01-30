using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class EnhancedAirBookRequest : BaseSabreApiRequest<
        EnhancedAirBookRQ.EnhancedAirBookRQResponse,
        EnhancedAirBookRQ.MessageHeader,
        EnhancedAirBookRQ.Security,
        EnhancedAirBookRQ.EnhancedAirBookRQ>
    {
        public EnhancedAirBookRequest(ILogger logger) : base(logger, "EnhancedAirBookRQ") { }

        protected override async Task<EnhancedAirBookRQ.EnhancedAirBookRQResponse> SabreMethodCaller
        (
            EnhancedAirBookRQ.MessageHeader header,
            EnhancedAirBookRQ.Security security,
            EnhancedAirBookRQ.EnhancedAirBookRQ request
        )
        {
            var proxy = new EnhancedAirBookRQ.EnhancedAirBookPortTypeClient("EnhancedAirBookPortType");
            var response = await proxy.EnhancedAirBookRQAsync(header, security, request);

            return response;
        }
    }
}
