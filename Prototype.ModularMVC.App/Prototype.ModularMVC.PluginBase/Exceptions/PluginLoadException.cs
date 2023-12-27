
namespace Prototype.ModularMVC.PluginBase.Exceptions;
public class PluginLoadException : Exception
{
    public string? Path { get; }
    public PluginLoadException()
    {
    }

    public PluginLoadException(string? message) : base(message)
    {
    }

    public PluginLoadException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public PluginLoadException(string? message, string? path, Exception? innerException) : base(message, innerException)
    {
        @Path = path;
    }

}
