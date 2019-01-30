﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;

using BFM = SabreApiClient.BargainFinderMax;
using Domain.Models;
using System.Collections.Generic;

namespace SabreClientTest
{
    [TestClass]
    public class SabreApiTests
    {
        ILogger _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());
        Session CurrentSession;// = PNRTests.CurrentSession;

        [TestMethod]
        public async Task WorkflowTest()
        {
            var sessionManager = new SessionManager(_logger);
            var CurrentSession = await sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            string airlineCode = "DL";
            string flightNumber = "1800";
            string originLocation = "LAS"; // "DFW"???
            string destinationLocation = "JFK";
            string resBookDesigCode = "E";

            string departureDateTime = "2019-04-15T16:55:00"; //"02-27"
            string pnrDepartureDateTime = "04-15";


            var pnrTests = new PNRTests();
            //var pnrResponse = await pnrTests.CreatePNR(CurrentSession, null, originLocation, "02-27", airlineCode);
            var pnrResponse = await pnrTests.CreatePNR(CurrentSession, null, originLocation, pnrDepartureDateTime, airlineCode);

            string pnrId = pnrResponse.PassengerDetailsRS.ItineraryRef.ID;

            var e = new EnhancedAirBookTests();
            var enhResp = await e.CreateEnhanced(
                CurrentSession, 
                pnrId,
                new List<FlightDescription>
                {
                    new FlightDescription { OriginLocation = originLocation, DestinationLocation = destinationLocation, DepartureDateTime = departureDateTime,
                        MarketingAirline = airlineCode, FlightNumber=flightNumber, Status="NN", ResBookDesigCode=resBookDesigCode, NumberInParty = "1",
                        InstantPurchase = false }
                    
                });

            var EndTransaction = await pnrTests.EndTransaction(CurrentSession);

            var updatedPnrResponse = await pnrTests.CreatePNR(CurrentSession, pnrId, originLocation, pnrDepartureDateTime, airlineCode);

            var loadPnrResp = await pnrTests.LoadPNR(CurrentSession, pnrId);

            var closeResp = await sessionManager.CloseSession(CurrentSession);
        }


        [TestMethod]
        public async Task AirBookLLSRQTest()
        {
            var sessionManager = new SessionManager(_logger);
            var session = await sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            var client = new SabreApi(_logger);
            var schedule = await client.BookAirSegment(session, GetAirBookRequest());
            schedule.Should().NotBeNull();
            schedule.OTA_AirBookRS.Should().NotBeNull();

            var response = await sessionManager.CloseSession(session);
            response.Should().Be("Approved");
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
