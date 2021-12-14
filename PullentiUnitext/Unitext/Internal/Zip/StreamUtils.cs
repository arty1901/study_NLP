/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;

namespace Pullenti.Unitext.Internal.Zip
{
    // Provides simple <see cref="Stream"/>" utilities.
    class StreamUtils
    {
        public static void ReadFully(Stream stream, byte[] buffer)
        {
            ReadFullyEx(stream, buffer, 0, buffer.Length);
        }
        public static void ReadFullyEx(Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null) 
                throw new ArgumentNullException("stream");
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (((offset < 0)) || (offset > buffer.Length)) 
                throw new ArgumentOutOfRangeException("offset");
            if (((count < 0)) || ((offset + count) > buffer.Length)) 
                throw new ArgumentOutOfRangeException("count");
            while (count > 0) 
            {
                int readCount = stream.Read(buffer, offset, count);
                if (readCount <= 0) 
                    throw new EndOfStreamException();
                offset += readCount;
                count -= readCount;
            }
        }
        public static void Copy(Stream source, Stream destination, byte[] buffer)
        {
            if (source == null) 
                throw new ArgumentNullException("source");
            if (destination == null) 
                throw new ArgumentNullException("destination");
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (buffer.Length < 128) 
                throw new ArgumentException("Buffer is too small", "buffer");
            bool copying = true;
            while (copying) 
            {
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0) 
                    destination.Write(buffer, 0, bytesRead);
                else 
                {
                    destination.Flush();
                    copying = false;
                }
            }
        }
        private StreamUtils()
        {
        }
    }
}