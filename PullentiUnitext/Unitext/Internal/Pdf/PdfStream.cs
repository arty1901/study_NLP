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
using System.Text;

namespace Pullenti.Unitext.Internal.Pdf
{
    class PdfStream : IDisposable
    {
        private Stream m_Stream = null;
        public PdfStream(string fileName, byte[] fileContent)
        {
            if (fileContent != null) 
                m_Stream = new MemoryStream(fileContent);
            else 
                m_Stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }
        public void Dispose()
        {
            if (m_Stream != null) 
                m_Stream.Dispose();
            m_Stream = null;
        }
        bool isSpace(byte ch)
        {
            if ((ch == 0x20 || ch == 0xD || ch == 0xA) || ch == 9) 
                return true;
            return false;
        }
        bool isDelimiter(byte ch)
        {
            if ((ch == '(' || ch == ')' || ch == '<') || ch == '>') 
                return true;
            if ((ch == '[' || ch == ']' || ch == '{') || ch == '}') 
                return true;
            if (ch == '/') 
                return true;
            return false;
        }
        private int hexToInt(byte ch)
        {
            if (ch >= '0' && ch <= '9') 
                return (int)((ch - '0'));
            if (ch >= 'A' && ch <= 'F') 
                return (int)(((ch - 'A') + 10));
            if (ch >= 'a' && ch <= 'f') 
                return (int)(((ch - 'a') + 10));
            return -1;
        }
        private byte toHex(int val)
        {
            if (val < 10) 
                return (byte)(('0' + val));
            return (byte)((('A' + val) - 10));
        }
        public int Position
        {
            get
            {
                return (int)m_Stream.Position;
            }
            set
            {
                m_Stream.Position = value;
            }
        }
        public int Length
        {
            get
            {
                return (int)m_Stream.Length;
            }
        }
        public int ReadByte()
        {
            return m_Stream.ReadByte();
        }
        public int Read(byte[] arr, int off, int len)
        {
            return m_Stream.Read(arr, off, len);
        }
        public int PeekSolidByte()
        {
            bool cmt = false;
            for (int i = m_Stream.ReadByte(); i >= 0; i = m_Stream.ReadByte()) 
            {
                byte ch = (byte)i;
                if (ch == '%') 
                    cmt = true;
                else if (ch == 0xD || ch == 0xA) 
                    cmt = false;
                else if (ch != 0x20 && ch != 9 && !cmt) 
                {
                    m_Stream.Position -= 1;
                    return i;
                }
            }
            return -1;
        }
        public byte[] ReadHead()
        {
            byte[] head = new byte[(int)4];
            int p0 = (int)m_Stream.Position;
            int i;
            m_Stream.Read(head, 0, 4);
            if ((head.Length != 4 || head[0] != '%' || head[1] != 'P') || head[2] != 'D' || head[3] != 'F') 
                throw new Exception("File not in PDF format");
            for (i = 4; i < (m_Stream.Length - 4); i++) 
            {
                m_Stream.Position = i;
                m_Stream.Read(head, 0, 4);
                if ((head[0] == ' ' && head[1] == 'o' && head[2] == 'b') && head[3] == 'j') 
                    break;
            }
            if (i >= (m_Stream.Length - 4)) 
                throw new Exception("Bad PDF format");
            for (; i > 4; i--) 
            {
                m_Stream.Position = i;
                byte b = (byte)m_Stream.ReadByte();
                if (b != ' ' && (((b < 0x30) || b > 0x39))) 
                {
                    i++;
                    break;
                }
            }
            m_Stream.Position = p0;
            head = new byte[(int)i];
            m_Stream.Read(head, 0, head.Length);
            return head;
        }
        public byte[] ReadLexemString()
        {
            byte ch = (byte)m_Stream.ReadByte();
            int p0 = (int)m_Stream.Position;
            int i;
            List<byte> val = new List<byte>();
            if (ch == '<') 
            {
                while ((((i = m_Stream.ReadByte()))) > 0) 
                {
                    ch = (byte)i;
                    if (ch == '>') 
                        break;
                    if (this.isSpace(ch)) 
                        continue;
                    int i1 = this.hexToInt(ch);
                    ch = (byte)m_Stream.ReadByte();
                    int i2 = (ch == '>' ? 0 : this.hexToInt(ch));
                    if ((i1 < 0) || (i2 < 0)) 
                        val.Add(0);
                    else 
                        val.Add((byte)(((i1 << 4) + i2)));
                    if (ch == '>') 
                        break;
                }
                return val.ToArray();
            }
            if (ch != '(') 
                return null;
            int lev = 0;
            int IsUnicode = 0;
            while ((((i = m_Stream.ReadByte()))) >= 0) 
            {
                ch = (byte)i;
                if ((val.Count < 2) && ((ch == 0xFE || ch == 0xFF))) 
                    IsUnicode++;
                if (IsUnicode > 1) 
                {
                    if (ch == ')' && (((val.Count % 2)) == 0)) 
                        break;
                    val.Add(ch);
                    continue;
                }
                val.Add(ch);
                if (ch == '(') 
                {
                    lev++;
                    continue;
                }
                if (ch == ')') 
                {
                    if ((--lev) < 0) 
                    {
                        val.RemoveAt(val.Count - 1);
                        break;
                    }
                    continue;
                }
                if (ch != '\\') 
                    continue;
                val.RemoveAt(val.Count - 1);
                ch = (byte)m_Stream.ReadByte();
                if (ch >= '0' && ch <= '7') 
                {
                    int res = (int)((ch - '0'));
                    for (int k = 0; k < 2; k++) 
                    {
                        ch = (byte)m_Stream.ReadByte();
                        if (ch >= '0' && ch <= '7') 
                            res = (res * 8) + ((int)((ch - '0')));
                        else 
                        {
                            m_Stream.Position -= 1;
                            break;
                        }
                    }
                    val.Add((byte)res);
                    continue;
                }
                if (ch == 0xD || ch == 0xA) 
                    continue;
                if (ch == 'r') 
                    ch = 0xD;
                else if (ch == 'n') 
                    ch = 0xA;
                else if (ch == 't') 
                    ch = 9;
                else if (ch == 'b') 
                    ch = 8;
                else if (ch == 'f') 
                    ch = 12;
                else if (ch == '\\' || ch == '(' || ch == ')') 
                {
                }
                else 
                {
                }
                val.Add(ch);
            }
            return val.ToArray();
        }
        public byte[] ReadName()
        {
            int i = m_Stream.ReadByte();
            if (i != '/') 
                return null;
            List<byte> res = new List<byte>();
            while ((((i = m_Stream.ReadByte()))) >= 0) 
            {
                byte ch = (byte)i;
                if (this.isSpace(ch)) 
                    break;
                if (this.isDelimiter(ch)) 
                {
                    m_Stream.Position -= 1;
                    break;
                }
                if (ch != '#') 
                {
                    res.Add(ch);
                    continue;
                }
                int i1 = this.hexToInt((byte)m_Stream.ReadByte());
                int i2 = this.hexToInt((byte)m_Stream.ReadByte());
                if ((i1 < 0) || (i2 < 0)) 
                    res.Add(0);
                else 
                    res.Add((byte)(((i1 << 4) + i2)));
            }
            if (res.Count == 0) 
                return null;
            return res.ToArray();
        }
        public string ReadWord(bool notNull = false)
        {
            int i;
            StringBuilder res = new StringBuilder();
            while ((((i = m_Stream.ReadByte()))) >= 0) 
            {
                byte ch = (byte)i;
                if (this.isSpace(ch)) 
                {
                    if (res.Length > 0) 
                        break;
                }
                else if (this.isDelimiter(ch)) 
                {
                    if (res.Length == 0 && notNull) 
                    {
                        res.Append((char)ch);
                        break;
                    }
                    m_Stream.Position -= 1;
                    break;
                }
                else 
                    res.Append((char)ch);
            }
            if (res.Length == 0) 
                return null;
            return res.ToString();
        }
        public bool TryReadIdAndVersion(bool mustBeRef, out int id, out ushort vers)
        {
            id = 0;
            vers = 0;
            int p0 = (int)m_Stream.Position;
            if (!int.TryParse(this.ReadWord(false), out id)) 
            {
                m_Stream.Position = p0;
                return false;
            }
            if (!ushort.TryParse(this.ReadWord(false), out vers)) 
            {
                m_Stream.Position = p0;
                return false;
            }
            string str = this.ReadWord(false);
            if (mustBeRef) 
            {
                if (str == "R") 
                    return true;
            }
            else if (str == "obj") 
                return true;
            m_Stream.Position = p0;
            return false;
        }
        public PdfObject ParseObject(PdfFile file, bool text)
        {
            PdfObject res = null;
            int i = this.PeekSolidByte();
            if (i < 0) 
                return res;
            byte ch = (byte)i;
            int p0 = (int)m_Stream.Position;
            if (ch >= '0' && ch <= '9' && !text) 
            {
                int id;
                ushort ver;
                if (this.TryReadIdAndVersion(true, out id, out ver)) 
                {
                    res = new PdfReference();
                    res.SourceFile = file;
                    res.Id = id;
                    res.Version = ver;
                    return res;
                }
            }
            if ((((ch >= '0' && ch <= '9')) || ch == '+' || ch == '-') || ch == '.') 
            {
                string str = this.ReadWord(false);
                if (str == null) 
                    return null;
                if (str.IndexOf('.') < 0) 
                {
                    int v;
                    if (!int.TryParse(str, out v)) 
                        return null;
                    PdfIntValue ires = new PdfIntValue();
                    ires.Val = v;
                    return ires;
                }
                else 
                {
                    PdfRealValue fres = new PdfRealValue();
                    fres.Val = str;
                    return fres;
                }
            }
            if (ch == '/') 
            {
                PdfName nres = new PdfName();
                nres.Name = PdfStringValue.GetStringByBytes(this.ReadName());
                if (nres.Name == null) 
                    return null;
                return nres;
            }
            else if (ch == '(') 
            {
                PdfStringValue sres = new PdfStringValue();
                sres.Val = this.ReadLexemString();
                if (sres.Val == null) 
                    return null;
                return sres;
            }
            else if (ch == '[') 
                res = new PdfArray();
            else if (ch == '<') 
            {
                m_Stream.Position = p0 + 1;
                ch = (byte)m_Stream.ReadByte();
                if (ch == '<') 
                    res = new PdfDictionary() { m_FilePos = p0 };
                else 
                {
                    PdfStringValue sres = new PdfStringValue();
                    m_Stream.Position = p0;
                    sres.Val = this.ReadLexemString();
                    if (sres.Val == null) 
                        return null;
                    return sres;
                }
            }
            else if (((ch == '\'' || ch == '\"')) && text) 
            {
                PdfName nres = new PdfName();
                nres.Name = (ch == '\'' ? "\'" : "\"");
                m_Stream.Position++;
                return nres;
            }
            else 
            {
                string str = this.ReadWord(false);
                switch (str) { 
                case "null":
                    return new PdfNull();
                case "true":
                    PdfBoolValue bres1 = new PdfBoolValue();
                    bres1.Val = true;
                    return bres1;
                case "false":
                    PdfBoolValue bres2 = new PdfBoolValue();
                    bres2.Val = false;
                    return bres2;
                case "endobj":
                    return null;
                }
                if (text && str != null) 
                {
                    PdfName nres = new PdfName();
                    nres.Name = str;
                    return nres;
                }
                return null;
            }
            res.SourceFile = file;
            if (res is PdfDictionary) 
                (res as PdfDictionary).PostParse(this);
            else if (res is PdfArray) 
                (res as PdfArray).PostParse(this);
            return res;
        }
        public List<PdfObject> ParseContent()
        {
            List<PdfObject> res = new List<PdfObject>();
            while (true) 
            {
                int i = this.PeekSolidByte();
                if (i < 0) 
                    break;
                try 
                {
                    PdfObject obj = this.ParseObject(null, true);
                    if (obj != null) 
                        res.Add(obj);
                    else 
                        this.ReadByte();
                }
                catch(Exception ex122) 
                {
                    break;
                }
            }
            return res;
        }
    }
}