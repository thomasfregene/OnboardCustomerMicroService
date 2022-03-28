using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using OnboardCustomer.Api.Dtos;
using OnboardCustomer.Api.Model;
using OnboardCustomer.Api.Repository;
using OnboardCustomer.Api.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace OnboardCustomer.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customer;
        private readonly IHttpClientFactory _httpFactory;

        public CustomerService(ICustomerRepository customer, IHttpClientFactory httpFactory)
        {
            _customer = customer;
            _httpFactory = httpFactory;
        }
        public async Task<ServiceResponse<GetCustomerDto>> AddCustomer(AddCustomerDto customerDto)
        {
            var customerModel = new Customer();
            var response = new ServiceResponse<GetCustomerDto>();
            try
            {
                var validemail = IsValidEmail(customerDto.Email);
                if (!validemail)
                {
                    var invalidemail = new ServiceResponse<GetCustomerDto>
                    {
                        IsSuccess = false,
                        Message = "Invalid email"
                    };
                    return invalidemail;
                }
                customerModel.Email = customerDto.Email;
                //customerModel.State = customerDto.State;
                customerModel.PhoneNumber = customerDto.PhoneNumber;
                customerModel.StateId = customerDto.StateId;
                customerModel.LocalGovtId = customerDto.LocalGovtId;
                customerModel.Password = HashedPassword(customerDto.Password);
                customerModel.DateCreated = DateTime.Now;
                customerModel.CreatedBy = "wema";
                customerModel.OTP = GenerateOTP();

                await _customer.Add(customerModel);


                //send OTP
                sendSMS(customerDto.PhoneNumber, customerModel.OTP);

                var newCustomer = await _customer.GetById(customerModel.Id);

                var customerData = new GetCustomerDto
                {
                    Email = newCustomer.Email,
                    PhoneNumber = newCustomer.PhoneNumber,
                    StateOfResidence = newCustomer.LocalGovt.State.Name,
                    LocalGovt = newCustomer.LocalGovt.Name
                };

                response = new ServiceResponse<GetCustomerDto>
                {
                    Data = customerData,
                    Message = "Please verify phone number using the OTP sent to you."
                };



            }
            catch (Exception ex)
            {

                response = new ServiceResponse<GetCustomerDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return response;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCustomerDto>> GetCustomerById(int id)
        {
            var customer = await _customer.GetById(id);
            var response = new ServiceResponse<GetCustomerDto>();
            try
            {

                var customerDetails = new GetCustomerDto
                {
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    StateOfResidence = customer.LocalGovt.State.Name,
                    LocalGovt = customer.LocalGovt.Name,
                    IsVerified = customer.IsVerified
                };
                if (customerDetails.IsVerified == true)
                {
                    response = new ServiceResponse<GetCustomerDto>
                    {
                        Data = customerDetails,
                    };
                }
                else
                {
                    response = new ServiceResponse<GetCustomerDto>
                    {
                        IsSuccess = false,
                        Message = "An OTP was send to your phone number to confirm your account "
                    };
                    var otp = GenerateOTP();
                    sendSMS(customerDetails.PhoneNumber, otp);
                }
            }
            catch (Exception ex)
            {

                response = new ServiceResponse<GetCustomerDto>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };

                return response;
            }

            return response;
        }

        public async Task<List<GetCustomerDto>> GetCustomers()
        {
            var customers = await _customer.GetAll();
            var customersDetails = customers.Select(x => new GetCustomerDto
            {
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                StateOfResidence = x.LocalGovt.State.Name,
                LocalGovt = x.LocalGovt.Name,
                IsVerified = x.IsVerified
            }).ToList();
            return customersDetails;
        }

        public async Task<List<LocalGovt>> GetLocalGovts(int id)
        {

            var localGovts = await _customer.GetLocalGovtByStateId(id);
            return localGovts;
        }

        public async Task<List<State>> GetStates()
        {
            var states = await _customer.GetAllStates();
            return states;
        }


        public async Task<ServiceResponse<Customer>> ActivateCustomer(string otp, string phone)
        {
            var response = new ServiceResponse<Customer>();
            try
            {
                var customer = await _customer.GetByOTP(otp, phone);

                if (customer != null)
                {
                    customer.IsVerified = true;
                    customer.OTP = string.Empty;
                    await _customer.ActivateAsync(customer);

                    response = new ServiceResponse<Customer>
                    {
                        Message = "Verification Sucessful. User is now activated"
                    };

                    return response;
                }
                response = new ServiceResponse<Customer>
                {
                    IsSuccess = false,
                    Message = "invalid OTP"
                };
            }
            catch (Exception)
            {

                throw;
            }
            return response;
        }

        public async Task<List<Bank>> GetBanks()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get, "https://wema-alatdev-apimgt.azure-api.net/alat-test/api/Shared/GetAllBanks")
                ;
            var client = _httpFactory.CreateClient();
            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var getbanksResponse = JsonSerializer.Deserialize<List<Bank>>(json);

            return getbanksResponse;
        }
        /// <summary>
        /// Helper Methods
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>

        private string HashedPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                MailAddress mail = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {

                return false;
            }
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            var randomNumber = (random.Next(100000, 999999)).ToString();
            return randomNumber;
        }


        private string sendSMS(string phone, string otp)
        {
            String message = HttpUtility.UrlEncode($"OTP:{otp}");
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.txtlocal.com/send/", new NameValueCollection()
                {
                {"apikey" , "yourapiKey"},
                {"numbers" , phone},
                {"message" , message},
                {"sender" , "wema bank"}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

    }
}
