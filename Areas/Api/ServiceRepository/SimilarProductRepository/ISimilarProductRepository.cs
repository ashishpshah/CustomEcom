

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;


namespace JewelryStore.Areas.Api.ServiceRepository.SimilarProductRepository
{
    public interface ISimilarProductRepository
    {
        Task<object> GetSimilarProduct(int ProductId = 0, int TopCount = 0);

       
    }
}
