using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class BookAirSegmentRequest : BaseSabreApiRequest<
        OTA_AirBookLLSRQ.OTA_AirBookRQResponse,
        OTA_AirBookLLSRQ.MessageHeader,
        OTA_AirBookLLSRQ.Security1,
        OTA_AirBookLLSRQ.OTA_AirBookRQ>
    {
        public BookAirSegmentRequest(ILogger logger) : base(logger, "OTA_AirBookLLSRQ") { }

        protected override async Task<OTA_AirBookLLSRQ.OTA_AirBookRQResponse> SabreMethodCaller
        (
            OTA_AirBookLLSRQ.MessageHeader header,
            OTA_AirBookLLSRQ.Security1 security,
            OTA_AirBookLLSRQ.OTA_AirBookRQ request
        )
        {
            var proxy = new OTA_AirBookLLSRQ.OTA_AirBookPortTypeClient("OTA_AirBookPortType");
            var response = await proxy.OTA_AirBookRQAsync(header, security, request);

            return response;
        }
    }
}
