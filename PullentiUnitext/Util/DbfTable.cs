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

namespace Pullenti.Util
{
    // Поддержка работы с таблицами DBF
    public class DbfTable : IDisposable
    {
        public DbfTable(string fileName)
        {
            m_Stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] buf = new byte[(int)1024];
            if (m_Stream.Read(buf, 0, 0x20) != 0x20 || buf[0] != 3) 
                throw new Exception("Bad DBF format");
            int headLen = (((int)buf[9]) << 8) | buf[8];
            int recLen = (((int)buf[11]) << 8) | buf[10];
            for (; (m_Stream.Position + 0x20) <= headLen; ) 
            {
                if (m_Stream.Read(buf, 0, 0x20) != 0x20) 
                    throw new Exception("Bad DBF format");
                int i = 0;
                for (; i < 11; i++) 
                {
                    if (buf[i] == 0) 
                        break;
                }
                string nam = MiscHelper.DecodeStringAscii(buf, 0, i).Trim();
                ColumnnNames.Add(nam);
                ColumnLengths.Add((int)((uint)buf[0x10]));
                Values.Add(null);
            }
            m_Stream.Position = headLen - 1;
            int b = m_Stream.ReadByte();
            if (b != 13) 
                throw new Exception("Bad DBF format (end of head not 13)");
            List<string> vals = new List<string>();
            int le = 0;
            foreach (int v in ColumnLengths) 
            {
                le += v;
                vals.Add(null);
            }
            if ((le + 1) != recLen) 
                throw new Exception(string.Format("Total columns length {0} != length of record {1}", le + 1, recLen));
            m_Enc = new EncodingWrapper(EncodingStandard.CP1251);
            bufRec = new byte[(int)recLen];
            m_FirstRecPos = m_Stream.Position;
        }
        public void Dispose()
        {
            if (m_Stream != null) 
                m_Stream.Dispose();
            m_Stream = null;
        }
        FileStream m_Stream;
        long m_FirstRecPos;
        public List<string> ColumnnNames = new List<string>();
        public List<int> ColumnLengths = new List<int>();
        byte[] bufRec;
        EncodingWrapper m_Enc;
        public List<string> Values = new List<string>();
        public int Percent = 0;
        public string GetString(string columnName)
        {
            int i = ColumnnNames.IndexOf(columnName);
            if (i < 0) 
                return null;
            if (string.IsNullOrEmpty(Values[i])) 
                return null;
            return Values[i];
        }
        public Guid? GetGuid(string columnName)
        {
            string val = this.GetString(columnName);
            if (string.IsNullOrEmpty(val)) 
                return null;
            try 
            {
                return new Guid(val);
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public bool ReadNext(int onlyColumnIndex = -1)
        {
            if (m_Stream == null) 
                return false;
            if (m_Stream.Read(bufRec, 0, bufRec.Length) != bufRec.Length) 
                return false;
            if (bufRec[0] != 0x20) 
                return false;
            int d = 1;
            int i;
            for (i = 0; i < bufRec.Length; i++) 
            {
                int j = (int)bufRec[i];
                if (j >= 0xE0 && j <= 0xEF) 
                    j -= 0x30;
                if (j >= 0x80 && j <= 0xDF) 
                    bufRec[i] = (byte)((j + 0x40));
                if (j == 0xF1) 
                    bufRec[i] = 0xB8;
                if (j == 0xF0) 
                    bufRec[i] = 0xA8;
            }
            for (i = 0; i < ColumnnNames.Count; i++) 
            {
                if ((onlyColumnIndex < 0) || onlyColumnIndex == i) 
                {
                    string val = m_Enc.GetString(bufRec, d, ColumnLengths[i]).Trim();
                    Values[i] = val;
                }
                d += ColumnLengths[i];
            }
            if (m_Stream.Length < 100000) 
                Percent = (int)(((m_Stream.Position * 100) / m_Stream.Length));
            else 
                Percent = (int)((m_Stream.Position / ((m_Stream.Length / 100))));
            return true;
        }
        public void Reread()
        {
            if (m_Stream != null) 
                m_Stream.Position = m_FirstRecPos;
        }
    }
}