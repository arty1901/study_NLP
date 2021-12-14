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
using System.Xml;

namespace Pullenti.Util
{
    /// <summary>
    /// Набор полезных функций
    /// </summary>
    public static class MiscHelper
    {
        /// <summary>
        /// Выделить текст из всех форматов, какие только поддерживаются
        /// </summary>
        /// <param name="unzipArchives">при true будет распаковывать архивы и извлекать тексты из файлов, 
        /// сумммарный текст получается конкатенацией</param>
        /// <return>результат или null, если не получилось</return>
        public static string ExtractText(string fileName, byte[] content = null, bool unzipArchives = true)
        {
            return TextHelper.ExtractText(fileName, content, unzipArchives);
        }
        /// <summary>
        /// Получить hash-значение для строки. В отличие от штатных функций, эта работает 
        /// одинаково во всех случаях (например, в C# функция GetHashCode возвращает разные значения 
        /// на 32-х и 64-х разрядных компьютерах).
        /// </summary>
        /// <param name="str">строка</param>
        /// <return>хэш-значение</return>
        public static int GetStringHashCode(string str)
        {
            if (string.IsNullOrEmpty(str)) 
                return 0;
            int num = 0x15051505;
            int num2 = num;
            for (int i = 0; i < str.Length; i += 4) 
            {
                int num0 = ((i + 1) < str.Length ? (int)str[i + 1] : 0);
                num0 <<= 16;
                num0 |= (((int)str[i]) & 0xFFFF);
                num = (((((num << 5) + num)) + ((num >> 0x1b)))) ^ num0;
                if ((i + 3) >= str.Length) 
                    break;
                num0 = (int)str[i + 3];
                num0 <<= 16;
                num0 |= (((int)str[i + 2]) & 0xFFFF);
                num2 = (((((num2 << 5) + num2)) + ((num2 >> 0x1b)))) ^ num0;
            }
            return (num + ((num2 * 0x5d588b65)));
        }
        public static int GetCharHashCode(char ch)
        {
            return ((int)ch) | (((int)ch) << 16);
        }
        static MiscHelper()
        {
            m_1251_utf = new int[(int)256];
            m_utf_1251 = new Dictionary<int, byte>();
            for (int i = 0; i < 0x80; i++) 
            {
                m_1251_utf[i] = i;
            }
            int[] m_1251_80_BF = new int[] {0x0402, 0x0403, 0x201A, 0x0453, 0x201E, 0x2026, 0x2020, 0x2021, 0x20AC, 0x2030, 0x0409, 0x2039, 0x040A, 0x040C, 0x040B, 0x040F, 0x0452, 0x2018, 0x2019, 0x201C, 0x201D, 0x2022, 0x2013, 0x2014, 0x0000, 0x2122, 0x0459, 0x203A, 0x045A, 0x045C, 0x045B, 0x045F, 0x00A0, 0x040E, 0x045E, 0x0408, 0x00A4, 0x0490, 0x00A6, 0x00A7, 0x0401, 0x00A9, 0x0404, 0x00AB, 0x00AC, 0x00AD, 0x00AE, 0x0407, 0x00B0, 0x00B1, 0x0406, 0x0456, 0x0491, 0x00B5, 0x00B6, 0x00B7, 0x0451, 0x2116, 0x0454, 0x00BB, 0x0458, 0x0405, 0x0455, 0x0457};
            for (int i = 0; i < 0x40; i++) 
            {
                m_1251_utf[i + 0x80] = m_1251_80_BF[i];
                m_utf_1251.Add(m_1251_80_BF[i], (byte)((i + 0x80)));
            }
            for (int i = 0; i < 0x20; i++) 
            {
                m_1251_utf[i + 0xC0] = ((int)'А') + i;
                m_utf_1251.Add(((int)'А') + i, (byte)((i + 0xC0)));
            }
            for (int i = 0; i < 0x20; i++) 
            {
                m_1251_utf[i + 0xE0] = ((int)'а') + i;
                m_utf_1251.Add(((int)'а') + i, (byte)((i + 0xE0)));
            }
            int[] m_1252_80_9F = new int[] {0x20AC, 0, 0x201A, 0x0192, 0x201E, 0x2026, 0x2020, 0x2021, 0x02C6, 0x2030, 0x0160, 0x2039, 0x0152, 0, 0x017D, 0, 0, 0x2018, 0x2019, 0x201C, 0x201D, 0x2022, 0x2013, 0x2014, 0x02DC, 0x2122, 0x0161, 0x203A, 0x0153, 0, 0x017E, 0x0178};
            m_1252_utf = new int[(int)256];
            m_utf_1252 = new Dictionary<int, byte>();
            for (int i = 0; i < 0x100; i++) 
            {
                m_1252_utf[i] = i;
            }
            for (int i = 0; i < 0x20; i++) 
            {
                if (m_1252_80_9F[i] != 0) 
                {
                    m_1252_utf[i + 0x80] = m_1252_80_9F[i];
                    m_utf_1252.Add(m_1252_80_9F[i], (byte)((i + 0x80)));
                }
            }
        }
        /// <summary>
        /// Преобразовать строку в float. Pаботает независимо от региональных настроек.
        /// </summary>
        /// <param name="str">строка, разделитель может быть как точка, так и запятая</param>
        /// <param name="res">результат</param>
        /// <return>признак корректности</return>
        public static bool TryParseFloat(string str, out float res)
        {
            res = 0;
            if (string.IsNullOrEmpty(str)) 
                return false;
            if (float.TryParse(str, out res)) 
                return true;
            if (str.IndexOf(',') >= 0 && float.TryParse(str.Replace(',', '.'), out res)) 
                return true;
            if (str.IndexOf('.') >= 0 && float.TryParse(str.Replace('.', ','), out res)) 
                return true;
            return false;
        }
        /// <summary>
        /// Преобразовать строку в double. Pаботает независимо от региональных настроек.
        /// </summary>
        /// <param name="str">строка, разделитель может быть как точка, так и запятая</param>
        /// <param name="res">результат</param>
        /// <return>признак корректности</return>
        public static bool TryParseDouble(string str, out double res)
        {
            res = 0;
            if (string.IsNullOrEmpty(str)) 
                return false;
            if (double.TryParse(str, out res)) 
                return true;
            if (str.IndexOf(',') >= 0 && double.TryParse(str.Replace(',', '.'), out res)) 
                return true;
            if (str.IndexOf('.') >= 0 && double.TryParse(str.Replace('.', ','), out res)) 
                return true;
            return false;
        }
        /// <summary>
        /// Вывести значение в строку. Не зависит от региональных настроек, разделитель всегда точка.
        /// </summary>
        public static string OutDouble(double val)
        {
            return val.ToString().Replace(',', '.');
        }
        /// <summary>
        /// Вывести дату-время. Не зависит от региональных настроек, всегда в формате YYYY.MM.DD HH:MM:SS
        /// </summary>
        /// <param name="dt">дата-время</param>
        /// <param name="timeIgnore">не выводить время</param>
        /// <return>строка с результатом</return>
        public static string OutDateTime(DateTime dt, bool timeIgnore = false)
        {
            if (timeIgnore) 
                return string.Format("{0}.{1}.{2}", dt.Year, dt.Month.ToString("D02"), dt.Day.ToString("D02"));
            else 
                return string.Format("{0}.{1}.{2} {3}:{4}:{5}", dt.Year, dt.Month.ToString("D02"), dt.Day.ToString("D02"), dt.Hour.ToString("D02"), dt.Minute.ToString("D02"), dt.Second.ToString("D02"));
        }
        /// <summary>
        /// Преобразовать строку в DateTime. Pаботает независимо от региональных настроек.
        /// </summary>
        /// <param name="str">строка с датой в разных форматах написания</param>
        /// <return>результат или null</return>
        public static DateTime? TryParseDateTime(string val)
        {
            if (string.IsNullOrEmpty(val)) 
                return null;
            List<int> ints = new List<int>();
            bool isT = false;
            for (int i = 0; i < val.Length; i++) 
            {
                if (char.IsDigit(val[i])) 
                {
                    int v = ((int)val[i]) - 0x30;
                    for (++i; i < val.Length; i++) 
                    {
                        if (!char.IsDigit(val[i])) 
                            break;
                        else 
                            v = ((v * 10) + ((int)val[i])) - 0x30;
                    }
                    ints.Add(v);
                    if (ints.Count == 3 && (i < val.Length) && val[i] == 'T') 
                        isT = true;
                }
                else if (ints.Count == 3 && val[i] == 'T') 
                    isT = true;
            }
            try 
            {
                if (ints.Count == 3) 
                {
                    if (ints[2] > 1900) 
                        return new DateTime(ints[2], ints[1], ints[0]);
                    if (ints[1] > 12 || ints[2] > 31) 
                        return null;
                    return new DateTime(ints[0], ints[1], ints[2]);
                }
                else if (ints.Count == 6 || ((ints.Count >= 6 && isT))) 
                    return new DateTime(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5]);
            }
            catch(Exception ex) 
            {
            }
            return null;
        }
        /// <summary>
        /// Преобразовать строку в DateTime. Pаботает независимо от региональных настроек.
        /// </summary>
        /// <param name="str">строка с датой в разных форматах написания</param>
        /// <return>результат или DateTime.MinValue при ошибке</return>
        public static DateTime ParseDateTime(string val)
        {
            DateTime? dt = TryParseDateTime(val);
            if (dt == null) 
                return DateTime.MinValue;
            return dt.Value;
        }
        public static void SerializeInt(Stream stream, int val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, 4);
        }
        public static int DeserializeInt(Stream stream)
        {
            byte[] buf = new byte[(int)4];
            stream.Read(buf, 0, 4);
            return BitConverter.ToInt32(buf, 0);
        }
        public static void SerializeShort(Stream stream, short val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, 2);
        }
        public static short DeserializeShort(Stream stream)
        {
            byte[] buf = new byte[(int)2];
            stream.Read(buf, 0, 2);
            return BitConverter.ToInt16(buf, 0);
        }
        public static void SerializeString(Stream stream, string val)
        {
            if (val == null) 
            {
                SerializeInt(stream, -1);
                return;
            }
            if (string.IsNullOrEmpty(val)) 
            {
                SerializeInt(stream, 0);
                return;
            }
            byte[] data = EncodeStringUtf8(val, false);
            SerializeInt(stream, data.Length);
            stream.Write(data, 0, data.Length);
        }
        public static string DeserializeString(Stream stream)
        {
            int len = DeserializeInt(stream);
            if (len < 0) 
                return null;
            if (len == 0) 
                return "";
            byte[] data = new byte[(int)len];
            stream.Read(data, 0, data.Length);
            return DecodeStringUtf8(data, 0, -1);
        }
        /// <summary>
        /// Прочитать байтовый массив из потока
        /// </summary>
        /// <param name="s">поток</param>
        /// <return>результат</return>
        public static byte[] ReadStream(Stream s)
        {
            if (s.CanSeek) 
            {
                if (s.Length > 0) 
                {
                    byte[] res = new byte[(int)s.Length];
                    s.Read(res, 0, res.Length);
                    return res;
                }
            }
            byte[] buf = new byte[(int)10000];
            MemoryStream mem = new MemoryStream();
            int k = 0;
            while (true) 
            {
                int i = s.Read(buf, 0, buf.Length);
                if (i < 0) 
                    break;
                if (i == 0) 
                {
                    if ((++k) > 3) 
                        break;
                    continue;
                }
                mem.Write(buf, 0, i);
                k = 0;
            }
            byte[] arr = mem.ToArray();
            mem.Dispose();
            return arr;
        }
        // Замена стандартной функции, которая очень тормозит
        public static bool EndsWith(string str, string substr)
        {
            if (str == null || substr == null) 
                return false;
            int i = str.Length - 1;
            int j = substr.Length - 1;
            if (j > i || (j < 0)) 
                return false;
            for (; j >= 0; j--,i--) 
            {
                if (str[i] != substr[j]) 
                    return false;
            }
            return true;
        }
        public static bool EndsWithEx(string str, string substr, string substr2, string substr3 = null)
        {
            if (str == null) 
                return false;
            for (int k = 0; k < 3; k++) 
            {
                if (k == 1) 
                    substr = substr2;
                else if (k == 2) 
                    substr = substr3;
                if (substr == null) 
                    continue;
                int i = str.Length - 1;
                int j = substr.Length - 1;
                if (j > i || (j < 0)) 
                    continue;
                for (; j >= 0; j--,i--) 
                {
                    if (str[i] != substr[j]) 
                        break;
                }
                if (j < 0) 
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Закодировать строку кодировкой ASCII. Работает на всех платформах.
        /// </summary>
        /// <param name="str">строка</param>
        /// <return>результат</return>
        public static byte[] EncodeStringAscii(string str)
        {
            if (str == null) 
                return new byte[(int)0];
            byte[] res = new byte[(int)str.Length];
            for (int j = 0; j < str.Length; j++) 
            {
                int i = (int)str[j];
                if (i < 0x100) 
                    res[j] = (byte)i;
                else 
                    res[j] = (byte)'?';
            }
            return res;
        }
        /// <summary>
        /// Закодировать строку кодировкой windows-1251. Работает на всех платформах.
        /// </summary>
        /// <param name="str">строка</param>
        /// <return>результат</return>
        public static byte[] EncodeString1251(string str)
        {
            if (str == null) 
                return new byte[(int)0];
            byte[] res = new byte[(int)str.Length];
            for (int j = 0; j < str.Length; j++) 
            {
                int i = (int)str[j];
                if (i < 0x80) 
                    res[j] = (byte)i;
                else 
                {
                    byte b;
                    if (m_utf_1251.TryGetValue(i, out b)) 
                        res[j] = b;
                    else 
                        res[j] = (byte)'?';
                }
            }
            return res;
        }
        /// <summary>
        /// Закодировать строку кодировкой windows-1252. Работает на всех платформах.
        /// </summary>
        /// <param name="str">строка</param>
        /// <return>результат</return>
        public static byte[] EncodeString1252(string str)
        {
            if (str == null) 
                return new byte[(int)0];
            byte[] res = new byte[(int)str.Length];
            for (int j = 0; j < str.Length; j++) 
            {
                int i = (int)str[j];
                if ((i < 0x80) || i >= 0xA0) 
                    res[j] = (byte)i;
                else 
                {
                    byte b;
                    if (m_utf_1252.TryGetValue(i, out b)) 
                        res[j] = b;
                    else 
                        res[j] = (byte)'?';
                }
            }
            return res;
        }
        /// <summary>
        /// Декодировать строку из массива в кодировке Ascii. Работает на всех платформах.
        /// </summary>
        /// <param name="dat">байтовый масств</param>
        /// <param name="pos">начальная позиция в массиве</param>
        /// <param name="len">длина байт</param>
        /// <return>строка с результатом</return>
        public static string DecodeStringAscii(byte[] dat, int pos = 0, int len = -1)
        {
            if (dat == null) 
                return null;
            if (dat.Length == 0) 
                return "";
            if (len < 0) 
                len = dat.Length - pos;
            StringBuilder tmp = new StringBuilder(len);
            for (int j = pos; (j < (pos + len)) && (j < dat.Length); j++) 
            {
                int i = (int)((byte)dat[j]);
                if (i < 0x100) 
                    tmp.Append((char)i);
                else 
                    tmp.Append('?');
            }
            return tmp.ToString();
        }
        /// <summary>
        /// Декодировать строку из массива в кодировке windows-1251. Работает на всех платформах.
        /// </summary>
        /// <param name="dat">байтовый масств</param>
        /// <param name="pos">начальная позиция в массиве</param>
        /// <param name="len">длина байт</param>
        /// <return>строка с результатом</return>
        public static string DecodeString1251(byte[] dat, int pos = 0, int len = -1)
        {
            if (dat == null) 
                return null;
            if (dat.Length == 0) 
                return "";
            if (len < 0) 
                len = dat.Length - pos;
            StringBuilder tmp = new StringBuilder(len);
            for (int j = pos; (j < (pos + len)) && (j < dat.Length); j++) 
            {
                int i = (int)((byte)dat[j]);
                if (i < 0x80) 
                    tmp.Append((char)i);
                else if (m_1251_utf[i] == 0) 
                    tmp.Append('?');
                else 
                    tmp.Append((char)m_1251_utf[i]);
            }
            return tmp.ToString();
        }
        /// <summary>
        /// Декодировать строку из массива в кодировке windows-1252. Работает на всех платформах.
        /// </summary>
        /// <param name="dat">байтовый масств</param>
        /// <param name="pos">начальная позиция в массиве</param>
        /// <param name="len">длина байт</param>
        /// <return>строка с результатом</return>
        public static string DecodeString1252(byte[] dat, int pos = 0, int len = -1)
        {
            if (dat == null) 
                return null;
            if (dat.Length == 0) 
                return "";
            if (len < 0) 
                len = dat.Length - pos;
            StringBuilder tmp = new StringBuilder(len);
            for (int j = pos; (j < (pos + len)) && (j < dat.Length); j++) 
            {
                int i = (int)((byte)dat[j]);
                if ((i < 0x80) || i >= 0xA0) 
                    tmp.Append((char)i);
                else if (m_1252_utf[i] == 0) 
                    tmp.Append('?');
                else 
                    tmp.Append((char)m_1252_utf[i]);
            }
            return tmp.ToString();
        }
        internal static int[] m_1251_utf;
        internal static int[] m_1252_utf;
        static Dictionary<int, byte> m_utf_1251;
        static Dictionary<int, byte> m_utf_1252;
        /// <summary>
        /// Закодировать строку в коде UTF-8 с добавлением преамбулы
        /// </summary>
        /// <param name="str">кодируемая строка</param>
        /// <param name="addPreambl">добавлять ли преамбулу EF BB BF</param>
        /// <return>результирующий массив</return>
        public static byte[] EncodeStringUtf8(string str, bool addPreambl = false)
        {
            if (str == null) 
                return new byte[(int)0];
            byte[] res = Encoding.UTF8.GetBytes(str);
            if (!addPreambl) 
                return res;
            List<byte> tmp = new List<byte>(res.Length + 3);
            tmp.AddRange(Encoding.UTF8.GetPreamble());
            tmp.AddRange(res);
            return tmp.ToArray();
        }
        /// <summary>
        /// Декодировать строку из UTF-8. Если есть преамбула, то она проигнорируется.
        /// </summary>
        /// <param name="dat">массив с закодированной строкой</param>
        /// <return>раскодированная строка</return>
        public static string DecodeStringUtf8(byte[] dat, int pos = 0, int len = -1)
        {
            if (dat == null) 
                return null;
            if (len < 0) 
                len = dat.Length;
            if ((len > 3 && dat[pos] == ((byte)0xEF) && dat[pos + 1] == ((byte)0xBB)) && dat[pos + 2] == ((byte)0xBF)) 
            {
                pos += 3;
                len -= 3;
            }
            return Encoding.UTF8.GetString(dat, pos, len);
        }
        /// <summary>
        /// Закодировать строку в 2-х байтовой кодировке Unicode, младший байт первый (UTF-16LE).
        /// </summary>
        /// <param name="str">кодируемая строка</param>
        /// <return>результирующий массив</return>
        public static byte[] EncodeStringUnicode(string str)
        {
            if (string.IsNullOrEmpty(str)) 
                return new byte[(int)0];
            byte[] res = new byte[(int)(str.Length * 2)];
            for (int i = 0; i < str.Length; i++) 
            {
                int cod = (int)str[i];
                res[i * 2] = (byte)((cod & 0xFF));
                res[(i * 2) + 1] = (byte)((cod >> 8));
            }
            return res;
        }
        /// <summary>
        /// Декодировать строку из 2-х байтовой кодировки Unicode, младший байт первый (UTF-16LE).
        /// </summary>
        /// <param name="dat">массив с закодированной строкой</param>
        /// <return>раскодированная строка</return>
        public static string DecodeStringUnicode(byte[] dat, int pos = 0, int len = -1)
        {
            if (dat == null) 
                return null;
            if (len < 0) 
                len = dat.Length;
            StringBuilder res = new StringBuilder(len / 2);
            for (int i = pos; i < (pos + len); i += 2) 
            {
                int cod = (int)dat[i + 1];
                cod <<= 8;
                cod |= ((int)dat[i]);
                res.Append((char)cod);
            }
            return res.ToString();
        }
        /// <summary>
        /// Закодировать строку в 2-х байтовой кодировке Unicode, старший байт первый (UTF-16BE).
        /// </summary>
        /// <param name="str">кодируемая строка</param>
        /// <return>результирующий массив</return>
        public static byte[] EncodeStringUnicodeBE(string str)
        {
            if (string.IsNullOrEmpty(str)) 
                return new byte[(int)0];
            byte[] res = new byte[(int)(str.Length * 2)];
            for (int i = 0; i < str.Length; i++) 
            {
                int cod = (int)str[i];
                res[(i * 2) + 1] = (byte)((cod & 0xFF));
                res[i * 2] = (byte)((cod >> 8));
            }
            return res;
        }
        /// <summary>
        /// Декодировать строку из 2-х байтовой кодировки Unicode, старший байт первый (UTF-16BE).
        /// </summary>
        /// <param name="dat">массив с закодированной строкой</param>
        /// <return>раскодированная строка</return>
        public static string DecodeStringUnicodeBE(byte[] dat, int pos = 0, int len = -1)
        {
            if (dat == null) 
                return null;
            if (len < 0) 
                len = dat.Length;
            StringBuilder res = new StringBuilder(len / 2);
            for (int i = pos; i < (pos + len); i += 2) 
            {
                int cod = (int)dat[i];
                cod <<= 8;
                cod |= ((int)dat[i + 1]);
                res.Append((char)cod);
            }
            return res.ToString();
        }
        public static string CorrectCsvValue(string txt)
        {
            if (txt == null) 
                return null;
            if (txt.IndexOf('"') >= 0) 
                return txt.Replace("\"", "\"\"");
            return txt;
        }
        public static string CorrectJsonValue(string txt)
        {
            StringBuilder res = new StringBuilder();
            foreach (char ch in txt) 
            {
                if (ch == '"') 
                    res.Append("\\\"");
                else if (ch == '\\') 
                    res.Append("\\\\");
                else if (ch == '/') 
                    res.Append("\\/");
                else if (ch == 0xD) 
                    res.Append("\\r");
                else if (ch == 0xA) 
                    res.Append("\\n");
                else if (ch == '\t') 
                    res.Append("\\t");
                else if (((int)ch) < 0x20) 
                    res.Append(' ');
                else 
                    res.Append(ch);
            }
            return res.ToString();
        }
        public static void CorrectXmlFile(StringBuilder res)
        {
            int i = res.ToString().IndexOf('>');
            if (i > 10 && res[1] == '?') 
                res.Remove(0, i + 1);
            for (i = 0; i < res.Length; i++) 
            {
                char ch = res[i];
                int cod = (int)ch;
                if ((cod < 0x80) && cod >= 0x20) 
                    continue;
                if (cod >= 0x400 || (cod < 0x500)) 
                    continue;
                res.Remove(i, 1);
                res.Insert(i, string.Format("&#x{0};", cod.ToString("X04")));
            }
        }
        /// <summary>
        /// Сериализация объекта, реализующего IXmlReadWriteSupport, в байтовый массив. 
        /// Работает одинаково на всех языках программирования.
        /// </summary>
        /// <param name="obj">объект</param>
        /// <return>результат</return>
        public static byte[] SerializeToBin(IXmlReadWriteSupport obj)
        {
            if (obj == null) 
                return null;
            StringBuilder res = new StringBuilder();
            using (XmlWriter xml = XmlWriter.Create(res)) 
            {
                obj.WriteToXml(xml, null);
            }
            CorrectXmlFile(res);
            byte[] dat = EncodeStringUtf8(res.ToString(), false);
            return ArchiveHelper.CompressGZip(dat);
        }
        /// <summary>
        /// Десериализация из байтового массива
        /// </summary>
        /// <param name="dat">массив, полученный функцией SerializeToBin</param>
        /// <param name="obj">экземпляр десериализуемого объекта</param>
        public static void DeserializeFromBin(byte[] dat, IXmlReadWriteSupport obj)
        {
            if (dat == null) 
                return;
            byte[] data = ArchiveHelper.DecompressGZip(dat);
            string str = DecodeStringUtf8(data, 0, -1);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(str);
            obj.ReadFromXml(xml.DocumentElement);
        }
        public static byte[] SerializeToBinList(List<IXmlReadWriteSupport> objs)
        {
            StringBuilder res = new StringBuilder();
            using (XmlWriter xml = XmlWriter.Create(res)) 
            {
                xml.WriteStartElement("LIST");
                if (objs != null) 
                {
                    foreach (IXmlReadWriteSupport obj in objs) 
                    {
                        obj.WriteToXml(xml, null);
                    }
                }
                xml.WriteEndElement();
            }
            CorrectXmlFile(res);
            byte[] dat = EncodeStringUtf8(res.ToString(), false);
            return ArchiveHelper.CompressGZip(dat);
        }
        public static int DeserializeFromBinList(byte[] dat, List<IXmlReadWriteSupport> res)
        {
            if (dat == null) 
                return 0;
            byte[] data = ArchiveHelper.DecompressGZip(dat);
            string str = DecodeStringUtf8(data, 0, -1);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(str);
            int i = 0;
            foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
            {
                if (res != null) 
                {
                    if (i >= res.Count) 
                        break;
                    res[i].ReadFromXml(x);
                }
                i++;
            }
            return i;
        }
        /// <summary>
        /// При сохранении значений в XML рекомендуется пропускать через эту функцию. 
        /// Иначе если в строке окажутся некоторые символы (например, 0xC), то XML получается некорректным.
        /// </summary>
        /// <param name="txt">исходный текст узла или атрибута</param>
        /// <return>откорректированный</return>
        public static string CorrectXmlValue(string txt)
        {
            if (txt == null) 
                return "";
            bool corr = false;
            for (int i = 0; i < txt.Length; i++) 
            {
                int cod = (int)txt[i];
                if (((cod < 0x20) && cod != 0xD && cod != 0xA) && cod != 9) 
                {
                    corr = true;
                    break;
                }
                else if (cod >= 0xD800 && cod <= 0xDBFF) 
                {
                    if ((i + 1) >= txt.Length) 
                    {
                        corr = true;
                        break;
                    }
                    i++;
                    cod = (int)txt[i];
                    if ((cod < 0xDC00) || cod > 0xDFFF) 
                    {
                        corr = true;
                        break;
                    }
                }
                else if (cod >= 0xDC00 && cod <= 0xDFFF) 
                {
                    corr = true;
                    break;
                }
            }
            if (!corr) 
                return txt;
            StringBuilder tmp = new StringBuilder(txt);
            for (int i = 0; i < tmp.Length; i++) 
            {
                char ch = tmp[i];
                if (((((int)ch) < 0x20) && ch != '\r' && ch != '\n') && ch != '\t') 
                    tmp[i] = ' ';
                else if (((int)ch) >= 0xD800 && ((int)ch) <= 0xDBFF) 
                {
                    if ((i + 1) >= tmp.Length) 
                    {
                        tmp[i] = ' ';
                        break;
                    }
                    char ch1 = tmp[i + 1];
                    if (((int)ch1) >= 0xDC00 && ((int)ch1) <= 0xDFFF) 
                        i++;
                    else 
                        tmp[i] = ' ';
                }
                else if (((int)ch) >= 0xDC00 && ((int)ch) <= 0xDFFF) 
                    tmp[i] = '?';
            }
            return tmp.ToString();
        }
        public static void CorrectHtmlChar(StringBuilder res, char c)
        {
            if (((((int)c) < 0x20) && c != '\r' && c != '\n') && c != '\t') 
                res.Append(' ');
            else if (((int)c) > 0xF000) 
                res.AppendFormat("&#{0};", (int)c);
            else if (c == 0xA || c == 0xD) 
                res.Append("\r\n<br/>");
            else if (c == ((char)0xA0)) 
                res.Append("&nbsp;");
            else if (c == '<') 
                res.Append("&lt;");
            else if (c == '>') 
                res.Append("&gt;");
            else if (c == '&') 
                res.Append("&amp;");
            else 
                res.Append(c);
        }
        public static void CorrectHtmlValue(StringBuilder res, string txt, bool isAttr = false, bool isPureXml = false)
        {
            if (txt == null) 
                return;
            for (int i = 0; i < txt.Length; i++) 
            {
                char c = txt[i];
                if (((((int)c) < 0x20) && c != '\r' && c != '\n') && c != '\t') 
                    res.Append(' ');
                else if (((int)c) > 0xF000) 
                    res.AppendFormat("&#{0};", (int)c);
                else if (c == 0xA || c == 0xD) 
                {
                    if (i > 0 && txt[i - 1] == 0xD) 
                    {
                    }
                    else if (isAttr || isPureXml) 
                        res.Append("\r\n");
                    else 
                        res.Append("\r\n<br/>");
                }
                else if (c == ((char)0xA0)) 
                {
                    if (isPureXml) 
                        res.AppendFormat("&#{0};", 0xA0);
                    else 
                        res.Append("&nbsp;");
                }
                else if (c == '<') 
                    res.Append("&lt;");
                else if (c == '>') 
                    res.Append("&gt;");
                else if (c == '&') 
                    res.Append("&amp;");
                else if (isAttr && c == '"') 
                    res.Append("&quot;");
                else 
                    res.Append(c);
            }
        }
    }
}