/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Util
{
    public class StructExpressLine
    {
        public int BeginChar;
        public int EndChar;
        public string Text;
        public string Keyword;
        public string Number;
        public bool IsStartOfApp;
        public int IllegalChars;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (IsStartOfApp) 
                res.Append("StartOfApp ");
            if (Keyword != null) 
                res.AppendFormat("[{0}] ", Keyword);
            if (Number != null) 
                res.AppendFormat("N{0} ", Number);
            if (IllegalChars > 0) 
                res.AppendFormat("(illegals={0}) ", IllegalChars);
            int len = (EndChar + 1) - BeginChar;
            if (len > 100) 
                len = 100;
            res.AppendFormat("\"{0}\"", Text.Substring(BeginChar, len));
            return res.ToString();
        }
        public static StructExpressLine TryParse(string txt, ref int i)
        {
            for (; i < txt.Length; i++) 
            {
                if (txt[i] != '\n' && txt[i] != '\r') 
                    break;
            }
            if (i >= txt.Length) 
                return null;
            StructExpressLine res = new StructExpressLine();
            res.BeginChar = (res.EndChar = i);
            res.Text = txt;
            for (; i < txt.Length; i++) 
            {
                if (txt[i] == '\n' || txt[i] == '\r') 
                    break;
                else 
                    res.EndChar = i;
            }
            res._analyze();
            return res;
        }
        void _analyze()
        {
            int i;
            for (i = BeginChar; i <= EndChar; i++) 
            {
                if (!char.IsWhiteSpace(Text[i])) 
                    break;
            }
            if ((i < EndChar) && char.IsUpper(Text[i])) 
            {
                foreach (string kw in m_Keywords) 
                {
                    if (_checkWord(Text, i, kw)) 
                    {
                        Keyword = kw;
                        i += kw.Length;
                        break;
                    }
                }
            }
            for (; i <= EndChar; i++) 
            {
                if (!char.IsWhiteSpace(Text[i])) 
                    break;
            }
            if (i <= EndChar) 
            {
                if (Text[i] == 'N' || Text[i] == '№') 
                {
                    for (i = i + 1; i <= EndChar; i++) 
                    {
                        if (!char.IsWhiteSpace(Text[i])) 
                            break;
                    }
                }
            }
            if (i <= EndChar) 
            {
                int j;
                for (j = i; j <= EndChar; j++) 
                {
                    char ch = Text[j];
                    if (char.IsDigit(ch)) 
                        continue;
                    if (ch == ')' || ch == '.') 
                        continue;
                    if (char.IsLetter(ch)) 
                    {
                        if (j > BeginChar && char.IsDigit(Text[j - 1])) 
                            continue;
                        if ((j + 1) <= EndChar && Text[j + 1] == ')') 
                            continue;
                    }
                    break;
                }
                if (j > i) 
                {
                    Number = Text.Substring(i, j - i);
                    i = j;
                }
            }
            if (Keyword == "ПРИЛОЖЕНИЕ" || Keyword == "ДОДАТОК") 
            {
                if (i > EndChar) 
                    IsStartOfApp = true;
            }
            IllegalChars = 0;
            for (; i <= EndChar; i++) 
            {
                char ch = Text[i];
                if (char.IsWhiteSpace(ch) || char.IsLetterOrDigit(ch)) 
                    continue;
                if (ch == '_' || ch == '|') 
                {
                    IllegalChars++;
                    continue;
                }
                if (((int)ch) <= 0x80 || ch == '№') 
                    continue;
                IllegalChars++;
            }
        }
        static string[] m_Keywords = new string[] {"СТАТЬЯ", "СТАТТЯ", "ПРИЛОЖЕНИЕ", "ДОДАТОК"};
        static bool _checkWord(string txt, int i, string sub)
        {
            int j;
            for (j = 0; (j < sub.Length) && ((i + j) < txt.Length); j++) 
            {
                if (char.ToUpper(txt[i + j]) != char.ToUpper(sub[j])) 
                    return false;
            }
            if (j >= sub.Length) 
                return true;
            return false;
        }
    }
}