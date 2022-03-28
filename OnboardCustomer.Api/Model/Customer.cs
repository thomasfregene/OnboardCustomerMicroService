using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardCustomer.Api.Model
{
    public class Customer
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        //public State State { get; set; }
        public int StateId { get; set; }
        public LocalGovt LocalGovt { get; set; }
        public int LocalGovtId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public bool IsVerified { get; set; }
        public string OTP { get; set; }
    }
}
