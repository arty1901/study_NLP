/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // This class is general purpose class for writing data to a buffer.
    // It allows you to write bits as well as bytes
    // Based on DeflaterPending.java
    class PendingBuffer
    {
        byte[] buffer_;
        int start;
        int end;
        uint bits;
        int m_BitCount;
        public PendingBuffer(int bufferSize = 4096)
        {
            buffer_ = new byte[(int)bufferSize];
        }
        public void Reset()
        {
            start = (end = (m_BitCount = 0));
        }
        public void WriteByte(int value)
        {
            buffer_[end++] = unchecked((byte)value);
        }
        public void WriteShort(int value)
        {
            buffer_[end++] = unchecked((byte)value);
            buffer_[end++] = unchecked((byte)((value >> 8)));
        }
        public void WriteInt(int value)
        {
            buffer_[end++] = unchecked((byte)value);
            buffer_[end++] = unchecked((byte)((value >> 8)));
            buffer_[end++] = unchecked((byte)((value >> 16)));
            buffer_[end++] = unchecked((byte)((value >> 24)));
        }
        public void WriteBlock(byte[] block, int offset, int length)
        {
            Array.Copy(block, offset, buffer_, end, length);
            end += length;
        }
        public int BitCount
        {
            get
            {
                return m_BitCount;
            }
        }
        public void AlignToByte()
        {
            if (m_BitCount > 0) 
            {
                buffer_[end++] = unchecked((byte)bits);
                if (m_BitCount > 8) 
                    buffer_[end++] = unchecked((byte)((bits >> 8)));
            }
            bits = 0;
            m_BitCount = 0;
        }
        public void WriteBits(int b, int count)
        {
            bits |= ((uint)(b << m_BitCount));
            m_BitCount += count;
            if (m_BitCount >= 16) 
            {
                buffer_[end++] = unchecked((byte)bits);
                buffer_[end++] = unchecked((byte)((bits >> 8)));
                bits >>= 16;
                m_BitCount -= 16;
            }
        }
        public void WriteShortMSB(int s)
        {
            buffer_[end++] = unchecked((byte)((s >> 8)));
            buffer_[end++] = unchecked((byte)s);
        }
        public bool IsFlushed
        {
            get
            {
                return end == 0;
            }
        }
        public int Flush(byte[] output, int offset, int length)
        {
            if (m_BitCount >= 8) 
            {
                buffer_[end++] = unchecked((byte)bits);
                bits >>= 8;
                m_BitCount -= 8;
            }
            if (length > (end - start)) 
            {
                length = end - start;
                Array.Copy(buffer_, start, output, offset, length);
                start = 0;
                end = 0;
            }
            else 
            {
                Array.Copy(buffer_, start, output, offset, length);
                start += length;
            }
            return length;
        }
        public byte[] ToByteArray()
        {
            byte[] result = new byte[(int)(end - start)];
            Array.Copy(buffer_, start, result, 0, result.Length);
            start = 0;
            end = 0;
            return result;
        }
    }
}