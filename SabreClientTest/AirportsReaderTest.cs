using System;
using System.Linq;
using BusinessLogic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SabreClientTest
{
    [TestClass]
    public class AirportsReaderTest
    {
        [TestMethod]
        public void ReadAirportsTest()
        {
            var airports = (new AirportsReader()).Read();

            airports.Should().NotBeEmpty();
            airports.FirstOrDefault(i => i.Code.Equals("ABE")).Should().NotBeNull();
        }
    }
}
