using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FluentAssertions;
using Domain.Models;
using SabreApiClient;
using AutoFixture;
using Domain.Models.Customer;

namespace SabreClientTest
{
    [TestClass]
    public class SabreApiClientTests
    {
        Client _client = new Client("7971", "WS102513", "92RG", "DEFAULT");
        //Client _client = new Client("*", "*", "*", "DEFAULT");

        [TestMethod]
        public async Task GetToken()
        {
            var sabreApiClient = new SabreApiClient.SabreApiClient();
            var session = await sabreApiClient.CreateAccessToken(_client, "TokenCreateRQ");

            Assert.IsNotNull(session);
            Assert.IsNotNull(session.ConversationId);
            Assert.IsNotNull(session.Token);
            Assert.IsNotNull(session.MessageId);
            Assert.IsNotNull(session.TimeStamp);
        }

        [TestMethod]
        public async Task GetTravelItineraryAddInfoTest()
        {
            var sabreApiClient = new SabreApiClient.SabreApiClient();
            var session = await sabreApiClient.CreateAccessToken(_client, "SessionCreateRQ");

            //var fixture = new Fixture();
            //var customer = fixture.Create<Customer>();
            //var agency = fixture.Create<Agency>();

            var response = await sabreApiClient.GetTravelItineraryAddInfo(_client, session, GetCustomer(), GetAgency());

            var r1 = response.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }

        private static Customer GetCustomer()
        {
            var person = new Person("1.1", "MAX", "PLANCK", "REF1");
            var phone = new Phone("817", "555-1212", "H");

            var customer = new Customer(person, phone, "1234567890", "1.1", "MAX.PLANCK@HOTMAIL.COM");

            return customer;
        }

        private static Agency GetAgency()
        {
            var phone = new Phone("817", "555-1212", "H");
            var address = new Address("US", "TX", "76092", "SOUTHLAKE", "3150 SABRE DRIVE");

            var agency = new Agency("SABRE TRAVEL", address, phone, "7T-");
            return agency;
        }
    }
}
