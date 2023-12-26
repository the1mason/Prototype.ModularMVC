namespace Prototype.ModularMVC.App.Server.PluginBase.Enums;

public enum Priority : byte
{
    Lowest = byte.MinValue,

    Low = 64,

    Normal = 128,

    High = 192,

    Highest = byte.MaxValue
}
