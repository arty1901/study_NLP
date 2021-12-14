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
    /// <summary>
    /// A <see cref="PartialInputStream"/> is an <see cref="InflaterInputStream"/> 
    /// whose data is only a part or subsection of a file.
    /// </summary>
    class PartialInputStream : Stream
    {
        /// <summary>
        /// Initialise a new instance of the <see cref="PartialInputStream"/> class.
        /// </summary>
        /// <param name="zipFile">The <see cref="ZipFile"/> containing the underlying stream to use for IO.</param>
        /// <param name="start">The start of the partial data.</param>
        /// <param name="length">The length of the partial data.</param>
        public PartialInputStream(ZipFile zipFile, int start, int length)
        {
            m_Start = start;
            m_Length = length;
            m_ZipFile = zipFile;
            m_BaseStream = m_ZipFile.m_BaseStream;
            m_ReadPos = start;
            m_End = start + length;
        }
        /// <summary>
        /// Read a byte from this stream.
        /// </summary>
        /// <return>Returns the byte read or -1 on end of stream.</return>
        public override int ReadByte()
        {
            if (m_ReadPos >= m_End) 
                return -1;
            m_BaseStream.Seek(m_ReadPos++, SeekOrigin.Begin);
            return m_BaseStream.ReadByte();
        }
        /// <summary>
        /// Close this <see cref="PartialInputStream">partial input stream</see>.
        /// </summary>
        public override void Close()
        {
        }
        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <return>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</return>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > (m_End - m_ReadPos)) 
            {
                count = (int)((m_End - m_ReadPos));
                if (count == 0) 
                    return 0;
            }
            m_BaseStream.Seek(m_ReadPos, SeekOrigin.Begin);
            int readCount = m_BaseStream.Read(buffer, offset, count);
            if (readCount > 0) 
                m_ReadPos += readCount;
            return readCount;
        }
        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new IOException("Not supported");
        }
        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            throw new IOException("Not supported");
        }
        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"></see> indicating the reference point used to obtain the new position.</param>
        /// <return>The new position within the current stream.</return>
        public override long Seek(long offset, SeekOrigin origin)
        {
            int newPos = m_ReadPos;
            switch (origin) { 
            case SeekOrigin.Begin:
                newPos = m_Start + ((int)offset);
                break;
            case SeekOrigin.Current:
                newPos = m_ReadPos + ((int)offset);
                break;
            case SeekOrigin.End:
                newPos = m_End + ((int)offset);
                break;
            }
            if (newPos < m_Start) 
                throw new IOException("Negative position is invalid");
            if (newPos >= m_End) 
                throw new IOException("Cannot seek past end");
            m_ReadPos = newPos;
            return m_ReadPos;
        }
        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
        }
        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <return>The current position within the stream.</return>
        public override long Position
        {
            get
            {
                return m_ReadPos - m_Start;
            }
            set
            {
                int newPos = m_Start + ((int)value);
                if (newPos < m_Start) 
                    throw new IOException("Negative position is invalid");
                if (newPos >= m_End) 
                    throw new IOException("Cannot seek past end");
                m_ReadPos = newPos;
            }
        }
        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <return>A long value representing the length of the stream in bytes.</return>
        public override long Length
        {
            get
            {
                return m_Length;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <return>true if the stream supports writing; otherwise, false.</return>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <return>true if the stream supports seeking; otherwise, false.</return>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <return>true if the stream supports reading; otherwise, false.</return>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        ZipFile m_ZipFile;
        Stream m_BaseStream;
        int m_Start;
        int m_Length;
        int m_ReadPos;
        int m_End;
    }
}