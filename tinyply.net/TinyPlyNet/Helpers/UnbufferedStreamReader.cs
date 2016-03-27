// oritinal source code http://stackoverflow.com/questions/520722/unbuffered-streamreader

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TinyPlyNet.Helpers
{
    public class UnbufferedStreamReader : TextReader
    {
        Stream s;

        public UnbufferedStreamReader(Stream stream)
        {
            s = stream;
        }

        // This method assumes lines end with a line feed.
        // You may need to modify this method if your stream
        // follows the Windows convention of \r\n or some other 
        // convention that isn't just \n
        public override string ReadLine()
        {
            List<byte> bytes = new List<byte>();
            int current;
            while ((current = Read()) != -1 && current != (int)'\n')
            {
                byte b = (byte)current;
                bytes.Add(b);
            }
            return Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
        }

        // Read works differently than the `Read()` method of a 
        // TextReader. It reads the next BYTE rather than the next character
        public override int Read()
        {
            return s.ReadByte();
        }

        protected override void Dispose(bool disposing)
        {
            s.Dispose();
        }

        public override int Peek()
        {
            throw new NotImplementedException();
        }

        public override int Read(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override string ReadToEnd()
        {
            throw new NotImplementedException();
        }
    }
}
