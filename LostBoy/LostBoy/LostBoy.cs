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


// Mostly everything in this was my own implementation, for better or for worse. I wanted to challenge myself to see how far I can get off of my own ideas in crafting
// my own game. More so a fun project than anything else.



namespace LostBoy
{

    
    internal class LostBoy
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Clear();

            Player player = new Player();
            Story.TimedText(Story.introduction, 1, true);
            Console.ForegroundColor = ConsoleColor.Green;
            player.GetName();
            Story.afterIntro(player.Name);
            
            Map Dungeon = new Map(100, 30, 10); // Testing Map One
            Enemy enemy = new Enemy(Dungeon);
            Dungeon.EnemyCreation(Dungeon);// Testing an enemy within the map.


            Console.Clear();
            Map.FillBorder('=');
            //Map.FillScreen('█');

            Map.EnemiesToScreen('o', Dungeon);
            Map.ScreenMovement(player, Dungeon); // Need to rename function, this is the main gameplay loop.
            Console.Clear();
            Console.ReadLine();
            Map.EnemiesToScreen('o', Dungeon);
            Map.ScreenMovement(player, Dungeon);
            //Console.SetCursorPosition((Console.WindowWidth/2), (Console.WindowHeight-5));



            //Console.ReadLine();



        }
    }
}
    



