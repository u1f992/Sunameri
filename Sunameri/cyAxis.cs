namespace Sunameri;

/// <summary>
/// Represents messages to tilt the y-axis of the C Stick.
/// <see href="https://github.com/mizuyoukanao/WHALE/blob/f8620dd746854babd635b8ed21e728afead16522/WHALE/WHALE.ino#L179-L190"/>
/// </summary>
public static class cyAxis
{
    /// <summary>
    /// Tilt to the left.
    /// </summary>
    public const char _0 = '8';
    /// <summary>
    /// Return to its center.
    /// </summary>
    public const char _128 = '9';
    /// <summary>
    /// Tilt to the right.
    /// </summary>
    public const char _255 = '0';
}
