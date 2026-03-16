using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Areas.Api.ServiceRepository.CouponRepository;
using JewelryStore.Areas.Api.ServiceRepository.InquiryRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace JewelryStore.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InquiryController : ControllerBase
    {
        private readonly IInquiryRepository _repository;
        private readonly ApiResponseModel CommonViewModel = new();

        public InquiryController(IInquiryRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> GetAll(PagingRequest request)
        {
            try
            {
                var data = await _repository.GetAllInquires(request);

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
                var data = await _repository.GetInquiryById(id);

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
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetInquryStatusHistory(int id)
        {
            try
            {
                var data = await _repository.GetInquiryStatusHistory(id);

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
                    CommonViewModel.Message = "Inquiry Status History not found";
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
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetInquryConversationHistory(int id)
        {
            try
            {
                var data = await _repository.GetInquiryRepliedHistory(id);

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
                    CommonViewModel.Message = "Conversation History not found";
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
        public async Task<IActionResult> Save([FromBody] Inquiries obj)
        {
            try
            {
              
                if (obj.Inquiry_Date == null)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please Select Inquiry_Date.";
                    return Ok(CommonViewModel);
                }
                if (string.IsNullOrEmpty(obj.Subject))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Enter Subject.";
                    return Ok(CommonViewModel);
                }

                if (string.IsNullOrEmpty(obj.Message))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Enter Message.";
                    return Ok(CommonViewModel);
                }



                var (IsSuccess, Message, Id, Extra) = await _repository.SaveInquiry(obj);

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
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeInquiryStatusRequest obj)
        {
            try
            {

                var (IsSuccess, Message, Id, Extra) = await _repository.ChangeStatus(obj.InquiryId,obj.Status,obj.Remarks);

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
                var (IsSuccess, Message, Id, Extra) = await _repository.DeleteInquiries(id);

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