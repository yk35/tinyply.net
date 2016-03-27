using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyPlyNet.Helpers;

namespace TinyPlyNet
{
    public class PlyElement
    {
        public PlyElement(TextReader stream)
        {
            Properties = new List<PlyProperty>();
            parseInternal(stream);
        }

        public PlyElement(string name, int count)
        {
            Properties = new List<PlyProperty>();
            Name = string.Empty;
            Size = 0;

        }

        public string Name { get; set; }

        public uint Size { get; set; }

        public List<PlyProperty> Properties { get; set; }

        private void parseInternal(TextReader stream)
        {
            Name = stream.ReadWord();
            Size = uint.Parse(stream.ReadWord());
        }
    }
}
