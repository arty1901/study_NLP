/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Util
{
    /// <summary>
    /// Реализация кодировщика строк, замена системного Encoding. 
    /// Введена из-за того, что в .NET Core не поддержаны разные 1251 и пр. 
    /// Да и для кросспрограммности и кроссплатформенности это необходимо.
    /// </summary>
    public class EncodingWrapper
    {
        /// <summary>
        /// Создать обёртку
        /// </summary>
        /// <param name="typ">Стандартная кодировка, для которой есть собственная реализация</param>
        /// <param name="str">Строковое название, например, windows-1251, utf-8 и пр.</param>
        /// <param name="cp">Кодовая страница Windows, например, 1251</param>
        public EncodingWrapper(EncodingStandard typ, string str = null, int cp = 0)
        {
            CodePage = cp;
            StdTyp = typ;
            if (cp == 1251) 
                StdTyp = EncodingStandard.CP1251;
            else if (cp == 1252) 
                StdTyp = EncodingStandard.CP1252;
            if (string.IsNullOrEmpty(str)) 
                return;
            string ss = str.Replace("-", "").ToUpper();
            if (ss.EndsWith("ASCII")) 
            {
                StdTyp = EncodingStandard.ACSII;
                return;
            }
            if (ss.StartsWith("WINDOWS")) 
            {
                int cod;
                if (int.TryParse(ss.Substring(7), out cod)) 
                {
                    CodePage = cod;
                    if (cod == 1251) 
                        StdTyp = EncodingStandard.CP1251;
                    else if (cod == 1252) 
                        StdTyp = EncodingStandard.CP1252;
                    return;
                }
            }
            if (ss.StartsWith("CP")) 
            {
                int cod;
                if (int.TryParse(ss.Substring(2), out cod)) 
                {
                    CodePage = cod;
                    if (cod == 1251) 
                        StdTyp = EncodingStandard.CP1251;
                    else if (cod == 1252) 
                        StdTyp = EncodingStandard.CP1252;
                    return;
                }
            }
            if (ss == "UTF8") 
            {
                StdTyp = EncodingStandard.UTF8;
                return;
            }
            if (ss == "UNICODE" || ss == "UTF16" || ss == "UTF16LE") 
            {
                StdTyp = EncodingStandard.UTF16LE;
                return;
            }
            if (ss == "UTF16BE") 
            {
                StdTyp = EncodingStandard.UTF16BE;
                return;
            }
        }
        /// <summary>
        /// Стандартный тип
        /// </summary>
        public EncodingStandard StdTyp = EncodingStandard.Undefined;
        /// <summary>
        /// Кодовая страница Windows
        /// </summary>
        public int CodePage;
        public override string ToString()
        {
            if (StdTyp != EncodingStandard.Undefined) 
                return StdTyp.ToString();
            if (CodePage > 0) 
                return string.Format("CodePage={0}", CodePage);
            return "?";
        }
        /// <summary>
        /// Закодировать строку
        /// </summary>
        /// <param name="str">кодируемая строка</param>
        /// <return>массив файт</return>
        public byte[] GetBytes(string str)
        {
            if (str == null) 
                return new byte[(int)0];
            if (StdTyp == EncodingStandard.ACSII) 
                return MiscHelper.EncodeStringAscii(str);
            if (StdTyp == EncodingStandard.UTF8) 
                return MiscHelper.EncodeStringUtf8(str, false);
            if (StdTyp == EncodingStandard.CP1251) 
                return MiscHelper.EncodeString1251(str);
            if (StdTyp == EncodingStandard.CP1252) 
                return MiscHelper.EncodeString1252(str);
            if (StdTyp == EncodingStandard.UTF16LE) 
                return MiscHelper.EncodeStringUnicode(str);
            if (StdTyp == EncodingStandard.UTF16BE) 
                return MiscHelper.EncodeStringUnicodeBE(str);
            return MiscHelper.EncodeString1252(str);
        }
        /// <summary>
        /// Раскодировать строку
        /// </summary>
        /// <param name="dat">массив байт</param>
        /// <param name="len">начальная позиция в массиве</param>
        /// <param name="pos">длина</param>
        public string GetString(byte[] dat, int pos = 0, int len = -1)
        {
            if (dat == null || (pos < 0)) 
                return null;
            if (dat.Length == 0) 
                return "";
            if (len < 0) 
                len = dat.Length - pos;
            if (StdTyp == EncodingStandard.ACSII) 
                return MiscHelper.DecodeStringAscii(dat, pos, len);
            if (StdTyp == EncodingStandard.UTF8) 
                return MiscHelper.DecodeStringUtf8(dat, pos, len);
            if (StdTyp == EncodingStandard.CP1251) 
                return MiscHelper.DecodeString1251(dat, pos, len);
            if (StdTyp == EncodingStandard.CP1252) 
                return MiscHelper.DecodeString1252(dat, pos, len);
            if (StdTyp == EncodingStandard.UTF16LE) 
                return MiscHelper.DecodeStringUnicode(dat, pos, len);
            if (StdTyp == EncodingStandard.UTF16BE) 
                return MiscHelper.DecodeStringUnicodeBE(dat, pos, len);
            return MiscHelper.DecodeString1252(dat, pos, len);
        }
        /// <summary>
        /// Определение кодировки по байтовому массиву
        /// </summary>
        /// <param name="data">кодированный массив</param>
        /// <param name="headLen">размер префикса, если он есть</param>
        /// <return>результирующая кодировка</return>
        public static EncodingWrapper CheckEncoding(byte[] data, out int headLen)
        {
            headLen = 0;
            if ((data.Length >= 3 && data[0] == ((byte)0xEF) && data[1] == ((byte)0xBB)) && data[2] == ((byte)0xBF)) 
            {
                if (data.Length == 3) 
                    return null;
                headLen = 3;
                return new EncodingWrapper(EncodingStandard.UTF8);
            }
            if (data.Length >= 2 && data[0] == ((byte)0xFF) && data[1] == ((byte)0xFE)) 
            {
                if (data.Length == 2) 
                    return null;
                headLen = 2;
                return new EncodingWrapper(EncodingStandard.UTF16LE);
            }
            if (data.Length >= 2 && data[0] == ((byte)0xFE) && data[1] == ((byte)0xFF)) 
            {
                if (data.Length == 2) 
                    return null;
                headLen = 2;
                return new EncodingWrapper(EncodingStandard.UTF16BE);
            }
            int i;
            int j;
            int dos = 0;
            int win = 0;
            int utf8 = 0;
            int d0 = 0;
            int rus = 0;
            for (i = 0; i < data.Length; i++) 
            {
                j = (int)data[i];
                if (j == 0xE2 && ((i + 2) < data.Length)) 
                {
                    if (data[i + 1] == 0x80 && data[i + 2] == 0x99) 
                    {
                        utf8++;
                        i += 2;
                        continue;
                    }
                }
                if (j >= 0xC0) 
                    win++;
                if ((j >= 0x80 && j <= 0xAF)) 
                    dos++;
                else if (j >= 0xE0 && j <= 0xEF) 
                    dos++;
                if (j >= 0x80) 
                {
                    rus++;
                    if (j == 0xD0 || j == 0xD1) 
                        d0++;
                }
            }
            if (dos > win && utf8 == 0) 
            {
                byte[] data2 = new byte[(int)data.Length];
                for (i = 0; i < data.Length; i++) 
                {
                    data2[i] = data[i];
                    j = (int)data[i];
                    if (j >= 0xE0 && j <= 0xEF) 
                        j -= 0x30;
                    if (j >= 0x80 && j <= 0xDF) 
                        data2[i] = (byte)((j + 0x40));
                    if (j == 0xF1) 
                        data2[i] = 0xB8;
                    if (j == 0xF0) 
                        data2[i] = 0xA8;
                }
                data = data2;
            }
            EncodingWrapper enc = null;
            string txt = null;
            if (d0 > ((rus / 5)) || utf8 > 0) 
            {
                try 
                {
                    txt = MiscHelper.DecodeStringUtf8(data, 0, -1);
                    enc = new EncodingWrapper(EncodingStandard.UTF8);
                    if (utf8 > 0) 
                        return enc;
                }
                catch(Exception ex346) 
                {
                }
            }
            if (txt == null || enc == null) 
                return new EncodingWrapper(EncodingStandard.CP1251);
            try 
            {
                string txt2 = MiscHelper.DecodeString1251(data, 0, -1);
                int ru = 0;
                int ru2 = 0;
                foreach (char ch in txt) 
                {
                    if (ch >= 0x400 && (ch < 0x500)) 
                    {
                        if (char.IsLetter(ch)) 
                            ru++;
                    }
                }
                foreach (char ch in txt2) 
                {
                    if (ch >= 0x400 && (ch < 0x500)) 
                    {
                        if (char.IsLetter(ch)) 
                            ru2++;
                    }
                }
                if (ru2 > ((ru * 2))) 
                    return new EncodingWrapper(EncodingStandard.CP1251);
                return enc;
            }
            catch(Exception ex) 
            {
            }
            return new EncodingWrapper(EncodingStandard.UTF8);
        }
    }
}