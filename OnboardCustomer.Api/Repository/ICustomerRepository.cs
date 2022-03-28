using OnboardCustomer.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardCustomer.Api.Repository
{
    public interface ICustomerRepository
    {
        Task<Customer> Add(Customer customer);
        Task<List<Customer>> GetAll();
        Task<List<State>> GetAllStates();
        Task<List<LocalGovt>> GetLocalGovtByStateId(int id);
        Task<Customer> GetById(long id);
        Task<Customer> GetByOTP(string otp, string phone);
        Task ActivateAsync(Customer customer);
    }
}
