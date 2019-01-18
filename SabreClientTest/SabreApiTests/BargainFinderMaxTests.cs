using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;

using BFM = SabreApiClient.BargainFinderMax;

namespace SabreClientTest
{
    [TestClass]
    public class BargainFinderMaxTests
    {
        ILogger _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());

        [TestMethod]
        public async Task GetBargainFinderMaxTest()
        {
            try
            {
                var sessionManager = new SessionManager(_logger);
                var session = await sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

                var client = new SabreApi(_logger);

                //var schedule = await client.GetFlightSchedules(session, GetFlightScheduleRequest());
                //schedule.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption[0].FlightSegment.Length.Should().BeGreaterThan(1);
                //AirSchedService.OTA_AirScheduleRSOriginDestinationOptionsOriginDestinationOption[] options = 
                //    schedule.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption;
                //AirSchedService.OTA_AirScheduleRSOriginDestinationOptionsOriginDestinationOptionFlightSegment[] segments = 
                //    schedule.OTA_AirScheduleRS.OriginDestinationOptions.OriginDestinationOption[0].FlightSegment;

                var req = GetBargainRequest();

                //var jss = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                //var reqSer = JsonConvert.SerializeObject(req, jss);
                //_logger.Debug(reqSer);

                var bargainFinderMax = await client.GetBargainFinderMax(session, req);

                //bargainFinderMax.Should().NotBeNull();
                //bargainFinderMax.OTA_AirLowFareSearchRS.Should().NotBeNull();

                var bfmAirLowFareSearchRS = JsonConvert.SerializeObject(bargainFinderMax.OTA_AirLowFareSearchRS);
                _logger.Debug(bfmAirLowFareSearchRS);

                //var its = ((BFM.OTA_AirLowFareSearchRSPricedItineraries)bargainFinderMax.OTA_AirLowFareSearchRS.Items.FirstOrDefault(s => s is BFM.OTA_AirLowFareSearchRSPricedItineraries)).PricedItinerary.First();

                //foreach (var item in bargainFinderMax.OTA_AirLowFareSearchRS.Items)
                //{
                //    item.Should().NotBeOfType<BFM.ErrorsType>();
                //}
                //var airReq = GetAirBookRequest(its);
                //var schedule = await client.GetAirBook(session, airReq);

                var response = await sessionManager.CloseSession(session);
                response.Should().Be("Approved");
            }
            catch (System.Exception e)
            {
                throw;
            }
        }


        private static BFM.OTA_AirLowFareSearchRQ GetBargainRequest()
        {
            var odi1 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                //Item = "2019-03-09T00:00:00",
                Item = "2019-04-16T12:08:00",
                RPH = "1",
                //OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "TPE" },
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "LAS" },
                //DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "HKG" },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "JFK" },
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
                            Code = "KA"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "AA"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "CA"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "BR"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "CX"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "B6"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "HA"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "DL"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "KE"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "VS"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "BA"
                        }
                    },
                    Baggage = new BFM.BaggageType { FreePieceRequired = true }
                }
            };
            var odi2 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = "2019-07-16T11:35:00",
                RPH = "2",
                //OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "HKG" },
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "JFK" },
                //DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "EWR" },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "LAS" },
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
                            Code = "KA"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "AA"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "CA"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "BR"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "CX"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "B6"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "HA"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "DL"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "KE"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "VS"
                        },
                        new BFM.IncludeVendorPrefType
                        {
                            Code = "BA"
                        }
                    },
                    Baggage = new BFM.BaggageType { FreePieceRequired = true }
                }
            };

            //var odis = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation[] { odi1, odi2 };
            var odis = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation[] { odi1 };

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
                    //TripType = new BFM.AirSearchPrefsTypeTPA_ExtensionsTripType { Value = BFM.AirTripType.OpenJaw }
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


            /*var o1 = GetBFMOriginDestination(options[0].FlightSegment[0], "2019-03-09T00:00:00");
            var o2 = GetBFMOriginDestination(options[1].FlightSegment[0], "2019-03-14T00:00:00");
            var odis = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation[2] { o1, o2 };*/

            /*var odi1 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = "2019-04-16T12:08:00",
                RPH = "1",
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "LAS" },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "JFK" },
                TPA_Extensions = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                {
                    SegmentType = new BFM.ExchangeOriginDestinationInformationTypeSegmentType
                    {
                        Code = BFM.ExchangeOriginDestinationInformationTypeSegmentTypeCode.X,
                        CodeSpecified = true
                    },
                    //IncludeVendorPref = new BFM.IncludeVendorPrefType[]
                    //{
                    //    new BFM.IncludeVendorPrefType
                    //    {
                    //        Code = "B6"
                    //    }
                    //},
                    Baggage = new BFM.BaggageType { FreePieceRequired = true }
                }
            };
            var odi2 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = "2019-07-16T11:35:00",
                RPH = "2",
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = "JFK" },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = "LAS" },
                TPA_Extensions = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformationTPA_Extensions
                {
                    SegmentType = new BFM.ExchangeOriginDestinationInformationTypeSegmentType
                    {
                        Code = BFM.ExchangeOriginDestinationInformationTypeSegmentTypeCode.O,
                        CodeSpecified = true
                    },
                    //IncludeVendorPref = new BFM.IncludeVendorPrefType[]
                    //{
                    //    new BFM.IncludeVendorPrefType
                    //    {
                    //        Code = "AA"
                    //    }
                    //},
                    Baggage = new BFM.BaggageType { FreePieceRequired = true }
                }
            };*/

        }
    }
}
