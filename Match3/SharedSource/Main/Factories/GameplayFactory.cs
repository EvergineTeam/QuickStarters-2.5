using Match3.Gameboard;
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace Match3.Factories
{
    public static class GameplayFactory
    {
        private static Dictionary<CandyTypes, Dictionary<CandyColors, string>> candyTextures = new Dictionary<CandyTypes, Dictionary<CandyColors, string>>
        {
            {
                CandyTypes.Regular, new Dictionary<CandyColors, string>
                {
                    { CandyColors.Blue, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.bean_blue },
                    { CandyColors.Green, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.bean_green },
                    { CandyColors.Red, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.bean_red },
                    { CandyColors.Yellow, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.bean_yellow }
                }
            },
            {
                CandyTypes.FourInLine, new Dictionary<CandyColors, string>
                {
                    { CandyColors.Blue, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.wrappedsolid_blue},
                    { CandyColors.Green, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.wrappedsolid_green },
                    { CandyColors.Red, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.wrappedsolid_red },
                    { CandyColors.Yellow, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.wrappedsolid_yellow }
                }
            },
            {
                CandyTypes.FourInSquare, new Dictionary<CandyColors, string>
                {
                    { CandyColors.Blue, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.lollipop_blue },
                    { CandyColors.Green, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.lollipop_green },
                    { CandyColors.Red, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.lollipop_red },
                    { CandyColors.Yellow, WaveContent.Assets.GUI.Candies_spritesheet_TextureName.lollipop_yellow }
                }
            }
        };

        public static Entity CreateCandy(Scene scene, Vector2 localPosition, CandyTypes candyType, CandyColors candyColor)
        {
            var candyEntity = scene.EntityManager.Instantiate(WaveContent.Prefabs.Gameplay.Candy);
            candyEntity.IsSerializable = false;

            var transform = candyEntity.FindComponent<Transform2D>();
            transform.LocalPosition = localPosition;

            var spriteAtlas = candyEntity.FindComponent<SpriteAtlas>();
            spriteAtlas.TextureName = candyTextures[candyType][candyColor];

            var animation2D = candyEntity.FindChild("Particles").FindComponent<Animation2D>();
            switch (candyColor)
            {
                case CandyColors.Red:
                    animation2D.CurrentAnimation = "ExplosionPink";
                    break;
                case CandyColors.Yellow:
                case CandyColors.Green:
                    animation2D.CurrentAnimation = "ExplosionGreen";
                    break;
                case CandyColors.Blue:
                    animation2D.CurrentAnimation = "ExplosionBlue";
                    break;
                default:
                    throw new InvalidOperationException("Invalid candy color");
            }

            return candyEntity;
        }
    }
}
