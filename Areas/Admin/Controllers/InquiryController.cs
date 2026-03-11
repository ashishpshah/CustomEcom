using JewelryStore.Areas.Admin.Models;
using JewelryStore.Controllers;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InquiryController : BaseController<ResponseModel<Inquiries>>
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataList(JqueryDatatableParam param)
        {
            int TotalRecords = 0;
            int FilteredRecords = 0;
            List<Inquiries> result = new List<Inquiries>();

            try
            {
                var oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("@Id", null));
                oParams.Add(new SqlParameter("@Search", param.sSearch));
                oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
                oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
                oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
                oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Inquiry_Get", oParams);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
                        FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
                    }

                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                        foreach (DataRow dr in ds.Tables[1].Rows)
                            result.Add(new Inquiries()
                            {
                                SrNo = GetValue<int>(dr, "SrNo"),
                                Id = GetValue<int>(dr, "Id"),
                                ProductId = GetValue<int>(dr, "ProductId"),
                                ProductName=GetValue<string>(dr, "ProductName"),
                                Inquiry_Date = GetValue<DateTime?>(dr, "Inquiry_Date"),
                                Subject = GetValue<string>(dr, "Subject"),
                                Message = GetValue<string>(dr, "Message"),
                                Status = GetValue<string>(dr, "Status"),
                                Status_Desc = GetValue<string>(dr, "Status_Desc"),
                                IsActive = GetValue<bool>(dr, "IsActive"),
                                LastModifiedDate = GetValue<DateTime?>(dr, "LastModifiedDate")
                            });
                }

            }
            catch (Exception ex) { }

            return Json(new { param.sEcho, iTotalRecords = TotalRecords, iTotalDisplayRecords = FilteredRecords, aaData = result });

        }

        [HttpGet]
        public IActionResult Partial_AddEditForm(int id)
        {
            var obj = new Inquiries() { Inquiry_Date = DateTime.Now };
            var list = new List<SelectListItem_Custom>();

            var oParams = new List<SqlParameter>();

            DataTable dt = null;

            try
            {
                if (id > 0)
                {
                     oParams = new List<SqlParameter>();
                    oParams.Add(new SqlParameter("@Id", id));

                    dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Inquiry_Get", oParams);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var dr = dt.Rows[0];

                        obj = new Inquiries()
                        {
                            Id = GetValue<int>(dr, "Id"),
                            ProductId = GetValue<int>(dr, "ProductId"),
                            ProductName = GetValue<string>(dr, "ProductName"),
                            Inquiry_Date = GetValue<DateTime?>(dr, "Inquiry_Date"),
                            Subject = GetValue<string>(dr, "Subject"),
                            Message = GetValue<string>(dr, "Message"),
                            Status = GetValue<string>(dr, "Status"),
                            Status_Desc = GetValue<string>(dr, "Status_Desc"),
                            IsActive = GetValue<bool>(dr, "IsActive")
                        };
                    }
                }
                dt = new DataTable();
                dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Product_Combo", null, true);

                if (dt != null && dt.Rows.Count > 0)
                    foreach (DataRow dr in dt.Rows)
                        list.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), GetValue<string>(dr, "ProductName"), "PRODUCT", GetValue<int>(dr, "Id")));


            }
            catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

            CommonViewModel.Obj = obj;
            CommonViewModel.SelectListItems = list;
            return PartialView("_Partial_AddEditForm", CommonViewModel);

        }



        [HttpGet]
        public IActionResult Partial_AddEditForm_StatusHistory(int id , string Flag = "")
        {
            List<Inquiries> result = new List<Inquiries>();


            var oParams = new List<SqlParameter>();

            DataTable dt = null;

            try
            {
                if (id > 0)
                {
                    oParams = new List<SqlParameter>();
                    oParams.Add(new SqlParameter("@Id", id));
                    oParams.Add(new SqlParameter("@Flag", Flag));

                    dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Inquiry_History_Get", oParams);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                       
                        foreach (DataRow dr in dt.Rows)
                        {
                            result.Add(new Inquiries()
                            {
                                Id = GetValue<int>(dr, "Id"),
                                InquiryId = GetValue<int>(dr, "InquiryId"),
                                OldStatus = GetValue<string>(dr, "OldStatus"),
                                NewStatus = GetValue<string>(dr, "NewStatus"),
                                OldStatus_Desc = GetValue<string>(dr, "OldStatus_Desc"),
                                NewStatus_Desc = GetValue<string>(dr, "NewStatus_Desc"),
                                Remarks = GetValue<string>(dr, "Remarks"),
                                IsActive = GetValue<bool>(dr, "IsActive"),
                                CreatedDate = GetValue<DateTime>(dr, "CreatedDate")
                            });
                        }
                           
                    }
                }
              
            }
            catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

            CommonViewModel.ObjList = result;
            
            return PartialView("_Partial_AddEditForm_StatusHistory", CommonViewModel);

        }


        [HttpGet]
        public IActionResult Partial_AddEditForm_RepliedHistory(int id, string Flag = "")
        {
            List<Inquiries> result = new List<Inquiries>();


            var oParams = new List<SqlParameter>();

            DataTable dt = null;

            try
            {
                if (id > 0)
                {
                    oParams = new List<SqlParameter>();
                    oParams.Add(new SqlParameter("@Id", id));
                    oParams.Add(new SqlParameter("@Flag", Flag));

                    dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Inquiry_History_Get", oParams);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            result.Add(new Inquiries()
                            {
                                Id = GetValue<int>(dr, "Id"),
                                InquiryId = GetValue<int>(dr, "InquiryId"),
                                ReplyBy = GetValue<string>(dr, "ReplyBy"),
                                ReplyBy_Text = GetValue<string>(dr, "ReplyBy_Text"),
                                Remarks = GetValue<string>(dr, "ReplyMessage"),
                                IsActive = GetValue<bool>(dr, "IsActive"),
                                ReplyDate = GetValue<DateTime>(dr, "ReplyDate")
                            });
                        }
                    }
                }

            }
            catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

            CommonViewModel.ObjList = result;

            return PartialView("_Partial_AddEditForm_RepliedHistory", CommonViewModel);

        }

        [HttpPost]
        public JsonResult Save(Inquiries viewModel)
        {
            try
            {
                // Required field validation
                //if (viewModel.ProductId == 0)
                //{
                //    CommonViewModel.IsSuccess = false;

                //    CommonViewModel.Message = "Please Select Product.";
                //    return Json(CommonViewModel);
                //}

                if (viewModel.Inquiry_Date == null)
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Select Inquiry_Date.";
                    return Json(CommonViewModel);
                }
                if (string.IsNullOrEmpty(viewModel.Subject))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Enter Subject.";
                    return Json(CommonViewModel);
                }

                if (string.IsNullOrEmpty(viewModel.Message))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Enter Message.";
                    return Json(CommonViewModel);
                }
                var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", viewModel.Id));
                oParams.Add(new SqlParameter("ProductId", viewModel.ProductId));
                oParams.Add(new SqlParameter("Subject", viewModel.Subject));
                oParams.Add(new SqlParameter("Message", viewModel.Message));
                oParams.Add(new SqlParameter("Inquiry_Date", viewModel.Inquiry_Date));
                oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));
                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

                (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Inquiry_Save", oParams, true);

                CommonViewModel.IsConfirm = true;
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.Message = Message;

                CommonViewModel.RedirectURL = IsSuccess ? Url.Content("~/") + GetCurrentControllerUrl() + "/Index" : "";

            }
            catch (Exception ex)
            {
                LogService.LogInsert(GetCurrentAction(), "", ex);

                CommonViewModel.IsSuccess = false;
                CommonViewModel.Message = ResponseStatusMessage.Error + " | " + ex.Message;
            }

            return Json(CommonViewModel);
        }

        [HttpPost]
        public JsonResult Change_Status(int InquiryId, string Status = "", string Remarks = "")
        {
            try
            {
               

               
                var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", InquiryId));
                oParams.Add(new SqlParameter("@Status", Status));
                oParams.Add(new SqlParameter("Remarks", Remarks));
               oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

                (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Inquiry_Status_Change", oParams, true);

                CommonViewModel.IsConfirm = true;
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.Message = Message;

                CommonViewModel.RedirectURL = IsSuccess ? Url.Content("~/") + GetCurrentControllerUrl() + "/Index" : "";

            }
            catch (Exception ex)
            {
                LogService.LogInsert(GetCurrentAction(), "", ex);

                CommonViewModel.IsSuccess = false;
                CommonViewModel.Message = ResponseStatusMessage.Error + " | " + ex.Message;
            }

            return Json(CommonViewModel);
        }
        [HttpPost]
        public JsonResult DeleteConfirmed(long id = 0)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", id));

                oParams.Add(new SqlParameter("Mode", "DELETE"));
                oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

                (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Inquiry_Save", oParams, true);

                CommonViewModel.IsConfirm = true;
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.Message = Message;

                CommonViewModel.RedirectURL = IsSuccess ? Url.Content("~/") + GetCurrentControllerUrl() + "/Index" : "";

            }
            catch (Exception ex)
            {
                LogService.LogInsert(GetCurrentAction(), "", ex);
                CommonViewModel.IsSuccess = false;
                CommonViewModel.Message = ResponseStatusMessage.Error + " | " + ex.Message;
            }

            return Json(CommonViewModel);
        }
    }
}
