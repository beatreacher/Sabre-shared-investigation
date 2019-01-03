using Domain.Models;
using Newtonsoft.Json;
using SabreApiClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ApiService.Controllers
{
    public class SabreController : ApiController
    {
        private static SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQ GetFlightScheduleRequest()
        {
            string originLocation = "DFW";
            string destinationLocation = "LHR";
            string departureDateTime = "12-21";
            string arrivalDateTime = null;
            SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQOriginDestinationInformationFlightSegmentConnectionLocations connectionLocations = null;

            var originFlightLocation = new SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQOriginDestinationInformationFlightSegmentOriginLocation
            {
                LocationCode = originLocation
            };

            var destinationFlightLocation = new SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQOriginDestinationInformationFlightSegmentDestinationLocation
            {
                LocationCode = destinationLocation
            };

            var flightSegment = new SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQOriginDestinationInformationFlightSegment
            {
                DepartureDateTime = departureDateTime,
                DestinationLocation = destinationFlightLocation,
                OriginLocation = originFlightLocation,
                ArrivalDateTime = arrivalDateTime,
                ConnectionLocations = connectionLocations
            };

            var destinationInfo = new SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQOriginDestinationInformation
            {
                FlightSegment = flightSegment
            };

            var req = new SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQ
            {
                OriginDestinationInformation = destinationInfo
            };

            return req;
        }

        [HttpPost]
        [ActionName("CreateSession")]
        public async Task<Session> CreateSession(Credentials credentials)
        {
            var sessionManager = new SessionManager();
            var session = await sessionManager.CreateSession(credentials, "SessionCreateRQ");
            return session;
        }

        [HttpPost]
        [ActionName("CloseSession")]
        public async Task<string> CloseSession()
        {
            Session session = GetSession(Request);
            var sessionManager = new SessionManager();
            return await sessionManager.CloseSession(session);
        }

        [HttpPost]
        [ActionName("GetDummyFlightSchedule")]
        public async Task<SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRS> GetDummyFlightSchedule(object requestObject)
        {
            var airScheduleRequest = JsonConvert.DeserializeObject<SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQ>(requestObject.ToString());
            //var airScheduleRequest = JsonConvert.DeserializeObject<SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQ>(requestObject);

            //var json = new JavaScriptSerializer().Serialize(airScheduleRequest);
            Session session = GetSession(Request);

            var client = new SabreApi();
            var schedule = await client.GetFlightSchedules(session, airScheduleRequest);

            return schedule.OTA_AirScheduleRS;
        }

        [HttpPost]
        [ActionName("GetBargainFinderMax")]
        //public async Task<SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRS> GetBargainFinderMax(SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQ request)
        public async Task<SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRS> GetBargainFinderMax(object requestObject)
        {

            var request = JsonConvert.DeserializeObject<SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQ>(requestObject.ToString());
            Session session = GetSession(Request);

            var client = new SabreApi();
            var bargainFinderMax = await client.GetBargainFinderMax(session, request);

            return bargainFinderMax.OTA_AirLowFareSearchRS;
        }

        private static Session GetSession(HttpRequestMessage request)
        {
            if (!request.Headers.Contains("Token")
                || !request.Headers.Contains("ConversationId")
                || !request.Headers.Contains("MessageId")
                || !request.Headers.Contains("TimeStamp")
                || !request.Headers.Contains("Organization")
                )
            {
                throw new ArgumentException("Session parameters incomplete");
            }

            Session session = new Session
            (
                request.Headers.GetValues("Token").First(),
                request.Headers.GetValues("ConversationId").First(),
                request.Headers.GetValues("MessageId").First(),
                request.Headers.GetValues("TimeStamp").First(),
                request.Headers.GetValues("Organization").First()
            );

            return session;
        }
    }
}
