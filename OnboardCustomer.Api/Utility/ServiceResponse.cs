using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardCustomer.Api.Utility
{
    public class ServiceResponse<T> where T : class
    {
        public bool IsSuccess { get; set; } = true;
        public T Data { get; set; } = null;
        public string Message { get; set; }
    }
}
