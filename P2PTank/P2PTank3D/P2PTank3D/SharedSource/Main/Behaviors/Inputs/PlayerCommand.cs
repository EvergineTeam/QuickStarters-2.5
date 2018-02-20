using WaveEngine.Common.Math;

namespace P2PTank.Behaviors.Inputs
{
    public struct PlayerCommand
    {
        public float Move { get; private set; }

        public Vector2 AbsoluteMove { get; private set; }

        public float Rotate { get; private set; }

        public bool Shoot { get; private set; }

        public void SetMove(float move)
        {
            this.Move = MathHelper.Clamp(this.Move + move, -1.0f, 1.0f);
            var temp = this.AbsoluteMove;
            temp.Y = this.Move;
            this.AbsoluteMove = temp;
        }

        public void SetRotate(float rotate)
        {
            this.Rotate = MathHelper.Clamp(this.Rotate + rotate, -1.0f, 1.0f);
            var temp = this.AbsoluteMove;
            temp.X = this.Rotate;
            this.AbsoluteMove = temp;
        }

        public void SetShoot()
        {
            this.Shoot = true;
        }
    }
}