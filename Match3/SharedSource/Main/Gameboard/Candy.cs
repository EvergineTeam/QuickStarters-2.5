using System;
using System.Collections.Generic;
using System.Text;

namespace Match3.Gameboard
{
    public struct Candy
    {
        public CandyTypes Type { get; set; }

        public CandyColors Color { get; set; }

        public override string ToString()
        {
            return $"Type={this.Type}, Color={this.Color}";
        }
    }
}
