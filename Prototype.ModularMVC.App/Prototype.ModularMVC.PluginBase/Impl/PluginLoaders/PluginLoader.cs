using Microsoft.AspNetCore.Builder;
using Serilog;
using System.Reflection;

namespace Prototype.ModularMVC.PluginBase.Impl.PluginLoaders;

public sealed class PluginLoader(string lookupDirectory) : IPluginLoader
{
    public string LookupDirectory { get; } = lookupDirectory;

    public IPlugin[] LoadPlugins()
    {
        string[] pluginPaths = GetPluginPaths(LookupDirectory);

        List<IPlugin> plugins = [];

        foreach (var path in pluginPaths)
        {
            Assembly pluginAssembly = LoadPluginAssembly(path);
            if (pluginAssembly == null)
                continue;
            IPlugin? plugin = CreatePlugin(pluginAssembly);
            if (plugin != null)
                plugins.Add(plugin);
        }

        return [.. plugins];
    }

    #region Private Methods
    private static string[] GetPluginPaths(string pluginDirectory)
    {
        var dllFiles = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);
        var paths = new string[dllFiles.Length];

        for (int i = 0; i < dllFiles.Length; i++)
        {
            paths[i] = Path.GetFullPath(dllFiles[i]);
        }

        return paths;
    }

    private static Assembly LoadPluginAssembly(string path)
    {
        PluginLoadContext loadContext = new PluginLoadContext(path);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
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

