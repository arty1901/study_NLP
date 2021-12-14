/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Class handling NT date time values.
    class NTTaggedData : ITaggedData
    {
        public short TagID
        {
            get
            {
                return 10;
            }
        }
        public void SetData(byte[] data, int index, int count)
        {
        }
        public byte[] GetData()
        {
            return null;
        }
        public static bool IsValidValue(DateTime value)
        {
            return true;
        }
        public DateTime LastModificationTime
        {
            get
            {
                return _lastModificationTime;
            }
            set
            {
                if (!IsValidValue(value)) 
                    throw new ArgumentOutOfRangeException("value");
                _lastModificationTime = value;
            }
        }
        public DateTime CreateTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                if (!IsValidValue(value)) 
                    throw new ArgumentOutOfRangeException("value");
                _createTime = value;
            }
        }
        public DateTime LastAccessTime
        {
            get
            {
                return _lastAccessTime;
            }
            set
            {
                if (!IsValidValue(value)) 
                    throw new ArgumentOutOfRangeException("value");
                _lastAccessTime = value;
            }
        }
        DateTime _lastAccessTime = DateTime.MinValue;
        DateTime _lastModificationTime = DateTime.MinValue;
        DateTime _createTime = DateTime.MinValue;
    }
}