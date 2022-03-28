using Microsoft.AspNetCore.Mvc;
using OnboardCustomer.Api.Dtos;
using OnboardCustomer.Api.Model;
using OnboardCustomer.Api.Services;
using OnboardCustomer.Api.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardCustomer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("createcustomer")]
        //public async Task<IActionResult> CreateCustomer(AddCustomerDto customer)
        public async Task<ServiceResponse<GetCustomerDto>> CreateCustomer([FromBody]AddCustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                var serviceResponse = new ServiceResponse<GetCustomerDto>
                {
                    IsSuccess = false,
                    Message = "Bad Request"
                };
                return serviceResponse;
            }
            //if (!ModelState.IsValid)
            //    return BadRequest();


            var response = await _customerService.AddCustomer(customer);
            //return Ok(response);
            return response;
        }

        [HttpGet("getcustomers")]
        //public async Task<IActionResult> GetCustomers()
        public async Task<List<GetCustomerDto>> GetCustomers()
        {
            var customers = await _customerService.GetCustomers();

            //return Ok(customers);
            return customers;
        }

        [HttpGet("getcustomer")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerById(id);
            return Ok(customer);
        }

        [HttpGet("getstates")]
        //public async Task<IActionResult> GetStates()
        public async Task<List<State>> GetStates()
        {
            var states = await _customerService.GetStates();

            //return Ok(states);
            return states;
        }

        [HttpPut("verifycustomerOTP")]
        public async Task<IActionResult> OTPVerification(string otp, string phone)
        {
            var response = await _customerService.ActivateCustomer(otp, phone);

            if (response.IsSuccess == false)
                return NotFound(response);

            return Ok(response);

        }

        [HttpGet("getlocalgovtbystate")]
        public async Task<IActionResult> GetLocalGovtByStateId(int id)
        {
            var localGovts = await _customerService.GetLocalGovts(id);

            return Ok(localGovts);
        }


        [HttpGet("getbanks")]
        public IActionResult GetBank()
        {
            return Ok();
        }
    }
}
