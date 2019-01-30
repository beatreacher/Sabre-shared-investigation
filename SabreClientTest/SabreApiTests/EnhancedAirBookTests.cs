using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;
using Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using Enh = SabreApiClient.EnhancedAirBookRQ;

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
            CurrentSession = await _sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            await CreateEnhanced(CurrentSession, "ULJUAA",
                new List<FlightDescription>
                {
                    new FlightDescription { OriginLocation = "JFK", DestinationLocation = "LAS", DepartureDateTime = "2019-02-15T08:30:00",
                        MarketingAirline = "DL", FlightNumber="1549", Status="NN", ResBookDesigCode="E", NumberInParty = "1", InstantPurchase = false },
                    new FlightDescription { OriginLocation = "LAS", DestinationLocation = "JFK", DepartureDateTime = "2019-02-26T23:15:00",
                        MarketingAirline = "DL", FlightNumber="1694", Status="NN", ResBookDesigCode="E", NumberInParty = "1", InstantPurchase = false}
                });

            var response = await _sessionManager.CloseSession(CurrentSession);
            response.Should().Be("Approved");
        }

        public async Task<Enh.EnhancedAirBookRQResponse> CreateEnhanced
        (
            Session session,
            string pnr,
            IList<FlightDescription> flightDescriptions
        )
        {
            var enhacnedReq = GetEnhancedRequest(pnr, flightDescriptions);
            var enhacnedReqSer = JsonConvert.SerializeObject(enhacnedReq, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            File.WriteAllText("enhacnedReqSer.txt", enhacnedReqSer);
            Process.Start("enhacnedReqSer.txt");

            var enhacned = await _client.GetEnhancedAirBook(session, enhacnedReq);
            enhacned.Should().NotBeNull();

            var resp = JsonConvert.SerializeObject(enhacned.EnhancedAirBookRS);
            _logger.Debug(resp);

            return enhacned;
        }

        private Enh.EnhancedAirBookRQ GetEnhancedRequest(string pnr, IList<FlightDescription> flightDescriptions)
        {
            var segments = flightDescriptions.Select(i => new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegment
            {
                DepartureDateTime = i.DepartureDateTime,
                FlightNumber = i.FlightNumber,
                NumberInParty = i.NumberInParty,
                ResBookDesigCode = i.ResBookDesigCode,
                Status = i.Status,
                InstantPurchase = i.InstantPurchase,
                DestinationLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentDestinationLocation { LocationCode = i.DestinationLocation },
                OriginLocation = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentOriginLocation { LocationCode = i.OriginLocation },
                MarketingAirline = new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentMarketingAirline { Code = i.MarketingAirline, FlightNumber = i.FlightNumber },
                OperatingAirline = string.IsNullOrWhiteSpace(i.OperatingAirline) ? null : new Enh.EnhancedAirBookRQOTA_AirBookRQFlightSegmentOperatingAirline { Code = i.OperatingAirline }
            }).ToArray();

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
                    OriginDestinationInformation = segments
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

            return enhacnedReq;
        }
    }
}
