/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Pullenti.Unitext.Internal.Misc
{
    class MyZipFile : IDisposable
    {
        public List<MyZipEntry> Entries = new List<MyZipEntry>();
        public MyZipFile(string fileName, byte[] content)
        {
            if (content != null) 
                m_Stream = new MemoryStream(content);
            else 
                m_Stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            m_Buf = new byte[(int)4];
            while (true) 
            {
                int i = m_Stream.Read(m_Buf, 0, 4);
                if ((i != 4 || m_Buf[0] != 0x50 || m_Buf[1] != 0x4B) || m_Buf[2] != 3 || m_Buf[3] != 4) 
                    break;
                MyZipEntry e = new MyZipEntry(this);
                this._readShort();
                int flags = this._readShort();
                if (((flags & 1)) != 0) 
                    e.Encrypted = true;
                e.Method = this._readShort();
                this._readShort();
                this._readShort();
                int crc = this._readInt();
                e.CompressDataSize = this._readInt();
                e.UncompressDataSize = this._readInt();
                int flen = this._readShort();
                int extr = this._readShort();
                byte[] fnam = new byte[(int)flen];
                m_Stream.Read(fnam, 0, flen);
                try 
                {
                    e.Name = Pullenti.Util.MiscHelper.DecodeStringUtf8(fnam, 0, -1);
                }
                catch(Exception ex) 
                {
                    e.Name = Pullenti.Util.MiscHelper.DecodeString1251(fnam, 0, -1);
                }
                e.Pos = ((int)m_Stream.Position) + extr;
                if (extr > 10 && (e.CompressDataSize < 0)) 
                {
                    int ii = this._readShort();
                    if (ii == 1) 
                    {
                        this._readShort();
                        e.UncompressDataSize = this._readInt();
                        this._readInt();
                        e.CompressDataSize = this._readInt();
                        this._readInt();
                    }
                }
                m_Stream.Position = e.Pos;
                if (e.CompressDataSize > 0) 
                    m_Stream.Position += e.CompressDataSize;
                if (((flags & 8)) != 0 && e.CompressDataSize == 0) 
                {
                    byte[] buf = new byte[(int)10000];
                    while (m_Stream.Position < m_Stream.Length) 
                    {
                        int p0 = (int)m_Stream.Position;
                        i = m_Stream.Read(buf, 0, buf.Length);
                        if (i < 6) 
                            break;
                        for (int j = 0; j < (i - 4); j++) 
                        {
                            if (buf[j] == 0x50 && buf[j + 1] == 0x4B) 
                            {
                                if (((buf[j + 2] == 3 && buf[j + 3] == 4)) || ((buf[j + 2] == 1 && buf[j + 3] == 2)) || ((buf[j + 2] == 5 && buf[j + 3] == 6))) 
                                {
                                    p0 += j;
                                    e.CompressDataSize = p0 - 12 - e.Pos;
                                    m_Stream.Position = p0 - 4;
                                    e.UncompressDataSize = this._readInt();
                                    break;
                                }
                            }
                        }
                        if (e.CompressDataSize > 0) 
                            break;
                        m_Stream.Position -= 4;
                    }
                }
                Entries.Add(e);
            }
        }
        Stream m_Stream;
        byte[] m_Buf;
        int _readShort()
        {
            int i = m_Stream.Read(m_Buf, 0, 2);
            if (i != 2) 
                return -1;
            i = m_Buf[1];
            i <<= 8;
            i |= m_Buf[0];
            return i;
        }
        int _readInt()
        {
            int i = m_Stream.Read(m_Buf, 0, 4);
            if (i != 4) 
                return -1;
            i = m_Buf[3];
            i <<= 8;
            i |= m_Buf[2];
            i <<= 8;
            i |= m_Buf[1];
            i <<= 8;
            i |= m_Buf[0];
            return i;
        }
        public void Dispose()
        {
            if (m_Stream != null) 
                m_Stream.Dispose();
        }
        public byte[] Unzip(MyZipEntry e)
        {
            if (e.CompressDataSize == 0) 
                return null;
            if (e.Method == 8 || e.Method == 0) 
            {
            }
            else 
                return null;
            byte[] buf = new byte[(int)e.CompressDataSize];
            m_Stream.Position = e.Pos;
            m_Stream.Read(buf, 0, buf.Length);
            if (e.Method == 0) 
                return buf;
            return Pullenti.Util.ArchiveHelper.DecompressDeflate(buf);
        }
    }
}