using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Args;
public class WebAppConfiguringHookArgs : IWebAppHookArgs
{

    public WebAppConfiguringHookArgs(WebApplication app)
    {
        Actions = [];
        App = app;
    }
    public List<Action<WebApplication>> Actions { get; set; }

    public WebApplication App { get; set; }

    public void Configure()
    {
        foreach (var action in Actions)
            action(App);
    }
}
