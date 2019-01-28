using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac.Extras.NLog;
using Domain.Models;
using Newtonsoft.Json;
using SabreApiClient.Interfaces;

namespace ApiService.Controllers
{
    public class SabreController : ApiController
    {
        private readonly ISessionManager _sessionManager;
        private readonly ISabreApi _sabreApiClient;
        private readonly ILogger _logger;

        public SabreController(ISessionManager sessionManager, ISabreApi sabreApiClient, ILogger logger)
        {
            _sessionManager = sessionManager;
            _sabreApiClient = sabreApiClient;
            _logger = logger;
        }

        [HttpPost]
        [ActionName("CreateSession")]
        public async Task<Session> CreateSession(Credentials credentials)
        {
            try
            {
                _logger.Debug("CreateSession started");

                var session = await _sessionManager.CreateSession(credentials, "SessionCreateRQ");
                return session;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("CreateSession finished");
            }
        }

        [HttpPost]
        [ActionName("CloseSession")]
        public async Task<string> CloseSession()
        {
            try
            {
                _logger.Debug("CloseSession started");
                Session session = GetSession(Request);
                return await _sessionManager.CloseSession(session);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("CloseSession finished");
            }
        }

        [HttpPost]
        [ActionName("GetDummyFlightSchedule")]
        public async Task<SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRS> GetDummyFlightSchedule(object requestObject)
        {
            try
            {
                _logger.Debug("GetDummyFlightSchedule started");
                var airScheduleRequest = JsonConvert.DeserializeObject<SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQ>(requestObject.ToString());
                //var airScheduleRequest = JsonConvert.DeserializeObject<SabreApiClient.OTA_AirScheduleService.OTA_AirScheduleRQ>(requestObject);
                //var json = new JavaScriptSerializer().Serialize(airScheduleRequest);

                Session session = GetSession(Request);

                var schedule = await _sabreApiClient.GetFlightSchedules(session, airScheduleRequest);

                return schedule.OTA_AirScheduleRS;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("GetDummyFlightSchedule finished");
            }
        }

        [HttpPost]
        [ActionName("GetBargainFinderMax")]
        //public async Task<SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRS> GetBargainFinderMax(SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQ request)
        public async Task<SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRS> GetBargainFinderMax([FromBody]SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQ requestObject)
        {
            try
            {
                _logger.Debug("GetBargainFinderMax started");
                var request = JsonConvert.DeserializeObject<SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQ>(requestObject.ToString(), new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
                Session session = GetSession(Request);

                var bargainFinderMax = await _sabreApiClient.GetBargainFinderMax(session, request);
            //var request = JsonConvert.DeserializeObject<SabreApiClient.BargainFinderMax.OTA_AirLowFareSearchRQ>(requestObject.ToString());
            //Session session = GetSession(Request);

            //var client = new SabreApi();
            //var bargainFinderMax = await client.GetBargainFinderMax(session, requestObject);

            //return bargainFinderMax.OTA_AirLowFareSearchRS;
            return null;
                return bargainFinderMax.OTA_AirLowFareSearchRS;
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

        [HttpPost]
        [ActionName("EnhancedAirBook")]
        public async Task<SabreApiClient.EnhancedAirBookRQ.EnhancedAirBookRQResponse> EnhancedAirBook(object requestObject)
        {
            try
            {
                _logger.Debug("EnhancedAirBook started");
                var request = JsonConvert.DeserializeObject<SabreApiClient.EnhancedAirBookRQ.EnhancedAirBookRQ>(requestObject.ToString(), new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
                Session session = GetSession(Request);

                var enhacned = await _sabreApiClient.GetEnhancedAirBook(session, request);

                return enhacned;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("EnhancedAirBook finished");
            }
        }

        [HttpPost]
        [ActionName("CreateOrUpdatePnr")]
        public async Task<SabreApiClient.CreatePNR.PassengerDetailsRQResponse> CreateOrUpdatePnr(object requestObject)
        {
            try
            {
                _logger.Debug("CreateOrUpdatePnr started");
                var request = JsonConvert.DeserializeObject<SabreApiClient.CreatePNR.PassengerDetailsRQ>(requestObject.ToString(), new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
                Session session = GetSession(Request);

                var enhacned = await _sabreApiClient.CreatePNR(session, request);

                return enhacned;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("CreateOrUpdatePnr finished");
            }
        }

        [HttpPost]
        [ActionName("LoadPnr")]
        public async Task<SabreApiClient.LoadPNR.TravelItineraryReadRQResponse> LoadPnr(object requestObject)
        {
            try
            {
                _logger.Debug("LoadPnr started");
                var request = JsonConvert.DeserializeObject<SabreApiClient.LoadPNR.TravelItineraryReadRQ>(requestObject.ToString(), new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
                Session session = GetSession(Request);

                var enhacned = await _sabreApiClient.LoadPNR(session, request);

                return enhacned;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("LoadPnr finished");
            }
        }

        [HttpPost]
        [ActionName("EndTransaction")]
        public async Task<SabreApiClient.EndTransactionLLSRQ.EndTransactionRQResponse> EndTransaction(object requestObject)
        {
            try
            {
                _logger.Debug("EndTransaction started");
                var request = JsonConvert.DeserializeObject<SabreApiClient.EndTransactionLLSRQ.EndTransactionRQ>(requestObject.ToString(), new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
                Session session = GetSession(Request);

                var enhacned = await _sabreApiClient.EndTransaction(session, request);

                return enhacned;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            finally
            {
                _logger.Debug("EndTransaction finished");
            }
        }


        private static Session GetSession(HttpRequestMessage request)
        {
            if (!request.Headers.Contains("Token")
                || !request.Headers.Contains("ConversationId")
                || !request.Headers.Contains("MessageId")
                || !request.Headers.Contains("TimeStamp")
                || !request.Headers.Contains("Organization")
                )
            {
                throw new ArgumentException("Session parameters incomplete");
            }

            Session session = new Session
            (
                request.Headers.GetValues("Token").First(),
                request.Headers.GetValues("ConversationId").First(),
                request.Headers.GetValues("MessageId").First(),
                request.Headers.GetValues("TimeStamp").First(),
                request.Headers.GetValues("Organization").First()
            );

            return session;
        }
    }
}
