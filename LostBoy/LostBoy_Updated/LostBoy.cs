using LostBoy;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LostBoy.Items;


// Mostly everything in this was my own implementation, for better or for worse. I wanted to challenge myself to see how far I can get off of my own ideas in crafting
// my own game. More so a fun project than anything else.



namespace LostBoy
{

    
    internal class LostBoy
    {
        // Documentation: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-deletemenu
        private const int MF_BYCOMMAND = 0x00000000;

        // Documentation: https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand
        public const int SC_SIZE = 0xF000;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;

        // Documentation: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportattribute?view=netcore-3.1
        [DllImport("user32.dll")]
        // Documentation: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-deletemenu
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        // Documentation: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmenu
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        // Documentation: https://docs.microsoft.com/en-us/windows/console/getconsolewindow
        private static extern IntPtr GetConsoleWindow();
        static void Main(string[] args)
        {

            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);        // Disable resizing
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);    // Disable minimizing
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);    // Disable maximizing
            
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Clear();
            Console.SetWindowSize((Console.LargestWindowWidth / 2), (Console.LargestWindowHeight - 10));

            Player player = new Player();
            //player.playerInventory.AddItem(new Chainmail(), 1);
            //player.playerInventory.AddItem(new Chainmail(), 1);
            //player.playerInventory.AddItem(new Chainmail(), 1);
            //player.playerInventory.AddItem(new Chainmail(), 1);
            player.playerInventory.AddItem(new Chainmail(), 1);
            player.playerInventory.AddItem(new Chainmail(), 1);
            player.playerInventory.AddItem(new Potion(), 2);
            Story.DoIntro(ref player);
            //Story.TimedText(Story.introduction, 1, true);
            Console.ForegroundColor = ConsoleColor.Green;

            
            Map Dungeon = new Map(200, 20, 10); // Testing Map One
            Map Castle = new Map(100, 50, 30);
            Map.DrawMap(Dungeon, player);
            Console.Clear();
            Map.DrawMap(Castle, player);

            
            //Console.SetCursorPosition((Console.WindowWidth/2), (Console.WindowHeight-5));



            //Console.ReadLine();



        }
    }
}
    



