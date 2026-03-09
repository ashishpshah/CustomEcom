using ClosedXML.Excel;
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
	public class UserController : BaseController<ResponseModel<Users>>
	{
		public IActionResult Index()
		{
			return View();
		}

		public ActionResult GetDataList(JqueryDatatableParam param)
		{
			int TotalRecords = 0;
			int FilteredRecords = 0;
			List<Users> result = new List<Users>();

			try
			{
				var oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("@Id", null));
				oParams.Add(new SqlParameter("@Search", param.sSearch));
				oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
				oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
				oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
				oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

				DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Users_Get", oParams);

				if (ds != null && ds.Tables.Count > 0)
				{
					if (ds.Tables[0].Rows.Count > 0)
					{
						TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
						FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
					}

					if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[1].Rows)
							result.Add(new Users()
							{
								SrNo = GetValue<int>(dr, "SrNo"),
								Id = GetValue<int>(dr, "Id"),
								RoleId = GetValue<int>(dr, "RoleId"),
								FirstName = GetValue<string>(dr, "FirstName"),
								LastName = GetValue<string>(dr, "LastName"),
								UserName = GetValue<string>(dr, "UserName"),
								RoleName = GetValue<string>(dr, "RoleName"),
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
			var obj = new Users();

			var list = new List<SelectListItem_Custom>();

			var oParams = new List<SqlParameter>();

			DataTable dt = null;

			try
			{
				if (id > 0)
				{
					oParams = new List<SqlParameter>();
					oParams.Add(new SqlParameter("@Id", id));

					dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Users_Get", oParams);

					if (dt != null && dt.Rows.Count > 0)
					{
						var dr = dt.Rows[0];

						obj = new Users()
						{
							Id = GetValue<int>(dr, "Id"),
							RoleId = GetValue<int>(dr, "RoleId"),
							FirstName = GetValue<string>(dr, "FirstName"),
							LastName = GetValue<string>(dr, "LastName"),
							UserName = GetValue<string>(dr, "UserName"),
							RoleName = GetValue<string>(dr, "RoleName"),
							IsActive = GetValue<bool>(dr, "IsActive"),
						};
					}
				}

				oParams = new List<SqlParameter>();
				oParams.Add(new SqlParameter("@Id", -1));

				list.Add(new SelectListItem_Custom("0", "-- Select --", "ROLE"));
				dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Roles_Get", oParams, true);

				if (dt != null && dt.Rows.Count > 0)
					foreach (DataRow dr in dt.Rows)
						list.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), GetValue<string>(dr, "RoleName"), "ROLE"));


			}
			catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

			CommonViewModel.Obj = obj;
			CommonViewModel.SelectListItems = list;

			return PartialView("_Partial_AddEditForm", CommonViewModel);

		}


		[HttpPost]
		public JsonResult Save(Users viewModel)
		{
			try
			{
				var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

				if (viewModel.IsPassword_Reset == true) viewModel.Password = "12345";

				List<SqlParameter> oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("Id", viewModel.Id));
				oParams.Add(new SqlParameter("RoleId", viewModel.RoleId));
				oParams.Add(new SqlParameter("UserName", viewModel.UserName));
				oParams.Add(new SqlParameter("Password", string.IsNullOrEmpty(viewModel.Password) ? null : Common.Encrypt(viewModel.Password)));
				oParams.Add(new SqlParameter("FirstName", viewModel.FirstName));
				oParams.Add(new SqlParameter("LastName", viewModel.LastName));
				oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));

				oParams.Add(new SqlParameter("Mode", "SAVE"));
				oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Users_Save", oParams, true);

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

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Users_Save", oParams, true);

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
		public JsonResult BulkDelete(List<int> ids)
		{
			var oParams = new List<SqlParameter> { new SqlParameter("@Ids", string.Join(",", ids)) };

			DataContext.ExecuteStoredProcedure("SP_Users_BulkDelete", oParams, true);

			return Json(true);
		}

		public ActionResult ExportExcel()
		{
			var oParams = new List<SqlParameter>();
			DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Users_Get_All", oParams);

			var dt = ds.Tables[0];

			using (XLWorkbook wb = new XLWorkbook())
			{
				wb.Worksheets.Add(dt, "Users");
				using (MemoryStream stream = new MemoryStream())
				{
					wb.SaveAs(stream);
					return File(stream.ToArray(),
						"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
						"Users.xlsx");
				}
			}
		}
	}
}
