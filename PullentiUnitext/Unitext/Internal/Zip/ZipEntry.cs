/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // This class represents an entry in a zip archive.  This can be a file
    // or a directory
    // ZipFile and ZipInputStream will give you instances of this class as
    // information about the members in an archive.  ZipOutputStream
    // uses an instance of this class when creating an entry in a Zip file.
    class ZipEntry
    {
        enum Known : byte
        {
            None = 0,
            Size = 0x01,
            CompressedSize = 0x02,
            Crc = 0x04,
            Time = 0x08,
            ExternalAttributes = 0x10,
        }

        internal ZipEntry(string name, int versionRequiredToExtract = 0, int madeByInfo = ZipConstants.VersionMadeBy, CompressionMethod method = CompressionMethod.Deflated)
        {
            if (name == null) 
                throw new ArgumentNullException("name");
            if (name.Length > 0xffff) 
                throw new ArgumentException("Name is too long", "name");
            if ((versionRequiredToExtract != 0) && ((versionRequiredToExtract < 10))) 
                throw new ArgumentOutOfRangeException("versionRequiredToExtract");
            this.DateTime = DateTime.Now;
            this.m_Name = name;
            this.m_VersionMadeBy = (ushort)madeByInfo;
            this.versionToExtract = (ushort)versionRequiredToExtract;
            this.m_Method = method;
        }
        public bool HasCrc
        {
            get
            {
                return ((m_Known & Known.Crc)) != 0;
            }
        }
        public bool IsCrypted
        {
            get
            {
                return ((Flags & 1)) != 0;
            }
            set
            {
                if (value) 
                    Flags |= 1;
                else 
                    Flags &= (~1);
            }
        }
        public bool IsUnicodeText
        {
            get
            {
                return ((Flags & ((int)GeneralBitFlags.UnicodeText))) != 0;
            }
            set
            {
                if (value) 
                    Flags |= ((int)GeneralBitFlags.UnicodeText);
                else 
                    Flags &= (~((int)GeneralBitFlags.UnicodeText));
            }
        }
        internal byte CryptoCheckValue
        {
            get
            {
                return m_CryptoCheckValue;
            }
            set
            {
                m_CryptoCheckValue = value;
            }
        }
        public int ZipFileIndex
        {
            get
            {
                return m_ZipFileIndex;
            }
            set
            {
                m_ZipFileIndex = value;
            }
        }
        public int Offset
        {
            get
            {
                return m_Ooffset;
            }
            set
            {
                m_Ooffset = value;
            }
        }
        public int ExternalFileAttributes
        {
            get
            {
                if (((m_Known & Known.ExternalAttributes)) == 0) 
                    return -1;
                else 
                    return m_ExternalFileAttributes;
            }
            set
            {
                m_ExternalFileAttributes = value;
                m_Known |= Known.ExternalAttributes;
            }
        }
        public int VersionMadeBy
        {
            get
            {
                return (m_VersionMadeBy & 0xff);
            }
        }
        public bool IsDOSEntry
        {
            get
            {
                return ((HostSystem == ((int)HostSystemID.Msdos)) || (HostSystem == ((int)HostSystemID.WindowsNT)));
            }
        }
        bool HasDosAttributes(int attributes)
        {
            bool result = false;
            if (((m_Known & Known.ExternalAttributes)) != 0) 
            {
                if ((((HostSystem == ((int)HostSystemID.Msdos)) || (HostSystem == ((int)HostSystemID.WindowsNT)))) && ((ExternalFileAttributes & attributes)) == attributes) 
                    result = true;
            }
            return result;
        }
        public int HostSystem
        {
            get
            {
                return ((m_VersionMadeBy >> 8)) & 0xff;
            }
            set
            {
                m_VersionMadeBy &= 0xff;
                m_VersionMadeBy |= ((ushort)(((value & 0xff)) << 8));
            }
        }
        public int Version
        {
            get
            {
                if (versionToExtract != 0) 
                    return versionToExtract;
                else 
                {
                    int result = 10;
                    if (AESKeySize > 0) 
                        result = ZipConstants.VERSION_AES;
                    else if (CentralHeaderRequiresZip64) 
                        result = ZipConstants.VersionZip64;
                    else if (CompressionMethod.Deflated == m_Method) 
                        result = 20;
                    else if (IsDirectory == true) 
                        result = 20;
                    else if (IsCrypted == true) 
                        result = 20;
                    else if (this.HasDosAttributes(0x08)) 
                        result = 11;
                    return result;
                }
            }
        }
        public bool CanDecompress
        {
            get
            {
                return (Version <= ZipConstants.VersionMadeBy) && ((((Version == 10) || (Version == 11) || (Version == 20)) || (Version == 45) || (Version == 51))) && this.IsCompressionMethodSupported();
            }
        }
        public void ForceZip64()
        {
            forceZip64_ = true;
        }
        public bool IsZip64Forced()
        {
            return forceZip64_;
        }
        public bool LocalHeaderRequiresZip64
        {
            get
            {
                bool result = forceZip64_;
                if (!result) 
                {
                    int trueCompressedSize = m_CompressedSize;
                    if ((versionToExtract == 0) && IsCrypted) 
                        trueCompressedSize += ZipConstants.CryptoHeaderSize;
                    result = ((versionToExtract == 0) || (((int)versionToExtract) >= ZipConstants.VersionZip64));
                }
                return result;
            }
        }
        public bool CentralHeaderRequiresZip64
        {
            get
            {
                return LocalHeaderRequiresZip64;
            }
        }
        public uint DosTime
        {
            get
            {
                if (((m_Known & Known.Time)) == 0) 
                    return 0;
                else 
                    return m_DosTime;
            }
            set
            {
                m_DosTime = (uint)value;
                m_Known |= Known.Time;
            }
        }
        public DateTime DateTime
        {
            get
            {
                uint sec = (uint)Math.Min(59, 2 * ((m_DosTime & 0x1f)));
                uint min = (uint)Math.Min(59, ((m_DosTime >> 5)) & 0x3f);
                uint hrs = (uint)Math.Min(23, ((m_DosTime >> 11)) & 0x1f);
                uint mon = (uint)Math.Max(1, Math.Min(12, (((m_DosTime >> 21)) & 0xf)));
                uint year = (uint)(((((m_DosTime >> 25)) & 0x7f)) + 1980);
                int day = Math.Max(1, Math.Min(DateTime.DaysInMonth((int)year, (int)mon), (int)((((m_DosTime >> 16)) & 0x1f))));
                return new DateTime((int)year, (int)mon, day, (int)hrs, (int)min, (int)sec);
            }
            set
            {
                uint year = (uint)value.Year;
                uint month = (uint)value.Month;
                uint day = (uint)value.Day;
                uint hour = (uint)value.Hour;
                uint minute = (uint)value.Minute;
                uint second = (uint)value.Second;
                if (year < 1980) 
                {
                    year = 1980;
                    month = 1;
                    day = 1;
                    hour = 0;
                    minute = 0;
                    second = 0;
                }
                else if (year > 2107) 
                {
                    year = 2107;
                    month = 12;
                    day = 31;
                    hour = 23;
                    minute = 59;
                    second = 59;
                }
                DosTime = ((((((year - 1980)) & 0x7f)) << 25 | (month << 21) | (day << 16)) | (hour << 11) | (minute << 5)) | ((second >> 1));
            }
        }
        public string Name
        {
            get
            {
                return m_Name;
            }
        }
        public int Size
        {
            get
            {
                return (((m_Known & Known.Size)) != 0 ? m_Size : -1);
            }
            set
            {
                this.m_Size = value;
                this.m_Known |= Known.Size;
            }
        }
        public int CompressedSize
        {
            get
            {
                return (((m_Known & Known.CompressedSize)) != 0 ? m_CompressedSize : -1);
            }
            set
            {
                this.m_CompressedSize = value;
                this.m_Known |= Known.CompressedSize;
            }
        }
        public uint Crc
        {
            get
            {
                return (((m_Known & Known.Crc)) != 0 ? m_Crc : (uint)0);
            }
            set
            {
                this.m_Crc = value;
                this.m_Known |= Known.Crc;
            }
        }
        public bool CrcOk
        {
            get
            {
                return ((m_Known & Known.Crc)) != 0;
            }
        }
        public CompressionMethod _CompressionMethod
        {
            get
            {
                return m_Method;
            }
            set
            {
                if (!IsCompressionMethodSupportedEx(value)) 
                    throw new NotSupportedException("Compression method not supported");
                this.m_Method = value;
            }
        }
        internal CompressionMethod CompressionMethodForHeader
        {
            get
            {
                return (AESKeySize > 0 ? CompressionMethod.WinZipAES : m_Method);
            }
        }
        public byte[] ExtraData
        {
            get
            {
                return m_Extra;
            }
            set
            {
                if (value == null) 
                    m_Extra = null;
                else 
                {
                    if (value.Length > 0xffff) 
                        throw new ArgumentOutOfRangeException("value");
                    m_Extra = new byte[(int)value.Length];
                    Array.Copy(value, 0, m_Extra, 0, value.Length);
                }
            }
        }
        public int AESKeySize
        {
            get
            {
                switch (_aesEncryptionStrength) { 
                case 0:
                    return 0;
                case 1:
                    return 128;
                case 2:
                    return 192;
                case 3:
                    return 256;
                default: 
                    throw new Exception("Invalid AESEncryptionStrength " + _aesEncryptionStrength);
                }
            }
            set
            {
                switch (value) { 
                case 0:
                    _aesEncryptionStrength = 0;
                    break;
                case 128:
                    _aesEncryptionStrength = 1;
                    break;
                case 256:
                    _aesEncryptionStrength = 3;
                    break;
                default: 
                    throw new Exception("AESKeySize must be 0, 128 or 256: " + value);
                }
            }
        }
        internal byte AESEncryptionStrength
        {
            get
            {
                return (byte)_aesEncryptionStrength;
            }
        }
        internal int AESSaltLen
        {
            get
            {
                return AESKeySize / 16;
            }
        }
        internal int AESOverheadSize
        {
            get
            {
                return 12 + AESSaltLen;
            }
        }
        internal void ProcessExtraData(bool localHeader)
        {
            ZipExtraData extraData = new ZipExtraData(this.m_Extra);
            if (extraData.Find(0x0001)) 
            {
                forceZip64_ = true;
                if (extraData.ValueLength < 4) 
                    return;
                if (localHeader || (m_Size == int.MaxValue)) 
                    m_Size = (int)extraData.ReadLong();
                if (localHeader || (m_CompressedSize == int.MaxValue)) 
                    m_CompressedSize = (int)extraData.ReadLong();
                if (!localHeader && (m_Ooffset == int.MaxValue)) 
                    m_Ooffset = (int)extraData.ReadLong();
            }
            else if ((((int)((versionToExtract & 0xff))) >= ZipConstants.VersionZip64) && (((m_Size == int.MaxValue) || (m_CompressedSize == int.MaxValue)))) 
                throw new Exception("Zip64 Extended information required but is missing.");
            if (extraData.Find(10)) 
            {
                if (extraData.ValueLength < 4) 
                    throw new Exception("NTFS Extra data invalid");
                extraData.ReadInt();
                while (extraData.UnreadCount >= 4) 
                {
                    int ntfsTag = extraData.ReadShort();
                    int ntfsLength = extraData.ReadShort();
                    if (ntfsTag == 1) 
                    {
                        if (ntfsLength >= 24) 
                        {
                            int lastModification = extraData.ReadLong();
                            int lastAccess = extraData.ReadLong();
                            int createTime = extraData.ReadLong();
                        }
                        break;
                    }
                    else 
                        extraData.Skip(ntfsLength);
                }
            }
            else if (extraData.Find(0x5455)) 
            {
                int length = extraData.ValueLength;
                int flags = extraData.ReadByte();
                if ((((flags & 1)) != 0) && (length >= 5)) 
                {
                    int iTime = extraData.ReadInt();
                    DateTime = (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(iTime);
                }
            }
            if (m_Method == CompressionMethod.WinZipAES) 
                this.ProcessAESExtraData(extraData);
        }
        private void ProcessAESExtraData(ZipExtraData extraData)
        {
            if (extraData.Find(0x9901)) 
            {
                versionToExtract = ZipConstants.VERSION_AES;
                Flags = Flags | ((int)GeneralBitFlags.StrongEncryption);
                int length = extraData.ValueLength;
                if (length < 7) 
                    throw new Exception("AES Extra Data Length " + length + " invalid.");
                int ver = extraData.ReadShort();
                int vendorId = extraData.ReadShort();
                int encrStrength = extraData.ReadByte();
                int actualCompress = extraData.ReadShort();
                _aesVer = ver;
                _aesEncryptionStrength = encrStrength;
                m_Method = (CompressionMethod)actualCompress;
            }
            else 
                throw new Exception("AES Extra Data missing");
        }
        public string Comment
        {
            get
            {
                return m_Comment;
            }
            set
            {
                if ((value != null) && (value.Length > 0xffff)) 
                    throw new ArgumentOutOfRangeException("value", "cannot exceed 65535");
                m_Comment = value;
            }
        }
        public bool IsDirectory
        {
            get
            {
                int nameLength = m_Name.Length;
                bool result = (((nameLength > 0) && (((m_Name[nameLength - 1] == '/') || (m_Name[nameLength - 1] == '\\'))))) || this.HasDosAttributes(16);
                return result;
            }
        }
        public bool IsFile
        {
            get
            {
                return !IsDirectory && !this.HasDosAttributes(8);
            }
        }
        public bool IsCompressionMethodSupported()
        {
            return IsCompressionMethodSupportedEx(_CompressionMethod);
        }
        public object Clone()
        {
            return this;
        }
        public override string ToString()
        {
            return string.Format("{0}: {1}", m_Name, m_CompressedSize);
        }
        public static bool IsCompressionMethodSupportedEx(CompressionMethod method)
        {
            return (method == CompressionMethod.Deflated) || (method == CompressionMethod.Stored);
        }
        Known m_Known;
        int m_ExternalFileAttributes = -1;
        ushort m_VersionMadeBy;
        string m_Name;
        int m_Size;
        int m_CompressedSize;
        ushort versionToExtract;
        uint m_Crc;
        uint m_DosTime;
        CompressionMethod m_Method = CompressionMethod.Deflated;
        byte[] m_Extra;
        string m_Comment;
        public int Flags;
        int m_ZipFileIndex = -1;
        int m_Ooffset;
        bool forceZip64_;
        byte m_CryptoCheckValue;
        int _aesVer;
        int _aesEncryptionStrength;
    }
}