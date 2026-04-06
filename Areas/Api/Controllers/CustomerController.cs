using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.CustomerRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JewelryStore.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _repository;
        private readonly ApiResponseModel CommonViewModel = new();

        public CustomerController(ICustomerRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> GetAll(ReviewPagingRequest request)
        {
            try
            {
                var data = await _repository.GetAllCustomer(request);

                CommonViewModel.IsSuccess = true;
                CommonViewModel.StatusCode = ResponseStatusCode.Success;
                CommonViewModel.Message = "Data retrieved successfully";
                CommonViewModel.Data = data;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }

        [HttpGet("[Action]")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _repository.GetCustomerById(id);

                if (data != null)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = "Data retrieved successfully";
                    CommonViewModel.Data = data;
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = "Customer not found";
                }
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> Save(Customer customer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customer.AddressLine1))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter street address.";
                    return Ok(CommonViewModel);
                }

                if (string.IsNullOrWhiteSpace(customer.City))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter City.";
                    return Ok(CommonViewModel);
                }

                if (string.IsNullOrWhiteSpace(customer.State))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter State.";
                    return Ok(CommonViewModel);
                }

                // Postal Code
                if (string.IsNullOrWhiteSpace(customer.PostalCode))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter Postal Code.";
                    return Ok(CommonViewModel);
                }

                if (!customer.PostalCode.All(char.IsDigit))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Postal Code must contain numbers only.";
                    return Ok(CommonViewModel);
                }

                if (customer.PostalCode.Length != 6)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Postal Code must be exactly 6 digits.";
                    return Ok(CommonViewModel);
                }

                // Country
                if (string.IsNullOrWhiteSpace(customer.Country))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter Country.";
                    return Ok(CommonViewModel);
                }

                // Mobile
                if (string.IsNullOrWhiteSpace(customer.MobileNo))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter Mobile Number.";
                    return Ok(CommonViewModel);
                }

                if (!customer.MobileNo.All(char.IsDigit))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Mobile Number must contain numbers only.";
                    return Ok(CommonViewModel);
                }

                if (customer.MobileNo.Length != 10)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Mobile Number must be exactly 10 digits.";
                    return Ok(CommonViewModel);
                }

                // Alternative Mobile (optional)
                if (!string.IsNullOrWhiteSpace(customer.AlternativeMobileNo))
                {
                    if (!customer.AlternativeMobileNo.All(char.IsDigit))
                    {
                        CommonViewModel.IsSuccess = false;
                        CommonViewModel.Message = "Alternative Mobile Number must contain numbers only.";
                        return Ok(CommonViewModel);
                    }

                    if (customer.AlternativeMobileNo.Length != 10)
                    {
                        CommonViewModel.IsSuccess = false;
                        CommonViewModel.Message = "Alternative Mobile Number must be exactly 10 digits.";
                        return Ok(CommonViewModel);
                    }
                }

                var (IsSuccess, Message, Id, Extra) = await _repository.AddOrUpdateCustomer(customer);

                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }
        [HttpDelete("[Action]")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _repository.RemoveCustomerAddress(id);

                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
                CommonViewModel.Message = Message;
                CommonViewModel.Data = Id;
            }
            catch (Exception ex)
            {
                CommonViewModel.IsSuccess = false;
                CommonViewModel.StatusCode = ResponseStatusCode.Error;
                CommonViewModel.Message = ex.Message;
            }

            return Ok(CommonViewModel);
        }
    }
}