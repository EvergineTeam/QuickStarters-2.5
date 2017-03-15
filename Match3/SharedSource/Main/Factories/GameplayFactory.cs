using Match3.Gameboard;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace Match3.Factories
{
    public static class GameplayFactory
    {
        public static Entity CreateCandy(Scene scene, Vector2 localPosition, CandyTypes candyType, CandyColors candyColor)
        {
            var candyEntity = scene.EntityManager.Instantiate(WaveContent.Prefabs.Gameplay.Candy);
            candyEntity.IsSerializable = false;

            var transform = candyEntity.FindComponent<Transform2D>();
            transform.LocalPosition = localPosition;

            var spriteAtlas = candyEntity.FindComponent<SpriteAtlas>();
            spriteAtlas.TextureName = GetCandyTextureTypeByType(candyType, candyColor);

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

        private static string GetCandyTextureTypeByType(CandyTypes candyType, CandyColors candyColor)
        {
            string typeString;

            switch (candyType)
            {
                case CandyTypes.Regular:
                    typeString = "bean";
                    break;
                case CandyTypes.FourInLine:
                    typeString = "wrappedsolid";
                    break;
                case CandyTypes.FourInSquare:
                    typeString = "lollipop";
                    break;
                default:
                    typeString = "jelly";
                    break;
                    //throw new InvalidOperationException("Invalid candy type");
            }

            var colorString = candyColor.ToString().ToLowerInvariant();
            return string.Format("{0}_{1}", typeString, colorString);
        }
    }
}
