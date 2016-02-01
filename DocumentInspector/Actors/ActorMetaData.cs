using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentInspector.Actors
{
    /// <summary>
    /// Meta-data class
    /// </summary>
    public class ActorMetaData
    {
        public ActorMetaData(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; private set; }
        public string Path { get; private set; }
    }
}
