

using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;


namespace JewelryStore.Areas.Api.ServiceRepository.InquiryRepository
{
    public interface IInquiryRepository
    {
        Task<object> GetAllInquires(PagingRequest request);

        Task<Inquiries?> GetInquiryById(int id);
        Task<List<Inquiries?>> GetInquiryStatusHistory(int id);
        Task<List<Inquiries?>> GetInquiryRepliedHistory(int id);

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveInquiry(Inquiries obj);
        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> ChangeStatus(int InquiryId, string Status = "", string Remarks = "");

        Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteInquiries(long id);
    }
}
