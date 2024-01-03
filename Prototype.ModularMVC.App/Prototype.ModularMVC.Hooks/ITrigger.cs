using Prototype.ModularMVC.Hooks.Args;
using Prototype.ModularMVC.Hooks.Impl.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks;

/// <summary>
/// A trigger is a service that would trigger all hooks of a certain type.
/// Triggers are a way to distribute events to all services that are interested in them.
/// To subscribe to a trigger, a service must implement <typeparamref name="THook"/> and be registered in DI container.
/// </summary>
/// <typeparam name="THook">A type of hooks that would be triggered</typeparam>
/// <typeparam name="TArgs">Arguments that would be passed down to every hook</typeparam>
public interface ITrigger<THook, TArgs> where THook : class, IHook
{
    /// <summary>
    /// Executes <paramref name="action"/> for every service that implements <typeparamref name="THook"/>
    /// </summary>
    /// <param name="action"></param>
    /// <param name="args"></param>
    /// <returns><see cref="TArgs"/>, modified by all <typeparamref name="THook"/></returns>
    /// <example>
    /// <code>
    /// var result = myTrigger.Execute((service, args) => service.DoSomething(args), new MyHookArgs());
    /// </code>
    /// </example>
    TArgs Execute(Action<THook, TArgs> action, TArgs args);
}