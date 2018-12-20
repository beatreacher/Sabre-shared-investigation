using System;

namespace Domain.Models
{
    public class SabreException : Exception
    {
        public string SabreRequest { get; set; }

        public string SabreResponse { get; set; }

        public SabreException(string message) : base(message)
        {
        }
    }
}
