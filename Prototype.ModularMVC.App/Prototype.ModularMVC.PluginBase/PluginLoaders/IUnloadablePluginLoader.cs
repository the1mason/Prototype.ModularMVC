using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.PluginBase.PluginLoaders;

/// <summary>
/// Defines a plugin loader that can unload plugins.
/// </summary>
public interface IUnloadablePluginLoader : IPluginLoader
{
    /// <summary>
    /// Unloads all <see cref="IPlugin"/>s.
    /// </summary>
    void UnloadAll();
}