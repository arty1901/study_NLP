/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfFont
    {
        public string Alias;
        public override string ToString()
        {
            return string.Format("Font {0}", Alias ?? "?");
        }
        public PdfFont(PdfDictionary dic)
        {
            string subtyp = dic.GetStringItem("Subtype");
            m_CodeSize = 1;
            if (subtyp == "Type0") 
                m_CodeSize = 2;
            string enc = dic.GetStringItem("Encoding");
            PdfDictionary fontDescr = dic.GetDictionary("FontDescriptor", null);
            PdfDictionary desc = dic.GetDictionary("DescendantFonts", null);
            if (desc != null && desc.IsTypeItem("Font")) 
            {
                if (fontDescr == null) 
                    fontDescr = desc.GetDictionary("FontDescriptor", null);
                PdfArray wlist = desc.GetObject("W", false) as PdfArray;
                if (wlist != null) 
                {
                    for (int i = 0; i < (wlist.ItemsCount - 1); i++) 
                    {
                        PdfObject it0 = wlist.GetItem(i);
                        PdfObject it1 = wlist.GetItem(i + 1);
                        if (it0 == null || it1 == null) 
                            break;
                        int cod0 = (int)it0.GetDouble();
                        if (it1 is PdfArray) 
                        {
                            PdfArray aaa = it1 as PdfArray;
                            for (int j = 0; j < aaa.ItemsCount; j++) 
                            {
                                PdfObject it2 = aaa.GetItem(j);
                                if (it2 == null) 
                                    break;
                                if (!m_Glypths.ContainsKey(cod0 + j)) 
                                    m_Glypths.Add(cod0 + j, new PdfFontGlyph() { Width = it2.GetDouble(), Code = cod0 + j });
                                else 
                                    m_Glypths[cod0 + j].Width = it2.GetDouble();
                                if (m_CodeSize == 1 && (cod0 + j) >= 256) 
                                    m_CodeSize = 2;
                            }
                            i += 1;
                            continue;
                        }
                        PdfObject it3 = wlist.GetItem(i + 2);
                        if (!(it1 is PdfIntValue) || it3 == null) 
                            break;
                        int cod1 = (int)it1.GetDouble();
                        if (cod1 < cod0) 
                            break;
                        if (m_CodeSize == 1 && cod1 >= 256) 
                            m_CodeSize = 2;
                        double w = it3.GetDouble();
                        for (int j = cod0; j <= cod1; j++) 
                        {
                            if (!m_Glypths.ContainsKey(j)) 
                                m_Glypths.Add(j, new PdfFontGlyph() { Width = w, Code = j });
                            else 
                                m_Glypths[j].Width = w;
                        }
                        i += 2;
                    }
                }
            }
            PdfArray warr = dic.GetObject("Widths", false) as PdfArray;
            if (warr != null) 
            {
                PdfIntValue fiChar = dic.GetObject("FirstChar", false) as PdfIntValue;
                PdfIntValue laChar = dic.GetObject("LastChar", false) as PdfIntValue;
                if (fiChar != null && laChar != null) 
                {
                    int cod0 = (int)fiChar.GetDouble();
                    int cod1 = (int)laChar.GetDouble();
                    if (cod1 >= 256 && (m_CodeSize < 2)) 
                        m_CodeSize = 2;
                    for (int i = 0; i < warr.ItemsCount; i++) 
                    {
                        PdfObject it = warr.GetItem(i);
                        if (it == null) 
                            continue;
                        double w = it.GetDouble();
                        if (!m_Glypths.ContainsKey(cod0 + i)) 
                            m_Glypths.Add(cod0 + i, new PdfFontGlyph() { Width = w, Code = cod0 + i });
                        else 
                            m_Glypths[cod0 + i].Width = w;
                    }
                }
            }
            PdfDictionary uniDic = dic.GetDictionary("ToUnicode", null);
            PdfArray diff = null;
            PdfDictionary denc = dic.GetDictionary("Encoding", null);
            if (denc != null) 
                diff = denc.GetObject("Differences", false) as PdfArray;
            if (uniDic != null) 
                this._loadToUnicode(uniDic);
            else 
            {
                string iii = dic.GetStringItem("ToUnicode");
                if (iii == "Identity-H") 
                    m_IsIdentityH = true;
                bool canBeWindows1251 = subtyp == "Type3";
                string encStr = dic.GetStringItem("Encoding");
                if (encStr == "WinAnsiEncoding") 
                    canBeWindows1251 = true;
                bool isMac = encStr == "MacRomanEncoding";
                if (((encStr == "Identity-H" || encStr == "Identity-V")) && fontDescr != null && fontDescr.GetDictionary("FontFile2", null) != null) 
                    this._loadCMapFromTrueType(fontDescr.GetDictionary("FontFile2", null));
                else 
                    foreach (KeyValuePair<int, PdfFontGlyph> kp in m_Glypths) 
                    {
                        kp.Value.Char = (char)kp.Value.Code;
                        if (isMac) 
                        {
                            if (kp.Value.Code >= 0x80 && (kp.Value.Code < 0x100)) 
                                kp.Value.Char = (char)m_MacRomanToUnicode[kp.Value.Code - 0x80];
                        }
                        else if (canBeWindows1251) 
                        {
                            if (kp.Key > 0 && (kp.Key < m_1251_utf.Length)) 
                                kp.Value.Char = (char)m_1251_utf[kp.Key];
                        }
                    }
            }
            if (diff != null) 
                this._loadDiff(diff);
            if (fontDescr != null) 
            {
                PdfObject it = fontDescr.GetObject("Ascent", false);
                if (it != null) 
                    m_Ascent = it.GetDouble();
                it = fontDescr.GetObject("Descent", false);
                if (it != null) 
                {
                    double d = it.GetDouble();
                    if (m_Ascent > d) 
                        m_LineHeight = m_Ascent - d;
                }
            }
            if (m_LineHeight == 0) 
            {
                PdfArray bbox = dic.GetObject("FontBBox", false) as PdfArray;
                if (bbox != null && bbox.ItemsCount == 4) 
                {
                    PdfObject it1 = bbox.GetItem(1);
                    PdfObject it3 = bbox.GetItem(3);
                    if (it1 != null && it3 != null) 
                    {
                        m_LineHeight = it3.GetDouble() - it1.GetDouble();
                        if (m_LineHeight < 0) 
                            m_LineHeight = 0;
                    }
                }
            }
            if (subtyp == "Type3") 
            {
                m_IsType3 = true;
                PdfArray bbox = dic.GetObject("FontMatrix", false) as PdfArray;
                if (bbox != null && bbox.ItemsCount == 6) 
                {
                    PdfObject it0 = bbox.GetItem(0);
                    PdfObject it3 = bbox.GetItem(3);
                    if (it0 != null && it3 != null) 
                    {
                        m_MultX = it0.GetDouble();
                        m_MultY = it3.GetDouble();
                    }
                }
            }
            int cou = 0;
            m_AverCharWidth = 0;
            foreach (KeyValuePair<int, PdfFontGlyph> kp in m_Glypths) 
            {
                if (char.IsLetterOrDigit(kp.Value.Char) && kp.Value.Width > 0) 
                {
                    cou++;
                    m_AverCharWidth += kp.Value.Width;
                }
                if (kp.Value.Char == ' ') 
                    m_SpaceWidth = kp.Value.Width;
            }
            if (cou > 0) 
                m_AverCharWidth /= cou;
            if (subtyp == "Type3") 
            {
                PdfArray arr = dic.GetObject("FontMatrix", false) as PdfArray;
                if (arr != null && arr.ItemsCount == 6) 
                {
                    m_Matrix = new Matrix();
                    m_Matrix.Set(arr.GetItem(0).GetDouble(), arr.GetItem(1).GetDouble(), arr.GetItem(2).GetDouble(), arr.GetItem(3).GetDouble(), arr.GetItem(4).GetDouble(), arr.GetItem(5).GetDouble());
                    m_MultX = m_Matrix.A;
                    m_MultY = m_Matrix.D;
                }
            }
        }
        void _loadDiff(PdfArray diff)
        {
            for (int i = 0; i < diff.ItemsCount; i++) 
            {
                PdfIntValue cod = diff.GetItem(i) as PdfIntValue;
                if (cod == null) 
                    break;
                int co = cod.Val;
                for (int j = i + 1; j < diff.ItemsCount; j++) 
                {
                    PdfName nam = diff.GetItem(j) as PdfName;
                    if (nam == null) 
                        break;
                    int uni = 0;
                    if (m_Diffs.TryGetValue(nam.Name, out uni)) 
                    {
                    }
                    else if (nam.Name.StartsWith("uni") && nam.Name.Length > 4 && ((char.IsDigit(nam.Name[3]) || char.IsUpper(nam.Name[3])))) 
                    {
                        for (int kk = 3; kk < nam.Name.Length; kk++) 
                        {
                            char chh = nam.Name[kk];
                            if (char.IsDigit(chh)) 
                                uni = (uni * 16) + ((int)((chh - '0')));
                            else if (chh >= 'A' && chh <= 'F') 
                                uni = (uni * 16) + ((int)(((chh - 'A') + 10)));
                            else if (chh >= 'a' && chh <= 'f') 
                                uni = (uni * 16) + ((int)(((chh - 'a') + 10)));
                            else 
                                break;
                        }
                    }
                    PdfFontGlyph gli;
                    if (!m_Glypths.TryGetValue(co, out gli)) 
                    {
                        gli = new PdfFontGlyph();
                        gli.Code = co;
                        m_Glypths.Add(co, gli);
                    }
                    if (uni > 0) 
                        gli.Char = (char)uni;
                    else 
                    {
                        gli.UndefMnem = nam.Name;
                        gli.Char = (char)0;
                        if (nam.Name.Length == 2) 
                        {
                            char ch0 = nam.Name[0];
                            char ch1 = nam.Name[1];
                            if (ch0 >= 'A' && ch0 <= 'H') 
                            {
                                int ii = ((int)((ch0 - 'A'))) * 36;
                                if (ch1 >= '0' && ch1 <= '9') 
                                    ii += ((int)((ch1 - '0')));
                                else if (ch1 >= 'A' && ch1 <= 'Z') 
                                    ii += (10 + ((int)((ch1 - 'A'))));
                                if (ii >= 0 && (ii < Pullenti.Util.MiscHelper.m_1251_utf.Length)) 
                                    gli.Char = (char)Pullenti.Util.MiscHelper.m_1251_utf[ii];
                            }
                        }
                    }
                    co++;
                    i = j;
                }
            }
        }
        void _loadToUnicode(PdfDictionary dic)
        {
            byte[] data = dic.ExtractData();
            if (data == null) 
                return;
            List<PdfObject> list = null;
            using (PdfStream pstr = new PdfStream(null, data)) 
            {
                list = pstr.ParseContent();
            }
            for (int i = 1; i < list.Count; i++) 
            {
                if (!(list[i - 1] is PdfIntValue) || !(list[i] is PdfName)) 
                    continue;
                string typ = (list[i] as PdfName).Name;
                if (typ != "beginbfchar" && typ != "beginbfrange") 
                    continue;
                int c;
                int Col = (int)(list[i - 1] as PdfIntValue).Val;
                i++;
                for (c = 0; ((i < list.Count)) && ((c < Col)); c++) 
                {
                    if (!(list[i] is PdfStringValue)) 
                        break;
                    byte[] val = (list[i] as PdfStringValue).Val;
                    if (val.Length < 1) 
                        break;
                    if (m_CodeSize < 1) 
                        m_CodeSize = val.Length;
                    int range0 = toInt(val, 0, -1);
                    int range1 = range0;
                    i++;
                    if (typ == "beginbfrange") 
                    {
                        if (!(list[i] is PdfStringValue)) 
                            break;
                        val = (list[i] as PdfStringValue).Val;
                        if (val.Length < 1) 
                            break;
                        range1 = toInt(val, 0, -1);
                        if (range1 < range0) 
                            break;
                        i++;
                    }
                    if (list[i] is PdfStringValue) 
                    {
                        val = (list[i] as PdfStringValue).Val;
                        if (val.Length < 1) 
                            break;
                        int uniChar = toInt(val, 0, -1);
                        for (int j = range0; j <= range1; j++,uniChar++) 
                        {
                            PdfFontGlyph gl = null;
                            if (!m_Glypths.TryGetValue(j, out gl)) 
                                m_Glypths.Add(j, (gl = new PdfFontGlyph() { Code = j }));
                            gl.Char = (char)uniChar;
                            if (((uniChar & 0xFFFF0000)) != 0) 
                            {
                                gl.Char2 = gl.Char;
                                gl.Char = (char)((((uniChar >> 16)) & 0xFFFF));
                            }
                        }
                        i++;
                        continue;
                    }
                    if (list[i] is PdfArray) 
                    {
                        PdfArray arr = list[i] as PdfArray;
                        if (arr.ItemsCount == (((range1 - range0) + 1))) 
                        {
                            for (int j = 0; j < arr.ItemsCount; j++) 
                            {
                                PdfStringValue it = arr.GetItem(j) as PdfStringValue;
                                if (it == null) 
                                    break;
                                val = it.Val;
                                if (val.Length < 1) 
                                    break;
                                int uniChar = toInt(val, 0, -1);
                                PdfFontGlyph gl = null;
                                if (!m_Glypths.TryGetValue(j + range0, out gl)) 
                                    m_Glypths.Add(j + range0, (gl = new PdfFontGlyph() { Code = j + range0 }));
                                gl.Char = (char)uniChar;
                                if (((uniChar & 0xFFFF0000)) != 0) 
                                {
                                    gl.Char2 = gl.Char;
                                    gl.Char = (char)((((uniChar >> 16)) & 0xFFFF));
                                }
                            }
                        }
                        i++;
                        continue;
                    }
                    break;
                }
            }
        }
        void _loadCMapFromTrueType(PdfDictionary ttf)
        {
            byte[] dat = ttf.ExtractData();
            if (dat == null) 
                return;
            int pos = 0;
            int i;
            int j;
            i = this._readInt(dat, 0);
            if (i != 0x10000) 
                return;
            int tabs = this._readShort(dat, 4);
            pos = 12;
            int len = 0;
            for (; tabs > 0; tabs--,pos += 16) 
            {
                if ((dat[pos] != 0x63 || dat[pos + 1] != 0x6D || dat[pos + 2] != 0x61) || dat[pos + 3] != 0x70) 
                    continue;
                len = this._readInt(dat, pos + 12);
                pos = this._readInt(dat, pos + 8);
                break;
            }
            if (len <= 0) 
                return;
            int baseOffset = pos;
            int vers = this._readShort(dat, pos);
            tabs = this._readShort(dat, pos + 2);
            pos += 4;
            for (; tabs > 0; tabs--,pos += 8) 
            {
                int platformId = this._readShort(dat, pos);
                if (platformId != 0) 
                {
                }
                int encodId = this._readShort(dat, pos + 2);
                int offset = this._readInt(dat, pos + 4);
                offset += baseOffset;
                int format = this._readShort(dat, offset);
                if (format == 0) 
                    this._readFormat0(dat, offset, platformId);
                else if (format == 4) 
                    this._readFormat4(dat, offset, platformId);
                else if (format == 6) 
                    this._readFormat6(dat, offset, platformId);
                else 
                {
                }
            }
        }
        int _readShort(byte[] dat, int pos)
        {
            uint res = (uint)dat[pos];
            res <<= 8;
            res |= dat[pos + 1];
            return (int)res;
        }
        int _readInt(byte[] dat, int pos)
        {
            uint res = (uint)dat[pos];
            res <<= 8;
            res |= dat[pos + 1];
            res <<= 8;
            res |= dat[pos + 2];
            res <<= 8;
            res |= dat[pos + 3];
            return (int)res;
        }
        void _readFormat0(byte[] dat, int pos, int platform)
        {
            int p = pos + 6;
            int lang = this._readShort(dat, pos + 4);
            for (int i = 0; i < 256; i++,p++) 
            {
                int id = (int)dat[p];
                PdfFontGlyph gli;
                if (!m_Glypths.TryGetValue(id, out gli)) 
                    m_Glypths.Add(id, (gli = new PdfFontGlyph() { Code = id }));
                if (platform == 1 && id >= 0x80) 
                    id = m_MacRomanToUnicode[id - 0x80];
                gli.Char = (char)id;
            }
        }
        void _readFormat4(byte[] dat, int pos, int platform)
        {
            if (platform != 0) 
            {
            }
            int lang = this._readShort(dat, pos + 4);
            int p = pos + 6;
            int segCount = this._readShort(dat, p);
            segCount /= 2;
            p += 8;
            int[] ecs = new int[(int)segCount];
            for (int i = 0; i < segCount; i++,p += 2) 
            {
                ecs[i] = this._readShort(dat, p);
            }
            p += 2;
            int[] scs = new int[(int)segCount];
            for (int i = 0; i < segCount; i++,p += 2) 
            {
                scs[i] = this._readShort(dat, p);
            }
            int[] idds = new int[(int)segCount];
            for (int i = 0; i < segCount; i++,p += 2) 
            {
                idds[i] = this._readShort(dat, p);
            }
            int p0 = p;
            int[] idrs = new int[(int)segCount];
            for (int i = 0; i < segCount; i++,p += 2) 
            {
                idrs[i] = this._readShort(dat, p);
            }
            for (int i = 0; i < segCount; i++) 
            {
                for (int cod = scs[i]; cod <= ecs[i]; cod++) 
                {
                    int id = cod + idds[i];
                    if (idrs[i] > 0) 
                    {
                        p = p0 + idrs[i];
                        id = this._readShort(dat, p) + idds[i];
                    }
                    id &= 0xFFFF;
                    PdfFontGlyph gli;
                    if (!m_Glypths.TryGetValue(id, out gli)) 
                        m_Glypths.Add(id, (gli = new PdfFontGlyph() { Code = id }));
                    gli.Char = (char)cod;
                }
            }
        }
        void _readFormat6(byte[] dat, int pos, int platform)
        {
            if (platform != 0) 
            {
            }
            int lang = this._readShort(dat, pos + 4);
            int p = pos + 6;
            int first = this._readShort(dat, p);
            int cou = this._readShort(dat, p + 2);
            p += 4;
            for (int i = 0; i < cou; i++,p += 2) 
            {
                int id = this._readShort(dat, p);
                PdfFontGlyph gli;
                if (!m_Glypths.TryGetValue(id, out gli)) 
                    m_Glypths.Add(id, (gli = new PdfFontGlyph() { Code = id }));
                int cod = first + i;
                if (platform == 1 && cod >= 0x80 && cod <= 0x100) 
                    cod = m_MacRomanToUnicode[cod - 0x80];
                gli.Char = (char)cod;
            }
        }
        double m_LineHeight;
        double m_AverCharWidth;
        double m_Ascent;
        int m_CodeSize;
        bool m_IsType3;
        bool m_IsIdentityH;
        double m_MultX = 0.001;
        double m_MultY = 0.001;
        double m_SpaceWidth;
        Matrix m_Matrix;
        static int toInt(byte[] val, int pos, int len)
        {
            int res = 0;
            if (len < 0) 
                len = val.Length;
            if ((pos + len) > val.Length) 
                len = val.Length - pos;
            for (int i = pos; i < (pos + len); i++) 
            {
                res <<= 8;
                res += (((int)val[i]) & 0xFF);
            }
            return res;
        }
        Dictionary<int, PdfFontGlyph> m_Glypths = new Dictionary<int, PdfFontGlyph>();
        internal PdfText ExtractText(PdfObject tobj, PdfTextState state)
        {
            PdfText res = new PdfText();
            double scale = state.WScale / 100;
            double scaledFactor = scale * state.FontSize * m_MultX;
            double wordSpace = state.WordSpace * scale;
            double charSpace = state.CharSpace * scale;
            res.SpaceWidth = state.Font.GetSpaceWidth(state.FontSize);
            res.SpaceWidth *= scale;
            StringBuilder word = new StringBuilder();
            PdfArray tarr = tobj as PdfArray;
            if (tarr == null) 
            {
                if (!(tobj is PdfStringValue)) 
                    return null;
                tarr = new PdfArray();
                tarr.Add(tobj);
            }
            Matrix trm = new Matrix();
            for (int k = 0; k < tarr.ItemsCount; k++) 
            {
                PdfObject obj = tarr.GetItem(k);
                if (!(obj is PdfStringValue)) 
                {
                    double w = obj.GetDouble();
                    w *= scaledFactor;
                    if (w < 0) 
                    {
                        if ((-w) >= res.SpaceWidth) 
                        {
                            if (word.Length > 0 && word[word.Length - 1] != ' ') 
                                word.Append(" ");
                        }
                    }
                    state.Tm.Translate(-w, 0);
                    continue;
                }
                byte[] input = (obj as PdfStringValue).Val;
                if (input == null) 
                    continue;
                for (int i = 0; i < input.Length; i += m_CodeSize) 
                {
                    int cod = toInt(input, i, m_CodeSize);
                    PdfFontGlyph gl;
                    if (!m_Glypths.TryGetValue(cod, out gl)) 
                    {
                        if (m_IsIdentityH && cod > 0) 
                            word.Append((char)cod);
                        continue;
                    }
                    if (gl.Char != 0) 
                        word.Append(gl.Char);
                    else if (gl.UndefMnem != null) 
                        word.Append("\\" + gl.UndefMnem);
                    if (gl.Char2 != 0) 
                        word.Append(gl.Char2);
                    trm.CopyFrom(state.Tm);
                    if (state.CtmStack.Count > 0) 
                        trm.Multiply(state.CtmStack[0]);
                    double charWidth = gl.Width * scaledFactor;
                    double charHeight = m_LineHeight * m_MultY * state.FontSize;
                    double x = trm.E;
                    double wi = charWidth * trm.A;
                    double y = state.BoxHeight - trm.F;
                    double hi = charHeight * trm.D;
                    if (trm.D >= 0) 
                        y -= ((m_Ascent * m_MultY * state.FontSize) * trm.D);
                    else 
                    {
                        hi = -hi;
                        if (trm.F > state.BoxHeight) 
                            y = trm.F - state.BoxHeight;
                        else 
                            y = state.BoxHeight - y;
                        y += ((m_Ascent * m_MultY * state.FontSize) * trm.D);
                    }
                    if (res.X1 <= 0) 
                    {
                        res.X1 = x;
                        res.X2 = x + wi;
                        res.Y1 = y;
                        res.Y2 = y + hi;
                    }
                    else 
                    {
                        if ((x + wi) > res.X2) 
                            res.X2 = x + wi;
                        else 
                        {
                        }
                        if (y < res.Y1) 
                            res.Y1 = y;
                        if ((y + hi) > res.Y2) 
                            res.Y2 = y + hi;
                    }
                    state.Tm.Translate((charWidth + charSpace + ((gl.Char == ' ' ? wordSpace : (double)0))), 0);
                }
            }
            res.Text = word.ToString();
            res.FontType3 = m_IsType3;
            res.FontSize = state.FontSize;
            if (word.Length == 0) 
                return null;
            return res;
        }
        internal double GetSpaceWidth(double fontSize)
        {
            if (m_SpaceWidth == 0) 
                return fontSize * 0.25;
            return m_SpaceWidth * fontSize * m_MultX;
        }
        static int[] m_1251_utf;
        static Dictionary<string, int> m_Diffs;
        static int[] m_MacRomanToUnicode;
        static PdfFont()
        {
            m_1251_utf = new int[(int)256];
            for (int i = 0; i < 0x80; i++) 
            {
                m_1251_utf[i] = i;
            }
            int[] m_1251_80_BF = new int[] {0x0402, 0x0403, 0x201A, 0x0453, 0x201E, 0x2026, 0x2020, 0x2021, 0x20AC, 0x2030, 0x0409, 0x2039, 0x040A, 0x040C, 0x040B, 0x040F, 0x0452, 0x2018, 0x2019, 0x201C, 0x201D, 0x2022, 0x2013, 0x2014, 0x0000, 0x2122, 0x0459, 0x203A, 0x045A, 0x045C, 0x045B, 0x045F, 0x00A0, 0x040E, 0x045E, 0x0408, 0x00A4, 0x0490, 0x00A6, 0x00A7, 0x0401, 0x00A9, 0x0404, 0x00AB, 0x00AC, 0x00AD, 0x00AE, 0x0407, 0x00B0, 0x00B1, 0x0406, 0x0456, 0x0491, 0x00B5, 0x00B6, 0x00B7, 0x0451, 0x2116, 0x0454, 0x00BB, 0x0458, 0x0405, 0x0455, 0x0457};
            for (int i = 0; i < 0x40; i++) 
            {
                m_1251_utf[i + 0x80] = m_1251_80_BF[i];
            }
            for (int i = 0; i < 0x20; i++) 
            {
                m_1251_utf[i + 0xC0] = ((int)'А') + i;
            }
            for (int i = 0; i < 0x20; i++) 
            {
                m_1251_utf[i + 0xE0] = ((int)'а') + i;
            }
            m_MacRomanToUnicode = new int[] {0x00C4, 0x00C5, 0x00C7, 0x00C9, 0x00D1, 0x00D6, 0x00DC, 0x00E1, 0x00E0, 0x00E2, 0x00E4, 0x00E3, 0x00E5, 0x00E7, 0x00E9, 0x00E8, 0x00EA, 0x00EB, 0x00ED, 0x00EC, 0x00EE, 0x00EF, 0x00F1, 0x00F3, 0x00F2, 0x00F4, 0x00F6, 0x00F5, 0x00FA, 0x00F9, 0x00FB, 0x00FC, 0x2020, 0x00B0, 0x00A2, 0x00A3, 0x00A7, 0x2022, 0x00B6, 0x00DF, 0x00AE, 0x00A9, 0x2122, 0x00B4, 0x00A8, 0x2260, 0x00C6, 0x00D8, 0x221E, 0x00B1, 0x2264, 0x2265, 0x00A5, 0x00B5, 0x2202, 0x2211, 0x220F, 0x03C0, 0x222B, 0x00AA, 0x00BA, 0x03A9, 0x00E6, 0x00F8, 0x00BF, 0x00A1, 0x00AC, 0x221A, 0x0192, 0x2248, 0x2206, 0x00AB, 0x00BB, 0x2026, 0x00A0, 0x00C0, 0x00C3, 0x00D5, 0x0152, 0x0153, 0x2013, 0x2014, 0x201C, 0x201D, 0x2018, 0x2019, 0x00F7, 0x25CA, 0x00FF, 0x0178, 0x2044, 0x20AC, 0x2039, 0x203A, 0xFB01, 0xFB02, 0x2021, 0x00B7, 0x201A, 0x201E, 0x2030, 0x00C2, 0x00CA, 0x00C1, 0x00CB, 0x00C8, 0x00CD, 0x00CE, 0x00CF, 0x00CC, 0x00D3, 0x00D4, 0xF8FF, 0x00D2, 0x00DA, 0x00DB, 0x00D9, 0x0131, 0x02C6, 0x02DC, 0x00AF, 0x02D8, 0x02D9, 0x02DA, 0x00B8, 0x02DD, 0x02DB, 0x02C7};
            m_Diffs = new Dictionary<string, int>();
            string data = "A;0041\nAE;00C6\nAEacute;01FC\nAEmacron;01E2\nAEsmall;F7E6\nAacute;00C1\nAacutesmall;F7E1\nAbreve;0102\nAbreveacute;1EAE\nAbrevecyrillic;04D0\nAbrevedotbelow;1EB6\nAbrevegrave;1EB0\nAbrevehookabove;1EB2\nAbrevetilde;1EB4\nAcaron;01CD\nAcircle;24B6\nAcircumflex;00C2\nAcircumflexacute;1EA4\nAcircumflexdotbelow;1EAC\nAcircumflexgrave;1EA6\nAcircumflexhookabove;1EA8\nAcircumflexsmall;F7E2\nAcircumflextilde;1EAA\nAcute;F6C9\nAcutesmall;F7B4\nAcyrillic;0410\nAdblgrave;0200\nAdieresis;00C4\nAdieresiscyrillic;04D2\nAdieresismacron;01DE\nAdieresissmall;F7E4\nAdotbelow;1EA0\nAdotmacron;01E0\nAgrave;00C0\nAgravesmall;F7E0\nAhookabove;1EA2\nAiecyrillic;04D4\nAinvertedbreve;0202\nAlpha;0391\nAlphatonos;0386\nAmacron;0100\nAmonospace;FF21\nAogonek;0104\nAring;00C5\nAringacute;01FA\nAringbelow;1E00\nAringsmall;F7E5\nAsmall;F761\nAtilde;00C3\nAtildesmall;F7E3\nAybarmenian;0531\nB;0042\nBcircle;24B7\nBdotaccent;1E02\nBdotbelow;1E04\nBecyrillic;0411\nBenarmenian;0532\nBeta;0392\nBhook;0181\nBlinebelow;1E06\nBmonospace;FF22\nBrevesmall;F6F4\nBsmall;F762\nBtopbar;0182\nC;0043\nCaarmenian;053E\nCacute;0106\nCaron;F6CA\nCaronsmall;F6F5\nCcaron;010C\nCcedilla;00C7\nCcedillaacute;1E08\nCcedillasmall;F7E7\nCcircle;24B8\nCcircumflex;0108\nCdot;010A\nCdotaccent;010A\nCedillasmall;F7B8\nChaarmenian;0549\nCheabkhasiancyrillic;04BC\nChecyrillic;0427\nChedescenderabkhasiancyrillic;04BE\nChedescendercyrillic;04B6\nChedieresiscyrillic;04F4\nCheharmenian;0543\nChekhakassiancyrillic;04CB\nCheverticalstrokecyrillic;04B8\nChi;03A7\nChook;0187\nCircumflexsmall;F6F6\nCmonospace;FF23\nCoarmenian;0551\nCsmall;F763\nD;0044\nDZ;01F1\nDZcaron;01C4\nDaarmenian;0534\nDafrican;0189\nDcaron;010E\nDcedilla;1E10\nDcircle;24B9\nDcircumflexbelow;1E12\nDcroat;0110\nDdotaccent;1E0A\nDdotbelow;1E0C\nDecyrillic;0414\nDeicoptic;03EE\nDelta;2206\nDeltagreek;0394\nDhook;018A\nDieresis;F6CB\nDieresisAcute;F6CC\nDieresisGrave;F6CD\nDieresissmall;F7A8\nDigammagreek;03DC\nDjecyrillic;0402\nDlinebelow;1E0E\nDmonospace;FF24\nDotaccentsmall;F6F7\nDslash;0110\nDsmall;F764\nDtopbar;018B\nDz;01F2\nDzcaron;01C5\nDzeabkhasiancyrillic;04E0\nDzecyrillic;0405\nDzhecyrillic;040F\nE;0045\nEacute;00C9\nEacutesmall;F7E9\nEbreve;0114\nEcaron;011A\nEcedillabreve;1E1C\nEcharmenian;0535\nEcircle;24BA\nEcircumflex;00CA\nEcircumflexacute;1EBE\nEcircumflexbelow;1E18\nEcircumflexdotbelow;1EC6\nEcircumflexgrave;1EC0\nEcircumflexhookabove;1EC2\nEcircumflexsmall;F7EA\nEcircumflextilde;1EC4\nEcyrillic;0404\nEdblgrave;0204\nEdieresis;00CB\nEdieresissmall;F7EB\nEdot;0116\nEdotaccent;0116\nEdotbelow;1EB8\nEfcyrillic;0424\nEgrave;00C8\nEgravesmall;F7E8\nEharmenian;0537\nEhookabove;1EBA\nEightroman;2167\nEinvertedbreve;0206\nEiotifiedcyrillic;0464\nElcyrillic;041B\nElevenroman;216A\nEmacron;0112\nEmacronacute;1E16\nEmacrongrave;1E14\nEmcyrillic;041C\nEmonospace;FF25\nEncyrillic;041D\nEndescendercyrillic;04A2\nEng;014A\nEnghecyrillic;04A4\nEnhookcyrillic;04C7\nEogonek;0118\nEopen;0190\nEpsilon;0395\nEpsilontonos;0388\nErcyrillic;0420\nEreversed;018E\nEreversedcyrillic;042D\nEscyrillic;0421\nEsdescendercyrillic;04AA\nEsh;01A9\nEsmall;F765\nEta;0397\nEtarmenian;0538\nEtatonos;0389\nEth;00D0\nEthsmall;F7F0\nEtilde;1EBC\nEtildebelow;1E1A\nEuro;20AC\nEzh;01B7\nEzhcaron;01EE\nEzhreversed;01B8\nF;0046\nFcircle;24BB\nFdotaccent;1E1E\nFeharmenian;0556\nFeicoptic;03E4\nFhook;0191\nFitacyrillic;0472\nFiveroman;2164\nFmonospace;FF26\nFourroman;2163\nFsmall;F766\nG;0047\nGBsquare;3387\nGacute;01F4\nGamma;0393\nGammaafrican;0194\nGangiacoptic;03EA\nGbreve;011E\nGcaron;01E6\nGcedilla;0122\nGcircle;24BC\nGcircumflex;011C\nGcommaaccent;0122\nGdot;0120\nGdotaccent;0120\nGecyrillic;0413\nGhadarmenian;0542\nGhemiddlehookcyrillic;0494\nGhestrokecyrillic;0492\nGheupturncyrillic;0490\nGhook;0193\nGimarmenian;0533\nGjecyrillic;0403\nGmacron;1E20\nGmonospace;FF27\nGrave;F6CE\nGravesmall;F760\nGsmall;F767\nGsmallhook;029B\nGstroke;01E4\nH;0048\nH18533;25CF\nH18543;25AA\nH18551;25AB\nH22073;25A1\nHPsquare;33CB\nHaabkhasiancyrillic;04A8\nHadescendercyrillic;04B2\nHardsigncyrillic;042A\nHbar;0126\nHbrevebelow;1E2A\nHcedilla;1E28\nHcircle;24BD\nHcircumflex;0124\nHdieresis;1E26\nHdotaccent;1E22\nHdotbelow;1E24\nHmonospace;FF28\nHoarmenian;0540\nHoricoptic;03E8\nHsmall;F768\nHungarumlaut;F6CF\nHungarumlautsmall;F6F8\nHzsquare;3390\nI;0049\nIAcyrillic;042F\nIJ;0132\nIUcyrillic;042E\nIacute;00CD\nIacutesmall;F7ED\nIbreve;012C\nIcaron;01CF\nIcircle;24BE\nIcircumflex;00CE\nIcircumflexsmall;F7EE\nIcyrillic;0406\nIdblgrave;0208\nIdieresis;00CF\nIdieresisacute;1E2E\nIdieresiscyrillic;04E4\nIdieresissmall;F7EF\nIdot;0130\nIdotaccent;0130\nIdotbelow;1ECA\nIebrevecyrillic;04D6\nIecyrillic;0415\nIfraktur;2111\nIgrave;00CC\nIgravesmall;F7EC\nIhookabove;1EC8\nIicyrillic;0418\nIinvertedbreve;020A\nIishortcyrillic;0419\nImacron;012A\nImacroncyrillic;04E2\nImonospace;FF29\nIniarmenian;053B\nIocyrillic;0401\nIogonek;012E\nIota;0399\nIotaafrican;0196\nIotadieresis;03AA\nIotatonos;038A\nIsmall;F769\nIstroke;0197\nItilde;0128\nItildebelow;1E2C\nIzhitsacyrillic;0474\nIzhitsadblgravecyrillic;0476\nJ;004A\nJaarmenian;0541\nJcircle;24BF\nJcircumflex;0134\nJecyrillic;0408\nJheharmenian;054B\nJmonospace;FF2A\nJsmall;F76A\nK;004B\nKBsquare;3385\nKKsquare;33CD\nKabashkircyrillic;04A0\nKacute;1E30\nKacyrillic;041A\nKadescendercyrillic;049A\nKahookcyrillic;04C3\nKappa;039A\nKastrokecyrillic;049E\nKaverticalstrokecyrillic;049C\nKcaron;01E8\nKcedilla;0136\nKcircle;24C0\nKcommaaccent;0136\nKdotbelow;1E32\nKeharmenian;0554\nKenarmenian;053F\nKhacyrillic;0425\nKheicoptic;03E6\nKhook;0198\nKjecyrillic;040C\nKlinebelow;1E34\nKmonospace;FF2B\nKoppacyrillic;0480\nKoppagreek;03DE\nKsicyrillic;046E\nKsmall;F76B\nL;004C\nLJ;01C7\nLL;F6BF\nLacute;0139\nLambda;039B\nLcaron;013D\nLcedilla;013B\nLcircle;24C1\nLcircumflexbelow;1E3C\nLcommaaccent;013B\nLdot;013F\nLdotaccent;013F\nLdotbelow;1E36\nLdotbelowmacron;1E38\nLiwnarmenian;053C\nLj;01C8\nLjecyrillic;0409\nLlinebelow;1E3A\nLmonospace;FF2C\nLslash;0141\nLslashsmall;F6F9\nLsmall;F76C\nM;004D\nMBsquare;3386\nMacron;F6D0\nMacronsmall;F7AF\nMacute;1E3E\nMcircle;24C2\nMdotaccent;1E40\nMdotbelow;1E42\nMenarmenian;0544\nMmonospace;FF2D\nMsmall;F76D\nMturned;019C\nMu;039C\nN;004E\nNJ;01CA\nNacute;0143\nNcaron;0147\nNcedilla;0145\nNcircle;24C3\nNcircumflexbelow;1E4A\nNcommaaccent;0145\nNdotaccent;1E44\nNdotbelow;1E46\nNhookleft;019D\nNineroman;2168\nNj;01CB\nNjecyrillic;040A\nNlinebelow;1E48\nNmonospace;FF2E\nNowarmenian;0546\nNsmall;F76E\nNtilde;00D1\nNtildesmall;F7F1\nNu;039D\nO;004F\nOE;0152\nOEsmall;F6FA\nOacute;00D3\nOacutesmall;F7F3\nObarredcyrillic;04E8\nObarreddieresiscyrillic;04EA\nObreve;014E\nOcaron;01D1\nOcenteredtilde;019F\nOcircle;24C4\nOcircumflex;00D4\nOcircumflexacute;1ED0\nOcircumflexdotbelow;1ED8\nOcircumflexgrave;1ED2\nOcircumflexhookabove;1ED4\nOcircumflexsmall;F7F4\nOcircumflextilde;1ED6\nOcyrillic;041E\nOdblacute;0150\nOdblgrave;020C\nOdieresis;00D6\nOdieresiscyrillic;04E6\nOdieresissmall;F7F6\nOdotbelow;1ECC\nOgoneksmall;F6FB\nOgrave;00D2\nOgravesmall;F7F2\nOharmenian;0555\nOhm;2126\nOhookabove;1ECE\nOhorn;01A0\nOhornacute;1EDA\nOhorndotbelow;1EE2\nOhorngrave;1EDC\nOhornhookabove;1EDE\nOhorntilde;1EE0\nOhungarumlaut;0150\nOi;01A2\nOinvertedbreve;020E\nOmacron;014C\nOmacronacute;1E52\nOmacrongrave;1E50\nOmega;2126\nOmegacyrillic;0460\nOmegagreek;03A9\nOmegaroundcyrillic;047A\nOmegatitlocyrillic;047C\nOmegatonos;038F\nOmicron;039F\nOmicrontonos;038C\nOmonospace;FF2F\nOneroman;2160\nOogonek;01EA\nOogonekmacron;01EC\nOopen;0186\nOslash;00D8\nOslashacute;01FE\nOslashsmall;F7F8\nOsmall;F76F\nOstrokeacute;01FE\nOtcyrillic;047E\nOtilde;00D5\nOtildeacute;1E4C\nOtildedieresis;1E4E\nOtildesmall;F7F5\nP;0050\nPacute;1E54\nPcircle;24C5\nPdotaccent;1E56\nPecyrillic;041F\nPeharmenian;054A\nPemiddlehookcyrillic;04A6\nPhi;03A6\nPhook;01A4\nPi;03A0\nPiwrarmenian;0553\nPmonospace;FF30\nPsi;03A8\nPsicyrillic;0470\nPsmall;F770\nQ;0051\nQcircle;24C6\nQmonospace;FF31\nQsmall;F771\nR;0052\nRaarmenian;054C\nRacute;0154\nRcaron;0158\nRcedilla;0156\nRcircle;24C7\nRcommaaccent;0156\nRdblgrave;0210\nRdotaccent;1E58\nRdotbelow;1E5A\nRdotbelowmacron;1E5C\nReharmenian;0550\nRfraktur;211C\nRho;03A1\nRingsmall;F6FC\nRinvertedbreve;0212\nRlinebelow;1E5E\nRmonospace;FF32\nRsmall;F772\nRsmallinverted;0281\nRsmallinvertedsuperior;02B6\nS;0053\nSF010000;250C\nSF020000;2514\nSF030000;2510\nSF040000;2518\nSF050000;253C\nSF060000;252C\nSF070000;2534\nSF080000;251C\nSF090000;2524\nSF100000;2500\nSF110000;2502\nSF190000;2561\nSF200000;2562\nSF210000;2556\nSF220000;2555\nSF230000;2563\nSF240000;2551\nSF250000;2557\nSF260000;255D\nSF270000;255C\nSF280000;255B\nSF360000;255E\nSF370000;255F\nSF380000;255A\nSF390000;2554\nSF400000;2569\nSF410000;2566\nSF420000;2560\nSF430000;2550\nSF440000;256C\nSF450000;2567\nSF460000;2568\nSF470000;2564\nSF480000;2565\nSF490000;2559\nSF500000;2558\nSF510000;2552\nSF520000;2553\nSF530000;256B\nSF540000;256A\nSacute;015A\nSacutedotaccent;1E64\nSampigreek;03E0\nScaron;0160\nScarondotaccent;1E66\nScaronsmall;F6FD\nScedilla;015E\nSchwa;018F\nSchwacyrillic;04D8\nSchwadieresiscyrillic;04DA\nScircle;24C8\nScircumflex;015C\nScommaaccent;0218\nSdotaccent;1E60\nSdotbelow;1E62\nSdotbelowdotaccent;1E68\nSeharmenian;054D\nSevenroman;2166\nShaarmenian;0547\nShacyrillic;0428\nShchacyrillic;0429\nSheicoptic;03E2\nShhacyrillic;04BA\nShimacoptic;03EC\nSigma;03A3\nSixroman;2165\nSmonospace;FF33\nSoftsigncyrillic;042C\nSsmall;F773\nStigmagreek;03DA\nT;0054\nTau;03A4\nTbar;0166\nTcaron;0164\nTcedilla;0162\nTcircle;24C9\nTcircumflexbelow;1E70\nTcommaaccent;0162\nTdotaccent;1E6A\nTdotbelow;1E6C\nTecyrillic;0422\nTedescendercyrillic;04AC\nTenroman;2169\nTetsecyrillic;04B4\nTheta;0398\nThook;01AC\nThorn;00DE\nThornsmall;F7FE\nThreeroman;2162\nTildesmall;F6FE\nTiwnarmenian;054F\nTlinebelow;1E6E\nTmonospace;FF34\nToarmenian;0539\nTonefive;01BC\nTonesix;0184\nTonetwo;01A7\nTretroflexhook;01AE\nTsecyrillic;0426\nTshecyrillic;040B\nTsmall;F774\nTwelveroman;216B\nTworoman;2161\nU;0055\nUacute;00DA\nUacutesmall;F7FA\nUbreve;016C\nUcaron;01D3\nUcircle;24CA\nUcircumflex;00DB\nUcircumflexbelow;1E76\nUcircumflexsmall;F7FB\nUcyrillic;0423\nUdblacute;0170\nUdblgrave;0214\nUdieresis;00DC\nUdieresisacute;01D7\nUdieresisbelow;1E72\nUdieresiscaron;01D9\nUdieresiscyrillic;04F0\nUdieresisgrave;01DB\nUdieresismacron;01D5\nUdieresissmall;F7FC\nUdotbelow;1EE4\nUgrave;00D9\nUgravesmall;F7F9\nUhookabove;1EE6\nUhorn;01AF\nUhornacute;1EE8\nUhorndotbelow;1EF0\nUhorngrave;1EEA\nUhornhookabove;1EEC\nUhorntilde;1EEE\nUhungarumlaut;0170\nUhungarumlautcyrillic;04F2\nUinvertedbreve;0216\nUkcyrillic;0478\nUmacron;016A\nUmacroncyrillic;04EE\nUmacrondieresis;1E7A\nUmonospace;FF35\nUogonek;0172\nUpsilon;03A5\nUpsilon1;03D2\nUpsilonacutehooksymbolgreek;03D3\nUpsilonafrican;01B1\nUpsilondieresis;03AB\nUpsilondieresishooksymbolgreek;03D4\nUpsilonhooksymbol;03D2\nUpsilontonos;038E\nUring;016E\nUshortcyrillic;040E\nUsmall;F775\nUstraightcyrillic;04AE\nUstraightstrokecyrillic;04B0\nUtilde;0168\nUtildeacute;1E78\nUtildebelow;1E74\nV;0056\nVcircle;24CB\nVdotbelow;1E7E\nVecyrillic;0412\nVewarmenian;054E\nVhook;01B2\nVmonospace;FF36\nVoarmenian;0548\nVsmall;F776\nVtilde;1E7C\nW;0057\nWacute;1E82\nWcircle;24CC\nWcircumflex;0174\nWdieresis;1E84\nWdotaccent;1E86\nWdotbelow;1E88\nWgrave;1E80\nWmonospace;FF37\nWsmall;F777\nX;0058\nXcircle;24CD\nXdieresis;1E8C\nXdotaccent;1E8A\nXeharmenian;053D\nXi;039E\nXmonospace;FF38\nXsmall;F778\nY;0059\nYacute;00DD\nYacutesmall;F7FD\nYatcyrillic;0462\nYcircle;24CE\nYcircumflex;0176\nYdieresis;0178\nYdieresissmall;F7FF\nYdotaccent;1E8E\nYdotbelow;1EF4\nYericyrillic;042B\nYerudieresiscyrillic;04F8\nYgrave;1EF2\nYhook;01B3\nYhookabove;1EF6\nYiarmenian;0545\nYicyrillic;0407\nYiwnarmenian;0552\nYmonospace;FF39\nYsmall;F779\nYtilde;1EF8\nYusbigcyrillic;046A\nYusbigiotifiedcyrillic;046C\nYuslittlecyrillic;0466\nYuslittleiotifiedcyrillic;0468\nZ;005A\nZaarmenian;0536\nZacute;0179\nZcaron;017D\nZcaronsmall;F6FF\nZcircle;24CF\nZcircumflex;1E90\nZdot;017B\nZdotaccent;017B\nZdotbelow;1E92\nZecyrillic;0417\nZedescendercyrillic;0498\nZedieresiscyrillic;04DE\nZeta;0396\nZhearmenian;053A\nZhebrevecyrillic;04C1\nZhecyrillic;0416\nZhedescendercyrillic;0496\nZhedieresiscyrillic;04DC\nZlinebelow;1E94\nZmonospace;FF3A\nZsmall;F77A\nZstroke;01B5\na;0061\naabengali;0986\naacute;00E1\naadeva;0906\naagujarati;0A86\naagurmukhi;0A06\naamatragurmukhi;0A3E\naarusquare;3303\naavowelsignbengali;09BE\naavowelsigndeva;093E\naavowelsigngujarati;0ABE\nabbreviationmarkarmenian;055F\nabbreviationsigndeva;0970\nabengali;0985\nabopomofo;311A\nabreve;0103\nabreveacute;1EAF\nabrevecyrillic;04D1\nabrevedotbelow;1EB7\nabrevegrave;1EB1\nabrevehookabove;1EB3\nabrevetilde;1EB5\nacaron;01CE\nacircle;24D0\nacircumflex;00E2\nacircumflexacute;1EA5\nacircumflexdotbelow;1EAD\nacircumflexgrave;1EA7\nacircumflexhookabove;1EA9\nacircumflextilde;1EAB\nacute;00B4\nacutebelowcmb;0317\nacutecmb;0301\nacutecomb;0301\nacutedeva;0954\nacutelowmod;02CF\nacutetonecmb;0341\nacyrillic;0430\nadblgrave;0201\naddakgurmukhi;0A71\nadeva;0905\nadieresis;00E4\nadieresiscyrillic;04D3\nadieresismacron;01DF\nadotbelow;1EA1\nadotmacron;01E1\nae;00E6\naeacute;01FD\naekorean;3150\naemacron;01E3\nafii00208;2015\nafii08941;20A4\nafii10017;0410\nafii10018;0411\nafii10019;0412\nafii10020;0413\nafii10021;0414\nafii10022;0415\nafii10023;0401\nafii10024;0416\nafii10025;0417\nafii10026;0418\nafii10027;0419\nafii10028;041A\nafii10029;041B\nafii10030;041C\nafii10031;041D\nafii10032;041E\nafii10033;041F\nafii10034;0420\nafii10035;0421\nafii10036;0422\nafii10037;0423\nafii10038;0424\nafii10039;0425\nafii10040;0426\nafii10041;0427\nafii10042;0428\nafii10043;0429\nafii10044;042A\nafii10045;042B\nafii10046;042C\nafii10047;042D\nafii10048;042E\nafii10049;042F\nafii10050;0490\nafii10051;0402\nafii10052;0403\nafii10053;0404\nafii10054;0405\nafii10055;0406\nafii10056;0407\nafii10057;0408\nafii10058;0409\nafii10059;040A\nafii10060;040B\nafii10061;040C\nafii10062;040E\nafii10063;F6C4\nafii10064;F6C5\nafii10065;0430\nafii10066;0431\nafii10067;0432\nafii10068;0433\nafii10069;0434\nafii10070;0435\nafii10071;0451\nafii10072;0436\nafii10073;0437\nafii10074;0438\nafii10075;0439\nafii10076;043A\nafii10077;043B\nafii10078;043C\nafii10079;043D\nafii10080;043E\nafii10081;043F\nafii10082;0440\nafii10083;0441\nafii10084;0442\nafii10085;0443\nafii10086;0444\nafii10087;0445\nafii10088;0446\nafii10089;0447\nafii10090;0448\nafii10091;0449\nafii10092;044A\nafii10093;044B\nafii10094;044C\nafii10095;044D\nafii10096;044E\nafii10097;044F\nafii10098;0491\nafii10099;0452\nafii10100;0453\nafii10101;0454\nafii10102;0455\nafii10103;0456\nafii10104;0457\nafii10105;0458\nafii10106;0459\nafii10107;045A\nafii10108;045B\nafii10109;045C\nafii10110;045E\nafii10145;040F\nafii10146;0462\nafii10147;0472\nafii10148;0474\nafii10192;F6C6\nafii10193;045F\nafii10194;0463\nafii10195;0473\nafii10196;0475\nafii10831;F6C7\nafii10832;F6C8\nafii10846;04D9\nafii299;200E\nafii300;200F\nafii301;200D\nafii57381;066A\nafii57388;060C\nafii57392;0660\nafii57393;0661\nafii57394;0662\nafii57395;0663\nafii57396;0664\nafii57397;0665\nafii57398;0666\nafii57399;0667\nafii57400;0668\nafii57401;0669\nafii57403;061B\nafii57407;061F\nafii57409;0621\nafii57410;0622\nafii57411;0623\nafii57412;0624\nafii57413;0625\nafii57414;0626\nafii57415;0627\nafii57416;0628\nafii57417;0629\nafii57418;062A\nafii57419;062B\nafii57420;062C\nafii57421;062D\nafii57422;062E\nafii57423;062F\nafii57424;0630\nafii57425;0631\nafii57426;0632\nafii57427;0633\nafii57428;0634\nafii57429;0635\nafii57430;0636\nafii57431;0637\nafii57432;0638\nafii57433;0639\nafii57434;063A\nafii57440;0640\nafii57441;0641\nafii57442;0642\nafii57443;0643\nafii57444;0644\nafii57445;0645\nafii57446;0646\nafii57448;0648\nafii57449;0649\nafii57450;064A\nafii57451;064B\nafii57452;064C\nafii57453;064D\nafii57454;064E\nafii57455;064F\nafii57456;0650\nafii57457;0651\nafii57458;0652\nafii57470;0647\nafii57505;06A4\nafii57506;067E\nafii57507;0686\nafii57508;0698\nafii57509;06AF\nafii57511;0679\nafii57512;0688\nafii57513;0691\nafii57514;06BA\nafii57519;06D2\nafii57534;06D5\nafii57636;20AA\nafii57645;05BE\nafii57658;05C3\nafii57664;05D0\nafii57665;05D1\nafii57666;05D2\nafii57667;05D3\nafii57668;05D4\nafii57669;05D5\nafii57670;05D6\nafii57671;05D7\nafii57672;05D8\nafii57673;05D9\nafii57674;05DA\nafii57675;05DB\nafii57676;05DC\nafii57677;05DD\nafii57678;05DE\nafii57679;05DF\nafii57680;05E0\nafii57681;05E1\nafii57682;05E2\nafii57683;05E3\nafii57684;05E4\nafii57685;05E5\nafii57686;05E6\nafii57687;05E7\nafii57688;05E8\nafii57689;05E9\nafii57690;05EA\nafii57694;FB2A\nafii57695;FB2B\nafii57700;FB4B\nafii57705;FB1F\nafii57716;05F0\nafii57717;05F1\nafii57718;05F2\nafii57723;FB35\nafii57793;05B4\nafii57794;05B5\nafii57795;05B6\nafii57796;05BB\nafii57797;05B8\nafii57798;05B7\nafii57799;05B0\nafii57800;05B2\nafii57801;05B1\nafii57802;05B3\nafii57803;05C2\nafii57804;05C1\nafii57806;05B9\nafii57807;05BC\nafii57839;05BD\nafii57841;05BF\nafii57842;05C0\nafii57929;02BC\nafii61248;2105\nafii61289;2113\nafii61352;2116\nafii61573;202C\nafii61574;202D\nafii61575;202E\nafii61664;200C\nafii63167;066D\nafii64937;02BD\nagrave;00E0\nagujarati;0A85\nagurmukhi;0A05\nahiragana;3042\nahookabove;1EA3\naibengali;0990\naibopomofo;311E\naideva;0910\naiecyrillic;04D5\naigujarati;0A90\naigurmukhi;0A10\naimatragurmukhi;0A48\nainarabic;0639\nainfinalarabic;FECA\naininitialarabic;FECB\nainmedialarabic;FECC\nainvertedbreve;0203\naivowelsignbengali;09C8\naivowelsigndeva;0948\naivowelsigngujarati;0AC8\nakatakana;30A2\nakatakanahalfwidth;FF71\nakorean;314F\nalef;05D0\nalefarabic;0627\nalefdageshhebrew;FB30\naleffinalarabic;FE8E\nalefhamzaabovearabic;0623\nalefhamzaabovefinalarabic;FE84\nalefhamzabelowarabic;0625\nalefhamzabelowfinalarabic;FE88\nalefhebrew;05D0\naleflamedhebrew;FB4F\nalefmaddaabovearabic;0622\nalefmaddaabovefinalarabic;FE82\nalefmaksuraarabic;0649\nalefmaksurafinalarabic;FEF0\nalefmaksurainitialarabic;FEF3\nalefmaksuramedialarabic;FEF4\nalefpatahhebrew;FB2E\nalefqamatshebrew;FB2F\naleph;2135\nallequal;224C\nalpha;03B1\nalphatonos;03AC\namacron;0101\namonospace;FF41\nampersand;0026\nampersandmonospace;FF06\nampersandsmall;F726\namsquare;33C2\nanbopomofo;3122\nangbopomofo;3124\nangkhankhuthai;0E5A\nangle;2220\nanglebracketleft;3008\nanglebracketleftvertical;FE3F\nanglebracketright;3009\nanglebracketrightvertical;FE40\nangleleft;2329\nangleright;232A\nangstrom;212B\nanoteleia;0387\nanudattadeva;0952\nanusvarabengali;0982\nanusvaradeva;0902\nanusvaragujarati;0A82\naogonek;0105\napaatosquare;3300\naparen;249C\napostrophearmenian;055A\napostrophemod;02BC\napple;F8FF\napproaches;2250\napproxequal;2248\napproxequalorimage;2252\napproximatelyequal;2245\naraeaekorean;318E\naraeakorean;318D\narc;2312\narighthalfring;1E9A\naring;00E5\naringacute;01FB\naringbelow;1E01\narrowboth;2194\narrowdashdown;21E3\narrowdashleft;21E0\narrowdashright;21E2\narrowdashup;21E1\narrowdblboth;21D4\narrowdbldown;21D3\narrowdblleft;21D0\narrowdblright;21D2\narrowdblup;21D1\narrowdown;2193\narrowdownleft;2199\narrowdownright;2198\narrowdownwhite;21E9\narrowheaddownmod;02C5\narrowheadleftmod;02C2\narrowheadrightmod;02C3\narrowheadupmod;02C4\narrowhorizex;F8E7\narrowleft;2190\narrowleftdbl;21D0\narrowleftdblstroke;21CD\narrowleftoverright;21C6\narrowleftwhite;21E6\narrowright;2192\narrowrightdblstroke;21CF\narrowrightheavy;279E\narrowrightoverleft;21C4\narrowrightwhite;21E8\narrowtableft;21E4\narrowtabright;21E5\narrowup;2191\narrowupdn;2195\narrowupdnbse;21A8\narrowupdownbase;21A8\narrowupleft;2196\narrowupleftofdown;21C5\narrowupright;2197\narrowupwhite;21E7\narrowvertex;F8E6\nasciicircum;005E\nasciicircummonospace;FF3E\nasciitilde;007E\nasciitildemonospace;FF5E\nascript;0251\nascriptturned;0252\nasmallhiragana;3041\nasmallkatakana;30A1\nasmallkatakanahalfwidth;FF67\nasterisk;002A\nasteriskaltonearabic;066D\nasteriskarabic;066D\nasteriskmath;2217\nasteriskmonospace;FF0A\nasterisksmall;FE61\nasterism;2042\nasuperior;F6E9\nasymptoticallyequal;2243\nat;0040\natilde;00E3\natmonospace;FF20\natsmall;FE6B\naturned;0250\naubengali;0994\naubopomofo;3120\naudeva;0914\naugujarati;0A94\naugurmukhi;0A14\naulengthmarkbengali;09D7\naumatragurmukhi;0A4C\nauvowelsignbengali;09CC\nauvowelsigndeva;094C\nauvowelsigngujarati;0ACC\navagrahadeva;093D\naybarmenian;0561\nayin;05E2\nayinaltonehebrew;FB20\nayinhebrew;05E2\nb;0062\nbabengali;09AC\nbackslash;005C\nbackslashmonospace;FF3C\nbadeva;092C\nbagujarati;0AAC\nbagurmukhi;0A2C\nbahiragana;3070\nbahtthai;0E3F\nbakatakana;30D0\nbar;007C\nbarmonospace;FF5C\nbbopomofo;3105\nbcircle;24D1\nbdotaccent;1E03\nbdotbelow;1E05\nbeamedsixteenthnotes;266C\nbecause;2235\nbecyrillic;0431\nbeharabic;0628\nbehfinalarabic;FE90\nbehinitialarabic;FE91\nbehiragana;3079\nbehmedialarabic;FE92\nbehmeeminitialarabic;FC9F\nbehmeemisolatedarabic;FC08\nbehnoonfinalarabic;FC6D\nbekatakana;30D9\nbenarmenian;0562\nbet;05D1\nbeta;03B2\nbetasymbolgreek;03D0\nbetdagesh;FB31\nbetdageshhebrew;FB31\nbethebrew;05D1\nbetrafehebrew;FB4C\nbhabengali;09AD\nbhadeva;092D\nbhagujarati;0AAD\nbhagurmukhi;0A2D\nbhook;0253\nbihiragana;3073\nbikatakana;30D3\nbilabialclick;0298\nbindigurmukhi;0A02\nbirusquare;3331\nblackcircle;25CF\nblackdiamond;25C6\nblackdownpointingtriangle;25BC\nblackleftpointingpointer;25C4\nblackleftpointingtriangle;25C0\nblacklenticularbracketleft;3010\nblacklenticularbracketleftvertical;FE3B\nblacklenticularbracketright;3011\nblacklenticularbracketrightvertical;FE3C\nblacklowerlefttriangle;25E3\nblacklowerrighttriangle;25E2\nblackrectangle;25AC\nblackrightpointingpointer;25BA\nblackrightpointingtriangle;25B6\nblacksmallsquare;25AA\nblacksmilingface;263B\nblacksquare;25A0\nblackstar;2605\nblackupperlefttriangle;25E4\nblackupperrighttriangle;25E5\nblackuppointingsmalltriangle;25B4\nblackuppointingtriangle;25B2\nblank;2423\nblinebelow;1E07\nblock;2588\nbmonospace;FF42\nbobaimaithai;0E1A\nbohiragana;307C\nbokatakana;30DC\nbparen;249D\nbqsquare;33C3\nbraceex;F8F4\nbraceleft;007B\nbraceleftbt;F8F3\nbraceleftmid;F8F2\nbraceleftmonospace;FF5B\nbraceleftsmall;FE5B\nbracelefttp;F8F1\nbraceleftvertical;FE37\nbraceright;007D\nbracerightbt;F8FE\nbracerightmid;F8FD\nbracerightmonospace;FF5D\nbracerightsmall;FE5C\nbracerighttp;F8FC\nbracerightvertical;FE38\nbracketleft;005B\nbracketleftbt;F8F0\nbracketleftex;F8EF\nbracketleftmonospace;FF3B\nbracketlefttp;F8EE\nbracketright;005D\nbracketrightbt;F8FB\nbracketrightex;F8FA\nbracketrightmonospace;FF3D\nbracketrighttp;F8F9\nbreve;02D8\nbrevebelowcmb;032E\nbrevecmb;0306\nbreveinvertedbelowcmb;032F\nbreveinvertedcmb;0311\nbreveinverteddoublecmb;0361\nbridgebelowcmb;032A\nbridgeinvertedbelowcmb;033A\nbrokenbar;00A6\nbstroke;0180\nbsuperior;F6EA\nbtopbar;0183\nbuhiragana;3076\nbukatakana;30D6\nbullet;2022\nbulletinverse;25D8\nbulletoperator;2219\nbullseye;25CE\nc;0063\ncaarmenian;056E\ncabengali;099A\ncacute;0107\ncadeva;091A\ncagujarati;0A9A\ncagurmukhi;0A1A\ncalsquare;3388\ncandrabindubengali;0981\ncandrabinducmb;0310\ncandrabindudeva;0901\ncandrabindugujarati;0A81\ncapslock;21EA\ncareof;2105\ncaron;02C7\ncaronbelowcmb;032C\ncaroncmb;030C\ncarriagereturn;21B5\ncbopomofo;3118\nccaron;010D\nccedilla;00E7\nccedillaacute;1E09\nccircle;24D2\nccircumflex;0109\nccurl;0255\ncdot;010B\ncdotaccent;010B\ncdsquare;33C5\ncedilla;00B8\ncedillacmb;0327\ncent;00A2\ncentigrade;2103\ncentinferior;F6DF\ncentmonospace;FFE0\ncentoldstyle;F7A2\ncentsuperior;F6E0\nchaarmenian;0579\nchabengali;099B\nchadeva;091B\nchagujarati;0A9B\nchagurmukhi;0A1B\nchbopomofo;3114\ncheabkhasiancyrillic;04BD\ncheckmark;2713\nchecyrillic;0447\nchedescenderabkhasiancyrillic;04BF\nchedescendercyrillic;04B7\nchedieresiscyrillic;04F5\ncheharmenian;0573\nchekhakassiancyrillic;04CC\ncheverticalstrokecyrillic;04B9\nchi;03C7\nchieuchacirclekorean;3277\nchieuchaparenkorean;3217\nchieuchcirclekorean;3269\nchieuchkorean;314A\nchieuchparenkorean;3209\nchochangthai;0E0A\nchochanthai;0E08\nchochingthai;0E09\nchochoethai;0E0C\nchook;0188\ncieucacirclekorean;3276\ncieucaparenkorean;3216\ncieuccirclekorean;3268\ncieuckorean;3148\ncieucparenkorean;3208\ncieucuparenkorean;321C\ncircle;25CB\ncirclemultiply;2297\ncircleot;2299\ncircleplus;2295\ncirclepostalmark;3036\ncirclewithlefthalfblack;25D0\ncirclewithrighthalfblack;25D1\ncircumflex;02C6\ncircumflexbelowcmb;032D\ncircumflexcmb;0302\nclear;2327\nclickalveolar;01C2\nclickdental;01C0\nclicklateral;01C1\nclickretroflex;01C3\nclub;2663\nclubsuitblack;2663\nclubsuitwhite;2667\ncmcubedsquare;33A4\ncmonospace;FF43\ncmsquaredsquare;33A0\ncoarmenian;0581\ncolon;003A\ncolonmonetary;20A1\ncolonmonospace;FF1A\ncolonsign;20A1\ncolonsmall;FE55\ncolontriangularhalfmod;02D1\ncolontriangularmod;02D0\ncomma;002C\ncommaabovecmb;0313\ncommaaboverightcmb;0315\ncommaaccent;F6C3\ncommaarabic;060C\ncommaarmenian;055D\ncommainferior;F6E1\ncommamonospace;FF0C\ncommareversedabovecmb;0314\ncommareversedmod;02BD\ncommasmall;FE50\ncommasuperior;F6E2\ncommaturnedabovecmb;0312\ncommaturnedmod;02BB\ncompass;263C\ncongruent;2245\ncontourintegral;222E\ncontrol;2303\ncontrolACK;0006\ncontrolBEL;0007\ncontrolBS;0008\ncontrolCAN;0018\ncontrolCR;000D\ncontrolDC1;0011\ncontrolDC2;0012\ncontrolDC3;0013\ncontrolDC4;0014\ncontrolDEL;007F\ncontrolDLE;0010\ncontrolEM;0019\ncontrolENQ;0005\ncontrolEOT;0004\ncontrolESC;001B\ncontrolETB;0017\ncontrolETX;0003\ncontrolFF;000C\ncontrolFS;001C\ncontrolGS;001D\ncontrolHT;0009\ncontrolLF;000A\ncontrolNAK;0015\ncontrolRS;001E\ncontrolSI;000F\ncontrolSO;000E\ncontrolSOT;0002\ncontrolSTX;0001\ncontrolSUB;001A\ncontrolSYN;0016\ncontrolUS;001F\ncontrolVT;000B\ncopyright;00A9\ncopyrightsans;F8E9\ncopyrightserif;F6D9\ncornerbracketleft;300C\ncornerbracketlefthalfwidth;FF62\ncornerbracketleftvertical;FE41\ncornerbracketright;300D\ncornerbracketrighthalfwidth;FF63\ncornerbracketrightvertical;FE42\ncorporationsquare;337F\ncosquare;33C7\ncoverkgsquare;33C6\ncparen;249E\ncruzeiro;20A2\ncstretched;0297\ncurlyand;22CF\ncurlyor;22CE\ncurrency;00A4\ncyrBreve;F6D1\ncyrFlex;F6D2\ncyrbreve;F6D4\ncyrflex;F6D5\nd;0064\ndaarmenian;0564\ndabengali;09A6\ndadarabic;0636\ndadeva;0926\ndadfinalarabic;FEBE\ndadinitialarabic;FEBF\ndadmedialarabic;FEC0\ndagesh;05BC\ndageshhebrew;05BC\ndagger;2020\ndaggerdbl;2021\ndagujarati;0AA6\ndagurmukhi;0A26\ndahiragana;3060\ndakatakana;30C0\ndalarabic;062F\ndalet;05D3\ndaletdagesh;FB33\ndaletdageshhebrew;FB33\ndalethatafpatah;05D3 05B2\ndalethatafpatahhebrew;05D3 05B2\ndalethatafsegol;05D3 05B1\ndalethatafsegolhebrew;05D3 05B1\ndalethebrew;05D3\ndalethiriq;05D3 05B4\ndalethiriqhebrew;05D3 05B4\ndaletholam;05D3 05B9\ndaletholamhebrew;05D3 05B9\ndaletpatah;05D3 05B7\ndaletpatahhebrew;05D3 05B7\ndaletqamats;05D3 05B8\ndaletqamatshebrew;05D3 05B8\ndaletqubuts;05D3 05BB\ndaletqubutshebrew;05D3 05BB\ndaletsegol;05D3 05B6\ndaletsegolhebrew;05D3 05B6\ndaletsheva;05D3 05B0\ndaletshevahebrew;05D3 05B0\ndalettsere;05D3 05B5\ndalettserehebrew;05D3 05B5\ndalfinalarabic;FEAA\ndammaarabic;064F\ndammalowarabic;064F\ndammatanaltonearabic;064C\ndammatanarabic;064C\ndanda;0964\ndargahebrew;05A7\ndargalefthebrew;05A7\ndasiapneumatacyrilliccmb;0485\ndblGrave;F6D3\ndblanglebracketleft;300A\ndblanglebracketleftvertical;FE3D\ndblanglebracketright;300B\ndblanglebracketrightvertical;FE3E\ndblarchinvertedbelowcmb;032B\ndblarrowleft;21D4\ndblarrowright;21D2\ndbldanda;0965\ndblgrave;F6D6\ndblgravecmb;030F\ndblintegral;222C\ndbllowline;2017\ndbllowlinecmb;0333\ndbloverlinecmb;033F\ndblprimemod;02BA\ndblverticalbar;2016\ndblverticallineabovecmb;030E\ndbopomofo;3109\ndbsquare;33C8\ndcaron;010F\ndcedilla;1E11\ndcircle;24D3\ndcircumflexbelow;1E13\ndcroat;0111\nddabengali;09A1\nddadeva;0921\nddagujarati;0AA1\nddagurmukhi;0A21\nddalarabic;0688\nddalfinalarabic;FB89\ndddhadeva;095C\nddhabengali;09A2\nddhadeva;0922\nddhagujarati;0AA2\nddhagurmukhi;0A22\nddotaccent;1E0B\nddotbelow;1E0D\ndecimalseparatorarabic;066B\ndecimalseparatorpersian;066B\ndecyrillic;0434\ndegree;00B0\ndehihebrew;05AD\ndehiragana;3067\ndeicoptic;03EF\ndekatakana;30C7\ndeleteleft;232B\ndeleteright;2326\ndelta;03B4\ndeltaturned;018D\ndenominatorminusonenumeratorbengali;09F8\ndezh;02A4\ndhabengali;09A7\ndhadeva;0927\ndhagujarati;0AA7\ndhagurmukhi;0A27\ndhook;0257\ndialytikatonos;0385\ndialytikatonoscmb;0344\ndiamond;2666\ndiamondsuitwhite;2662\ndieresis;00A8\ndieresisacute;F6D7\ndieresisbelowcmb;0324\ndieresiscmb;0308\ndieresisgrave;F6D8\ndieresistonos;0385\ndihiragana;3062\ndikatakana;30C2\ndittomark;3003\ndivide;00F7\ndivides;2223\ndivisionslash;2215\ndjecyrillic;0452\ndkshade;2593\ndlinebelow;1E0F\ndlsquare;3397\ndmacron;0111\ndmonospace;FF44\ndnblock;2584\ndochadathai;0E0E\ndodekthai;0E14\ndohiragana;3069\ndokatakana;30C9\ndollar;0024\ndollarinferior;F6E3\ndollarmonospace;FF04\ndollaroldstyle;F724\ndollarsmall;FE69\ndollarsuperior;F6E4\ndong;20AB\ndorusquare;3326\ndotaccent;02D9\ndotaccentcmb;0307\ndotbelowcmb;0323\ndotbelowcomb;0323\ndotkatakana;30FB\ndotlessi;0131\ndotlessj;F6BE\ndotlessjstrokehook;0284\ndotmath;22C5\ndottedcircle;25CC\ndoubleyodpatah;FB1F\ndoubleyodpatahhebrew;FB1F\ndowntackbelowcmb;031E\ndowntackmod;02D5\ndparen;249F\ndsuperior;F6EB\ndtail;0256\ndtopbar;018C\nduhiragana;3065\ndukatakana;30C5\ndz;01F3\ndzaltone;02A3\ndzcaron;01C6\ndzcurl;02A5\ndzeabkhasiancyrillic;04E1\ndzecyrillic;0455\ndzhecyrillic;045F\ne;0065\neacute;00E9\nearth;2641\nebengali;098F\nebopomofo;311C\nebreve;0115\necandradeva;090D\necandragujarati;0A8D\necandravowelsigndeva;0945\necandravowelsigngujarati;0AC5\necaron;011B\necedillabreve;1E1D\necharmenian;0565\nechyiwnarmenian;0587\necircle;24D4\necircumflex;00EA\necircumflexacute;1EBF\necircumflexbelow;1E19\necircumflexdotbelow;1EC7\necircumflexgrave;1EC1\necircumflexhookabove;1EC3\necircumflextilde;1EC5\necyrillic;0454\nedblgrave;0205\nedeva;090F\nedieresis;00EB\nedot;0117\nedotaccent;0117\nedotbelow;1EB9\neegurmukhi;0A0F\neematragurmukhi;0A47\nefcyrillic;0444\negrave;00E8\negujarati;0A8F\neharmenian;0567\nehbopomofo;311D\nehiragana;3048\nehookabove;1EBB\neibopomofo;311F\neight;0038\neightarabic;0668\neightbengali;09EE\neightcircle;2467\neightcircleinversesansserif;2791\neightdeva;096E\neighteencircle;2471\neighteenparen;2485\neighteenperiod;2499\neightgujarati;0AEE\neightgurmukhi;0A6E\neighthackarabic;0668\neighthangzhou;3028\neighthnotebeamed;266B\neightideographicparen;3227\neightinferior;2088\neightmonospace;FF18\neightoldstyle;F738\neightparen;247B\neightperiod;248F\neightpersian;06F8\neightroman;2177\neightsuperior;2078\neightthai;0E58\neinvertedbreve;0207\neiotifiedcyrillic;0465\nekatakana;30A8\nekatakanahalfwidth;FF74\nekonkargurmukhi;0A74\nekorean;3154\nelcyrillic;043B\nelement;2208\nelevencircle;246A\nelevenparen;247E\nelevenperiod;2492\nelevenroman;217A\nellipsis;2026\nellipsisvertical;22EE\nemacron;0113\nemacronacute;1E17\nemacrongrave;1E15\nemcyrillic;043C\nemdash;2014\nemdashvertical;FE31\nemonospace;FF45\nemphasismarkarmenian;055B\nemptyset;2205\nenbopomofo;3123\nencyrillic;043D\nendash;2013\nendashvertical;FE32\nendescendercyrillic;04A3\neng;014B\nengbopomofo;3125\nenghecyrillic;04A5\nenhookcyrillic;04C8\nenspace;2002\neogonek;0119\neokorean;3153\neopen;025B\neopenclosed;029A\neopenreversed;025C\neopenreversedclosed;025E\neopenreversedhook;025D\neparen;24A0\nepsilon;03B5\nepsilontonos;03AD\nequal;003D\nequalmonospace;FF1D\nequalsmall;FE66\nequalsuperior;207C\nequivalence;2261\nerbopomofo;3126\nercyrillic;0440\nereversed;0258\nereversedcyrillic;044D\nescyrillic;0441\nesdescendercyrillic;04AB\nesh;0283\neshcurl;0286\neshortdeva;090E\neshortvowelsigndeva;0946\neshreversedloop;01AA\neshsquatreversed;0285\nesmallhiragana;3047\nesmallkatakana;30A7\nesmallkatakanahalfwidth;FF6A\nestimated;212E\nesuperior;F6EC\neta;03B7\netarmenian;0568\netatonos;03AE\neth;00F0\netilde;1EBD\netildebelow;1E1B\netnahtafoukhhebrew;0591\netnahtafoukhlefthebrew;0591\netnahtahebrew;0591\netnahtalefthebrew;0591\neturned;01DD\neukorean;3161\neuro;20AC\nevowelsignbengali;09C7\nevowelsigndeva;0947\nevowelsigngujarati;0AC7\nexclam;0021\nexclamarmenian;055C\nexclamdbl;203C\nexclamdown;00A1\nexclamdownsmall;F7A1\nexclammonospace;FF01\nexclamsmall;F721\nexistential;2203\nezh;0292\nezhcaron;01EF\nezhcurl;0293\nezhreversed;01B9\nezhtail;01BA\nf;0066\nfadeva;095E\nfagurmukhi;0A5E\nfahrenheit;2109\nfathaarabic;064E\nfathalowarabic;064E\nfathatanarabic;064B\nfbopomofo;3108\nfcircle;24D5\nfdotaccent;1E1F\nfeharabic;0641\nfeharmenian;0586\nfehfinalarabic;FED2\nfehinitialarabic;FED3\nfehmedialarabic;FED4\nfeicoptic;03E5\nfemale;2640\nff;FB00\nffi;FB03\nffl;FB04\nfi;FB01\nfifteencircle;246E\nfifteenparen;2482\nfifteenperiod;2496\nfiguredash;2012\nfilledbox;25A0\nfilledrect;25AC\nfinalkaf;05DA\nfinalkafdagesh;FB3A\nfinalkafdageshhebrew;FB3A\nfinalkafhebrew;05DA\nfinalkafqamats;05DA 05B8\nfinalkafqamatshebrew;05DA 05B8\nfinalkafsheva;05DA 05B0\nfinalkafshevahebrew;05DA 05B0\nfinalmem;05DD\nfinalmemhebrew;05DD\nfinalnun;05DF\nfinalnunhebrew;05DF\nfinalpe;05E3\nfinalpehebrew;05E3\nfinaltsadi;05E5\nfinaltsadihebrew;05E5\nfirsttonechinese;02C9\nfisheye;25C9\nfitacyrillic;0473\nfive;0035\nfivearabic;0665\nfivebengali;09EB\nfivecircle;2464\nfivecircleinversesansserif;278E\nfivedeva;096B\nfiveeighths;215D\nfivegujarati;0AEB\nfivegurmukhi;0A6B\nfivehackarabic;0665\nfivehangzhou;3025\nfiveideographicparen;3224\nfiveinferior;2085\nfivemonospace;FF15\nfiveoldstyle;F735\nfiveparen;2478\nfiveperiod;248C\nfivepersian;06F5\nfiveroman;2174\nfivesuperior;2075\nfivethai;0E55\nfl;FB02\nflorin;0192\nfmonospace;FF46\nfmsquare;3399\nfofanthai;0E1F\nfofathai;0E1D\nfongmanthai;0E4F\nforall;2200\nfour;0034\nfourarabic;0664\nfourbengali;09EA\nfourcircle;2463\nfourcircleinversesansserif;278D\nfourdeva;096A\nfourgujarati;0AEA\nfourgurmukhi;0A6A\nfourhackarabic;0664\nfourhangzhou;3024\nfourideographicparen;3223\nfourinferior;2084\nfourmonospace;FF14\nfournumeratorbengali;09F7\nfouroldstyle;F734\nfourparen;2477\nfourperiod;248B\nfourpersian;06F4\nfourroman;2173\nfoursuperior;2074\nfourteencircle;246D\nfourteenparen;2481\nfourteenperiod;2495\nfourthai;0E54\nfourthtonechinese;02CB\nfparen;24A1\nfraction;2044\nfranc;20A3\ng;0067\ngabengali;0997\ngacute;01F5\ngadeva;0917\ngafarabic;06AF\ngaffinalarabic;FB93\ngafinitialarabic;FB94\ngafmedialarabic;FB95\ngagujarati;0A97\ngagurmukhi;0A17\ngahiragana;304C\ngakatakana;30AC\ngamma;03B3\ngammalatinsmall;0263\ngammasuperior;02E0\ngangiacoptic;03EB\ngbopomofo;310D\ngbreve;011F\ngcaron;01E7\ngcedilla;0123\ngcircle;24D6\ngcircumflex;011D\ngcommaaccent;0123\ngdot;0121\ngdotaccent;0121\ngecyrillic;0433\ngehiragana;3052\ngekatakana;30B2\ngeometricallyequal;2251\ngereshaccenthebrew;059C\ngereshhebrew;05F3\ngereshmuqdamhebrew;059D\ngermandbls;00DF\ngershayimaccenthebrew;059E\ngershayimhebrew;05F4\ngetamark;3013\nghabengali;0998\nghadarmenian;0572\nghadeva;0918\nghagujarati;0A98\nghagurmukhi;0A18\nghainarabic;063A\nghainfinalarabic;FECE\nghaininitialarabic;FECF\nghainmedialarabic;FED0\nghemiddlehookcyrillic;0495\nghestrokecyrillic;0493\ngheupturncyrillic;0491\nghhadeva;095A\nghhagurmukhi;0A5A\nghook;0260\nghzsquare;3393\ngihiragana;304E\ngikatakana;30AE\ngimarmenian;0563\ngimel;05D2\ngimeldagesh;FB32\ngimeldageshhebrew;FB32\ngimelhebrew;05D2\ngjecyrillic;0453\nglottalinvertedstroke;01BE\nglottalstop;0294\nglottalstopinverted;0296\nglottalstopmod;02C0\nglottalstopreversed;0295\nglottalstopreversedmod;02C1\nglottalstopreversedsuperior;02E4\nglottalstopstroke;02A1\nglottalstopstrokereversed;02A2\ngmacron;1E21\ngmonospace;FF47\ngohiragana;3054\ngokatakana;30B4\ngparen;24A2\ngpasquare;33AC\ngradient;2207\ngrave;0060\ngravebelowcmb;0316\ngravecmb;0300\ngravecomb;0300\ngravedeva;0953\ngravelowmod;02CE\ngravemonospace;FF40\ngravetonecmb;0340\ngreater;003E\ngreaterequal;2265\ngreaterequalorless;22DB\ngreatermonospace;FF1E\ngreaterorequivalent;2273\ngreaterorless;2277\ngreateroverequal;2267\ngreatersmall;FE65\ngscript;0261\ngstroke;01E5\nguhiragana;3050\nguillemotleft;00AB\nguillemotright;00BB\nguilsinglleft;2039\nguilsinglright;203A\ngukatakana;30B0\nguramusquare;3318\ngysquare;33C9\nh;0068\nhaabkhasiancyrillic;04A9\nhaaltonearabic;06C1\nhabengali;09B9\nhadescendercyrillic;04B3\nhadeva;0939\nhagujarati;0AB9\nhagurmukhi;0A39\nhaharabic;062D\nhahfinalarabic;FEA2\nhahinitialarabic;FEA3\nhahiragana;306F\nhahmedialarabic;FEA4\nhaitusquare;332A\nhakatakana;30CF\nhakatakanahalfwidth;FF8A\nhalantgurmukhi;0A4D\nhamzaarabic;0621\nhamzadammaarabic;0621 064F\nhamzadammatanarabic;0621 064C\nhamzafathaarabic;0621 064E\nhamzafathatanarabic;0621 064B\nhamzalowarabic;0621\nhamzalowkasraarabic;0621 0650\nhamzalowkasratanarabic;0621 064D\nhamzasukunarabic;0621 0652\nhangulfiller;3164\nhardsigncyrillic;044A\nharpoonleftbarbup;21BC\nharpoonrightbarbup;21C0\nhasquare;33CA\nhatafpatah;05B2\nhatafpatah16;05B2\nhatafpatah23;05B2\nhatafpatah2f;05B2\nhatafpatahhebrew;05B2\nhatafpatahnarrowhebrew;05B2\nhatafpatahquarterhebrew;05B2\nhatafpatahwidehebrew;05B2\nhatafqamats;05B3\nhatafqamats1b;05B3\nhatafqamats28;05B3\nhatafqamats34;05B3\nhatafqamatshebrew;05B3\nhatafqamatsnarrowhebrew;05B3\nhatafqamatsquarterhebrew;05B3\nhatafqamatswidehebrew;05B3\nhatafsegol;05B1\nhatafsegol17;05B1\nhatafsegol24;05B1\nhatafsegol30;05B1\nhatafsegolhebrew;05B1\nhatafsegolnarrowhebrew;05B1\nhatafsegolquarterhebrew;05B1\nhatafsegolwidehebrew;05B1\nhbar;0127\nhbopomofo;310F\nhbrevebelow;1E2B\nhcedilla;1E29\nhcircle;24D7\nhcircumflex;0125\nhdieresis;1E27\nhdotaccent;1E23\nhdotbelow;1E25\nhe;05D4\nheart;2665\nheartsuitblack;2665\nheartsuitwhite;2661\nhedagesh;FB34\nhedageshhebrew;FB34\nhehaltonearabic;06C1\nheharabic;0647\nhehebrew;05D4\nhehfinalaltonearabic;FBA7\nhehfinalalttwoarabic;FEEA\nhehfinalarabic;FEEA\nhehhamzaabovefinalarabic;FBA5\nhehhamzaaboveisolatedarabic;FBA4\nhehinitialaltonearabic;FBA8\nhehinitialarabic;FEEB\nhehiragana;3078\nhehmedialaltonearabic;FBA9\nhehmedialarabic;FEEC\nheiseierasquare;337B\nhekatakana;30D8\nhekatakanahalfwidth;FF8D\nhekutaarusquare;3336\nhenghook;0267\nherutusquare;3339\nhet;05D7\nhethebrew;05D7\nhhook;0266\nhhooksuperior;02B1\nhieuhacirclekorean;327B\nhieuhaparenkorean;321B\nhieuhcirclekorean;326D\nhieuhkorean;314E\nhieuhparenkorean;320D\nhihiragana;3072\nhikatakana;30D2\nhikatakanahalfwidth;FF8B\nhiriq;05B4\nhiriq14;05B4\nhiriq21;05B4\nhiriq2d;05B4\nhiriqhebrew;05B4\nhiriqnarrowhebrew;05B4\nhiriqquarterhebrew;05B4\nhiriqwidehebrew;05B4\nhlinebelow;1E96\nhmonospace;FF48\nhoarmenian;0570\nhohipthai;0E2B\nhohiragana;307B\nhokatakana;30DB\nhokatakanahalfwidth;FF8E\nholam;05B9\nholam19;05B9\nholam26;05B9\nholam32;05B9\nholamhebrew;05B9\nholamnarrowhebrew;05B9\nholamquarterhebrew;05B9\nholamwidehebrew;05B9\nhonokhukthai;0E2E\nhookabovecomb;0309\nhookcmb;0309\nhookpalatalizedbelowcmb;0321\nhookretroflexbelowcmb;0322\nhoonsquare;3342\nhoricoptic;03E9\nhorizontalbar;2015\nhorncmb;031B\nhotsprings;2668\nhouse;2302\nhparen;24A3\nhsuperior;02B0\nhturned;0265\nhuhiragana;3075\nhuiitosquare;3333\nhukatakana;30D5\nhukatakanahalfwidth;FF8C\nhungarumlaut;02DD\nhungarumlautcmb;030B\nhv;0195\nhyphen;002D\nhypheninferior;F6E5\nhyphenmonospace;FF0D\nhyphensmall;FE63\nhyphensuperior;F6E6\nhyphentwo;2010\ni;0069\niacute;00ED\niacyrillic;044F\nibengali;0987\nibopomofo;3127\nibreve;012D\nicaron;01D0\nicircle;24D8\nicircumflex;00EE\nicyrillic;0456\nidblgrave;0209\nideographearthcircle;328F\nideographfirecircle;328B\nideographicallianceparen;323F\nideographiccallparen;323A\nideographiccentrecircle;32A5\nideographicclose;3006\nideographiccomma;3001\nideographiccommaleft;FF64\nideographiccongratulationparen;3237\nideographiccorrectcircle;32A3\nideographicearthparen;322F\nideographicenterpriseparen;323D\nideographicexcellentcircle;329D\nideographicfestivalparen;3240\nideographicfinancialcircle;3296\nideographicfinancialparen;3236\nideographicfireparen;322B\nideographichaveparen;3232\nideographichighcircle;32A4\nideographiciterationmark;3005\nideographiclaborcircle;3298\nideographiclaborparen;3238\nideographicleftcircle;32A7\nideographiclowcircle;32A6\nideographicmedicinecircle;32A9\nideographicmetalparen;322E\nideographicmoonparen;322A\nideographicnameparen;3234\nideographicperiod;3002\nideographicprintcircle;329E\nideographicreachparen;3243\nideographicrepresentparen;3239\nideographicresourceparen;323E\nideographicrightcircle;32A8\nideographicsecretcircle;3299\nideographicselfparen;3242\nideographicsocietyparen;3233\nideographicspace;3000\nideographicspecialparen;3235\nideographicstockparen;3231\nideographicstudyparen;323B\nideographicsunparen;3230\nideographicsuperviseparen;323C\nideographicwaterparen;322C\nideographicwoodparen;322D\nideographiczero;3007\nideographmetalcircle;328E\nideographmooncircle;328A\nideographnamecircle;3294\nideographsuncircle;3290\nideographwatercircle;328C\nideographwoodcircle;328D\nideva;0907\nidieresis;00EF\nidieresisacute;1E2F\nidieresiscyrillic;04E5\nidotbelow;1ECB\niebrevecyrillic;04D7\niecyrillic;0435\nieungacirclekorean;3275\nieungaparenkorean;3215\nieungcirclekorean;3267\nieungkorean;3147\nieungparenkorean;3207\nigrave;00EC\nigujarati;0A87\nigurmukhi;0A07\nihiragana;3044\nihookabove;1EC9\niibengali;0988\niicyrillic;0438\niideva;0908\niigujarati;0A88\niigurmukhi;0A08\niimatragurmukhi;0A40\niinvertedbreve;020B\niishortcyrillic;0439\niivowelsignbengali;09C0\niivowelsigndeva;0940\niivowelsigngujarati;0AC0\nij;0133\nikatakana;30A4\nikatakanahalfwidth;FF72\nikorean;3163\nilde;02DC\niluyhebrew;05AC\nimacron;012B\nimacroncyrillic;04E3\nimageorapproximatelyequal;2253\nimatragurmukhi;0A3F\nimonospace;FF49\nincrement;2206\ninfinity;221E\niniarmenian;056B\nintegral;222B\nintegralbottom;2321\nintegralbt;2321\nintegralex;F8F5\nintegraltop;2320\nintegraltp;2320\nintersection;2229\nintisquare;3305\ninvbullet;25D8\ninvcircle;25D9\ninvsmileface;263B\niocyrillic;0451\niogonek;012F\niota;03B9\niotadieresis;03CA\niotadieresistonos;0390\niotalatin;0269\niotatonos;03AF\niparen;24A4\nirigurmukhi;0A72\nismallhiragana;3043\nismallkatakana;30A3\nismallkatakanahalfwidth;FF68\nissharbengali;09FA\nistroke;0268\nisuperior;F6ED\niterationhiragana;309D\niterationkatakana;30FD\nitilde;0129\nitildebelow;1E2D\niubopomofo;3129\niucyrillic;044E\nivowelsignbengali;09BF\nivowelsigndeva;093F\nivowelsigngujarati;0ABF\nizhitsacyrillic;0475\nizhitsadblgravecyrillic;0477\nj;006A\njaarmenian;0571\njabengali;099C\njadeva;091C\njagujarati;0A9C\njagurmukhi;0A1C\njbopomofo;3110\njcaron;01F0\njcircle;24D9\njcircumflex;0135\njcrossedtail;029D\njdotlessstroke;025F\njecyrillic;0458\njeemarabic;062C\njeemfinalarabic;FE9E\njeeminitialarabic;FE9F\njeemmedialarabic;FEA0\njeharabic;0698\njehfinalarabic;FB8B\njhabengali;099D\njhadeva;091D\njhagujarati;0A9D\njhagurmukhi;0A1D\njheharmenian;057B\njis;3004\njmonospace;FF4A\njparen;24A5\njsuperior;02B2\nk;006B\nkabashkircyrillic;04A1\nkabengali;0995\nkacute;1E31\nkacyrillic;043A\nkadescendercyrillic;049B\nkadeva;0915\nkaf;05DB\nkafarabic;0643\nkafdagesh;FB3B\nkafdageshhebrew;FB3B\nkaffinalarabic;FEDA\nkafhebrew;05DB\nkafinitialarabic;FEDB\nkafmedialarabic;FEDC\nkafrafehebrew;FB4D\nkagujarati;0A95\nkagurmukhi;0A15\nkahiragana;304B\nkahookcyrillic;04C4\nkakatakana;30AB\nkakatakanahalfwidth;FF76\nkappa;03BA\nkappasymbolgreek;03F0\nkapyeounmieumkorean;3171\nkapyeounphieuphkorean;3184\nkapyeounpieupkorean;3178\nkapyeounssangpieupkorean;3179\nkaroriisquare;330D\nkashidaautoarabic;0640\nkashidaautonosidebearingarabic;0640\nkasmallkatakana;30F5\nkasquare;3384\nkasraarabic;0650\nkasratanarabic;064D\nkastrokecyrillic;049F\nkatahiraprolongmarkhalfwidth;FF70\nkaverticalstrokecyrillic;049D\nkbopomofo;310E\nkcalsquare;3389\nkcaron;01E9\nkcedilla;0137\nkcircle;24DA\nkcommaaccent;0137\nkdotbelow;1E33\nkeharmenian;0584\nkehiragana;3051\nkekatakana;30B1\nkekatakanahalfwidth;FF79\nkenarmenian;056F\nkesmallkatakana;30F6\nkgreenlandic;0138\nkhabengali;0996\nkhacyrillic;0445\nkhadeva;0916\nkhagujarati;0A96\nkhagurmukhi;0A16\nkhaharabic;062E\nkhahfinalarabic;FEA6\nkhahinitialarabic;FEA7\nkhahmedialarabic;FEA8\nkheicoptic;03E7\nkhhadeva;0959\nkhhagurmukhi;0A59\nkhieukhacirclekorean;3278\nkhieukhaparenkorean;3218\nkhieukhcirclekorean;326A\nkhieukhkorean;314B\nkhieukhparenkorean;320A\nkhokhaithai;0E02\nkhokhonthai;0E05\nkhokhuatthai;0E03\nkhokhwaithai;0E04\nkhomutthai;0E5B\nkhook;0199\nkhorakhangthai;0E06\nkhzsquare;3391\nkihiragana;304D\nkikatakana;30AD\nkikatakanahalfwidth;FF77\nkiroguramusquare;3315\nkiromeetorusquare;3316\nkirosquare;3314\nkiyeokacirclekorean;326E\nkiyeokaparenkorean;320E\nkiyeokcirclekorean;3260\nkiyeokkorean;3131\nkiyeokparenkorean;3200\nkiyeoksioskorean;3133\nkjecyrillic;045C\nklinebelow;1E35\nklsquare;3398\nkmcubedsquare;33A6\nkmonospace;FF4B\nkmsquaredsquare;33A2\nkohiragana;3053\nkohmsquare;33C0\nkokaithai;0E01\nkokatakana;30B3\nkokatakanahalfwidth;FF7A\nkooposquare;331E\nkoppacyrillic;0481\nkoreanstandardsymbol;327F\nkoroniscmb;0343\nkparen;24A6\nkpasquare;33AA\nksicyrillic;046F\nktsquare;33CF\nkturned;029E\nkuhiragana;304F\nkukatakana;30AF\nkukatakanahalfwidth;FF78\nkvsquare;33B8\nkwsquare;33BE\nl;006C\nlabengali;09B2\nlacute;013A\nladeva;0932\nlagujarati;0AB2\nlagurmukhi;0A32\nlakkhangyaothai;0E45\nlamaleffinalarabic;FEFC\nlamalefhamzaabovefinalarabic;FEF8\nlamalefhamzaaboveisolatedarabic;FEF7\nlamalefhamzabelowfinalarabic;FEFA\nlamalefhamzabelowisolatedarabic;FEF9\nlamalefisolatedarabic;FEFB\nlamalefmaddaabovefinalarabic;FEF6\nlamalefmaddaaboveisolatedarabic;FEF5\nlamarabic;0644\nlambda;03BB\nlambdastroke;019B\nlamed;05DC\nlameddagesh;FB3C\nlameddageshhebrew;FB3C\nlamedhebrew;05DC\nlamedholam;05DC 05B9\nlamedholamdagesh;05DC 05B9 05BC\nlamedholamdageshhebrew;05DC 05B9 05BC\nlamedholamhebrew;05DC 05B9\nlamfinalarabic;FEDE\nlamhahinitialarabic;FCCA\nlaminitialarabic;FEDF\nlamjeeminitialarabic;FCC9\nlamkhahinitialarabic;FCCB\nlamlamhehisolatedarabic;FDF2\nlammedialarabic;FEE0\nlammeemhahinitialarabic;FD88\nlammeeminitialarabic;FCCC\nlammeemjeeminitialarabic;FEDF FEE4 FEA0\nlammeemkhahinitialarabic;FEDF FEE4 FEA8\nlargecircle;25EF\nlbar;019A\nlbelt;026C\nlbopomofo;310C\nlcaron;013E\nlcedilla;013C\nlcircle;24DB\nlcircumflexbelow;1E3D\nlcommaaccent;013C\nldot;0140\nldotaccent;0140\nldotbelow;1E37\nldotbelowmacron;1E39\nleftangleabovecmb;031A\nlefttackbelowcmb;0318\nless;003C\nlessequal;2264\nlessequalorgreater;22DA\nlessmonospace;FF1C\nlessorequivalent;2272\nlessorgreater;2276\nlessoverequal;2266\nlesssmall;FE64\nlezh;026E\nlfblock;258C\nlhookretroflex;026D\nlira;20A4\nliwnarmenian;056C\nlj;01C9\nljecyrillic;0459\nll;F6C0\nlladeva;0933\nllagujarati;0AB3\nllinebelow;1E3B\nllladeva;0934\nllvocalicbengali;09E1\nllvocalicdeva;0961\nllvocalicvowelsignbengali;09E3\nllvocalicvowelsigndeva;0963\nlmiddletilde;026B\nlmonospace;FF4C\nlmsquare;33D0\nlochulathai;0E2C\nlogicaland;2227\nlogicalnot;00AC\nlogicalnotreversed;2310\nlogicalor;2228\nlolingthai;0E25\nlongs;017F\nlowlinecenterline;FE4E\nlowlinecmb;0332\nlowlinedashed;FE4D\nlozenge;25CA\nlparen;24A7\nlslash;0142\nlsquare;2113\nlsuperior;F6EE\nltshade;2591\nluthai;0E26\nlvocalicbengali;098C\nlvocalicdeva;090C\nlvocalicvowelsignbengali;09E2\nlvocalicvowelsigndeva;0962\nlxsquare;33D3\nm;006D\nmabengali;09AE\nmacron;00AF\nmacronbelowcmb;0331\nmacroncmb;0304\nmacronlowmod;02CD\nmacronmonospace;FFE3\nmacute;1E3F\nmadeva;092E\nmagujarati;0AAE\nmagurmukhi;0A2E\nmahapakhhebrew;05A4\nmahapakhlefthebrew;05A4\nmahiragana;307E\nmaichattawalowleftthai;F895\nmaichattawalowrightthai;F894\nmaichattawathai;0E4B\nmaichattawaupperleftthai;F893\nmaieklowleftthai;F88C\nmaieklowrightthai;F88B\nmaiekthai;0E48\nmaiekupperleftthai;F88A\nmaihanakatleftthai;F884\nmaihanakatthai;0E31\nmaitaikhuleftthai;F889\nmaitaikhuthai;0E47\nmaitholowleftthai;F88F\nmaitholowrightthai;F88E\nmaithothai;0E49\nmaithoupperleftthai;F88D\nmaitrilowleftthai;F892\nmaitrilowrightthai;F891\nmaitrithai;0E4A\nmaitriupperleftthai;F890\nmaiyamokthai;0E46\nmakatakana;30DE\nmakatakanahalfwidth;FF8F\nmale;2642\nmansyonsquare;3347\nmaqafhebrew;05BE\nmars;2642\nmasoracirclehebrew;05AF\nmasquare;3383\nmbopomofo;3107\nmbsquare;33D4\nmcircle;24DC\nmcubedsquare;33A5\nmdotaccent;1E41\nmdotbelow;1E43\nmeemarabic;0645\nmeemfinalarabic;FEE2\nmeeminitialarabic;FEE3\nmeemmedialarabic;FEE4\nmeemmeeminitialarabic;FCD1\nmeemmeemisolatedarabic;FC48\nmeetorusquare;334D\nmehiragana;3081\nmeizierasquare;337E\nmekatakana;30E1\nmekatakanahalfwidth;FF92\nmem;05DE\nmemdagesh;FB3E\nmemdageshhebrew;FB3E\nmemhebrew;05DE\nmenarmenian;0574\nmerkhahebrew;05A5\nmerkhakefulahebrew;05A6\nmerkhakefulalefthebrew;05A6\nmerkhalefthebrew;05A5\nmhook;0271\nmhzsquare;3392\nmiddledotkatakanahalfwidth;FF65\nmiddot;00B7\nmieumacirclekorean;3272\nmieumaparenkorean;3212\nmieumcirclekorean;3264\nmieumkorean;3141\nmieumpansioskorean;3170\nmieumparenkorean;3204\nmieumpieupkorean;316E\nmieumsioskorean;316F\nmihiragana;307F\nmikatakana;30DF\nmikatakanahalfwidth;FF90\nminus;2212\nminusbelowcmb;0320\nminuscircle;2296\nminusmod;02D7\nminusplus;2213\nminute;2032\nmiribaarusquare;334A\nmirisquare;3349\nmlonglegturned;0270\nmlsquare;3396\nmmcubedsquare;33A3\nmmonospace;FF4D\nmmsquaredsquare;339F\nmohiragana;3082\nmohmsquare;33C1\nmokatakana;30E2\nmokatakanahalfwidth;FF93\nmolsquare;33D6\nmomathai;0E21\nmoverssquare;33A7\nmoverssquaredsquare;33A8\nmparen;24A8\nmpasquare;33AB\nmssquare;33B3\nmsuperior;F6EF\nmturned;026F\nmu;00B5\nmu1;00B5\nmuasquare;3382\nmuchgreater;226B\nmuchless;226A\nmufsquare;338C\nmugreek;03BC\nmugsquare;338D\nmuhiragana;3080\nmukatakana;30E0\nmukatakanahalfwidth;FF91\nmulsquare;3395\nmultiply;00D7\nmumsquare;339B\nmunahhebrew;05A3\nmunahlefthebrew;05A3\nmusicalnote;266A\nmusicalnotedbl;266B\nmusicflatsign;266D\nmusicsharpsign;266F\nmussquare;33B2\nmuvsquare;33B6\nmuwsquare;33BC\nmvmegasquare;33B9\nmvsquare;33B7\nmwmegasquare;33BF\nmwsquare;33BD\nn;006E\nnabengali;09A8\nnabla;2207\nnacute;0144\nnadeva;0928\nnagujarati;0AA8\nnagurmukhi;0A28\nnahiragana;306A\nnakatakana;30CA\nnakatakanahalfwidth;FF85\nnapostrophe;0149\nnasquare;3381\nnbopomofo;310B\nnbspace;00A0\nncaron;0148\nncedilla;0146\nncircle;24DD\nncircumflexbelow;1E4B\nncommaaccent;0146\nndotaccent;1E45\nndotbelow;1E47\nnehiragana;306D\nnekatakana;30CD\nnekatakanahalfwidth;FF88\nnewsheqelsign;20AA\nnfsquare;338B\nngabengali;0999\nngadeva;0919\nngagujarati;0A99\nngagurmukhi;0A19\nngonguthai;0E07\nnhiragana;3093\nnhookleft;0272\nnhookretroflex;0273\nnieunacirclekorean;326F\nnieunaparenkorean;320F\nnieuncieuckorean;3135\nnieuncirclekorean;3261\nnieunhieuhkorean;3136\nnieunkorean;3134\nnieunpansioskorean;3168\nnieunparenkorean;3201\nnieunsioskorean;3167\nnieuntikeutkorean;3166\nnihiragana;306B\nnikatakana;30CB\nnikatakanahalfwidth;FF86\nnikhahitleftthai;F899\nnikhahitthai;0E4D\nnine;0039\nninearabic;0669\nninebengali;09EF\nninecircle;2468\nninecircleinversesansserif;2792\nninedeva;096F\nninegujarati;0AEF\nninegurmukhi;0A6F\nninehackarabic;0669\nninehangzhou;3029\nnineideographicparen;3228\nnineinferior;2089\nninemonospace;FF19\nnineoldstyle;F739\nnineparen;247C\nnineperiod;2490\nninepersian;06F9\nnineroman;2178\nninesuperior;2079\nnineteencircle;2472\nnineteenparen;2486\nnineteenperiod;249A\nninethai;0E59\nnj;01CC\nnjecyrillic;045A\nnkatakana;30F3\nnkatakanahalfwidth;FF9D\nnlegrightlong;019E\nnlinebelow;1E49\nnmonospace;FF4E\nnmsquare;339A\nnnabengali;09A3\nnnadeva;0923\nnnagujarati;0AA3\nnnagurmukhi;0A23\nnnnadeva;0929\nnohiragana;306E\nnokatakana;30CE\nnokatakanahalfwidth;FF89\nnonbreakingspace;00A0\nnonenthai;0E13\nnonuthai;0E19\nnoonarabic;0646\nnoonfinalarabic;FEE6\nnoonghunnaarabic;06BA\nnoonghunnafinalarabic;FB9F\nnoonhehinitialarabic;FEE7 FEEC\nnooninitialarabic;FEE7\nnoonjeeminitialarabic;FCD2\nnoonjeemisolatedarabic;FC4B\nnoonmedialarabic;FEE8\nnoonmeeminitialarabic;FCD5\nnoonmeemisolatedarabic;FC4E\nnoonnoonfinalarabic;FC8D\nnotcontains;220C\nnotelement;2209\nnotelementof;2209\nnotequal;2260\nnotgreater;226F\nnotgreaternorequal;2271\nnotgreaternorless;2279\nnotidentical;2262\nnotless;226E\nnotlessnorequal;2270\nnotparallel;2226\nnotprecedes;2280\nnotsubset;2284\nnotsucceeds;2281\nnotsuperset;2285\nnowarmenian;0576\nnparen;24A9\nnssquare;33B1\nnsuperior;207F\nntilde;00F1\nnu;03BD\nnuhiragana;306C\nnukatakana;30CC\nnukatakanahalfwidth;FF87\nnuktabengali;09BC\nnuktadeva;093C\nnuktagujarati;0ABC\nnuktagurmukhi;0A3C\nnumbersign;0023\nnumbersignmonospace;FF03\nnumbersignsmall;FE5F\nnumeralsigngreek;0374\nnumeralsignlowergreek;0375\nnumero;2116\nnun;05E0\nnundagesh;FB40\nnundageshhebrew;FB40\nnunhebrew;05E0\nnvsquare;33B5\nnwsquare;33BB\nnyabengali;099E\nnyadeva;091E\nnyagujarati;0A9E\nnyagurmukhi;0A1E\no;006F\noacute;00F3\noangthai;0E2D\nobarred;0275\nobarredcyrillic;04E9\nobarreddieresiscyrillic;04EB\nobengali;0993\nobopomofo;311B\nobreve;014F\nocandradeva;0911\nocandragujarati;0A91\nocandravowelsigndeva;0949\nocandravowelsigngujarati;0AC9\nocaron;01D2\nocircle;24DE\nocircumflex;00F4\nocircumflexacute;1ED1\nocircumflexdotbelow;1ED9\nocircumflexgrave;1ED3\nocircumflexhookabove;1ED5\nocircumflextilde;1ED7\nocyrillic;043E\nodblacute;0151\nodblgrave;020D\nodeva;0913\nodieresis;00F6\nodieresiscyrillic;04E7\nodotbelow;1ECD\noe;0153\noekorean;315A\nogonek;02DB\nogonekcmb;0328\nograve;00F2\nogujarati;0A93\noharmenian;0585\nohiragana;304A\nohookabove;1ECF\nohorn;01A1\nohornacute;1EDB\nohorndotbelow;1EE3\nohorngrave;1EDD\nohornhookabove;1EDF\nohorntilde;1EE1\nohungarumlaut;0151\noi;01A3\noinvertedbreve;020F\nokatakana;30AA\nokatakanahalfwidth;FF75\nokorean;3157\nolehebrew;05AB\nomacron;014D\nomacronacute;1E53\nomacrongrave;1E51\nomdeva;0950\nomega;03C9\nomega1;03D6\nomegacyrillic;0461\nomegalatinclosed;0277\nomegaroundcyrillic;047B\nomegatitlocyrillic;047D\nomegatonos;03CE\nomgujarati;0AD0\nomicron;03BF\nomicrontonos;03CC\nomonospace;FF4F\none;0031\nonearabic;0661\nonebengali;09E7\nonecircle;2460\nonecircleinversesansserif;278A\nonedeva;0967\nonedotenleader;2024\noneeighth;215B\nonefitted;F6DC\nonegujarati;0AE7\nonegurmukhi;0A67\nonehackarabic;0661\nonehalf;00BD\nonehangzhou;3021\noneideographicparen;3220\noneinferior;2081\nonemonospace;FF11\nonenumeratorbengali;09F4\noneoldstyle;F731\noneparen;2474\noneperiod;2488\nonepersian;06F1\nonequarter;00BC\noneroman;2170\nonesuperior;00B9\nonethai;0E51\nonethird;2153\noogonek;01EB\noogonekmacron;01ED\noogurmukhi;0A13\noomatragurmukhi;0A4B\noopen;0254\noparen;24AA\nopenbullet;25E6\noption;2325\nordfeminine;00AA\nordmasculine;00BA\northogonal;221F\noshortdeva;0912\noshortvowelsigndeva;094A\noslash;00F8\noslashacute;01FF\nosmallhiragana;3049\nosmallkatakana;30A9\nosmallkatakanahalfwidth;FF6B\nostrokeacute;01FF\nosuperior;F6F0\notcyrillic;047F\notilde;00F5\notildeacute;1E4D\notildedieresis;1E4F\noubopomofo;3121\noverline;203E\noverlinecenterline;FE4A\noverlinecmb;0305\noverlinedashed;FE49\noverlinedblwavy;FE4C\noverlinewavy;FE4B\noverscore;00AF\novowelsignbengali;09CB\novowelsigndeva;094B\novowelsigngujarati;0ACB\np;0070\npaampssquare;3380\npaasentosquare;332B\npabengali;09AA\npacute;1E55\npadeva;092A\npagedown;21DF\npageup;21DE\npagujarati;0AAA\npagurmukhi;0A2A\npahiragana;3071\npaiyannoithai;0E2F\npakatakana;30D1\npalatalizationcyrilliccmb;0484\npalochkacyrillic;04C0\npansioskorean;317F\nparagraph;00B6\nparallel;2225\nparenleft;0028\nparenleftaltonearabic;FD3E\nparenleftbt;F8ED\nparenleftex;F8EC\nparenleftinferior;208D\nparenleftmonospace;FF08\nparenleftsmall;FE59\nparenleftsuperior;207D\nparenlefttp;F8EB\nparenleftvertical;FE35\nparenright;0029\nparenrightaltonearabic;FD3F\nparenrightbt;F8F8\nparenrightex;F8F7\nparenrightinferior;208E\nparenrightmonospace;FF09\nparenrightsmall;FE5A\nparenrightsuperior;207E\nparenrighttp;F8F6\nparenrightvertical;FE36\npartialdiff;2202\npaseqhebrew;05C0\npashtahebrew;0599\npasquare;33A9\npatah;05B7\npatah11;05B7\npatah1d;05B7\npatah2a;05B7\npatahhebrew;05B7\npatahnarrowhebrew;05B7\npatahquarterhebrew;05B7\npatahwidehebrew;05B7\npazerhebrew;05A1\npbopomofo;3106\npcircle;24DF\npdotaccent;1E57\npe;05E4\npecyrillic;043F\npedagesh;FB44\npedageshhebrew;FB44\npeezisquare;333B\npefinaldageshhebrew;FB43\npeharabic;067E\npeharmenian;057A\npehebrew;05E4\npehfinalarabic;FB57\npehinitialarabic;FB58\npehiragana;307A\npehmedialarabic;FB59\npekatakana;30DA\npemiddlehookcyrillic;04A7\nperafehebrew;FB4E\npercent;0025\npercentarabic;066A\npercentmonospace;FF05\npercentsmall;FE6A\nperiod;002E\nperiodarmenian;0589\nperiodcentered;00B7\nperiodhalfwidth;FF61\nperiodinferior;F6E7\nperiodmonospace;FF0E\nperiodsmall;FE52\nperiodsuperior;F6E8\nperispomenigreekcmb;0342\nperpendicular;22A5\nperthousand;2030\npeseta;20A7\npfsquare;338A\nphabengali;09AB\nphadeva;092B\nphagujarati;0AAB\nphagurmukhi;0A2B\nphi;03C6\nphi1;03D5\nphieuphacirclekorean;327A\nphieuphaparenkorean;321A\nphieuphcirclekorean;326C\nphieuphkorean;314D\nphieuphparenkorean;320C\nphilatin;0278\nphinthuthai;0E3A\nphisymbolgreek;03D5\nphook;01A5\nphophanthai;0E1E\nphophungthai;0E1C\nphosamphaothai;0E20\npi;03C0\npieupacirclekorean;3273\npieupaparenkorean;3213\npieupcieuckorean;3176\npieupcirclekorean;3265\npieupkiyeokkorean;3172\npieupkorean;3142\npieupparenkorean;3205\npieupsioskiyeokkorean;3174\npieupsioskorean;3144\npieupsiostikeutkorean;3175\npieupthieuthkorean;3177\npieuptikeutkorean;3173\npihiragana;3074\npikatakana;30D4\npisymbolgreek;03D6\npiwrarmenian;0583\nplus;002B\nplusbelowcmb;031F\npluscircle;2295\nplusminus;00B1\nplusmod;02D6\nplusmonospace;FF0B\nplussmall;FE62\nplussuperior;207A\npmonospace;FF50\npmsquare;33D8\npohiragana;307D\npointingindexdownwhite;261F\npointingindexleftwhite;261C\npointingindexrightwhite;261E\npointingindexupwhite;261D\npokatakana;30DD\npoplathai;0E1B\npostalmark;3012\npostalmarkface;3020\npparen;24AB\nprecedes;227A\nprescription;211E\nprimemod;02B9\nprimereversed;2035\nproduct;220F\nprojective;2305\nprolongedkana;30FC\npropellor;2318\npropersubset;2282\npropersuperset;2283\nproportion;2237\nproportional;221D\npsi;03C8\npsicyrillic;0471\npsilipneumatacyrilliccmb;0486\npssquare;33B0\npuhiragana;3077\npukatakana;30D7\npvsquare;33B4\npwsquare;33BA\nq;0071\nqadeva;0958\nqadmahebrew;05A8\nqafarabic;0642\nqaffinalarabic;FED6\nqafinitialarabic;FED7\nqafmedialarabic;FED8\nqamats;05B8\nqamats10;05B8\nqamats1a;05B8\nqamats1c;05B8\nqamats27;05B8\nqamats29;05B8\nqamats33;05B8\nqamatsde;05B8\nqamatshebrew;05B8\nqamatsnarrowhebrew;05B8\nqamatsqatanhebrew;05B8\nqamatsqatannarrowhebrew;05B8\nqamatsqatanquarterhebrew;05B8\nqamatsqatanwidehebrew;05B8\nqamatsquarterhebrew;05B8\nqamatswidehebrew;05B8\nqarneyparahebrew;059F\nqbopomofo;3111\nqcircle;24E0\nqhook;02A0\nqmonospace;FF51\nqof;05E7\nqofdagesh;FB47\nqofdageshhebrew;FB47\nqofhatafpatah;05E7 05B2\nqofhatafpatahhebrew;05E7 05B2\nqofhatafsegol;05E7 05B1\nqofhatafsegolhebrew;05E7 05B1\nqofhebrew;05E7\nqofhiriq;05E7 05B4\nqofhiriqhebrew;05E7 05B4\nqofholam;05E7 05B9\nqofholamhebrew;05E7 05B9\nqofpatah;05E7 05B7\nqofpatahhebrew;05E7 05B7\nqofqamats;05E7 05B8\nqofqamatshebrew;05E7 05B8\nqofqubuts;05E7 05BB\nqofqubutshebrew;05E7 05BB\nqofsegol;05E7 05B6\nqofsegolhebrew;05E7 05B6\nqofsheva;05E7 05B0\nqofshevahebrew;05E7 05B0\nqoftsere;05E7 05B5\nqoftserehebrew;05E7 05B5\nqparen;24AC\nquarternote;2669\nqubuts;05BB\nqubuts18;05BB\nqubuts25;05BB\nqubuts31;05BB\nqubutshebrew;05BB\nqubutsnarrowhebrew;05BB\nqubutsquarterhebrew;05BB\nqubutswidehebrew;05BB\nquestion;003F\nquestionarabic;061F\nquestionarmenian;055E\nquestiondown;00BF\nquestiondownsmall;F7BF\nquestiongreek;037E\nquestionmonospace;FF1F\nquestionsmall;F73F\nquotedbl;0022\nquotedblbase;201E\nquotedblleft;201C\nquotedblmonospace;FF02\nquotedblprime;301E\nquotedblprimereversed;301D\nquotedblright;201D\nquoteleft;2018\nquoteleftreversed;201B\nquotereversed;201B\nquoteright;2019\nquoterightn;0149\nquotesinglbase;201A\nquotesingle;0027\nquotesinglemonospace;FF07\nr;0072\nraarmenian;057C\nrabengali;09B0\nracute;0155\nradeva;0930\nradical;221A\nradicalex;F8E5\nradoverssquare;33AE\nradoverssquaredsquare;33AF\nradsquare;33AD\nrafe;05BF\nrafehebrew;05BF\nragujarati;0AB0\nragurmukhi;0A30\nrahiragana;3089\nrakatakana;30E9\nrakatakanahalfwidth;FF97\nralowerdiagonalbengali;09F1\nramiddlediagonalbengali;09F0\nramshorn;0264\nratio;2236\nrbopomofo;3116\nrcaron;0159\nrcedilla;0157\nrcircle;24E1\nrcommaaccent;0157\nrdblgrave;0211\nrdotaccent;1E59\nrdotbelow;1E5B\nrdotbelowmacron;1E5D\nreferencemark;203B\nreflexsubset;2286\nreflexsuperset;2287\nregistered;00AE\nregistersans;F8E8\nregisterserif;F6DA\nreharabic;0631\nreharmenian;0580\nrehfinalarabic;FEAE\nrehiragana;308C\nrehyehaleflamarabic;0631 FEF3 FE8E 0644\nrekatakana;30EC\nrekatakanahalfwidth;FF9A\nresh;05E8\nreshdageshhebrew;FB48\nreshhatafpatah;05E8 05B2\nreshhatafpatahhebrew;05E8 05B2\nreshhatafsegol;05E8 05B1\nreshhatafsegolhebrew;05E8 05B1\nreshhebrew;05E8\nreshhiriq;05E8 05B4\nreshhiriqhebrew;05E8 05B4\nreshholam;05E8 05B9\nreshholamhebrew;05E8 05B9\nreshpatah;05E8 05B7\nreshpatahhebrew;05E8 05B7\nreshqamats;05E8 05B8\nreshqamatshebrew;05E8 05B8\nreshqubuts;05E8 05BB\nreshqubutshebrew;05E8 05BB\nreshsegol;05E8 05B6\nreshsegolhebrew;05E8 05B6\nreshsheva;05E8 05B0\nreshshevahebrew;05E8 05B0\nreshtsere;05E8 05B5\nreshtserehebrew;05E8 05B5\nreversedtilde;223D\nreviahebrew;0597\nreviamugrashhebrew;0597\nrevlogicalnot;2310\nrfishhook;027E\nrfishhookreversed;027F\nrhabengali;09DD\nrhadeva;095D\nrho;03C1\nrhook;027D\nrhookturned;027B\nrhookturnedsuperior;02B5\nrhosymbolgreek;03F1\nrhotichookmod;02DE\nrieulacirclekorean;3271\nrieulaparenkorean;3211\nrieulcirclekorean;3263\nrieulhieuhkorean;3140\nrieulkiyeokkorean;313A\nrieulkiyeoksioskorean;3169\nrieulkorean;3139\nrieulmieumkorean;313B\nrieulpansioskorean;316C\nrieulparenkorean;3203\nrieulphieuphkorean;313F\nrieulpieupkorean;313C\nrieulpieupsioskorean;316B\nrieulsioskorean;313D\nrieulthieuthkorean;313E\nrieultikeutkorean;316A\nrieulyeorinhieuhkorean;316D\nrightangle;221F\nrighttackbelowcmb;0319\nrighttriangle;22BF\nrihiragana;308A\nrikatakana;30EA\nrikatakanahalfwidth;FF98\nring;02DA\nringbelowcmb;0325\nringcmb;030A\nringhalfleft;02BF\nringhalfleftarmenian;0559\nringhalfleftbelowcmb;031C\nringhalfleftcentered;02D3\nringhalfright;02BE\nringhalfrightbelowcmb;0339\nringhalfrightcentered;02D2\nrinvertedbreve;0213\nrittorusquare;3351\nrlinebelow;1E5F\nrlongleg;027C\nrlonglegturned;027A\nrmonospace;FF52\nrohiragana;308D\nrokatakana;30ED\nrokatakanahalfwidth;FF9B\nroruathai;0E23\nrparen;24AD\nrrabengali;09DC\nrradeva;0931\nrragurmukhi;0A5C\nrreharabic;0691\nrrehfinalarabic;FB8D\nrrvocalicbengali;09E0\nrrvocalicdeva;0960\nrrvocalicgujarati;0AE0\nrrvocalicvowelsignbengali;09C4\nrrvocalicvowelsigndeva;0944\nrrvocalicvowelsigngujarati;0AC4\nrsuperior;F6F1\nrtblock;2590\nrturned;0279\nrturnedsuperior;02B4\nruhiragana;308B\nrukatakana;30EB\nrukatakanahalfwidth;FF99\nrupeemarkbengali;09F2\nrupeesignbengali;09F3\nrupiah;F6DD\nruthai;0E24\nrvocalicbengali;098B\nrvocalicdeva;090B\nrvocalicgujarati;0A8B\nrvocalicvowelsignbengali;09C3\nrvocalicvowelsigndeva;0943\nrvocalicvowelsigngujarati;0AC3\ns;0073\nsabengali;09B8\nsacute;015B\nsacutedotaccent;1E65\nsadarabic;0635\nsadeva;0938\nsadfinalarabic;FEBA\nsadinitialarabic;FEBB\nsadmedialarabic;FEBC\nsagujarati;0AB8\nsagurmukhi;0A38\nsahiragana;3055\nsakatakana;30B5\nsakatakanahalfwidth;FF7B\nsallallahoualayhewasallamarabic;FDFA\nsamekh;05E1\nsamekhdagesh;FB41\nsamekhdageshhebrew;FB41\nsamekhhebrew;05E1\nsaraaathai;0E32\nsaraaethai;0E41\nsaraaimaimalaithai;0E44\nsaraaimaimuanthai;0E43\nsaraamthai;0E33\nsaraathai;0E30\nsaraethai;0E40\nsaraiileftthai;F886\nsaraiithai;0E35\nsaraileftthai;F885\nsaraithai;0E34\nsaraothai;0E42\nsaraueeleftthai;F888\nsaraueethai;0E37\nsaraueleftthai;F887\nsarauethai;0E36\nsarauthai;0E38\nsarauuthai;0E39\nsbopomofo;3119\nscaron;0161\nscarondotaccent;1E67\nscedilla;015F\nschwa;0259\nschwacyrillic;04D9\nschwadieresiscyrillic;04DB\nschwahook;025A\nscircle;24E2\nscircumflex;015D\nscommaaccent;0219\nsdotaccent;1E61\nsdotbelow;1E63\nsdotbelowdotaccent;1E69\nseagullbelowcmb;033C\nsecond;2033\nsecondtonechinese;02CA\nsection;00A7\nseenarabic;0633\nseenfinalarabic;FEB2\nseeninitialarabic;FEB3\nseenmedialarabic;FEB4\nsegol;05B6\nsegol13;05B6\nsegol1f;05B6\nsegol2c;05B6\nsegolhebrew;05B6\nsegolnarrowhebrew;05B6\nsegolquarterhebrew;05B6\nsegoltahebrew;0592\nsegolwidehebrew;05B6\nseharmenian;057D\nsehiragana;305B\nsekatakana;30BB\nsekatakanahalfwidth;FF7E\nsemicolon;003B\nsemicolonarabic;061B\nsemicolonmonospace;FF1B\nsemicolonsmall;FE54\nsemivoicedmarkkana;309C\nsemivoicedmarkkanahalfwidth;FF9F\nsentisquare;3322\nsentosquare;3323\nseven;0037\nsevenarabic;0667\nsevenbengali;09ED\nsevencircle;2466\nsevencircleinversesansserif;2790\nsevendeva;096D\nseveneighths;215E\nsevengujarati;0AED\nsevengurmukhi;0A6D\nsevenhackarabic;0667\nsevenhangzhou;3027\nsevenideographicparen;3226\nseveninferior;2087\nsevenmonospace;FF17\nsevenoldstyle;F737\nsevenparen;247A\nsevenperiod;248E\nsevenpersian;06F7\nsevenroman;2176\nsevensuperior;2077\nseventeencircle;2470\nseventeenparen;2484\nseventeenperiod;2498\nseventhai;0E57\nsfthyphen;00AD\nshaarmenian;0577\nshabengali;09B6\nshacyrillic;0448\nshaddaarabic;0651\nshaddadammaarabic;FC61\nshaddadammatanarabic;FC5E\nshaddafathaarabic;FC60\nshaddafathatanarabic;0651 064B\nshaddakasraarabic;FC62\nshaddakasratanarabic;FC5F\nshade;2592\nshadedark;2593\nshadelight;2591\nshademedium;2592\nshadeva;0936\nshagujarati;0AB6\nshagurmukhi;0A36\nshalshelethebrew;0593\nshbopomofo;3115\nshchacyrillic;0449\nsheenarabic;0634\nsheenfinalarabic;FEB6\nsheeninitialarabic;FEB7\nsheenmedialarabic;FEB8\nsheicoptic;03E3\nsheqel;20AA\nsheqelhebrew;20AA\nsheva;05B0\nsheva115;05B0\nsheva15;05B0\nsheva22;05B0\nsheva2e;05B0\nshevahebrew;05B0\nshevanarrowhebrew;05B0\nshevaquarterhebrew;05B0\nshevawidehebrew;05B0\nshhacyrillic;04BB\nshimacoptic;03ED\nshin;05E9\nshindagesh;FB49\nshindageshhebrew;FB49\nshindageshshindot;FB2C\nshindageshshindothebrew;FB2C\nshindageshsindot;FB2D\nshindageshsindothebrew;FB2D\nshindothebrew;05C1\nshinhebrew;05E9\nshinshindot;FB2A\nshinshindothebrew;FB2A\nshinsindot;FB2B\nshinsindothebrew;FB2B\nshook;0282\nsigma;03C3\nsigma1;03C2\nsigmafinal;03C2\nsigmalunatesymbolgreek;03F2\nsihiragana;3057\nsikatakana;30B7\nsikatakanahalfwidth;FF7C\nsiluqhebrew;05BD\nsiluqlefthebrew;05BD\nsimilar;223C\nsindothebrew;05C2\nsiosacirclekorean;3274\nsiosaparenkorean;3214\nsioscieuckorean;317E\nsioscirclekorean;3266\nsioskiyeokkorean;317A\nsioskorean;3145\nsiosnieunkorean;317B\nsiosparenkorean;3206\nsiospieupkorean;317D\nsiostikeutkorean;317C\nsix;0036\nsixarabic;0666\nsixbengali;09EC\nsixcircle;2465\nsixcircleinversesansserif;278F\nsixdeva;096C\nsixgujarati;0AEC\nsixgurmukhi;0A6C\nsixhackarabic;0666\nsixhangzhou;3026\nsixideographicparen;3225\nsixinferior;2086\nsixmonospace;FF16\nsixoldstyle;F736\nsixparen;2479\nsixperiod;248D\nsixpersian;06F6\nsixroman;2175\nsixsuperior;2076\nsixteencircle;246F\nsixteencurrencydenominatorbengali;09F9\nsixteenparen;2483\nsixteenperiod;2497\nsixthai;0E56\nslash;002F\nslashmonospace;FF0F\nslong;017F\nslongdotaccent;1E9B\nsmileface;263A\nsmonospace;FF53\nsofpasuqhebrew;05C3\nsofthyphen;00AD\nsoftsigncyrillic;044C\nsohiragana;305D\nsokatakana;30BD\nsokatakanahalfwidth;FF7F\nsoliduslongoverlaycmb;0338\nsolidusshortoverlaycmb;0337\nsorusithai;0E29\nsosalathai;0E28\nsosothai;0E0B\nsosuathai;0E2A\nspace;0020\nspacehackarabic;0020\nspade;2660\nspadesuitblack;2660\nspadesuitwhite;2664\nsparen;24AE\nsquarebelowcmb;033B\nsquarecc;33C4\nsquarecm;339D\nsquarediagonalcrosshatchfill;25A9\nsquarehorizontalfill;25A4\nsquarekg;338F\nsquarekm;339E\nsquarekmcapital;33CE\nsquareln;33D1\nsquarelog;33D2\nsquaremg;338E\nsquaremil;33D5\nsquaremm;339C\nsquaremsquared;33A1\nsquareorthogonalcrosshatchfill;25A6\nsquareupperlefttolowerrightfill;25A7\nsquareupperrighttolowerleftfill;25A8\nsquareverticalfill;25A5\nsquarewhitewithsmallblack;25A3\nsrsquare;33DB\nssabengali;09B7\nssadeva;0937\nssagujarati;0AB7\nssangcieuckorean;3149\nssanghieuhkorean;3185\nssangieungkorean;3180\nssangkiyeokkorean;3132\nssangnieunkorean;3165\nssangpieupkorean;3143\nssangsioskorean;3146\nssangtikeutkorean;3138\nssuperior;F6F2\nsterling;00A3\nsterlingmonospace;FFE1\nstrokelongoverlaycmb;0336\nstrokeshortoverlaycmb;0335\nsubset;2282\nsubsetnotequal;228A\nsubsetorequal;2286\nsucceeds;227B\nsuchthat;220B\nsuhiragana;3059\nsukatakana;30B9\nsukatakanahalfwidth;FF7D\nsukunarabic;0652\nsummation;2211\nsun;263C\nsuperset;2283\nsupersetnotequal;228B\nsupersetorequal;2287\nsvsquare;33DC\nsyouwaerasquare;337C\nt;0074\ntabengali;09A4\ntackdown;22A4\ntackleft;22A3\ntadeva;0924\ntagujarati;0AA4\ntagurmukhi;0A24\ntaharabic;0637\ntahfinalarabic;FEC2\ntahinitialarabic;FEC3\ntahiragana;305F\ntahmedialarabic;FEC4\ntaisyouerasquare;337D\ntakatakana;30BF\ntakatakanahalfwidth;FF80\ntatweelarabic;0640\ntau;03C4\ntav;05EA\ntavdages;FB4A\ntavdagesh;FB4A\ntavdageshhebrew;FB4A\ntavhebrew;05EA\ntbar;0167\ntbopomofo;310A\ntcaron;0165\ntccurl;02A8\ntcedilla;0163\ntcheharabic;0686\ntchehfinalarabic;FB7B\ntchehinitialarabic;FB7C\ntchehmedialarabic;FB7D\ntchehmeeminitialarabic;FB7C FEE4\ntcircle;24E3\ntcircumflexbelow;1E71\ntcommaaccent;0163\ntdieresis;1E97\ntdotaccent;1E6B\ntdotbelow;1E6D\ntecyrillic;0442\ntedescendercyrillic;04AD\nteharabic;062A\ntehfinalarabic;FE96\ntehhahinitialarabic;FCA2\ntehhahisolatedarabic;FC0C\ntehinitialarabic;FE97\ntehiragana;3066\ntehjeeminitialarabic;FCA1\ntehjeemisolatedarabic;FC0B\ntehmarbutaarabic;0629\ntehmarbutafinalarabic;FE94\ntehmedialarabic;FE98\ntehmeeminitialarabic;FCA4\ntehmeemisolatedarabic;FC0E\ntehnoonfinalarabic;FC73\ntekatakana;30C6\ntekatakanahalfwidth;FF83\ntelephone;2121\ntelephoneblack;260E\ntelishagedolahebrew;05A0\ntelishaqetanahebrew;05A9\ntencircle;2469\ntenideographicparen;3229\ntenparen;247D\ntenperiod;2491\ntenroman;2179\ntesh;02A7\ntet;05D8\ntetdagesh;FB38\ntetdageshhebrew;FB38\ntethebrew;05D8\ntetsecyrillic;04B5\ntevirhebrew;059B\ntevirlefthebrew;059B\nthabengali;09A5\nthadeva;0925\nthagujarati;0AA5\nthagurmukhi;0A25\nthalarabic;0630\nthalfinalarabic;FEAC\nthanthakhatlowleftthai;F898\nthanthakhatlowrightthai;F897\nthanthakhatthai;0E4C\nthanthakhatupperleftthai;F896\ntheharabic;062B\nthehfinalarabic;FE9A\nthehinitialarabic;FE9B\nthehmedialarabic;FE9C\nthereexists;2203\ntherefore;2234\ntheta;03B8\ntheta1;03D1\nthetasymbolgreek;03D1\nthieuthacirclekorean;3279\nthieuthaparenkorean;3219\nthieuthcirclekorean;326B\nthieuthkorean;314C\nthieuthparenkorean;320B\nthirteencircle;246C\nthirteenparen;2480\nthirteenperiod;2494\nthonangmonthothai;0E11\nthook;01AD\nthophuthaothai;0E12\nthorn;00FE\nthothahanthai;0E17\nthothanthai;0E10\nthothongthai;0E18\nthothungthai;0E16\nthousandcyrillic;0482\nthousandsseparatorarabic;066C\nthousandsseparatorpersian;066C\nthree;0033\nthreearabic;0663\nthreebengali;09E9\nthreecircle;2462\nthreecircleinversesansserif;278C\nthreedeva;0969\nthreeeighths;215C\nthreegujarati;0AE9\nthreegurmukhi;0A69\nthreehackarabic;0663\nthreehangzhou;3023\nthreeideographicparen;3222\nthreeinferior;2083\nthreemonospace;FF13\nthreenumeratorbengali;09F6\nthreeoldstyle;F733\nthreeparen;2476\nthreeperiod;248A\nthreepersian;06F3\nthreequarters;00BE\nthreequartersemdash;F6DE\nthreeroman;2172\nthreesuperior;00B3\nthreethai;0E53\nthzsquare;3394\ntihiragana;3061\ntikatakana;30C1\ntikatakanahalfwidth;FF81\ntikeutacirclekorean;3270\ntikeutaparenkorean;3210\ntikeutcirclekorean;3262\ntikeutkorean;3137\ntikeutparenkorean;3202\ntilde;02DC\ntildebelowcmb;0330\ntildecmb;0303\ntildecomb;0303\ntildedoublecmb;0360\ntildeoperator;223C\ntildeoverlaycmb;0334\ntildeverticalcmb;033E\ntimescircle;2297\ntipehahebrew;0596\ntipehalefthebrew;0596\ntippigurmukhi;0A70\ntitlocyrilliccmb;0483\ntiwnarmenian;057F\ntlinebelow;1E6F\ntmonospace;FF54\ntoarmenian;0569\ntohiragana;3068\ntokatakana;30C8\ntokatakanahalfwidth;FF84\ntonebarextrahighmod;02E5\ntonebarextralowmod;02E9\ntonebarhighmod;02E6\ntonebarlowmod;02E8\ntonebarmidmod;02E7\ntonefive;01BD\ntonesix;0185\ntonetwo;01A8\ntonos;0384\ntonsquare;3327\ntopatakthai;0E0F\ntortoiseshellbracketleft;3014\ntortoiseshellbracketleftsmall;FE5D\ntortoiseshellbracketleftvertical;FE39\ntortoiseshellbracketright;3015\ntortoiseshellbracketrightsmall;FE5E\ntortoiseshellbracketrightvertical;FE3A\ntotaothai;0E15\ntpalatalhook;01AB\ntparen;24AF\ntrademark;2122\ntrademarksans;F8EA\ntrademarkserif;F6DB\ntretroflexhook;0288\ntriagdn;25BC\ntriaglf;25C4\ntriagrt;25BA\ntriagup;25B2\nts;02A6\ntsadi;05E6\ntsadidagesh;FB46\ntsadidageshhebrew;FB46\ntsadihebrew;05E6\ntsecyrillic;0446\ntsere;05B5\ntsere12;05B5\ntsere1e;05B5\ntsere2b;05B5\ntserehebrew;05B5\ntserenarrowhebrew;05B5\ntserequarterhebrew;05B5\ntserewidehebrew;05B5\ntshecyrillic;045B\ntsuperior;F6F3\nttabengali;099F\nttadeva;091F\nttagujarati;0A9F\nttagurmukhi;0A1F\ntteharabic;0679\nttehfinalarabic;FB67\nttehinitialarabic;FB68\nttehmedialarabic;FB69\ntthabengali;09A0\ntthadeva;0920\ntthagujarati;0AA0\ntthagurmukhi;0A20\ntturned;0287\ntuhiragana;3064\ntukatakana;30C4\ntukatakanahalfwidth;FF82\ntusmallhiragana;3063\ntusmallkatakana;30C3\ntusmallkatakanahalfwidth;FF6F\ntwelvecircle;246B\ntwelveparen;247F\ntwelveperiod;2493\ntwelveroman;217B\ntwentycircle;2473\ntwentyhangzhou;5344\ntwentyparen;2487\ntwentyperiod;249B\ntwo;0032\ntwoarabic;0662\ntwobengali;09E8\ntwocircle;2461\ntwocircleinversesansserif;278B\ntwodeva;0968\ntwodotenleader;2025\ntwodotleader;2025\ntwodotleadervertical;FE30\ntwogujarati;0AE8\ntwogurmukhi;0A68\ntwohackarabic;0662\ntwohangzhou;3022\ntwoideographicparen;3221\ntwoinferior;2082\ntwomonospace;FF12\ntwonumeratorbengali;09F5\ntwooldstyle;F732\ntwoparen;2475\ntwoperiod;2489\ntwopersian;06F2\ntworoman;2171\ntwostroke;01BB\ntwosuperior;00B2\ntwothai;0E52\ntwothirds;2154\nu;0075\nuacute;00FA\nubar;0289\nubengali;0989\nubopomofo;3128\nubreve;016D\nucaron;01D4\nucircle;24E4\nucircumflex;00FB\nucircumflexbelow;1E77\nucyrillic;0443\nudattadeva;0951\nudblacute;0171\nudblgrave;0215\nudeva;0909\nudieresis;00FC\nudieresisacute;01D8\nudieresisbelow;1E73\nudieresiscaron;01DA\nudieresiscyrillic;04F1\nudieresisgrave;01DC\nudieresismacron;01D6\nudotbelow;1EE5\nugrave;00F9\nugujarati;0A89\nugurmukhi;0A09\nuhiragana;3046\nuhookabove;1EE7\nuhorn;01B0\nuhornacute;1EE9\nuhorndotbelow;1EF1\nuhorngrave;1EEB\nuhornhookabove;1EED\nuhorntilde;1EEF\nuhungarumlaut;0171\nuhungarumlautcyrillic;04F3\nuinvertedbreve;0217\nukatakana;30A6\nukatakanahalfwidth;FF73\nukcyrillic;0479\nukorean;315C\numacron;016B\numacroncyrillic;04EF\numacrondieresis;1E7B\numatragurmukhi;0A41\numonospace;FF55\nunderscore;005F\nunderscoredbl;2017\nunderscoremonospace;FF3F\nunderscorevertical;FE33\nunderscorewavy;FE4F\nunion;222A\nuniversal;2200\nuogonek;0173\nuparen;24B0\nupblock;2580\nupperdothebrew;05C4\nupsilon;03C5\nupsilondieresis;03CB\nupsilondieresistonos;03B0\nupsilonlatin;028A\nupsilontonos;03CD\nuptackbelowcmb;031D\nuptackmod;02D4\nuragurmukhi;0A73\nuring;016F\nushortcyrillic;045E\nusmallhiragana;3045\nusmallkatakana;30A5\nusmallkatakanahalfwidth;FF69\nustraightcyrillic;04AF\nustraightstrokecyrillic;04B1\nutilde;0169\nutildeacute;1E79\nutildebelow;1E75\nuubengali;098A\nuudeva;090A\nuugujarati;0A8A\nuugurmukhi;0A0A\nuumatragurmukhi;0A42\nuuvowelsignbengali;09C2\nuuvowelsigndeva;0942\nuuvowelsigngujarati;0AC2\nuvowelsignbengali;09C1\nuvowelsigndeva;0941\nuvowelsigngujarati;0AC1\nv;0076\nvadeva;0935\nvagujarati;0AB5\nvagurmukhi;0A35\nvakatakana;30F7\nvav;05D5\nvavdagesh;FB35\nvavdagesh65;FB35\nvavdageshhebrew;FB35\nvavhebrew;05D5\nvavholam;FB4B\nvavholamhebrew;FB4B\nvavvavhebrew;05F0\nvavyodhebrew;05F1\nvcircle;24E5\nvdotbelow;1E7F\nvecyrillic;0432\nveharabic;06A4\nvehfinalarabic;FB6B\nvehinitialarabic;FB6C\nvehmedialarabic;FB6D\nvekatakana;30F9\nvenus;2640\nverticalbar;007C\nverticallineabovecmb;030D\nverticallinebelowcmb;0329\nverticallinelowmod;02CC\nverticallinemod;02C8\nvewarmenian;057E\nvhook;028B\nvikatakana;30F8\nviramabengali;09CD\nviramadeva;094D\nviramagujarati;0ACD\nvisargabengali;0983\nvisargadeva;0903\nvisargagujarati;0A83\nvmonospace;FF56\nvoarmenian;0578\nvoicediterationhiragana;309E\nvoicediterationkatakana;30FE\nvoicedmarkkana;309B\nvoicedmarkkanahalfwidth;FF9E\nvokatakana;30FA\nvparen;24B1\nvtilde;1E7D\nvturned;028C\nvuhiragana;3094\nvukatakana;30F4\nw;0077\nwacute;1E83\nwaekorean;3159\nwahiragana;308F\nwakatakana;30EF\nwakatakanahalfwidth;FF9C\nwakorean;3158\nwasmallhiragana;308E\nwasmallkatakana;30EE\nwattosquare;3357\nwavedash;301C\nwavyunderscorevertical;FE34\nwawarabic;0648\nwawfinalarabic;FEEE\nwawhamzaabovearabic;0624\nwawhamzaabovefinalarabic;FE86\nwbsquare;33DD\nwcircle;24E6\nwcircumflex;0175\nwdieresis;1E85\nwdotaccent;1E87\nwdotbelow;1E89\nwehiragana;3091\nweierstrass;2118\nwekatakana;30F1\nwekorean;315E\nweokorean;315D\nwgrave;1E81\nwhitebullet;25E6\nwhitecircle;25CB\nwhitecircleinverse;25D9\nwhitecornerbracketleft;300E\nwhitecornerbracketleftvertical;FE43\nwhitecornerbracketright;300F\nwhitecornerbracketrightvertical;FE44\nwhitediamond;25C7\nwhitediamondcontainingblacksmalldiamond;25C8\nwhitedownpointingsmalltriangle;25BF\nwhitedownpointingtriangle;25BD\nwhiteleftpointingsmalltriangle;25C3\nwhiteleftpointingtriangle;25C1\nwhitelenticularbracketleft;3016\nwhitelenticularbracketright;3017\nwhiterightpointingsmalltriangle;25B9\nwhiterightpointingtriangle;25B7\nwhitesmallsquare;25AB\nwhitesmilingface;263A\nwhitesquare;25A1\nwhitestar;2606\nwhitetelephone;260F\nwhitetortoiseshellbracketleft;3018\nwhitetortoiseshellbracketright;3019\nwhiteuppointingsmalltriangle;25B5\nwhiteuppointingtriangle;25B3\nwihiragana;3090\nwikatakana;30F0\nwikorean;315F\nwmonospace;FF57\nwohiragana;3092\nwokatakana;30F2\nwokatakanahalfwidth;FF66\nwon;20A9\nwonmonospace;FFE6\nwowaenthai;0E27\nwparen;24B2\nwring;1E98\nwsuperior;02B7\nwturned;028D\nwynn;01BF\nx;0078\nxabovecmb;033D\nxbopomofo;3112\nxcircle;24E7\nxdieresis;1E8D\nxdotaccent;1E8B\nxeharmenian;056D\nxi;03BE\nxmonospace;FF58\nxparen;24B3\nxsuperior;02E3\ny;0079\nyaadosquare;334E\nyabengali;09AF\nyacute;00FD\nyadeva;092F\nyaekorean;3152\nyagujarati;0AAF\nyagurmukhi;0A2F\nyahiragana;3084\nyakatakana;30E4\nyakatakanahalfwidth;FF94\nyakorean;3151\nyamakkanthai;0E4E\nyasmallhiragana;3083\nyasmallkatakana;30E3\nyasmallkatakanahalfwidth;FF6C\nyatcyrillic;0463\nycircle;24E8\nycircumflex;0177\nydieresis;00FF\nydotaccent;1E8F\nydotbelow;1EF5\nyeharabic;064A\nyehbarreearabic;06D2\nyehbarreefinalarabic;FBAF\nyehfinalarabic;FEF2\nyehhamzaabovearabic;0626\nyehhamzaabovefinalarabic;FE8A\nyehhamzaaboveinitialarabic;FE8B\nyehhamzaabovemedialarabic;FE8C\nyehinitialarabic;FEF3\nyehmedialarabic;FEF4\nyehmeeminitialarabic;FCDD\nyehmeemisolatedarabic;FC58\nyehnoonfinalarabic;FC94\nyehthreedotsbelowarabic;06D1\nyekorean;3156\nyen;00A5\nyenmonospace;FFE5\nyeokorean;3155\nyeorinhieuhkorean;3186\nyerahbenyomohebrew;05AA\nyerahbenyomolefthebrew;05AA\nyericyrillic;044B\nyerudieresiscyrillic;04F9\nyesieungkorean;3181\nyesieungpansioskorean;3183\nyesieungsioskorean;3182\nyetivhebrew;059A\nygrave;1EF3\nyhook;01B4\nyhookabove;1EF7\nyiarmenian;0575\nyicyrillic;0457\nyikorean;3162\nyinyang;262F\nyiwnarmenian;0582\nymonospace;FF59\nyod;05D9\nyoddagesh;FB39\nyoddageshhebrew;FB39\nyodhebrew;05D9\nyodyodhebrew;05F2\nyodyodpatahhebrew;FB1F\nyohiragana;3088\nyoikorean;3189\nyokatakana;30E8\nyokatakanahalfwidth;FF96\nyokorean;315B\nyosmallhiragana;3087\nyosmallkatakana;30E7\nyosmallkatakanahalfwidth;FF6E\nyotgreek;03F3\nyoyaekorean;3188\nyoyakorean;3187\nyoyakthai;0E22\nyoyingthai;0E0D\nyparen;24B4\nypogegrammeni;037A\nypogegrammenigreekcmb;0345\nyr;01A6\nyring;1E99\nysuperior;02B8\nytilde;1EF9\nyturned;028E\nyuhiragana;3086\nyuikorean;318C\nyukatakana;30E6\nyukatakanahalfwidth;FF95\nyukorean;3160\nyusbigcyrillic;046B\nyusbigiotifiedcyrillic;046D\nyuslittlecyrillic;0467\nyuslittleiotifiedcyrillic;0469\nyusmallhiragana;3085\nyusmallkatakana;30E5\nyusmallkatakanahalfwidth;FF6D\nyuyekorean;318B\nyuyeokorean;318A\nyyabengali;09DF\nyyadeva;095F\nz;007A\nzaarmenian;0566\nzacute;017A\nzadeva;095B\nzagurmukhi;0A5B\nzaharabic;0638\nzahfinalarabic;FEC6\nzahinitialarabic;FEC7\nzahiragana;3056\nzahmedialarabic;FEC8\nzainarabic;0632\nzainfinalarabic;FEB0\nzakatakana;30B6\nzaqefgadolhebrew;0595\nzaqefqatanhebrew;0594\nzarqahebrew;0598\nzayin;05D6\nzayindagesh;FB36\nzayindageshhebrew;FB36\nzayinhebrew;05D6\nzbopomofo;3117\nzcaron;017E\nzcircle;24E9\nzcircumflex;1E91\nzcurl;0291\nzdot;017C\nzdotaccent;017C\nzdotbelow;1E93\nzecyrillic;0437\nzedescendercyrillic;0499\nzedieresiscyrillic;04DF\nzehiragana;305C\nzekatakana;30BC\nzero;0030\nzeroarabic;0660\nzerobengali;09E6\nzerodeva;0966\nzerogujarati;0AE6\nzerogurmukhi;0A66\nzerohackarabic;0660\nzeroinferior;2080\nzeromonospace;FF10\nzerooldstyle;F730\nzeropersian;06F0\nzerosuperior;2070\nzerothai;0E50\nzerowidthjoiner;FEFF\nzerowidthnonjoiner;200C\nzerowidthspace;200B\nzeta;03B6\nzhbopomofo;3113\nzhearmenian;056A\nzhebrevecyrillic;04C2\nzhecyrillic;0436\nzhedescendercyrillic;0497\nzhedieresiscyrillic;04DD\nzihiragana;3058\nzikatakana;30B8\nzinorhebrew;05AE\nzlinebelow;1E95\nzmonospace;FF5A\nzohiragana;305E\nzokatakana;30BE\nzparen;24B5\nzretroflexhook;0290\nzstroke;01B6\nzuhiragana;305A\nzukatakana;30BA";
            foreach (string f in data.Split('\n')) 
            {
                int ii = f.IndexOf(';');
                if (ii < 0) 
                    continue;
                string key = f.Substring(0, ii);
                int code = 0;
                for (++ii; ii < f.Length; ii++) 
                {
                    if (char.IsDigit(f[ii])) 
                        code = ((code * 16) + ((int)f[ii])) - 0x30;
                    else if ("ABCDEF".IndexOf(f[ii]) >= 0) 
                        code = (((code * 16) + ((int)f[ii])) - 0x41) + 10;
                    else 
                        break;
                }
                m_Diffs.Add(key, code);
            }
        }
    }
}