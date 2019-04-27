using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TinyPlyNet.Helpers;

namespace TinyPlyNet
{

    public class DataCursor
    {
        /// <summary>
        /// data source
        /// </summary>
        public IList vector;

        /// <summary>
        /// multi vector source
        /// </summary>
        public bool isMultivector = false;
    };

    public class PlyFile
    {
        /// <summary>
        /// create empty object(for writing)
        /// </summary>
        public PlyFile()
        {
            Elements = new List<PlyElement>();
            Comments = new List<string>();
            ObjInfo = new List<string>();
            IsBinary = false;
        }

        /// <summary>
        /// create for ply reading
        /// </summary>
        /// <param name="stream"></param>
        public PlyFile(Stream stream)
        {
            Elements = new List<PlyElement>();
            Comments = new List<string>();
            ObjInfo = new List<string>();
            IsBinary = false;
            var reader = new UnbufferedStreamReader(stream);
            this.ParseHeader(reader);
        }

        /// <summary>
        /// read ply from stream
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            this.ReadInternal(stream);
        }

        /// <summary>
        /// write ply to stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="isBinary"></param>
        public void Write(Stream stream, bool isBinary = false)
        {
            this.IsBinary = isBinary;
            this.WriteInternal(stream);
        }


        List<PlyElement> Elements { get; set; }

        List<string> Comments { get; set; }

        List<string> ObjInfo { get; set; }

        public bool IsBinary { get; set; }

