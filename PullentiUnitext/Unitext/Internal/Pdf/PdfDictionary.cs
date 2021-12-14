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
    public class PdfDictionary : PdfObject
    {
        Dictionary<string, PdfObject> m_Items = new Dictionary<string, PdfObject>();
        private byte[] m_StreamData = null;
        private PdfStream m_StreamIn = null;
        private int m_StreamPos = 0;
        private int m_StreamLength = 0;
        internal int m_FilePos;
        public byte[] Data
        {
            get
            {
                if (m_StreamData == null) 
                {
                    if (m_StreamLength == 0) 
                        return null;
                    m_StreamData = new byte[(int)m_StreamLength];
                    m_StreamIn.Position = m_StreamPos;
                    m_StreamIn.Read(m_StreamData, 0, m_StreamLength);
                }
                return m_StreamData;
            }
            set
            {
                m_StreamData = value;
            }
        }
        public List<string> Keys
        {
            get
            {
                return new List<string>(m_Items.Keys);
            }
        }
        public override bool IsSimple(int lev)
        {
            if (m_Items.Count > 10 || lev > 10) 
                return false;
            foreach (string key in Keys) 
            {
                if (key == "Parent") 
                    continue;
                PdfObject it = this.GetObject(key, false);
                if (it != null && !it.IsSimple(lev + 1)) 
                    return false;
            }
            return true;
        }
        public override string ToString(int lev)
        {
            if (lev > 5) 
                return string.Format("<<...{0}>>", m_Items.Count);
            StringBuilder res = new StringBuilder();
            PdfObject it = this.GetObject("Type", false);
            if (it != null) 
                res.Append(it.ToString());
            res.Append("<<");
            bool fi = true;
            bool partial = false;
            foreach (string key in Keys) 
            {
                if (key == "Parent" || key == "Type") 
                    continue;
                it = this.GetObject(key, false);
                if (it == null) 
                {
                    PdfObject indir = m_Items[key];
                    if (!fi) 
                        res.Append(", ");
                    else 
                        fi = false;
                    res.AppendFormat("{0}={1}", key, (indir == null ? "NULL" : indir.ToString()));
                    if (res.Length > 100) 
                        break;
                    continue;
                }
                if (!it.IsSimple(lev + 1)) 
                {
                    partial = true;
                    continue;
                }
                if (!fi) 
                    res.Append(", ");
                else 
                    fi = false;
                if (res.Length > 100) 
                {
                    partial = true;
                    break;
                }
                string str = it.ToString(lev + 1);
                if (str.Length > 50) 
                    str = str.Substring(0, 50) + "...";
                res.AppendFormat("{0}={1}", key, str);
            }
            if (partial) 
                res.AppendFormat("...{0}", m_Items.Count);
            res.Append(">>");
            if (m_StreamLength > 0) 
                res.AppendFormat(" DATA:{0}", m_StreamLength);
            return res.ToString();
        }
        public bool HasDataStream
        {
            get
            {
                return m_StreamLength > 0;
            }
        }
        internal void PostParse(PdfStream stream)
        {
            m_Items.Clear();
            m_StreamIn = null;
            m_StreamLength = 0;
            m_StreamData = null;
            int p0 = (int)stream.Position;
            byte ch;
            int i;
            string str;
            while ((((i = stream.PeekSolidByte()))) >= 0) 
            {
                ch = (byte)i;
                if (ch == '/') 
                {
                    str = PdfStringValue.GetStringByBytes(stream.ReadName());
                    if (str == null) 
                        break;
                    if (str == "Contents") 
                    {
                    }
                    PdfObject obj = stream.ParseObject(SourceFile, false);
                    if (obj == null) 
                        break;
                    if (!m_Items.ContainsKey(str)) 
                        m_Items.Add(str, obj);
                    continue;
                }
                if (ch != '>') 
                    break;
                stream.Position++;
                ch = (byte)stream.ReadByte();
                if (ch != '>') 
                    break;
                this._parseStream(stream);
                return;
            }
        }
        void _parseStream(PdfStream stream)
        {
            int p0 = (int)stream.Position;
            string str = stream.ReadWord(false);
            if (str != "stream") 
            {
                stream.Position = p0;
                return;
            }
            byte[] StreamTag = new byte[(int)6];
            byte ch = (byte)stream.ReadByte();
            if (ch != 0xD) 
                stream.Position--;
            ch = (byte)stream.ReadByte();
            if (ch != 0xA) 
                stream.Position--;
            m_StreamIn = stream;
            m_StreamPos = stream.Position;
            m_StreamLength = (int)this.GetIntItem("Length");
            if (m_StreamLength > 0 && ((m_StreamPos + m_StreamLength) < stream.Length)) 
                stream.Position = m_StreamPos + m_StreamLength;
            int len = 0;
            int i;
            while ((((i = stream.ReadByte()))) >= 0) 
            {
                ch = (byte)i;
                len++;
                if (ch != 'e') 
                    continue;
                ch = (byte)stream.ReadByte();
                len++;
                if (ch != 'n') 
                    continue;
                ch = (byte)stream.ReadByte();
                len++;
                if (ch != 'd') 
                    continue;
                p0 = stream.Position;
                stream.Read(StreamTag, 0, 6);
                if (PdfStringValue.GetStringByBytes(StreamTag) == "stream") 
                {
                    len -= 3;
                    break;
                }
                stream.Position = p0;
            }
            if (m_StreamLength == 0) 
                m_StreamLength = len;
        }
        public string GetStringItem(string Key)
        {
            PdfObject val = this.GetObject(Key, false);
            while (true) 
            {
                if (val == null) 
                    return null;
                if (val is PdfName) 
                    return (val as PdfName).Name;
                if (val is PdfBoolValue) 
                    return ((val as PdfBoolValue).Val ? "true" : "false");
                if (val is PdfStringValue) 
                    return PdfStringValue.GetStringByBytes((val as PdfStringValue).Val);
                if (val is PdfIntValue) 
                    return (val as PdfIntValue).Val.ToString();
                if (val is PdfArray) 
                {
                    val = (val as PdfArray).GetItem(0);
                    if (val == null) 
                        break;
                    continue;
                }
                break;
            }
            return null;
        }
        public bool IsTypeItem(string typeVal)
        {
            return this.GetStringItem("Type") == typeVal;
        }
        public int GetIntItem(string key)
        {
            PdfObject val = this.GetObject(key, false);
            if (val == null) 
                return 0;
            if (val is PdfIntValue) 
                return (val as PdfIntValue).Val;
            if (val is PdfRealValue) 
                return (int)(val as PdfRealValue).GetDouble();
            return 0;
        }
        public PdfObject GetObject(string key, bool keepRef = false)
        {
            PdfObject res = null;
            if (!m_Items.TryGetValue(key, out res)) 
                return null;
            if ((res is PdfReference) && !keepRef) 
            {
                res = SourceFile.GetObject(res.Id);
                if (res == null) 
                    return null;
                m_Items[key] = res;
            }
            return res;
        }
        public PdfDictionary GetDictionary(string key, string typVal = null)
        {
            PdfObject obj = this.GetObject(key, false);
            if (obj is PdfArray) 
                obj = (obj as PdfArray).GetItem(0);
            if (obj is PdfDictionary) 
            {
                PdfDictionary res = obj as PdfDictionary;
                if (typVal != null) 
                {
                    if (!res.IsTypeItem(typVal)) 
                        return null;
                }
                return res;
            }
            return null;
        }
        public void GetAllPages(List<PdfDictionary> res)
        {
            if (this.IsTypeItem("Page")) 
            {
                res.Add(this);
                return;
            }
            PdfDictionary dic = this.GetDictionary("Pages", "Pages");
            if (dic != null) 
                dic.GetAllPages(res);
            else 
            {
                PdfArray kids = this.GetObject("Kids", false) as PdfArray;
                if (kids != null) 
                {
                    for (int i = 0; i < kids.ItemsCount; i++) 
                    {
                        PdfObject it = kids.GetItem(i);
                        if (it is PdfDictionary) 
                            (it as PdfDictionary).GetAllPages(res);
                    }
                }
            }
        }
        public byte[] ExtractData()
        {
            byte[] StreamData = Data;
            if (StreamData == null) 
                return new byte[(int)0];
            string str = this.GetStringItem("Filter");
            if (string.IsNullOrEmpty(str)) 
                return StreamData;
            if (str != "FlateDecode") 
                return null;
            if (StreamData.Length < 10) 
                return null;
            byte[] res = Pullenti.Util.ArchiveHelper.DecompressZlib(StreamData);
            if (res == null) 
                return null;
            PdfDictionary dparms = this.GetDictionary("DecodeParms", null);
            int predict = 0;
            int N = 0;
            int columns = 0;
            if (dparms != null) 
            {
                PdfIntValue iv = dparms.GetObject("Predictor", false) as PdfIntValue;
                if (iv != null && iv.Val > 1) 
                    predict = iv.Val;
                iv = dparms.GetObject("Columns", false) as PdfIntValue;
                if (iv != null) 
                    columns = iv.Val;
                iv = dparms.GetObject("Colors", false) as PdfIntValue;
                if (iv != null) 
                    N = iv.Val;
            }
            if ((predict >= 10 && predict <= 15 && N > 0) && columns > 0) 
            {
                int rowSize = columns * N;
                if (predict == 15) 
                    rowSize += 1;
                for (int pos0 = 0; (pos0 + rowSize) < res.Length; pos0 += rowSize) 
                {
                    int cod = predict - 10;
                    int pos = pos0;
                    if (predict == 15) 
                    {
                        cod = res[pos0];
                        pos++;
                    }
                    for (int j = pos; j < (pos0 + rowSize); j++) 
                    {
                        byte valLeft = (byte)0;
                        byte valTop = (byte)0;
                        byte valCorner = (byte)0;
                        int k = j - N;
                        int kk = j - rowSize;
                        if (kk >= 0) 
                            valTop = res[kk];
                        if (k >= pos) 
                        {
                            valLeft = res[k];
                            if (kk >= 0) 
                                valCorner = res[kk - N];
                        }
                        res[j] = Pullenti.Unitext.Internal.Misc.PngWrapper.FilterByte(cod, false, res[j], valLeft, valTop, valCorner);
                    }
                }
                if (predict == 15) 
                {
                    int p = 0;
                    for (int i = 0; i < res.Length; i++) 
                    {
                        if (((i % rowSize)) == 0) 
                        {
                        }
                        else 
                        {
                            res[p] = res[i];
                            p++;
                        }
                    }
                    Array.Resize(ref res, p);
                }
            }
            return res;
        }
        public byte[] GetTotalDataStream(string key)
        {
            PdfObject obj = this.GetObject(key, false);
            if (obj == null) 
                return null;
            if (obj is PdfDictionary) 
                return ((PdfDictionary)obj).ExtractData();
            PdfArray arr = obj as PdfArray;
            if (arr == null) 
                return null;
            List<byte> res = null;
            for (int i = 0; i < arr.ItemsCount; i++) 
            {
                obj = arr.GetItem(i);
                if (obj == null) 
                    continue;
                if (!(obj is PdfDictionary)) 
                    continue;
                byte[] tmp = ((PdfDictionary)obj).ExtractData();
                if (tmp == null) 
                    continue;
                if (tmp.Length < 1) 
                    continue;
                if (res == null) 
                    res = new List<byte>(tmp);
                else 
                {
                    res.Add(0xD);
                    res.Add(0xA);
                    res.AddRange(tmp);
                }
            }
            return res.ToArray();
        }
    }
}