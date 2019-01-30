using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class BargainFinderMaxRequest : BaseSabreApiRequest<
        BargainFinderMax.BargainFinderMaxRQResponse,
        BargainFinderMax.MessageHeader,
        BargainFinderMax.Security,
        BargainFinderMax.OTA_AirLowFareSearchRQ
        >
    {
        public BargainFinderMaxRequest(ILogger logger) : base(logger, "BargainFinderMaxRQ") {}

        protected override async Task<BargainFinderMax.BargainFinderMaxRQResponse> SabreMethodCaller
        (
            BargainFinderMax.MessageHeader header,
            BargainFinderMax.Security security,
            BargainFinderMax.OTA_AirLowFareSearchRQ request
        )
        {
            var proxy = new BargainFinderMax.BargainFinderMaxPortTypeClient("BargainFinderMaxPortType");
            return await proxy.BargainFinderMaxRQAsync(header, security, request);
        }

    }
}
