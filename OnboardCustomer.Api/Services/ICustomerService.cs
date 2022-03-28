using OnboardCustomer.Api.Dtos;
using OnboardCustomer.Api.Model;
using OnboardCustomer.Api.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardCustomer.Api.Services
{
    public interface ICustomerService
    {
        Task<ServiceResponse<GetCustomerDto>> AddCustomer(AddCustomerDto customerDto);
        Task<List<GetCustomerDto>> GetCustomers();
        Task<ServiceResponse<GetCustomerDto>> GetCustomerById(int id);

        Task<List<State>> GetStates();
        Task<List<LocalGovt>> GetLocalGovts(int id);
        Task<ServiceResponse<Customer>> ActivateCustomer(string otp, string phone);
        Task<List<Bank>> GetBanks();
    }
}
