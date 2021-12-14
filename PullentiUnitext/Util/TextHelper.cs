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

namespace Pullenti.Util
{
    // Различные утилитки работы с текстами
    public static class TextHelper
    {
        public static IList<string> GetWords(string text)
        {
            if (text == null) 
                return null;
            List<string> res = new List<string>();
            for (int i = 0; i < text.Length; i++) 
            {
                if (char.IsWhiteSpace(text[i])) 
                    continue;
                int j;
                for (j = i; j < text.Length; j++) 
                {
                    if (char.IsWhiteSpace(text[j])) 
                        break;
                }
                res.Add(text.Substring(i, j - i));
                i = j;
            }
            return res;
        }
        public static string CorrectWhitespaces(string txt)
        {
            if (txt == null) 
                return null;
            int i;
            for (i = 0; i < txt.Length; i++) 
            {
                if (txt[i] == 0xD) 
                {
                    if ((i + 1) >= txt.Length) 
                        break;
                    if (txt[i + 1] != 0xA) 
                        break;
                }
                else if (txt[i] == 0xA) 
                {
                    if (i == 0) 
                        break;
                    if (txt[i - 1] != 0xD) 
                        break;
                }
                else if (txt[i] == 0xA0) 
                    break;
            }
            if (i >= txt.Length) 
                return txt;
            StringBuilder res = new StringBuilder(txt.Length);
            for (i = 0; i < txt.Length; i++) 
            {
                if (txt[i] == 0xD) 
                {
                    res.Append("\r\n");
                    if (((i + 1) < txt.Length) && txt[i + 1] == 0xA) 
                        i++;
                }
                else if (txt[i] == 0xA) 
                    res.Append("\r\n");
                else if (txt[i] == 0xA0) 
                    res.Append(' ');
                else 
                    res.Append(txt[i]);
            }
            string resTxt = res.ToString();
            for (i = 0; i < (resTxt.Length - 1); i++) 
            {
                if (resTxt[i] == 0xD && resTxt[i + 1] == 0xD) 
                {
                }
            }
            return resTxt;
        }
        public static string CorrectNewlinesForParagraphs(string txt, out int count)
        {
            count = 0;
            if (string.IsNullOrEmpty(txt)) 
                return txt;
            int i;
            int j;
            int cou = 0;
            int totalLen = 0;
            for (i = 0; i < txt.Length; i++) 
            {
                char ch = txt[i];
                if (ch != 0xD && ch != 0xA && ch != '\f') 
                    continue;
                int len = 0;
                char lastChar = ch;
                for (j = i + 1; j < txt.Length; j++) 
                {
                    ch = txt[j];
                    if (ch == 0xD || ch == 0xA || ch == '\f') 
                        break;
                    else if (ch == 0x9) 
                        len += 5;
                    else 
                    {
                        lastChar = ch;
                        len++;
                    }
                }
                if (j >= txt.Length) 
                    break;
                if (len < 30) 
                    continue;
                if (len > 200) 
                    return txt;
                if (lastChar != '.' && lastChar != ':' && lastChar != ';') 
                {
                    bool nextIsDig = false;
                    for (int k = j + 1; k < txt.Length; k++) 
                    {
                        if (!char.IsWhiteSpace(txt[k])) 
                        {
                            if (char.IsDigit(txt[k])) 
                                nextIsDig = true;
                            break;
                        }
                    }
                    if (!nextIsDig) 
                    {
                        cou++;
                        totalLen += len;
                    }
                }
                i = j;
            }
            if (cou < 4) 
                return txt;
            totalLen /= cou;
            if ((totalLen < 50) || totalLen > 100) 
                return txt;
            StringBuilder tmp = new StringBuilder(txt);
            for (i = 0; i < tmp.Length; i++) 
            {
                char ch = tmp[i];
                int jj;
                int len = 0;
                char lastChar = ch;
                for (j = i + 1; j < tmp.Length; j++) 
                {
                    ch = tmp[j];
                    if (ch == 0xD || ch == 0xA || ch == '\f') 
                        break;
                    else if (ch == 0x9) 
                        len += 5;
                    else 
                    {
                        lastChar = ch;
                        len++;
                    }
                }
                if (j >= tmp.Length) 
                    break;
                for (jj = j - 1; jj >= 0; jj--) 
                {
                    if (!char.IsWhiteSpace((lastChar = tmp[jj]))) 
                        break;
                }
                bool notSingle = false;
                jj = j + 1;
                if ((jj < tmp.Length) && tmp[j] == 0xD && tmp[jj] == 0xA) 
                    jj++;
                for (; jj < tmp.Length; jj++) 
                {
                    ch = tmp[jj];
                    if (!char.IsWhiteSpace(ch)) 
                        break;
                    if (ch == 0xD || ch == 0xA || ch == '\f') 
                    {
                        notSingle = true;
                        break;
                    }
                }
                if (((!notSingle && len > (totalLen - 20) && (len < (totalLen + 10))) && lastChar != '.' && lastChar != ':') && lastChar != ';') 
                {
                    tmp[j] = ' ';
                    count++;
                    if ((j + 1) < tmp.Length) 
                    {
                        ch = tmp[j + 1];
                        if (ch == 0xA) 
                        {
                            tmp[j + 1] = ' ';
                            j++;
                        }
                    }
                }
                i = j - 1;
            }
            return tmp.ToString();
        }
        public static string ReadStringFromFile(string fileName, bool utf8 = false)
        {
            byte[] data = Pullenti.Unitext.Internal.Uni.UnitextHelper.LoadDataFromFile(fileName, 0);
            if (data == null) 
                return null;
            return ReadStringFromBytes(data, false);
        }
        public static EncodingWrapper CheckEncoding(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            if (!fi.Exists || fi.Length == 0) 
                return null;
            byte[] buf = new byte[(int)(fi.Length > 5000 ? 5000 : (int)fi.Length)];
            using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
            {
                f.Read(buf, 0, buf.Length);
            }
            int i;
            return EncodingWrapper.CheckEncoding(buf, out i);
        }
        public static string ReadStringFromBytes(byte[] data, bool utf8 = false)
        {
            if (utf8) 
                return MiscHelper.DecodeStringUtf8(data, 0, -1);
            int hlen = 0;
            EncodingWrapper enc = EncodingWrapper.CheckEncoding(data, out hlen);
            if (enc == null) 
                return null;
            string txt = enc.GetString(data, 0, -1);
            return CorrectWhitespaces(txt);
        }
        public static void WriteStringToFile(string str, string fileName)
        {
            if (str == null) 
                str = "";
            byte[] data = MiscHelper.EncodeStringUtf8(str, true);
            using (FileStream f = new FileStream(fileName, FileMode.Create, FileAccess.Write)) 
            {
                f.Write(data, 0, data.Length);
            }
        }
        public static void WriteString1251ToFile(string str, string fileName)
        {
            if (str == null || fileName == null) 
                return;
            using (FileStream f = new FileStream(fileName, FileMode.Create, FileAccess.Write)) 
            {
                byte[] data = MiscHelper.EncodeString1251(str);
                f.Write(data, 0, data.Length);
            }
        }
        public enum ComapreTextsResult : int
        {
            NotEqual = 0,
            FirstStartsWithSecond = 1,
            SecondStartsWithFirst = 2,
            Equals = 3,
        }

