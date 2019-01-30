using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class RetrieveItineraryRequest : BaseSabreApiRequest<
        GetReservationRQ.GetReservationOperationResponse,
        GetReservationRQ.MessageHeader,
        GetReservationRQ.Security,
        GetReservationRQ.GetReservationRQ>
    {
        public RetrieveItineraryRequest(ILogger logger) : base(logger, "GetReservationRQ") { }

        protected override async Task<GetReservationRQ.GetReservationOperationResponse> SabreMethodCaller
        (
            GetReservationRQ.MessageHeader header,
            GetReservationRQ.Security security,
            GetReservationRQ.GetReservationRQ request
        )
        {
            var proxy = new GetReservationRQ.GetReservationPortTypeClient("GetReservationPortType");
            return await proxy.GetReservationOperationAsync(header, security, request);
        }
    }
}

