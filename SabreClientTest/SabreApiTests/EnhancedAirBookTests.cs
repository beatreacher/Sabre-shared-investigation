using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;
using Enh = SabreApiClient.EnhancedAirBookRQ;
using Domain.Models;

namespace SabreClientTest
{
    [TestClass]
    public class EnhancedAirBookTests
    {
        ILogger _logger;
        SabreApi _client;
        SessionManager _sessionManager;
        public static Session CurrentSession;

        public EnhancedAirBookTests()
        {
            _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());
            _sessionManager = new SessionManager(_logger);
            _client = new SabreApi(_logger);
        }


        [TestMethod]
        public async Task EnhancedAirBookTest()
        {
            //var sessionManager = new SessionManager(_logger);
            //var session = await sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            await CreateEnhanced(CurrentSession, "ULJUAA", "1800", "LAS", "JFK", "DL", "2019-04-15T16:55:00", "E");

            //var response = await sessionManager.CloseSession(session);
            //response.Should().Be("Approved");
        }

        public async Task<Enh.EnhancedAirBookRQResponse> CreateEnhanced
        (
            Session session, 
            string pnr, 
            string flightNumber, 
            string originLocation,
            string destinationLocation, 
            string marketingAirlineCode, 
            string departureDateTime, 
            string resBookDesigCode
        )
        {
            var odi = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment
            {
                DepartureDateTime = departureDateTime,
                FlightNumber = flightNumber,
                NumberInParty = "1",
                ResBookDesigCode = resBookDesigCode,
                Status = "NN",
                InstantPurchase = false,
                DestinationLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = destinationLocation },
                MarketingAirline = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentMarketingAirline { Code = marketingAirlineCode, FlightNumber = flightNumber },
                OriginLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentOriginLocation { LocationCode = originLocation }
            };

            var enhacnedReq = new Enh.EnhancedAirBookRQ
            {
                version = "3.9.0",
                HaltOnError = true,
                IgnoreOnError = true,
                OTA_AirBookRQ = new Enh.EnhancedAirBookRQOTA_AirBookRQ
                {
                    HaltOnStatus = new Enh.EnhancedAirBookRQOTA_AirBookRQHaltOnStatus[]
                    {
                        new Enh.EnhancedAirBookRQOTA_AirBookRQHaltOnStatus
                        {
                            Code = "UC"
                        }
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
                    UniqueID = new Enh.EnhancedAirBookRQPreProcessingUniqueID { ID = pnr }
                }
                //PreProcessing = new Enh.EnhancedAirBookRQPreProcessing { IgnoreBefore = true/*, UniqueID = new Enh.EnhancedAirBookRQPreProcessingUniqueID { ID = "JEGYLT" } */}
            };
            var schedule = await _client.GetEnhancedAirBook(session, enhacnedReq);
            schedule.Should().NotBeNull();

            var resp = JsonConvert.SerializeObject(schedule.EnhancedAirBookRS);
            _logger.Debug(resp);

            return schedule;
        }
    }
}
