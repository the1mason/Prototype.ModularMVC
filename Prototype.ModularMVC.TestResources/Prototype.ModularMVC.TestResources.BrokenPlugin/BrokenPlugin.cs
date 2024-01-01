using Microsoft.AspNetCore.Builder;
using Prototype.ModularMVC.PluginBase;

namespace Prototype.ModularMVC.TestResources.DepdPlugin;
public class BrokenPlugin : IPlugin
{
    public string Id => "prototype.test-resources.broken-plugin";
    public string Name => "Broken Plugin";
    public WebApplication ConfigureWebApplication(WebApplication application)
    {
        throw new NotImplementedException("Plugin is not intended to be loaded");
    }

    public WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder builder)
    {
        throw new NotImplementedException("Plugin is not intended to be loaded");
    }

}
