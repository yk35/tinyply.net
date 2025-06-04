using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TinyPlyNet.Helpers
{
    /// <summary>
    /// stream helper
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// read 1 word
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>word string</returns>
        public static string ReadWord(this TextReader stream)
        {
            return stream.ReadWord(Encoding.UTF8);
        }

        /// <summary>
        /// read 1 word
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="encoding">char encoding</param>
        /// <returns>word string</returns>
        public static string ReadWord(this TextReader stream, Encoding encoding)
        {
            string word = string.Empty;

            // skip whitespace
            while (stream.Peek() > 0 && char.IsWhiteSpace((char)stream.Peek()))
            {
                stream.Read();
            }

            // read single character at a time building a word 
            // until reaching whitespace or (-1)
            while (stream.Read()
               .With(c => { // with each character . . .
                            // convert read bytes to char
                   var chr = encoding.GetChars(BitConverter.GetBytes(c)).First();

                   if (c == -1 || char.IsWhiteSpace(chr))
                       return -1; // signal end of word
                   else
                       word = word + chr; // append the char to our word

                   return c;
               }) > -1) ; // end while(stream.Read() if char returned is -1
            return word;
        }

        /// <summary>
        /// convertion function 
        /// </summary>
        /// <typeparam name="T">target object type</typeparam>
        /// <param name="obj">src object</param>
        /// <param name="f">dst object</param>
        /// <returns>converted object</returns>
        public static T With<T>(this T obj, Func<T, T> f)
        {
            return f(obj);
        }

        #region read
        #region binary read
        /// <summary>
        /// read data
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="t">read data type</param>
        /// <returns>readed object</returns>
        public static object ReadData(this BinaryReader stream, Type t)
        {
            var size = Marshal.SizeOf(t);
            byte[] buf = new byte[size];
            stream.Read(buf, 0, size);
            return ByteHelper.FromByteArray(buf, t);
        }

        /// <summary>
        /// read data big endian
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="t">read data type</param>
        /// <returns>readed object</returns>
        public static object ReadDataBigEndian(this BinaryReader stream, Type t)
        {
            var size = Marshal.SizeOf(t);
            byte[] buf = new byte[size];
            stream.Read(buf, 0, size);
            switch (size)
            {
                case 4:
                    {
                        byte temp1 = buf[0];
                        buf[0] = buf[3];
                        buf[3] = temp1;
                        byte temp2 = buf[1];
                        buf[1] = buf[2];
                        buf[2] = temp2;
                    }
                    break;
                case 8:
                    {
                        byte temp1 = buf[0];
                        buf[0] = buf[7];
                        buf[7] = temp1;

                        byte temp2 = buf[1];
                        buf[1] = buf[6];
                        buf[6] = temp2;

                        byte temp3 = buf[2];
                        buf[2] = buf[5];
                        buf[5] = temp3;

                        byte temp4 = buf[3];
                        buf[3] = buf[4];
                        buf[4] = temp4;
                    }
                    break;
                case 2:
                    {
                        byte temp = buf[0];
                        buf[0] = buf[1];
                        buf[1] = temp;
                    }
                    break;
            }

            return ByteHelper.FromByteArray(buf, t);
        }
        
        /// <summary>
        /// skipped data
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="t">read data type</param>
        public static void SkipData(this BinaryReader stream, Type t)
        {
            var size = Marshal.SizeOf(t);
            byte[] buf = new byte[size];
            stream.Read(buf, 0, size);
        }
        #endregion

        #region text read
        /// <summary>
        /// read data
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="t">target data type</param>
        /// <returns>readed object</returns>
        public static object ReadData(this TextReader stream, Type t)
        {
            var w = stream.ReadWord();
            if (w.ToLower() == "nan")
            {
                w = "NaN";
            }
            return Convert.ChangeType(w, t);
        }

        /// <summary>
        /// skipped data
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="t">read data type</param>
        public static void SkipData(this TextReader stream, Type t)
        {
            var w = stream.ReadWord();
        }
        #endregion
        #endregion read

        #region write
        #region write binary
        #endregion

        #region write text
        /// <summary>
        /// write one data with separation space
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        /// <param name="stream">stream</param>
        /// <param name="value">output value</param>
        public static void WriteData<T>(this TextWriter stream, T value)
        {
            stream.Write(value);
            stream.Write(" ");
        }

        #endregion
        #endregion

    }
}
