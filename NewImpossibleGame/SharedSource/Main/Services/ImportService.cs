using NewImpossibleGame.Enums;
using NewImpossibleGame.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using WaveEngine.Framework.Services;

namespace NewImpossibleGame.Services
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
                        ModelFactoryService.Instance.Scale.X = blockScaleX;
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

                        if (!string.IsNullOrWhiteSpace(line)) // line must be filled with data
                        {
                            string[] splitted = line.Split(SEPARATOR); // split the line
                            if (splitted.Count() > 0)
                            {
                                Column column = new Column(); // creates the column
                                foreach (string value in splitted)
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
