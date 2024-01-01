using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.PluginBase.Impl.ManifestLoaders;
public class ManifestLoader : IManifestLoader
{
    public string LookupDirectory { get; }

    private IFileSystem _fileSystem { get; }

    public ManifestLoader(string lookupDirectory, IFileSystem fileSystem)
    {
        LookupDirectory = lookupDirectory;
        _fileSystem = fileSystem;
    }

    public virtual List<Manifest> LoadManifests()
    {
        var manifestsPath = _fileSystem.Directory.GetFiles(LookupDirectory, "plugin.json", SearchOption.AllDirectories);

        List<Manifest> manifests = [];

        foreach (var path in manifestsPath)
        {
            Manifest? manifest = LoadManifest(path);
            if (manifest != null)
            {
                manifest.Path = path;
                manifests.Add(manifest);
            }
        }
        return manifests;
    }

    public Manifest? LoadManifest(string path)
    {
        try
        {
            string json = _fileSystem.File.ReadAllText(path);
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Manifest>(json, options);
        }
        catch (JsonException ex)
        {
            Log.Warning(ex, "Failed to deserialize manifest file {Path}", path);
            return null;
        }
    }
}
