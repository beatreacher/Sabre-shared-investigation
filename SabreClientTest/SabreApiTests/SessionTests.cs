using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Domain.Models;
using SabreApiClient;
using Autofac.Extras.NLog;

namespace SabreClientTest
{
    [TestClass]
    public class SessionTests
    {
        ILogger _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());

        public static readonly Credentials ApiCredentials = new Credentials(
                ConfigurationManager.AppSettings["UserName"],
                ConfigurationManager.AppSettings["Password"],
                ConfigurationManager.AppSettings["Organization"],
                ConfigurationManager.AppSettings["Domain"]
                );

        [TestMethod]
        public async Task GetToken()
        {
            var sessionManager = new SessionManager(_logger);
            var session = await sessionManager.CreateSession(ApiCredentials, "TokenCreateRQ");

            session.Should().NotBeNull();
            session.ConversationId.Should().NotBeNull();
            session.Token.Should().NotBeNull();
            session.MessageId.Should().NotBeNull();
            session.TimeStamp.Should().NotBeNull();
        }

        [TestMethod]
        public async Task CreateCloseSessionTest()
        {
            var client = new SessionManager(_logger);
            var session = await client.CreateSession(ApiCredentials, "SessionCreateRQ");

            session.Should().NotBeNull();
            session.ConversationId.Should().NotBeNull();
            session.Token.Should().NotBeNull();
            session.MessageId.Should().NotBeNull();
            session.TimeStamp.Should().NotBeNull();

            var response = await client.CloseSession(session);
            response.Should().NotBeNull();
            response.Should().Be("Approved");
        }
    }
}
