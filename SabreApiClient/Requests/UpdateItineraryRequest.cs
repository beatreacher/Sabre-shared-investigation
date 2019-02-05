using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class UpdateItineraryRequest : BaseSabreApiRequest<
        UpdateReservationRQ.UpdateReservationOperationResponse,
        UpdateReservationRQ.MessageHeader,
        UpdateReservationRQ.Security,
        UpdateReservationRQ.UpdateReservationRQ>
    {
        public UpdateItineraryRequest(ILogger logger) : base(logger, "UpdateReservationRQ") { }

        protected override async Task<UpdateReservationRQ.UpdateReservationOperationResponse> SabreMethodCaller
        (
            UpdateReservationRQ.MessageHeader header,
            UpdateReservationRQ.Security security,
            UpdateReservationRQ.UpdateReservationRQ request
        )
        {
            var proxy = new UpdateReservationRQ.UpdateReservationPortTypeClient("UpdateReservationPortType");
            var response = await proxy.UpdateReservationOperationAsync(header, security, request);
            return response;
        }
    }
}
