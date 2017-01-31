namespace Match3.Gameboard
{
    public struct Coordinate
    {
        public int X;

        public int Y;

        public override string ToString()
        {
            return $"[X={this.X}, Y={this.Y}]";
        }
    }
}