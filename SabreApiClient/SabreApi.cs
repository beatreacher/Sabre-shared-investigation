﻿using System;
using SabreApiClient.Helpers;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

using Domain.Models;
using SabreApiClient.Interfaces;

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
    }
}