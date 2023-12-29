using Xunit;
using System.IO.Abstractions.TestingHelpers;
using Prototype.ModularMVC.PluginBase.Impl.ManifestLoaders;
using Prototype.ModularMVC.PluginBase;

namespace Prorotype.ModularMVC.PluginBase.Tests.ManifestLoaderTests;

public class ManifestLoaderTests
{
    private readonly MockFileSystem _mockFileSystem;
    private readonly ManifestLoader _manifestLoader;

    public ManifestLoaderTests()
    {
        _mockFileSystem = new MockFileSystem();
        _manifestLoader = new ManifestLoader("" ,_mockFileSystem);
    }


    [Fact]
    public void LoadManifest_ShouldReturnNull_WhenRequiredFieldIsMissing()
    {
        const string MANIFEST = @"{
                ""name"": ""Test Plugin"",
                ""version"": ""1.0.0_425"",
                ""description"": ""Description""
            }";

        const string MANIFEST_DIRECTORY = "ExamplePlugin";
        const string MANIFEST_PATH = MANIFEST_DIRECTORY + "/plugin.json";

        // Arrange
        var mockFileData = new MockFileData(MANIFEST);
        _mockFileSystem.AddDirectory(MANIFEST_DIRECTORY);
        _mockFileSystem.AddFile(MANIFEST_PATH, mockFileData);

        // Act
        Manifest? result = _manifestLoader.LoadManifest(MANIFEST_PATH);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void LoadManifest_ShouldThrowException_WhenFileNotFount()
    {
        const string MANIFEST_PATH = "ExamplePlugin";

        // Arrange
        _mockFileSystem.AddDirectory(MANIFEST_PATH);

        // Act
        void action() => _manifestLoader.LoadManifest(MANIFEST_PATH + "/plugin.json");

        // Assert
        Assert.Throws<FileNotFoundException>(action);
    }


    [Fact]
    public void LoadManifest_ShouldReturnManifest_WhenOnlyRequiredFieldsArePresent()
    {

        const string MANIFEST = @"{
                ""id"": ""test.plugin"",
                ""name"": ""Test Plugin"",
                ""version"": ""1.0.0_425"",
                ""Assembly"": ""TestAssembly.dll""
            }";

        const string MANIFEST_DIRECTORY = "ExamplePlugin";
        const string MANIFEST_PATH = MANIFEST_DIRECTORY + "/plugin.json";

        // Arrange
        var mockFileData = new MockFileData(MANIFEST);
        _mockFileSystem.AddFile(MANIFEST_PATH, mockFileData);

        string[] files = _mockFileSystem.Directory.GetFiles("./", "*", SearchOption.AllDirectories);
        // Act
        Manifest? result = _manifestLoader.LoadManifest(MANIFEST_PATH);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.plugin", result?.Id);
        Assert.Equal("Test Plugin", result?.Name);
        Assert.Equal("1.0.0_425", result?.Version);
        Assert.Equal("TestAssembly.dll", result?.Assembly);
    }


}
