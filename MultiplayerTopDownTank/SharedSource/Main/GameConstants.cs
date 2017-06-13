namespace MultiplayerTopDownTank
{
    public class GameConstants
    {
        // Map
        public static string EntityMap = "Map";
        public static string MapColliderEntity = "MapColliderEntity";
        public static string SpawnPointPrefix = "SpawnPoint";
        public static string SpawnCount = "SpawnCount";

        // Map Layers
        public static string PhysicLayer = "PhysicLayer";
        public static string AnchorLayer = "AnchorLayer";
    
        // Player
        public static string Player = "Player";
        public static string PlayerBarrel = "Barrel";
        public static string TagCollider = "collider";
        public static string TankFactory = "TankFactory";
        public static string TankTag = "tank";

        // Manager
        public static string Manager = "Manager";

        // Bullet
        public static string BulletTag = "bullet";
        public static int BulletPoolSize = 10;
        public static string BulletFactory = "BulletFactory";
        public static int BulletDamage = 25;

        // Hud
        public static string Hud = "Hud";
    }
}
