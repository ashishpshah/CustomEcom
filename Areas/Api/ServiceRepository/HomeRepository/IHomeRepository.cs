using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Infra;

namespace JewelryStore.Areas.Api.ServiceRepository.HomeRepository
{
    public interface IHomeRepository
    {
        Task<List<DropdownModel?>> GetCategory_SubCategory_Dropdown(int ParentId = 0);
        Task<LoginResult?> CustomerLogin(LoginRequest request);
    }
}