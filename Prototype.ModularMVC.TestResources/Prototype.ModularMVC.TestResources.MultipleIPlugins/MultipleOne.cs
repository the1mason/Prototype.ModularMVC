using Microsoft.AspNetCore.Builder;
using Prototype.ModularMVC.PluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.TestResources.MultipleIPlugins;
public class MultipleOne : IPlugin
{
    public string Id => "prototype.test-resources.multiple-one";
    public string Name => "MultipleOne";
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
