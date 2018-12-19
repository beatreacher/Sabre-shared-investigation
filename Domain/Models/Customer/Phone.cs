using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Customer
{
    public class Phone
    {
        [StringLength(3)]
        public string AreaCityCode { get; }

        [StringLength(10)]
        public string PhoneNumber { get; }

        [StringLength(1)]
        public string PhoneUseType { get; }

        public Phone(string areaCityCode, string phoneNumber, string phoneUseType)
        {
            AreaCityCode = areaCityCode;
            PhoneNumber = phoneNumber;
            PhoneUseType = phoneUseType;
        }
    }
}
