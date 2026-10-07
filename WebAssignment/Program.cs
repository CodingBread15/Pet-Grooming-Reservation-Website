global using WebAssignment.Models;
using AspNetCoreGeneratedDocument;
using WebAssignment;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSqlServer<DB>($@"
    Data Source=(LocalDB)\MSSQLLocalDB;
    AttachDbFilename={builder.Environment.ContentRootPath}\DB.mdf;
");

builder.Services.AddSingleton<Helper>();

builder.Services.AddAuthentication().AddCookie(options => 
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/";
    options.AccessDeniedPath = "/User/AccessDenied";
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.Run();
