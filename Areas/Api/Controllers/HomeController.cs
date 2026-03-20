using Azure.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Areas.Api.ServiceRepository.HomeRepository;
using JewelryStore.Infra;
using JewelryStore.Infra.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
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
       
    }
	
}
