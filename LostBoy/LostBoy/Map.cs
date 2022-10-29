using System.Collections.Specialized;
using System.Security.Cryptography;

public class Map
{
    private Player.Vec3 mapSize;
    private int mapDifficulty;
    public Enemy[] enemies = new Enemy[10];


    public Player.Vec3 MapSize
    {
        get { return mapSize; }
    }
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
    public void EnemyCreation(Map map) // All enemy creation will be done within the map, currently works off of map difficulty but can be modularized. 
    {
        Enemy[] enemy = new Enemy[map.mapDifficulty];
        for (int i = 0; i < map.mapDifficulty; i++)
        {
            Player.Vec3 loc;
            
            loc.x = Player.RandomNumber(1, (int)map.MapSize.x);  // rand x for monster
            loc.y = Player.RandomNumber(1, (int)map.MapSize.y);  // rand y for monster
            loc.z = MapSize.z; // z can be initialized within the map.
            enemy[i] = new Enemy(map, loc); 
            map.enemies[i] = enemy[i]; // possibly need to do another for loop after this to detect for monsters on duplicate spaces.
            
      


        }
    }

}