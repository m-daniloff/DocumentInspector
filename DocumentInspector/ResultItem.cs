using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentInspector
{
    public class ResultItem
    {
        public ResultItem()
        { }
        public int FileNumber { get; set; }
        public string DirectoryPath { get; set; }
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string DocumentAuthor { get; set; }

        public long ElapsedMs { get; set; }
    }
}
