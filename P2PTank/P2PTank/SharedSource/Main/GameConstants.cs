using WaveEngine.Common.Graphics;

namespace P2PTank
{
    public class GameConstants
    {
        // Entity Names
        // Game Scene
        public static string ManagerEntityPath = "manager";
        public static string MapEntityPath = "gamearena.map";
        public static string SpawnPointPathFormat = "gamearena.spawnpoints.spawn{0:00}";

        // TiledMap names
        public static string TiledMapBordersLayerName = "borders";

        // Tank Entity
        public static string EntityNameTankBody = "body";
        public static string EntitynameTankBarrel = "barrel";

        // Tag
        public static string TagCollider = "collider";

        // Color Palette
        public static Color[] Palette = new Color[]{
            Color.Green,
            Color.Red,
            Color.Orange,
            Color.Violet,
            Color.Yellow,
            Color.Pink,
            Color.Blue,

            Color.Chartreuse,
            Color.Firebrick,
            Color.LawnGreen,
            Color.Sienna,
            Color.SlateGray,
            Color.Coral,
            Color.IndianRed,
            Color.LightSeaGreen,
        };
    }
}