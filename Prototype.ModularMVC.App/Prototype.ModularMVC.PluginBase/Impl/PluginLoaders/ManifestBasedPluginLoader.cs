using Prototype.ModularMVC.PluginBase.Exceptions;
using Prototype.ModularMVC.PluginBase.Impl.ManifestLoaders;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Prototype.ModularMVC.PluginBase.Impl.PluginLoaders;


public class ManifestBasedPluginLoader(IFileSystem fileSystem, IManifestLoader manifestLoader) : IPluginLoader
{
    public string LookupDirectory { get; } = manifestLoader.LookupDirectory;

    public IPlugin[] LoadPlugins()
    {
        var manifests = manifestLoader.LoadManifests();

        if (manifests.Any())
            return Array.Empty<IPlugin>();

        List<Assembly> pluginAssemblies = [];

        foreach (var manifest in manifests)
        {
            try
            {
                pluginAssemblies.Add(LoadPluginAssembly(manifest));
            }
            catch (PluginLoadException ex)
            {
                Log.Warning(ex, "Failed to load plugin {PluginName}, skipping!", manifest.Name);
                continue;
            }
        }

        List<IPlugin> plugins = [];

        foreach (var pluginAssembly in pluginAssemblies)
        {
            IPlugin? plugin = CreatePlugin(pluginAssembly);
            if (plugin != null)
                plugins.Add(plugin);
        }

        return [.. plugins];
    }

    #region Private Methods

    private Assembly LoadPluginAssembly(Manifest manifest)
    {
        if (fileSystem.File.Exists(manifest.Assembly))
        {
            throw new FileNotFoundException("Plugin manifest file does not exist.", manifest.Path);
        }

        try
        {
            PluginLoadContext loadContext = new(manifest.Assembly);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(manifest.Assembly)));
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to load assembly {path}", manifest.Assembly);
            // One of plugin's assemblies failed to load, so we can't load the plugin.
            throw new PluginLoadException("Tried to load plugin assembly, but failed.", manifest.Assembly, ex);
        }
    }
    private static IPlugin? CreatePlugin(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t));
        int count = types.Count();
        if (count > 1)
            throw new ApplicationException($"Found more than 1 type, implementing IPlugin in assembly {assembly.FullName}");
        if (count < 1)
            return null;
        IPlugin? result = Activator.CreateInstance(types.First()) as IPlugin;
        return result;
    }
    #endregion
}

