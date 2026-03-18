using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Infra;

namespace JewelryStore.Areas.Api.ServiceRepository.HomeRepository
{
    public interface IHomeRepository
    {
        Task<object> GetHomePageComponent();
        Task<List<DropdownModel?>> GetCategory_SubCategory_Dropdown(int ParentId = 0);
        Task<List<CustomerCart?>> ShoppingCartList_Get(int CustomerId = 0);
        Task<LoginResult?> CustomerLogin(LoginRequest request);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> Register(Customer obj);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveCart(CustomerCart obj);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> RemoveCart(long id);
    }
}