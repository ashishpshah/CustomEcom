using DocumentFormat.OpenXml.Office2010.Excel;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.HomeRepository;
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


        [HttpGet("[Action]")]
        public async Task<IActionResult> GetCategory_Subcategory_Dropdown(int ParentId = 0)
        {
            try
            {
                var data = await _repository.GetCategory_SubCategory_Dropdown(ParentId);

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
    }
}
