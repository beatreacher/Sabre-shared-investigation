using System.Configuration;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Domain.Models;
using SabreApiClient;

namespace SabreClientTest
{

    [TestClass]
    public class SabreApiTests
    {
        Credentials _credentials = new Credentials(
                ConfigurationManager.AppSettings["UserName"],
                ConfigurationManager.AppSettings["Password"],
                ConfigurationManager.AppSettings["Organization"],
                ConfigurationManager.AppSettings["Domain"]
                );

        [TestMethod]
        public async Task GetToken()
        {
            var sessionManager = new SessionManager();
            var session = await sessionManager.CreateSession(_credentials, "TokenCreateRQ");

            session.Should().NotBeNull();
            session.ConversationId.Should().NotBeNull();
            session.Token.Should().NotBeNull();
            session.MessageId.Should().NotBeNull();
            session.TimeStamp.Should().NotBeNull();
        }

        [TestMethod]
        public async Task CreateCloseSessionTest()
        {
            var client = new SessionManager();
            var session = await client.CreateSession(_credentials, "SessionCreateRQ");

            session.Should().NotBeNull();
            session.ConversationId.Should().NotBeNull();
            session.Token.Should().NotBeNull();
            session.MessageId.Should().NotBeNull();
            session.TimeStamp.Should().NotBeNull();

            var response = await client.CloseSession(session);
            response.Should().NotBeNull();
            response.Should().Be("Approved");
        }

        [TestMethod]
        public async Task GetFlightSchedulesTest()
        {
            var sessionManager = new SessionManager();
            var session = await sessionManager.CreateSession(_credentials, "SessionCreateRQ");

            var client = new SabreApi();
            var schedule = await client.GetFlightSchedules(session, GetFlightScheduleRequest());
            schedule.Should().NotBeNull();
            schedule.OTA_AirScheduleRS.Should().NotBeNull();

            var response = await sessionManager.CloseSession(session);
            response.Should().Be("Approved");
        }

        [TestMethod]
        public async Task GetBargainFinderMaxTest()
        {
            try
            {
                var sessionManager = new SessionManager();
                var session = await sessionManager.CreateSession(_credentials, "SessionCreateRQ");

                var client = new SabreApi();

                var req = GetBargainRequest();

                var bargainFinderMax = await client.GetBargainFinderMax(session, req);
                bargainFinderMax.Should().NotBeNull();
                bargainFinderMax.OTA_AirLowFareSearchRS.Should().NotBeNull();

                var response = await sessionManager.CloseSession(session);
                response.Should().Be("Approved");
            }
            catch (System.Exception e)
            {
                throw;
            }
            
        }

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

        private static SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQ GetBargainRequest()
        {
            var odi1 = new SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = "2016-09-09T00:00:00",
                RPH = "1",
                OriginLocation = new SabreApiClient.BargainFinderMax.OriginDestinationInformationTypeOriginLocation { LocationCode = "SLC" },
                DestinationLocation = new SabreApiClient.BargainFinderMax.OriginDestinationInformationTypeDestinationLocation { LocationCode = "LAX" },
                TPA_Extensions = new SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                {
                    SegmentType = new SabreApiClient.BargainFinderMax.ExchangeOriginDestinationInformationTypeSegmentType { Code = SabreApiClient.BargainFinderMax.ExchangeOriginDestinationInformationTypeSegmentTypeCode.O }
                }
            };
            var odi2 = new SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = "2016-09-14T00:00:00",
                RPH = "2",
                OriginLocation = new SabreApiClient.BargainFinderMax.OriginDestinationInformationTypeOriginLocation { LocationCode = "SFO" },
                DestinationLocation = new SabreApiClient.BargainFinderMax.OriginDestinationInformationTypeDestinationLocation { LocationCode = "LAS" },
                TPA_Extensions = new SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                {
                    SegmentType = new SabreApiClient.BargainFinderMax.ExchangeOriginDestinationInformationTypeSegmentType { Code = SabreApiClient.BargainFinderMax.ExchangeOriginDestinationInformationTypeSegmentTypeCode.O }
                }
            };
            var odis = new SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQOriginDestinationInformation[2] { odi1, odi2 };

            var travelPreferences = new SabreApiClient.BargainFinderMax.AirSearchPrefsType
            {
                CabinPref = new SabreApiClient.BargainFinderMax.CabinPrefType[1]
                {
                    new SabreApiClient.BargainFinderMax.CabinPrefType
                    {
                        Cabin = SabreApiClient.BargainFinderMax.CabinType.Y,
                        PreferLevel = SabreApiClient.BargainFinderMax.PreferLevelType.Preferred
                    }
                },
                TPA_Extensions = new SabreApiClient.BargainFinderMax.AirSearchPrefsTypeTPA_Extensions
                {
                    TripType = new SabreApiClient.BargainFinderMax.AirSearchPrefsTypeTPA_ExtensionsTripType { Value = SabreApiClient.BargainFinderMax.AirTripType.OpenJaw }
                }
            };

            var travelerInfoSummary = new SabreApiClient.BargainFinderMax.TravelerInfoSummaryType
            {
                SeatsRequested = new string[] { "1" },
                AirTravelerAvail = new SabreApiClient.BargainFinderMax.TravelerInformationType[]
                {
                    new SabreApiClient.BargainFinderMax.TravelerInformationType
                    {
                        PassengerTypeQuantity = new SabreApiClient.BargainFinderMax.PassengerTypeQuantityType[]
                        {
                            new SabreApiClient.BargainFinderMax.PassengerTypeQuantityType { Code = "ADT", Quantity = "1" }
                        }
                    }
                }
            };

            var req = new SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQ
            {
                TPA_Extensions = new SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQTPA_Extensions
                {
                    IntelliSellTransaction = new SabreApiClient.BargainFinderMax.TransactionType
                    {
                        RequestType = new SabreApiClient.BargainFinderMax.TransactionTypeRequestType { Name = "50ITINS" }
                    }
                },
                POS = new SabreApiClient.BargainFinderMax.SourceType[1]
                {
                    new SabreApiClient.BargainFinderMax.SourceType
                    {
                        RequestorID = new SabreApiClient.BargainFinderMax.UniqueID_Type
                        {
                            CompanyName = new SabreApiClient.BargainFinderMax.CompanyNameType { Code = "TN", Value = "TN" }
                        },
                        PseudoCityCode ="PCC"
                    }
                },
                OriginDestinationInformation = odis,
                TravelPreferences = travelPreferences,
                TravelerInfoSummary = travelerInfoSummary,
            };

            return req;
        }
        
    }
}
