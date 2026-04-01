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

        //[HttpPost("[Action]")]
        //public async Task<IActionResult> Save([FromForm] Category category)
        //{
        //    try
        //    {
        //        if (category.ImageFile != null)
        //        {
        //            string uploadFolder = Path.Combine(AppHttpContextAccessor.WebRootPath, "Uploads", "Category");

        //            string imagePath = await FileUploadService.UploadImageAsync(category.ImageFile, uploadFolder);

        //            category.ImagePath = imagePath;
        //        }

        //        var (IsSuccess, Message, Id, Extra) = await _repository.SaveCategory(category);

        //        CommonViewModel.IsSuccess = IsSuccess;
        //        CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
        //        CommonViewModel.Message = Message;
        //        CommonViewModel.Data = Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonViewModel.IsSuccess = false;
        //        CommonViewModel.StatusCode = ResponseStatusCode.Error;
        //        CommonViewModel.Message = ex.Message;
        //    }

        //    return Ok(CommonViewModel);
        //}

        //[HttpDelete("[Action]")]
        //public async Task<IActionResult> Delete(long id)
        //{
        //    try
        //    {
        //        var (IsSuccess, Message, Id, Extra) = await _repository.DeleteCategory(id);

        //        CommonViewModel.IsSuccess = IsSuccess;
        //        CommonViewModel.StatusCode = IsSuccess ? ResponseStatusCode.Success : ResponseStatusCode.Error;
        //        CommonViewModel.Message = Message;
        //        CommonViewModel.Data = Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonViewModel.IsSuccess = false;
        //        CommonViewModel.StatusCode = ResponseStatusCode.Error;
        //        CommonViewModel.Message = ex.Message;
        //    }

        //    return Ok(CommonViewModel);
        //}
    }
}