using System.Runtime.InteropServices;

public static class Keyboad
{
    [DllImport("USER32.dll")] public static extern short GetKeyState(Key key);
    [DllImport("USER32.dll")] public static extern short GetKeyState(char key);
}
public enum Key
{
    VK_RETURN = 0x0D,
    VK_CONTROL = 0x11,
    VK_ESCAPE = 0x1B,
    VK_SPACE = 0x20,
    VK_LEFT = 0x25,
    VK_UP = 0x26,
    VK_RIGHT = 0x27,
    VK_DOWN = 0x28
}
