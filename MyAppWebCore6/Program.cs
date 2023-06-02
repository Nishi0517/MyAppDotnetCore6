using Microsoft.EntityFrameworkCore;
using MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository;
using MyAppDotnetCore6.DataAccessLayer.Infrastructure.Repository;
using MyAppWebCore6.DataAccessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyAppDotnetCore6.CommonHelper;
using Stripe;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddDbContext<MyAppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//builder.Services.AddDefaultIdentity<IdentityUser>()
//    .AddEntityFrameworkStores<MyAppDbContext>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("PaymentSettings"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<MyAppDbContext>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.LoginPath = $"/Identity/Account/Login";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("PaymentSettings:SecretKey").Get<string>();

//dataSedding();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(100);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});
app.UseAuthentication();;

app.UseAuthorization();
//app.UseSession();
app.MapRazorPages();

app.Run();

//void dataSedding()
//{
//    using (var scope = app.Services.CreateScope())
//    {

//        var DbInitilizer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
//    }
//}