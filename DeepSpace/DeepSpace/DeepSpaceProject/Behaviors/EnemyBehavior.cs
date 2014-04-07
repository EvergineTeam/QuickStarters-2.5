#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services; 
#endregion

namespace DeepSpaceProject.Behaviors
{
    public class EnemyBehavior : Behavior
    {
        private static System.Random rnd = new System.Random();

        [RequiredComponent]
        private Transform2D transform;

        private float speed;
        private int deepPosition;
        private int difficultyLevel;

        public EnemyBehavior()
            : base()
        {
            transform = null;
            speed = 2;
            deepPosition = (int)-WaveServices.ViewportManager.VirtualHeight * 3;
        }

        protected override void Update(TimeSpan gameTime)
        {
            float time = (float)gameTime.TotalMilliseconds / 10;

            transform.Y += speed * time;

            // Reset position
            if(transform.Y > WaveServices.ViewportManager.VirtualHeight)
            {
                difficultyLevel++;
                transform.Y = deepPosition;
                transform.X = rnd.Next((int)WaveServices.ViewportManager.VirtualWidth);
                
                speed = speed + (float)rnd.NextDouble() / 2;

                if(speed > 7)
                {
                    speed = rnd.Next(2, 5);
                }
            }
        }
    }
}
