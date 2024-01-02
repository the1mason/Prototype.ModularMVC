using Microsoft.AspNetCore.Builder;
using Prototype.ModularMVC.PluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.TestResources.NoIPlugin;
public class NotIPlugin
{
    public string Id => "not.plugin";

    public string Name => "Not a Plugin: same fields, no implementation";

    public string Version => "-1";

    public WebApplication ConfigureWebApplication(WebApplication application)
    {
        throw new NotImplementedException("Plugin is not intended to be loaded");
    }

    public WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder builder)
    {
        throw new NotImplementedException("Plugin is not intended to be loaded");
    }
}
