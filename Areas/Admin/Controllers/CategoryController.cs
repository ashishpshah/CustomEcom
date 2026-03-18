using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Controllers;
using JewelryStore.Infra;
using JewelryStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : BaseController<ResponseModel<Category>>
	{
		public IActionResult Index()
		{
			return View();
		}

		public ActionResult GetDataList(JqueryDatatableParam param)
		{
			int TotalRecords = 0;
			int FilteredRecords = 0;
			List<Category> result = new List<Category>();

			try
			{
				var oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("@Id", null));
				oParams.Add(new SqlParameter("@Search", param.sSearch));
				oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
				oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
				oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
				oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

				DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Category_Get", oParams);

				if (ds != null && ds.Tables.Count > 0)
				{
					if (ds.Tables[0].Rows.Count > 0)
					{
						TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
						FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
					}

					if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[1].Rows)
							result.Add(new Category()
							{
								SrNo = GetValue<int>(dr, "SrNo"),
								Id = GetValue<int>(dr, "Id"),
								CategoryName = GetValue<string>(dr, "CategoryName"),
								ParentCategoryId = GetValue<int>(dr, "ParentCategoryId"),
								ParentCategoryName = GetValue<string>(dr, "ParentCategoryName"),
								SKU = GetValue<string>(dr, "SKU"),
								ImagePath = GetValue<string>(dr, "ImagePath"),
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
			var obj = new Category();

			var list = new List<SelectListItem_Custom>();

			var oParams = new List<SqlParameter>();

			DataTable dt = null;

			try
			{
				if (id > 0)
				{
					oParams = new List<SqlParameter>();
					oParams.Add(new SqlParameter("@Id", id));

					dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Category_Get", oParams);

					if (dt != null && dt.Rows.Count > 0)
					{
						var dr = dt.Rows[0];

						obj = new Category()
						{
							Id = GetValue<int>(dr, "Id"),
							CategoryName = GetValue<string>(dr, "CategoryName"),
							ParentCategoryId = GetValue<int>(dr, "ParentCategoryId"),
							ParentCategoryName = GetValue<string>(dr, "ParentCategoryName"),
							SKU = GetValue<string>(dr, "SKU"),
							ImagePath = GetValue<string>(dr, "ImagePath"),
							IsActive = GetValue<bool>(dr, "IsActive"),
						};
					}
				}

				oParams = new List<SqlParameter>();
				oParams.Add(new SqlParameter("@Id", -1));

				dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Category_Get", oParams, true);

				if (dt != null && dt.Rows.Count > 0)
					foreach (DataRow dr in dt.Rows)
						list.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), (string.IsNullOrEmpty(GetValue<string>(dr, "ParentCategoryName")) ? GetValue<string>(dr, "CategoryName") : GetValue<string>(dr, "CategoryName") + " - " + GetValue<string>(dr, "ParentCategoryName")), GetValue<string>(dr, "ParentCategoryId"), "CAT", GetValue<int>(dr, "DisplayOrder")));

			}
			catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

			CommonViewModel.Obj = obj;
			CommonViewModel.SelectListItems = list;

			return PartialView("_Partial_AddEditForm", CommonViewModel);

		}


		[HttpPost]
		public async Task<JsonResult> Save(Category viewModel)
		{
			try
			{
				var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

				if (Request.Form.Files.Count > 0)
				{
					var file = Request.Form.Files[0];

					string uploadFolder = Path.Combine(AppHttpContextAccessor.WebRootPath, "Uploads", "Category");

					string imagePath = await FileUploadService.UploadImageAsync(file, uploadFolder);

					viewModel.ImagePath = imagePath;
				}

				List<SqlParameter> oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("Id", viewModel.Id));
				oParams.Add(new SqlParameter("CategoryName", viewModel.CategoryName));
				oParams.Add(new SqlParameter("ParentCategoryId", viewModel.ParentCategoryId));
				oParams.Add(new SqlParameter("SKU", viewModel.SKU));
				oParams.Add(new SqlParameter("ImagePath", viewModel.ImagePath));
				oParams.Add(new SqlParameter("ImagePath_Remove", viewModel.ImagePath_Remove));
				oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));

				oParams.Add(new SqlParameter("Mode", "SAVE"));
				oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Category_Save", oParams, true);

				CommonViewModel.IsConfirm = true;
				CommonViewModel.IsSuccess = IsSuccess;
				CommonViewModel.Message = Message;

				if (Extra != null) foreach (var path in Extra) FileUploadService.DeleteOldImage(path);

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

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Category_Save", oParams, true);

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
		public JsonResult GenerateSKU(int categoryId, string categoryName, int? parentCategoryId)
		{
			string sku = "";

			List<SqlParameter> oParams = new List<SqlParameter>();

			oParams.Add(new SqlParameter("CategoryId", categoryId));
			oParams.Add(new SqlParameter("CategoryName", categoryName));
			oParams.Add(new SqlParameter("ParentCategoryId", parentCategoryId));

			sku = DataContext.ExecuteStoredProcedure("SP_CategorySKU_GenerateUnique", oParams);

			return Json(new { sku = sku });
		}
	}
}
