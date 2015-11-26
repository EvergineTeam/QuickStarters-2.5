using System.Collections.Generic;

namespace NewImpossibleGame.Models
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