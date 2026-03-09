using JewelryStore.Controllers;
using JewelryStore.Infra;
using JewelryStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomeController : BaseController<ResponseModel<LoginViewModel>>
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		//[ValidateAntiForgeryToken]
		public JsonResult Login(LoginViewModel viewModel)
		{
			try
			{
				if (!string.IsNullOrEmpty(viewModel.UserName) && viewModel.UserName.Length > 0)
				{
					viewModel.Password = Common.Encrypt(viewModel.Password);

					var parameters = new List<SqlParameter>
					{
						new SqlParameter("@UserName", viewModel.UserName),
						new SqlParameter("@Password", viewModel.Password)
					};

					var result = DataContext.ExecuteStoredProcedure("SP_CheckAuthentication", parameters, true);

					if (result.IsSuccess)
					{
						var userId = result.Id;
						string userName = result.Extra.Count > 0 ? result.Extra[0] : "";
						long roleId = result.Extra.Count > 1 ? Convert.ToInt64(result.Extra[1]) : 0;
						string roleName = result.Extra.Count > 2 ? result.Extra[2] : "";

						Common.Set_Session_Int(SessionKey.KEY_USER_ID, userId);
						Common.Set_Session(SessionKey.KEY_USER_NAME, userName);
						Common.Set_Session_Int(SessionKey.KEY_USER_ROLE_ID, roleId);
						Common.Set_Session(SessionKey.KEY_USER_ROLE, roleName);

						parameters = new List<SqlParameter> { new SqlParameter("@UserId", userId) };

						var ds = DataContext.ExecuteStoredProcedure_DataSet("SP_UserMenuAccess", parameters);

						bool isSuperAdmin = false;
						bool isAdmin = false;
						var menuList = new List<UserMenuAccess>();

						if (ds != null && ds.Tables.Count > 0)
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								var row = ds.Tables[0].Rows[0];

								isSuperAdmin = row["IsSuperAdmin"] != DBNull.Value && Convert.ToBoolean(row["IsSuperAdmin"]);

								isAdmin = row["IsAdmin"] != DBNull.Value && Convert.ToBoolean(row["IsAdmin"]);
							}

							if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
							{
								foreach (DataRow dr in ds.Tables[1].Rows)
								{
									var item = new UserMenuAccess
									{
										RoleId = dr["RoleId"] != DBNull.Value ? Convert.ToInt32(dr["RoleId"]) : 0,
										UserId = dr["UserId"] != DBNull.Value ? Convert.ToInt32(dr["UserId"]) : 0,
										MenuId = dr["MenuId"] != DBNull.Value ? Convert.ToInt32(dr["MenuId"]) : 0,

										RoleName = dr["RoleName"]?.ToString(),
										UserName = dr["UserName"]?.ToString(),
										MenuName = dr["MenuName"]?.ToString(),
										Area = dr["Area"]?.ToString(),
										Controller = dr["Controller"]?.ToString(),
										Url = dr["Url"]?.ToString(),
										ParentMenuId = dr["ParentMenuId"] != DBNull.Value ? Convert.ToInt32(dr["ParentMenuId"]) : 0,
										ParentMenuName = dr["ParentMenuName"]?.ToString(),
										DisplayOrder = dr["DisplayOrder"] != DBNull.Value
													   ? Convert.ToInt32(dr["DisplayOrder"])
													   : (int?)null
									};

									menuList.Add(item);
								}
							}
						}

						Common.Set_Session_Int(SessionKey.KEY_IS_ADMIN, isAdmin ? 1 : 0);
						Common.Set_Session_Int(SessionKey.KEY_IS_SUPER_USER, isSuperAdmin ? 1 : 0);

						Common.Configure_UserMenuAccess(BuildMenuTree(menuList));

						CommonViewModel.IsConfirm = false;
						CommonViewModel.IsSuccess = true;
						CommonViewModel.Message = ResponseStatusMessage.Success;
						CommonViewModel.RedirectURL = Url.Content("~/") + "Admin/" + this.ControllerContext.RouteData.Values["Controller"].ToString() + "/Index";

						return Json(CommonViewModel);
					}

					CommonViewModel.IsConfirm = true;
					CommonViewModel.IsSuccess = false;
					CommonViewModel.Message = result.Message;

					return Json(CommonViewModel);
				}

			}
			catch (Exception ex)
			{
				CommonViewModel.IsConfirm = true;
				CommonViewModel.IsSuccess = false;
				CommonViewModel.Message = ResponseStatusMessage.Error + " | " + ex.Message;
			}

			CommonViewModel.IsConfirm = true;
			CommonViewModel.IsSuccess = false;
			CommonViewModel.Message = "User not available.";

			return Json(CommonViewModel);
		}

		public IActionResult Logout()
		{
			Common.Clear_Session();

			return RedirectToAction("Login", "Home", new { Area = "Admin" });
		}

		public static List<UserMenuAccess> BuildMenuTree(List<UserMenuAccess> listMenu)
		{
			var dictionary = listMenu.ToDictionary(x => x.MenuId);

			List<UserMenuAccess> roots = new();

			foreach (var menu in listMenu.OrderBy(x => x.DisplayOrder))
			{
				if (menu.ParentMenuId == 0)
				{
					roots.Add(menu);
				}
				else if (dictionary.ContainsKey(menu.ParentMenuId))
				{
					dictionary[menu.ParentMenuId].Children.Add(menu);
				}
			}

			// Order children recursively
			void SortChildren(List<UserMenuAccess> items)
			{
				foreach (var item in items)
				{
					item.Children = item.Children
						.OrderBy(x => x.DisplayOrder)
						.ToList();

					SortChildren(item.Children);
				}
			}

			SortChildren(roots);

			return roots.OrderBy(x => x.DisplayOrder).ToList();
		}
	}
}
