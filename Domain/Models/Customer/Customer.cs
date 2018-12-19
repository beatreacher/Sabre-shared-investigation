using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Customer
{
    public class Customer
    {
        public Person Person { get; }
        public Phone Phone { get; }

        [StringLength(15)]
        public string CustomerId { get; }

        [StringLength(5)]
        public string TravelerRefNumber { get; }

        [StringLength(15)]
        public string Email { get; }

        public Customer(Person person, Phone phone, string customerId, string travelerRefNumber, string email)
        {
            Person = person;
            Phone = phone;
            CustomerId = customerId;
            TravelerRefNumber = travelerRefNumber;
            Email = email;
        }
    }
}
