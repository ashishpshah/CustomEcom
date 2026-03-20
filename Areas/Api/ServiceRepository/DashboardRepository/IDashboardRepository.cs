
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Infra;
namespace JewelryStore.Areas.Api.ServiceRepository.DashboardRepository
{
    public interface IDashboardRepository  
    {
        Task<object> GetCustomerDashboard();
        Task<ChangePasswordResult?> ChangePassword(ChangePasswordRequest request);
    }
}
