using System.Threading.Tasks;
using Domain.Models;

namespace SabreApiClient.Interfaces
{
    public interface ISabreApi
    {
        Task<OTA_AirScheduleService.OTA_AirScheduleRQResponse> GetFlightSchedules(
            Session session,
            OTA_AirScheduleService.OTA_AirScheduleRQ req
            );

        Task<BargainFinderMax.BargainFinderMaxRQResponse> GetBargainFinderMax(Session session, BargainFinderMax.OTA_AirLowFareSearchRQ request);

        Task<EnhancedAirBookRQ.EnhancedAirBookRQResponse> GetEnhancedAirBook(Session session, EnhancedAirBookRQ.EnhancedAirBookRQ request);

        Task<CreatePNR.PassengerDetailsRQResponse> CreatePNR(Session session, CreatePNR.PassengerDetailsRQ request);

        Task<LoadPNR.TravelItineraryReadRQResponse> LoadPNR(Session session, LoadPNR.TravelItineraryReadRQ request);

        Task<EndTransactionLLSRQ.EndTransactionRQResponse> EndTransaction(Session session, EndTransactionLLSRQ.EndTransactionRQ request);

        Task<ExchangeBookingRQ.ExchangeBookingRQResponse> ExchangeBooking(Session session, ExchangeBookingRQ.ExchangeBookingRQ request);
    }
}
