using Prototype.ModularMVC.Hooks.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Triggers;
public interface IWebAppTrigger
{
    void ExecuteOnConfiguring(WebAppConfiguringHookArgs args);

    void ExecuteOnStarted(WebAppHookArgs args);
}
