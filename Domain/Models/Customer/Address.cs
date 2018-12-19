using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Customer
{
    public class Address
    {
        [StringLength(15)]
        public string CountryName { get; }

        [StringLength(2)]
        public string State { get; }

        [StringLength(9)]
        public string PostalCode { get; }

        [StringLength(15)]
        public string CityName { get; }

        [StringLength(25)]
        public string Street { get; }

        public Address(string country, string state, string postalCode, string city, string street)
        {
            CountryName = country;
            State = state;
            PostalCode = postalCode;
            CityName = city;
            Street = street;
        }
    }
}
