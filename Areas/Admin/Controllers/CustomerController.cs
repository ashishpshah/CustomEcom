using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
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
	public class CustomerController : BaseController<ResponseModel<Customer>>
	{
		public IActionResult Index()
		{
			return View();
		}

		public ActionResult GetDataList(JqueryDatatableParam param)
		{
			int TotalRecords = 0;
			int FilteredRecords = 0;
			List<Customer> result = new List<Customer>();

			try
			{
				var oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("@Id", null));
				oParams.Add(new SqlParameter("@Search", param.sSearch));
				oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
				oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
				oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
				oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

				DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Customer_Get", oParams);

				if (ds != null && ds.Tables.Count > 0)
				{
					if (ds.Tables[0].Rows.Count > 0)
					{
						TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
						FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
					}

					if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[1].Rows)
							result.Add(new Customer()
							{
								SrNo = GetValue<int>(dr, "SrNo"),
								Id = GetValue<int>(dr, "Id"),

								FirstName = GetValue<string>(dr, "FirstName"),
								LastName = GetValue<string>(dr, "LastName"),

								Email = GetValue<string>(dr, "Email"),
								MobileNo = GetValue<string>(dr, "MobileNo"),

								Password = GetValue<string>(dr, "Password"),

								DateOfBirth = GetValue<DateTime?>(dr, "DateOfBirth"),
								Gender = GetValue<string>(dr, "Gender"),

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
			var obj = new Customer();

			var list = new List<SelectListItem_Custom>();

			var oParams = new List<SqlParameter>();

			try
			{
				if (id > 0)
				{
					oParams = new List<SqlParameter>();
					oParams.Add(new SqlParameter("@Id", id));

					DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Customer_Get", oParams);

					if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
					{
						var dr = ds.Tables[0].Rows[0];

						obj = new Customer()
						{
							Id = GetValue<int>(dr, "Id"),

							FirstName = GetValue<string>(dr, "FirstName"),
							LastName = GetValue<string>(dr, "LastName"),

							Email = GetValue<string>(dr, "Email"),
							MobileNo = GetValue<string>(dr, "MobileNo"),

							DateOfBirth = GetValue<DateTime?>(dr, "DateOfBirth"),
							Gender = GetValue<string>(dr, "Gender"),

							IsActive = GetValue<bool>(dr, "IsActive")
						};

					}

					var addressList = new List<CustomerAddress>();

					if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[1].Rows)
						{
							addressList.Add(new CustomerAddress()
							{
								Id = GetValue<int>(dr, "Id"),
								CustomerId = GetValue<int>(dr, "CustomerId"),

								AddressLine1 = GetValue<string>(dr, "AddressLine1"),
								AddressLine2 = GetValue<string>(dr, "AddressLine2"),

								City = GetValue<string>(dr, "City"),
								State = GetValue<string>(dr, "State"),
								PostalCode = GetValue<string>(dr, "PostalCode"),
								Country = GetValue<string>(dr, "Country"),

								IsDefault = GetValue<bool>(dr, "IsDefault"),

								IsActive = GetValue<bool>(dr, "IsActive"),
								IsDeleted = GetValue<bool>(dr, "IsDeleted"),

								CreatedBy = GetValue<int>(dr, "CreatedBy"),
								CreatedDate = GetValue<DateTime>(dr, "CreatedDate"),

								LastModifiedBy = GetValue<int?>(dr, "LastModifiedBy"),
								LastModifiedDate = GetValue<DateTime?>(dr, "LastModifiedDate")
							});
						}

					var cartList = new List<CustomerCart>();

					if (ds != null && ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[2].Rows)
						{
							cartList.Add(new CustomerCart()
							{
								Id = GetValue<int>(dr, "Id"),
								CustomerId = GetValue<int>(dr, "CustomerId"),

								ProductId = GetValue<int>(dr, "ProductId"),
								VariantId = GetValue<int>(dr, "VariantId"),

								Quantity = GetValue<int>(dr, "Quantity"),

								IsActive = GetValue<bool>(dr, "IsActive"),
								IsDeleted = GetValue<bool>(dr, "IsDeleted"),

								CreatedBy = GetValue<int>(dr, "CreatedBy"),
								CreatedDate = GetValue<DateTime>(dr, "CreatedDate"),

								LastModifiedDate = GetValue<DateTime?>(dr, "LastModifiedDate"),

								/* Joined Fields from SP */
								ProductName = GetValue<string>(dr, "ProductName"),
								SKU = GetValue<string>(dr, "SKU"),
								Price = GetValue<decimal>(dr, "Price")
							});
						}
				}

				oParams = new List<SqlParameter>();
				oParams.Add(new SqlParameter("@Id", -1));

			}
			catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

			CommonViewModel.Obj = obj;
			CommonViewModel.SelectListItems = list;

			return PartialView("_Partial_AddEditForm", CommonViewModel);

		}


		[HttpPost]
		public async Task<JsonResult> Save(Customer viewModel)
		{
			try
			{
				var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

				List<SqlParameter> oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("Id", viewModel.Id));

				oParams.Add(new SqlParameter("FirstName", viewModel.FirstName));
				oParams.Add(new SqlParameter("LastName", viewModel.LastName));

				oParams.Add(new SqlParameter("Email", viewModel.Email));
				oParams.Add(new SqlParameter("MobileNo", viewModel.MobileNo));

				oParams.Add(new SqlParameter("Password", viewModel.Password));

				oParams.Add(new SqlParameter("DateOfBirth", viewModel.DateOfBirth ?? (object)DBNull.Value));
				oParams.Add(new SqlParameter("Gender", viewModel.Gender ?? (object)DBNull.Value));

				oParams.Add(new SqlParameter("AddressLine1", viewModel.AddressLine1));
				oParams.Add(new SqlParameter("AddressLine2", viewModel.AddressLine2));

				oParams.Add(new SqlParameter("City", viewModel.City));
				oParams.Add(new SqlParameter("State", viewModel.State));

				oParams.Add(new SqlParameter("PostalCode", viewModel.PostalCode));
				oParams.Add(new SqlParameter("Country", viewModel.Country));

				oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));
				oParams.Add(new SqlParameter("Mode", "SAVE"));
				oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Customer_Save", oParams, true);

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

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Customer_Save", oParams, true);

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
