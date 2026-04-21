using System.Runtime.InteropServices;

namespace LostBoy.Core;

public static class Input
{
    [DllImport("user32.dll")]
    private static extern ushort GetAsyncKeyState(int vKey);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    private static readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

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

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    public static bool IsConsoleFocused()
    {
        if (!_isWindows) return true;
        try
        {
            IntPtr fg = GetForegroundWindow();
            if (fg == IntPtr.Zero) return false;

            // Check if the foreground window belongs to our process.
            // This works with both conhost and Windows Terminal.
            GetWindowThreadProcessId(fg, out uint fgPid);
            return fgPid == (uint)Environment.ProcessId;
        }
        catch { return true; }
    }

    public static bool IsKeyDown(int vKey)
    {
        if (_isWindows)
        {
            return (GetAsyncKeyState(vKey) & 0x8000) == 0x8000;
        }
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);
            return key.Key == VirtualKeyToConsoleKey(vKey);
        }
        return false;
    }

    public static bool IsKeyDown(int vKey, int delayMs)
    {
        if (IsKeyDown(vKey))
        {
            if (delayMs > 0) Thread.Sleep(delayMs);
            return true;
        }
        return false;
    }

    public static void ConsumeKey(int vKey, int timeoutMs = 500)
    {
        int waited = 0;
        while (IsKeyDown(vKey) && waited < timeoutMs)
        {
            Thread.Sleep(10);
            waited += 10;
        }
        Thread.Sleep(50);
    }

    public static void FlushKeys()
    {
        while (Console.KeyAvailable)
            Console.ReadKey(true);
    }

    public static string ReadLineClean()
    {
        FlushKeys();
        return Console.ReadLine() ?? "";
    }

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