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
public sealed class ManifestBasedPluginLoader : IPluginLoader, IUnloadablePluginLoader
{
    public bool IsUnloadable;
    List<AssemblyLoadContext> _loadedContexts = [];
    readonly IFileSystem _fileSystem;
    readonly IManifestLoader _manifestLoader;

    public ManifestBasedPluginLoader(IFileSystem fileSystem, IManifestLoader manifestLoader, bool isUnloadable = false)
    {
        _fileSystem = fileSystem;
        _manifestLoader = manifestLoader;
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
            try
            {
                IPlugin? plugin = CreatePlugin(pluginAssembly);
                if (plugin != null)
                    plugins.Add(plugin);
            }
            catch (ApplicationException ex)
            {
                Log.Warning(ex.Message);
                continue;
            }
        }

        return [.. plugins];
    }

    #region Private Methods

    Assembly LoadPluginAssembly(Manifest manifest)
    {
        var path = Directory.GetParent(manifest.Path!) + "/" + manifest.Assembly;

        if (!_fileSystem.File.Exists(path))
        {
            throw new FileNotFoundException("Plugin assembly file does not exist.", path);
        }

        try
        {
            PluginLoadContext loadContext = new(path, IsUnloadable);
            Assembly pluginAssembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(manifest.Assembly)));
            _loadedContexts.Add(loadContext);
            return pluginAssembly;
        }
         catch (Exception ex)
        {
            Log.Warning(ex, "Failed to load assembly {path}", path);
            throw new PluginLoadException("Tried to load plugin assembly, but failed.", path, ex);
        }
    }
    static IPlugin? CreatePlugin(Assembly assembly)
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
        if(!IsUnloadable)
            throw new InvalidOperationException("This plugin loader is not unloadable!");

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

