using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.CouponRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attribute = JewelryStore.Areas.Admin.Models.Attribute;

namespace JewelryStore.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _repository;
        private readonly ApiResponseModel CommonViewModel = new();

        public CouponController(ICouponRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> GetAll(PagingRequest request)
        {
            try
            {
                var data = await _repository.GetAllCoupon(request);

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
                var data = await _repository.GetCouponById(id);

                if (data != null)
                {
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = "Data retrieved successfully";
                    CommonViewModel.Data = new
                    {
                        data.Id,
                        data.CouponCode,
                        data.DiscountType,
                        data.DiscountValue,
                        data.MinimumOrderAmount,
                        ExpiryDate = data.ExpiryDate?.ToString("dd-MMM-yyyy"),
                        data.UsageLimit,
                        data.IsActive
                    };
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
        public async Task<IActionResult> Save([FromBody] Coupons obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.CouponCode))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter coupon code.";
                    return Ok(CommonViewModel);
                }
                if (obj.MinimumOrderAmount <= obj.DiscountValue)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Minimum Order Amount must be greater than Discount Value.";
                    return Ok(CommonViewModel);
                }
                if (obj.ExpiryDate == null || obj.ExpiryDate < DateTime.Today)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Expiry Date cannot be previous date.";
                    return Ok(CommonViewModel);
                }

               

                var (IsSuccess, Message, Id, Extra) = await _repository.SaveCoupon(obj);

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
                var (IsSuccess, Message, Id, Extra) = await _repository.DeleteCoupon(id);

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