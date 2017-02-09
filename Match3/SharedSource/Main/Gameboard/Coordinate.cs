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

        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            return c1.X == c2.X && 
                   c1.Y == c2.Y;
        }

        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return c1.X != c2.X ||
                   c1.Y != c2.Y;
        }
    }
}