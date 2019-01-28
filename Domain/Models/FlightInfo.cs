using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class FlightInfo
    {
        public DateTimeOffset DepartureDateTime { get; set; }
        public DateTimeOffset ArrivalDateTime { get; set; }
        //Duration in minutes
        public double Duration { get; set; }
        public string OriginAirportCode { get; set; }
        public string ArrivalAirportCode { get; set; }
        public decimal Price { get; set; }
        public int NumberOfFlights { get; set; }
        public string AirlineCode { get; set; }
        public string ResBookDesignCode { get; set; }
        public string SerializedJson { get; set; }
    }
}
