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

namespace Pullenti.Unitext.Internal.Misc
{
    static class WebdingsHelper
    {
        static WebdingsHelper()
        {
            int[] data = new int[] {0x33, 9204, 0x34, 9205, 0x35, 9206, 0x36, 9207, 0x37, 9194, 0x38, 9193, 0x39, 9198, 0x3A, 9197, 0x3B, 9208, 0x3C, 9209, 0x3D, 9210, 0x60, 11156, 0x61, 10004, 0x63, 9633, 0x67, 9632, 0x6E, 9899, 0x73, 10067, 0x77, 9971, 0x79, 8854, 0x7C, 124, 0x87, 9975, 0x98, 10031, 0xD9, 9729, 0xE8, 9413, 0xE9, 9855};
            for (int i = 0; i < (data.Length - 1); i += 2) 
            {
                if (data[i] < 0xFFFF) 
                {
                    if (!m_Convert.ContainsKey(data[i])) 
                    {
                        int cod = data[i + 1] & 0xFFFF;
                        m_Convert.Add(data[i], (char)cod);
                    }
                }
            }
        }
        static Dictionary<int, char> m_Convert = new Dictionary<int, char>();
        public static char GetUnicode(int code)
        {
            char res;
            if (m_Convert.TryGetValue(code, out res)) 
                return res;
            else 
                return (char)0;
        }
        public static string GetUnicodeString(string str)
        {
            StringBuilder res = new StringBuilder();
            foreach (char c in str) 
            {
                char ch = GetUnicode((int)c);
                if (ch != ((char)0)) 
                    res.Append(ch);
                else 
                    res.Append("?");
            }
            return res.ToString();
        }
    }
}