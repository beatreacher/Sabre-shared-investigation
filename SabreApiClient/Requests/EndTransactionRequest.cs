using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class EndTransactionRequest : BaseSabreApiRequest<
        EndTransactionLLSRQ.EndTransactionRQResponse,
        EndTransactionLLSRQ.MessageHeader,
        EndTransactionLLSRQ.Security1,
        EndTransactionLLSRQ.EndTransactionRQ>
    {
        public EndTransactionRequest(ILogger logger) : base(logger, "OTA_AirScheduleLLSRQ") { }

        protected override async Task<EndTransactionLLSRQ.EndTransactionRQResponse> SabreMethodCaller
        (
            EndTransactionLLSRQ.MessageHeader header,
            EndTransactionLLSRQ.Security1 security,
            EndTransactionLLSRQ.EndTransactionRQ request
        )
        {
            var proxy = new EndTransactionLLSRQ.EndTransactionPortTypeClient("EndTransactionPortType");
            var response = await proxy.EndTransactionRQAsync(header, security, request);

            return response;
        }
    }
}
