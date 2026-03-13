

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;


namespace JewelryStore.Areas.Api.ServiceRepository.CouponRepository
{
    public interface ICouponRepository
    {
        Task<object> GetAllCoupon(PagingRequest request);

        Task<Coupons?> GetCouponById(int id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveCoupon(Coupons obj);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteCoupon(long id);
    }
}
