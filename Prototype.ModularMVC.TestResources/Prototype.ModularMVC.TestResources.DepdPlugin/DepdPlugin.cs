using Microsoft.AspNetCore.Builder;
using Prototype.ModularMVC.PluginBase;
using Newtonsoft.Json;

namespace Prototype.ModularMVC.TestResources.DepdPlugin;
public class DepdPlugin : IPlugin
{
    public string Id => "prototype.test-resources.plugin-with-dependency";
    public string Name => GetNameFromJson();
    public string Version => "0.2";

    public WebApplication ConfigureWebApplication(WebApplication application)
    {
        throw new NotImplementedException("Plugin is not intended to be loaded");
    }

    public WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder builder)
    {
        throw new NotImplementedException("Plugin is not intended to be loaded");
    }

    private static string GetNameFromJson()
    {
        string json = "{\"Name\":\"DepdPlugin\"}";
        return JsonConvert.DeserializeObject<NameJson>(json)!.Name;
    }

    private class NameJson
    {
        public required string Name { get; set; }
    }

}
