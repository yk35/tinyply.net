using System;
using System.Collections.Generic;

namespace TinyPlyNet
{
    /// <summary>
    /// Information about PLY property types
    /// </summary>
    public class PropertyInfo
    {
        /// <summary>
        /// Size in bytes of the property type
        /// </summary>
        public required int Stride { get; init; }
        
        /// <summary>
        /// String representation of the property type for PLY format
        /// </summary>
        public required string Str { get; init; }
    }

    /// <summary>
    /// Helper methods for PLY file processing
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Mapping of .NET types to PLY property information (type name and byte size)
        /// </summary>
        public static Dictionary<Type, PropertyInfo> PropertyTable = new Dictionary<Type, PropertyInfo>()
        {
            {typeof(sbyte), new PropertyInfo() { Stride=1, Str = "char" } },
            {typeof(byte), new PropertyInfo() { Stride=1, Str = "uchar" } },
            {typeof(short), new PropertyInfo() { Stride=2, Str = "short" } },
            {typeof(ushort), new PropertyInfo() { Stride=2, Str = "ushort" }  },
            {typeof(int), new PropertyInfo() { Stride=4, Str = "int" } },
            {typeof(uint), new PropertyInfo() { Stride=4, Str = "uint" } },
            {typeof(float),  new PropertyInfo() { Stride=4, Str = "float" }  },
            {typeof(double),  new PropertyInfo() { Stride=8, Str = "double" } }
        };

        /// <summary>
        /// Converts PLY property type string to .NET Type
        /// </summary>
        /// <param name="t">PLY property type string (e.g., "int8", "float", "double")</param>
        /// <returns>.NET Type corresponding to the PLY property type</returns>
        /// <exception cref="NotSupportedException">Thrown when the property type is not supported</exception>
        // ReSharper disable once CognitiveComplexity
        public static Type PropertyTypeFromString(string t)
        {
            if (t == "int8" || t == "char") return typeof(sbyte);
            if (t == "uint8" || t == "uchar") return typeof(byte);
            if (t == "int16" || t == "short") return typeof(short);
            if (t == "uint16" || t == "ushort") return typeof(ushort);
            if (t == "int32" || t == "int") return typeof(int);
            if (t == "uint32" || t == "uint") return typeof(uint);
            if (t == "float32" || t == "float") return typeof(float);
            if (t == "float64" || t == "double") return typeof(double);
            throw new NotSupportedException("not supported type");
        }

        /// <summary>
        /// Creates a unique key by combining two strings
        /// </summary>
        /// <param name="a">First string</param>
        /// <param name="b">Second string</param>
        /// <returns>Combined key in format "a-b"</returns>
        public static string MakeKey(string a, string b)
        {
            return a + "-" + b;
        }

        /// <summary>
        /// make unique key
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="property">property</param>
        /// <returns>{elementName}-{propertyName}</returns>
        public static string MakeKey(PlyElement element, PlyProperty property)
        {
            return MakeKey(element.Name, property.Name);
        }

    }
}
