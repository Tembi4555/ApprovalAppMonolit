using ApprovalApp.Application;
using ApprovalApp.Data;
using ApprovalApp.Data.PersonsRepository;
using ApprovalApp.Data.TicketsRepository;
using ApprovalApp.Data.UsersRepositoty;
using ApprovalApp.Domain.Abstractions;
using ApprovalAppMonolit.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

builder.Services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
{
    options.PayloadSerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    options.PayloadSerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    options.PayloadSerializerSettings.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default;
}); ;

builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<IPersonsPerository, PersonsRepository>();
builder.Services.AddScoped<ITicketsRepository, TicketsRepository>();
builder.Services.AddScoped<ITicketsService, TicketsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();

app.MapHub<AppHub>("/channel", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
});

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
