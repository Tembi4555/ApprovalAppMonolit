using ApprovalApp.Application;
using ApprovalApp.Data;
using ApprovalApp.Data.PersonsRepository;
using ApprovalApp.Data.TicketsRepository;
using ApprovalApp.Data.UsersRepositoty;
using ApprovalApp.Domain.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddDbContext<ApprovalDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("ApprovalDbConnection"),
        b => b.MigrationsAssembly("ApprovalApp.API")));

builder.Services.AddAuthentication("Cookies");
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Login/Login"));
builder.Services.AddAuthorization();


builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<IPersonsPerository, PersonsRepository>();
builder.Services.AddScoped<ITicketsRepository, TicketsRepository>();
builder.Services.AddScoped<ITicketsService, TicketsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
