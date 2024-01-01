using Prototype.ModularMVC.PluginBase.Exceptions;
using Prototype.ModularMVC.PluginBase.Impl.ManifestLoaders;
using Prototype.ModularMVC.PluginBase.PluginLoaders;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Prototype.ModularMVC.PluginBase.Impl.PluginLoaders;

/// <summary>
/// Uses <see cref="IManifestLoader"/> to load manifests, then loads plugins from them.
/// </summary>
public class ManifestBasedPluginLoader : IPluginLoader, IUnloadablePluginLoader
{
    public string LookupDirectory { get; }
    private List<AssemblyLoadContext> _loadedContexts = [];
    public bool IsUnloadable;
    private readonly IFileSystem _fileSystem;
    private readonly IManifestLoader _manifestLoader;

    public ManifestBasedPluginLoader(IFileSystem fileSystem, IManifestLoader manifestLoader, string lookupDirectory, bool isUnloadable = false)
    {
        _fileSystem = fileSystem;
        _manifestLoader = manifestLoader;
        LookupDirectory = lookupDirectory;
        IsUnloadable = isUnloadable;
    }

    public IPlugin[] LoadPlugins()
    {
        var manifests = _manifestLoader.LoadManifests();

        if (manifests.Count == 0)
            return [];

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
        if (!_fileSystem.File.Exists(manifest.Assembly))
        {
            throw new FileNotFoundException("Plugin manifest file does not exist.", manifest.Assembly);
        }

        try
        {
            PluginLoadContext loadContext = new(manifest.Assembly, IsUnloadable);
            Assembly pluginAssembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(manifest.Assembly)));
            _loadedContexts.Add(loadContext);
            return pluginAssembly;
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
        try
        {
            var types = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t)).ToArray();
            if (types.Length > 1)
                throw new ApplicationException($"Found more than 1 type, implementing IPlugin in assembly {assembly.FullName}");
            if (types.Length < 1)
                return null;
            IPlugin? result = Activator.CreateInstance(types.First()) as IPlugin;
            return result;
        }
        catch (ReflectionTypeLoadException ex)
        {
            Log.Warning(ex, "Failed to load plugin {PluginName}, skipping!", assembly.FullName);
            return null;
        }
    }

    public void UnloadAll()
    {
        List<WeakReference> assemblies = [];
        foreach (var context in _loadedContexts)
        {
            assemblies.AddRange(context?.Assemblies.Select(x => new WeakReference(x))!);
            context?.Unload();
        }

        foreach (var assembly in assemblies)
        {
            if (TryWaitUntilUnloaded(assembly))
                continue;

            Log.Warning("Failed to unload plugin context!");
        }
    }

    static void WaitUntilAllDies(List<AssemblyLoadContext> contexts)
    {
        var references = contexts
            .SelectMany(static context => context.Assemblies)
            .Select(static assembly => new WeakReference(assembly))
            .ToList();

        contexts.ForEach(static context => context.Unload());
        contexts.Clear();

        foreach (var reference in references)
        {
            if (!TryWaitUntilUnloaded(reference))
            {
                Log.Warning("Could not unload assembly");
            }
        }
    }

    static bool TryWaitUntilUnloaded(WeakReference reference)
    {
        const int infiniteLoopGuard = 10;

        for (var i = 0; reference.IsAlive && i < infiniteLoopGuard; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        return !reference.IsAlive;
    }
    #endregion
}

