using JewelryStore.Areas.Admin.Models;
using JewelryStore.Controllers;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponsController : BaseController<ResponseModel<Coupons>>
    {

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataList(JqueryDatatableParam param)
        {
            int TotalRecords = 0;
            int FilteredRecords = 0;

            List<Coupons> result = new List<Coupons>();

            try
            {
                var oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("@Id", null));
                oParams.Add(new SqlParameter("@Search", param.sSearch));
                oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
                oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
                oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
                oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Coupons_Get", oParams);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
                        FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
                    }

                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            result.Add(new Coupons()
                            {
                                SrNo = GetValue<int>(dr, "SrNo"),
                                Id = GetValue<int>(dr, "Id"),
                                CouponCode = GetValue<string>(dr, "CouponCode"),
                                DiscountType = GetValue<string>(dr, "DiscountType"),
                                DiscountValue = GetValue<decimal?>(dr, "DiscountValue"),
                                MinimumOrderAmount = GetValue<decimal?>(dr, "MinimumOrderAmount"),
                                ExpiryDate = GetValue<DateTime?>(dr, "ExpiryDate"),
                                UsageLimit = GetValue<int?>(dr, "UsageLimit"),
                                IsActive = GetValue<bool>(dr, "IsActive"),
                                LastModifiedDate = GetValue<DateTime?>(dr, "LastModifiedDate")
                            });
                        }
                    }
                }

            }
            catch (Exception ex) { }

            return Json(new
            {
                param.sEcho,
                iTotalRecords = TotalRecords,
                iTotalDisplayRecords = FilteredRecords,
                aaData = result
            });
        }

        //--------------------------------------------------

        [HttpGet]
        public IActionResult Partial_AddEditForm(int id)
        {
            var obj = new Coupons();
            DataTable dt = null;

            try
            {
                if (id > 0)
                {
                    var oParams = new List<SqlParameter>();

                    oParams.Add(new SqlParameter("@Id", id));

                    dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Coupons_Get", oParams);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var dr = dt.Rows[0];

                        obj = new Coupons()
                        {
                            Id = GetValue<int>(dr, "Id"),
                            CouponCode = GetValue<string>(dr, "CouponCode"),
                            DiscountType = GetValue<string>(dr, "DiscountType"),
                            DiscountValue = GetValue<decimal?>(dr, "DiscountValue"),
                            MinimumOrderAmount = GetValue<decimal?>(dr, "MinimumOrderAmount"),
                            ExpiryDate = GetValue<DateTime?>(dr, "ExpiryDate"),
                            UsageLimit = GetValue<int?>(dr, "UsageLimit"),
                            IsActive = GetValue<bool>(dr, "IsActive")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogInsert(GetCurrentAction(), "", ex);
            }

            CommonViewModel.Obj = obj;

            return PartialView("_Partial_AddEditForm", CommonViewModel);
        }

        //--------------------------------------------------

        [HttpPost]
        public JsonResult Save(Coupons viewModel)
        {
            try
            {

                if (string.IsNullOrEmpty(viewModel.CouponCode))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter coupon code.";
                    return Json(CommonViewModel);
                }
                if (viewModel.MinimumOrderAmount <= viewModel.DiscountValue)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Minimum Order Amount must be greater than Discount Value.";
                    return Json(CommonViewModel);
                }
                if (viewModel.ExpiryDate == null || viewModel.ExpiryDate < DateTime.Today)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Expiry Date cannot be past date.";
                    return Json(CommonViewModel);
                }
                var (IsSuccess, Message, Id, Extra) =
                    (false, ResponseStatusMessage.Error, 0M, new List<string>());

                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", viewModel.Id));
                oParams.Add(new SqlParameter("CouponCode", viewModel.CouponCode));
                oParams.Add(new SqlParameter("DiscountType", viewModel.DiscountType));
                oParams.Add(new SqlParameter("DiscountValue", viewModel.DiscountValue));
                oParams.Add(new SqlParameter("MinimumOrderAmount", viewModel.MinimumOrderAmount));
                oParams.Add(new SqlParameter("ExpiryDate", viewModel.ExpiryDate));
                oParams.Add(new SqlParameter("UsageLimit", viewModel.UsageLimit));
                oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));

                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

                (IsSuccess, Message, Id, Extra) =
                    DataContext.ExecuteStoredProcedure("SP_Coupons_Save", oParams, true);

                CommonViewModel.IsConfirm = true;
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.Message = Message;

                CommonViewModel.RedirectURL =
                    IsSuccess ? Url.Content("~/") + GetCurrentControllerUrl() + "/Index" : "";
            }
            catch (Exception ex)
            {
                LogService.LogInsert(GetCurrentAction(), "", ex);

                CommonViewModel.IsSuccess = false;
                CommonViewModel.Message = ex.Message;
            }

            return Json(CommonViewModel);
        }

        //--------------------------------------------------

        [HttpPost]
        public JsonResult DeleteConfirmed(long id = 0)
        {
            try
            {
                var (IsSuccess, Message, Id, Extra) =
                    (false, ResponseStatusMessage.Error, 0M, new List<string>());

                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", id));
                oParams.Add(new SqlParameter("Mode", "DELETE"));
                oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

                (IsSuccess, Message, Id, Extra) =
                    DataContext.ExecuteStoredProcedure("SP_Coupons_Save", oParams, true);

                CommonViewModel.IsConfirm = true;
                CommonViewModel.IsSuccess = IsSuccess;
                CommonViewModel.Message = Message;

                CommonViewModel.RedirectURL =
                    IsSuccess ? Url.Content("~/") + GetCurrentControllerUrl() + "/Index" : "";
            }
            catch (Exception ex)
            {
                LogService.LogInsert(GetCurrentAction(), "", ex);

                CommonViewModel.IsSuccess = false;
                CommonViewModel.Message = ex.Message;
            }

            return Json(CommonViewModel);
        }
    }
}
