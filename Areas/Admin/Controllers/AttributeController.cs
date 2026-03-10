using JewelryStore.Areas.Admin.Models;
using JewelryStore.Controllers;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AttributeController : BaseController<ResponseModel<AttributeMaster>>
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataList(JqueryDatatableParam param)
        {
            int TotalRecords = 0;
            int FilteredRecords = 0;
            List<AttributeMaster> result = new List<AttributeMaster>();

            try
            {
                var oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("@Id", null));
                oParams.Add(new SqlParameter("@Search", param.sSearch));
                oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
                oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
                oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
                oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Attribute_Get", oParams);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
                        FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
                    }

                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                        foreach (DataRow dr in ds.Tables[1].Rows)
                            result.Add(new AttributeMaster()
                            {
                                SrNo = GetValue<int>(dr, "SrNo"),
                                Id = GetValue<int>(dr, "Id"),
                                AttributeName = GetValue<string>(dr, "AttributeName"),
                                AttributeValues = GetValue<string>(dr, "AttributeValues"),
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
            var obj = new AttributeMaster();
            DataTable dt = null;

            try
            {
                if (id > 0)
                {
                    var oParams = new List<SqlParameter>();
                    oParams.Add(new SqlParameter("@Id", id));

                    dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Attribute_Get", oParams);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var dr = dt.Rows[0];

                        obj = new AttributeMaster()
                        {
                            Id = GetValue<int>(dr, "Id"),
                            AttributeName = GetValue<string>(dr, "AttributeName"),
                            AttributeValues = GetValue<string>(dr, "AttributeValues"),
                            IsActive = GetValue<bool>(dr, "IsActive")
                        };
                    }
                }

            }
            catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

            CommonViewModel.Obj = obj;

            return PartialView("_Partial_AddEditForm", CommonViewModel);

        }

        [HttpPost]
        public JsonResult Save(AttributeMaster viewModel)
        {
            try
            {
                // Required field validation
                if (string.IsNullOrEmpty(viewModel.AttributeName))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Enter Attribute Name.";
                    return Json(CommonViewModel);
                }

                if (string.IsNullOrEmpty(viewModel.AttributeValues))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Please Enter Attribute Values.";
                    return Json(CommonViewModel);
                }

                var values = viewModel.AttributeValues.Trim();

                // Validation for commas
                if (values.StartsWith(","))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Values cannot start with comma.";
                    return Json(CommonViewModel);
                }

                if (values.EndsWith(","))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Values cannot end with comma.";
                    return Json(CommonViewModel);
                }

                if (values.Contains(",,"))
                {
                    CommonViewModel.IsSuccess = false;

                    CommonViewModel.Message = "Invalid format. Remove extra commas.";
                    return Json(CommonViewModel);
                }
                var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", viewModel.Id));
                oParams.Add(new SqlParameter("AttributeName", viewModel.AttributeName));
                oParams.Add(new SqlParameter("AttributeValues", viewModel.AttributeValues));
                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

                (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Attribute_Save", oParams, true);

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

                (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Attribute_Save", oParams, true);

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
