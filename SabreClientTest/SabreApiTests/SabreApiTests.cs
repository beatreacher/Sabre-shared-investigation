using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;

using BFM = SabreApiClient.BargainFinderMax;
using Domain.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

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

            //string airlineCode = "DL";
            //string flightNumber = "1800";
            //string originLocation = "LAS"; // "DFW"???
            //string destinationLocation = "JFK";
            //string resBookDesigCode = "E";
            //string departureDateTime = "2019-04-15T16:55:00"; //"02-27"
            //string pnrDepartureDateTime = FlightDescription.GetPnrFormattedDateTime(departureDateTime);

            var flightSegmentForward = new FlightDescription
            {
                OriginLocation = "JFK",
                DestinationLocation = "LAS",
                DepartureDateTime = "2019-02-15T16:55:00",
                MarketingAirline = "DL",
                FlightNumber = "1549",
                Status = "NN",
                ResBookDesigCode = "E",
                NumberInParty = "1",
                InstantPurchase = false
            };

            var flightSegmentBack = new FlightDescription
            {
                OriginLocation = "LAS",
                DestinationLocation = "JFK",
                DepartureDateTime = "2019-02-26T23:15:00",
                MarketingAirline = "DL",
                FlightNumber = "1694",
                Status = "NN",
                ResBookDesigCode = "E",
                NumberInParty = "1",
                InstantPurchase = false
            };

            try
            {
                var enhancedAirBook = new EnhancedAirBookTests();
                var enhResp = await enhancedAirBook.CreateEnhanced(
                    CurrentSession,
                    null,
                    //pnrId,
                    new List<FlightDescription>
                    {
                        flightSegmentForward,
                        flightSegmentBack
                    });

                var enhRespSer = JsonConvert.SerializeObject(enhResp);

                var pnrTests = new PNRTests();
                //var pnrResponse = await pnrTests.CreatePNR(CurrentSession, null, flightSegmentForward.OriginLocation, flightSegmentForward.PnrDepartureDateTime, flightSegmentForward.MarketingAirline);
                var pnrResponse = await pnrTests.CreatePNR(CurrentSession, null, flightSegmentForward.OriginLocation, "06-15", flightSegmentForward.MarketingAirline);

                var pnrResponseSer = JsonConvert.SerializeObject(pnrResponse);
                string pnrId = pnrResponse.PassengerDetailsRS.ItineraryRef.ID;

                //var EndTransaction = await pnrTests.EndTransaction(CurrentSession);
                //var updatedPnrResponse = await pnrTests.CreatePNR(CurrentSession, pnrId, originLocation, pnrDepartureDateTime, airlineCode);

                var loadPnrResp = await pnrTests.LoadPNR(CurrentSession, pnrId);
                var loadPnrRespSer = JsonConvert.SerializeObject(loadPnrResp);

                var cancelResponse = await pnrTests.CancelPnr(CurrentSession);
                var cancelResponseSer = JsonConvert.SerializeObject(cancelResponse);

                var loadPnrResp1 = await pnrTests.LoadPNR(CurrentSession, pnrId);
                var loadPnrResp1Ser = JsonConvert.SerializeObject(loadPnrResp1);
            }
            catch (Exception e)
            {
            }


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
