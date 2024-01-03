using Prototype.ModularMVC.Hooks.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Impl.Args;
public class CancellableHookArgs : ICancellableHookArgs
{
    public bool IsCancelled { get; set; }
}
