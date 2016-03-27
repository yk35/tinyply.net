using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TinyPlyNet.Helpers
{
    public static class StreamHelper
    {
        public static string ReadWord(this TextReader stream)
        {
            return stream.ReadWord(Encoding.UTF8);
        }

        public static string ReadWord(this TextReader stream, Encoding encoding)
        {
            string word = "";

            // skip whitespace
            while (stream.Peek() > 0 && Char.IsWhiteSpace((char)stream.Peek()))
            {
                stream.Read();
            }

            // read single character at a time building a word 
            // until reaching whitespace or (-1)
            while (stream.Read()
               .With(c => { // with each character . . .
                            // convert read bytes to char
                   var chr = encoding.GetChars(BitConverter.GetBytes(c)).First();

                   if (c == -1 || Char.IsWhiteSpace(chr))
                       return -1; //signal end of word
                   else
                       word = word + chr; //append the char to our word

                   return c;
               }) > -1) ;  // end while(stream.Read() if char returned is -1
            return word;
        }

        public static T With<T>(this T obj, Func<T, T> f)
        {
            return f(obj);
        }

        public static object ReadData(this BinaryReader stream, Type t)
        {
            var size = Marshal.SizeOf(t);
            byte[] buf = new byte[size];
            stream.Read(buf, 0, size);
            return ByteHelper.FromByteArray(buf, t);
        }

        public static void ReadData(this BinaryReader stream, Type t, Array dst, ref int offset)
        {
            var size = Marshal.SizeOf(t);
            byte[] buf = new byte[size];
            stream.Read(buf, 0, size);
            Buffer.BlockCopy(buf, 0, dst, offset, size);
            offset += size;
        }

        public static void SkipData(this BinaryReader stream, Type t)
        {
            var size = Marshal.SizeOf(t);
            byte[] buf = new byte[size];
            stream.Read(buf, 0, size);
        }

        public static object ReadData(this TextReader stream, Type t)
        {
            var w = stream.ReadWord();
            var parse = t.GetMethod("Parse", new[] { typeof(string) });
            return parse.Invoke(null, new[] { w });
        }

        public static unsafe void ReadData(this TextReader stream, Type t, Array dst, ref int offset)
        {
            var w = stream.ReadWord();
            var size = Marshal.SizeOf(t);
            byte[] buf = new byte[size];
            var parse = t.GetMethod("Parse", new[] { typeof(string) });
            var v = parse.Invoke(null, new[] { w });
            var bytes = ByteHelper.ToByteArray(v);
            Buffer.BlockCopy(buf, 0, dst, offset, size);
            offset += size;
        }

        public static void SkipData(this TextReader stream, Type t)
        {
            var w = stream.ReadWord();
        }
    }
}
