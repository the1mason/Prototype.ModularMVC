using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.PluginBase;
public interface IManifestLoader
{
    string LookupDirectory { get; }
    List<Manifest> LoadManifests();
    Manifest? LoadManifest(string path);

}
