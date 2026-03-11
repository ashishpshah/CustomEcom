

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;

namespace JewelryStore.Areas.Api.ServiceRepository.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategory(PagingRequest request);

        Task<Category?> GetCategoryById(int id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveCategory(Category category);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteCategory(long id, long operatedBy);
    }
}
