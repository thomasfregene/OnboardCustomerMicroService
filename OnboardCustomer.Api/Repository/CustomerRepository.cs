using Microsoft.EntityFrameworkCore;
using OnboardCustomer.Api.Data;
using OnboardCustomer.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardCustomer.Api.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _context;

        public CustomerRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Customer> Add(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<List<Customer>> GetAll()
        {
            var result = await _context.Customers
                .Include(x => x.LocalGovt)
                .ThenInclude(l => l.State)
                .ToListAsync();
            return result;
        }

        public async Task<List<State>> GetAllStates()
        {
            var states = await _context.States.ToListAsync();
            return states;
        }

        public async Task<Customer> GetById(long id)
        {
            var result = await _context.Customers
                .Include(x => x.LocalGovt)
                .ThenInclude(s => s.State)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<Customer> GetByOTP(string otp, string phone)
        {
            var customer = await _context.Customers.Where(x => x.OTP == otp && x.PhoneNumber == phone).FirstOrDefaultAsync();
            return customer;
        }

        public Task<List<LocalGovt>> GetLocalGovtByStateId(int id)
        {
            var localGovts = _context.LocalGovts.Where(x => x.StateId == id).ToListAsync();
            return localGovts;
        }

        public async Task ActivateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}
