

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;

namespace JewelryStore.Areas.Api.ServiceRepository.ProductStockRepository
{
    public interface IProductStockRepository
    {
        Task<object> GetAllProductStock(PagingRequest request);

        Task<object> GetStockHistory(JsonParameters request);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AdjustStock(ProductStockHistory obj);

        //Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteProductStock(long id);
    }
}
