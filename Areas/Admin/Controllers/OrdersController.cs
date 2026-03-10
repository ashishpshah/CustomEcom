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
	public class OrdersController : BaseController<ResponseModel<Orders>>
	{
		public IActionResult Index()
	{
            var list = new List<SelectListItem_Custom>();

            var oParams = new List<SqlParameter>();

            DataTable dt = null;
            dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Variant_Combo", null, true);

            if (dt != null && dt.Rows.Count > 0)
                foreach (DataRow dr in dt.Rows)
                    list.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), GetValue<string>(dr, "Product_Name"), GetValue<decimal>(dr, "Price"), "VARIANT", GetValue<int>(dr, "Id")));

			CommonViewModel.SelectListItems = list;
            return View(CommonViewModel);
		}

		public ActionResult GetDataList(JqueryDatatableParam param)
		{
			int TotalRecords = 0;
			int FilteredRecords = 0;
			List<Orders> result = new List<Orders>();

			try
			{
				var oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("@Id", null));
				oParams.Add(new SqlParameter("@Search", param.sSearch));
				oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
				oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
				oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
				oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

				DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Orders_Get", oParams);

				if (ds != null && ds.Tables.Count > 0)
				{
					if (ds.Tables[0].Rows.Count > 0)
					{
						TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
						FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
					}

					if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[1].Rows)
							result.Add(new Orders()
							{
								SrNo = GetValue<int>(dr, "SrNo"),
								Id = GetValue<int>(dr, "Id"),
								OrderNumber = GetValue<string>(dr, "OrderNumber"),
                                OrderStatus = GetValue<string>(dr, "OrderStatus"),
                                OrderDate = GetValue<DateTime?>(dr, "OrderDate"),
                                No_Of_Items = GetValue<int>(dr, "No_Of_Items"),
								TotalAmount = GetValue<decimal?>(dr, "TotalAmount"),
								DiscountAmount = GetValue<decimal?>(dr, "DiscountAmount"),
								FinalAmount = GetValue<decimal?>(dr, "FinalAmount"),								
								IsActive = GetValue<bool>(dr, "IsActive"),
								LastModifiedDate = GetValue<DateTime?>(dr, "LastModifiedDate")
							});
				}

			}
			catch (Exception ex) { }

			return Json(new { param.sEcho, iTotalRecords = TotalRecords, iTotalDisplayRecords = FilteredRecords, aaData = result });

		}


		[HttpGet]
		public IActionResult Partial_AddEditForm(int id ,string Flag = "")
		{
			var obj = new Orders() {};
			var listOrderItems = new List<OrderItems>();

			var list = new List<SelectListItem_Custom>();

			var oParams = new List<SqlParameter>();

			DataTable dt = null;

			try
			{
				if (id > 0)
				{

					oParams = new List<SqlParameter>();
					oParams.Add(new SqlParameter("@Id", id));

					dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Orders_Get", oParams);

					if (dt != null && dt.Rows.Count > 0)
					{
						var dr = dt.Rows[0];

						obj = new Orders()
						{
							Id = GetValue<int>(dr, "Id"),
							OrderNumber = GetValue<string>(dr, "OrderNumber"),
							OrderStatus = GetValue<string>(dr, "OrderStatus"),
                            OrderDate = GetValue<DateTime?>(dr, "OrderDate"),
							No_Of_Items = GetValue<int>(dr, "No_Of_Items"),
							TotalAmount = GetValue<decimal?>(dr, "TotalAmount"),
							DiscountAmount = GetValue<decimal?>(dr, "DiscountAmount"),
							FinalAmount = GetValue<decimal?>(dr, "FinalAmount"),
							IsActive = GetValue<bool>(dr, "IsActive")

						};
					}
					oParams = new List<SqlParameter>();
					oParams.Add(new SqlParameter("@Id", id));

					dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Order_Detail_Get", oParams);

					if (dt != null && dt.Rows.Count > 0)
					{
						foreach (DataRow dr in dt.Rows)
						{
							listOrderItems.Add(new OrderItems()
							{
								Id = GetValue<int>(dr, "Id"),
								OrderId = GetValue<int>(dr, "OrderId"),
								VariantId = GetValue<int>(dr, "VariantId"),
								ProductId = GetValue<int>(dr, "ProductId"),
								VariantName = GetValue<string>(dr, "VariantName"),
								ProductName = GetValue<string>(dr, "ProductName"),
								Quantity = GetValue<int>(dr, "Quantity"),
								UnitPrice = GetValue<decimal?>(dr, "UnitPrice"),
								TotalPrice = GetValue<decimal?>(dr, "TotalPrice"),
								IsActive = GetValue<bool>(dr, "IsActive")
							});


						}
					}

				}
				else
				{
					dt = new DataTable();
                    dt = DataContext.ExecuteStoredProcedure_DataTable("SP_GenerateOrderNumber", null);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var dr = dt.Rows[0];

                        obj = new Orders()
                        {
                           
                            OrderNumber = GetValue<string>(dr, "OrderNumber"),
                            OrderDate = DateTime.Now

                        };
                    }
                }
					//oParams = new List<SqlParameter>();
					//oParams.Add(new SqlParameter("@Id", -1));

					dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Variant_Combo", null, true);

                if (dt != null && dt.Rows.Count > 0)
                    foreach (DataRow dr in dt.Rows)
                        list.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), GetValue<string>(dr, "Product_Name"), GetValue<decimal>(dr, "Price"), "VARIANT", GetValue<int>(dr, "Id")));


            }
            catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }
			obj.ListOrderItems = listOrderItems;
			CommonViewModel.Obj = obj;
			
			CommonViewModel.SelectListItems = list;

			if(Flag == "VIEW")
			{
                return PartialView("_Partial_AddEditForm_View", CommonViewModel);
            }
			else if (Flag == "VIEWITEMS")
			{
                return PartialView("_Partial_AddEditForm_ViewItems", CommonViewModel);
            }

			return PartialView("_Partial_AddEditForm", CommonViewModel);

		}


		[HttpPost]
		public async Task<JsonResult> Save(Orders viewModel)
		{
			try
			{
				var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

                DataTable order_items_table = new DataTable();
                order_items_table.Columns.Add("VariantId", typeof(int));
                order_items_table.Columns.Add("Quantity", typeof(int));
                order_items_table.Columns.Add("UnitPrice", typeof(decimal));              
                order_items_table.Columns.Add("TotalPrice", typeof(decimal));

                if (viewModel != null && viewModel.ListOrderItems.Count > 0)
                {
                    foreach (var orderitems in viewModel.ListOrderItems)
                    {
                        order_items_table.Rows.Add(orderitems.VariantId , orderitems.Quantity,orderitems.UnitPrice,orderitems.TotalPrice);
                    }
                }

                List<SqlParameter> oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("Id", viewModel.Id));
				oParams.Add(new SqlParameter("OrderNumber", viewModel.OrderNumber));
				oParams.Add(new SqlParameter("OrderDate", viewModel.OrderDate));
				oParams.Add(new SqlParameter("TotalAmount", viewModel.TotalAmount));
				oParams.Add(new SqlParameter("DiscountAmount", viewModel.DiscountAmount));
				oParams.Add(new SqlParameter("FinalAmount", viewModel.FinalAmount));
				oParams.Add(new SqlParameter("OrderItems",  order_items_table));				
				oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));
				oParams.Add(new SqlParameter("Mode", "SAVE"));
				oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Orders_Save", oParams, true);

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

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Orders_Save", oParams, true);

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
