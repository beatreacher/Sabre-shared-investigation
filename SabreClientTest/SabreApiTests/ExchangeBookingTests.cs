using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SabreApiClient;
using Autofac.Extras.NLog;
using Newtonsoft.Json;
using Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using Exc = SabreApiClient.ExchangeBookingRQ;

namespace SabreClientTest.SabreApiTests
{
    [TestClass]
    public class ExchangeBookingTests
    {
        ILogger _logger;
        SabreApi _client;
        SessionManager _sessionManager;
        public static Session CurrentSession;

        public ExchangeBookingTests()
        {
            _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());
            _sessionManager = new SessionManager(_logger);
            _client = new SabreApi(_logger);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }

        private static void CreateExchangeRequest()
        {
            var req = new Exc.ExchangeBookingRQ
            {

            };
        }
    }
}
