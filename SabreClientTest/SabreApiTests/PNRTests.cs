using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;

using PNR = SabreApiClient.CreatePNR;
using LOR = SabreApiClient.LoadPNR;
using Domain.Models;

namespace SabreClientTest
{
    [TestClass]
    public class PNRTests
    {
        ILogger _logger;
        SabreApi _client;
        SessionManager _sessionManager;

        //Session session;
        public static Session CurrentSession = new Session(
                "Shared/IDL:IceSess\\/SessMgr:1\\.0.IDL/Common/!ICESMS\\/CERTG!ICESMSLB\\/CRT.LB!1547822266488!114!9",
                "9d28ccc5-ef13-4428-a304-771053a0d40d",
                "379200526662920151",
                "2019-01-18T14:37:46",
                "92RG");

        public PNRTests()
        {
            _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());
            _sessionManager = new SessionManager(_logger);
            _client = new SabreApi(_logger);
        }

        [TestMethod]
        public async Task EndTransactionTest()
        {
            CurrentSession = await _sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");
            var response = await EndTransaction(CurrentSession);
        }

        public async Task<SabreApiClient.EndTransactionLLSRQ.EndTransactionRQResponse> EndTransaction(Session session)
        {
            var req = new SabreApiClient.EndTransactionLLSRQ.EndTransactionRQ
            {
                Version = "2.0.9",
                EndTransaction = new SabreApiClient.EndTransactionLLSRQ.EndTransactionRQEndTransaction
                {
                    Ind = true,
                },
                Source = new SabreApiClient.EndTransactionLLSRQ.EndTransactionRQSource
                {
                    ReceivedFrom = "SWS TEST"
                }
            };

            var resp = await _client.EndTransaction(session, req);

            var respSer = JsonConvert.SerializeObject(resp.EndTransactionRS);
            _logger.Debug("-------------- EndTransaction --------------");
            _logger.Debug(respSer);

            return resp;
        }

        [TestMethod]
        public async Task CreatePNRTest()
        {
            CurrentSession = await _sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            var sessionSer = JsonConvert.SerializeObject(CurrentSession);
            _logger.Debug("-------------- session --------------");
            _logger.Debug(sessionSer);


            await CreatePNR(CurrentSession, null, "JFK", "2019-04-15T16:55:00", "DL");

            //await CreatePNR(CurrentSession, "ULJUAA", "DFW", "02-27", "DL");
            //await CreatePNR(CurrentSession, "ULJUAA", "JFK", "2019-04-15T16:55:00", "DL");

            var response = await _sessionManager.CloseSession(CurrentSession);
            response.Should().Be("Approved");
        }

        [TestMethod]
        public async Task LoadPNRTest()
        {
            CurrentSession = await _sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            var pnrResp = await LoadPNR(CurrentSession, "HQOOAM");

            var response = await _sessionManager.CloseSession(CurrentSession);
            response.Should().Be("Approved");
        }

