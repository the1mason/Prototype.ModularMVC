using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.PluginBase;
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


/*
 
 public string PluginDirectory { get; }

    public IEnumerable<IPlugin> LoadPlugins()
    {
        string[] pluginPaths = GetPluginPaths(PluginDirectory);

        List<IPlugin> plugins = new();

        foreach (var path in pluginPaths)
        {
            Assembly pluginAssembly = LoadPluginAssembly(path);
            if (pluginAssembly == null)
                continue;
            IPlugin plugin = CreatePlugin(pluginAssembly);
            if (plugin != null)
                plugins.Add(plugin);
        }

        return plugins;
    }

    public string[] GetPluginPaths(string pluginDirectory)
    {
        var dllFiles = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);
        var paths = new string[dllFiles.Length];

        for (int i = 0; i < dllFiles.Length; i++)
        {
            paths[i] = Path.GetFullPath(dllFiles[i]);
        }

        return paths;
    }

    public static Assembly LoadPluginAssembly(string path)
    {
        try
        {
            PluginLoadContext loadContext = new PluginLoadContext(path);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
        }
        catch
        {
            return null;
        }
    }

    public static IPlugin CreatePlugin(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t));
        int count = types.Count();
        if (count > 1)
            throw new ApplicationException($"Found more than 1 type, implementing IPlugin in assembly {assembly.FullName}");
        if (count < 1)
            return null;
        IPlugin result = Activator.CreateInstance(types.First()) as IPlugin;
        return result;
    }

 */