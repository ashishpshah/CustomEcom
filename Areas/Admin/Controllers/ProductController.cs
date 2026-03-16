using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Controllers;
using JewelryStore.Infra;
using JewelryStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace JewelryStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : BaseController<ResponseModel<Product>>
	{
		public IActionResult Index()
		{
			return View();
		}

		public ActionResult GetDataList(JqueryDatatableParam param)
		{
			int TotalRecords = 0;
			int FilteredRecords = 0;
			List<Product> result = new List<Product>();

			try
			{
				var oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("@Id", null));
				oParams.Add(new SqlParameter("@Search", param.sSearch));
				oParams.Add(new SqlParameter("@Start", param.iDisplayStart));
				oParams.Add(new SqlParameter("@Length", param.iDisplayLength));
				oParams.Add(new SqlParameter("@SortColumnIndex", param.iSortCol_0));
				oParams.Add(new SqlParameter("@SortDirection", param.sSortDir_0 ?? "DESC"));

				DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Product_Get", oParams);

				if (ds != null && ds.Tables.Count > 0)
				{
					if (ds.Tables[0].Rows.Count > 0)
					{
						TotalRecords = GetValue<int>(ds.Tables[0].Rows[0], "TotalRecords");
						FilteredRecords = GetValue<int>(ds.Tables[0].Rows[0], "FilteredRecords");
					}

					if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
						foreach (DataRow dr in ds.Tables[1].Rows)
							result.Add(new Product()
							{
								SrNo = GetValue<int>(dr, "SrNo"),
								Id = GetValue<int>(dr, "Id"),
								ProductName = GetValue<string>(dr, "ProductName"),
								CategoryId = GetValue<int>(dr, "CategoryId"),
								CategoryName = GetValue<string>(dr, "CategoryName"),
								ImagePath = GetValue<string>(dr, "ImagePath"),
								Price = GetValue<decimal>(dr, "Price"),
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
			var obj = new Product();

			var list = new List<SelectListItem_Custom>();

			var oParams = new List<SqlParameter>();

			DataTable dt = null;

			try
			{
				if (id > 0)
				{
					oParams = new List<SqlParameter>();
					oParams.Add(new SqlParameter("@Id", id));

					var ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Product_Get", oParams);

					if (ds != null && ds.Tables.Count > 0)
					{
						var dr = ds.Tables[0].Rows[0];

						obj = new Product()
						{
							Id = GetValue<int>(dr, "Id"),
							ProductName = GetValue<string>(dr, "ProductName"),
							CategoryId = GetValue<int>(dr, "CategoryId"),
							CategoryName = GetValue<string>(dr, "CategoryName"),
							ImagePath = GetValue<string>(dr, "ImagePath"),
							Price = GetValue<decimal>(dr, "Price"),
							IsActive = GetValue<bool>(dr, "IsActive"),
							ProductVariantMapping = new List<ProductVariantMapping>()
						};

						dt = ds.Tables[1];

						obj.ProductImages = new List<ProductImages>();

						foreach (DataRow row in dt.Rows)
						{
							obj.ProductImages.Add(new ProductImages()
							{
								Id = GetValue<int>(row, "Id"),
								ImagePath = GetValue<string>(row, "ImagePath"),
								IsPrimary = GetValue<bool>(row, "IsPrimary"),
								VariantId = GetValue<int>(row, "VariantId")
							});
						}

						dt = ds.Tables[2];
						DataTable dtDetails = ds.Tables[3];

						foreach (DataRow row in dt.Rows)
						{
							var details = dtDetails.AsEnumerable()
								.Where(x => GetValue<int>(x, "VariantId") == GetValue<int>(row, "Id"))
								.Select(x => new ProductVariantDetails()
								{
									Id = GetValue<int>(x, "Id"),
									VariantId = GetValue<int>(x, "VariantId"),
									AttributeId = GetValue<int>(x, "AttributeId"),
									AttributeName = GetValue<string>(x, "AttributeName"),
									AttributeValueId = GetValue<int>(x, "AttributeValueId"),
                                    AttributeValueName = Convert.ToInt32(row["AttributeValueId"]) + "-" + row["AttributeValue"].ToString()
                                })
								.ToList();

							obj.ProductVariantMapping.Add(new ProductVariantMapping()
							{
								Id = GetValue<int>(row, "Id"),
								Price = GetValue<decimal>(row, "Price"),
								SKU = GetValue<string>(row, "SKU"),
								ProductVariantDetails = details
							});
						}
					}
				}

				oParams = new List<SqlParameter>();
				oParams.Add(new SqlParameter("@Id", -1));

				dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Category_Get", oParams, true);

				if (dt != null && dt.Rows.Count > 0)
					foreach (DataRow dr in dt.Rows)
						list.Add(new SelectListItem_Custom(GetValue<string>(dr, "Id"), (string.IsNullOrEmpty(GetValue<string>(dr, "ParentCategoryName")) ? GetValue<string>(dr, "CategoryName") : GetValue<string>(dr, "CategoryName") + " - " + GetValue<string>(dr, "ParentCategoryName")), GetValue<string>(dr, "ParentCategoryId"), "CAT", GetValue<int>(dr, "DisplayOrder")));

				oParams = new List<SqlParameter>();
				oParams.Add(new SqlParameter("@Id", -2));

				dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Attribute_Get", oParams);

				if (dt != null && dt.Rows.Count > 0)
					foreach (DataRow row in dt.Rows)
						list.Add(new SelectListItem_Custom(GetValue<string>(row, "Value"), GetValue<string>(row, "Text"), "ATTR"));

			}
			catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }

			CommonViewModel.Obj = obj;
			CommonViewModel.SelectListItems = list;

			return PartialView("_Partial_AddEditForm", CommonViewModel);

		}


		[HttpPost]
		public async Task<JsonResult> Save(Product viewModel)
		{
			try
			{
				string uploadFolder = Path.Combine(AppHttpContextAccessor.WebRootPath, "Uploads", "Product");

				var (IsSuccess, Message, Id, Extra) = (false, ResponseStatusMessage.Error, 0M, new List<string>());

				List<string> productImages = new List<string>();

				if (Request.Form.Files.Count > 0)
				{
					bool isFirst = true;

					foreach (var file in Request.Form.Files.Where(x => x.Name.StartsWith("ProductImage")))
					{
						var match = Regex.Match(file.Name, @"\[(\d+)\]");
						int index = int.Parse(match.Groups[1].Value);

						ProductImages imageModel = new ProductImages();

						if (viewModel.ProductImages?.Count > index && viewModel.ProductImages[index] != null)
							imageModel = viewModel.ProductImages[index];

						string imagePath = await FileUploadService.UploadImageAsync(file, uploadFolder);

						productImages.Add($"{imageModel.Id}|{imagePath}|{(isFirst ? 1 : 0)}|{(imageModel.IsRemove ? 1 : 0)}");
						//productImages.Add($"{imageModel.Id}|{imagePath}|{(imageModel.IsPrimary ? 1 : 0)}|{(imageModel.IsRemove ? 1 : 0)}");

						isFirst = false;
					}
				}

				if (viewModel.ProductImages != null)
				{
					foreach (var img in viewModel.ProductImages.Where(x => x.Id > 0 && !productImages.Any(z => z.StartsWith(x.Id.ToString()))))
					{
						productImages.Add($"{img.Id}|-|{(img.IsPrimary ? 1 : 0)}|{(img.IsRemove ? 1 : 0)}");
					}
				}

				string imagesString = string.Join("||", productImages);

				List<string> productVariant = new List<string>();

				if (viewModel.ProductVariantMapping != null && viewModel.ProductVariantMapping.Any(x => x.ProductVariantDetails != null))
				{
					for (int i = 0; i < viewModel.ProductVariantMapping.Count; i++)
					{
						var variantImage = "-";

						if (Request.Form.Files.Count > 0 && Request.Form.Files.Any(x => x.Name.StartsWith($"ProductVariantMapping[{i}].VarientImage")))
						{
							var file = Request.Form.Files.First(x => x.Name.StartsWith($"ProductVariantMapping[{i}].VarientImage"));
							var match = Regex.Match(file.Name, @"\[(\d+)\]");
							int index = int.Parse(match.Groups[1].Value);
							variantImage = await FileUploadService.UploadImageAsync(file, uploadFolder);
						}

						productVariant.Add($"{viewModel.ProductVariantMapping[i].Id}|{string.Join("=", viewModel.ProductVariantMapping[i].ProductVariantDetails.Select(x => x.AttributeValueName).ToArray())}|" +
											$"{viewModel.ProductVariantMapping[i].Price}|{viewModel.ProductVariantMapping[i].SKU}|{variantImage}");
					}
				}

				string variantString = string.Join("||", productVariant);

				List<SqlParameter> oParams = new List<SqlParameter>();

				oParams.Add(new SqlParameter("Id", viewModel.Id));
				oParams.Add(new SqlParameter("CategoryId", viewModel.CategoryId));
				oParams.Add(new SqlParameter("ProductName", viewModel.ProductName));
				oParams.Add(new SqlParameter("ProductDescription", viewModel.ProductDescription));
				//oParams.Add(new SqlParameter("SKU", viewModel.SKU));
				oParams.Add(new SqlParameter("Price", viewModel.Price));

				oParams.Add(new SqlParameter("ProductImages", imagesString));
				oParams.Add(new SqlParameter("ProductVariants", variantString));
				oParams.Add(new SqlParameter("IsActive", viewModel.IsActive ? 1 : 0));

				oParams.Add(new SqlParameter("Mode", "SAVE"));
				oParams.Add(new SqlParameter("OperatedBy", Logged_In_UserId));

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Product_Save", oParams, true);

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

				(IsSuccess, Message, Id, Extra) = DataContext.ExecuteStoredProcedure("SP_Product_Save", oParams, true);

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
		public JsonResult GetAttributeValues()
		{
			var list = new List<SelectListItem_Custom>();

			var oParams = new List<SqlParameter>();
			oParams.Add(new SqlParameter("@Id", -2));

			var dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Attribute_Get", oParams);

			if (dt != null && dt.Rows.Count > 0)
				foreach (DataRow row in dt.Rows)
					list.Add(new SelectListItem_Custom(GetValue<string>(row, "Value"), GetValue<string>(row, "Text")));

			return Json(new { data = list });
		}
	}
}
