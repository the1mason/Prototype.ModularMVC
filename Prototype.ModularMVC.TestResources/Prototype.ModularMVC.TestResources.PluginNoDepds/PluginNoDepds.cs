using Microsoft.AspNetCore.Builder;
using Prototype.ModularMVC.PluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.TestResources.PluginNoDepds;
public class PluginNoDepds : IPlugin
{
    public string Id => "prototype.test-resources.plugin-without-dependencies";
    public string Name => "PluginNoDepds";
    public string Version => "0.1";

    public WebApplication ConfigureWebApplication(WebApplication application)
    {
        throw new NotImplementedException("Plugin is not intended to be loaded");
    }

    public WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder builder)
    {
        throw new NotImplementedException("Plugin is not intended to be loaded");
    }
}
