using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.ProductStockRepository;

using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attribute = JewelryStore.Areas.Admin.Models.Attribute;

namespace JewelryStore.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStockController : ControllerBase
    {
        private readonly IProductStockRepository _repository;
        private readonly ApiResponseModel CommonViewModel = new();

        public ProductStockController(IProductStockRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> GetAll(PagingRequest request)
        {
            try
            {
                var data = await _repository.GetAllProductStock(request);

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
        public async Task<IActionResult> GetStockHistory(JsonParameters request)
        {
            try
            {
                var data = await _repository.GetStockHistory(request);

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
                    CommonViewModel.Message = "Stock history not found";
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
        public async Task<IActionResult> AdjustStock([FromBody] ProductStockHistory obj)
        {
            try
            {
               

                var (IsSuccess, Message, Id, Extra) = await _repository.AdjustStock(obj);

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