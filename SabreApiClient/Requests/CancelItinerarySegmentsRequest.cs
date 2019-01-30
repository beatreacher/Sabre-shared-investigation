using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class CancelItinerarySegmentsRequest : BaseSabreApiRequest<
        CancelItinerarySegments.OTA_CancelRQResponse,
        CancelItinerarySegments.MessageHeader,
        CancelItinerarySegments.Security1,
        CancelItinerarySegments.OTA_CancelRQ>
    {
        public CancelItinerarySegmentsRequest(ILogger logger) : base(logger, "OTA_CancelLLSRQ") { }

        protected override async Task<CancelItinerarySegments.OTA_CancelRQResponse> SabreMethodCaller
        (
            CancelItinerarySegments.MessageHeader header,
            CancelItinerarySegments.Security1 security,
            CancelItinerarySegments.OTA_CancelRQ request
        )
        {
            var proxy = new CancelItinerarySegments.OTA_CancelPortTypeClient("OTA_CancelPortType");
            return await proxy.OTA_CancelRQAsync(header, security, request);
        }
    }
}