﻿using LostBoy;
using System;
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

    struct Vec3 { public float x; public float y; public float z; }; // Z May be used just to dictate the level we're on? Not quite sure. Going to be a 2d game currently.
                                                                     //    struct Vec2 { public float x; public float y;};



    public class Story // Not sure if making a story object is better than instantiating within a player object. Just didn't want to do this everytime or have this derived in other classes.
    {
        public string introduction = "On a moonlit night, unlike any other, you stand outside of a breathtaking castle. You take in the air as you stare at the water that separates the land. " +
            "\"What a beautiful sight! I wonder what this castle was like before it was abandoned..\" You think to yourself. As soon as that thought finishes playing in your mind, a sudden chill " +
            "goes throughout your body, completely freezing you to a a halt. People pass by you as you stand there in shock, not able to move a muscle, other than your eyes. Your attention is quickly " +
            "diverted to the sight of a malformed castle, being tarnished by a fantastical beast. You can't believe your eyes; you faint in almost an instant... \n\n\n\n " +
            "You wake up in a dimly lit room; the air reeks of damp laundry. Something is eerily familiar about this place, but you can't piece it together in the shocked state you are in.\n" +
            "You attempt to push yourself off of the ground, and quickly realize your arms are chained to the floor behind you. You begin to panic further, trying to pull yourself off of the rusty chains. " +
            "As you begin to scream \"LET ME OUT!\" A voice comes from the distance,\n\nUnknown Man: \"Hush! You'll wake the beasts..\"\n\n" +
            "You freeze dead in your tracks, chains in hand, paralyzed as you were when you saw the fortress being destroyed. Your eyes slowly shift to the center of the room, where you can see the man" +
            " sitting at a dusty table, with some strange contraptions upon it. Your body, without your control, starts moving towards the table, but are quickly stopped by the chains.. Or so you thought. " +
            "The man had done something with the chains, now they are dragging on the floor behind you as you begin to sit at the only other chair at the table, against your own will.\n\n" +
            "Strangely enough, the man reaches out to hold one of your hands, and proceeds to speak. \nUnknown Man: \"I've been alone for so long... Please, tell me, what is your name?\"\n";
    }

    public class Map
    {
        private Vec3 mapSize;
        private int mapDifficulty;
        public int MapDifficulty
        {
            get
            {
                return mapDifficulty;
            }
            set
            {
                mapDifficulty = value;
            }
        }


        public Map(float map_x, float map_y, int difficulty)
        {
            this.mapSize.x = map_x;
            this.mapSize.y = map_y;
            this.mapDifficulty = difficulty;
        }
    }


        public class Enemy : Player
        {
            public Enemy(Map map)
            {
                this.level = (map.MapDifficulty); // Difficulty of map = monster level (? might not be best.).
                this.Health = ((this.level * RandomNumber(1, 3)) + 100);
            }
        }
    
    public class Player : ICharacter
    {


        [DllImport("user32.dll")]
        internal static extern ushort GetAsyncKeyState(int vKey); // Used for getting keys pressed.


        private string name;
        private float health;
        private float damage;
        private Vec3 location;
        private bool bMoving;
        public int level = 1;
        private int experience = 0;
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public Player(string inName)
        {
            this.health = 100;
            this.damage = 1;
            this.location = new Vec3() { x = 0, y = 0, z = 0 };
            this.name = inName;
        }
        public Player()
        {
            this.health = 100;
            this.damage = 1;
            this.location = new Vec3() { x = 0, y = 0, z = 0 };
            this.name = "Name Not Set. Now, how did that happen?\n";
        }
        public void GetName()
        {
            do
            {
                Console.Write("Enter your name: ");
                this.name = Console.ReadLine();
            } while (this.name == "");
        }


        public float RandomNumber(float lowerRange, float upperRange)
        {
            Random random = new Random();
            return (float)random.NextDouble() + random.Next((int)lowerRange, (int)upperRange);


        }
        public void Movement(string option) // Movement for now, may do something like getting ASYNC KeyState
        {
            switch (option)
            {
                case "d":
                    Console.WriteLine("You have moved to the right.");
                    this.location.x++;
                    break;
                case "a":
                    Console.WriteLine("You have moved to the left.");
                    this.location.x--;
                    break;
                case "w":
                    Console.WriteLine("You have moved forward.");
                    this.location.y++;
                    break;
                case "s":
                    Console.WriteLine("You have moved backward.");
                    this.location.y--;
                    break;
                default:
                    Console.WriteLine("\n\n Not a proper Movement Key. \n\n");
                    break;
            }
        }
        public void TimedText(string inText, int inSpeed = 50, bool clearConsole = false) // This allows for text to be timed, need to make a skippable feature into this as well, and make it print out the text as a whole if so.
        {

            for (int i = 1; i <= inText.Length; i++)
            {
                if (clearConsole) // Clear console if chosen, otherwise false.
                {
                    Console.Clear();
                    clearConsole = false;
                }

                Console.Write(inText[i - 1]); // Main loop of execution for text, takes it one char at a time. Like a char array. -1 to account for array starting at 0.
                System.Threading.Thread.Sleep(inSpeed);

                if ((GetAsyncKeyState(27) & 0x8000) == 0x8000) // If key pressed, clear the text, and output the text as a whole. 27 = Escape key. 
                {
                    Console.WriteLine(inText.Substring(i, (inText.Length - i)));
                    break;
                };

            }
        }



    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Clear();
            Player player = new Player();
            Story story = new Story();
            player.TimedText(story.introduction, 10, true);
            player.GetName();
            Map Dungeon = new Map(10, 10, 100); // Testing Map One
            Enemy enemy = new Enemy(Dungeon); // Testing an enemy within the map.
            Console.WriteLine(enemy.Health);
            Console.ReadLine();



        }
    }
}
    

public interface ICharacter
{
    // Fight
    void Movement(string Ioption);
    // Inventory (maybe just local to Player)
    //

}


