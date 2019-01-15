using System.Configuration;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Domain.Models;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;
using System.Linq;

using BFM = SabreApiClient.BargainFinderMax;
using AirSchedService = SabreApiClient.OTA_AirScheduleService;
using Enh = SabreApiClient.EnhancedAirBookRQ;

namespace SabreClientTest
{

    [TestClass]
    public class SabreApiTests
    {
        ILogger _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());

        Credentials _credentials = new Credentials(
                ConfigurationManager.AppSettings["UserName"],
                ConfigurationManager.AppSettings["Password"],
                ConfigurationManager.AppSettings["Organization"],
                ConfigurationManager.AppSettings["Domain"]
                );

        [TestMethod]
        public async Task GetToken()
        {
            var sessionManager = new SessionManager(_logger);
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
            var client = new SessionManager(_logger);
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
            var sessionManager = new SessionManager(_logger);
            var session = await sessionManager.CreateSession(_credentials, "SessionCreateRQ");

            var client = new SabreApi(_logger);
            var req = GetFlightScheduleRequest();
            var schedule = await client.GetFlightSchedules(session, req);
            schedule.Should().NotBeNull();
            schedule.OTA_AirScheduleRS.Should().NotBeNull();

            var airScheduleRS = JsonConvert.SerializeObject(schedule.OTA_AirScheduleRS);
            _logger.Debug(airScheduleRS);

            var response = await sessionManager.CloseSession(session);
            response.Should().Be("Approved");
        }

        [TestMethod]
        public async Task GetBargainFinderMaxTest()
        {
            try
            {
                var sessionManager = new SessionManager(_logger);
                var session = await sessionManager.CreateSession(_credentials, "SessionCreateRQ");

                var client = new SabreApi(_logger);

                //var schedule = await client.GetFlightSchedules(session, GetFlightScheduleRequest());
                //schedule.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption[0].FlightSegment.Length.Should().BeGreaterThan(1);
                //AirSchedService.OTA_AirScheduleRSOriginDestinationOptionsOriginDestinationOption[] options = 
                //    schedule.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption;
                //AirSchedService.OTA_AirScheduleRSOriginDestinationOptionsOriginDestinationOptionFlightSegment[] segments = 
                //    schedule.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption[0].FlightSegment;

                var req = GetBargainRequest();
                var bargainFinderMax = await client.GetBargainFinderMax(session, req);

                bargainFinderMax.Should().NotBeNull();
                bargainFinderMax.OTA_AirLowFareSearchRS.Should().NotBeNull();

                var bfmAirLowFareSearchRS = JsonConvert.SerializeObject(bargainFinderMax.OTA_AirLowFareSearchRS);
                _logger.Debug(bfmAirLowFareSearchRS);

                var its = ((BFM.OTA_AirLowFareSearchRSPricedItineraries)bargainFinderMax.OTA_AirLowFareSearchRS.Items.FirstOrDefault(s => s is BFM.OTA_AirLowFareSearchRSPricedItineraries)).PricedItinerary.First();
                
                foreach (var item in bargainFinderMax.OTA_AirLowFareSearchRS.Items)
                {
                    item.Should().NotBeOfType<BFM.ErrorsType>();
                }

                var airReq = GetAirBookRequest(its);
                var schedule = await client.GetAirBook(session, airReq);

                var response = await sessionManager.CloseSession(session);
                response.Should().Be("Approved");
            }
            catch (System.Exception e)
            {
                throw;
            }
            
        }

        [TestMethod]
        public async Task EnhancedAirBookTest()
        {
            var sessionManager = new SessionManager(_logger);
            var session = await sessionManager.CreateSession(_credentials, "SessionCreateRQ");

            /*var item1 = new Enh.EnhancedAirBookRSTravelItineraryReadRSTravelItineraryItineraryInfoItineraryPricingPriceQuoteMiscInformationTicketingFeesFeeInformationAssociatedDataAssociatedDataItem
            {

            };*/

            var odi = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment
            {
                DepartureDateTime = "2014-06-03T12:30:00",
                FlightNumber = "1022",
                NumberInParty = "1",
                ResBookDesigCode = "F",
                Status = "NN",
                InstantPurchase = false,
                DestinationLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = "LAS" },
                MarketingAirline = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentMarketingAirline { Code = "US", FlightNumber = "1022" },
                OriginLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentOriginLocation { LocationCode = "DFW" }
            };

