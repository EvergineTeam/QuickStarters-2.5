/***************************
 *  Level Format Descriptor:
 ***************************
 * 
 * Line 1: X block size
 * Line 2: Y block size
 * Line 3: Z block size
 * Line 4+: vertical columns
 * 
 * Colum format:
 * a[,b]+...
 * 
 * where a and b are integers with the number of the block from BlockTypeEnum enumeration. 
 * There are not limits in stacked blocks: first position is lowest block, second is the next stacked and so...
 * An empty block type will create a space, used to create "falling to dead" spaces or rise the ground level like a stairs:
 * 
 * SAMPLES:
 * 
 * * draw a ground block: 1
 * * draw two ground blocks, one on lower level and other four level upper:  1, 0, 0, 0, 0, 1
 * * draw a non-ground columm with a box obstacle and a spike over it: 0, 2, 3
 *  
 * "falling to dead"
 * 0,0,0,0,0,1
 * 0,0,0,0,0,1
 * 0
 * 0
 * 0
 * 0
 * 1
 * 
 * "filled stairs"
 * 1
 * 1,1
 * 1,1,1
 * 1,1,1,1
 * 1,1,1,1,1
 * 1,1,1,1,1
 * 1,1,1,1,1
 * 
 * "stairs", same as "filled stairs" but not filled....
 * 1
 * 0,1
 * 0,0,1
 * 0,0,0,1
 * 0,0,0,0,1
 * 0,0,0,0,1
 * 0,0,0,0,1
 */
using NewImpossibleGameProject.Enums;
using NewImpossibleGameProject.GameModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework.Services;

namespace NewImpossibleGameProject.GameServices
{
    /// <summary>
    /// Import Service, load files and convert to game format
    /// </summary>
    public class ImportService
    {
        // Singleton instance object
        private static ImportService instance;

        /// <summary>
        /// The values separator in file
        /// </summary>
        private char SEPARATOR = ',';

        /// <summary>
        /// Gets the instance of the Singleton.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ImportService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImportService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ImportService"/> class from being created.
        /// </summary>
        private ImportService()
        {
        }

        /// <summary>
        /// Imports a level file. You can read the Level file format description on this file header
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Exception Loading level file:  + path</exception>
        public LevelModel ImportLevel(string path)
        {
            string line = string.Empty;
            BlockTypeEnum blockType = BlockTypeEnum.EMPTY;

            LevelModel model = new LevelModel();

            try
            {
                // Read the level file
                using (Stream fs = WaveServices.Storage.OpenContentFile(path))
                //using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read))
                using (BufferedStream bs = new BufferedStream(fs, 8388608)) // 8Mb buffer it's a good buffer for files.
                using (StreamReader sr = new StreamReader(bs))
                {
                    float blockScaleX = 1.0f;
                    float blockScaleY = 1.0f;
                    float blockScaleZ = 1.0f;

                    // FIRST LINE of the file: Block X scale
                    if (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        float.TryParse(line, NumberStyles.Number, CultureInfo.InvariantCulture, out blockScaleX);
                        ModelFactoryService.Instance.Scale.X= blockScaleX;
                    }

                    // SECOND LINE of the file: Block Y Scale
                    if (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        float.TryParse(line, NumberStyles.Number, CultureInfo.InvariantCulture, out blockScaleY);
                        ModelFactoryService.Instance.Scale.Y = blockScaleY;
                    }

                    // THIRD LINE of the file: Block Z Scale
                    if (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        float.TryParse(line, NumberStyles.Number, CultureInfo.InvariantCulture, out blockScaleZ);
                        ModelFactoryService.Instance.Scale.Z = blockScaleZ;
                    }

                    // The rest of the file is the level
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        if(!string.IsNullOrWhiteSpace(line)) // line must be filled with data
                        {
                            string[] splitted = line.Split(SEPARATOR); // split the line
                            if(splitted.Count() > 0)
                            {
                                Column column = new Column(); // creates the column
                                foreach(string value in splitted)
                                {
                                    blockType = BlockTypeEnum.EMPTY; // default value if reading error or out of block type error
                                    Enum.TryParse<BlockTypeEnum>(value, out blockType);
                                    column.Add(blockType);
                                }
                                model.ColumnCollection.Enqueue(column);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception Loading level file: " + path, ex);
            }

            return model;
        }
    }
}
