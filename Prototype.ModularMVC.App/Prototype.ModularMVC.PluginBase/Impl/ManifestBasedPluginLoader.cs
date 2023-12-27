using Prototype.ModularMVC.PluginBase.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.PluginBase.Impl;


public class ManifestBasedPluginLoader(string lookupDirectory) : IPluginLoader
{
    public string LookupDirectory { get; } = lookupDirectory;

    public IEnumerable<IPlugin> LoadPlugins()
    {
        Dictionary<string, Manifest> manifests = LoadManifests(LookupDirectory);

        if (manifests.Count == 0)
            return Enumerable.Empty<IPlugin>();

        List<Assembly> pluginAssemblies = [];

        foreach (var manifest in manifests)
        {
            try
            {
                var dlls = Directory.GetFiles(Directory.GetParent(manifest.Key).ToString(), "*.dll", SearchOption.AllDirectories);
                foreach (var dll in dlls)
                {
                    var assembly = LoadAssembly(dll);
                    if (assembly == null)
                        continue;

                    if (manifest.Value.Assembly == assembly.ManifestModule.Name)
                        pluginAssemblies.Add(assembly);
                }
            }
            catch (PluginLoadException ex)
            {
                Log.Warning(ex, "Failed to load plugin {PluginName}, skipping!", manifest.Value.Name);
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

        return plugins;
    }

    #region Private Methods
    private static Dictionary<string ,Manifest> LoadManifests(string pluginDirectory)
    {
        var manifestPath = Directory.GetFiles(pluginDirectory, "plugin.json", SearchOption.AllDirectories);

        Dictionary<string ,Manifest> manifests = [];

        foreach (var path in manifestPath)
        {
            Manifest? manifest = GetManifest(path);
            if (manifest != null)
                manifests.Add(path, manifest);
        }

        return manifests;
    }

    private static Manifest? GetManifest(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Manifest>(json, options);
        }
        catch (JsonException ex)
        {
            Log.Warning(ex, "Failed to deserialize manifest file {Path}", path);
            return null;
        }
    }

    private static Assembly? LoadAssembly(string path)
    {
        try
        {
            PluginLoadContext loadContext = new(path);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
        }
        catch(Exception ex)
        {
            Log.Warning(ex, "Failed to load assembly {path}", path);
                // One of plugin's assemblies failed to load, so we can't load the plugin.
                throw new PluginLoadException("Tried to load plugin assembly, but failed.", path, ex);
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

