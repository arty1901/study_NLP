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
    public class PngWrapper
    {
        public PngWrapper(int width, int height, bool isGray, byte[] colorMap)
        {
            m_Data = new List<byte[]>();
            m_RowSize = width;
            if (!isGray) 
            {
                if (colorMap == null) 
                    m_RowSize *= 3;
            }
            m_RowSize++;
            m_Chunks = new List<PngChunk>();
            m_Head = new PngHead();
            m_Chunks.Add(m_Head);
            m_Head.Width = width;
            m_Head.Height = height;
            m_Head.ColorType = (byte)((isGray ? 0 : 2));
            if (colorMap != null) 
            {
                m_ColorMap = colorMap;
                if (!isGray) 
                {
                    m_Head.ColorType = (byte)3;
                    PngChunk mmm = new PngChunk("PLTE");
                    mmm.Data = colorMap;
                    if (!isGray && ((colorMap.Length % 3)) != 0 && colorMap.Length >= 6) 
                    {
                        int ii = colorMap.Length % 3;
                        byte[] dat = new byte[(int)(colorMap.Length - ii)];
                        for (int j = 0; j < dat.Length; j++) 
                        {
                            dat[j] = colorMap[j];
                        }
                        mmm.Data = dat;
                    }
                    m_Chunks.Add(mmm);
                }
            }
        }
        PngHead m_Head;
        int m_RowSize;
        byte[] m_ColorMap;
        List<byte[]> m_Data;
        byte[] m_Row;
        int m_RowPos;
        List<PngChunk> m_Chunks;
        public void BeginRow()
        {
            m_Row = new byte[(int)m_RowSize];
            m_RowPos = 0;
        }
        public void EndRow()
        {
            m_Data.Add(m_Row);
        }
        public void AddPixelRgb(byte r, byte g, byte b)
        {
            if ((m_RowPos + 2) >= m_RowSize) 
                return;
            m_Row[m_RowPos] = r;
            if (m_Head.ColorType == 0) 
                m_RowPos += 1;
            else 
            {
                m_Row[m_RowPos + 1] = g;
                m_Row[m_RowPos + 2] = b;
                m_RowPos += 3;
            }
        }
        public void AddPixelGray(byte gr)
        {
            if (m_RowPos >= m_RowSize) 
                return;
            m_Row[m_RowPos] = gr;
            m_RowPos += 1;
        }
        public void AddPixelIndex(byte gr)
        {
            if (m_RowPos >= m_RowSize) 
                return;
            if (m_ColorMap != null && m_Head.ColorType == 0) 
            {
                int i = (int)gr;
                if (i >= 0 && (i < m_ColorMap.Length)) 
                {
                    m_Row[m_RowPos] = m_ColorMap[i];
                    m_RowPos += 1;
                }
            }
            else 
            {
                m_Row[m_RowPos] = gr;
                m_RowPos += 1;
            }
        }
        public void Commit()
        {
            PngChunk dat = new PngChunk("IDAT");
            using (MemoryStream body = new MemoryStream()) 
            {
                for (int r = 0; r < m_Data.Count; r++) 
                {
                    body.Write(m_Data[r], 0, m_RowSize);
                }
                byte[] src0 = body.ToArray();
                dat.Data = Pullenti.Util.ArchiveHelper.CompressZlib(src0);
                m_Head.Height = m_Data.Count;
            }
            m_Chunks.Add(dat);
            m_Chunks.Add(new PngChunk("IEND"));
        }
        public byte[] GetBytes()
        {
            if (m_Chunks.Count < 1) 
                this.Commit();
            using (MemoryStream res = new MemoryStream()) 
            {
                res.WriteByte((byte)137);
                res.WriteByte((byte)80);
                res.WriteByte((byte)78);
                res.WriteByte((byte)71);
                res.WriteByte((byte)13);
                res.WriteByte((byte)10);
                res.WriteByte((byte)26);
                res.WriteByte((byte)10);
                foreach (PngChunk ch in m_Chunks) 
                {
                    ch.Write(res);
                }
                return res.ToArray();
            }
        }
        public static byte FilterByte(int typ, bool encode, byte x, byte a, byte b, byte c)
        {
            if (typ == 1) 
                return (encode ? (byte)((x - a)) : (byte)((x + a)));
            if (typ == 2) 
                return (encode ? (byte)((x - b)) : (byte)((x + b)));
            if (typ == 3) 
            {
                int i = (int)a;
                i += ((int)b);
                i /= 2;
                return (encode ? (byte)((x - i)) : (byte)((x + i)));
            }
            if (typ == 4) 
            {
                int p = (int)a;
                p += ((int)b);
                p -= ((int)c);
                int pa = p - a;
                if (pa < 0) 
                    pa = -pa;
                int pb = p - b;
                if (pb < 0) 
                    pb = -pb;
                int pc = p - c;
                if (pc < 0) 
                    pc = -pc;
                int i = 0;
                if (pa <= pb && pa <= pc) 
                    i = a;
                else if (pb <= pc) 
                    i = b;
                else 
                    i = c;
                return (encode ? (byte)((x - i)) : (byte)((x + i)));
            }
            return x;
        }
    }
}