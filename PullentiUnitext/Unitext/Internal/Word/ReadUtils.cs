/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;

namespace Pullenti.Unitext.Internal.Word
{
    static class ReadUtils
    {
        internal const int ByteSize = 1;
        internal const int WordSize = 2;
        internal const int DWordSize = 4;
        internal static byte ReadByte(Stream s)
        {
            int b = s.ReadByte();
            if (b < 0) 
                throw new Exception("Unexpected EOF");
            return (byte)b;
        }
        internal static byte ReadByteRef(Stream s, ref int read)
        {
            byte b = ReadByte(s);
            ++read;
            return b;
        }
        static byte[] buf = new byte[(int)32];
        internal static int ReadInt(Stream s)
        {
            int res = 0;
            int i = s.Read(buf, 0, 4);
            if (i < 0) 
                return -1;
            if (i < 4) 
                return 0;
            for (i = 3; i >= 0; i--) 
            {
                res = (res << 8) | buf[i];
            }
            return res;
        }
        internal static short ReadShort(Stream s)
        {
            short res = (short)0;
            int i = s.Read(buf, 0, 2);
            if (i < 2) 
                return 0;
            for (i = 1; i >= 0; i--) 
            {
                res = (short)(((res << 8) | buf[i]));
            }
            return res;
        }
        internal static byte[] ReadExact(Stream s, int count)
        {
            byte[] data = new byte[(int)count];
            int read = s.Read(data, 0, count);
            if (read != count) 
                throw new Exception("Unexpected EOF");
            return data;
        }
        internal static byte[] ReadExactRef(Stream s, int count, ref int read)
        {
            byte[] data = ReadExact(s, count);
            read += data.Length;
            return data;
        }
        internal static void Skip(Stream s, int count)
        {
            s.Seek(count, SeekOrigin.Current);
        }
    }
}