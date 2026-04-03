

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;

namespace JewelryStore.Areas.Api.ServiceRepository.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<object> GetAllCustomer(ReviewPagingRequest request);

        Task<Customer?> GetCustomerById(int id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateCustomer(Customer customer);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> RemoveCustomerAddress(long id);
        //Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveCategory(Category category);

        //Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteCategory(long id);
    }
}
