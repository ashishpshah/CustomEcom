using JewelryStore.Areas.Admin.Models;
using JewelryStore.Controllers;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OfferController : BaseController<ResponseModel<Offer>>
	{
		public IActionResult Index()
		{
			//ViewBag.ConditionTypes = EnumHelper.ToSelectList<OfferConditionTypeEnum>();
			//ViewBag.BenefitTypes = EnumHelper.ToSelectList<OfferBenefitTypeEnum>();

			return View();
		}

		public ActionResult GetDataList(JqueryDatatableParam param)
		{
			string QuickFilter = HttpContext.Request.Query["QuickFilter"];

			int TotalRecords = 0;
			int FilteredRecords = 0;

			List<Offer> result = new List<Offer>();

			try
			{
				var oParams = new List<SqlParameter>
				{
					new SqlParameter("@Id", null),
					new SqlParameter("@Search", param.sSearch),
					new SqlParameter("@Start", param.iDisplayStart),
					new SqlParameter("@Length", param.iDisplayLength),
					new SqlParameter("@SortColumnIndex", param.iSortCol_0),
					new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"),
					new SqlParameter("@QuickFilter", QuickFilter)
				};

				DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Offer_Get", oParams);

				if (ds != null && ds.Tables.Count > 0)
				{
					/* COUNTS */
					if (ds.Tables[0].Rows.Count > 0)
					{
						TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
						FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
					}

					/* DATA */
					if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
					{
						foreach (DataRow dr in ds.Tables[1].Rows)
						{
							result.Add(new Offer
							{
								SrNo = GetValue<int>(dr, "SrNo"),
								Id = GetValue<int>(dr, "Id"),
								OfferName = GetValue<string>(dr, "OfferName"),
								OfferType = GetValue<string>(dr, "OfferType"),
								DiscountType = GetValue<string>(dr, "DiscountType"),
								DiscountValue = GetValue<decimal?>(dr, "DiscountValue"),
								CouponCode = GetValue<string>(dr, "CouponCode"),
								IsCouponRequired = GetValue<bool>(dr, "IsCouponRequired"),
								StartDate = GetValue<DateTime?>(dr, "StartDate"),
								EndDate = GetValue<DateTime?>(dr, "EndDate"),
								Priority = GetValue<int>(dr, "Priority"),
								IsActive = GetValue<bool>(dr, "IsActive"),
								UsedCount = GetValue<int>(dr, "UsedCount"),
								//Applicability = GetValue<string>(dr, "Applicability"),
								LastModifiedDate = GetValue<DateTime?>(dr, "LastModifiedDate"),
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogService.LogInsert("Offer_GetDataList", "", ex);
			}

			return Json(new
			{
				param.sEcho,
				iTotalRecords = TotalRecords,
				iTotalDisplayRecords = FilteredRecords,
				aaData = result
			});
		}


		[HttpGet]
		public IActionResult Partial_AddEditForm(long id)
		{
			Offer obj = new Offer();
			var oParams = new List<SqlParameter>();

			try
			{
				oParams = new List<SqlParameter>
				{
					new SqlParameter("@Id", id)
				};

				DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Offer_Get", oParams);

				if (ds != null && ds.Tables.Count > 0)
				{
					/* MASTER */
					if (ds.Tables[0].Rows.Count > 0)
					{
						var dr = ds.Tables[0].Rows[0];

						obj = new Offer
						{
							Id = GetValue<int>(dr, "Id"),
							OfferName = GetValue<string>(dr, "OfferName") ?? string.Empty,
							OfferType = GetValue<string>(dr, "OfferType") ?? string.Empty,
							DiscountType = GetValue<string>(dr, "DiscountType") ?? string.Empty,
							DiscountValue = GetValue<decimal?>(dr, "DiscountValue") ?? 0,
							CouponCode = GetValue<string>(dr, "CouponCode") ?? string.Empty,
							IsCouponRequired = GetValue<bool>(dr, "IsCouponRequired"),
							StartDate = GetValue<DateTime?>(dr, "StartDate"),
							EndDate = GetValue<DateTime?>(dr, "EndDate"),
							ApplyOnly_Category = GetValue<string>(dr, "ApplyOnly_Category") ?? string.Empty,
							ApplyOnly_Product = GetValue<string>(dr, "ApplyOnly_Product") ?? string.Empty,
							IsActive = GetValue<bool>(dr, "IsActive")
						};
					}

					///* APPLICABILITY */
					//if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
					//{
					//	obj.OfferApplicability = ParseApplicability(
					//		GetValue<string>(ds.Tables[1].Rows[0], "ApplicabilityData")
					//	);
					//}

					///* CONDITIONS */
					//if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
					//{
					//	obj.OfferConditions = ParseConditions(
					//		GetValue<string>(ds.Tables[2].Rows[0], "ConditionData")
					//	);
					//}

					///* BENEFITS */
					//if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
					//{
					//	obj.OfferBenefits = ParseBenefits(
					//		GetValue<string>(ds.Tables[3].Rows[0], "BenefitData")
					//	);
					//}
				}
			}
			catch (Exception ex)
			{
				LogService.LogInsert("Offer_GetSingle", "", ex);
			}

			CommonViewModel.Obj = obj;

			var list = new List<SelectListItem_Custom>();

			//list.AddRange(EnumHelper.ToSelectList<OfferTypeEnum>("OFFER_TYPE"));
			list.AddRange(EnumHelper.ToSelectList<DiscountTypeEnum>("DISCOUNT_TYPE"));
			//list.AddRange(EnumHelper.ToSelectList<OfferConditionTypeEnum>("CONDITION_TYPE"));
			//list.AddRange(EnumHelper.ToSelectList<OfferBenefitTypeEnum>("BENEFIT_TYPE"));
			//list.AddRange(EnumHelper.ToSelectList<DayOfWeekEnum>("DAY_OF_WEEK"));


			oParams = new List<SqlParameter>();
			oParams.Add(new SqlParameter("@Id", -1));

			var dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Category_Get", oParams, true);

			if (dt != null && dt.Rows.Count > 0)
				foreach (DataRow dr in dt.Rows)
					if (GetValue<bool>(dr, "IsActive") == true && GetValue<int>(dr, "ParentCategoryId") > 0)
						list.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), (string.IsNullOrEmpty(GetValue<string>(dr, "ParentCategoryName")) ? GetValue<string>(dr, "CategoryName") : GetValue<string>(dr, "CategoryName") + " - " + GetValue<string>(dr, "ParentCategoryName")), GetValue<string>(dr, "ParentCategoryId"), "CAT", GetValue<int>(dr, "DisplayOrder")));

			oParams = new List<SqlParameter>();
			oParams.Add(new SqlParameter("@Id", -2));

			dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Product_Get", oParams, true);

			if (dt != null && dt.Rows.Count > 0)
			{
				list.AddRange(dt.AsEnumerable().GroupBy(r => new { Id = r.Field<int>("ProductId"), Name = r.Field<string>("ProductName") })
					.Select(g => new SelectListItem_Custom(g.Key.Id.ToString(), g.Key.Name, "PROD")).ToList());
			}
			//foreach (DataRow dr in dt.Rows)
			//CommonViewModel.SelectListItems.Add(new SelectListItem_Custom(GetValue<string>(dr, "ProductId") + "-" + GetValue<string>(dr, "VarientId"), GetValue<string>(dr, "ProductName") + " - " + GetValue<string>(dr, "VariantAttributeValues_Only"), GetValue<string>(dr, "CategoryId"), "PROD"));

			CommonViewModel.SelectListItems = list;

			return PartialView("_Partial_AddEditForm", CommonViewModel);
		}


		/* =========================================
		   SAVE
		========================================= */
		[HttpPost]
		public JsonResult Save(Offer viewModel)
		{
			try
			{
				var oParams = new List<SqlParameter>
				{
					new SqlParameter("@Id", viewModel.Id),
					new SqlParameter("@OfferName", viewModel.OfferName),
					new SqlParameter("@OfferType", viewModel.OfferType),
					new SqlParameter("@DiscountType", viewModel.DiscountType),
					new SqlParameter("@DiscountValue", viewModel.DiscountValue ?? (object)DBNull.Value),
					new SqlParameter("@MaxDiscountAmount", viewModel.MaxDiscountAmount ?? (object)DBNull.Value),
					new SqlParameter("@MinOrderAmount", viewModel.MinOrderAmount ?? (object)DBNull.Value),
					new SqlParameter("@MinQty", viewModel.MinQty ?? (object)DBNull.Value),
					new SqlParameter("@IsCouponRequired", viewModel.IsCouponRequired),
					new SqlParameter("@CouponCode", viewModel.CouponCode ?? (object)DBNull.Value),
					new SqlParameter("@StartDate", viewModel.StartDate),
					new SqlParameter("@EndDate", viewModel.EndDate),
					new SqlParameter("@ApplyOnly_Category", viewModel.ApplyOnly_Category ?? (object)DBNull.Value),
					new SqlParameter("@ApplyOnly_Product", viewModel.ApplyOnly_Product ?? (object)DBNull.Value),

					///* CSV DATA */
					//new SqlParameter("@ApplicabilityData", OfferHelper.BuildApplicability(viewModel.OfferApplicability)),
					//new SqlParameter("@ConditionData", OfferHelper.BuildConditions(viewModel.OfferConditions)),
					//new SqlParameter("@BenefitData", OfferHelper.BuildBenefits(viewModel.OfferBenefits)),
					new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0),

					new SqlParameter("Mode", "SAVE"),
					new SqlParameter("OperatedBy", Logged_In_UserId)

				};

				var (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Offer_Save", oParams, true);

				CommonViewModel.IsConfirm = true;
				CommonViewModel.IsSuccess = IsSuccess;
				CommonViewModel.Message = Message;
				CommonViewModel.RedirectURL = IsSuccess ? Url.Content("~/Admin/Offer/Index") : "";
			}
			catch (Exception ex)
			{
				LogService.LogInsert("Offer_Save", "", ex);

				CommonViewModel.IsSuccess = false;
				CommonViewModel.Message = ResponseStatusMessage.Error + " | " + ex.Message;
			}

			return Json(CommonViewModel);
		}


		/* =========================================
		   DELETE
		========================================= */
		[HttpPost]
		public JsonResult DeleteConfirmed(long id)
		{
			try
			{
				var oParams = new List<SqlParameter>
			{
				new SqlParameter("@Id", id),
				new SqlParameter("@Mode", "DELETE")
			};

				var (IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Offer_Save_FULL", oParams, true);

				CommonViewModel.IsSuccess = IsSuccess;
				CommonViewModel.Message = Message;
			}
			catch (Exception ex)
			{
				LogService.LogInsert("Offer_Delete", "", ex);

				CommonViewModel.IsSuccess = false;
				CommonViewModel.Message = ex.Message;
			}

			return Json(CommonViewModel);
		}


		/* =========================================
		   🔥 PARSE HELPERS (VERY IMPORTANT)
		========================================= */

		//private List<OfferApplicability> ParseApplicability(string data)
		//{
		//	var list = new List<OfferApplicability>();

		//	if (string.IsNullOrEmpty(data)) return list;

		//	foreach (var item in data.Split(','))
		//	{
		//		var parts = item.Split('~');

		//		list.Add(new OfferApplicability
		//		{
		//			ProductId = parts[0] == "NULL" ? null : Convert.ToInt64(parts[0]),
		//			VariantId = parts[1] == "NULL" ? null : Convert.ToInt64(parts[1]),
		//			CategoryId = parts[2] == "NULL" ? null : Convert.ToInt64(parts[2])
		//		});
		//	}

		//	return list;
		//}

		//private List<OfferCondition> ParseConditions(string data)
		//{
		//	var list = new List<OfferCondition>();

		//	if (string.IsNullOrEmpty(data)) return list;

		//	foreach (var item in data.Split(','))
		//	{
		//		var parts = item.Split('~');

		//		list.Add(new OfferCondition
		//		{
		//			ConditionType = parts[0],
		//			ConditionValue = parts[1]
		//		});
		//	}

		//	return list;
		//}

		//private List<OfferBenefit> ParseBenefits(string data)
		//{
		//	var list = new List<OfferBenefit>();

		//	if (string.IsNullOrEmpty(data)) return list;

		//	foreach (var item in data.Split(','))
		//	{
		//		var parts = item.Split('~');

		//		list.Add(new OfferBenefit
		//		{
		//			BenefitType = parts[0],
		//			Value = parts[1] == "NULL" ? null : Convert.ToDecimal(parts[1]),
		//			FreeProductId = parts[2] == "NULL" ? null : Convert.ToInt64(parts[2]),
		//			FreeQty = parts[3] == "NULL" ? null : Convert.ToInt32(parts[3])
		//		});
		//	}

		//	return list;
		//}
	}
}
