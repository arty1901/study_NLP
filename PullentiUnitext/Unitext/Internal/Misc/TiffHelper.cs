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
    class TiffHelper
    {
        public static Pullenti.Unitext.UnitextDocument CreateDoc(byte[] content)
        {
            Pullenti.Unitext.UnitextDocument res = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Tif };
            using (MemoryStream mem = new MemoryStream(content)) 
            {
                try 
                {
                    Tiff ti = new Tiff(mem);
                    int wi;
                    int hei;
                    int dpx;
                    int dpy;
                    bool m;
                    bool hasTxt;
                    if (ti.PagesCount == 1) 
                    {
                        Pullenti.Unitext.UnilayPage p = new Pullenti.Unitext.UnilayPage() { ImageContent = content };
                        ti.GetPageInfo(0, out wi, out hei, out dpx, out dpy, out m, out hasTxt);
                        p.Width = wi;
                        p.Height = hei;
                        res.Pages.Add(p);
                    }
                    else 
                        for (int i = 0; i < ti.PagesCount; i++) 
                        {
                            using (MemoryStream pm = new MemoryStream()) 
                            {
                                ti.Store(i, pm);
                                Pullenti.Unitext.UnilayPage p = new Pullenti.Unitext.UnilayPage() { ImageContent = pm.ToArray() };
                                ti.GetPageInfo(0, out wi, out hei, out dpx, out dpy, out m, out hasTxt);
                                p.Width = wi;
                                p.Height = hei;
                                res.Pages.Add(p);
                            }
                        }
                }
                catch(Exception ex) 
                {
                    res.ErrorMessage = ex.Message;
                }
            }
            return res;
        }
        public enum CCITT_Types : int
        {
            G3_1D = 2,
            G3_2D = 3,
            G4 = 4,
        }

        /// <summary>
        /// Работа с TIFF-документами
        /// </summary>
        public class Tiff
        {
            byte[] m_Buf = new byte[(int)32];
            bool m_Msb;
            bool m_Mdi;
            List<long> m_IfdOffset = new List<long>();
            public bool IsMDI
            {
                get
                {
                    return m_Mdi;
                }
            }
            Stream m_Stream = null;
            public Tiff(Stream stream)
            {
                Stream = stream;
            }
            public Stream Stream
            {
                get
                {
                    return m_Stream;
                }
                set
                {
                    if (value == null) 
                        return;
                    m_Stream = value;
                    m_IfdOffset.Clear();
                    m_Stream.Position = 0;
                    m_Stream.Read(m_Buf, 0, 2);
                    m_Mdi = false;
                    m_Msb = false;
                    if (m_Buf[0] == 0x49 && m_Buf[1] == 0x49) 
                        m_Msb = false;
                    else if (m_Buf[0] == 0x4D && m_Buf[1] == 0x4D) 
                        m_Msb = true;
                    else if (m_Buf[0] == 0x45 && m_Buf[1] == 0x50) 
                        m_Mdi = true;
                    else 
                    {
                        m_Stream = null;
                        return;
                    }
                    int i = this.ReadInt(2);
                    while (true) 
                    {
                        i = this.ReadInt(4);
                        if (i <= 0) 
                            break;
                        m_IfdOffset.Add(i);
                        m_Stream.Position = i;
                        int cou = this.ReadInt(2);
                        m_Stream.Position = i + 2 + ((cou * 12));
                    }
                }
            }
            /// <summary>
            /// Число страниц (если 0, то это не TIFF)
            /// </summary>
            public int PagesCount
            {
                get
                {
                    return m_IfdOffset.Count;
                }
            }
            /// <summary>
            /// Сохранение страницы в отдельном файле
            /// </summary>
            public void Store(int PageIndex, Stream fout)
            {
                if ((PageIndex < 0) || PageIndex >= m_IfdOffset.Count) 
                    throw new Exception("Page index error");
                if (m_Msb) 
                    m_Buf[0] = (m_Buf[1] = 0x4D);
                else 
                    m_Buf[0] = (m_Buf[1] = 0x49);
                fout.Write(m_Buf, 0, 2);
                this.WriteInt(fout, 0x2A, 2);
                this.WriteInt(fout, 8, 4);
                long baseoff = m_IfdOffset[PageIndex];
                m_Stream.Position = baseoff;
                int i;
                int j;
                int ii = 0;
                int dircount = this.ReadInt(2);
                int outpos = (8 + 2 + (12 * dircount)) + 4;
                int stripoffpos = 0;
                int stripoffitemsize = 0;
                bool directData = false;
                int directDataPos = 0;
                int directDataLen = 0;
                List<int> StripPos = new List<int>();
                List<int> StripLen = new List<int>();
                this.WriteInt(fout, dircount, 2);
                for (i = 0; i < dircount; i++) 
                {
                    m_Stream.Position = baseoff + 2 + (i * 12);
                    int tag = this.ReadInt(2);
                    int typ = this.ReadInt(2);
                    int cou = this.ReadInt(4);
                    int dsp = this.ReadInt(4);
                    int itemsize = 1;
                    fout.Position = 8 + 2 + (ii * 12);
                    ii++;
                    this.WriteInt(fout, tag, 2);
                    this.WriteInt(fout, typ, 2);
                    this.WriteInt(fout, cou, 4);
                    switch (typ) { 
                    case 3:
                        itemsize = 2;
                        break;
                    case 4:
                        itemsize = 4;
                        break;
                    case 5:
                        itemsize = 8;
                        break;
                    case 8:
                        itemsize = 2;
                        break;
                    case 9:
                        itemsize = 4;
                        break;
                    case 10:
                        itemsize = 8;
                        break;
                    }
                    int len = itemsize * cou;
                    if (len <= 4 && tag != 0x111) 
                    {
                        this.WriteInt(fout, dsp, 4);
                        if (tag == 0x117) 
                            directDataLen = dsp;
                        continue;
                    }
                    if (tag == 0x111) 
                    {
                        stripoffpos = outpos;
                        stripoffitemsize = itemsize;
                        if (cou == 1) 
                        {
                            directData = true;
                            directDataPos = dsp;
                            stripoffpos = (int)fout.Position;
                            this.WriteInt(fout, dsp, 4);
                            continue;
                        }
                        else 
                        {
                            m_Stream.Position = dsp;
                            for (j = 0; j < cou; j++) 
                            {
                                StripPos.Add(this.ReadInt(itemsize));
                            }
                            m_Stream.Position = dsp;
                        }
                    }
                    else if (tag == 0x117) 
                    {
                        if (cou != StripPos.Count) 
                        {
                            fout.Dispose();
                            throw new Exception("StripOffsetCount != StripCount");
                        }
                        m_Stream.Position = dsp;
                        for (j = 0; j < cou; j++) 
                        {
                            StripLen.Add(this.ReadInt(itemsize));
                        }
                        m_Stream.Position = dsp;
                    }
                    this.ReadBuf(dsp, len);
                    this.WriteInt(fout, outpos, 4);
                    fout.Position = outpos;
                    fout.Write(m_Buf, 0, len);
                    outpos += len;
                    if (((outpos & 1)) == 1) 
                    {
                        outpos++;
                        this.WriteInt(fout, 0, 1);
                    }
                }
                fout.Position = 8 + 2 + (ii * 12);
                this.WriteInt(fout, 0, 4);
                fout.Position = 8;
                this.WriteInt(fout, ii, 2);
                if (directData) 
                {
                    this.ReadBuf(directDataPos, directDataLen);
                    fout.Position = stripoffpos;
                    this.WriteInt(fout, outpos, 4);
                    fout.Position = outpos;
                    fout.Write(m_Buf, 0, directDataLen);
                    outpos += directDataLen;
                }
                else 
                {
                    fout.Position = outpos;
                    for (i = 0; i < StripPos.Count; i++) 
                    {
                        this.ReadBuf(StripPos[i], StripLen[i]);
                        StripPos[i] = outpos;
                        fout.Write(m_Buf, 0, StripLen[i]);
                        outpos += StripLen[i];
                    }
                    fout.Position = stripoffpos;
                    for (i = 0; i < StripPos.Count; i++) 
                    {
                        this.WriteInt(fout, StripPos[i], stripoffitemsize);
                    }
                }
            }
            /// <summary>
            /// Извлечение текста, сформированного MODI (хранится в теге 932Fh)
            /// </summary>
            /// <param name="PageIndex">номер страницы</param>
            /// <return>текст (null при ошибке или отсутствии)</return>
            public string GetPageText(int PageIndex)
            {
                if ((PageIndex < 0) || PageIndex >= m_IfdOffset.Count) 
                    throw new Exception("Page index error");
                long baseoff = m_IfdOffset[PageIndex];
                m_Stream.Position = baseoff;
                int i;
                int dircount = this.ReadInt(2);
                for (i = 0; i < dircount; i++) 
                {
                    m_Stream.Position = baseoff + 2 + (i * 12);
                    int tag = this.ReadInt(2);
                    int typ = this.ReadInt(2);
                    int cou = this.ReadInt(4);
                    int dsp = this.ReadInt(4);
                    if (tag != 0x932F) 
                        continue;
                    m_Stream.Position = dsp;
                    if (this.ReadInt(2) != 1) 
                        return null;
                    cou = this.ReadInt(4);
                    if (cou < 1) 
                        return null;
                    byte[] buf = new byte[(int)cou];
                    m_Stream.Read(buf, 0, cou);
                    int blen = buf.Length;
                    if (blen > 0 && buf[blen - 1] == 0) 
                        blen--;
                    if (blen == 0) 
                        return null;
                    return Pullenti.Util.MiscHelper.DecodeStringUtf8(buf, 0, blen);
                }
                return null;
            }
            /// <summary>
            /// Проверка, имеет ли файл встроенные MODI-тексты
            /// </summary>
            public bool HasText
            {
                get
                {
                    for (int p = 0; p < m_IfdOffset.Count; p++) 
                    {
                        long baseoff = m_IfdOffset[p];
                        m_Stream.Position = baseoff;
                        int i;
                        int dircount = this.ReadInt(2);
                        for (i = 0; i < dircount; i++) 
                        {
                            m_Stream.Position = baseoff + 2 + (i * 12);
                            int tag = this.ReadInt(2);
                            int typ = this.ReadInt(2);
                            int cou = this.ReadInt(4);
                            int dsp = this.ReadInt(4);
                            if (tag == 0x932F) 
                                return true;
                        }
                    }
                    return false;
                }
            }
            /// <summary>
            /// Это извлечение информации о самой картинке вместе с положением данных во входном потоке 
            /// - используется для выделения CCITT-данных для PDF FaxDecode
            /// </summary>
            public bool GetDataInfo(int pageIndex, out int dataStartPos, out int dataLength, out int width, out int height, out Pullenti.Unitext.Internal.Misc.TiffHelper.CCITT_Types ccittTyp, out int pageNumber)
            {
                dataStartPos = (dataLength = 0);
                pageNumber = 0;
                width = (height = 0);
                ccittTyp = Pullenti.Unitext.Internal.Misc.TiffHelper.CCITT_Types.G3_1D;
                if ((pageIndex < 0) || pageIndex >= m_IfdOffset.Count) 
                    throw new Exception("Page index error");
                long baseoff = m_IfdOffset[pageIndex];
                m_Stream.Position = baseoff;
                int i;
                int dircount = this.ReadInt(2);
                for (i = 0; i < dircount; i++) 
                {
                    m_Stream.Position = baseoff + 2 + (i * 12);
                    int tag = this.ReadInt(2);
                    int typ = this.ReadInt(2);
                    int cou = this.ReadInt(4);
                    int dsp = this.ReadInt(4);
                    if (tag == 0x100) 
                        width = dsp;
                    else if (tag == 0x101) 
                        height = dsp;
                    else if (tag == 0x111) 
                        dataStartPos = dsp;
                    else if (tag == 0x117) 
                        dataLength = dsp;
                    else if (tag == 0x103) 
                        ccittTyp = (Pullenti.Unitext.Internal.Misc.TiffHelper.CCITT_Types)dsp;
                    else if (tag == 0x129) 
                    {
                    }
                }
                return dataLength > 0;
            }
            public void GetPageInfo(int pageIndex, out int width, out int height, out int dpi_x, out int dpi_y, out bool isMono, out bool hasText)
            {
                width = (height = (dpi_x = (dpi_y = 0)));
                isMono = (hasText = false);
                if ((pageIndex < 0) || pageIndex >= m_IfdOffset.Count) 
                    return;
                long baseoff = m_IfdOffset[pageIndex];
                m_Stream.Position = baseoff;
                int i;
                int dircount = this.ReadInt(2);
                int mono = -1;
                int resUnit = 2;
                long resXpos = (long)0;
                long resYpos = (long)0;
                for (i = 0; i < dircount; i++) 
                {
                    m_Stream.Position = baseoff + 2 + (i * 12);
                    int tag = this.ReadInt(2);
                    int typ = this.ReadInt(2);
                    int cou = this.ReadInt(4);
                    int dsp = this.ReadInt(4);
                    if (tag == 0x100) 
                        width = dsp;
                    else if (tag == 0x101) 
                        height = dsp;
                    else if (tag == 0x106 && (mono < 0)) 
                        mono = ((dsp == 0 || dsp == 1) ? 1 : 0);
                    else if (tag == 0x102 && dsp > 1) 
                        mono = 0;
                    else if (tag == 0x932F) 
                        hasText = true;
                    else if (tag == 0x128) 
                        resUnit = dsp;
                    else if (tag == 0x11A) 
                        resXpos = dsp;
                    else if (tag == 0x11B) 
                        resYpos = dsp;
                }
                isMono = mono == 1;
                if (((resUnit == 2 || resUnit == 3)) && resXpos > 0) 
                {
                    m_Stream.Position = resXpos;
                    int v1 = this.ReadInt(4);
                    int v2 = this.ReadInt(4);
                    float a = (float)v1;
                    if (v2 > 1) 
                        a /= v2;
                    if (resUnit == 3) 
                        a *= 2.54F;
                    dpi_x = (int)a;
                }
                if (((resUnit == 2 || resUnit == 3)) && resYpos > 0) 
                {
                    m_Stream.Position = resYpos;
                    int v1 = this.ReadInt(4);
                    int v2 = this.ReadInt(4);
                    float a = (float)v1;
                    if (v2 > 1) 
                        a /= v2;
                    if (resUnit == 3) 
                        a *= 2.54F;
                    dpi_y = (int)a;
                }
            }
            /// <summary>
            /// Создание TIFF по чистым данным без заголовка (фактически обрамляет заголовком)
            /// </summary>
            public void WriteTiff(Stream res, int width, int height, Pullenti.Unitext.Internal.Misc.TiffHelper.CCITT_Types typ, float xResolution, float yResolution, bool invert, byte[] data)
            {
                res.WriteByte((byte)0x49);
                res.WriteByte((byte)0x49);
                this._WriteInt(res, 0x2A, 2);
                this._WriteInt(res, (8 + data.Length + 8) + 8, 4);
                res.Write(data, 0, data.Length);
                this._WriteInt(res, (int)xResolution, 4);
                this._WriteInt(res, 1, 4);
                this._WriteInt(res, (int)yResolution, 4);
                this._WriteInt(res, 1, 4);
                int dirs = 0;
                this._WriteInt(res, 0, 2);
                this.WriteDirEntry(res, 0xFE, 4, 1, 0);
                dirs++;
                this.WriteDirEntry(res, 0x100, 3, 1, width);
                dirs++;
                this.WriteDirEntry(res, 0x101, 3, 1, height);
                dirs++;
                this.WriteDirEntry(res, 0x102, 3, 1, 1);
                dirs++;
                this.WriteDirEntry(res, 0x103, 3, 1, (int)typ);
                dirs++;
                this.WriteDirEntry(res, 0x106, 3, 1, (invert ? 1 : 0));
                dirs++;
                this.WriteDirEntry(res, 0x111, 4, 1, 8);
                dirs++;
                this.WriteDirEntry(res, 0x115, 3, 1, 1);
                dirs++;
                this.WriteDirEntry(res, 0x116, 3, 1, height);
                dirs++;
                this.WriteDirEntry(res, 0x117, 4, 1, data.Length);
                dirs++;
                this.WriteDirEntry(res, 0x11A, 5, 1, 8 + data.Length);
                dirs++;
                this.WriteDirEntry(res, 0x11B, 5, 1, 8 + data.Length + 8);
                dirs++;
                this.WriteDirEntry(res, 0x128, 3, 1, 2);
                dirs++;
                this._WriteInt(res, 0, 4);
                res.Position = (8 + data.Length + 8) + 8;
                this._WriteInt(res, dirs, 2);
            }
            int ReadInt(int count)
            {
                int res = 0;
                for (int i = 0; i < count; i++) 
                {
                    int j = m_Stream.ReadByte();
                    if (j < 0) 
                        break;
                    if (m_Msb) 
                        res |= j << (((count - 1 - i)) << 3);
                    else 
                        res |= j << (i << 3);
                }
                return res;
            }
            void ReadBuf(long pos, int size)
            {
                if (m_Buf.Length < size) 
                    m_Buf = new byte[(int)size];
                m_Stream.Position = pos;
                m_Stream.Read(m_Buf, 0, size);
            }
            byte[] fBuf2 = new byte[(int)16];
            void WriteInt(Stream fOut, int val, int count)
            {
                for (int i = 0; i < count; i++) 
                {
                    int j;
                    if (m_Msb) 
                        j = ((val >> (((count - 1 - i)) << 3))) & 0xFF;
                    else 
                        j = ((val >> (i << 3))) & 0xFF;
                    fBuf2[i] = (byte)j;
                }
                fOut.Write(fBuf2, 0, count);
            }
            void _WriteInt(Stream fOut, int val, int count)
            {
                for (int i = 0; i < count; i++) 
                {
                    int j = ((val >> (i << 3))) & 0xFF;
                    fBuf2[i] = (byte)j;
                }
                fOut.Write(fBuf2, 0, count);
            }
            void WriteDirEntry(Stream fOut, int tag, int typ, int count, int data)
            {
                this._WriteInt(fOut, tag, 2);
                this._WriteInt(fOut, typ, 2);
                this._WriteInt(fOut, count, 4);
                this._WriteInt(fOut, data, 4);
            }
        }

    }
}