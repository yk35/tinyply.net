using System;
using System.Runtime.InteropServices;

namespace TinyPlyNet.Helpers
{
    /// <summary>
    /// Helper methods for converting between byte arrays and objects
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        /// Converts a byte array to an object of the specified type
        /// </summary>
        /// <param name="rawValue">Byte array to convert</param>
        /// <param name="t">Target type for conversion</param>
        /// <returns>Object of the specified type</returns>
        public static object FromByteArray(byte[] rawValue, Type t)
        {
            GCHandle handle = GCHandle.Alloc(rawValue, GCHandleType.Pinned);
            object structure = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), t);
            handle.Free();
            return structure;
        }

        /// <summary>
        /// Converts a byte array to an object of the specified generic type
        /// </summary>
        /// <typeparam name="T">Target type for conversion</typeparam>
        /// <param name="rawValue">Byte array to convert</param>
        /// <returns>Object of type T</returns>
        public static T FromByteArray<T>(byte[] rawValue)
        {
            GCHandle handle = GCHandle.Alloc(rawValue, GCHandleType.Pinned);
            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }

        /// <summary>
        /// Converts an object to a byte array
        /// </summary>
        /// <param name="value">Object to convert to byte array</param>
        /// <returns>Byte array representation of the object</returns>
        public static byte[] ToByteArray(object value)
        {
            int rawsize = Marshal.SizeOf(value);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle =
                GCHandle.Alloc(rawdata,
                GCHandleType.Pinned);
            Marshal.StructureToPtr(value,
                handle.AddrOfPinnedObject(),
                false);
            handle.Free();
            return rawdata;
        }
    }
}
