using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.SimilarProductRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using Attribute = JewelryStore.Areas.Admin.Models.Attribute;

namespace JewelryStore.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimilarProductController : ControllerBase
    {
        private readonly ISimilarProductRepository _repository;
        private readonly ApiResponseModel CommonViewModel = new();

        public SimilarProductController(ISimilarProductRepository repository)
        {
            _repository = repository;
        }
        [HttpPost("[Action]")]
        public async Task<IActionResult> GetSimilarProduct(int ProductId = 0, int TopCount = 0)
        {
            try
            {
                var data = await _repository.GetSimilarProduct(ProductId, TopCount);

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
