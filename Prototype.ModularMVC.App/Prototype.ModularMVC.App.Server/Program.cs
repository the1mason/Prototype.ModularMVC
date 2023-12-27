using Prototype.ModularMVC.PluginBase;
using Prototype.ModularMVC.PluginBase.Impl;
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

        IPluginLoader pluginLoader = new ManifestBasedPluginLoader(pluginDirectory);
        IEnumerable<IPlugin> plugins = pluginLoader.LoadPlugins();

        var builder = WebApplication.CreateBuilder(args);
        
        builder.ConfigureWebApplicationBuilder(plugins);

        builder.Services.AddControllersWithViews();
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

public static class PluginLoaderExtensions
{
    /// <summary>
    /// This method passes down <see cref="WebApplicationBuilder"/> to every <see cref="IPlugin"/> in <paramref name="plugins"/>
    /// </summary>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    public static WebApplicationBuilder ConfigureWebApplicationBuilder(this WebApplicationBuilder builder, IEnumerable<IPlugin> plugins)
    {
        foreach (IPlugin plugin in plugins)
        {
            builder = plugin.ConfigureWebApplicationBuilder(builder);
        }
        return builder;
    }

    /// <summary>
    /// This method passes down <see cref="WebApplication"/> to every <see cref="IPlugin"/> in <paramref name="plugins"/>
    /// </summary>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    public static WebApplication ConfigureWebApplication(this WebApplication app, IEnumerable<IPlugin> plugins)
    {
        foreach (IPlugin plugin in plugins)
        {
            app = plugin.ConfigureWebApplication(app);
        }
        return app;
    }
}
