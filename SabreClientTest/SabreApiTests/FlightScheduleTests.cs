using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;
using AirSchedService = SabreApiClient.OTA_AirScheduleService;

namespace SabreClientTest
{
    [TestClass]
    public class FlightScheduleTests
    {
        ILogger _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());

        [TestMethod]
        public async Task GetFlightSchedulesTest()
        {
            var sessionManager = new SessionManager(_logger);
            var session = await sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            var client = new SabreApi(_logger);

            var req = GetFlightScheduleRequest("LAS", "JFK", DateTime.Now.AddMonths(3).Date.ToString("s"));

            //var req = GetFlightScheduleRequest("TPE", "HKG", "2019-03-09T00:00:00");
            var schedule = await client.GetFlightSchedules(session, req);
            schedule.Should().NotBeNull();
            schedule.OTA_AirScheduleRS.Should().NotBeNull();
            var airScheduleRS = JsonConvert.SerializeObject(schedule.OTA_AirScheduleRS);
            _logger.Debug(airScheduleRS);
            var fsOrigin = schedule.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption[0].FlightSegment[0];

            //req = GetFlightScheduleRequest("HKG", "EWR", "2019-03-14T00:00:00");
            var req1 = GetFlightScheduleRequest("JFK", "LAS", DateTime.Now.AddMonths(6).Date.ToString("s"));
            var schedule1 = await client.GetFlightSchedules(session, req1);
            var airScheduleRS1 = JsonConvert.SerializeObject(schedule1.OTA_AirScheduleRS);
            _logger.Debug(airScheduleRS1);
            var fsDest = schedule1.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption[0].FlightSegment[0];

            var closeResponse = await sessionManager.CloseSession(session);
            closeResponse.Should().Be("Approved");
        }

        public static AirSchedService.OTA_AirScheduleRQ GetFlightScheduleRequest(string originLocation, string destinationLocation, string departureDateTime)
        {
            //string originLocation = "DFW";
            //string destinationLocation = "LHR";
            //string departureDateTime = "12-21";
            //string originLocation = "LAS";
            //string destinationLocation = "JFK";
            //string departureDateTime = "2019-02-21";
            string arrivalDateTime = null;
            AirSchedService.OTA_AirScheduleRQOriginDestinationInformationFlightSegmentConnectionLocations connectionLocations = null;

            var originFlightLocation = new AirSchedService.OTA_AirScheduleRQOriginDestinationInformationFlightSegmentOriginLocation
            {
                LocationCode = originLocation
            };

            var destinationFlightLocation = new AirSchedService.OTA_AirScheduleRQOriginDestinationInformationFlightSegmentDestinationLocation
            {
                LocationCode = destinationLocation
            };

            var flightSegment = new AirSchedService.OTA_AirScheduleRQOriginDestinationInformationFlightSegment
            {
                DepartureDateTime = departureDateTime,
                DestinationLocation = destinationFlightLocation,
                OriginLocation = originFlightLocation,
                ArrivalDateTime = arrivalDateTime,
                ConnectionLocations = connectionLocations
            };

            var destinationInfo = new AirSchedService.OTA_AirScheduleRQOriginDestinationInformation
            {
                FlightSegment = flightSegment
            };

            var req = new AirSchedService.OTA_AirScheduleRQ
            {
                OriginDestinationInformation = destinationInfo
            };

            return req;
        }
    }
}
