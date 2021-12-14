/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // This class contains constants used for Zip format files
    class ZipConstants
    {
        public const int VersionMadeBy = 51;
        public const int VersionStrongEncryption = 50;
        public const int VERSION_AES = 51;
        public const int VersionZip64 = 45;
        public const int LocalHeaderBaseSize = 30;
        public const int Zip64DataDescriptorSize = 24;
        public const int DataDescriptorSize = 16;
        public const int CentralHeaderBaseSize = 46;
        public const int EndOfCentralRecordBaseSize = 22;
        public const int CryptoHeaderSize = 12;
        public const int LocalHeaderSignature = ('P' | ('K' << 8) | (3 << 16)) | (4 << 24);
        public const int LOCSIG = ('P' | ('K' << 8) | (3 << 16)) | (4 << 24);
        public const int SpanningSignature = ('P' | ('K' << 8) | (7 << 16)) | (8 << 24);
        public const int SPANNINGSIG = ('P' | ('K' << 8) | (7 << 16)) | (8 << 24);
        public const int SpanningTempSignature = ('P' | ('K' << 8) | ('0' << 16)) | ('0' << 24);
        public const int SPANTEMPSIG = ('P' | ('K' << 8) | ('0' << 16)) | ('0' << 24);
        public const int DataDescriptorSignature = ('P' | ('K' << 8) | (7 << 16)) | (8 << 24);
        public const int EXTSIG = ('P' | ('K' << 8) | (7 << 16)) | (8 << 24);
        public const int CENSIG = ('P' | ('K' << 8) | (1 << 16)) | (2 << 24);
        public const int CentralHeaderSignature = ('P' | ('K' << 8) | (1 << 16)) | (2 << 24);
        public const int Zip64CentralFileHeaderSignature = ('P' | ('K' << 8) | (6 << 16)) | (6 << 24);
        public const int CENSIG64 = ('P' | ('K' << 8) | (6 << 16)) | (6 << 24);
        public const int Zip64CentralDirLocatorSignature = ('P' | ('K' << 8) | (6 << 16)) | (7 << 24);
        public const int ArchiveExtraDataSignature = ('P' | ('K' << 8) | (6 << 16)) | (7 << 24);
        public const int CentralHeaderDigitalSignature = ('P' | ('K' << 8) | (5 << 16)) | (5 << 24);
        public const int CENDIGITALSIG = ('P' | ('K' << 8) | (5 << 16)) | (5 << 24);
        public const int EndOfCentralDirectorySignature = ('P' | ('K' << 8) | (5 << 16)) | (6 << 24);
        public const int ENDSIG = ('P' | ('K' << 8) | (5 << 16)) | (6 << 24);
        public static string ConvertToString(byte[] data, int count)
        {
            if (data == null) 
                return string.Empty;
            string tmp = Pullenti.Util.TextHelper.ReadStringFromBytes(data, false);
            return tmp;
        }
        public static string ConvertToString0(byte[] data)
        {
            if (data == null) 
                return string.Empty;
            return ConvertToString(data, data.Length);
        }
        public static string ConvertToStringExt(int flags, byte[] data, int count)
        {
            if (data == null) 
                return string.Empty;
            if (((flags & ((int)GeneralBitFlags.UnicodeText))) != 0) 
                return Pullenti.Util.MiscHelper.DecodeStringUtf8(data, 0, count);
            else 
                return ConvertToString(data, count);
        }
        public static string ConvertToStringExt0(int flags, byte[] data)
        {
            if (data == null) 
                return string.Empty;
            if (((flags & ((int)GeneralBitFlags.UnicodeText))) != 0) 
                return Pullenti.Util.MiscHelper.DecodeStringUtf8(data, 0, data.Length);
            else 
                return ConvertToString(data, data.Length);
        }
        public static byte[] ConvertToArrayStr(string str)
        {
            if (str == null) 
                return new byte[(int)0];
            return Pullenti.Util.MiscHelper.EncodeStringUtf8(str, false);
        }
        public static byte[] ConvertToArray(int flags, string str)
        {
            if (str == null) 
                return new byte[(int)0];
            if (((flags & ((int)GeneralBitFlags.UnicodeText))) != 0) 
                return Pullenti.Util.MiscHelper.EncodeStringUtf8(str, false);
            else 
                return ConvertToArrayStr(str);
        }
        ZipConstants()
        {
        }
    }
}