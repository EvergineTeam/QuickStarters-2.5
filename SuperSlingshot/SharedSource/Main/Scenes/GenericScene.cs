#region Using Statements
using WaveEngine.Framework;
#endregion

namespace SuperSlingshot.Scenes
{
    public class GenericScene : Scene
    {
        private readonly string content;

        public GenericScene(string content) : base()
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
