using System;
using SabreApiClient.Helpers;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Domain.Models;
using SabreApiClient.Interfaces;
using Newtonsoft.Json;

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
            OTA_AirScheduleService.OTA_AirScheduleRQ req
            )
        {
            try
            {
                _logger.Debug("GetFlightSchedules started");

                var header = SabreMapper.GetMessageHeader<OTA_AirScheduleService.MessageHeader>(session.ConversationId, "OTA_AirScheduleLLSRQ", session.Organization);

                var security = new OTA_AirScheduleService.Security1 { BinarySecurityToken = session.Token };

                var proxy = new OTA_AirScheduleService.OTA_AirSchedulePortTypeClient("OTA_AirSchedulePortType");
                var response = await proxy.OTA_AirScheduleRQAsync(header, security, req);
                ResponseProcessor.CheckErrors(req, response);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("GetFlightSchedules finished");
            }
        }

        public async Task<BargainFinderMax.BargainFinderMaxRQResponse> GetBargainFinderMax(Session session, BargainFinderMax.OTA_AirLowFareSearchRQ request)
        {
            try
            {
                _logger.Debug("GetBargainFinderMax started");
                var header = SabreMapper.GetMessageHeader<BargainFinderMax.MessageHeader>(session.ConversationId, "BargainFinderMaxRQ", session.Organization);
                var security = new BargainFinderMax.Security { BinarySecurityToken = session.Token };

                var proxy = new BargainFinderMax.BargainFinderMaxPortTypeClient("BargainFinderMaxPortType");
                var t = JsonConvert.SerializeObject(request);
                var response = await proxy.BargainFinderMaxRQAsync(header, security, request);

                ResponseProcessor.CheckErrors(request, response);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("GetBargainFinderMax finished");
            }
        }

        public async Task<OTA_AirBookLLSRQ.OTA_AirBookRQResponse> GetAirBook(Session session, OTA_AirBookLLSRQ.OTA_AirBookRQ request)
        {
            try
            {
                _logger.Debug("GetAirBook started");
                var header = SabreMapper.GetMessageHeader<OTA_AirBookLLSRQ.MessageHeader>(session.ConversationId, "OTA_AirBookLLSRQ", session.Organization);
                var security = new OTA_AirBookLLSRQ.Security1 { BinarySecurityToken = session.Token };

                var proxy = new OTA_AirBookLLSRQ.OTA_AirBookPortTypeClient("OTA_AirBookPortType");
                var response = await proxy.OTA_AirBookRQAsync(header, security, request);

                ResponseProcessor.CheckErrors(request, response);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("GetAirBook finished");
            }
        }

        public async Task<EnhancedAirBookRQ.EnhancedAirBookRQResponse> GetEnhancedAirBook(Session session, EnhancedAirBookRQ.EnhancedAirBookRQ request)
        {
            try
            {
                _logger.Debug("GetAirBook started");
                var header = SabreMapper.GetMessageHeader<EnhancedAirBookRQ.MessageHeader>(session.ConversationId, "EnhancedAirBookRQ", session.Organization);
                var security = new EnhancedAirBookRQ.Security { BinarySecurityToken = session.Token };

                var proxy = new EnhancedAirBookRQ.EnhancedAirBookPortTypeClient("EnhancedAirBookPortType");
                var response = await proxy.EnhancedAirBookRQAsync(header, security, request);

                ResponseProcessor.CheckErrors(request, response);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("GetAirBook finished");
            }
        }

        public async Task<CreatePNR.PassengerDetailsRQResponse> CreatePNR(Session session, CreatePNR.PassengerDetailsRQ request)
        {
            try
            {
                _logger.Debug("CreatePNR started");
                var header = SabreMapper.GetMessageHeader<CreatePNR.MessageHeader>(session.ConversationId, "PassengerDetailsRQ", session.Organization);
                var security = new CreatePNR.Security { BinarySecurityToken = session.Token };

                var proxy = new CreatePNR.PassengerDetailsPortTypeClient("PassengerDetailsPortType");
                var response = await proxy.PassengerDetailsRQAsync(header, security, request);

                ResponseProcessor.CheckErrors(request, response);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("CreatePNR finished");
            }
        }

        public async Task<LoadPNR.TravelItineraryReadRQResponse> LoadPNR(Session session, LoadPNR.TravelItineraryReadRQ request)
        {
            try
            {
                //SabreApiClient.LoadPNR.
                _logger.Debug("LoadPNR started");
                var header = SabreMapper.GetMessageHeader<LoadPNR.MessageHeader>(session.ConversationId, "TravelItineraryReadRQ", session.Organization);
                var security = new LoadPNR.Security1 { BinarySecurityToken = session.Token };

                var proxy = new LoadPNR.TravelItineraryReadPortTypeClient("TravelItineraryReadPortType");
                var response = await proxy.TravelItineraryReadRQAsync(header, security, request);

                ResponseProcessor.CheckErrors(request, response);

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("LoadPNR finished");
            }
        }
    }
}