        /// <summary>
        /// request read property from elements
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="elementKey">element key(ex. vertex)</param>
        /// <param name="propertyKeys">element properties(ex. "x","y","z")</param>
        /// <param name="source">read/write collection source</param>
        /// <returns></returns>
        public int RequestPropertyFromElement<T>(string elementKey, IEnumerable<string> propertyKeys, List<T> source)
        {
            if (!this.Elements.Any())
            {
                return 0;
            }

            if (this.Elements.FindIndex(x => x.Name == elementKey) >= 0)
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

            var cursor = new DataCursor();

            List<int> instanceCounts = new List<int>();

            var propertyKeyList = propertyKeys.ToList();

            int InstanceCounter(string propertyKey)
            {
                foreach (var e in this.Elements)
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
            }

            foreach (var key in propertyKeyList)
            {
                var instanceCount = InstanceCounter(key);
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
            var totalInstanceSize = instanceCounts.Sum(x => x);
            cursor.vector = source;
            return totalInstanceSize / propertyKeyList.Count;
        }

        /// <summary>
        /// request read list property from elements
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="elementKey">element key(ex. vertex)</param>
        /// <param name="propertyKeys">element properties(ex. "x","y","z")</param>
        /// <param name="source">read/write collection source</param>
        /// <returns></returns>
        public void RequestListPropertyFromElement<T>(string elementKey, string propertyKey, List<List<T>> source)
        {
            if (!this.Elements.Any())
            {
                return;
            }

            if (this.Elements.FindIndex(x => x.Name == elementKey) >= 0)
            {
                if (!_requestedElements.Contains(elementKey))
                {
                    _requestedElements.Add(elementKey);
                }
            }
            else
            {
                return;
            }

            var cursor = new DataCursor();

            var userKey = Helper.MakeKey(elementKey, propertyKey);
            if (this._userDataTable.ContainsKey(userKey))
            {
                throw new Exception("property has already been requested: " + propertyKey);
            }
            this._userDataTable[userKey] = cursor;
            cursor.vector = source;
            cursor.isMultivector = true;
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

        public void AddPropertiesToElement<T>(
            string elementKey,
            IEnumerable<string> propertyKeys,
            List<T> source)
        {
            if (this.Elements.FindIndex(x => x.Name == elementKey) >= 0)
            {
                throw new ArgumentException("already exist property key {elementKey}");
            }

            var propertyKeyList = propertyKeys.ToList();

            var cursor = new DataCursor();
            var plyElement = new PlyElement(elementKey);
            plyElement.Size = source.Count / propertyKeyList.Count;
            foreach (var key in propertyKeyList)
            {
                var plyProperty = new PlyProperty(typeof(T), key);
                plyElement.Properties.Add(plyProperty);
                var userKey = Helper.MakeKey(elementKey, key);
                if (_userDataTable.ContainsKey(userKey))
                {
                    throw new Exception("property has already been requested: " + key);
                }
                _userDataTable[userKey] = cursor;
            }
            this.Elements.Add(plyElement);
            cursor.vector = source;
        }

        public void AddListPropertyToElement<T>(
            string elementKey,
            string propertyKey,
            List<List<T>> data
        )
        {
            this.AddListPropertyToElement(elementKey, propertyKey, data, typeof(ushort));
        }

        public void AddListPropertyToElement<T>(
            string elementKey,
            string propertyKey,
            List<List<T>> data,
            Type listCountType
            )
        {
            if (this.Elements.FindIndex(x => x.Name == elementKey) >= 0)
            {
                throw new ArgumentException("already exist property key {elementKey}");
            }

            var cursor = new DataCursor();
            var plyElement = new PlyElement(elementKey);
            plyElement.Size = data.Count;
            var plyProperty = new PlyProperty(listCountType, typeof(T), propertyKey);
            plyElement.Properties.Add(plyProperty);
            var userKey = Helper.MakeKey(elementKey, propertyKey);
            if (_userDataTable.ContainsKey(userKey))
            {
                throw new Exception("property has already been requested: " + propertyKey);
            }
            this._userDataTable[userKey] = cursor;
            this.Elements.Add(plyElement);
            cursor.vector = data;
            cursor.isMultivector = true;
        }


        #region reader
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
                                        var listSizeString = sr.ReadData(property.ListType).ToString();
                                        if (uint.TryParse(listSizeString, out listSize))
                                        {
                                            IList sourceList = cursor.vector;
                                            if (cursor.isMultivector)
                                            {
                                                var listType = typeof(List<>);
                                                var listGenericType = listType.MakeGenericType(property.PropertyType);
                                                IList list = (IList)Activator.CreateInstance(listGenericType);
                                                sourceList.Add(list);
                                                sourceList = list;

                                            }
                                            for (var i = 0; i < listSize; ++i)
                                            {
                                                sourceList.Add(sr.ReadData(property.PropertyType));
                                            }
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
        #endregion reader

        #region writer

        private void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("ply");
            if (this.IsBinary)
            {
                throw new NotImplementedException("not supported binary format");
            }
            else
            {
                writer.WriteLine("format ascii 1.0");
            }

            // write each elements
            foreach (var element in this.Elements)
            {
                writer.WriteLine($"element {element.Name} {element.Size}");
                foreach (var prop in element.Properties)
                {
                    if (prop.IsList)
                    {
                        var listInfo = Helper.PropertyTable[prop.ListType];
                        var propertyInfo = Helper.PropertyTable[prop.PropertyType];
                        writer.WriteLine($"property list {listInfo.Str} {propertyInfo.Str} {prop.Name}");
                    }
                    else
                    {
                        var propertyInfo = Helper.PropertyTable[prop.PropertyType];
                        writer.WriteLine($"property {propertyInfo.Str} {prop.Name}");
                    }
                }
            }
            writer.WriteLine("end_header");
        }

        private void WriteInternal(Stream stream)
        {
            var streamWriter = new StreamWriter(stream);
            this.WriteHeader(streamWriter);
            if (this.IsBinary)
            {
                this.WriteBinaryInternal(stream);
            }
            else
            {
                this.WriteTextInternal(streamWriter);
            }
            streamWriter.Flush();
        }

        private void WriteBinaryInternal(Stream stream)
        {
            throw new NotImplementedException();
        }

        private void WriteTextInternal(TextWriter writer)
        {
            foreach (var element in this.Elements)
            {
                var current = 0;
                for (uint i = 0; i < element.Size; ++i)
                {
                    foreach (var prop in element.Properties)
                    {
                        var data = this._userDataTable[Helper.MakeKey(element, prop)];
                        if (prop.IsList)
                        {
                            if (!data.isMultivector)
                            {
                                throw new NotSupportedException("list parameter supported only multi list data.");
                            }
                            var listData = (IList) data.vector[current];
                            writer.WriteData(listData.Count);
                            for (int j = 0; j < listData.Count; ++j)
                            {
                                writer.WriteData(listData[j]);
                            }
                        }
                        else
                        {
                            writer.WriteData(data.vector[current]);
                        }
                        current++;
                    }
                    writer.WriteLine("");
                }
            }
        }

        struct PropertyLookup
        {
            public DataCursor DataCursor { get; set; }
            public bool Skip => this.DataCursor != null;

            public int ListStride { get; set; }
            public int PropertyStride { get; set; }
        }

        #endregion

        private List<string> _requestedElements = new List<string>();
        Dictionary<string, DataCursor> _userDataTable = new Dictionary<string, DataCursor>();
    }
}
