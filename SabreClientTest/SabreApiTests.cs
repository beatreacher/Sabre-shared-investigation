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
using System;

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

            var req = GetFlightScheduleRequest("LAS", "LAX", DateTime.Now.AddMonths(3).Date.ToString("s"));
            var schedule = await client.GetFlightSchedules(session, req);
            schedule.Should().NotBeNull();
            schedule.OTA_AirScheduleRS.Should().NotBeNull();
            var airScheduleRS = JsonConvert.SerializeObject(schedule.OTA_AirScheduleRS);
            _logger.Debug(airScheduleRS);
            var fsOrigin = schedule.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption[0].FlightSegment[0];

            req = GetFlightScheduleRequest("LAX", "LAS", DateTime.Now.AddMonths(6).Date.ToString("s"));
            var schedule1 = await client.GetFlightSchedules(session, req);
            var airScheduleRS1 = JsonConvert.SerializeObject(schedule1.OTA_AirScheduleRS);
            _logger.Debug(airScheduleRS1);
            var fsDest = schedule1.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption[0].FlightSegment[0];

            var closeResponse = await sessionManager.CloseSession(session);
            closeResponse.Should().Be("Approved");
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

                var jss = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                var reqSer = JsonConvert.SerializeObject(req, jss);
                _logger.Debug(reqSer);

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

            var odi = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment
            {
                DepartureDateTime = "2019-04-16T11:02:00",
                FlightNumber = "1366",
                NumberInParty = "1",
                ResBookDesigCode = "Y",
                Status = "NN",
                InstantPurchase = false,
                DestinationLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = "LAX" },
                MarketingAirline = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentMarketingAirline { Code = "AA", FlightNumber = "1366" },
                OperatingAirline = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentOperatingAirline { Code="AA"},
                OriginLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentOriginLocation { LocationCode = "LAS" }
            };

            var client = new SabreApi(_logger);
            var enhacnedReq = new Enh.EnhancedAirBookRQ
            {
                version = "3.9.0",
                HaltOnError = true,
                IgnoreOnError = true,
                OTA_AirBookRQ = new Enh.EnhancedAirBookRQOTA_AirBookRQ
                {
                    HaltOnStatus = new Enh.EnhancedAirBookRQOTA_AirBookRQHaltOnStatus[] { new Enh.EnhancedAirBookRQOTA_AirBookRQHaltOnStatus { Code = "UC" } },
                    RetryRebook = new Enh.EnhancedAirBookRQOTA_AirBookRQRetryRebook { Option = true },
                    RedisplayReservation = new Enh.EnhancedAirBookRQOTA_AirBookRQRedisplayReservation
                    {
                        NumAttempts = "5",
                        WaitInterval = "2000"
                    },
                    OriginDestinationInformation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment[]
                    {
                        odi
                    }
                },
                PostProcessing = new Enh.EnhancedAirBookRQPostProcessing
                {
                    IgnoreAfter = false,
                    RedisplayReservation = new Enh.EnhancedAirBookRQPostProcessingRedisplayReservation { WaitInterval="5000"}
                },
                PreProcessing = new Enh.EnhancedAirBookRQPreProcessing { IgnoreBefore = false, UniqueID = new Enh.EnhancedAirBookRQPreProcessingUniqueID { ID = "PCOLDH" } },
                //OTA_AirPriceRQ = new Enh.EnhancedAirBookRQOTA_AirPriceRQ[]
                //{
                //    new Enh.EnhancedAirBookRQOTA_AirPriceRQ
                //    {
                //        PriceRequestInformation = new Enh.EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformation
                //        {
                //            OptionalQualifiers=new Enh.EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiers
                //            {
                //                PricingQualifiers = new Enh.EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiers
                //                {
                //                    CurrencyCode = "USD",
                //                    PassengerType = new Enh.EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType[]
                //                    {
                //                        new Enh.EnhancedAirBookRQOTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType
                //                        {
                //                            Code="ADT",
                //                            Quantity="1",
                //                            Force=true
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            };
            var schedule = await client.GetEnhancedAirBook(session, enhacnedReq);
            schedule.Should().NotBeNull();

            var resp = JsonConvert.SerializeObject(schedule.EnhancedAirBookRS);
            _logger.Debug(resp);

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

        private static AirSchedService.OTA_AirScheduleRQ GetFlightScheduleRequest(string originLocation, string destinationLocation, string departureDateTime)
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

        private static BFM.OTA_AirLowFareSearchRQ GetBargainRequest()
        {

            //var o1 = GetBFMOriginDestination(options[0].FlightSegment[0], "2019-03-09T00:00:00");
            //var o2 = GetBFMOriginDestination(options[1].FlightSegment[0], "2019-03-14T00:00:00");
            //var odis = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation[2] { o1, o2 };

            var odi1 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = "2019-04-16T13:45:00",
                DepartureWindow = "11001500",
                RPH = "1",
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "LAS" },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "LAX" },
                //TPA_Extensions = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                //{
                //    SegmentType = new BFM.ExchangeOriginDestinationInformationTypeSegmentType
                //    {
                //        Code = BFM.ExchangeOriginDestinationInformationTypeSegmentTypeCode.X,
                //        CodeSpecified = true
                //    },
                //    IncludeVendorPref = new BFM.IncludeVendorPrefType[]
                //    {
                //        new BFM.IncludeVendorPrefType
                //        {
                //            Code = "B6"
                //        }
                //    }
                //}
            };
            var odis = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation[] { odi1 };

            var travelPreferences = new BFM.AirSearchPrefsType
            {
                TPA_Extensions = new BFM.AirSearchPrefsTypeTPA_Extensions
                {
                    TripType = new BFM.AirSearchPrefsTypeTPA_ExtensionsTripType { Value = BFM.AirTripType.OneWay }
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
                                Quantity = "1",
                                Changeable=false
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
                NumberInParty = "1",
                ResBookDesigCode = fs.ResBookDesigCode,
                Status = "NN",
                DestinationLocation = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = fs.ArrivalAirport.LocationCode },
                OriginLocation = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentOriginLocation { LocationCode = fs.DepartureAirport.LocationCode },
                Equipment = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentEquipment { AirEquipType = fs.Equipment[0].AirEquipType },
                MarketingAirline = new SabreApiClient.OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentMarketingAirline
                {
                    Code = fs.MarketingAirline.Code,
                    FlightNumber = fs.OperatingAirline.FlightNumber
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
