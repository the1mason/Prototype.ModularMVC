using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prototype.ModularMVC.App.Server.PluginBase;

namespace Prototype.ModularMVC.ExamplePlugin;

/// <summary>
/// <inheritdoc cref="IPlugin"/>
/// </summary>
public class ExamplePlugin : IPlugin
{
    public string Id => "prototype.modularmvc.exampleplugin";

    public string Name => "Example Plugin";

    public string Version => "0.0.1";

    public string ViewDirectory => "";

    public WebApplication ConfigureWebApplication(WebApplication application)
    {
        return application;
    }

    public WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder application)
    {
        application.Services.AddControllersWithViews().AddApplicationPart(typeof(ExamplePlugin).Assembly).AddControllersAsServices().AddViewComponentsAsServices();
        application.Services.AddRazorPages().AddApplicationPart(typeof(ExamplePlugin).Assembly);

        return application;
    }
    /*
     builder.Services.Configure<RazorViewEngineOptions>(options =>
        {
            var environment = builder.Services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();
            environment.ContentRootFileProvider = new CompositeFileProvider(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "path_to_your_project")),
                environment.ContentRootFileProvider);
        });
     */
}
