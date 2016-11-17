#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using SuperSlingshot.Behaviors;
using SuperSlingshot.Components;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.ImageEffects;
using WaveEngine.TiledMap;
#endregion

namespace SuperSlingshot.Scenes
{
    public class BackgroundScene : Scene
    {
        private readonly string content;

        public BackgroundScene(string content) : base()
        {
            this.content = content;
        }

        protected override void CreateScene()
        {
            this.Load(this.content);
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}
