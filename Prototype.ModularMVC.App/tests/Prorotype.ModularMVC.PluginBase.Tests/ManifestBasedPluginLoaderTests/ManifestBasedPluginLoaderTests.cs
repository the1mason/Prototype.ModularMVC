using NSubstitute;
using Prototype.ModularMVC.PluginBase;
using Prototype.ModularMVC.PluginBase.Impl.ManifestLoaders;
using Prototype.ModularMVC.PluginBase.Impl.PluginLoaders;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prorotype.ModularMVC.PluginBase.Tests.ManifestBasedPluginLoaderTests;
public class ManifestBasedPluginLoaderTests : IDisposable
{
    private readonly IFileSystem _fileSystem;
    private readonly ManifestLoader _manifestLoader;
    private readonly ManifestBasedPluginLoader _pluginLoader;

    const string MANIFEST_DIRECTORY = "ExamplePlugin";

    private readonly List<Manifest> manifests = new()
    {
        new()
        {
            Id = "prototype.test-resources.plugin-without-dependencies",
            Name = "PluginNoDepds",
            Version = "0.1",
            Description = "Plugin without dependencies",
            Assembly = "resources/Prototype.ModularMVC.TestResources.PluginNoDepds.dll"
        },
        new()
        {
            Id = "prototype.test-resources.plugin-with-dependency",
            Name = "DepdPlugin",
            Version = "0.2",
            Description = "Plugin with a Newtonsoft.JSON dependency",
            Assembly = "resources/Prototype.ModularMVC.TestResources.DepdPlugin.dll"
        },
        new()
        {
            Id = "prototype.test-resources.broken-plugin",
            Name = "Broken Plugin",
            Version = "0.3",
            Description = "Plugin that doesn't fully implements IPlugin",
            Assembly = "resources/Prototype.ModularMVC.TestResources.BrokenPlugin.dll"
        }
    };

    public ManifestBasedPluginLoaderTests()
    {
        // I am using real file system because the AssemblyLoadContext
        // references the Path static class under the hood.
        // For now I don't consider even trying to redo the AssemblyLoadContext,
        // and even then I was copying the files from the real file system anyways.
        // Let's stick a #wontfix on this one.
        _fileSystem = new FileSystem();

        _manifestLoader = Substitute.For<ManifestLoader>("./", _fileSystem);

        _manifestLoader.LoadManifests().Returns(manifests);

        _pluginLoader = new ManifestBasedPluginLoader(_fileSystem, _manifestLoader, MANIFEST_DIRECTORY, true);
    }

    public void Dispose()
    {
        _pluginLoader.UnloadAll();
    }

    #region LoadPlugins Tests

    [Fact]
    public void LoadPlugins_ShouldReturnEmptyList_WhenNoPluginsAreFound()
    {
        // Arrange
        _manifestLoader.LoadManifests().Returns([]);

        // Act
        IEnumerable<IPlugin> result = _pluginLoader.LoadPlugins();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void LoadPlugins_ShouldNotReturnPlugins_WhenPluginsAreBroken()
    {
        // Arrange
        _manifestLoader.LoadManifests().Returns([manifests[2]]);

        // Act
        IEnumerable<IPlugin> result = _pluginLoader.LoadPlugins();

        // Assert
        Assert.Empty(result);
    }


    [Fact]
    public void LoadPlugins_ShouldExcludeBrokenPluginsFromResult_WhenSomePluginsAreBroken()
    {
        // Act
        IEnumerable<IPlugin> result = _pluginLoader.LoadPlugins();

        // Assert
        Assert.DoesNotContain(result, x => x.Id == "prototype.test-resources.broken-plugin");
    }

    [Fact]
    public void LoadPlugins_ShouldReturnPlugins_WhenPluginsAreFound()
    {
        // Act
        IEnumerable<IPlugin> result = _pluginLoader.LoadPlugins();

        // Assert
        Assert.Contains(result, x => x.Id == "prototype.test-resources.plugin-without-dependencies");
        Assert.Contains(result, x => x.Id == "prototype.test-resources.plugin-with-dependency");
    }

    [Fact]
    public void LoadPlugins_ShouldLoadPlugins_WhenPluginsHaveDependencies()
    {
        // Act
        IEnumerable<IPlugin> result = _pluginLoader.LoadPlugins();

        // Assert
        Assert.Contains(result, x => x.Id == "prototype.test-resources.plugin-with-dependency");

        Assert.Equal("DepdPlugin", result.First(x => x.Id == "prototype.test-resources.plugin-with-dependency").Name); // Name property for this plugin uses Newtonsoft.JSON dependency.
    }

    [Fact]
    public void LoadPlugins_ShouldLoadPlugins_WhenPluginsHaveNoDependencies()
    {
        // Act
        IEnumerable<IPlugin> result = _pluginLoader.LoadPlugins();

        // Assert
        Assert.Contains(result, x => x.Id == "prototype.test-resources.plugin-without-dependencies");
    }

    #endregion
}
