namespace Narwhal;

/// <summary>
/// Represents messages to tilt the x-axis of the C Stick.
/// <see href="https://github.com/mizuyoukanao/WHALE/blob/f8620dd746854babd635b8ed21e728afead16522/WHALE/WHALE.ino#L167-L178"/>
/// </summary>
public static class cxAxis
{
    /// <summary>
    /// Tilt to the left.
    /// </summary>
    public const char _0 = '5';
    /// <summary>
    /// Return to the center.
    /// </summary>
    public const char _128 = '6';
    /// <summary>
    /// Tilt to the right.
    /// </summary>
    public const char _255 = '7';
}
