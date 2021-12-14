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
    class CorrPageText
    {
        public int Number;
        public int Number2;
        public string HeadText;
        public string Text;
        public int I0;
        public int I1;
        public bool Corr = false;
        public void RemoveHeadText()
        {
            if (string.IsNullOrEmpty(HeadText)) 
                return;
            if (m_Top) 
                Text = HeadText + "\r\n" + ((Text ?? ""));
            else 
                Text = ((Text ?? "")) + "\r\n" + HeadText;
        }
        public override string ToString()
        {
            return string.Format("N={0} '{1}'", Number, HeadText ?? "");
        }
        public bool IsNumberDist1(CorrPageText next)
        {
            if (next.Number == 0 || Number == 0) 
                return false;
            if (next.Number == (Number + 1)) 
                return true;
            if (Number2 > 0 && next.Number == (Number2 + 1)) 
                return true;
            if (next.Number2 > 0 && next.Number2 == (Number + 1)) 
                return true;
            if (next.Number2 > 0 && Number2 > 0 && next.Number2 == (Number2 + 1)) 
                return true;
            return false;
        }
        bool m_Top;
        public CorrPageText(string txt, bool top)
        {
            m_Top = top;
            if (txt == null) 
                txt = "";
            int i0;
            int i1;
            int i;
            if (top) 
            {
                i0 = 0;
                for (i1 = i0; i1 < txt.Length; i1++) 
                {
                    if (txt[i1] == 0xD || txt[i1] == 0xA) 
                        break;
                }
                if (i1 > i0) 
                {
                    HeadText = txt.Substring(0, i1).Trim();
                    if (i1 < txt.Length) 
                        Text = txt.Substring(i1).Trim();
                    else 
                        Text = "";
                }
                else 
                    Text = txt;
            }
            else 
            {
                txt = txt.Trim();
                i1 = txt.Length - 1;
                for (i0 = i1; i0 >= 0; i0--) 
                {
                    if (txt[i0] == 0xD || txt[i0] == 0xA) 
                        break;
                }
                if (i1 > i0) 
                {
                    if (i0 < 0) 
                    {
                        HeadText = txt;
                        Text = "";
                    }
                    else 
                    {
                        HeadText = txt.Substring(i0 + 1).Trim();
                        Text = txt.Substring(0, i0).Trim();
                    }
                }
                else 
                    Text = txt;
            }
            if (!string.IsNullOrEmpty(HeadText) && char.IsDigit(HeadText[0])) 
            {
                for (i = 0; i < HeadText.Length; i++) 
                {
                    if (!char.IsDigit(HeadText[i])) 
                        break;
                }
                if (i == HeadText.Length || HeadText[i] == ' ') 
                {
                    int num;
                    if (int.TryParse(HeadText.Substring(0, i), out num)) 
                    {
                        Number = num;
                        HeadText = HeadText.Substring(i).Trim();
                    }
                }
            }
            if (!string.IsNullOrEmpty(HeadText) && char.IsDigit(HeadText[HeadText.Length - 1])) 
            {
                while (true) 
                {
                    for (i = HeadText.Length - 1; i >= 0; i--) 
                    {
                        if (!char.IsDigit(HeadText[i])) 
                            break;
                    }
                    if ((i < 0) || HeadText[i] == ' ') 
                    {
                        int num;
                        if (int.TryParse(HeadText.Substring(i + 1), out num)) 
                        {
                            if (Number > 0) 
                                Number2 = num;
                            else 
                                Number = num;
                            HeadText = HeadText.Substring(0, i + 1).Trim();
                            if (HeadText.EndsWith("СТР", StringComparison.OrdinalIgnoreCase)) 
                                HeadText = HeadText.Substring(0, HeadText.Length - 3).Trim();
                            if (HeadText.EndsWith("СТР.", StringComparison.OrdinalIgnoreCase)) 
                                HeadText = HeadText.Substring(0, HeadText.Length - 4).Trim();
                            string from = null;
                            if (HeadText.EndsWith("ИЗ", StringComparison.OrdinalIgnoreCase)) 
                                from = HeadText.Substring(0, HeadText.Length - 2).Trim();
                            else if (HeadText.EndsWith("/") || HeadText.EndsWith("\\")) 
                                from = HeadText.Substring(0, HeadText.Length - 1).Trim();
                            if (from != null && from.Length > 0 && char.IsDigit(from[from.Length - 1])) 
                            {
                                HeadText = from;
                                Number = 0;
                                continue;
                            }
                        }
                    }
                    break;
                }
            }
            if (top && !string.IsNullOrEmpty(Text) && char.IsDigit(Text[0])) 
            {
                for (i = 0; i < Text.Length; i++) 
                {
                    if (!char.IsDigit(Text[i])) 
                        break;
                }
                if (i >= Text.Length || Text[i] == '\r' || Text[i] == '\n') 
                {
                    int num;
                    if (int.TryParse(Text.Substring(0, i), out num)) 
                    {
                        Number = num;
                        if (i >= Text.Length) 
                            Text = "";
                        else 
                            Text = Text.Substring(i).Trim();
                    }
                }
            }
            if (!top && !string.IsNullOrEmpty(Text) && char.IsDigit(Text[Text.Length - 1])) 
            {
                for (i = Text.Length - 1; i >= 0; i--) 
                {
                    if (!char.IsDigit(Text[i])) 
                        break;
                }
                if ((i < 0) || Text[i] == '\r' || Text[i] == '\n') 
                {
                    int num;
                    if (int.TryParse(Text.Substring(i + 1), out num)) 
                    {
                        Number = num;
                        if (i <= 0) 
                            Text = "";
                        else 
                            Text = Text.Substring(0, i).Trim();
                    }
                }
            }
            if (string.IsNullOrEmpty(HeadText)) 
                HeadText = null;
        }
        public static string Process(List<CorrPageText> pts)
        {
            int cou = 0;
            int i;
            foreach (CorrPageText p in pts) 
            {
                p.Corr = false;
            }
            StringBuilder res = new StringBuilder();
            for (i = 1; i < (pts.Count - 1); i++) 
            {
                if (pts[i - 1].Number > 0 && ((pts[i].Number == 0 || pts[i].Number != (pts[i - 1].Number + 1)))) 
                {
                    int j;
                    bool ok1 = false;
                    bool ok2 = false;
                    int err = 0;
                    for (j = i + 1; j < pts.Count; j++) 
                    {
                        if (pts[j].Number > 0) 
                        {
                            if (pts[j].Number == (pts[i - 1].Number + (((j - i) + 1)))) 
                            {
                                ok1 = true;
                                break;
                            }
                            if (((j + 2) < pts.Count) && pts[j + 1].Number == (pts[j].Number + 1) && pts[j + 2].Number == (pts[j].Number + 2)) 
                            {
                                ok2 = true;
                                break;
                            }
                            if ((++err) > 10) 
                                break;
                        }
                    }
                    if (ok1 || ok2) 
                    {
                        for (int k = i; k < j; k++) 
                        {
                            pts[k].Number = 0;
                        }
                    }
                }
            }
            bool noCorrFirst = false;
            if (pts.Count > 0 && pts[0].Number > 1900) 
            {
                pts[0].Number = 0;
                noCorrFirst = true;
            }
            if (pts.Count > 2) 
            {
                if (pts[0].Number != 1 && pts[1].Number > 0 && pts[2].Number == (pts[1].Number + 1)) 
                {
                    if (pts[0].Number != (pts[1].Number - 1)) 
                    {
                        pts[0].Number = pts[1].Number - 1;
                        noCorrFirst = true;
                    }
                }
            }
            int firstPage = 0;
            if (pts.Count > 4 && pts[0].Number == 0) 
            {
                for (i = 0; i < (pts.Count - 5); i++) 
                {
                    if (pts[i].Number != 0) 
                    {
                        bool ok1 = true;
                        for (int j = i + 1; (j < pts.Count) && (j < (i + 4)); j++) 
                        {
                            if (!pts[j].IsNumberDist1(pts[j + 1])) 
                            {
                                ok1 = false;
                                break;
                            }
                        }
                        if (ok1) 
                        {
                            firstPage = i;
                            for (--i; i >= 0; i--) 
                            {
                                pts[i].Number = pts[i + 1].Number - 1;
                            }
                        }
                        break;
                    }
                }
            }
            int errcou = 0;
            for (i = 0; i < (pts.Count - 1); i++) 
            {
                if (pts[i + 1].Number <= 0) 
                {
                    if ((i + 1) < pts.Count) 
                        continue;
                    errcou++;
                    continue;
                }
                if (pts[i].Number <= 0) 
                {
                    if (i <= firstPage) 
                        continue;
                    errcou++;
                    continue;
                }
                if (pts[i].IsNumberDist1(pts[i + 1])) 
                    cou++;
                else if (i == 0) 
                {
                }
                else 
                    errcou++;
            }
            bool ok = false;
            if (cou == 0) 
            {
                if ((pts.Count == 3 && pts[1].Number == 2 && pts[0].Number == 0) && pts[2].Number == 0) 
                    ok = true;
                else if (pts.Count == 2 && pts[1].Number == 2 && ((pts[0].Number == 1 || pts[0].Number == 0))) 
                    ok = true;
                else 
                    ok = false;
            }
            else if (errcou == 0) 
                ok = true;
            Dictionary<string, int> colstat = new Dictionary<string, int>();
            for (i = 1; i < pts.Count; i++) 
            {
                if (pts[i].HeadText != null) 
                {
                    if (colstat.ContainsKey(pts[i].HeadText)) 
                        colstat[pts[i].HeadText]++;
                    else 
                        colstat.Add(pts[i].HeadText, 1);
                }
            }
            int lev = pts.Count / 2;
            if (lev < 2) 
                lev = 2;
            for (i = 1; i < pts.Count; i++) 
            {
                string h = pts[i].HeadText;
                if (h == null) 
                    continue;
                int c = colstat[h];
                if (c >= lev) 
                    pts[i].Corr = true;
                else 
                    pts[i].RemoveHeadText();
            }
            foreach (KeyValuePair<string, int> kp in colstat) 
            {
                if (kp.Value >= lev && kp.Key.Length > 5) 
                {
                    if (res.Length > 0) 
                        res.Append("\r\n");
                    res.Append(kp.Key);
                }
            }
            if (cou > (errcou * 3)) 
            {
                foreach (CorrPageText p in pts) 
                {
                    if (p.Number > 0) 
                    {
                        if (noCorrFirst && p == pts[0]) 
                            continue;
                        if (!p.Corr) 
                            p.RemoveHeadText();
                        p.Corr = true;
                    }
                }
            }
            return (res.Length == 0 ? null : res.ToString());
        }
    }
}