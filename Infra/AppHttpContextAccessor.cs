using Microsoft.AspNetCore.DataProtection;

namespace JewelryStore.Infra
{
	public static class AppHttpContextAccessor
	{
		private static IHttpContextAccessor _httpContextAccessor;
		private static string _contentRootPath;
		private static string _webRootPath;
		private static IDataProtector _dataProtector;
		private static IConfiguration _iConfig;
		private static IHttpClientFactory _clientFactory;
		private static string _connectionString;

		public static void Configure(IHttpContextAccessor httpContextAccessor, IHostEnvironment env_Host, IWebHostEnvironment env_Web, IDataProtectionProvider provider, IConfiguration iConfig, IHttpClientFactory clientFactory)
		{
			_httpContextAccessor = httpContextAccessor;
			_contentRootPath = env_Host.ContentRootPath;
			_webRootPath = env_Web.WebRootPath;
			_dataProtector = provider.CreateProtector("20260227095731001");
			_iConfig = iConfig;
			_clientFactory = clientFactory;

			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
			string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
			configurationBuilder.AddJsonFile(path, optional: false);
			IConfigurationRoot configurationRoot = configurationBuilder.Build();

			_connectionString = configurationRoot.GetSection("ConnectionStrings").GetSection("DataConnection").Value;
		}

		public static string EncrKey => "20260227095731001";
		public static HttpContext AppHttpContext => _httpContextAccessor.HttpContext;
		public static HttpClient AppHttpClient => _clientFactory.CreateClient();
		public static IConfiguration AppConfiguration => _iConfig;

		public static string AppBaseUrl => $"{AppHttpContext.Request.Scheme}://{AppHttpContext.Request.Host}{AppHttpContext.Request.PathBase}";
		public static string ContentRootPath => $"{_contentRootPath}";
		public static string WebRootPath => $"{_webRootPath}";
		public static bool IsLogActive_Info => Convert.ToBoolean(AppHttpContextAccessor.AppConfiguration.GetSection("IsLogActive_Info").Value);
		public static bool IsLogActive_Error => Convert.ToBoolean(AppHttpContextAccessor.AppConfiguration.GetSection("IsLogActive_Error").Value);

		public static bool IsLogged() => (AppHttpContext.Session.GetInt32(SessionKey.KEY_USER_ID) ?? 0) > 0;
		public static int LoggedUserId() => AppHttpContext.Session.GetInt32(SessionKey.KEY_USER_ID) ?? 0;
		public static string LoggedUserName() { return AppHttpContext.Session.Keys.Contains(SessionKey.KEY_USER_NAME) ? AppHttpContext.Session.GetString(SessionKey.KEY_USER_NAME) : ""; }

		public static int LoggedUserRoleId() => AppHttpContext.Session.GetInt32(SessionKey.KEY_USER_ROLE_ID) ?? 0;
		public static string LoggedUserRoleName() { return AppHttpContext.Session.Keys.Contains(SessionKey.KEY_USER_ROLE) ? AppHttpContext.Session.GetString(SessionKey.KEY_USER_ROLE) : ""; }

		public static void SetSessionInt(string key, int value) => AppHttpContext.Session.SetInt32(key, value);
		public static long GetSessionInt(string key) => AppHttpContext.Session.GetInt32(key) ?? 0;

		public static void SetSessionSting(string key, string value) => AppHttpContext.Session.SetString(key, value);
		public static string GetSessionSting(string key) => AppHttpContext.Session.Keys.Contains(key) ? AppHttpContext.Session.GetString(key) : null;


		public static string Protect(string str) => $"{_dataProtector.Protect(str)}";
		public static string UnProtect(string str) => $"{_dataProtector.Unprotect(str)}";


	}
}
