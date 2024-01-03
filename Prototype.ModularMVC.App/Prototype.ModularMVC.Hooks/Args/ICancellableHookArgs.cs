using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Args;
public interface ICancellableHookArgs
{
    bool IsCancelled { get; set; }
}
