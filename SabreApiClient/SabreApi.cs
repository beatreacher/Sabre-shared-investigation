using System;
using SabreApiClient.Helpers;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Domain.Models;
using SabreApiClient.Interfaces;
using Newtonsoft.Json;
using SabreApiClient.Requests;

namespace SabreApiClient
{
    public class SabreApi : ISabreApi
    {
        private readonly ILogger _logger;

        public SabreApi(ILogger logger)
        {
            _logger = logger;
        }

        private readonly SabreMapper SabreMapper = new SabreMapper();
        private readonly XmlResponseProcessor ResponseProcessor = new XmlResponseProcessor();

        public async Task<OTA_AirScheduleService.OTA_AirScheduleRQResponse> GetFlightSchedules(
            Session session,
            OTA_AirScheduleService.OTA_AirScheduleRQ request)
        {
            var req = new FlightSchedulesRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<BargainFinderMax.BargainFinderMaxRQResponse> GetBargainFinderMax(Session session, BargainFinderMax.OTA_AirLowFareSearchRQ request)
        {
            var req = new BargainFinderMaxRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<OTA_AirBookLLSRQ.OTA_AirBookRQResponse> BookAirSegment(Session session, OTA_AirBookLLSRQ.OTA_AirBookRQ request)
        {
            var req = new BookAirSegmentRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<EnhancedAirBookRQ.EnhancedAirBookRQResponse> GetEnhancedAirBook(Session session, EnhancedAirBookRQ.EnhancedAirBookRQ request)
        {
            var req = new EnhancedAirBookRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<CreatePNR.PassengerDetailsRQResponse> CreatePNR(Session session, CreatePNR.PassengerDetailsRQ request)
        {
            var req = new CreatePnrRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<LoadPNR.TravelItineraryReadRQResponse> LoadPNR(Session session, LoadPNR.TravelItineraryReadRQ request)
        {
            var req = new LoadPnrRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<EndTransactionLLSRQ.EndTransactionRQResponse> EndTransaction(Session session, EndTransactionLLSRQ.EndTransactionRQ request)
        {
            var req = new EndTransactionRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<ExchangeBookingRQ.ExchangeBookingRQResponse> ExchangeBooking(Session session, ExchangeBookingRQ.ExchangeBookingRQ request)
        {
            var req = new ExchangeBookingRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<CancelItinerarySegments.OTA_CancelRQResponse> CancelItinerarySegments(Session session, CancelItinerarySegments.OTA_CancelRQ request)
        {
            var req = new CancelItinerarySegmentsRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }

        public async Task<GetReservationRQ.GetReservationOperationResponse> RetrieveItineraryResources(Session session, GetReservationRQ.GetReservationRQ request)
        {
            var req = new RetrieveItineraryRequest(_logger);
            return await req.CallSabreMethod(session, request);
            /*
            //SabreApiClient.GetReservationRQ
                _logger.Debug("RetrieveItineraryResources started");
                var header = SabreMapper.GetMessageHeader<GetReservationRQ.MessageHeader>(session.ConversationId, "GetReservationRQ", session.Organization);
                var security = new GetReservationRQ.Security { BinarySecurityToken = session.Token };

                var proxy = new GetReservationRQ.GetReservationPortTypeClient("GetReservationPortType");
                var response = await proxy.GetReservationOperationAsync(header, security, request);*/
        }

        public async Task<UpdateReservationRQ.UpdateReservationOperationResponse> UpdateItinerary(Session session, UpdateReservationRQ.UpdateReservationRQ request)
        {
            var req = new UpdateItineraryRequest(_logger);
            return await req.CallSabreMethod(session, request);
        }
    }
}