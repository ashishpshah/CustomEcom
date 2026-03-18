using JewelryStore.Areas.Admin.Models;
using JewelryStore.Controllers;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace JewelryStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomePageController : BaseController<ResponseModel<HomePageComponent>>
	{
		public IActionResult Index()
		{
			List<HomePageComponent> result = new List<HomePageComponent>();

			try
			{
				var ds = DataContext.ExecuteStoredProcedure_DataSet("SP_HomePageComponent_Get");

				if (ds != null && ds.Tables.Count > 0)
				{
					foreach (DataRow dr in ds.Tables[0].Rows)
					{
						result.Add(new HomePageComponent()
						{
							Id = GetValue<long>(dr, "Id"),
							Name = GetValue<string>(dr, "Name"),
							Key = GetValue<string>(dr, "Key"),
							Type = GetValue<string>(dr, "Type"),
							DisplayOrder = GetValue<int>(dr, "DisplayOrder"),
							IsActive = GetValue<bool>(dr, "IsActive")
						});
					}
				}
			}
			catch { }

			return View(result);
		}

		#region ADD EDIT

		[HttpGet]
		public IActionResult Partial_AddEditForm(long id)
		{
			var obj = new HomePageComponent();

			List<SqlParameter> oParams = new List<SqlParameter>();
			oParams.Add(new SqlParameter("@Id", id));

			var ds = DataContext.ExecuteStoredProcedure_DataSet("SP_HomePageComponent_Get", oParams);

			if (ds != null && ds.Tables.Count > 0)
			{
				if (ds.Tables[0].Rows.Count > 0)
				{
					var dr = ds.Tables[0].Rows[0];

					obj.Id = GetValue<long>(dr, "Id");
					obj.Name = GetValue<string>(dr, "Name");
					obj.Key = GetValue<string>(dr, "Key");
					obj.Type = GetValue<string>(dr, "Type");
					obj.DisplayOrder = GetValue<int>(dr, "DisplayOrder");
					obj.IsActive = GetValue<bool>(dr, "IsActive");
				}

				obj.Items = new List<HomePageComponentItem>();

				if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow row in ds.Tables[1].Rows)
					{
						obj.Items.Add(new HomePageComponentItem
						{
							Id = GetValue<long>(row, "Id"),
							Title = GetValue<string>(row, "Title"),
							Subtitle = GetValue<string>(row, "Subtitle"),
							ImagePath = GetValue<string>(row, "ImagePath"),
							RedirectUrl = GetValue<string>(row, "RedirectUrl"),
							RefId = GetValue<string>(row, "RefId"),
							DisplayOrder = GetValue<int>(row, "DisplayOrder"),
							IsActive = GetValue<bool>(row, "IsActive")
						});
					}
				}
			}

			CommonViewModel.Obj = obj;

			CommonViewModel.SelectListItems = new List<SelectListItem_Custom>();

			oParams = new List<SqlParameter>();
			oParams.Add(new SqlParameter("@Id", -1));

			var dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Category_Get", oParams, true);

			if (dt != null && dt.Rows.Count > 0)
				foreach (DataRow dr in dt.Rows)
					if (GetValue<bool>(dr, "IsActive") == true && GetValue<int>(dr, "ParentCategoryId") > 0)
						CommonViewModel.SelectListItems.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), (string.IsNullOrEmpty(GetValue<string>(dr, "ParentCategoryName")) ? GetValue<string>(dr, "CategoryName") : GetValue<string>(dr, "CategoryName") + " - " + GetValue<string>(dr, "ParentCategoryName")), GetValue<string>(dr, "ParentCategoryId"), "CAT", GetValue<int>(dr, "DisplayOrder")));

			oParams = new List<SqlParameter>();
			oParams.Add(new SqlParameter("@Id", -2));

			dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Product_Get", oParams, true);

			if (dt != null && dt.Rows.Count > 0)
				foreach (DataRow dr in dt.Rows)
					CommonViewModel.SelectListItems.Add(new SelectListItem_Custom(GetValue<string>(dr, "ProductId") + "-" + GetValue<string>(dr, "VarientId"), GetValue<string>(dr, "ProductName") + " - " + GetValue<string>(dr, "VariantAttributeValues_Only"), GetValue<string>(dr, "CategoryId"), "PROD"));

			return PartialView("_Partial_AddEditForm", CommonViewModel);
		}

		#endregion

		#region SAVE

		[HttpPost]
		public async Task<JsonResult> Save(HomePageComponent model)
		{
			try
			{
				string uploadFolder = Path.Combine(AppHttpContextAccessor.WebRootPath, "Uploads", "HomePage");

				List<string> items = new List<string>();

				if (model.Items != null && model.Type == "Banner")
				{
					for (int i = 0; i < model.Items.Count; i++)
					{
						var image = "-";

						if (Request.Form.Files.Count > 0 && Request.Form.Files.Any(x => x.Name.StartsWith($"Items[{i}].Image")))
						{
							var file = Request.Form.Files.First(x => x.Name.StartsWith($"Items[{i}].Image"));
							var match = Regex.Match(file.Name, @"\[(\d+)\]");
							int index = int.Parse(match.Groups[1].Value);
							image = await FileUploadService.UploadImageAsync(file, uploadFolder);
						}

						items.Add($"{model.Items[i].Id}|{model.Items[i].Title}|{model.Items[i].Subtitle}|{image}|{model.Items[i].DisplayOrder}|{(model.Items[i].IsActive ? 1 : 0)}");
					}
				}

				string itemString = string.Join("</i><i>", items);

				List<SqlParameter> oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("Id", model.Id));
				oParams.Add(new SqlParameter("Key", model.Key));
				oParams.Add(new SqlParameter("Name", model.Name));
				oParams.Add(new SqlParameter("Type", model.Type));
				//oParams.Add(new SqlParameter("DisplayOrder", model.DisplayOrder));
				oParams.Add(new SqlParameter("IsActive", model.IsActive ? 1 : 0));
				oParams.Add(new SqlParameter("Items", itemString));
				oParams.Add(new SqlParameter("RefId", model.RefId));
				oParams.Add(new SqlParameter("ItemId", model.ItemId));

				oParams.Add(new SqlParameter("Mode", "SAVE"));
				oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

				var (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_HomePageComponent_Save", oParams, true);

				CommonViewModel.IsSuccess = IsSuccess;
				CommonViewModel.Message = Message;
				CommonViewModel.IsConfirm = true;
			}
			catch (Exception ex)
			{
				CommonViewModel.IsSuccess = false;
				CommonViewModel.Message = ex.Message;
			}

			return Json(CommonViewModel);
		}

		#endregion

		#region DELETE

		[HttpPost]
		public JsonResult DeleteConfirmed(long id)
		{
			List<SqlParameter> oParams = new List<SqlParameter>();

			oParams.Add(new SqlParameter("Id", id));
			oParams.Add(new SqlParameter("Mode", "DELETE"));
			oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

			var (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_HomePageComponent_Save", oParams, true);

			CommonViewModel.IsSuccess = IsSuccess;
			CommonViewModel.Message = Message;
			CommonViewModel.IsConfirm = true;

			return Json(CommonViewModel);
		}

		#endregion
	}
}
