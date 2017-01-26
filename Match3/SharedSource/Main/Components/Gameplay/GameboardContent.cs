using Match3.Factories;
using Match3.Gameboard;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Models.Assets;
using WaveEngine.Framework.Services;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class GameboardContent : Component
    {
        [RequiredComponent]
        protected Transform2D transform;

        protected override void Initialize()
        {
            base.Initialize();

            //var m = 6;
            //var n = 6;
            //var distanceBtwItems = 150;
            
            //var mByTwo = (m / 2f);
            //var nByTwo = (n / 2f);

            //int index = 0;
            //for (float i = -mByTwo + 0.5f; i < mByTwo + 0.5f; i++)
            //{
            //    for (float j = -nByTwo + 0.5f; j < nByTwo + 0.5f; j++)
            //    {
            //        var position = new Vector2(i * distanceBtwItems, j * distanceBtwItems);

            //        var random = WaveServices.Random;
            //        var allTypes = Enum.GetValues(typeof(CandyTypes));
            //        var allColors = Enum.GetValues(typeof(CandyColors));
            //        var randomType = (CandyTypes)allTypes.GetValue(random.Next(allTypes.Length));
            //        var randomColor = (CandyColors)allColors.GetValue(random.Next(allColors.Length));

            //        var candyEntity = GameplayFactory.CreateCandy(this.Owner.Scene, position, randomType, randomColor);
            //        candyEntity.Name += index++;
            //        this.Owner.AddChild(candyEntity);
            //    }
            //}
        }

        public void RenderGameboard(Board board)
        {
            var m = board.CurrentStatus.Length;
            var n = board.CurrentStatus[0].Length;

            var distanceBtwItems = 150;

            var mByTwo = (m / 2f);
            var nByTwo = (n / 2f);

            int indexI = 0;
            int indexJ = 0;
            for (float i = -mByTwo + 0.5f; i < mByTwo + 0.5f; i++)
            {
                for (float j = -nByTwo + 0.5f; j < nByTwo + 0.5f; j++)
                {
                    var position = new Vector2(i * distanceBtwItems, j * distanceBtwItems);
                    var candy = board.CurrentStatus[indexI][indexJ];

                    var candyEntity = GameplayFactory.CreateCandy(this.Owner.Scene, position, candy.Type, candy.Color);
                    candyEntity.Name += indexI + "_" + indexJ;
                    this.Owner.AddChild(candyEntity);
                    indexJ++;
                }
                indexI++;
            }
        }
    }
}
