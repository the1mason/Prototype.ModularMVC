using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks;

/// <summary>
/// Hooks are services that are interested in certain events. They are triggered by <see cref="ITrigger{THook, TArgs}"/>.
/// You can use provided <see cref="ITrigger{THook, TArgs}"/> implementations or create your own.
/// </summary>
public interface IHook
{
    /// <summary>
    /// Priority of the hook. Hooks with lower priority would be executed last.
    /// </summary>
    byte Priority { get; }
}
