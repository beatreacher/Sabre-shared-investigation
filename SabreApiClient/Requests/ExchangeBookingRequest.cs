using System;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace SabreApiClient.Requests
{
    public class ExchangeBookingRequest : BaseSabreApiRequest<
        ExchangeBookingRQ.ExchangeBookingRQResponse,
        ExchangeBookingRQ.MessageHeader,
        ExchangeBookingRQ.Security,
        ExchangeBookingRQ.ExchangeBookingRQ>
    {
        public ExchangeBookingRequest(ILogger logger) : base(logger, "ExchangeBookingRQ") { }

        protected override async Task<ExchangeBookingRQ.ExchangeBookingRQResponse> SabreMethodCaller
        (
            ExchangeBookingRQ.MessageHeader header,
            ExchangeBookingRQ.Security security,
            ExchangeBookingRQ.ExchangeBookingRQ request
        )
        {
            var proxy = new ExchangeBookingRQ.ExchangeBookingPortTypeClient("ExchangeBookingPortType");
            return await proxy.ExchangeBookingRQAsync(header, security, request);
        }
    }
}
