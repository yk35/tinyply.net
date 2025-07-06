using System.Collections.Generic;
using System.IO;
using TinyPlyNet.Helpers;

namespace TinyPlyNet
{
    /// <summary>
    /// representation of ply elements
    /// </summary>
    public class PlyElement
    {
        /// <summary>
        /// create from stream(for reading)
        /// </summary>
        /// <param name="stream"></param>
        public PlyElement(TextReader stream)
        {
            Properties = new List<PlyProperty>();
            ParseInternal(stream);
        }

        /// <summary>
        /// create by element name(for writing)
        /// </summary>
        /// <param name="name"></param>
        public PlyElement(string name)
        {
            this.Properties = new List<PlyProperty>();
            this.Name = name;
            this.Size = 0;
        }

        /// <summary>
        /// element name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// number of element values
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// properties
        /// </summary>
        public List<PlyProperty> Properties { get; private set; }

        private void ParseInternal(TextReader stream)
        {
            Name = stream.ReadWord();
            Size = int.Parse(stream.ReadWord());
        }
    }
}
