/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;
using System.Text;

namespace Pullenti.Unitext.Internal.Word
{
    class MSOfficeHelper
    {
        public static bool CheckExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToUpper();
            return (ext == ".DOC" || ext == ".DOCX" || ext == ".PPTX") || ext == ".XLSX";
        }
        static Pullenti.Unitext.UnitextDocument _uniFromWord97(CompoundFileSystem cf)
        {
            WordDocument word = new WordDocument();
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Doc };
            word.Load(cf, doc);
            return doc;
        }
        internal static Pullenti.Unitext.UnitextDocument _uniFromWord97Arr(byte[] data)
        {
            if (data.Length > 0x300 && data[0] == 0xD0 && data[1] == 0xCF) 
            {
                int i0 = 0x50;
                for (; i0 < data.Length; i0++) 
                {
                    if (data[i0] != 0xFF) 
                        break;
                }
                if ((i0 + 0x200) <= data.Length && data[i0 + 1] == 0xA5) 
                {
                    byte[] buf = new byte[(int)(data.Length - i0)];
                    for (int i = 0; i < buf.Length; i++) 
                    {
                        buf[i] = data[i0 + i];
                    }
                    Pullenti.Unitext.UnitextDocument doc = _uniFromWord6orEarly(buf);
                    if (doc != null) 
                    {
                        doc.SourceFormat = Pullenti.Unitext.FileFormat.Doc;
                        doc.Attrs.Add("word", "6");
                    }
                    return doc;
                }
            }
            using (CompoundFileSystem cf = new CompoundFileSystem(null, data)) 
            {
                return _uniFromWord97(cf);
            }
        }
        internal static Pullenti.Unitext.UnitextDocument _uniFromWord6orEarly(byte[] buf)
        {
            int pos0 = BitConverter.ToInt32(buf, 0x18);
            int pos1 = BitConverter.ToInt32(buf, 0x1C);
            if (pos1 <= buf.Length && pos0 >= 0x20 && pos1 > pos0) 
            {
            }
            else 
                return null;
            int v0 = 0;
            int v1 = 0;
            for (int i = pos0; i < pos1; i++) 
            {
                if (buf[i] == 0 || buf[i] == 4) 
                {
                    if (((((i - pos0)) & 1)) == 0) 
                        v1++;
                    else 
                        v0++;
                }
            }
            string str = null;
            if (v0 > (v1 * 3)) 
                str = Pullenti.Util.MiscHelper.DecodeStringUnicode(buf, pos0, pos1 - pos0);
            else 
                str = Pullenti.Util.MiscHelper.DecodeString1251(buf, pos0, pos1 - pos0);
            Pullenti.Unitext.UnitextDocument ddd = Pullenti.Unitext.UnitextService.CreateDocumentFromText(str);
            return ddd;
        }
        internal static Pullenti.Unitext.UnitextDocument _uniFromWord97File(string fileName)
        {
            using (CompoundFileSystem cf = new CompoundFileSystem(fileName, null)) 
            {
                return _uniFromWord97(cf);
            }
        }
        static string _correctContent(StringBuilder txt)
        {
            StringBuilder res = new StringBuilder(txt.Length);
            int off = 0;
            int lf = 0;
            StringBuilder offTmp = new StringBuilder();
            for (int j = 0; j < txt.Length; j++) 
            {
                char ch = txt[j];
                if (ch == ':') 
                {
                }
                if (ch == 127) 
                    ch = ' ';
                if (off > 0) 
                {
                    if (ch == 0x15) 
                    {
                        if ((--off) == 0) 
                        {
                            string ttt = extractSpecText(offTmp.ToString().Trim());
                            if (ttt != null) 
                                res.Append(ttt);
                            offTmp.Length = 0;
                        }
                        else 
                            offTmp.Append(ch);
                    }
                    else 
                    {
                        offTmp.Append(ch);
                        if (ch == 0x13) 
                            off++;
                    }
                    continue;
                }
                if (ch == 0x13) 
                {
                    if (off == 0) 
                        offTmp.Length = 0;
                    off++;
                }
                else if (ch == 0xD || ch == '\v') 
                {
                    res.Append("\r\n");
                    lf++;
                }
                else if (ch == 0xC) 
                {
                    res.Append("\r\n\r\n\r\n");
                    lf += 3;
                }
                else if (char.IsWhiteSpace(ch) || ((int)ch) > 0x20 || ch == 7) 
                {
                    if (ch == '\n') 
                    {
                        if (res.Length == 0 || res[res.Length - 1] != '\r') 
                            res.Append('\r');
                    }
                    res.Append(ch);
                    if (!char.IsWhiteSpace(ch)) 
                        lf = 0;
                }
                else if (ch == 3 || ch == 4) 
                    break;
                else 
                {
                }
            }
            return res.ToString();
        }
        internal static string extractSpecText(string txt)
        {
            txt = txt.TrimStart();
            if (txt.StartsWith("HYPERLINK")) 
            {
                int i1 = txt.IndexOf("PAGEREF");
                if (i1 > 0) 
                {
                    for (int ii = i1 - 1; ii >= 0; ii--) 
                    {
                        if (txt[ii] == 0x14 || txt[ii] == 0x1) 
                        {
                            txt = txt.Substring(ii + 1, i1 - ii - 1).Trim();
                            return txt + "\n";
                        }
                    }
                }
                int i = txt.LastIndexOf((char)0x14);
                if (i < 0) 
                    i = txt.LastIndexOf((char)0x1);
                if (i > 0 && (i < txt.Length)) 
                {
                    txt = txt.Substring(i + 1);
                    return txt;
                }
                return null;
            }
            if (txt.StartsWith("TOC")) 
            {
                int ii = txt.IndexOf("HYPERLINK");
                if (ii > 0) 
                    return extractSpecText(txt.Substring(ii));
                StringBuilder res = new StringBuilder();
                bool treg = false;
                foreach (char ch in txt) 
                {
                    if (ch == 0x14 || ch == 0x15) 
                        treg = true;
                    else if (ch == 0x13) 
                        treg = false;
                    else if (treg) 
                    {
                        if (ch == 0xD) 
                            res.Append("\r\n");
                        else if (ch == '\v' || ch == 7) 
                            res.Append("  ");
                        else 
                            res.Append(ch);
                    }
                }
                return res.ToString();
            }
            return null;
        }
    }
}