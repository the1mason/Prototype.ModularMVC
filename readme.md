# C# Web application with plugin system

![Dotnet Workflow](https://github.com/the1mason/Prototype.ModularMVC/actions/workflows/dotnet-build.yml/badge.svg)
![Dotnet Workflow](https://github.com/the1mason/Prototype.ModularMVC/actions/workflows/dotnet-test.yml/badge.svg)

This project is a proof of concept. I wanted to create an ASP.NET MVC app with plugin system. Plugins can contain views and controllers, which allows to extend the app without recompiling it.

### Disclaimer

First of all, this is a prototype, which means that it's architecture is not as producion ready as it could have been, but you can still use it as a starting point (I sure will). As of right now this application lacks dependency resolving for plugins, any authenticion, etc.. 
I have another project on [my gitea](https://git.the1mason.com/the1mason/OctoCore), which is a WIP web app, based on this prototype.

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
  - PluginBase - the base dependency that any plugin should be built upon
  - tests/Prorotype.ModularMVC.PluginBase.Tests - unit tests for PluginBase
  - Hooks - an event system, more on that below
  - Server - MVC project that servs as a core of this web app
- Prototype.ModularMVC.ExamplePlugin - a sample plugin that adds a controller with a view, located at /Example/Index
- Prototype.ModularMVC.TestResources - a set of plugins, made for testing plugin loader. I have compiled them already and put in `resources` folder of the test project, so there is no need to build those.

## PluginBase

It contains the base interface for a plugin (IPlugin) and an interface and an implementation for a plugin loader.

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


```csharp
public interface IPluginLoader
{
    string LookupDirectory { get; }

    IPlugin[] LoadPlugins();
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
    // this method allows the plugin to setup the app and configure the DI container
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

Also, to access ASP.NET-related classes, the plugin should reference `Microsoft.AspNetCore.App` framework:

```xml
<ItemGroup>
	<FrameworkReference Include="Microsoft.AspNetCore.App">
	</FrameworkReference>
</ItemGroup>
```

## Hooks and Triggers

It is a custom event system. I haven't used the built-in `event` keyword because of a need in custom iteration logic for executing events.    

#### Hook definition

`hook` is a class in plugin's library, that implements `ISomeHook` with some handling method:  

```csharp

// Hook Interface
public interface ISomeHook : IHook
{
    void Some_ExecuteSomething(SomethingHookArgs args);
}

```

#### Hook implementation

A subscriber's class should implement `ISomeHook` and be registred as `ISomeHook` implementation in a DI container:  

```csharp

public SomePluginService : ISomePluginService, ISomeHook
{
    // ... ISomePluginServiceImplementation ...//
    // ...

    // ISomeHook implementation
    void Some_ExecuteSomething(SomethingHookArgs args)
    {
        args.SomeField = "someNewValue";
    }
}


```

```csharp

// In IPlugin

public WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder application)
    {
	// Adding ISomePluginService
        application.Services.AddScoped<ISomePluginService>();
	// then making sure to return the same ISomePluginService instance for each hook type that it implements
	application.Services.AddScoped<ISomeHook>(x => x.GetRequiredService<ISomePluginService>();
        return application;
    }

```

#### Triggering registered hooks

As you can see, different plugins are implementing the same IHook, which results multiple implementations of the same interface. That's how a trigger executes all hooks:

```csharp

public class SomeTrigger : ISomeTrigger
{

    private readonly IServiceProvider _serviceProvider;

    public SomeTrigger(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public void ExecuteSome(SomeHookArgs args)
    {
        var hooks = _serviceProvider.GetServices<ISomeHook>();
	// iterating through all subscribed hooks
        foreach (var hook in hooks.OrderByDescending(x => x.Priority))
        {
            if (hook.Cancelled)
                break;

            hook.Some_ExecuteSomething(args); // executing the subsibed hook's action
        }
    }

}

```

## Conclusion

This might look a little overcomplicated, but if you approach it from a plugin developer perspective, you just have to implement some interfaces and register some stuff in the DI container!  
I like the result and am goint to continue to iterate over it's design in my non-prototype project.

### Other

Huge thanks to [Paul Braetz](https://github.com/PaulBraetz) for help :)  
Even bigger thanks to [Asylkhan Azat](https://github.com/asylkhan-azat) from the C# discord server
