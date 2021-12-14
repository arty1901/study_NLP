/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // A raw binary tagged value
    class RawTaggedData : ITaggedData
    {
        public RawTaggedData(short tag)
        {
            _tag = tag;
        }
        public short TagID
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }
        public void SetData(byte[] data, int offset, int count)
        {
            if (data == null) 
                throw new ArgumentNullException("data");
            m_Data = new byte[(int)count];
            Array.Copy(data, offset, m_Data, 0, count);
        }
        public byte[] GetData()
        {
            return m_Data;
        }
        public byte[] _Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                m_Data = value;
            }
        }
        short _tag;
        byte[] m_Data;
    }
}