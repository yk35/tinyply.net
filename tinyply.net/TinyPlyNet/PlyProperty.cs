using System;
using System.IO;
using TinyPlyNet.Helpers;

namespace TinyPlyNet
{
    /// <summary>
    /// representation of ply property
    /// </summary>
    public class PlyProperty
    {
        /// <summary>
        /// create from stream(for reading)
        /// </summary>
        /// <param name="stream">text source</param>
        public PlyProperty(TextReader stream)
        {
            IsList = false;
            ListType = null;
            Name = string.Empty;
            PropertyType = typeof(object);
            ParseInternal(stream);
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
        /// <param name="listType">number of property list type</param>
        /// <param name="propType">property type</param>
        /// <param name="name">property name</param>
        public PlyProperty(Type listType, Type propType, string name)
        {
            IsList = true;
            ListType = listType;
            PropertyType = propType;
            Name = name;
        }

        public Type? ListType { get; private set; }
        public Type PropertyType { get; private set; }
        public bool IsList { get; private set; }
        public string Name { get; private set; }

        private void ParseInternal(TextReader stream)
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
