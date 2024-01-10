using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks.Args;
public interface IWebAppHookArgs
{
    WebApplication App { get; set; }
}
