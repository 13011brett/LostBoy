using System.Runtime.InteropServices;

namespace LostBoy.Core;

/// <summary>
/// Centralized input handling.
/// On Windows, uses GetAsyncKeyState for real-time polling (needed for the game loop).
/// Falls back to Console.KeyAvailable on other platforms.
/// </summary>
public static class Input
{
    [DllImport("user32.dll")]
    private static extern ushort GetAsyncKeyState(int vKey);

    private static readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    // Virtual key codes
    public const int VK_W = 0x57;
    public const int VK_A = 0x41;
    public const int VK_S = 0x53;
    public const int VK_D = 0x44;
    public const int VK_I = 0x49;
    public const int VK_K = 0x4B;
    public const int VK_R = 0x52;
    public const int VK_E = 0x45;
    public const int VK_ESCAPE = 0x1B;
    public const int VK_ENTER = 0x0D;
    public const int VK_UP = 0x26;
    public const int VK_DOWN = 0x28;

    /// <summary>
    /// Check if a key is currently pressed (non-blocking).
    /// </summary>
    public static bool IsKeyDown(int vKey)
    {
        if (_isWindows)
        {
            return (GetAsyncKeyState(vKey) & 0x8000) == 0x8000;
        }

        // Fallback for non-Windows
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);
            return key.Key == VirtualKeyToConsoleKey(vKey);
        }
        return false;
    }

    /// <summary>
    /// Check if a key is pressed, with an optional delay after detection.
    /// </summary>
    public static bool IsKeyDown(int vKey, int delayMs)
    {
        if (IsKeyDown(vKey))
        {
            if (delayMs > 0) Thread.Sleep(delayMs);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Read a line from console, clearing any buffered keys first.
    /// </summary>
    public static string ReadLineClean()
    {
        while (Console.KeyAvailable)
            Console.ReadKey(true);

        return Console.ReadLine() ?? "";
    }

    /// <summary>
    /// Read a menu choice (integer) from console.
    /// Returns 0 if input is invalid.
    /// </summary>
    public static int ReadChoice()
    {
        string input = ReadLineClean();
        return int.TryParse(input, out int result) ? result : 0;
    }

    private static ConsoleKey VirtualKeyToConsoleKey(int vKey) => vKey switch
    {
        VK_W => ConsoleKey.W,
        VK_A => ConsoleKey.A,
        VK_S => ConsoleKey.S,
        VK_D => ConsoleKey.D,
        VK_I => ConsoleKey.I,
        VK_K => ConsoleKey.K,
        VK_R => ConsoleKey.R,
        VK_E => ConsoleKey.E,
        VK_ESCAPE => ConsoleKey.Escape,
        VK_ENTER => ConsoleKey.Enter,
        VK_UP => ConsoleKey.UpArrow,
        VK_DOWN => ConsoleKey.DownArrow,
        _ => ConsoleKey.NoName,
    };
}