            var client = new SabreApi(_logger);

            var req = GetBargainRequest();
            var bargainFinderMax = await client.GetBargainFinderMax(session, req);

            var enhacnedReq = new Enh.EnhancedAirBookRQ
            {
                OTA_AirBookRQ = new Enh.EnhancedAirBookRQOTA_AirBookRQ
                {
                    OriginDestinationInformation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment[1]
                    {
                        odi
                    }
                },
                PostProcessing = new Enh.EnhancedAirBookRQPostProcessing
                {
                    IgnoreAfter = true,
                    RedisplayReservation = new Enh.EnhancedAirBookRQPostProcessingRedisplayReservation()
                },
                PreProcessing = new Enh.EnhancedAirBookRQPreProcessing { IgnoreBefore = false, UniqueID = new Enh.EnhancedAirBookRQPreProcessingUniqueID { ID = "JEGYLT" } }
            };
            var schedule = await client.GetEnhancedAirBook(session, enhacnedReq);
            schedule.Should().NotBeNull();

            var response = await sessionManager.CloseSession(session);
            response.Should().Be("Approved");
        }

        [TestMethod]
        public async Task AirBookLLSRQTest()
        {
            var sessionManager = new SessionManager(_logger);
            var session = await sessionManager.CreateSession(_credentials, "SessionCreateRQ");

            var client = new SabreApi(_logger);
            var schedule = await client.GetAirBook(session, GetAirBookRequest());
            schedule.Should().NotBeNull();
            schedule.OTA_AirBookRS.Should().NotBeNull();

            var response = await sessionManager.CloseSession(session);
            response.Should().Be("Approved");
        }

        private static AirSchedService.OTA_AirScheduleRQ GetFlightScheduleRequest()
        {
            //string originLocation = "DFW";
            //string destinationLocation = "LHR";
            //string departureDateTime = "12-21";
            string originLocation = "LAS";
            string destinationLocation = "JFK";
            string departureDateTime = "02-21";
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

        private static BFM.OTA_AirLowFareSearchRQ GetBargainRequest()
        {

            //var o1 = GetBFMOriginDestination(options[0].FlightSegment[0], "2019-03-09T00:00:00");
            //var o2 = GetBFMOriginDestination(options[1].FlightSegment[0], "2019-03-14T00:00:00");
            //var odis = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation[2] { o1, o2 };

            var odi1 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = "2019-03-09T00:00:00",
                RPH = "1",
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "TPE" },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "HKG" },
                TPA_Extensions = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                {
                    SegmentType = new BFM.ExchangeOriginDestinationInformationTypeSegmentType
                    {
                        Code = BFM.ExchangeOriginDestinationInformationTypeSegmentTypeCode.X,
                        CodeSpecified = true
                    },
                    IncludeVendorPref = new BFM.IncludeVendorPrefType[]
                    {
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "CX"
                        }
                    },
                    Baggage = new BFM.BaggageType { FreePieceRequired = true }
                }
            };
            var odi2 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = "2019-03-14T00:00:00",
                RPH = "2",
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "HKG" },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "EWR" },
                TPA_Extensions = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                {
                    SegmentType = new BFM.ExchangeOriginDestinationInformationTypeSegmentType
                    {
                        Code = BFM.ExchangeOriginDestinationInformationTypeSegmentTypeCode.O,
                        CodeSpecified = true
                    },
                    IncludeVendorPref = new BFM.IncludeVendorPrefType[]
                    {
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "CX"
                        }
                    },
                    Baggage = new BFM.BaggageType { FreePieceRequired = true }
                }
            };
            var odis = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation[2] { odi1, odi2 };

            var travelPreferences = new BFM.AirSearchPrefsType
            {
                CabinPref = new BFM.CabinPrefType[1] //
                {
                    new BFM.CabinPrefType
                    {
                        Cabin = BFM.CabinType.Y,
                        PreferLevel = BFM.PreferLevelType.Preferred
                    }
                },
                TPA_Extensions = new BFM.AirSearchPrefsTypeTPA_Extensions
                {
                    TripType = new BFM.AirSearchPrefsTypeTPA_ExtensionsTripType { Value = BFM.AirTripType.OpenJaw }
                }
            };

            var travelerInfoSummary = new BFM.TravelerInfoSummaryType
            {
                SeatsRequested = new string[] { "1" },
                AirTravelerAvail = new BFM.TravelerInformationType[]
                {
                    new BFM.TravelerInformationType
                    {
                        PassengerTypeQuantity = new BFM.PassengerTypeQuantityType[]
                        {
                            new BFM.PassengerTypeQuantityType
                            {
                                Code = "ADT",
                                Quantity = "1"
                            }
                        }
                    }
                }
            };

            var req = new BFM.OTA_AirLowFareSearchRQ
            {
                Version = "4.3.0",
                TPA_Extensions = new BFM.OTA_AirLowFareSearchRQTPA_Extensions
                {
                    IntelliSellTransaction = new BFM.TransactionType
                    {
                        RequestType = new BFM.TransactionTypeRequestType { Name = "50ITINS" }
                    }
                },
                POS = new BFM.SourceType[1]
                {
                    new BFM.SourceType
                    {
                        RequestorID = new BFM.UniqueID_Type
                        {
                            ID="1",
                            Type="1",
                            CompanyName = new BFM.CompanyNameType { Code = "TN", Value = "TN" }
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

        private static SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQ GetAirBookRequest
        (
            BFM.PricedItineraryType pit
        )
        {
            var fs = pit.AirItinerary.OriginDestinationOptions[0].FlightSegment[0];
            var api = pit.AirItineraryPricingInfo;
            var ti = pit.TicketingInfo;
            var tex = pit.TPA_Extensions;

            var o1 = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegment
            {
                DepartureDateTime = fs.DepartureDateTime,
                ArrivalDateTime = fs.ArrivalDateTime,
                FlightNumber = fs.FlightNumber,
                NumberInParty = fs.NumberInParty,
                ResBookDesigCode = fs.ResBookDesigCode,
                Status = "NN",
                DestinationLocation = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = fs.ArrivalAirport.LocationCode },
                OriginLocation = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentOriginLocation { LocationCode = fs.DepartureAirport.LocationCode },
                Equipment = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentEquipment { AirEquipType = fs.Equipment[0].AirEquipType },
                MarketingAirline = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentMarketingAirline
                {
                    Code = fs.MarketingAirline.Code,
                    FlightNumber = fs.OperatingAirline.FlightNumber //"1717"
                },
                OperatingAirline = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentOperatingAirline
                {
                    Code = fs.OperatingAirline.Code,
                }
            };

            var request = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQ
            {
                OriginDestinationInformation = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegment[] { o1 }
            };

            return request;
        }

        private static SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQ GetAirBookRequest()
        {
            var odi1 = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegment
            {
                DepartureDateTime = "2012-02-21T12:25",
                ArrivalDateTime = "2012-02-21T13:25",
                FlightNumber = "1717",
                NumberInParty = "2",
                ResBookDesigCode = "Y",
                Status = "NN",
                DestinationLocation = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentDestinationLocation
                {
                    LocationCode = "LAS"
                },
                OriginLocation = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentOriginLocation
                {
                    LocationCode = "DFW"
                },
                Equipment = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentEquipment
                {
                    AirEquipType = "757"
                },
                MarketingAirline = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentMarketingAirline
                {
                    Code = "AA",
                    FlightNumber = "1717"
                },
                OperatingAirline = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentOperatingAirline
                {
                    Code = "AA"
                }
            };

            var request = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQ
            {
                OriginDestinationInformation = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegment[] { odi1 }
            };

            return request;
        }
    }
}
