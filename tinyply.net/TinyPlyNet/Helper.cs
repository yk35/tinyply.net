using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyPlyNet
{
    public class PropertyInfo
    {
        public int Stride { get; set; }
        public string Str { get; set; }
    }

    public static class Helper
    {
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
            return null;
        }

        public static string MakeKey(string a, string b)
        {
            return a + "-" + b;
        }


    }
}
