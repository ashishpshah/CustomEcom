using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.AttributeRepository;
using JewelryStore.Areas.Api.ServiceRepository.CategoryRepository;
using JewelryStore.Areas.Api.ServiceRepository.ReviewsRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attribute = JewelryStore.Areas.Admin.Models.Attribute;

namespace JewelryStore.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewsRepository _repository;
        private readonly ApiResponseModel CommonViewModel = new();

        public ReviewController(IReviewsRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> GetAll(ReviewPagingRequest request)
        {
            try
            {
                var data = await _repository.GetAllReviews(request);

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
                var data = await _repository.GetReviewsById(id);

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
        public async Task<IActionResult> Save([FromBody] Reviews obj)
        {
            try
            {
                if (obj.ProductId == null || obj.ProductId <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter product.";
                    return Ok(CommonViewModel);
                }

                if (obj.UserId == null || obj.UserId <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter user.";
                    return Ok(CommonViewModel);
                }

                if (obj.Rating == null || obj.Rating <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please provide rating.";
                    return Ok(CommonViewModel);
                }

                if (string.IsNullOrWhiteSpace(obj.ReviewText))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter review text.";
                    return Ok(CommonViewModel);
                }


                var (IsSuccess, Message, Id, Extra) = await _repository.SaveReview(obj);

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
                var (IsSuccess, Message, Id, Extra) = await _repository.DeleteReview(id);

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