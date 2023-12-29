using Xunit;
using System.IO.Abstractions.TestingHelpers;
using Prototype.ModularMVC.PluginBase.Impl.ManifestLoaders;
using Prototype.ModularMVC.PluginBase;
using System.Text.Json;

namespace Prorotype.ModularMVC.PluginBase.Tests.ManifestLoaderTests;

public class ManifestLoaderTests
{
    private readonly MockFileSystem _mockFileSystem;
    private readonly ManifestLoader _manifestLoader;

    const string MANIFEST_DIRECTORY = "ExamplePlugin";
    const string MANIFEST_PATH = MANIFEST_DIRECTORY + "/plugin.json";

    public ManifestLoaderTests()
    {
        _mockFileSystem = new MockFileSystem();
        _manifestLoader = new ManifestLoader("./", _mockFileSystem);
    }

    #region LoadManifest Tests

    [Fact]
    public void LoadManifest_ShouldReturnNull_WhenRequiredFieldIsMissing()
    {
        const string MANIFEST = @"{
                ""name"": ""Test Plugin"",
                ""version"": ""1.0.0_425"",
                ""description"": ""Description""
            }";

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
    public void LoadManifest_ShouldReturnNull_WhenManifestIsInvalid()
    {
        const string MANIFEST = @"{
                ""id"" ""test.plugin"",
                ""name"": ""Test Plugin"",
                ""version"": ""1.0.0_425"",
                ""description"": ""Description
            }";

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
                ""assembly"": ""TestAssembly.dll""
            }";

        // Arrange
        var mockFileData = new MockFileData(MANIFEST);
        _mockFileSystem.AddFile(MANIFEST_PATH, mockFileData);

        // Act
        Manifest? result = _manifestLoader.LoadManifest(MANIFEST_PATH);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.plugin", result?.Id);
        Assert.Equal("Test Plugin", result?.Name);
        Assert.Equal("1.0.0_425", result?.Version);
        Assert.Equal("TestAssembly.dll", result?.Assembly);
        Assert.Null(result?.Description);
        Assert.Null(result?.Author);
        Assert.Null(result?.Website);
    }

    [Fact]
    public void LoadManifest_ShouldReturnManifest_WhenAllFieldsArePresent()
    {

        const string MANIFEST = @"{
                ""id"": ""test.plugin"",
                ""name"": ""Test Plugin"",
                ""version"": ""1.0.0_425"",
                ""assembly"": ""TestAssembly.dll"",
                ""description"": ""Test Plugin Description"",
                ""author"": ""me and myself"",
                ""website"": ""https://example.the1mason.com""
            }";

        // Arrange
        var mockFileData = new MockFileData(MANIFEST);
        _mockFileSystem.AddFile(MANIFEST_PATH, mockFileData);

        // Act
        Manifest? result = _manifestLoader.LoadManifest(MANIFEST_PATH);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.plugin", result?.Id);
        Assert.Equal("Test Plugin", result?.Name);
        Assert.Equal("1.0.0_425", result?.Version);
        Assert.Equal("TestAssembly.dll", result?.Assembly);
        Assert.Equal("Test Plugin Description", result?.Description);
        Assert.Equal("me and myself", result?.Author);
        Assert.Equal("https://example.the1mason.com", result?.Website);

    }

    [Fact]
    public void LoadManifest_ShouldReturnManifest_WhenContainsUnusedFields()
    {

        const string MANIFEST = @"{
                ""id"": ""test.plugin"",
                ""name"": ""Test Plugin"",
                ""version"": ""1.0.0_425"",
                ""assembly"": ""TestAssembly.dll"",
                ""description"": ""Test Plugin Description"",
                ""website"": ""https://example.the1mason.com"",
                ""xx__unused_field"": ""typo""
            }";

        // Arrange
        var mockFileData = new MockFileData(MANIFEST);
        _mockFileSystem.AddFile(MANIFEST_PATH, mockFileData);

        // Act
        Manifest? result = _manifestLoader.LoadManifest(MANIFEST_PATH);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.plugin", result?.Id);
        Assert.Equal("Test Plugin", result?.Name);
        Assert.Equal("1.0.0_425", result?.Version);
        Assert.Equal("TestAssembly.dll", result?.Assembly);
        Assert.Equal("Test Plugin Description", result?.Description);
        Assert.Equal("https://example.the1mason.com", result?.Website);

    }
    #endregion

    #region LoadManifests Tests

    [Fact]
    public void LoadManifests_ShouldReturnEmptyList_WhenNoManifestsFound()
    {
        // Arrange
        _mockFileSystem.AddDirectory(MANIFEST_DIRECTORY);

        // Act
        var result = _manifestLoader.LoadManifests();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }


    [Fact]
    public void LoadManifests_ShouldReturnEmptyList_WhenManifestsHaveWrongNames()
    {
        const string MANIFEST = @"{
            ""id"": ""test.plugin"",
            ""name"": ""Test Plugin"",
            ""version"": ""1.0.0_425"",
            ""assembly"": ""TestAssembly.dll""
        }";

        // Arrange
        _mockFileSystem.AddDirectory(MANIFEST_DIRECTORY);
        _mockFileSystem.AddFile(MANIFEST_DIRECTORY + "/notplugin.json", MANIFEST);

        // Act
        var result = _manifestLoader.LoadManifests();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void LoadManifests_ShouldReturnManifests_WhenManifestsArePresent()
    {
        const string MANIFEST = @"{
            ""id"": ""test.plugin"",
            ""name"": ""Test Plugin"",
            ""version"": ""1.0.0_425"",
            ""assembly"": ""TestAssembly.dll""
        }";

        // Arrange
        var mockFileData = new MockFileData(MANIFEST);
        _mockFileSystem.AddDirectory(MANIFEST_DIRECTORY);
        _mockFileSystem.AddFile(MANIFEST_PATH, mockFileData);

        // Act
        var result = _manifestLoader.LoadManifests();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("test.plugin", result[0].Id);
        Assert.Equal("Test Plugin", result[0].Name);
        Assert.Equal("1.0.0_425", result[0].Version);
        Assert.Equal("TestAssembly.dll", result[0].Assembly);
    }
        #endregion

}