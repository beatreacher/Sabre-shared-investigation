using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Customer
{
    public class Person
    {
        [StringLength(5)]
        public string TravelerRefNumber { get; }

        [StringLength(10)]
        public string GivenName { get; }

        [StringLength(10)]
        public string Surname { get; }

        [StringLength(3)]
        public string NameReference { get; }

        public Person(string travelerRefNumber, string givenName, string surname, string nameReference)
        {
            TravelerRefNumber = travelerRefNumber;
            GivenName = givenName;
            Surname = surname;
            NameReference = nameReference;
        }
    }
}
