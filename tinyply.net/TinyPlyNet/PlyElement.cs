using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyPlyNet.Helpers;

namespace TinyPlyNet
{
    /// <summary>
    /// ply elements
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
            parseInternal(stream);
        }

        /// <summary>
        /// create by element name(for writing)
        /// </summary>
        /// <param name="name"></param>
        public PlyElement(string name)
        {
            this.Properties = new List<PlyProperty>();
            this.Name = string.Empty;
            this.Size = 0;
        }

        /// <summary>
        /// element name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// number of element values
        /// </summary>
        public int Size { get; set; }

        public List<PlyProperty> Properties { get; set; }

        private void parseInternal(TextReader stream)
        {
            Name = stream.ReadWord();
            Size = int.Parse(stream.ReadWord());
        }
    }
}
