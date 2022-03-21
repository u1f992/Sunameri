namespace Narwhal;

/// <summary>
/// Represents messages to tilt the y-axis of the Control Stick.
/// <see href="https://github.com/mizuyoukanao/WHALE/blob/f8620dd746854babd635b8ed21e728afead16522/WHALE/WHALE.ino#L155-L166"/>
/// </summary>
public static class yAxis
{
    /// <summary>
    /// Tilt to the left.
    /// </summary>
    public const char _0 = '2';
    /// <summary>
    /// Return to the center.
    /// </summary>
    public const char _128 = '3';
    /// <summary>
    /// Tilt to the right.
    /// </summary>
    public const char _255 = '4';
}
