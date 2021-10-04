using System;
using System.Collections.Generic;
using System.Text;

namespace SQLScriptHelper.Tests.Models
{
    public class Person
    {
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public bool IsMale { get; set; }
        public DateTime? DateOfBirthOfPartner { get; set; }
        public int? AgeOfPartner { get; set; }
        public bool? IsPartnerMale { get; set; }
        public double ExchangeRateFromCountry { get; set; }
    }
}
