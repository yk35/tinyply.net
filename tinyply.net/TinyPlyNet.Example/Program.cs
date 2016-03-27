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
                f.Read(stream);
                foreach (var e in xyz)
                {
                    Console.WriteLine("{0}", e);
                }
            }
        }
    }
}
