using Prototype.ModularMVC.Hooks.Impl.Args;
using Prototype.ModularMVC.Hooks.Impl.Hooks;

namespace Prototype.ModularMVC.App.Server.Hooks;

public class ExampleHook : IExampleHook
{
    public byte Priority => byte.MinValue;

    public void OnMessageRequested(ExampleHookArgs args)
    {
        args.Messages.Add("Hello from ExampleHook!");
    }
}
