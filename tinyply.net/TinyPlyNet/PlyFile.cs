using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyPlyNet.Helpers;

namespace TinyPlyNet
{

    public class DataCursor
    {
        public IList vector;
        public uint offset = 0;
    };

    public class PlyFile
    {
        public PlyFile(Stream stream)
        {
            Elements = new List<PlyElement>();
            Comments = new List<string>();
            ObjInfo = new List<string>();
            IsBinary = false;
            var reader = new UnbufferedStreamReader(stream);
            ParseHeader(reader);
        }

        public void Read(Stream stream)
        {
            ReadInternal(stream);
        }

        public void Write(Stream stream, bool isBinary)
        {

        }


        List<PlyElement> Elements { get; set; }

        List<string> Comments { get; set; }

        List<string> ObjInfo { get; set; }

        public bool IsBinary { get; set; }

        public int RequestPropertyFromElement<T>(string elementKey, IEnumerable<string> propertyKeys, List<T> source)
        {
            if (!Elements.Any())
            {
                return 0;
            }

            if (Elements.FindIndex(x => x.Name == elementKey) >= 0)
            {
                if (!_requestedElements.Contains(elementKey))
                {
                    _requestedElements.Add(elementKey);
                }
            }
            else
            {
                return 0;
            }

            Func<string, string, uint> instanceCounter = (elemKey, propertyKey) =>
            {
                foreach (var e in Elements)
                {
                    if (e.Name != elementKey)
                    {
                        continue;
                    }

                    foreach (var p in e.Properties)
                    {
                        if (p.Name == propertyKey)
                        {
                            if (typeof(T) != p.PropertyType)
                            {
                                throw new Exception("destination vector is wrongly typed to hold this property");
                            }
                            return e.Size;
                        }
                    }
                }
                return 0;
            };

            var cursor = new DataCursor();

            List<uint> instanceCounts = new List<uint>();

            foreach (var key in propertyKeys)
            {
                var instanceCount = instanceCounter(elementKey, key);
                if (instanceCount != 0)
                {
                    instanceCounts.Add(instanceCount);
                    var userKey = Helper.MakeKey(elementKey, key);
                    if (_userDataTable.ContainsKey(userKey))
                    {
                        throw new Exception("property has already been requested: " + key);
                    }
                    _userDataTable[userKey] = cursor;
                }
                else
                {
                    return 0;
                }
            }
            var totalInstanceSize = (int)instanceCounts.Sum(x => x);
            cursor.vector = source;
            return totalInstanceSize / propertyKeys.Count();
        }

        private void ParseHeader(TextReader stream)
        {
            bool gotMagic = false;
            for (;;)
            {
                var line = stream.ReadLine();
                using (var ls = new StringReader(line))
                {
                    var token = ls.ReadWord();
                    if (token == "ply" || token == "PLY" || string.IsNullOrEmpty(token))
                    {
                        gotMagic = true;
                        continue;
                    }
                    else if (token == "comment")
                    {
                        ReadHeaderText(ls, Comments);
                    }
                    else if (token == "format")
                    {
                        ReadHeaderFormat(ls);
                    }
                    else if (token == "element")
                    {
                        ReadHeaderElement(ls);
                    }
                    else if (token == "property")
                    {
                        ReadHeaderProperty(ls);
                    }
                    else if (token == "obj_info")
                    {
                        ReadHeaderText(ls, ObjInfo);
                    }
                    else if (token == "end_header")
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("invalid header");
                    }

                }
            }
        }

        private void ReadHeaderText(TextReader line, List<string> comments)
        {
            var l = line.ReadLine();
            comments.Add(l);
        }

        private void ReadHeaderFormat(TextReader line)
        {
            var s = line.ReadWord();
            if (s == "binary_little_endian")
                IsBinary = true;
            else if (s == "binary_big_endian")
                throw new NotSupportedException("big endian formats are not supported!");
        }

        private void ReadHeaderProperty(TextReader stream)
        {
            Elements.Last().Properties.Add(new PlyProperty(stream));
        }

        private void ReadHeaderElement(TextReader stream)
        {
            Elements.Add(new PlyElement(stream));
        }

        private void ReadInternal(Stream stream)
        {
            if (IsBinary)
            {
                ReadBinaryInternal(stream);
            }
            else
            {
                ReadTextInternal(stream);
            }
        }

        private void ReadBinaryInternal(Stream stream)
        {
            var bs = new BinaryReader(stream);
            {
                foreach (var element in Elements)
                {
                    var idx = _requestedElements.FindIndex(x => x == element.Name);
                    if (idx != -1)
                    {
                        for (long count = 0; count < element.Size; ++count)
                        {
                            foreach (var property in element.Properties)
                            {
                                DataCursor cursor;
                                if (_userDataTable.TryGetValue(Helper.MakeKey(element.Name, property.Name), out cursor))
                                {
                                    if (property.IsList)
                                    {
                                        uint listSize = 0;
                                        listSize = (uint)bs.ReadData(property.ListType);
                                        for (var i = 0; i < listSize; ++i)
                                        {
                                            cursor.vector.Add(bs.ReadData(property.PropertyType));
                                        }
                                    }
                                    else
                                    {
                                        cursor.vector.Add(bs.ReadData(property.PropertyType));
                                    }
                                }
                                else
                                {
                                    bs.SkipData(property.PropertyType);
                                }
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }


        private void ReadTextInternal(Stream stream)
        {
            var sr = new StreamReader(stream);
            {
                foreach (var element in Elements)
                {
                    var idx = _requestedElements.FindIndex(x => x == element.Name);
                    if (idx != -1)
                    {
                        for (long count = 0; count < element.Size; ++count)
                        {
                            foreach (var property in element.Properties)
                            {
                                DataCursor cursor;
                                if (_userDataTable.TryGetValue(Helper.MakeKey(element.Name, property.Name), out cursor))
                                {
                                    if (property.IsList)
                                    {
                                        uint listSize = 0;
                                        listSize = (uint)sr.ReadData(property.ListType);
                                        for (var i = 0; i < listSize; ++i)
                                        {
                                            cursor.vector.Add(sr.ReadData(property.PropertyType));
                                        }
                                    }
                                    else
                                    {
                                        cursor.vector.Add(sr.ReadData(property.PropertyType));
                                    }
                                }
                                else
                                {
                                    sr.SkipData(property.PropertyType);
                                }
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
            }
            sr.DiscardBufferedData();
        }

        private List<string> _requestedElements = new List<string>();
        Dictionary<string, DataCursor> _userDataTable = new Dictionary<string, DataCursor>();
    }
}
