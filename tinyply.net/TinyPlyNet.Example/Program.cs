using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyPlyNet;

namespace TinyPlyNet.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var stream = new FileStream(args[0], FileMode.Open, FileAccess.Read))
            {
                var f = new PlyFile(stream);
                var xyz = new List<float>();
                f.RequestPropertyFromElement("vertex", new[] { "x", "y", "z" }, xyz);
                var index = new List<List<int>>();
                f.RequestListPropertyFromElement("face", "vertex_indices", index);
                f.Read(stream);
                foreach (var e in xyz)
                {
                    Console.WriteLine("{0}", e);
                }
                foreach (var i in index)
                {
                    Console.WriteLine("{0}", i.Count);
                }

                using (var writeStream = new FileStream("writeTest.ply", FileMode.Create, FileAccess.Write))
                {
                    var writeFile = new PlyFile();
                    writeFile.AddPropertiesToElement("vertex", new[] { "x", "y", "z" }, xyz);
                    writeFile.AddListPropertyToElement("face", "vertex_indices", index);
                    f.Write(writeStream);
                }
            }
        }
    }
}
