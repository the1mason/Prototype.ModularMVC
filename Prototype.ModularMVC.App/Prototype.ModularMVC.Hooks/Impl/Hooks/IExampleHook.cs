using Prototype.ModularMVC.Hooks.Impl.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Impl.Hooks;
/// <summary>
/// Just an example of hook.
/// </summary>
public interface IExampleHook : IHook
{
    void OnMessageRequested(ExampleHookArgs args);
}
