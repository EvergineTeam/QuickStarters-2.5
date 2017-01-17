namespace Match3.Game
{
    public class Board
    {
        private int[] pointsByStar;

        private BoardGenerator generator;

        private Candy[][] currentStatus;

        public Candy[][] CurrentStatus
        {
            get { return this.currentStatus; }
        }

        public Board(int sizeN, int sizeM, int[] pointsByStar)
        {
            this.pointsByStar = pointsByStar;
            this.generator = new BoardGenerator();
            this.currentStatus = this.generator.Generate(sizeN, sizeM);
            this.ShuffleIfNecessary();
        }

        private void ShuffleIfNecessary()
        {
            while (!this.CurrentStatusIsValid())
            {
                this.Shuffle();
            }
        }

        private bool CurrentStatusIsValid()
        {
            return this.HasMovements() && !this.HasOperations();
        }

        private bool HasMovements()
        {
            // TODO Returns true if there are possible movements in the board
            return true;
        }

        private bool HasOperations()
        {
            // TODO Returns true if the board has pending operations
            return false;
        }

        private void Shuffle()
        {
            // TODO Shuffle the board
        }

        public object[] Move(Coordinate candyPosition, CandyMoves move)
        {
            // TODO Move the candy and return the board operations
            return null;
        }
    }
}
