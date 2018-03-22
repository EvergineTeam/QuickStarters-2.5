using System;
using WaveEngine.Common.Graphics;

namespace P2PTank
{
    public class GameConstants
    {
        // Game Time
        public static TimeSpan GamePlayTime = TimeSpan.FromSeconds(30); // Seconds

        // Entity Names
        public static string ManagerEntityPath = "manager";
     
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
            Color.LightSeaGreen
        };

        // MiniMap
        public static int MiniMapScale = 4;
        public static int MiniMapMargin = 24;
    }
}