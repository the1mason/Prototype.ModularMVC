using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Args;
public class WebAppHookArgs : IWebAppHookArgs
{
    public WebAppHookArgs(WebApplication app)
    {
        App = app;
    }

    public WebApplication App { get; set; }
}

