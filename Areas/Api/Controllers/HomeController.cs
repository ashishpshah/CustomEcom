using DocumentFormat.OpenXml.Office2010.Excel;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Areas.Api.ServiceRepository.HomeRepository;
using JewelryStore.Areas.Api.ServiceRepository.ProductRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

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
    }
}
