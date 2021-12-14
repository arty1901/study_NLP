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
    // An input buffer customised for use by <see cref="InflaterInputStream"/>
    class InflaterInputBuffer
    {
        public InflaterInputBuffer(Stream stream, int bufferSize = 4096)
        {
            inputStream = stream;
            if (bufferSize < 1024) 
                bufferSize = 1024;
            m_RawData = new byte[(int)bufferSize];
            m_ClearText = m_RawData;
        }
        public int RawLength
        {
            get
            {
                return m_RawLength;
            }
        }
        public byte[] RawData
        {
            get
            {
                return m_RawData;
            }
        }
        public int ClearTextLength
        {
            get
            {
                return m_ClearTextLength;
            }
        }
        public byte[] ClearText
        {
            get
            {
                return m_ClearText;
            }
        }
        public int Available
        {
            get
            {
                return m_Available;
            }
            set
            {
                m_Available = value;
            }
        }
        public void SetInflaterInput(Inflater inflater)
        {
            if (m_Available > 0) 
            {
                inflater.SetInputEx(m_ClearText, m_ClearTextLength - m_Available, m_Available);
                m_Available = 0;
            }
        }
        public void Fill()
        {
            m_RawLength = 0;
            int toRead = m_RawData.Length;
            while (toRead > 0) 
            {
                int count = inputStream.Read(m_RawData, m_RawLength, toRead);
                if (count <= 0) 
                    break;
                m_RawLength += count;
                toRead -= count;
            }
                {
                    m_ClearTextLength = m_RawLength;
                }
            m_Available = m_ClearTextLength;
        }
        public int ReadRawBuffer(byte[] buffer)
        {
            return this.ReadRawBufferEx(buffer, 0, buffer.Length);
        }
        public int ReadRawBufferEx(byte[] outBuffer, int offset, int length)
        {
            if (length < 0) 
                throw new ArgumentOutOfRangeException("length");
            int currentOffset = offset;
            int currentLength = length;
            while (currentLength > 0) 
            {
                if (m_Available <= 0) 
                {
                    this.Fill();
                    if (m_Available <= 0) 
                        return 0;
                }
                int toCopy = Math.Min(currentLength, m_Available);
                Array.Copy(m_RawData, m_RawLength - ((int)m_Available), outBuffer, currentOffset, toCopy);
                currentOffset += toCopy;
                currentLength -= toCopy;
                m_Available -= toCopy;
            }
            return length;
        }
        public int ReadClearTextBuffer(byte[] outBuffer, int offset, int length)
        {
            if (length < 0) 
                throw new ArgumentOutOfRangeException("length");
            int currentOffset = offset;
            int currentLength = length;
            while (currentLength > 0) 
            {
                if (m_Available <= 0) 
                {
                    this.Fill();
                    if (m_Available <= 0) 
                        return 0;
                }
                int toCopy = Math.Min(currentLength, m_Available);
                Array.Copy(m_ClearText, m_ClearTextLength - ((int)m_Available), outBuffer, currentOffset, toCopy);
                currentOffset += toCopy;
                currentLength -= toCopy;
                m_Available -= toCopy;
            }
            return length;
        }
        public int ReadLeByte()
        {
            if (m_Available <= 0) 
            {
                this.Fill();
                if (m_Available <= 0) 
                    throw new Exception("EOF in header");
            }
            byte result = m_RawData[m_RawLength - m_Available];
            m_Available -= 1;
            return result;
        }
        public int ReadLeShort()
        {
            return this.ReadLeByte() | (this.ReadLeByte() << 8);
        }
        public int ReadLeInt()
        {
            return this.ReadLeShort() | (this.ReadLeShort() << 16);
        }
        int m_RawLength;
        byte[] m_RawData;
        int m_ClearTextLength;
        byte[] m_ClearText;
        int m_Available;
        Stream inputStream;
    }
}