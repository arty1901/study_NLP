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

namespace Pullenti.Unitext.Internal.Html
{
    class EmlPart
    {
        public Dictionary<string, string> Attrs = new Dictionary<string, string>();
        public byte[] Data;
        public string StringData;
        public string ContentType;
        public string ContentCharset;
        public string ContentLocation;
        public string ContentId;
        public string Filename;
        public string BoundaryRef;
        public string BoundaryId;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (BoundaryId == null) 
                tmp.Append("(*) ");
            tmp.AppendFormat("Attrs: {0}, String: {1}, Data: {2}", Attrs.Count, (StringData == null ? 0 : StringData.Length), (Data == null ? 0 : Data.Length));
            if (ContentType != null) 
                tmp.AppendFormat(" {0}", ContentType);
            if (ContentCharset != null) 
                tmp.AppendFormat(" {0}", ContentCharset);
            if (StringData != null) 
            {
                tmp.Append("  ");
                if (StringData.Length < 40) 
                    tmp.Append(StringData);
                else 
                    tmp.Append(StringData.Substring(0, 40) + "...");
            }
            return tmp.ToString();
        }
        public static EmlPart TryParse(string txt, ref int pos)
        {
            for (; pos < txt.Length; pos++) 
            {
                if (!char.IsWhiteSpace(txt[pos])) 
                    break;
            }
            if ((pos + 1) >= txt.Length) 
                return null;
            EmlPart res = new EmlPart();
            if (txt[pos] == '-' && txt[pos + 1] == '-') 
            {
                int p0 = pos + 2;
                for (pos += 2; pos < txt.Length; pos++) 
                {
                    if (char.IsWhiteSpace(txt[pos])) 
                        break;
                }
                res.BoundaryId = txt.Substring(p0, pos - p0);
                for (; pos < txt.Length; pos++) 
                {
                    if (!char.IsWhiteSpace(txt[pos])) 
                        break;
                }
            }
            StringBuilder attrValue = new StringBuilder();
            for (; pos < txt.Length; pos++) 
            {
                int p0 = pos;
                bool isAttr = false;
                for (; pos < txt.Length; pos++) 
                {
                    if (char.IsWhiteSpace(txt[pos])) 
                        break;
                    else if (txt[pos] == ':') 
                    {
                        isAttr = true;
                        break;
                    }
                }
                if (!isAttr) 
                    break;
                string attrName = txt.Substring(p0, pos - p0).Trim();
                if (attrName == "From") 
                {
                }
                pos++;
                attrValue.Length = 0;
                for (; pos < txt.Length; pos++) 
                {
                    char ch = txt[pos];
                    if (ch == ' ' || ch == 9) 
                    {
                        if (attrValue.Length == 0 || attrValue[attrValue.Length - 1] == ' ') 
                            continue;
                        attrValue.Append(' ');
                        continue;
                    }
                    if (ch != 0xD && ch != 0xA) 
                    {
                        attrValue.Append(ch);
                        continue;
                    }
                    if (((pos + 1) < txt.Length) && ch == 0xD && txt[pos + 1] == 0xA) 
                        pos++;
                    if (((pos + 1) < txt.Length) && ((txt[pos + 1] == 0xD || txt[pos + 1] == 0xA || !char.IsWhiteSpace(txt[pos + 1])))) 
                        break;
                }
                if (!res.Attrs.ContainsKey(attrName)) 
                    res.Attrs.Add(attrName, MhtHelper._decodeString(attrValue.ToString()));
            }
            string transfEnc = null;
            foreach (KeyValuePair<string, string> a in res.Attrs) 
            {
                if (string.Compare(a.Key, "CONTENT-TYPE", true) == 0) 
                {
                    res.ContentType = a.Value;
                    int i = a.Value.IndexOf(';');
                    if (i > 0) 
                        res.ContentType = res.ContentType.Substring(0, i);
                    res.ContentType = MhtHelper._corrString(res.ContentType);
                    i = a.Value.IndexOf("boundary=");
                    if (i > 0) 
                        res.BoundaryRef = MhtHelper._corrString(a.Value.Substring(i + 9).Trim());
                    i = a.Value.IndexOf("charset=");
                    if (i > 0) 
                        res.ContentCharset = MhtHelper._corrString(a.Value.Substring(i + 8).Trim());
                }
                else if (string.Compare(a.Key, "CONTENT-TRANSFER-ENCODING", true) == 0) 
                    transfEnc = MhtHelper._corrString(a.Value).ToLower();
                else if (string.Compare(a.Key, "CONTENT-ID", true) == 0) 
                {
                    res.ContentId = MhtHelper._corrString(a.Value) ?? "";
                    if (res.ContentId.Length > 2 && res.ContentId[0] == '<') 
                        res.ContentId = res.ContentId.Substring(1, res.ContentId.Length - 2);
                }
                else if (string.Compare(a.Key, "CONTENT-LOCATION", true) == 0) 
                    res.ContentLocation = MhtHelper._corrString(a.Value);
                else if (string.Compare(a.Key, "CONTENT-DISPOSITION", true) == 0) 
                {
                    int i = a.Value.IndexOf("filename=");
                    if (i > 0) 
                        res.Filename = MhtHelper._corrString(a.Value.Substring(i + 9).Trim());
                }
            }
            if (transfEnc == null && ((res.ContentType == "text/plain" || res.ContentType == "text/html"))) 
                transfEnc = "7bit";
            if (transfEnc == null) 
                return res;
            bool isQP = transfEnc == "quoted-printable";
            bool is64 = transfEnc == "base64";
            if (res.BoundaryId == null) 
            {
                if (!isQP && !is64) 
                    return res;
            }
            Pullenti.Util.EncodingWrapper enc = null;
            if (isQP && res.ContentCharset != null) 
            {
                try 
                {
                    enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, res.ContentCharset);
                }
                catch(Exception ex) 
                {
                }
            }
            if (isQP && enc == null) 
                enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.ACSII);
            List<byte> buf = new List<byte>();
            StringBuilder data = new StringBuilder();
            for (; pos < txt.Length; pos++) 
            {
                char ch = txt[pos];
                if ((ch == '-' && ((pos + 1) < txt.Length) && txt[pos + 1] == '-') && ((txt[pos - 1] == 0xD || txt[pos - 1] == 0xA)) && !string.IsNullOrEmpty(res.BoundaryId)) 
                {
                    int j;
                    for (j = 0; (j < res.BoundaryId.Length) && ((pos + 2 + j) < txt.Length); j++) 
                    {
                        if (res.BoundaryId[j] != txt[pos + 2 + j]) 
                            break;
                    }
                    if (j >= res.BoundaryId.Length) 
                    {
                        if (((pos + 4 + j) < txt.Length) && txt[pos + j + 2] == '-' && txt[pos + j + 3] == '-') 
                            pos += (j + 4);
                        break;
                    }
                }
                if (isQP && ch == '=' && ((pos + 2) < txt.Length)) 
                {
                    if (_toInt(txt[pos + 1]) >= 0 && _toInt(txt[pos + 2]) >= 0) 
                    {
                        int k = (_toInt(txt[pos + 1]) * 16) + _toInt(txt[pos + 2]);
                        buf.Add((byte)k);
                        pos += 2;
                        continue;
                    }
                    if (txt[pos + 1] == 0xD || txt[pos + 1] == 0xA) 
                    {
                        pos++;
                        if (txt[pos + 1] == 0xD || txt[pos + 1] == 0xA) 
                            pos++;
                        continue;
                    }
                }
                if (is64 && char.IsWhiteSpace(ch)) 
                    continue;
                if (isQP) 
                {
                    if (buf.Count > 0) 
                    {
                        data.Append(enc.GetString(buf.ToArray(), 0, -1));
                        buf.Clear();
                    }
                    data.Append(ch);
                }
                else 
                    data.Append(ch);
            }
            if (is64) 
            {
                if (data.Length > 0) 
                {
                    string str = data.ToString();
                    int ii = str.IndexOf("--");
                    if (ii > 0) 
                        str = str.Substring(0, ii);
                    try 
                    {
                        res.Data = Convert.FromBase64String(str);
                    }
                    catch(Exception ex) 
                    {
                    }
                }
            }
            else if (isQP) 
            {
                if (buf.Count > 0) 
                {
                    data.Append(enc.GetString(buf.ToArray(), 0, -1));
                    buf.Clear();
                }
                if (data.Length > 0) 
                    res.StringData = data.ToString();
            }
            else 
            {
                if (transfEnc != "7bit") 
                {
                }
                res.StringData = data.ToString();
            }
            return res;
        }
        internal static int _toInt(char ch)
        {
            if (ch >= '0' && ch <= '9') 
                return ch - '0';
            if (ch >= 'A' && ch <= 'F') 
                return 10 + ((int)((ch - 'A')));
            if (ch >= 'a' && ch <= 'f') 
                return 10 + ((int)((ch - 'a')));
            return -1;
        }
    }
}