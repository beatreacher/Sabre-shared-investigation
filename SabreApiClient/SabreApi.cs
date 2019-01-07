using System;
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
    }
}