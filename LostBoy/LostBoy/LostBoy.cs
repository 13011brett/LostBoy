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
            Console.ForegroundColor = ConsoleColor.Red;
            player.GetName();
            Story.afterIntro(player.Name);
            Map Dungeon = new Map(10, 10, 2); // Testing Map One
            Enemy enemy = new Enemy(Dungeon);
            Dungeon.EnemyCreation(Dungeon);// Testing an enemy within the map.

            
            Console.Clear();
            Map.FillScreen('█');

            Console.WriteLine(enemy.Monster);
            Console.ReadLine();



        }
    }
}
    



