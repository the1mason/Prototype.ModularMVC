﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.Hooks;
public interface IHook
{
    byte Priority { get; }
}
