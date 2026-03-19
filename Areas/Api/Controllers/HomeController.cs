using Azure.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Areas.Api.ServiceRepository.HomeRepository;
using JewelryStore.Areas.Api.ServiceRepository.ProductRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Data;

namespace JewelryStore.Areas.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]


	public class HomeController : ControllerBase
	{
		private readonly IHomeRepository _repository;
		private readonly ApiResponseModel CommonViewModel = new();
		public HomeController(IHomeRepository repository)
		{
			_repository = repository;
		}

		[HttpGet("[Action]")]
		public async Task<IActionResult> GetDropdown(int ParentId = 0)
		{
			try
			{
				var data = await _repository.GetCategory_SubCategory_Dropdown(ParentId);
				var Password = Common.Encrypt("12345");
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
					CommonViewModel.Message = "Category not found";
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
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			try
			{
				if (string.IsNullOrEmpty(request.Username))
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.Error;
					CommonViewModel.Message = "User Name is required";

					return Ok(CommonViewModel);
				}
				if (string.IsNullOrEmpty(request.Password))
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.Error;
					CommonViewModel.Message = "Password is required";

					return Ok(CommonViewModel);
				}
				var data = await _repository.CustomerLogin(request);

				if (data != null && data.Status == 1)
				{
					CommonViewModel.IsSuccess = true;
					CommonViewModel.StatusCode = ResponseStatusCode.Success;
					CommonViewModel.Message = data.Message;
					CommonViewModel.Data = data.Data;
				}
				else
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
					CommonViewModel.Message = data?.Message ?? "Invalid Email or Mobile No";
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
		public async Task<IActionResult> Register([FromBody] Customer obj)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(obj.FirstName))
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.Message = "Please enter First Name.";
					return Ok(CommonViewModel);
				}
				if (string.IsNullOrWhiteSpace(obj.LastName))
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.Error;
					CommonViewModel.Message = "Please enter Last Name.";
					return Ok(CommonViewModel);
				}
				if (string.IsNullOrWhiteSpace(obj.Email) && string.IsNullOrWhiteSpace(obj.MobileNo))
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.Error;
					CommonViewModel.Message = "Please enter Email or Mobile Number.";
					return Ok(CommonViewModel);
				}
				if (string.IsNullOrEmpty(obj.Password))
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.Error;
					CommonViewModel.Message = "Password is required";

					return Ok(CommonViewModel);
				}
				var (IsSuccess, Message, Id, Extra) = await _repository.Register(obj);

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
        [HttpPost("[Action]")]
        public async Task<IActionResult> SaveCart([FromBody] CustomerCart obj)
        {
            try
            {
               

                var (IsSuccess, Message, Id, Extra) = await _repository.SaveCart(obj);

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
        public async Task<IActionResult> RemoveCart(long id)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _repository.RemoveCart(id);

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
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetShoppingCartList(int CustomerId = 0)
        {
            try
            {
                var data = await _repository.ShoppingCartList_Get(CustomerId);

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
                    CommonViewModel.Message = "Cart List not found";
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
        public async Task<IActionResult> GetHomeComponent(long id = 0)
        {
            try
            {
                var data = await _repository.GetHomePageComponent(id);

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
        [HttpPost("[Action]")]
        public async Task<IActionResult> ForgotPassword_GenerateOTP([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "Email is required";

                    return Ok(CommonViewModel);
                }
              
                var data = await _repository.ForgotPassword_GenerateOTP(request.Email);

                if (data != null && data.Status == 1)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = data.Message;
                    CommonViewModel.Data = data.Data;
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = data?.Message ?? "Invalid Email";
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
        public async Task<IActionResult> ForgotPassword_VerifyOTP(string Email , int otp)
        {
            try
            {
                if (otp == 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "otp is required";

                    return Ok(CommonViewModel);
                }

                var data = await _repository.ForgotPassword_VerifyOTP(Email , otp);

                if (data != null && data.Status == 1)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = data.Message;
                    CommonViewModel.Data = data.Data;
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = data?.Message ?? "Invalid otp";
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
        public async Task<IActionResult> ForgotPassword_ResetPassword(string Email, string newPassword , string confirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(newPassword))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "New Password is required.";

                    return Ok(CommonViewModel);
                }
                if (string.IsNullOrEmpty(confirmPassword))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "Confirm Password is required.";

                    return Ok(CommonViewModel);
                }
                if (newPassword != confirmPassword)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "Passwords do not match.";

                    return Ok(CommonViewModel);
                }
                var data = await _repository.ForgotPassword_ResetPassword(Email, confirmPassword);

                if (data != null && data.Status == 1)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = data.Message;
                    CommonViewModel.Data = data.Data;
                }
                else
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.NotFound;
                    CommonViewModel.Message = data?.Message ?? "Invalid otp";
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

    }
	
}
