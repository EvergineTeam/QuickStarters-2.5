using System.Collections.Generic;
using System.Linq;

namespace Match3.Gameboard
{
    public class BoardOperation
    {
        public OperationTypes Type { get; set; }

        public List<CandyOperation> CandyOperations { get; private set; }

        public BoardOperation()
        {
            this.CandyOperations = new List<CandyOperation>();
        }

        public void AddCandyOperation(CandyOperation operation)
        {
            if (!this.CandyOperations.Any(x =>
                x.PreviousPosition.X == operation.PreviousPosition.X && x.PreviousPosition.Y == operation.PreviousPosition.Y
             && x.CurrentPosition.X == operation.CurrentPosition.X && x.CurrentPosition.Y == operation.CurrentPosition.Y))
            {
                this.CandyOperations.Add(operation);
            }
        }
    }
}
