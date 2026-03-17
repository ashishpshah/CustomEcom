using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
	public class ProductStockController : BaseController<ResponseModel<ProductStock>>
	{
		public IActionResult Index()
		{
			return View();
		}

		public ActionResult GetDataList(JqueryDatatableParam param)
		{
			int _TotalRecords = 0;
			int FilteredRecords = 0;
			List<ProductStock> result = new List<ProductStock>();

			try
			{
				var oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("@ProductId", null));
				oParams.Add(new SqlParameter("@Search", param.sSearch));
				oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
				oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
				oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
				oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

				DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_ProductStock_Get", oParams);

				if (ds != null && ds.Tables.Count > 0)
				{
					if (ds.Tables[0].Rows.Count > 0)
					{
						_TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
						FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
					}

					if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[1].Rows)
							result.Add(new ProductStock()
							{
								SrNo = GetValue<int>(dr, "SrNo"),
								Id = GetValue<int>(dr, "Id"),
								ProductId = GetValue<int>(dr, "ProductId"),
								VariantId = GetValue<int>(dr, "VariantId"),
								ProductName = GetValue<string>(dr, "ProductName"),
								Price = GetValue<decimal>(dr, "Price"),
								SKU = GetValue<string>(dr, "SKU"),
								VariantAttributes = GetValue<string>(dr, "VariantAttributes"),
								AvailableStock = GetValue<int>(dr, "AvailableStock"),
								ReservedStock = GetValue<int>(dr, "ReservedStock"),
								TotalStock = GetValue<int>(dr, "TotalStock"),
								LastModifiedDate = GetValue<DateTime?>(dr, "LastModifiedDate")
							});
				}

			}
			catch (Exception ex) { }

			return Json(new { param.sEcho, iTotalRecords = _TotalRecords, iTotalDisplayRecords = FilteredRecords, aaData = result });

		}


		[HttpGet]
		public IActionResult Partial_AddEditForm(int id)
		{
			var obj = new ProductStock();

			var list = new List<SelectListItem_Custom>();

			var oParams = new List<SqlParameter>();

			DataTable dt = null;

			try
			{
				if (id > 0)
				{
					oParams = new List<SqlParameter>();
					oParams.Add(new SqlParameter("@ProductId", id));

					dt = DataContext.ExecuteStoredProcedure_DataTable("SP_ProductStock_Get", oParams);

					if (dt != null && dt.Rows.Count > 0)
					{
						var dr = dt.Rows[0];

						obj = new ProductStock()
						{
							Id = GetValue<int>(dr, "Id"),
							Price = GetValue<decimal>(dr, "Price"),
							SKU = GetValue<string>(dr, "SKU"),
							VariantAttributes = GetValue<string>(dr, "VariantAttributes"),
							AvailableStock = GetValue<int>(dr, "AvailableStock"),
							ReservedStock = GetValue<int>(dr, "ReservedStock"),
							TotalStock = GetValue<int>(dr, "TotalStock"),
						};
					}
				}

				oParams = new List<SqlParameter>();
				oParams.Add(new SqlParameter("@Id", -1));

				var ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Product_Get", oParams);

				if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
					foreach (DataRow dr in ds.Tables[0].Rows)
						list.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), GetValue<string>(dr, "ProductName")));

			}
			catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

			CommonViewModel.Obj = obj;
			CommonViewModel.SelectListItems = list;

			return PartialView("_Partial_AddEditForm", CommonViewModel);

		}


		//[HttpPost]
		//public async Task<JsonResult> Save(ProductStock viewModel)
		//{
		//	try
		//	{
		//		var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

		//		if (Request.Form.Files.Count > 0)
		//		{
		//			var file = Request.Form.Files[0];

		//			string uploadFolder = Path.Combine(AppHttpContextAccessor.WebRootPath, "Uploads", "ProductStock");

		//			string imagePath = await FileUploadService.UploadImageAsync(file, uploadFolder);

		//			viewModel.ImagePath = imagePath;
		//		}

		//		List<SqlParameter> oParams = new List<SqlParameter>();

		//		oParams.Add(new SqlParameter("Id", viewModel.Id));
		//		oParams.Add(new SqlParameter("ProductStockName", viewModel.ProductStockName));
		//		oParams.Add(new SqlParameter("ParentProductStockId", viewModel.ParentProductStockId));
		//		oParams.Add(new SqlParameter("ImagePath", viewModel.ImagePath));
		//		oParams.Add(new SqlParameter("ImagePath_Remove", viewModel.ImagePath_Remove));
		//		oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));

		//		oParams.Add(new SqlParameter("Mode", "SAVE"));
		//		oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

		//		(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_ProductStock_Save", oParams, true);

		//		CommonViewModel.IsConfirm = true;
		//		CommonViewModel.IsSuccess = IsSuccess;
		//		CommonViewModel.Message = Message;

		//		if (Extra != null) foreach (var path in Extra) FileUploadService.DeleteOldImage(path);

		//		CommonViewModel.RedirectURL = IsSuccess ? Url.Content("~/") + GetCurrentControllerUrl() + "/Index" : "";

		//	}
		//	catch (Exception ex)
		//	{
		//		LogService.LogInsert(GetCurrentAction(), "", ex);

		//		CommonViewModel.IsSuccess = false;
		//		CommonViewModel.Message = ResponseStatusMessage.Error + " | " + ex.Message;
		//	}

		//	return Json(CommonViewModel);
		//}

		//[HttpPost]
		//public JsonResult DeleteConfirmed(long id = 0)
		//{
		//	try
		//	{
		//		var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

		//		List<SqlParameter> oParams = new List<SqlParameter>();

		//		oParams.Add(new SqlParameter("Id", id));

		//		oParams.Add(new SqlParameter("Mode", "DELETE"));
		//		oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

		//		(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_ProductStock_Save", oParams, true);

		//		CommonViewModel.IsConfirm = true;
		//		CommonViewModel.IsSuccess = IsSuccess;
		//		CommonViewModel.Message = Message;

		//		CommonViewModel.RedirectURL = IsSuccess ? Url.Content("~/") + GetCurrentControllerUrl() + "/Index" : "";

		//	}
		//	catch (Exception ex)
		//	{
		//		LogService.LogInsert(GetCurrentAction(), "", ex);
		//		CommonViewModel.IsSuccess = false;
		//		CommonViewModel.Message = ResponseStatusMessage.Error + " | " + ex.Message;
		//	}

		//	return Json(CommonViewModel);
		//}

		public JsonResult GetVariants(int productId)
		{
			List<ProductVariantMapping> list = new List<ProductVariantMapping>();

			List<SqlParameter> oParams = new List<SqlParameter>();
			oParams.Add(new SqlParameter("Id", productId));

			var ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Product_Get", oParams);

			if (ds != null && ds.Tables.Count > 2)
				foreach (DataRow dr in ds.Tables[2].Rows)
				{
					list.Add(new ProductVariantMapping()
					{
						Id = GetValue<int>(dr, "Id"),
						Price = GetValue<decimal>(dr, "Price"),
						SKU = GetValue<string>(dr, "SKU"),
						VariantAttributes = GetValue<string>(dr, "VariantAttributes"),
						AvailableStock = GetValue<int>(dr, "AvailableStock"),
						ReservedStock = GetValue<int>(dr, "ReservedStock"),
						TotalStock = GetValue<int>(dr, "TotalStock"),
					});
				}

			return Json(list);
		}

		public JsonResult GetStockHistory(JqueryDatatableParam param)
		{
			var ProductId = Convert.ToInt32(HttpContext.Request.Query["ProductId"]);
			var VariantId = Convert.ToInt32(HttpContext.Request.Query["VariantId"]);

			int TotalRecords = 0;
			int FilteredRecords = 0;
			List<ProductStock> result = new List<ProductStock>();

			List<ProductStockHistory> historyList = new List<ProductStockHistory>();

			List<SqlParameter> oParams = new List<SqlParameter>();
			oParams.Add(new SqlParameter("@ProductId", ProductId));

			var ds = DataContext.ExecuteStoredProcedure_DataSet("SP_ProductStock_Get", oParams);

			if (ds != null && ds.Tables.Count > 0 && ProductId <= 0)
			{
				if (ds.Tables[0].Rows.Count > 0)
				{
					TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
					FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
				}

				if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
					foreach (DataRow dr in ds.Tables[1].Rows)
						result.Add(new ProductStock()
						{
							Id = GetValue<int>(dr, "Id"),
							ProductId = GetValue<int>(dr, "ProductId"),
							VariantId = GetValue<int>(dr, "VariantId"),
							ProductName = GetValue<string>(dr, "ProductName"),
							VariantAttributes = GetValue<string>(dr, "VariantAttributes"),
							SKU = GetValue<string>(dr, "SKU"),
							Price = GetValue<decimal>(dr, "Price"),
							AvailableStock = GetValue<int>(dr, "AvailableStock"),
							ReservedStock = GetValue<int>(dr, "ReservedStock"),
							TotalStock = GetValue<int>(dr, "TotalStock")
						});
			}

			if (ds != null && ds.Tables.Count > 0 && ds.Tables.Count > 1)
			{
				int sr = 1;

				foreach (DataRow dr in ds.Tables[1].Rows)
				{
					historyList.Add(new ProductStockHistory()
					{
						SrNo = sr++,
						ChangeType = GetValue<string>(dr, "ChangeType"),
						Quantity = GetValue<int>(dr, "Quantity"),
						ReferenceType = GetValue<string>(dr, "ReferenceType"),
						Remarks = GetValue<string>(dr, "Remarks"),
						LastModifiedDate = GetValue<DateTime?>(dr, "LastModifiedDate")
					});
				}
			}

			if (ProductId > 0) return Json(new { product = result, history = historyList.OrderByDescending(x => x.LastModifiedDate.Value.Ticks) });
			else return Json(new { param.sEcho, iTotalRecords = TotalRecords, iTotalDisplayRecords = FilteredRecords, aaData = result });
		}

		[HttpPost]
		public JsonResult AdjustStock(ProductStockHistory viewModel)
		{
			try
			{
				var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

				List<SqlParameter> oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("@ProductId", viewModel.ProductId));
				oParams.Add(new SqlParameter("@VariantIds", viewModel.VariantIds));
				oParams.Add(new SqlParameter("@ChangeType", viewModel.ChangeType));
				oParams.Add(new SqlParameter("@Quantity", viewModel.Quantity));
				oParams.Add(new SqlParameter("@Remarks", viewModel.Remarks));

				oParams.Add(new SqlParameter("@OperatedBy", Logged_In_UserId));

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_ProductStock_Adjust", oParams, true);

				CommonViewModel.IsConfirm = true;
				CommonViewModel.IsSuccess = IsSuccess;
				CommonViewModel.Message = Message;

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
