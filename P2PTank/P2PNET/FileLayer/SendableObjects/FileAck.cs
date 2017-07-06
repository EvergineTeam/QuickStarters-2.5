using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.FileLayer.SendableObjects
{
    public class FileAck
    {
        //set true to let sender know to keep sending the
        //file parts
        public bool AcceptedFile { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public FileAck(FilePartObj filePart, bool acceptFutureParts = true)
        {
            this.AcceptedFile = acceptFutureParts;
            this.FileName = filePart.FileMetadata.FileName;
            this.FilePath = filePart.FileMetadata.FilePath;
        }
    }
}
