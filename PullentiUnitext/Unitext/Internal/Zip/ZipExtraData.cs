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
    // A class to handle the extra data field for Zip entries
    class ZipExtraData
    {
        public ZipExtraData(byte[] data = null)
        {
            if (data == null) 
                m_Data = new byte[(int)0];
            else 
                m_Data = data;
        }
        public byte[] GetEntryData()
        {
            if (((ulong)Length) > ushort.MaxValue) 
                throw new Exception("Data exceeds maximum length");
            if (m_Data == null) 
                return null;
            byte[] res = new byte[(int)m_Data.Length];
            Array.Copy(m_Data, res, m_Data.Length);
            return res;
        }
        public void Clear()
        {
            if ((m_Data == null) || (m_Data.Length != 0)) 
                m_Data = new byte[(int)0];
        }
        public int Length
        {
            get
            {
                return m_Data.Length;
            }
        }
        public Stream GetStreamForTag(int tag)
        {
            Stream result = null;
            if (this.Find(tag)) 
                result = new MemoryStream(m_Data, _index, _readValueLength, false);
            return result;
        }
        private ITaggedData GetData(short tag)
        {
            ITaggedData result = null;
            if (this.Find(tag)) 
                result = Create(tag, m_Data, _readValueStart, _readValueLength);
            return result;
        }
        static ITaggedData Create(short tag, byte[] data, int offset, int count)
        {
            ITaggedData result = null;
            switch (tag) { 
            case 0x000A:
                result = new NTTaggedData();
                break;
            case 0x5455:
                result = new ExtendedUnixData();
                break;
            default: 
                result = new RawTaggedData(tag);
                break;
            }
            result.SetData(data, offset, count);
            return result;
        }
        public int ValueLength
        {
            get
            {
                return _readValueLength;
            }
        }
        public int CurrentReadIndex
        {
            get
            {
                return _index;
            }
        }
        public int UnreadCount
        {
            get
            {
                if ((_readValueStart > m_Data.Length) || ((_readValueStart < 4))) 
                    throw new Exception("Find must be called before calling a Read method");
                return (_readValueStart + _readValueLength) - _index;
            }
        }
        public bool Find(int headerID)
        {
            _readValueStart = m_Data.Length;
            _readValueLength = 0;
            _index = 0;
            int localLength = _readValueStart;
            int localTag = headerID - 1;
            while ((localTag != headerID) && ((_index < (m_Data.Length - 3)))) 
            {
                localTag = this.ReadShortInternal();
                localLength = this.ReadShortInternal();
                if (localTag != headerID) 
                    _index += localLength;
            }
            bool result = (localTag == headerID) && (((_index + localLength)) <= m_Data.Length);
            if (result) 
            {
                _readValueStart = _index;
                _readValueLength = localLength;
            }
            return result;
        }
        public void AddEntry(ITaggedData taggedData)
        {
            if (taggedData == null) 
                throw new ArgumentNullException("taggedData");
            this.AddEntry2(taggedData.TagID, taggedData.GetData());
        }
        public void AddEntry2(int headerID, byte[] fieldData)
        {
            if ((((ulong)headerID) > ushort.MaxValue) || ((headerID < 0))) 
                throw new ArgumentOutOfRangeException("headerID");
            int addLength = (fieldData == null ? 0 : fieldData.Length);
            if (((ulong)addLength) > ushort.MaxValue) 
                throw new ArgumentOutOfRangeException("fieldData", "exceeds maximum length");
            int newLength = m_Data.Length + addLength + 4;
            if (this.Find(headerID)) 
                newLength -= ((ValueLength + 4));
            if (((ulong)newLength) > ushort.MaxValue) 
                throw new Exception("Data exceeds maximum length");
            this.Delete(headerID);
            byte[] newData = new byte[(int)newLength];
            Array.Copy(m_Data, 0, newData, 0, m_Data.Length);
            int index = m_Data.Length;
            m_Data = newData;
            this.SetShort(ref index, headerID);
            this.SetShort(ref index, addLength);
            if (fieldData != null) 
                Array.Copy(fieldData, 0, newData, index, fieldData.Length);
        }
        public void StartNewEntry()
        {
            _newEntry = new MemoryStream();
        }
        public void AddNewEntry(int headerID)
        {
            byte[] newData = _newEntry.ToArray();
            _newEntry = null;
            this.AddEntry2(headerID, newData);
        }
        public void AddData0(byte data)
        {
            _newEntry.WriteByte(data);
        }
        public void AddData(byte[] data)
        {
            if (data == null) 
                throw new ArgumentNullException("data");
            _newEntry.Write(data, 0, data.Length);
        }
        public void AddLeShort(int toAdd)
        {
            unchecked 
            {
                _newEntry.WriteByte((byte)toAdd);
                _newEntry.WriteByte((byte)((toAdd >> 8)));
            }
        }
        public void AddLeInt(int toAdd)
        {
            unchecked 
            {
                this.AddLeShort((short)toAdd);
                this.AddLeShort((short)((toAdd >> 16)));
            }
        }
        public void AddLeLong(int toAdd1, int toAdd2)
        {
            this.AddLeInt(toAdd1);
            this.AddLeInt(toAdd2);
        }
        public bool Delete(int headerID)
        {
            bool result = false;
            if (this.Find(headerID)) 
            {
                result = true;
                int trueStart = _readValueStart - 4;
                byte[] newData = new byte[(int)(m_Data.Length - ((ValueLength + 4)))];
                Array.Copy(m_Data, 0, newData, 0, trueStart);
                int trueEnd = trueStart + ValueLength + 4;
                Array.Copy(m_Data, trueEnd, newData, trueStart, m_Data.Length - trueEnd);
                m_Data = newData;
            }
            return result;
        }
        public int ReadLong()
        {
            this.ReadCheck(8);
            int res = this.ReadInt();
            this.ReadInt();
            return res;
        }
        public int ReadInt()
        {
            this.ReadCheck(4);
            int result = (m_Data[_index] + (m_Data[_index + 1] << 8) + (m_Data[_index + 2] << 16)) + (m_Data[_index + 3] << 24);
            _index += 4;
            return result;
        }
        public int ReadShort()
        {
            this.ReadCheck(2);
            int result = m_Data[_index] + (m_Data[_index + 1] << 8);
            _index += 2;
            return result;
        }
        public int ReadByte()
        {
            int result = -1;
            if (((_index < m_Data.Length)) && ((_readValueStart + _readValueLength) > _index)) 
            {
                result = m_Data[_index];
                _index += 1;
            }
            return result;
        }
        public void Skip(int amount)
        {
            this.ReadCheck(amount);
            _index += amount;
        }
        void ReadCheck(int length)
        {
            if ((_readValueStart > m_Data.Length) || ((_readValueStart < 4))) 
                throw new Exception("Find must be called before calling a Read method");
            if (_index > ((_readValueStart + _readValueLength) - length)) 
                throw new Exception("End of extra data");
            if ((_index + length) < 4) 
                throw new Exception("Cannot read before start of tag");
        }
        int ReadShortInternal()
        {
            if (_index > (m_Data.Length - 2)) 
                throw new Exception("End of extra data");
            int result = m_Data[_index] + (m_Data[_index + 1] << 8);
            _index += 2;
            return result;
        }
        void SetShort(ref int index, int source)
        {
            m_Data[index] = (byte)source;
            m_Data[index + 1] = (byte)((source >> 8));
            index += 2;
        }
        public void Dispose()
        {
            try 
            {
                if (_newEntry != null) 
                    _newEntry.Dispose();
            }
            catch(Exception ex) 
            {
            }
        }
        int _index;
        int _readValueStart;
        int _readValueLength;
        MemoryStream _newEntry;
        byte[] m_Data;
    }
}