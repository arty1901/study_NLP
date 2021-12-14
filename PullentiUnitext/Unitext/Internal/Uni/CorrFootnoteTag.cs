/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Uni
{
    class CorrFootnoteTag
    {
        public CorrFootnoteTyps Typ;
        public int Number;
        public static CorrFootnoteTag TryParse(string text, ref int i, bool forContaiter, bool robust = false)
        {
            for (; i < text.Length; i++) 
            {
                if (!char.IsWhiteSpace(text[i])) 
                    break;
            }
            if (i >= text.Length) 
                return null;
            if (text[i] != '<') 
            {
                if (robust && text[i] == '*') 
                {
                    CorrFootnoteTag res1 = new CorrFootnoteTag() { Typ = CorrFootnoteTyps.Stars };
                    for (; i < text.Length; i++) 
                    {
                        if (text[i] != '*') 
                        {
                            i--;
                            break;
                        }
                        else 
                            res1.Number++;
                    }
                    return res1;
                }
                if (((((i + 10) < text.Length) && text[i] == 'С' && text[i + 1] == 'н') && text[i + 2] == 'о' && text[i + 3] == 'с') && text[i + 4] == 'к' && text[i + 5] == 'а') 
                {
                    int j;
                    int num = 0;
                    for (j = i + 7; j < text.Length; j++) 
                    {
                        if (text[j] == ':' && num > 0) 
                        {
                            CorrFootnoteTag res1 = new CorrFootnoteTag() { Typ = CorrFootnoteTyps.Digit, Number = num };
                            i = j;
                            return res1;
                        }
                        else if (char.IsDigit(text[j])) 
                            num = (num * 10) + ((int)((text[j] - '0')));
                        else 
                            break;
                    }
                }
                if (char.IsDigit(text[i]) && forContaiter) 
                {
                    int j;
                    int num = 0;
                    for (j = i; j < text.Length; j++) 
                    {
                        if (char.IsDigit(text[j])) 
                            num = (num * 10) + ((int)((text[j] - '0')));
                        else if (text[j] == ')') 
                        {
                            CorrFootnoteTag res1 = new CorrFootnoteTag() { Typ = CorrFootnoteTyps.VeryDoubt, Number = num };
                            i = j;
                            return res1;
                        }
                        else 
                            break;
                    }
                }
                return null;
            }
            if ((i + 2) >= text.Length) 
                return null;
            int ii = i + 1;
            if (text[ii] != '*' && !char.IsDigit(text[ii])) 
                return null;
            CorrFootnoteTag res = new CorrFootnoteTag() { Typ = (text[ii] == '*' ? CorrFootnoteTyps.Stars : CorrFootnoteTyps.Digit) };
            for (; ii < text.Length; ii++) 
            {
                if (text[ii] == '*' && res.Typ == CorrFootnoteTyps.Stars) 
                    res.Number++;
                else if (char.IsDigit(text[ii]) && res.Typ == CorrFootnoteTyps.Digit) 
                    res.Number = (res.Number * 10) + ((int)((text[ii] - '0')));
                else if (text[ii] == '>') 
                {
                    i = ii;
                    return res;
                }
                else 
                    break;
            }
            return null;
        }
    }
}