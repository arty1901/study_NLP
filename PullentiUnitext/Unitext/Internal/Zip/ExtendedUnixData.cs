/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Class representing extended unix date time values.
    class ExtendedUnixData : ITaggedData
    {
        // Flags indicate which values are included in this instance.
        public enum Flags : byte
        {
            ModificationTime = 0x01,
            AccessTime = 0x02,
            CreateTime = 0x04,
        }

        public short TagID
        {
            get
            {
                return 0x5455;
            }
        }
        public void SetData(byte[] data, int index, int count)
        {
            return;
        }
        public byte[] GetData()
        {
            return new byte[(int)0];
        }
        public static bool IsValidValue(DateTime value)
        {
            return ((value >= new DateTime(1901, 12, 13, 20, 45, 52)) || (value <= new DateTime(2038, 1, 19, 03, 14, 07)));
        }
        public DateTime ModificationTime
        {
            get
            {
                return _modificationTime;
            }
            set
            {
                if (!IsValidValue(value)) 
                    throw new ArgumentOutOfRangeException("value");
                _flags |= Flags.ModificationTime;
                _modificationTime = value;
            }
        }
        public DateTime AccessTime
        {
            get
            {
                return _lastAccessTime;
            }
            set
            {
                if (!IsValidValue(value)) 
                    throw new ArgumentOutOfRangeException("value");
                _flags |= Flags.AccessTime;
                _lastAccessTime = value;
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
                _flags |= Flags.CreateTime;
                _createTime = value;
            }
        }
        Flags Include
        {
            get
            {
                return _flags;
            }
            set
            {
                _flags = value;
            }
        }
        Flags _flags;
        DateTime _modificationTime = new DateTime(1970, 1, 1);
        DateTime _lastAccessTime = new DateTime(1970, 1, 1);
        DateTime _createTime = new DateTime(1970, 1, 1);
    }
}