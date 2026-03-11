using JewelryStore.Areas.Api.ServiceRepository.CategoryRepository;
using JewelryStore.Areas.Api.ServiceRepository.ProductRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace JewelryStore
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddHttpClient();
			builder.Services.AddHttpContextAccessor();

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Add service For API
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();


            builder.WebHost.ConfigureKestrel(options =>
			{
				options.Limits.MaxRequestLineSize = 16 * 1024;
				options.Limits.MaxRequestHeadersTotalSize = 32 * 1024;
			});

			builder.Services.Configure<FormOptions>(o =>
			{
				o.ValueCountLimit = int.MaxValue;
				o.ValueLengthLimit = int.MaxValue;
				o.MultipartBodyLengthLimit = long.MaxValue;
			});

			builder.Services.AddControllersWithViews()
				.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

			// ✅ ONE localization config
			builder.Services.Configure<RequestLocalizationOptions>(options =>
			{
				var culture = new CultureInfo("en-IN");
				culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
				culture.DateTimeFormat.LongDatePattern = "dd/MM/yyyy HH:mm:ss";

				options.DefaultRequestCulture = new RequestCulture(culture);
				options.SupportedCultures = new[] { culture };
				options.SupportedUICultures = new[] { culture };
			});

			// ✅ REQUIRED for FTP hosting
			builder.Services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
			});

			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(10);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
				options.Cookie.SameSite = SameSiteMode.Lax;
				options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
			});

			var app = builder.Build();

			AppHttpContextAccessor.Configure(
				app.Services.GetRequiredService<IHttpContextAccessor>(),
				app.Services.GetRequiredService<IHostEnvironment>(),
				builder.Environment,
				app.Services.GetRequiredService<IDataProtectionProvider>(),
				app.Services.GetRequiredService<IConfiguration>(),
				app.Services.GetRequiredService<IHttpClientFactory>()
			);

			// 🔥 MUST be very early
			app.UseForwardedHeaders();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRequestLocalization();

			app.UseRouting();

			// ✅ AUTH FIRST
			app.UseAuthentication();

			// ✅ SESSION AFTER AUTH
			app.UseSession();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
				  name: "areas",
				  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
				);
			});

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapGet("/", context =>
			{
				context.Response.Redirect("/Admin/Home/Login");
				return Task.CompletedTask;
			});

			app.Run();
		}
	}
}
