using JewelryStore.Areas.Admin.Models;
using JewelryStore.Controllers;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReviewsController : BaseController<ResponseModel<Reviews>>
    {
        public IActionResult Index()
        {
            return View();
        }

        //--------------------------------------------------

        public ActionResult GetDataList(JqueryDatatableParam param)
        {
            int TotalRecords = 0;
            int FilteredRecords = 0;

            List<Reviews> result = new List<Reviews>();

            try
            {
                var oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("@Id", null));
                oParams.Add(new SqlParameter("@ProductId", null));
                oParams.Add(new SqlParameter("@Search", param.sSearch));
                oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
                oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
                oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
                oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Reviews_Get", oParams);

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
                            result.Add(new Reviews()
                            {
                                SrNo = GetValue<int>(dr, "SrNo"),
                                Id = GetValue<int>(dr, "Id"),
                                ProductName = GetValue<string>(dr, "ProductName"),
                                UserName = GetValue<string>(dr, "UserName"),
                                Rating = GetValue<int?>(dr, "Rating"),
                                ReviewText = GetValue<string>(dr, "ReviewText"),
                                IsApproved = GetValue<bool>(dr, "IsApproved"),
                                IsActive = GetValue<bool>(dr, "IsActive"),
                                CreatedDate = GetValue<DateTime?>(dr, "CreatedDate")
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
            var obj = new Reviews();
            DataTable dt = null;

            try
            {
                if (id > 0)
                {
                    var oParams = new List<SqlParameter>();

                    oParams.Add(new SqlParameter("@Id", id));

                    dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Reviews_Get", oParams);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var dr = dt.Rows[0];

                        obj = new Reviews()
                        {
                            Id = GetValue<int>(dr, "Id"),
                            ProductId = GetValue<int?>(dr, "ProductId"),
                            UserId = GetValue<int?>(dr, "UserId"),
                            ProductName = GetValue<string>(dr, "ProductName"),
                            UserName = GetValue<string>(dr, "UserName"),
                            Rating = GetValue<int?>(dr, "Rating"),
                            ReviewText = GetValue<string>(dr, "ReviewText"),
                            IsApproved = GetValue<bool>(dr, "IsApproved"),
                            IsActive = GetValue<bool>(dr, "IsActive")
                        };
                    }
                }
                var list = new List<SelectListItem_Custom>();

                // Product Dropdown
                var dtProduct = DataContext.ExecuteStoredProcedure_DataTable("SP_DD_Product", null);

                if (dtProduct != null && dtProduct.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtProduct.Rows)
                    {
                        list.Add(new SelectListItem_Custom(
                            dr["Id"].ToString(),
                            dr["ProductName"].ToString(),
                            "Product"
                        ));
                    }
                }

                // User Dropdown
                var dtUser = DataContext.ExecuteStoredProcedure_DataTable("SP_DD_User", null);

                if (dtUser != null && dtUser.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtUser.Rows)
                    {
                        list.Add(new SelectListItem_Custom(
                            dr["Id"].ToString(),
                            dr["UserName"].ToString(),
                            "User"
                        ));
                    }
                }

                CommonViewModel.SelectListItems = list;
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
        public JsonResult Save(Reviews viewModel)
        {
            try
            {

                if (viewModel.ProductId == null || viewModel.ProductId <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter product.";
                    return Json(CommonViewModel);
                }

                if (viewModel.UserId == null || viewModel.UserId <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter user.";
                    return Json(CommonViewModel);
                }

                if (viewModel.Rating == null || viewModel.Rating <= 0)
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please provide rating.";
                    return Json(CommonViewModel);
                }

                if (string.IsNullOrWhiteSpace(viewModel.ReviewText))
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.Message = "Please enter review text.";
                    return Json(CommonViewModel);
                }

                var (IsSuccess, Message, Id, Extra) =
                    (false, ResponseStatusMessage.Error, 0M, new List<string>());

                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", viewModel.Id));
                oParams.Add(new SqlParameter("ProductId", viewModel.ProductId));
                oParams.Add(new SqlParameter("UserId", viewModel.UserId));
                oParams.Add(new SqlParameter("Rating", viewModel.Rating));
                oParams.Add(new SqlParameter("ReviewText", viewModel.ReviewText));
                oParams.Add(new SqlParameter("IsApproved", (viewModel.IsApproved ?? false) ? 1 : 0));
                //oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));

                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

                (IsSuccess, Message, Id, Extra) =
                    DataContext.ExecuteStoredProcedure("SP_Reviews_Save", oParams, true);

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
                    DataContext.ExecuteStoredProcedure("SP_Reviews_Save", oParams, true);

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