        public async Task<PNR.PassengerDetailsRQResponse> CreatePNR
        (
            Session session,
            string existedPnr,
            string originLocation,
            string departureDateTime,
            string airlineCode
        )
        {
            var req = new PNR.PassengerDetailsRQ
            {
                version = "3.3.0",
                IgnoreOnError = false,
                HaltOnError = false,
                MiscSegmentSellRQ = new PNR.PassengerDetailsRQMiscSegmentSellRQ
                {
                    MiscSegment = new PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegment
                    {
                        DepartureDateTime = departureDateTime,
                        NumberInParty = "1",
                        //
                        //Type = PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegmentType.OTH,
                        Status = "HK",
                        OriginLocation = new PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegmentOriginLocation
                        {
                            LocationCode = originLocation
                        },
                        Text = "RETENTION SEGMENT FOR SWS ORCHESTRATED SERVICES TEST",
                        VendorPrefs = new PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegmentVendorPrefs
                        {
                            Airline = new PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegmentVendorPrefsAirline
                            {
                                Code = airlineCode
                            }
                        }
                    }
                },

                PostProcessing = new PNR.PassengerDetailsRQPostProcessing
                {
                    RedisplayReservation = true,
                    UnmaskCreditCard = true,
                    IgnoreAfter = false,
                    EndTransactionRQ = new PNR.PassengerDetailsRQPostProcessingEndTransactionRQ
                    {
                        EndTransaction = new PNR.PassengerDetailsRQPostProcessingEndTransactionRQEndTransaction
                        {
                            Ind = "true"
                        },
                        Source = new PNR.PassengerDetailsRQPostProcessingEndTransactionRQSource
                        {
                            ReceivedFrom = "SWS ORCHESTRATED SERVICES TEST"
                        }
                    }
                },

                PreProcessing = string.IsNullOrEmpty(existedPnr) ? //Use it to updating existing PNR
                    new PNR.PassengerDetailsRQPreProcessing() :
                    new PNR.PassengerDetailsRQPreProcessing
                    {
                        IgnoreBefore = false,
                        UniqueID = new PNR.PassengerDetailsRQPreProcessingUniqueID { ID = existedPnr }
                    },
                SpecialReqDetails = new PNR.PassengerDetailsRQSpecialReqDetails
                {
                    SpecialServiceRQ = new PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQ
                    {
                        SpecialServiceInfo = new PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfo
                        {
                            AdvancePassenger = new PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassenger[]
                            {
                                new PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassenger
                                {
                                    SegmentNumber = "A",
                                    Document = new PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerDocument
                                    {
                                        ExpirationDate = DateTime.Now.AddYears(3),
                                        Number = "1234567890",
                                        Type = "P",
                                        IssueCountry = "FR",
                                        NationalityCountry = "FR"
                                    },
                                    PersonName = new PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerPersonName
                                    {
                                        DateOfBirth = DateTime.Now.AddYears(-30),
                                        //
                                        //Gender = PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerPersonNameGender.M,
                                        //NameNumber = "1.1",
                                        DocumentHolder = true,
                                        GivenName = "BAMBARBIA",
                                        MiddleName = "N",
                                        Surname = "KIRGUDU"
                                    },
                                    //VendorPrefs = new PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerVendorPrefs
                                    //{
                                    //    Airline = new PNR.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoAdvancePassengerVendorPrefsAirline
                                    //    {
                                    //        Hosted = false
                                    //    }
                                    //}
                                },
                            }
                        },
                    }
                },

                TravelItineraryAddInfoRQ = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQ
                {
                    AgencyInfo = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfo
                    {
                        Address = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddress
                        {
                            AddressLine = "SABRE TRAVEL",
                            CityName = "SOUTHLAKE",
                            CountryCode = "US",
                            PostalCode = "76092",
                            StateCountyProv = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddressStateCountyProv
                            {
                                StateCode = "TX"
                            },
                            StreetNmbr = "3150 SABRE DRIVE",
                            VendorPrefs = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddressVendorPrefs
                            {
                                Airline = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddressVendorPrefsAirline
                                {
                                    Hosted = true
                                }
                            }
                        },
                        Ticketing = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoTicketing
                        {
                            TicketType = "7T-A"
                        }
                    },
                    CustomerInfo = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfo
                    {
                        ContactNumbers = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumber[]
                        {
                            new PNR.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumber
                            {
                                //NameNumber = "1.1",
                                Phone = "682-605-1000",
                                PhoneUseType = "H"
                            }
                        },
                        PersonName = new PNR.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoPersonName[]
                        {
                            new PNR.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoPersonName
                            {
                                //NameNumber = "1.1",
                                //NameReference = "ABC123",
                                //PassengerType = "ADT",
                                GivenName = "Octavian",
                                Surname = "August"
                            }
                        }
                    }
                }
            };
            var pnr = await _client.CreatePNR(session, req);

            var pnrSer = JsonConvert.SerializeObject(pnr.PassengerDetailsRS);
            _logger.Debug("-------------- PNR --------------");
            _logger.Debug(pnrSer);

            return pnr;
        }

        public async Task<LOR.TravelItineraryReadRQResponse> LoadPNR(Session session, string pnr)
        {
            var req = new LOR.TravelItineraryReadRQ
            {
                Version = "3.10.0",
                MessagingDetails = new LOR.TravelItineraryReadRQMessagingDetails
                {
                    SubjectAreas = new string[] { "FULL" }
                },
                UniqueID = new LOR.TravelItineraryReadRQUniqueID { ID = pnr }
            };

            var pnrResp = await _client.LoadPNR(session, req);

            var pnrSer = JsonConvert.SerializeObject(pnrResp.TravelItineraryReadRS);
            _logger.Debug("-------------- PNR --------------");
            _logger.Debug(pnrSer);

            return pnrResp;
        }
    }
}
