# C# Web application with plugin system

![Dotnet Workflow](https://github.com/the1mason/Prototype.ModularMVC/actions/workflows/dotnet-build.yml/badge.svg)
![Dotnet Workflow](https://github.com/the1mason/Prototype.ModularMVC/actions/workflows/dotnet-test.yml/badge.svg)

This project is a proof of concept. I wanted to create an ASP.NET MVC app with plugin system. Plugins can contain views and controllers, which allows to extend the app without recompiling it.

### Disclaimer

First of all, this is a prototype, which means that it's architecture is not producion ready. This kind of plugin-based application would need a rich event system, load priority for plugins, and lots of other important features. But you can still use it as a starting point (I sure will).

## Whys'

Self-hosted applications, including open source ones, have a wide range of users and use cases, which means that the application should be easily extensible in order to satisfy greater user base. A plugin system is needed: The application should load plugins from a directory during startup.

## Requirements

Developers should be able to extend application's functionality by creating plugins.

Plugins should be able to extend site's functionality both on the client and server side.

Plugins should be able to make changes to the database schema.

Using transpiled languages for the cliend is not an option, because the client would have to be rebuilt after a new plugin is installed. I decided to use MVC with htmx to make the cliend more responsive, but still defined by the server. This would make client extension far easier.

## Repository structure

This repository contains the following solutions:

- Prototype.ModularMVC.App - the server itself and plugin base.
- Prototype.ModularMVC.ExamplePlugin - a sample plugin.

## PluginBase

The plugin base is a class library that contains interfaces and classes that are used by the server and plugins. It is referenced by the server and plugins. 

```csharp

public interface IPlugin
{
    string Id { get; }

    string Name { get; }

    string Version { get; }

    WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder builder);
    
    WebApplication ConfigureWebApplication(WebApplication application);
}

```

It also contains an IPluginLoader interface and a PluginLoader class, which is used by the server to load plugins.

```csharp
public interface IPluginLoader
{
    string LookupDirectory { get; }

    IEnumerable<IPlugin> LoadPlugins();
}

```

## Plugin

A plugin is a class library that references the PluginBase. It contains controllers, views, and other files that are needed by the plugin. It also contains a class that implements the `IPlugin` interface.  
In `ConfigureWebApplicationBuilder` method, the plugin registers its controllers and views.

```csharp

using Prototype.ModularMVC.PluginBase;

public class ExamplePlugin : IPlugin
{
    public string Id => "prototype.modularmvc.exampleplugin";

    public string Name => "Example Plugin";

    public string Version => "0.0.1";

    public WebApplication ConfigureWebApplication(WebApplication application)
    {
        return application;
    }

    public WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder application)
    {
        application.Services.AddControllersWithViews()
            .AddApplicationPart(typeof(ExamplePlugin).Assembly); // This registers the plugin's controllers and views
        return application;
    }
```

#### Important

The plugin project file should target `Microsoft.NET.Sdk.Razor` SDK (not class library). Otherwise, plugin's views won't be compiled.  

To exclude PluginBase dependency from the plugin's output, add `<Private>false</Private>` to the PluginBase reference in the plugin's project file.

Also, to access WebApplication and WebApplicationBuilder, the plugin should reference `Microsoft.AspNetCore.App` framework:

```xml
<ItemGroup>
	<FrameworkReference Include="Microsoft.AspNetCore.App">
	</FrameworkReference>
</ItemGroup>
```

## Server

The server is an ASP.NET MVC application that references the PluginBase. It contains a `Program` class, which loads plugins from a directory and registers them.

```csharp

using Prototype.ModularMVC.PluginBase;
using Prototype.ModularMVC.PluginBase.Impl;

public class Program
{
    public static void Main(string[] args)
    {
        // ...

        var pluginDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");

        if (!Directory.Exists(pluginDirectory)) // Ensure that the plugin directory exists
            Directory.CreateDirectory(pluginDirectory);

        IPluginLoader pluginLoader = new PluginLoader(pluginDirectory);
        IEnumerable<IPlugin> plugins = pluginLoader.LoadPlugins();

        var builder = WebApplication.CreateBuilder(args);
        
        builder.ConfigureWebApplicationBuilder(plugins);

        // ...
    }
}

public static class PluginLoaderExtensions
{
    public static WebApplicationBuilder ConfigureWebApplicationBuilder(this WebApplicationBuilder builder, IEnumerable<IPlugin> plugins)
    {
        foreach (IPlugin plugin in plugins)
        {
            builder = plugin.ConfigureWebApplicationBuilder(builder);
        }
        return builder;
    }
}
```

### Other

Huge thanks to [Paul Braetz](https://github.com/PaulBraetz) for help :)  
Even bigger thanks to Left2Dotnet from the C# discord server
