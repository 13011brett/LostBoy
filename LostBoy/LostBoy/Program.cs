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

    public class Player //: character
    {
        public string name;
        private float health;
        private float speed;
        private float damage;
        private Vec3 location;
        private bool bMoving;

        public Player(string inName)
        {
            this.health = 100;
            this.speed = 1;
            this.damage = 1;
            this.location = new Vec3() { x = 0, y = 0, z = 0 };
            this.name = inName;
        }
        public void Movement()
        {
            this.location.x = 5;
        }
        
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your name: ");
            Player player = new Player(Console.ReadLine());
            

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


