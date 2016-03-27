using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TinyPlyNet
{
    using Helpers;
    public class PlyProperty
    {

        public PlyProperty(TextReader stream)
        {
            parseInternal(stream);
        }

        public PlyProperty(Type type, string name)
        {
            IsList = false;
            ListType = null;
            PropertyType = type;
            Name = name;
            ListCount = 0;
        }

        public PlyProperty(Type list_type, Type prop_type, string name, int listCount)
        {
            IsList = true;
            ListType = list_type;
            PropertyType = prop_type;
            Name = name;
            ListCount = ListCount;

        }

        public Type ListType { get; set; }
        public Type PropertyType { get; set; }
        public bool IsList { get; set; }
        public int ListCount { get; set; }
        public string Name { get; set; }

        private void parseInternal(TextReader stream)
        {
            var t = stream.ReadWord();
            if (t == "list")
            {
                var countType = stream.ReadWord();
                t = stream.ReadWord();
                ListType = Helper.PropertyTypeFromString(countType);
                IsList = true;
            }
            PropertyType = Helper.PropertyTypeFromString(t);
            Name = stream.ReadWord();
        }

    }
}
