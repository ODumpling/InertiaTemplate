using System.Security.Claims;
using InertiaCore;
using InertiaCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddInertia();

builder.Services.AddViteHelper(options =>
{
    options.PublicDirectory = "wwwroot";
    options.BuildDirectory = "build";
    options.HotFile = "hot";
    options.ManifestFilename = "manifest.json";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseInertia();

app.Use(async (context, next) =>
{
    object? user = null;
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        user = new
        {
            Id = context.User?.FindFirstValue(ClaimTypes.NameIdentifier),
            Username = context.User?.FindAll(ClaimTypes.Name).Select(x => x.Value).LastOrDefault(),
            Email = context.User?.FindFirstValue(ClaimTypes.Email)
        } ;

    }

    Inertia.Share("auth", new
    {
        IsAuthenicated = context.User?.Identity?.IsAuthenticated,
        User = user
                
    });
            
    await next(context);
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
