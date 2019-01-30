using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class LoadPnrRequest : BaseSabreApiRequest<
        LoadPNR.TravelItineraryReadRQResponse,
        LoadPNR.MessageHeader,
        LoadPNR.Security1,
        LoadPNR.TravelItineraryReadRQ>
    {
        public LoadPnrRequest(ILogger logger) : base(logger, "TravelItineraryReadRQ") { }

        protected override async Task<LoadPNR.TravelItineraryReadRQResponse> SabreMethodCaller
        (
            LoadPNR.MessageHeader header,
            LoadPNR.Security1 security,
            LoadPNR.TravelItineraryReadRQ request
        )
        {
            var proxy = new LoadPNR.TravelItineraryReadPortTypeClient("TravelItineraryReadPortType");
            var response = await proxy.TravelItineraryReadRQAsync(header, security, request);
            return response;
        }
    }
}
