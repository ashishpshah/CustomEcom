using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Data;

namespace JewelryStore.Controllers
{
	public class BaseController : Controller
	{
		public bool IsLogActive = false;

		public readonly DateTime? nullDateTime = null;
		public string ControllerName = "";
		public string ActionName = "";
		public string AreaName = "";

		public long Logged_In_UserId { get; set; }

		public BaseController() { try { Logged_In_UserId = Common.LoggedUser_Id(); } catch { } }
	}

	public class BaseController<T> : BaseController where T : class
	{
		public T CommonViewModel { get; set; } = default(T);

		public BaseController() { CommonViewModel = (dynamic)Activator.CreateInstance(typeof(T)); }

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			try
			{
				ControllerName = Convert.ToString(context.RouteData.Values["controller"]);
				ActionName = Convert.ToString(context.RouteData.Values["action"]);

				if (context.RouteData.DataTokens != null) AreaName = Convert.ToString(context.RouteData.DataTokens["area"]);

				if (string.IsNullOrEmpty(AreaName)) AreaName = Convert.ToString(context.RouteData.Values["area"]);

				List<UserMenuAccess> listMenuAccess = JsonConvert.DeserializeObject<List<UserMenuAccess>>(Common.Get_Session(SessionKey.KEY_USER_MENUPERMISSION));

				if (listMenuAccess != null && listMenuAccess.Count > 0)
				{
					if (listMenuAccess.FindIndex(x => x.Controller == ControllerName) > -1)
					{
						try { Common.Set_Session_Int(SessionKey.CURRENT_MENU_ID, listMenuAccess[listMenuAccess.FindIndex(x => x.Controller == ControllerName)].MenuId); }
						catch { Common.Set_Session_Int(SessionKey.CURRENT_MENU_ID, 0); }
					}
				}

				if (!Common.IsUserLogged() && !(Convert.ToString(ControllerName).ToLower() == "home"
					&& (new string[] { "account", "login" }).Any(x => x == Convert.ToString(ActionName).ToLower())))
				{
					context.Result = new RedirectResult(Url.Content("~/") + (string.IsNullOrEmpty(AreaName) || !Common.IsAdmin() ? "" : AreaName + "/") + "Home/Login");
					return;
				}

			}
			catch (Exception ex) { LogService.LogInsert(GetCurrentAction(), "", ex); }
		}


		public string GetCurrentAction() => (string.IsNullOrEmpty(AreaName) ? "" : AreaName + " - ") + ControllerName + " - " + ActionName;
		public string GetCurrentControllerUrl() => (string.IsNullOrEmpty(AreaName) ? "" : AreaName + "/") + ControllerName;

		public static TModel GetValue<TModel>(DataRow row, string columnName)
		{
			if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
				return default;

			Type targetType = Nullable.GetUnderlyingType(typeof(TModel)) ?? typeof(TModel);
			return (TModel)Convert.ChangeType(row[columnName], targetType);
		}
	}
}
