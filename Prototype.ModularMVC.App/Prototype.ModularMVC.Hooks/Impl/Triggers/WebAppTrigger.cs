using Microsoft.Extensions.DependencyInjection;
using Prototype.ModularMVC.Hooks.Args;
using Prototype.ModularMVC.Hooks.Hooks;
using Prototype.ModularMVC.Hooks.Triggers;

namespace Prototype.ModularMVC.Hooks.Impl.Triggers;
public class WebAppTrigger : IWebAppTrigger
{

    private readonly IServiceProvider _serviceProvider;

    public WebAppTrigger(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public void ExecuteOnConfiguring(WebAppConfiguringHookArgs args)
    {
        Execute<IWebAppConfiguringHook, WebAppConfiguringHookArgs>(args, (hook, args) => hook.WebApp_OnConfiguring(args));
    }

    public void ExecuteOnStarted(WebAppHookArgs args)
    {
        Execute<IWebAppStartedHook, WebAppHookArgs>(args, (hook, args) => hook.WebApp_Started(args));
    }

    public void Execute<TCancellableHook, TArgs>(TArgs args, Action<TCancellableHook, TArgs> action) where TCancellableHook : ICancellableHook where TArgs : IWebAppHookArgs
    {
        var hooks = _serviceProvider.GetServices<TCancellableHook>();

        foreach (var hook in hooks.OrderByDescending(x => x.Priority))
        {
            if (hook.Cancelled)
                break;

            action(hook, args);
        }
    }
}
