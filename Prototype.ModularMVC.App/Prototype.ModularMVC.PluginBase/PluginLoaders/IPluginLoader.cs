﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.PluginBase.PluginLoaders;
public interface IPluginLoader
{
    /// <summary>
    /// The directory where the plugins are located
    /// </summary>
    string LookupDirectory { get; }

    /// <summary>
    /// Loads all plugins from the plugin directory
    /// </summary>
    /// <returns>An array of instanciated <see cref="IPlugin"/>s </returns>
    IPlugin[] LoadPlugins();
}

