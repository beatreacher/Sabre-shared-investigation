using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class FlightSchedulesRequest : BaseSabreApiRequest<
        OTA_AirScheduleService.OTA_AirScheduleRQResponse,
        OTA_AirScheduleService.MessageHeader,
        OTA_AirScheduleService.Security1,
        OTA_AirScheduleService.OTA_AirScheduleRQ>
    {
        public FlightSchedulesRequest(ILogger logger) : base(logger, "OTA_AirScheduleLLSRQ"){}

        protected override async Task<OTA_AirScheduleService.OTA_AirScheduleRQResponse> SabreMethodCaller
        (
            OTA_AirScheduleService.MessageHeader header,
            OTA_AirScheduleService.Security1 security,
            OTA_AirScheduleService.OTA_AirScheduleRQ request
        )
        {
            var proxy = new OTA_AirScheduleService.OTA_AirSchedulePortTypeClient("OTA_AirSchedulePortType");
            var response = await proxy.OTA_AirScheduleRQAsync(header, security, request);

            return response;
        }
    }
}
