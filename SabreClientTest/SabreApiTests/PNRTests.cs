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
        ILogger _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());

        [TestMethod]
        public async Task CreatePNRTest()
        {
            //var sessionManager = new SessionManager(_logger);
            //var session = await sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");

            var session = new Session(
                "Shared/IDL:IceSess\\/SessMgr:1\\.0.IDL/Common/!ICESMS\\/CERTG!ICESMSLB\\/CRT.LB!1547813199832!4723!5", 
                "532941e3-cc1e-49dc-b008-e1708ab80c13",
                "115558435996450151", 
                "2019-01-18T12:06:39", 
                "92RG");

            await CreatePNR(session, null);

            //var response = await sessionManager.CloseSession(session);
            //response.Should().Be("Approved");
        }

        private async Task CreatePNR(Session session, string existedPnr)
        {
            var client = new SabreApi(_logger);
            var req = new PNR.PassengerDetailsRQ
            {
                version = "3.3.0",
                IgnoreOnError = false,
                HaltOnError = false,
                MiscSegmentSellRQ = new PNR.PassengerDetailsRQMiscSegmentSellRQ
                {
                    MiscSegment = new PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegment
                    {
                        DepartureDateTime = "02-27",
                        NumberInParty = "1",
                        Type = PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegmentType.OTH,
                        Status = "HK",
                        OriginLocation = new PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegmentOriginLocation
                        {
                            LocationCode = "DFW"
                        },
                        Text = "RETENTION SEGMENT FOR SWS ORCHESTRATED SERVICES TEST",
                        VendorPrefs = new PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegmentVendorPrefs
                        {
                            Airline = new PNR.PassengerDetailsRQMiscSegmentSellRQMiscSegmentVendorPrefsAirline
                            {
                                Code = "DL"
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
                    new PNR.PassengerDetailsRQPreProcessing():
                    new PNR.PassengerDetailsRQPreProcessing
                    {
                        IgnoreBefore = false,
                        UniqueID = new PNR.PassengerDetailsRQPreProcessingUniqueID { ID = existedPnr }
                    }                ,
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
            var pnr = await client.CreatePNR(session, req);

            var pnrSer = JsonConvert.SerializeObject(pnr.PassengerDetailsRS);
            _logger.Debug("-------------- PNR --------------");
            _logger.Debug(pnrSer);
        }

        [TestMethod]
        public async Task LoadPNRTest()
        {
            var sessionManager = new SessionManager(_logger);
            var session = await sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");
            var client = new SabreApi(_logger);

            var req = new LOR.TravelItineraryReadRQ
            {
                Version = "3.10.0",
                MessagingDetails = new LOR.TravelItineraryReadRQMessagingDetails
                {
                    SubjectAreas = new string[] { "FULL" }
                },
                //UniqueID = new LOR.TravelItineraryReadRQUniqueID { ID = "UFHEMQ" }
                //UniqueID = new LOR.TravelItineraryReadRQUniqueID { ID = "HQOOAM" }
                UniqueID = new LOR.TravelItineraryReadRQUniqueID { ID = "PMVALJ" }
            };

            var pnr = await client.LoadPNR(session, req);

            var pnrSer = JsonConvert.SerializeObject(pnr.TravelItineraryReadRS);
            _logger.Debug("-------------- PNR --------------");
            _logger.Debug(pnrSer);

            var response = await sessionManager.CloseSession(session);
            response.Should().Be("Approved");
        }
    }
}
