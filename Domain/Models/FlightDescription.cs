using System;

namespace Domain.Models
{
    public class FlightDescription
    {
        public string RPH { get; set; }

        public string OriginLocation { get; set; }

        public string DestinationLocation { get; set; }

        public string DepartureDateTime { get; set; }
        public string ArrivalDateTime { get; set; }

        public string FlightNumber { get; set; }

        public string NumberInParty { get; set; }

        public string ResBookDesigCode { get; set; }

        public string Status { get; set; }

        public bool InstantPurchase { get; set; }

        public string MarketingAirline { get; set; }

        public string OperatingAirline { get; set; }

        public string PnrDepartureDateTime
        {
            get
            {
                return DateTime.Parse(DepartureDateTime).ToString("MM-dd");
            }
        }
    }
}
