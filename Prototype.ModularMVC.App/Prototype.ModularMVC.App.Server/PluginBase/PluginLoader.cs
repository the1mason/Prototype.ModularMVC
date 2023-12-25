using System.Reflection;

namespace Prototype.ModularMVC.App.Server.PluginBase;

internal static class PluginLoader
{
    public static IEnumerable<IPlugin> LoadPlugins(string pluginDirectory)
    {
        string[] pluginPaths = GetPluginPaths(pluginDirectory);

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

    internal static string[] GetPluginPaths(string pluginDirectory)
    {
        var dllFiles = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);
        var paths = new string[dllFiles.Length];

        for (int i = 0; i < dllFiles.Length; i++)
        {
            paths[i] = Path.GetFullPath(dllFiles[i]);
        }

        return paths;
    }

    internal static Assembly LoadPluginAssembly(string path)
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
}

internal static class PluginLoaderExtensions
{
    /// <summary>
    /// This method passes down <see cref="WebApplicationBuilder"/> to every <see cref="IPlugin"/> in <paramref name="plugins"/>
    /// </summary>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    internal static WebApplicationBuilder ConfigureWebApplicationBuilder(this WebApplicationBuilder builder, IEnumerable<IPlugin> plugins)
    {
        foreach (IPlugin plugin in plugins)
        {
            builder = plugin.ConfigureWebApplicationBuilder(builder);
        }
        return builder;
    }

    /// <summary>
    /// This method passes down <see cref="WebApplication"/> to every <see cref="IPlugin"/> in <paramref name="plugins"/>
    /// </summary>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    internal static WebApplication ConfigureWebApplication(this WebApplication app, IEnumerable<IPlugin> plugins)
    {
        foreach (IPlugin plugin in plugins)
        {
            app = plugin.ConfigureWebApplication(app);
        }
        return app;
    }
}

