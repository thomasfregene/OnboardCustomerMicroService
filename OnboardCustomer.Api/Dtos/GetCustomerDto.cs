using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardCustomer.Api.Dtos
{
    public class GetCustomerDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string StateOfResidence { get; set; }
        //public LocalGovt LocalGovt { get; set; }
        public string LocalGovt { get; set; }
        public bool IsVerified { get; set; }
    }
}
