using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;

namespace P2PTank
{
    public class GameConstants
    {
        // Entity Names
        // Game Scene
        public static string ManagerEntityPath = "manager";

        // Tank Entity
        public static string EntityNameTankBody = "body";
        public static string EntitynameTankBarrel = "barrel";

        public static Color[] Palette = new Color[]{
            Color.Red,
            Color.Blue,
            Color.Green,
            Color.Orange,
            Color.Violet,
            Color.Pink,
            Color.Yellow
        };
    }
}
