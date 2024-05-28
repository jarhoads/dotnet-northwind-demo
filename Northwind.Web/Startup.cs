using Northwind.Shared; // AddNorthwindContext extension method

namespace Northwind.Web;

public class Startup
{
    public void ConfigureServices(IServiceCollection services) 
    {
        services.AddNorthwindContext();
        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseHsts();
        }
        app.UseRouting(); // start endpoint routing
        app.UseHttpsRedirection();
        app.UseDefaultFiles(); // index.html, default.html, and so on
        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapGet("/hello", () => "Hello World!");
        });
    }
}
