using Domain.Models.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Agency
    {
        [StringLength(25)]
        public string Name { get; }
        public Address Address { get; }
        public Phone Phone { get; }

        [StringLength(5)]
        public string TicketType { get; }

        public Agency(string name, Address address, Phone phone, string ticketType)
        {
            Name = name;
            Address = address;
            Phone = phone;
            TicketType = ticketType;
        }
    }
}
