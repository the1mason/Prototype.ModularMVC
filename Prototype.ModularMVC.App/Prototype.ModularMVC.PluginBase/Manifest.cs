using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.PluginBase;
internal class Manifest
{
    [JsonRequired]
    public string Id { get; set; } = null!;
    [JsonRequired]
    public string Name { get; set; } = null!;
    [JsonRequired]
    public string Version { get; set; } = null!;
    public string? Description { get; set; } 
    public string? Author { get; set; }
    public string? Website { get; set; }
    [JsonRequired]
    public string Assembly { get; set; } = null!;
    public ManifestDependency[]? Dependencies { get; set; }
}

internal class ManifestDependency
{
    public string? Id { get; set; }
    public string? Version { get; set; }

    [JsonRequired]
    public string Assembly { get; set; } = null!;
}
