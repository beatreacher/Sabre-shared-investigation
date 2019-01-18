using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;
using Enh = SabreApiClient.EnhancedAirBookRQ;

namespace SabreClientTest
{
    [TestClass]
    public class EnhancedAirBookTests
    {
        ILogger _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());

        [TestMethod]
        public async Task EnhancedAirBookTest()
        {
            {
                //var odi = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment
                //{
                //    DepartureDateTime = "2019-02-21T09:30",
                //    FlightNumber = "1815",
                //    NumberInParty = "1",
                //    ResBookDesigCode = "Y",
                //    Status = "NN",
                //    InstantPurchase = false,
                //    DestinationLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = "JFK" },
                //    MarketingAirline = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentMarketingAirline { Code = "DL", FlightNumber = "1815" },
                //    OriginLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentOriginLocation { LocationCode = "LAS" }
                //};
            }

            var sessionManager = new SessionManager(_logger);
            var session = await sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            //var odi = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment
            //{
            //    DepartureDateTime = "2019-03-09T14:50:00",
            //    FlightNumber = "467",
            //    NumberInParty = "1",
            //    ResBookDesigCode = "Y",
            //    Status = "NN",
            //    InstantPurchase = false,
            //    DestinationLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = "HKG" },
            //    MarketingAirline = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentMarketingAirline { Code = "CX", FlightNumber = "467" },
            //    OriginLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentOriginLocation { LocationCode = "TPE" }
            //};

            var odi = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment
            {
                DepartureDateTime = "2019-04-15T16:55:00",
                FlightNumber = "1800",
                NumberInParty = "1",
                ResBookDesigCode = "E",
                Status = "NN",
                InstantPurchase = false,
                DestinationLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = "JFK" },
                MarketingAirline = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentMarketingAirline { Code = "DL", FlightNumber = "1800" },
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
                    HaltOnStatus = new Enh.EnhancedAirBookRQOTA_AirBookRQHaltOnStatus[] 
                    {
                        new Enh.EnhancedAirBookRQOTA_AirBookRQHaltOnStatus { Code = "UC" }
                    },
                    RetryRebook = new Enh.EnhancedAirBookRQOTA_AirBookRQRetryRebook { Option = true },
                    RedisplayReservation = new Enh.EnhancedAirBookRQOTA_AirBookRQRedisplayReservation
                    {
                        NumAttempts = "5",
                        WaitInterval = "2000"
                    },
                    OriginDestinationInformation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment[1]
                    {
                        odi
                    }
                },
                PostProcessing = new Enh.EnhancedAirBookRQPostProcessing
                {
                    IgnoreAfter = false,
                    RedisplayReservation = new Enh.EnhancedAirBookRQPostProcessingRedisplayReservation
                    {
                        UnmaskCreditCard = true,
                        WaitInterval = "5000"
                    }
                },
                PreProcessing = new Enh.EnhancedAirBookRQPreProcessing
                {
                    IgnoreBefore = false,
                    UniqueID = new Enh.EnhancedAirBookRQPreProcessingUniqueID { ID = "PMVALJ" }
                }
                //PreProcessing = new Enh.EnhancedAirBookRQPreProcessing { IgnoreBefore = true/*, UniqueID = new Enh.EnhancedAirBookRQPreProcessingUniqueID { ID = "JEGYLT" } */}
            };
            var schedule = await client.GetEnhancedAirBook(session, enhacnedReq);
            schedule.Should().NotBeNull();

            var resp = JsonConvert.SerializeObject(schedule.EnhancedAirBookRS);
            _logger.Debug(resp);

            //var response = await sessionManager.CloseSession(session);
            //response.Should().Be("Approved");
        }
    }
}
