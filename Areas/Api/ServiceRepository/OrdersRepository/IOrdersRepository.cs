using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;

namespace JewelryStore.Areas.Api.ServiceRepository.OrdersRepository
{
    public interface IOrdersRepository
    {
        //   Task<List<Product>> GetAllProduct(PagingRequest request);
        Task<object> GetAllOrders(PagingRequest request);

        Task<Orders?> GetOrderById(int id);
        Task<Orders?> GetOrderNumber();

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveOrder(Orders product);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteOrder(long id);
    }

}
