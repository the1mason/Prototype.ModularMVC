using Microsoft.AspNetCore.Builder;

namespace Prototype.ModularMVC.PluginBase;

/// <summary>
/// Interface for all plugins
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Unique identifier of <see cref="IPlugin"/>."
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Name of <see cref="IPlugin"/>. Used for display purposes.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Version of <see cref="IPlugin"/>. Used to check for updates.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// For each plugin, this method is called to configure <see cref="WebApplication"/>
    /// </summary>
    WebApplication ConfigureWebApplication(WebApplication application);

    /// <summary>
    /// For each plugin, this method is called to configure <see cref="WebApplicationBuilder"/>
    /// </summary>
    WebApplicationBuilder ConfigureWebApplicationBuilder(WebApplicationBuilder builder);
}
