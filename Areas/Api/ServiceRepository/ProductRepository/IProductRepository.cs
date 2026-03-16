using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;

namespace JewelryStore.Areas.Api.ServiceRepository.ProductRepository
{
    public interface IProductRepository
    {
        //   Task<List<Product>> GetAllProduct(PagingRequest request);
        Task<object> GetAllProduct(PagingRequest request);

        Task<Product?> GetProductById(int id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveProduct(Product product);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteProduct(long id);
    }
}
