/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;

namespace Pullenti.Unitext.Internal.Misc
{
    class PngChunk
    {
        public PngChunk(string typ)
        {
            Typ = typ;
        }
        public string Typ;
        public byte[] Data;
        public override string ToString()
        {
            return string.Format("{0}: {1}bytes", Typ, (Data == null ? 0 : Data.Length));
        }
        public void Write(Stream str)
        {
            this._writeInt(str, (uint)((Data == null ? 0 : Data.Length)));
            for (int i = 0; i < 4; i++) 
            {
                str.WriteByte((byte)Typ[i]);
            }
            if (Data != null && Data.Length > 0) 
                str.Write(Data, 0, Data.Length);
            uint crc = this._calcCrc();
            this._writeInt(str, crc);
        }
        void _writeInt(Stream res, uint val)
        {
            res.WriteByte((byte)((((val >> 24)) & 0xFF)));
            res.WriteByte((byte)((((val >> 16)) & 0xFF)));
            res.WriteByte((byte)((((val >> 8)) & 0xFF)));
            res.WriteByte((byte)((val & 0xFF)));
        }
        uint _calcCrc()
        {
            uint crc = (uint)0xFFFFFFFF;
            for (int i = 0; i < 4; i++) 
            {
                byte b = (byte)Typ[i];
                crc = m_CrcTable[((crc ^ b)) & 0xff] ^ ((crc >> 8));
            }
            if (Data != null) 
            {
                for (int n = 0; n < Data.Length; n++) 
                {
                    crc = m_CrcTable[((crc ^ Data[n])) & 0xff] ^ ((crc >> 8));
                }
            }
            return crc ^ 0xFFFFFFFF;
        }
        static uint[] m_CrcTable;
        static PngChunk()
        {
            m_CrcTable = new uint[(int)256];
            for (int n = 0; n < 256; n++) 
            {
                uint c = (uint)n;
                for (int k = 0; k < 8; k++) 
                {
                    if (((c & 1)) != 0) 
                        c = 0xedb88320 ^ ((c >> 1));
                    else 
                        c = c >> 1;
                }
                m_CrcTable[n] = c;
            }
        }
    }
}