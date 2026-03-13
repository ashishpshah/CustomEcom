

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;
using Attribute = JewelryStore.Areas.Admin.Models.Attribute;

namespace JewelryStore.Areas.Api.ServiceRepository.AttributeRepository
{
    public interface IAttibuteRepository
    {
        Task<object> GetAllAttribute(PagingRequest request);

        Task<Attribute?> GetAttributeById(int id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveAttribute(Attribute obj);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteAttribute(long id, long operatedBy);
    }
}
