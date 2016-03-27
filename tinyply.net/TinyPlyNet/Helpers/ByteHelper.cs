using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TinyPlyNet.Helpers
{
    public static class ByteHelper
    {
        public static object FromByteArray(byte[] rawValue, Type t)
        {
            GCHandle handle = GCHandle.Alloc(rawValue, GCHandleType.Pinned);
            object structure = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), t);
            handle.Free();
            return structure;
        }

        public static T FromByteArray<T>(byte[] rawValue)
        {
            GCHandle handle = GCHandle.Alloc(rawValue, GCHandleType.Pinned);
            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }

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
