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

namespace Pullenti.Unitext.Internal.Uni
{
    class CorrLine
    {
        public int Length;
        public bool IsPureText;
        public int FirstInd;
        public int LastInd;
        public string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value.TrimEnd();
                Length = value.Length;
                m_CanBeEmptyLineOfTable = -1;
                m_CanHasHorLineOfTable = -1;
            }
        }
        string m_Text;
        public int NewLines = 0;
        public bool PageBreakAfter = false;
        public bool Merge = false;
        public bool CantFollowAny;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Merge) 
                res.Append("<-Merge ");
            res.AppendFormat("[{0}{1}]", Length, (IsPureText ? " txt" : ""));
            if (PageBreakAfter) 
                res.AppendFormat(" PageBreak");
            else 
                res.AppendFormat(" {0}nls", NewLines);
            if (Text.Length < 100) 
                res.AppendFormat(" '{0}'", Text);
            else 
                res.AppendFormat(" '{0}...", Text.Substring(0, 100));
            return res.ToString();
        }
        public static List<CorrLine> ParseList(Pullenti.Unitext.UnitextContainer cnt)
        {
            List<CorrLine> res = new List<CorrLine>();
            CorrLine line = null;
            StringBuilder tmp = new StringBuilder();
            for (int i = 0; i < cnt.Children.Count; i++) 
            {
                Pullenti.Unitext.UnitextItem ch = cnt.Children[i];
                if (ch is Pullenti.Unitext.UnitextPagebreak) 
                {
                    if (line != null) 
                        line.PageBreakAfter = true;
                    continue;
                }
                if (ch is Pullenti.Unitext.UnitextNewline) 
                {
                    if (line != null) 
                    {
                        if (ch.Tag != null) 
                            line.CantFollowAny = true;
                        line.NewLines += (ch as Pullenti.Unitext.UnitextNewline).Count;
                        continue;
                    }
                }
                if (line == null || line.NewLines > 0 || line.PageBreakAfter) 
                {
                    if (line != null) 
                    {
                        line.Text = tmp.ToString();
                        tmp.Length = 0;
                    }
                    line = null;
                }
                if (line == null) 
                {
                    line = new CorrLine() { IsPureText = true };
                    tmp.Length = 0;
                    res.Add(line);
                    line.FirstInd = i;
                }
                line.LastInd = i;
                if (ch is Pullenti.Unitext.UnitextPlaintext) 
                {
                    tmp.Append((ch as Pullenti.Unitext.UnitextPlaintext).Text);
                    continue;
                }
                if (ch is Pullenti.Unitext.UnitextFootnote) 
                {
                }
                else 
                    line.IsPureText = false;
            }
            if (line != null) 
                line.Text = tmp.ToString();
            return res;
        }
        public static bool IsHiphen(char ch)
        {
            if ((ch == '-' || ch == '–' || ch == '¬') || ch == '-') 
                return true;
            if (ch == ((char)0x00AD)) 
                return true;
            if ((ch == '-' || ch == '—' || ch == '–') || ch == '−' || ch == '-') 
                return true;
            return false;
        }
        public static bool IsTableChar(char ch)
        {
            if (IsHiphen(ch)) 
                return true;
            if (ch == '|' || ch == '_') 
                return true;
            int cod = (int)ch;
            if (cod >= 0x2500 && cod <= 0x2595) 
                return true;
            return false;
        }
        public bool CanFollowed(CorrLine li)
        {
            if (!li.IsPureText || !IsPureText) 
                return false;
            if (CantFollowAny) 
                return false;
            if (NewLines > 2) 
                return false;
            if (string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(li.Text)) 
                return false;
            char lastCh = Text[Text.Length - 1];
            char firstCh = (char)0;
            int wsBefore = 0;
            foreach (char ch in li.Text) 
            {
                if (!char.IsWhiteSpace(ch)) 
                {
                    firstCh = ch;
                    break;
                }
                else 
                    wsBefore++;
            }
            if (firstCh == 0) 
                return false;
            if (wsBefore > 10) 
            {
                int wsb = 0;
                foreach (char ch in Text) 
                {
                    if (!char.IsWhiteSpace(ch)) 
                        break;
                    else 
                        wsb++;
                }
                if (wsb < ((int)((0.8 * wsBefore)))) 
                    return false;
            }
            if (lastCh == '.' || lastCh == ';' || lastCh == ':') 
            {
                if (NewLines > 1 || PageBreakAfter) 
                    return false;
                if (!char.IsLetter(firstCh)) 
                    return false;
                if (!char.IsLower(firstCh)) 
                    return false;
                if (lastCh != '.') 
                    return false;
                return true;
            }
            if (char.IsLetter(firstCh)) 
            {
                if (char.IsLower(firstCh)) 
                    return true;
            }
            if (NewLines > 1 || PageBreakAfter) 
                return false;
            if (IsHiphen(lastCh) || lastCh == ',') 
            {
                if (!char.IsLetterOrDigit(firstCh)) 
                    return false;
                return true;
            }
            if (char.IsLetter(lastCh) && char.IsLower(lastCh)) 
            {
                int wsBefore0 = 0;
                foreach (char ch in Text) 
                {
                    if (!char.IsWhiteSpace(ch)) 
                        break;
                    else 
                        wsBefore0++;
                }
                if (wsBefore == wsBefore0) 
                {
                    if (li.Text[li.Text.Length - 1] == '.') 
                    {
                        if (char.IsLower(firstCh) && char.IsLower(firstCh)) 
                            return true;
                    }
                }
            }
            return false;
        }
        int m_CanBeEmptyLineOfTable = -1;
        public bool CanBeEmptyLineOfTable
        {
            get
            {
                if (!IsPureText) 
                    return false;
                if (m_CanBeEmptyLineOfTable >= 0) 
                    return m_CanBeEmptyLineOfTable > 0;
                int cou = 0;
                foreach (char ch in Text) 
                {
                    if (IsTableChar(ch)) 
                        cou++;
                    else if (!char.IsWhiteSpace(ch)) 
                    {
                        m_CanBeEmptyLineOfTable = 0;
                        return false;
                    }
                }
                m_CanBeEmptyLineOfTable = (cou > 1 ? 1 : 0);
                return m_CanBeEmptyLineOfTable > 0;
            }
        }
        int m_CanHasHorLineOfTable = -1;
        public bool CanHasHorLineOfTable
        {
            get
            {
                if (!IsPureText) 
                    return false;
                if (m_CanHasHorLineOfTable >= 0) 
                    return m_CanHasHorLineOfTable > 0;
                for (int i = 0; i < (Text.Length - 2); i++) 
                {
                    if (IsTableChar(Text[i]) && Text[i + 1] == Text[i] && Text[i + 2] == Text[i]) 
                    {
                        m_CanHasHorLineOfTable = 1;
                        return true;
                    }
                }
                m_CanHasHorLineOfTable = 0;
                return false;
            }
        }
    }
}