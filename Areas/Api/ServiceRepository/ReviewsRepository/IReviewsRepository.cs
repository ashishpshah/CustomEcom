

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;


namespace JewelryStore.Areas.Api.ServiceRepository.ReviewsRepository
{
    public interface IReviewsRepository
    {
        Task<object> GetAllReviews(PagingRequest request);

        Task<Reviews?> GetReviewsById(int id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveReview(Reviews obj);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteReview(long id);
    }
}
