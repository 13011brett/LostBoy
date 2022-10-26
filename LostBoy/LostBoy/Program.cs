using LostBoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LostBoy
{
    struct Vec3 { public float x; public float y; public float z; };
    
    public class Story
    {
        public string introduction = "On a moonlit night, unlike any other, you stand outside of a breathtaking castle. You take in the air as you stare at the water that separates the land. " +
            "\"What a beautiful sight! I wonder what this castle was like before it was abandoned..\" You think to yourself. As soon as that thought finishes playing in your mind, a sudden chill " +
            "goes throughout your body, completely freezing you to a a halt. People pass by you as you stand there in shock, not able to move a muscle, other than your eyes. Your attention is quickly " +
            "diverted to the sight of a malformed castle, being tarnished by a fantastical beast. You can't believe your eyes; you faint in almost an instant... \n\n\n\n " +
            "You wake up in a dimly lit room; the air reeks of damp laundry. Something is eerily familiar about this place, but you can't piece it together in the shocked state you are in.\n" +
            "You attempt to push yourself off of the ground, and quickly realize your arms are chained to the floor behind you. You begin to panic further, trying to pull yourself off of the rusty chains. " +
            "As you begin to scream \"LET ME OUT!\" A voice comes from the distance,\n Unknown Man: \"Hush! You'll wake the beasts..\" \n\n" +
            "You freeze dead in your tracks, chains in hand, paralyzed as you were when you saw the fortress being destroyed.";
    }
    public class Player //: character
    {

        public string name;
        private float health;
        private float damage;
        private Vec3 location;
        private bool bMoving;

        public Player(string inName)
        {
            this.health = 100;
            this.damage = 1;
            this.location = new Vec3() { x = 0, y = 0, z = 0 };
            this.name = inName;
        }
        public void Movement(string option)
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
            }
        }
        
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your name: ");
            Player player = new Player(Console.ReadLine());
            Story t = new Story();
            Console.WriteLine(t.introduction);
            Console.ReadLine();
            

        }
    }
}

public interface character
{
    // Fight
    // Movement
    // Inventory (maybe just local to Player)
    //

}


