using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class CreatePnrRequest : BaseSabreApiRequest<
        CreatePNR.PassengerDetailsRQResponse,
        CreatePNR.MessageHeader,
        CreatePNR.Security,
        CreatePNR.PassengerDetailsRQ>
    {
        public CreatePnrRequest(ILogger logger) : base(logger, "PassengerDetailsRQ") { }

        protected override async Task<CreatePNR.PassengerDetailsRQResponse> SabreMethodCaller
        (
            CreatePNR.MessageHeader header,
            CreatePNR.Security security,
            CreatePNR.PassengerDetailsRQ request
        )
        {
            var proxy = new CreatePNR.PassengerDetailsPortTypeClient("PassengerDetailsPortType");
            return await proxy.PassengerDetailsRQAsync(header, security, request);
        }
    }
}
