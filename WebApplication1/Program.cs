using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Localization;
using WebApplication1.Repository;
using WebApplication1.service; // where AppDbContext lives
var builder = WebApplication.CreateBuilder(args);

// 1) Add services to the container (ALL Add... go here)
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPaymentService, DemoPaymentService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSession();
//builder.Services.AddScoped<CartService>(); builder.Services.AddScoped<AnalyticsEfService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddSingleton<IQrService, QrService>();

builder.Services.AddAppLocalization(builder.Configuration);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";        // redirect if not logged in
        o.AccessDeniedPath = "/Account/Denied";
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
        o.SlidingExpiration = true;
    });
// 2) Build the app
builder.Services.Configure<RequestLocalizationOptions>(o =>
{
    var cultures = new[] { new CultureInfo("en-US"), new CultureInfo("km-KH") };
    o.DefaultRequestCulture = new RequestCulture("en-US");
    o.SupportedCultures = cultures;
    o.SupportedUICultures = cultures;
    o.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    o.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
});

var app = builder.Build();
app.UseAppLocalization();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // db.Database.Migrate();

    if (!db.users.Any())
    {
        db.users.Add(new User
        {
            Name = "Admin",
            Email = "admin@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin",
            RegisteredDate = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

app.UseSession();



// 3) Configure the HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
var locOpts = app.Services.GetRequiredService<
    Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;

app.UseRequestLocalization(locOpts);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Logout}/{id?}");

app.Run();
