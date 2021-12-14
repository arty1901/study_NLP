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
    // This filter stream is used to decompress data compressed using the "deflate"
    // format. The "deflate" format is described in RFC 1951.
    // This stream may form the basis for other decompression filters, such
    // as the <see cref="ICSharpCode.SharpZipLib.GZip.GZipInputStream">GZipInputStream</see>.
    class InflaterInputStream : Stream
    {
        public InflaterInputStream(Stream baseInputStream, Inflater inflater = null, int bufferSize = 4096)
        {
            if (inflater == null) 
                inflater = new Inflater();
            if (baseInputStream == null) 
                throw new ArgumentNullException("baseInputStream");
            if (inflater == null) 
                throw new ArgumentNullException("inflater");
            if (bufferSize <= 0) 
                throw new ArgumentOutOfRangeException("bufferSize");
            this.baseInputStream = baseInputStream;
            this.inf = inflater;
            inputBuffer = new InflaterInputBuffer(baseInputStream, bufferSize);
        }
        public bool IsStreamOwner
        {
            get
            {
                return m_IsStreamOwner;
            }
            set
            {
                m_IsStreamOwner = value;
            }
        }
        public int Skip(int count)
        {
            if (count <= 0) 
                throw new ArgumentOutOfRangeException("count");
            if (baseInputStream.CanSeek) 
            {
                baseInputStream.Seek(count, SeekOrigin.Current);
                return count;
            }
            else 
            {
                int length = 2048;
                if (count < length) 
                    length = (int)count;
                byte[] tmp = new byte[(int)length];
                int readCount = 1;
                int toSkip = count;
                while ((toSkip > 0) && (readCount > 0)) 
                {
                    if (toSkip < length) 
                        length = (int)toSkip;
                    readCount = baseInputStream.Read(tmp, 0, length);
                    toSkip -= readCount;
                }
                return count - toSkip;
            }
        }
        public virtual int Available
        {
            get
            {
                return (inf.IsFinished ? 0 : 1);
            }
        }
        protected void Fill()
        {
            if (inputBuffer.Available <= 0) 
            {
                inputBuffer.Fill();
                if (inputBuffer.Available <= 0) 
                    throw new Exception("Unexpected EOF");
            }
            inputBuffer.SetInflaterInput(inf);
        }
        public override bool CanRead
        {
            get
            {
                return baseInputStream.CanRead;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
        public override long Length
        {
            get
            {
                return inputBuffer.RawLength;
            }
        }
        public override long Position
        {
            get
            {
                return baseInputStream.Position;
            }
            set
            {
            }
        }
        public override void Flush()
        {
            baseInputStream.Flush();
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }
        public override void SetLength(long value)
        {
            throw new IOException("InflaterInputStream SetLength not supported");
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new IOException("InflaterInputStream Write not supported");
        }
        public override void WriteByte(byte value)
        {
            throw new IOException("InflaterInputStream WriteByte not supported");
        }
        public override void Close()
        {
            if (!isClosed) 
            {
                isClosed = true;
                if (m_IsStreamOwner) 
                    baseInputStream.Dispose();
            }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (inf.IsNeedingDictionary) 
                throw new IOException("Need a dictionary");
            int remainingBytes = count;
            while (true) 
            {
                int bytesRead = 0;
                try 
                {
                    bytesRead = inf.InflateEx(buffer, offset, remainingBytes);
                }
                catch(Exception ex) 
                {
                    throw new IOException(ex.Message, ex);
                }
                offset += bytesRead;
                remainingBytes -= bytesRead;
                if (remainingBytes == 0 || inf.IsFinished) 
                    break;
                if (inf.IsNeedingInput) 
                {
                    try 
                    {
                        this.Fill();
                    }
                    catch(Exception ex) 
                    {
                        throw new IOException(ex.Message, ex);
                    }
                }
                else if (bytesRead == 0) 
                    throw new IOException("Dont know what to do");
            }
            return count - remainingBytes;
        }
        protected Inflater inf;
        protected InflaterInputBuffer inputBuffer;
        private Stream baseInputStream;
        protected int csize;
        bool isClosed;
        bool m_IsStreamOwner = true;
    }
}