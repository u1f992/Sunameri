namespace Narwhal;

/// <summary>
/// Represents messages for special operations.
/// </summary>
public static class Special
{
    /// <summary>
    /// Run the servo motor to press the reset button.
    /// <see href="https://github.com/mizuyoukanao/WHALE/blob/f8620dd746854babd635b8ed21e728afead16522/WHALE/WHALE.ino#L207-L211"/>
    /// </summary>
    public const char Reset = '@';
    /// <summary>
    /// Do not send the message.<br/>
    /// Useful for representing the waiting time.
    /// </summary>
    public const char Empty = ' ';
}
