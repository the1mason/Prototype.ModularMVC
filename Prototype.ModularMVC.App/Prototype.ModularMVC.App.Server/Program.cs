using Prototype.ModularMVC.App.Server.PluginBase;
using Serilog;

namespace Prototype.ModularMVC.App.Server;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        var pluginDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");


        if (!Directory.Exists(pluginDirectory))
            Directory.CreateDirectory(pluginDirectory);

        IEnumerable<IPlugin> plugins = PluginLoader.LoadPlugins(pluginDirectory);

        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureWebApplicationBuilder(plugins);


        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add Razor Runtime Compilation services
        builder.Services.AddRazorPages();

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

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