        public static ComapreTextsResult CompareTexts(string str1, string str2, bool ignoreCase = false, bool ignoreSpecChars = false)
        {
            if (str1 == null || str2 == null) 
                return (str1 == str2 ? ComapreTextsResult.Equals : ComapreTextsResult.NotEqual);
            int i1 = 0;
            int i2 = 0;
            while (true) 
            {
                for (; i1 < str1.Length; i1++) 
                {
                    if (!char.IsWhiteSpace(str1[i1])) 
                    {
                        if (ignoreSpecChars) 
                        {
                            if (!char.IsLetterOrDigit(str1[i1])) 
                                continue;
                        }
                        break;
                    }
                }
                for (; i2 < str2.Length; i2++) 
                {
                    if (!char.IsWhiteSpace(str2[i2])) 
                    {
                        if (ignoreSpecChars) 
                        {
                            if (!char.IsLetterOrDigit(str2[i2])) 
                                continue;
                        }
                        break;
                    }
                }
                if (i1 >= str1.Length) 
                    break;
                if (i2 >= str2.Length) 
                    break;
                char ch1 = str1[i1];
                char ch2 = str2[i2];
                if (ch1 != ch2 && ignoreCase) 
                {
                    ch1 = char.ToUpper(ch1);
                    ch2 = char.ToUpper(ch2);
                }
                if (ch1 == 'ё') 
                    ch1 = 'е';
                if (ch2 == 'ё') 
                    ch2 = 'е';
                if (ch1 == 'Ё') 
                    ch1 = 'Е';
                if (ch2 == 'Ё') 
                    ch2 = 'Е';
                if (ch1 != ch2) 
                    return ComapreTextsResult.NotEqual;
                i1++;
                i2++;
            }
            m_CompareTextsEndChar1 = i1;
            m_CompareTextsEndChar2 = i2;
            if (i1 >= str1.Length && i2 >= str2.Length) 
                return ComapreTextsResult.Equals;
            if (i1 >= str1.Length) 
                return ComapreTextsResult.SecondStartsWithFirst;
            if (i2 >= str2.Length) 
                return ComapreTextsResult.FirstStartsWithSecond;
            return ComapreTextsResult.NotEqual;
        }
        public static int m_CompareTextsEndChar1;
        public static int m_CompareTextsEndChar2;
        public static string ExtractText(string fileName, byte[] content = null, bool unzipArchives = true)
        {
            Pullenti.Unitext.FileFormat frm = FileFormatsHelper.AnalizeFileFormat(fileName, content);
            if (frm == Pullenti.Unitext.FileFormat.Unknown) 
                return null;
            Pullenti.Unitext.FileFormatClass frmcl = FileFormatsHelper.GetFormatClass(frm);
            if (frmcl == Pullenti.Unitext.FileFormatClass.Archive && !unzipArchives) 
                return null;
            if (frmcl == Pullenti.Unitext.FileFormatClass.Image) 
                return null;
            Pullenti.Unitext.UnitextDocument doc = Pullenti.Unitext.UnitextService.CreateDocument(fileName, content, new Pullenti.Unitext.CreateDocumentParam() { OnlyForPureText = true, IgnoreInnerDocuments = !unzipArchives });
            if (doc == null) 
                return null;
            return Pullenti.Unitext.Internal.Uni.UnitextHelper.GetPlaintext(doc);
        }
    }
}