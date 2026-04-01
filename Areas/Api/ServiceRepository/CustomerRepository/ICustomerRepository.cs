

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;

namespace JewelryStore.Areas.Api.ServiceRepository.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<object> GetAllCustomer(PagingRequest request);

        Task<Customer?> GetCustomerById(int id);

        //Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveCategory(Category category);

        //Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteCategory(long id);
    }
}
