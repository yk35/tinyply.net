// original source code http://stackoverflow.com/questions/520722/unbuffered-streamreader

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TinyPlyNet.Helpers
{
    /// <summary>
    /// Unbuffered TextReader 
    /// </summary>
    public class UnbufferedStreamReader : TextReader
    {
        /// <summary>
        /// base stream,
        /// </summary>
        private Stream s;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="stream">base stream</param>
        public UnbufferedStreamReader(Stream stream)
        {
            this.s = stream;
        }

        /// <summary>
        /// This method assumes lines end with a line feed.
        /// You may need to modify this method if your stream
        /// follows the Windows convention of \r\n or some other 
        /// convention that isn't just \n
        /// </summary>
        /// <returns>string that one line</returns>
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


        /// <summary>
        /// Read works differently than the `Read()` method of a 
        /// TextReader. It reads the next BYTE rather than the next character
        /// </summary>
        /// <returns>-1: eof other: char code</returns>
        public override int Read()
        {
            return this.s.ReadByte();
        }

        /// <summary>
        /// dispose stream
        /// </summary>
        /// <param name="disposing">true: call from dispose() false: call from finalizer</param>
        protected override void Dispose(bool disposing)
        {
            this.s?.Dispose();

            this.s = null;
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
