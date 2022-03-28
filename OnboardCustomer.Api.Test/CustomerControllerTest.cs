using Moq;
using OnboardCustomer.Api.Controllers;
using OnboardCustomer.Api.Dtos;
using OnboardCustomer.Api.Services;
using OnboardCustomer.Api.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OnboardCustomer.Api.Test
{
    public class CustomerControllerTest
    {
        public Mock<ICustomerService> mock = new Mock<ICustomerService>();

        [Fact]
        public async void GetCustomer() 
        {
            var customers = new List<GetCustomerDto>();
            mock.Setup(c => c.GetCustomers()).ReturnsAsync(customers);
            CustomerController customer = new CustomerController(mock.Object);
            var result = await customer.GetCustomers();

            Assert.Equal(customers, (IEnumerable<GetCustomerDto>)result);
        }

        [Fact]
        public async void CreateCustomer()
        {
            var response = new ServiceResponse<GetCustomerDto>();
            var customer = new AddCustomerDto
            {
                Email = "judes@jude.com",
                PhoneNumber = "0902134567",
                Password = "password1",
                StateId = 1,
                LocalGovtId = 159
            };

            mock.Setup(c => c.AddCustomer(customer)).ReturnsAsync(response);
            CustomerController customerController = new CustomerController(mock.Object);
            var result = await customerController.CreateCustomer(customer);
        }
    }
}
