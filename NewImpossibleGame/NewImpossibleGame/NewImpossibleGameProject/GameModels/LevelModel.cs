using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewImpossibleGameProject.GameModels
{
    /// <summary>
    /// Single Level Model
    /// </summary>
    public class LevelModel
    {
        /// <summary>
        /// The column collection of the level
        /// </summary>
        public Queue<Column> ColumnCollection = new Queue<Column>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelModel"/> class.
        /// </summary>
        public LevelModel()
        {
        }

        /// <summary>
        /// Gets the next column.
        /// </summary>
        /// <returns></returns>
        public Column GetNextColumn()
        {
            var column = this.ColumnCollection.Dequeue();
            this.ColumnCollection.Enqueue(column);
            return column;
        }
    }
}
