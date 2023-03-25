namespace Saucery.Core.Util;

public sealed class IdGenerator
{
    private readonly string _theId;
    public static IdGenerator Instance { get; } = new();
    
    public static string Id => Instance._theId;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static IdGenerator()
    {
    }

    private IdGenerator()
    {
        _theId ??= Guid.NewGuid().ToString();
    }

    
}