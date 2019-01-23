using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;

using BFM = SabreApiClient.BargainFinderMax;
using Domain.Models;
using System.IO;
using System.Diagnostics;

namespace SabreClientTest
{
    [TestClass]
    public class BargainFinderMaxTests
    {
        ILogger _logger;
        SabreApi _client;
        SessionManager _sessionManager;
        public static Session CurrentSession;

        public BargainFinderMaxTests()
        {
            _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());
            _sessionManager = new SessionManager(_logger);
            _client = new SabreApi(_logger);
        }

        [TestMethod]
        public async Task GetBargainFinderMaxTest()
        {
            try
            {
                CurrentSession = await _sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");
                var bargainFinderMax = await GetBargainFinderMax(CurrentSession,
                    "JFK", "LAS", "2019-02-15T00:00:00",
                    "LAS", "JFK", "2019-02-27T00:00:00",
                    "50ITINS", BFM.AirTripType.OpenJaw);

                //bargainFinderMax.Should().NotBeNull();
                //bargainFinderMax.OTA_AirLowFareSearchRS.Should().NotBeNull();

                //var bfmAirLowFareSearchRS = JsonConvert.SerializeObject(bargainFinderMax.OTA_AirLowFareSearchRS, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var bfmAirLowFareSearchRS = JsonConvert.SerializeObject(bargainFinderMax.OTA_AirLowFareSearchRS, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                _logger.Debug(bfmAirLowFareSearchRS);

                //foreach (var item in bargainFinderMax.OTA_AirLowFareSearchRS.Items)
                //{
                //    item.Should().NotBeOfType<BFM.ErrorsType>();
                //}

                var response = await _sessionManager.CloseSession(CurrentSession);
                response.Should().Be("Approved");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<BFM.BargainFinderMaxRQResponse> GetBargainFinderMax
        (
            Session session,
            string originLocation1,
            string destinationLocation1,
            string departureDateTime1,
            string originLocation2,
            string destinationLocation2,
            string departureDateTime2,
            string itemsCount,
            BFM.AirTripType tripType
        )
        {
            var req = GetBargainRequest(
                    originLocation1,
                    destinationLocation1,
                    departureDateTime1,
                    originLocation2,
                    destinationLocation2,
                    departureDateTime2,
                    itemsCount,
                    tripType);

            //var bfmReq = JsonConvert.SerializeObject(req, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            //File.WriteAllText("bfmReq.txt", bfmReq);
            //Process.Start("bfmReq.txt");

            var bargainFinderMax = await _client.GetBargainFinderMax(session, req);

            //var bfmAirLowFareSearchRS = JsonConvert.SerializeObject(bargainFinderMax.OTA_AirLowFareSearchRS, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            //_logger.Debug(bfmAirLowFareSearchRS);

            return bargainFinderMax;
        }

        private static BFM.OTA_AirLowFareSearchRQ GetBargainRequest
        (
            string originLocation1,
            string destinationLocation1,
            string departureDateTime1,
            string originLocation2,
            string destinationLocation2,
            string departureDateTime2,
            string itemsCount,
            BFM.AirTripType tripType
        )
        {
            var odi1 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = departureDateTime1,
                RPH = "1",
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = originLocation1 },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = destinationLocation1 },
            };
            var odi2 = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation
            {
                Item = departureDateTime2,
                RPH = "2",
                OriginLocation = new BFM.OriginDestinationInformationTypeOriginLocation { LocationCode = originLocation2 },
                DestinationLocation = new BFM.OriginDestinationInformationTypeDestinationLocation { LocationCode = destinationLocation2 },
            };

            var odis = new BFM.OTA_AirLowFareSearchRQOriginDestinationInformation[] { odi1, odi2 };

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
                    OnlineIndicator = new BFM.AirSearchPrefsTypeTPA_ExtensionsOnlineIndicator
                    {
                        Ind = true
                    },
                    TripType = new BFM.AirSearchPrefsTypeTPA_ExtensionsTripType
                    {
                        Value = tripType,
                        ValueSpecified = true
                    }
                }
            };

            var travelerInfoSummary = new BFM.TravelerInfoSummaryType
            {
                SeatsRequested = new string[] { "2" },
                AirTravelerAvail = new BFM.TravelerInformationType[]
                {
                    new BFM.TravelerInformationType
                    {
                        PassengerTypeQuantity = new BFM.PassengerTypeQuantityType[]
                        {
                            new BFM.PassengerTypeQuantityType
                            {
                                Code = "ADT",
                                Quantity = "2",
                                Changeable = true
                            }
                        }
                    }
                }
            };

            var req = new BFM.OTA_AirLowFareSearchRQ
            {
                Version = "4.3.0",
                AvailableFlightsOnly = true,
                DirectFlightsOnly = true, //
                TPA_Extensions = new BFM.OTA_AirLowFareSearchRQTPA_Extensions
                {
                    IntelliSellTransaction = new BFM.TransactionType
                    {
                        RequestType = new BFM.TransactionTypeRequestType { Name = itemsCount }
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
    }
}
