namespace Narwhal;

/// <summary>
/// Represents messages to tilt the x-axis of the Control Stick.
/// <see href="https://github.com/mizuyoukanao/WHALE/blob/f8620dd746854babd635b8ed21e728afead16522/WHALE/WHALE.ino#L143-L154"/>
/// </summary>
public static class xAxis
{
    /// <summary>
    /// Tilt to the left.
    /// </summary>
    public const char _0 = 'y';
    /// <summary>
    /// Return to its center.
    /// </summary>
    public const char _128 = 'z';
    /// <summary>
    /// Tilt to the right.
    /// </summary>
    public const char _255 = '1';
}
