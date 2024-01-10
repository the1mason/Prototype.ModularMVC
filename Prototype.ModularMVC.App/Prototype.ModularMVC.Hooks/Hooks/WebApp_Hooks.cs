using Prototype.ModularMVC.Hooks.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Hooks;
public interface IWebAppConfiguringHook : ICancellableHook
{
    void WebApp_OnConfiguring(WebAppConfiguringHookArgs args);
}

public interface IWebAppStartedHook : ICancellableHook
{
    void WebApp_Started(WebAppHookArgs args);
}

