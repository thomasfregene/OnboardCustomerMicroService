using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardCustomer.Api.Dtos
{
    public class AddCustomerDto
    {
        public string Email { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{4})[-. ]?([0-9]{4})$", ErrorMessage = "invalid phone number")]
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int StateId { get; set; }
        public int LocalGovtId { get; set; }
    }
}
