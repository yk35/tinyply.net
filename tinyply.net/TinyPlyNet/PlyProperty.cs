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
    public class PlyProperty
    {
        /// <summary>
        /// create from stream(for reading)
        /// </summary>
        /// <param name="stream">text source</param>
        public PlyProperty(TextReader stream)
        {
            parseInternal(stream);
        }

        /// <summary>
        /// create property info by data type and name(for writing)
        /// </summary>
        /// <param name="type">property type</param>
        /// <param name="name">property name</param>
        public PlyProperty(Type type, string name)
        {
            IsList = false;
            ListType = null;
            PropertyType = type;
            Name = name;
        }

        /// <summary>
        ///  create list property info by data type and name(for writing)
        /// </summary>
        /// <param name="list_type">property list type</param>
        /// <param name="prop_type">property type</param>
        /// <param name="name">propety name</param>
        /// <param name="listCount">number of list</param>
        public PlyProperty(Type list_type, Type prop_type, string name)
        {
            IsList = true;
            ListType = list_type;
            PropertyType = prop_type;
            Name = name;
        }

        public Type ListType { get; set; }
        public Type PropertyType { get; set; }
        public bool IsList { get; set; }
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
