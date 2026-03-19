using DocumentFormat.OpenXml.Spreadsheet;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Areas.Api.ServiceRepository.HomeRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JewelryStore.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHomeRepository _repository;
        private readonly ApiResponseModel CommonViewModel = new();
        public AuthController(IHomeRepository repository)
        {
            _repository = repository;
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
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                        JsonConvert.SerializeObject(data.Data)
                    );

                    int customerId = Convert.ToInt32(dict["Id"]);
                    var token = GenerateJwtToken(customerId);
                    var message2 = $"{data.Message}|{token}";
                    CommonViewModel.Message = message2;
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
        private string GenerateJwtToken(int customerId)
        {
            var jwtSettings = AppHttpContextAccessor.AppConfiguration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customerId.ToString()),
               
             };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["DurationInDays"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
                    var dict = data.Data as IDictionary<string, object>;
                    var newOtp = dict?["OTP"]?.ToString();

                    Common.SendEmail(
                        "Your OTP Code",
                        request.Email,
                        true,
                        "",
                        "otp_message",
                        JsonConvert.SerializeObject(new { otp = newOtp })
                    );
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
        public async Task<IActionResult> ForgotPassword_VerifyOTP([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (request.Otp == 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "otp is required";

                    return Ok(CommonViewModel);
                }

                var data = await _repository.ForgotPassword_VerifyOTP(request.Email, request.Otp);

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
        public async Task<IActionResult> ForgotPassword_ResetPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.NewPassword))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "New Password is required.";

                    return Ok(CommonViewModel);
                }
                if (string.IsNullOrEmpty(request.ConfirmPassword))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "Confirm Password is required.";

                    return Ok(CommonViewModel);
                }
                if (request.NewPassword != request.ConfirmPassword)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = "Passwords do not match.";

                    return Ok(CommonViewModel);
                }
                var data = await _repository.ForgotPassword_ResetPassword(request.Email, request.NewPassword);

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
