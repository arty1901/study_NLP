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

namespace Pullenti.Unitext.Internal.Html
{
    class MhtPart
    {
        public string ContentType;
        public string ContentEncoding;
        public string ContentLocation;
        public Dictionary<string, string> Attrs = new Dictionary<string, string>();
        public byte[] Data;
        public string StringData;
        public MhtPart(string src)
        {
            int i = 0;
            int crlf = 0;
            for (i = 0; i < src.Length; i++) 
            {
                if (src[i] == 0xD) 
                {
                    crlf++;
                    if (((i + 1) < src.Length) && src[i + 1] == 0xA) 
                        i++;
                }
                else if (src[i] == 0xA) 
                    crlf++;
                else 
                    crlf = 0;
                if (crlf >= 2) 
                    break;
            }
            if ((crlf < 2) || i >= src.Length) 
                return;
            string head = src.Substring(0, i).Trim();
            int j;
            while (head.Length > 0) 
            {
                if (!head.StartsWith("Content-")) 
                    break;
                head = head.Substring(8);
                j = head.IndexOf("Content-");
                string rec = (j < 0 ? head : head.Substring(0, j).Trim());
                if (rec.StartsWith("Location:")) 
                    ContentLocation = rec.Substring(9).Trim();
                else if (rec.StartsWith("Transfer-Encoding:")) 
                    ContentEncoding = rec.Substring("Transfer-Encoding:".Length).Trim();
                else if (rec.StartsWith("Type:")) 
                    ContentType = rec.Substring(5).Trim();
                if (j < 0) 
                    break;
                head = head.Substring(j);
            }
            if (string.Compare(ContentEncoding, "base64", true) == 0) 
                Data = Convert.FromBase64String(src.Substring(i).Trim());
            else if (string.Compare(ContentEncoding, "quoted-printable", true) == 0) 
            {
                StringBuilder tmp = new StringBuilder(src.Length);
                for (j = i; j < src.Length; j++) 
                {
                    if (src[j] != '=') 
                        tmp.Append(src[j]);
                    else 
                    {
                        if (((j + 2) < src.Length) && _toInt(src[j + 1]) >= 0 && _toInt(src[j + 2]) >= 0) 
                        {
                            int k = (_toInt(src[j + 1]) * 16) + _toInt(src[j + 2]);
                            tmp.Append((char)k);
                            j += 2;
                            continue;
                        }
                        for (++j; j < src.Length; j++) 
                        {
                            if (src[j] != ' ') 
                                break;
                        }
                        for (; j < src.Length; j++) 
                        {
                            if (src[j] != 0xD && src[j] != 0xA) 
                                break;
                        }
                        j--;
                    }
                }
                StringData = tmp.ToString();
            }
        }
        static int _toInt(char ch)
        {
            if (ch >= '0' && ch <= '9') 
                return ch - '0';
            if (ch >= 'A' && ch <= 'F') 
                return 10 + ((int)((ch - 'A')));
            if (ch >= 'a' && ch <= 'f') 
                return 10 + ((int)((ch - 'a')));
            return -1;
        }
        public static List<MhtPart> ParseAllStr(string all, int i0, string bound, bool first)
        {
            List<MhtPart> res = new List<MhtPart>();
            int beg = 0;
            for (int i = i0; i < all.Length; i++) 
            {
                int j;
                for (j = 0; j < bound.Length; j++) 
                {
                    if (bound[j] != all[i + j]) 
                        break;
                }
                if (j < bound.Length) 
                    continue;
                if (beg > 0) 
                {
                    MhtPart pa = new MhtPart(all.Substring(beg, i - beg - 2).Trim());
                    if (pa.ContentLocation != null) 
                        res.Add(pa);
                    if (first) 
                        break;
                }
                i += bound.Length;
                for (; i < all.Length; i++) 
                {
                    if (!char.IsWhiteSpace(all[i])) 
                        break;
                }
                beg = i;
            }
            return res;
        }
        public static List<MhtPart> ParseAll(FileStream fs, byte[] bound)
        {
            return null;
        }
    }
}