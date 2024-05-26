using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PartsPaladin.Data;
using Microsoft.AspNetCore.Session;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PartsPaladinContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PartsPaladinContext") ?? throw new InvalidOperationException("Connection string 'PartsPaladinContext' not found.")));

builder.Services.AddSession();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
