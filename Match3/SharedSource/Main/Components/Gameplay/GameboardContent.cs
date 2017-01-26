using Match3.Factories;
using Match3.Gameboard;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int DistanceBtwItems = 150;

        [RequiredComponent]
        protected Transform2D transform;

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void RegenerateGameboard(Board board)
        {
            foreach (var child in this.Owner.ChildEntities.ToList())
            {
                this.Owner.RemoveChild(child.Name);
            }
            
            var mByTwo = (board.SizeM / 2f);
            var nByTwo = (board.SizeN / 2f);

            int indexI = 0;
            for (float i = -mByTwo + 0.5f; i < mByTwo + 0.5f; i++)
            {
                int indexJ = 0;
                for (float j = -nByTwo + 0.5f; j < nByTwo + 0.5f; j++)
                {
                    var position = new Vector2(i * DistanceBtwItems, j * DistanceBtwItems);
                    var candy = board.CurrentStatus[indexI][indexJ];

                    //        var random = WaveServices.Random;
                    //        var allTypes = Enum.GetValues(typeof(CandyTypes));
                    //        var allColors = Enum.GetValues(typeof(CandyColors));
                    //        var randomType = (CandyTypes)allTypes.GetValue(random.Next(allTypes.Length));
                    //        var randomColor = (CandyColors)allColors.GetValue(random.Next(allColors.Length));

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
