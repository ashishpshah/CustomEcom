using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.AttributeRepository;
using JewelryStore.Areas.Api.ServiceRepository.CategoryRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attribute = JewelryStore.Areas.Admin.Models.Attribute;

namespace JewelryStore.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : ControllerBase
    {
        private readonly IAttibuteRepository _repository;
        private readonly ApiResponseModel CommonViewModel = new();

        public AttributeController(IAttibuteRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("[Action]")]
        public async Task<IActionResult> GetAll(PagingRequest request)
        {
            try
            {
                var data = await _repository.GetAllAttribute(request);

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
                var data = await _repository.GetAttributeById(id);

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
        public async Task<IActionResult> Save([FromBody] Attribute obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Name))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Enter Attribute Name.";
                    return Ok(CommonViewModel);
                }

                if (string.IsNullOrEmpty(obj.Values))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Enter Attribute Values.";
                    return Ok(CommonViewModel);
                }

                var values = obj.Values.Trim();

                // Validation for commas
                if (values.StartsWith(","))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Values cannot start with comma.";
                    return Ok(CommonViewModel);
                }

                if (values.EndsWith(","))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Values cannot end with comma.";
                    return Ok(CommonViewModel);
                }

                if (values.Contains(",,"))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Invalid format. Remove extra commas.";
                    return Ok(CommonViewModel);
                }

                var (IsSuccess, Message, Id, Extra) = await _repository.SaveAttribute(obj);

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
        public async Task<IActionResult> Delete(long id, long operatedBy)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = await _repository.DeleteAttribute(id, operatedBy);

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