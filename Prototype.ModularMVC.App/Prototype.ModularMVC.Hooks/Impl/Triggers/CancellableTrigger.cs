using Prototype.ModularMVC.Hooks.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Impl.Publishers;


/// <summary>
/// An implementation of <see cref="ITrigger{THook, TArgs}"/> that allows to cancel execution of hooks.
/// </summary>
/// <typeparam name="THook"></typeparam>
public class CancellableTrigger<THook> : ITrigger<THook, ICancellableHookArgs> where THook : class, IHook
{
    readonly IEnumerable<THook> _hooks;

    public CancellableTrigger(IEnumerable<THook> hooks)
    {
        _hooks = hooks;
    }

    /// <inheritdoc cref="ITrigger{THook, TArgs}.Execute(Action{THook, TArgs}, TArgs)"/>
    public ICancellableHookArgs Execute(Action<THook, ICancellableHookArgs> action, ICancellableHookArgs args)
    {
        var hooks = _hooks.OrderByDescending(h => h.Priority).ToArray();
        foreach (var service in _hooks)
        {
            if (args.IsCancelled)
                break;

            action(service, args);
        }
        return args;
    }
}