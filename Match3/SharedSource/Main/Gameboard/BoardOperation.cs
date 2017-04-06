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
            if (!this.CandyOperations.Any(x => x.PreviousPosition == operation.PreviousPosition &&
                                               x.CurrentPosition == operation.CurrentPosition))
            {
                this.CandyOperations.Add(operation);
            }
        }
    }
}
