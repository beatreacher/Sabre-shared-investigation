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
    }
}
