

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Infra;

namespace JewelryStore.Areas.Api.ServiceRepository.CartRepository
{
    public interface ICartRepository
    {
        Task<(bool IsSuccess, string Message, long Id, List<Dictionary<string, object>> Data)> SaveCart(CartRequest obj);
        Task<(bool IsSuccess, long Id)> RemoveCart(CartRequest obj);
        Task<object> ApplyOffersToCart(OfferRequest obj);
        Task<(bool IsSuccess, object Data)> Checkout(int customerId, int createdBy);
    }
}
