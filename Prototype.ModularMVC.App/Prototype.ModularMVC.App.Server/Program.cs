using Prototype.ModularMVC.App.Server.Hooks;
using Prototype.ModularMVC.Hooks;
using Prototype.ModularMVC.Hooks.Args;
using Prototype.ModularMVC.Hooks.Impl.Hooks;
using Prototype.ModularMVC.Hooks.Impl.Publishers;
using Prototype.ModularMVC.PluginBase;
using Prototype.ModularMVC.PluginBase.Impl.ManifestLoaders;
using Prototype.ModularMVC.PluginBase.Impl.PluginLoaders;
using Prototype.ModularMVC.PluginBase.PluginLoaders;
using Serilog;
using System.IO.Abstractions;

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

        IPluginLoader pluginLoader = new ManifestBasedPluginLoader(new FileSystem(),new ManifestLoader(pluginDirectory, new FileSystem()));

        IEnumerable<IPlugin> plugins = pluginLoader.LoadPlugins();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton(plugins);
        
        builder.ConfigureTriggers();
        builder.ConfigureDefaultHooks();
        
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

internal static class PluginLoaderExtensions
{

    internal static WebApplicationBuilder ConfigureWebApplicationBuilder(this WebApplicationBuilder builder, IEnumerable<IPlugin> plugins)
    {
        foreach (IPlugin plugin in plugins)
        {
            builder = plugin.ConfigureWebApplicationBuilder(builder);
        }
        return builder;
    }


    internal static WebApplication ConfigureWebApplication(this WebApplication app, IEnumerable<IPlugin> plugins)
    {
        foreach (IPlugin plugin in plugins)
        {
            app = plugin.ConfigureWebApplication(app);
        }
        return app;
    }

    internal static WebApplicationBuilder ConfigureTriggers(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<CancellableTrigger<IExampleHook>>();
        return builder;
    }

    internal static WebApplicationBuilder ConfigureDefaultHooks(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IExampleHook, ExampleHook>();
        builder.Services.AddScoped<IExampleHook, ExampleHook2>();
        return builder;
    }
}
