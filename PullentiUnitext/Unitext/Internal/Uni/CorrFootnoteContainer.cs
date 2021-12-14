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
    class CorrFootnoteContainer
    {
        public CorrFootnoteTyps Typ = CorrFootnoteTyps.Digit;
        public int StartNumber = 1;
        public int BeginInd;
        public int EndInd;
        public List<string> Items = new List<string>();
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < Items.Count; i++) 
            {
                res.AppendFormat("<{0}>{1} \r\n", (Typ == CorrFootnoteTyps.Digit || Typ == CorrFootnoteTyps.SupDigit ? ((StartNumber + i)).ToString() : "*"), Items[i]);
            }
            return res.ToString();
        }
        public static CorrFootnoteContainer TryParse1(Pullenti.Unitext.UnitextItem it)
        {
            if (it is Pullenti.Unitext.UnitextPlaintext) 
            {
                Pullenti.Unitext.UnitextPlaintext pl = it as Pullenti.Unitext.UnitextPlaintext;
                int i = 0;
                CorrFootnoteTag tag = CorrFootnoteTag.TryParse(pl.Text, ref i, true, false);
                if (tag == null || tag.Number != 1) 
                    return null;
                CorrFootnoteContainer res = new CorrFootnoteContainer() { Typ = tag.Typ };
                string txt = pl.Text.Substring(i + 1).Trim();
                for (i = 0; i < txt.Length; i++) 
                {
                    int j = i;
                    CorrFootnoteTag tag1 = CorrFootnoteTag.TryParse(txt, ref j, true, false);
                    if (tag1 == null) 
                        continue;
                    if (tag1.Typ != tag.Typ) 
                        continue;
                    if (tag1.Number != (res.Items.Count + 2)) 
                        continue;
                    if (i > 0) 
                        res.Items.Add(txt.Substring(0, i).Trim());
                    txt = txt.Substring(j + 1).Trim();
                    i = -1;
                }
                if (!string.IsNullOrEmpty(txt)) 
                    res.Items.Add(txt);
                return res;
            }
            if (it is Pullenti.Unitext.UnitextContainer) 
            {
                Pullenti.Unitext.UnitextContainer cnt = it as Pullenti.Unitext.UnitextContainer;
                CorrFootnoteContainer res = TryParse(cnt.Children, 0, -1);
                if (res == null) 
                    return null;
                if (res.EndInd != (cnt.Children.Count - 1)) 
                    return null;
                return res;
            }
            return null;
        }
        public static CorrFootnoteContainer TryParse(List<Pullenti.Unitext.UnitextItem> items, int ind, int numStart)
        {
            if ((ind < 1) || (ind + 1) > items.Count) 
                return null;
            if (!(items[ind - 1] is Pullenti.Unitext.UnitextNewline) && !(items[ind - 1] is Pullenti.Unitext.UnitextPagebreak)) 
                return null;
            int ind0 = ind;
            Pullenti.Unitext.UnitextPlaintext it = items[ind] as Pullenti.Unitext.UnitextPlaintext;
            if (it == null || string.IsNullOrEmpty(it.Text)) 
                return null;
            if (string.IsNullOrEmpty(it.Text.Trim()) && ((ind + 1) < items.Count) && (items[ind + 1] is Pullenti.Unitext.UnitextPlaintext)) 
            {
                ind++;
                it = items[ind] as Pullenti.Unitext.UnitextPlaintext;
            }
            int i = 0;
            CorrFootnoteTag tag = CorrFootnoteTag.TryParse(it.Text, ref i, true, false);
            if (tag == null) 
            {
                int nn;
                if (it.Typ == Pullenti.Unitext.UnitextPlaintextType.Sup && int.TryParse(it.Text.Trim(), out nn)) 
                {
                    tag = new CorrFootnoteTag() { Number = nn, Typ = CorrFootnoteTyps.SupDigit };
                    if (ind != ind0) 
                    {
                        items.RemoveAt(ind0);
                        ind = ind0;
                    }
                }
                else 
                    return null;
            }
            else if (tag.Typ == CorrFootnoteTyps.VeryDoubt) 
            {
                if (tag.Number == 24) 
                {
                }
                bool ok = false;
                bool hasLines = false;
                for (int j = ind - 1; j >= 0 && (j > (ind - 10)); j--) 
                {
                    Pullenti.Unitext.UnitextItem itt = items[j];
                    if (itt.IsWhitespaces) 
                        continue;
                    string ttt = itt.GetPlaintextString(null);
                    if (string.IsNullOrEmpty(ttt)) 
                        continue;
                    if (ttt.StartsWith("-----")) 
                    {
                        hasLines = true;
                        continue;
                    }
                    if (!hasLines) 
                        break;
                    for (int ii = 0; ii < ttt.Length; ii++) 
                    {
                        CorrFootnoteTag tag0 = CorrFootnoteTag.TryParse(ttt, ref ii, false, false);
                        if (tag0 != null) 
                        {
                            if (tag0.Number == tag.Number && tag0.Typ == CorrFootnoteTyps.Digit) 
                            {
                                ok = true;
                                break;
                            }
                        }
                    }
                }
                if (!ok) 
                    return null;
                tag.Typ = CorrFootnoteTyps.Digit;
            }
            CorrFootnoteContainer res = new CorrFootnoteContainer() { Typ = tag.Typ };
            res.BeginInd = ind0;
            res.EndInd = ind;
            if (tag.Typ == CorrFootnoteTyps.Digit || tag.Typ == CorrFootnoteTyps.SupDigit) 
                res.StartNumber = tag.Number;
            for (--ind; ind < items.Count; ind++) 
            {
                if ((items[ind] is Pullenti.Unitext.UnitextNewline) || (items[ind] is Pullenti.Unitext.UnitextPagebreak)) 
                    continue;
                if (!(items[ind - 1] is Pullenti.Unitext.UnitextNewline) && !(items[ind - 1] is Pullenti.Unitext.UnitextPagebreak)) 
                    break;
                it = items[ind] as Pullenti.Unitext.UnitextPlaintext;
                if (it == null) 
                    break;
                i = 0;
                CorrFootnoteTag tag1 = CorrFootnoteTag.TryParse(it.Text, ref i, true, false);
                if (tag1 == null) 
                {
                    int nn;
                    if (it.Typ == Pullenti.Unitext.UnitextPlaintextType.Sup && int.TryParse(it.Text.Trim(), out nn)) 
                        tag1 = new CorrFootnoteTag() { Number = nn, Typ = CorrFootnoteTyps.SupDigit };
                }
                if (tag1 == null) 
                    break;
                if (tag1.Typ == CorrFootnoteTyps.VeryDoubt) 
                {
                    if (ind == res.EndInd) 
                        tag1.Typ = CorrFootnoteTyps.Digit;
                    else 
                        break;
                }
                if (tag.Typ != tag1.Typ) 
                    break;
                if ((tag1.Number - res.StartNumber) != res.Items.Count) 
                    break;
                res.EndInd = ind;
                string txt = (tag.Typ == CorrFootnoteTyps.SupDigit ? "" : it.Text.Substring(i + 1).Trim());
                for (int jj = ind + 1; jj < items.Count; jj++) 
                {
                    if (items[jj] is Pullenti.Unitext.UnitextNewline) 
                    {
                        if ((items[jj] as Pullenti.Unitext.UnitextNewline).Count > 1) 
                            break;
                        continue;
                    }
                    if (items[jj] is Pullenti.Unitext.UnitextComment) 
                        continue;
                    string txt1 = null;
                    Pullenti.Unitext.UnitextPlaintext pl = items[jj] as Pullenti.Unitext.UnitextPlaintext;
                    if (pl != null) 
                        txt1 = pl.Text;
                    else if (items[jj] is Pullenti.Unitext.UnitextHyperlink) 
                        txt1 = items[jj].GetPlaintextString(null);
                    if (txt1 == null) 
                        break;
                    if (txt1.StartsWith("Текст ", StringComparison.OrdinalIgnoreCase)) 
                        break;
                    i = 0;
                    if (CorrFootnoteTag.TryParse(txt1, ref i, true, false) != null) 
                        break;
                    if (it.Typ == Pullenti.Unitext.UnitextPlaintextType.Sup && int.TryParse(txt1.Trim(), out i)) 
                        break;
                    if (tag.Typ == CorrFootnoteTyps.SupDigit && jj == (ind + 1)) 
                    {
                    }
                    else if (txt.Length > ((int)((1.2 * it.Text.Length)))) 
                    {
                        if (jj > 0 && (items[jj - 1] is Pullenti.Unitext.UnitextComment)) 
                        {
                        }
                        else 
                            break;
                    }
                    int chs = 0;
                    int hiphs = 0;
                    foreach (char ch in txt1) 
                    {
                        if (char.IsLetterOrDigit(ch)) 
                            chs++;
                    }
                    if (txt1.Length > 1 && chs == 0) 
                    {
                        res.EndInd = (ind = jj);
                        break;
                    }
                    foreach (char ch in txt1) 
                    {
                        if (char.IsLetterOrDigit(ch)) 
                            break;
                        else if (ch == '_' || ch == '-') 
                            hiphs++;
                    }
                    if (hiphs > 10) 
                    {
                        res.EndInd = (ind = jj);
                        break;
                    }
                    if (txt.Length == 0) 
                        txt = txt1.Trim();
                    else 
                        txt = string.Format("{0} {1}", txt, txt1.Trim());
                    res.EndInd = (ind = jj);
                }
                res.Items.Add(txt);
            }
            bool delimLine = false;
            ind = res.BeginInd;
            if (ind >= 2 && (items[ind - 1] is Pullenti.Unitext.UnitextNewline)) 
            {
                it = items[ind - 2] as Pullenti.Unitext.UnitextPlaintext;
                if (it != null) 
                {
                    int cou = 0;
                    i = 0;
                    for (i = 0; i < it.Text.Length; i++) 
                    {
                        if (CorrLine.IsHiphen(it.Text[i]) || it.Text[i] == '_') 
                            cou++;
                        else if (!char.IsWhiteSpace(it.Text[i])) 
                            break;
                    }
                    if (cou > 0 && i >= it.Text.Length) 
                    {
                        res.BeginInd -= 2;
                        delimLine = true;
                    }
                }
            }
            if (res.StartNumber != 1 && res.StartNumber != numStart) 
            {
                if (!delimLine && (res.Items.Count < 2)) 
                {
                    if (res.Typ != CorrFootnoteTyps.SupDigit) 
                        return null;
                }
            }
            return res;
        }
        public Pullenti.Unitext.UnitextItem CorrectItem(Pullenti.Unitext.UnitextItem it, bool robust)
        {
            Pullenti.Unitext.UnitextPlaintext pl = it as Pullenti.Unitext.UnitextPlaintext;
            if (pl != null) 
            {
                if (Typ == CorrFootnoteTyps.SupDigit && pl.Typ == Pullenti.Unitext.UnitextPlaintextType.Sup) 
                {
                    int num;
                    if (int.TryParse(pl.Text.Trim(), out num)) 
                    {
                        int ii = num - StartNumber;
                        if ((ii < 0) || ii >= Items.Count) 
                            return pl;
                        Pullenti.Unitext.UnitextFootnote fn = new Pullenti.Unitext.UnitextFootnote();
                        fn.Content = new Pullenti.Unitext.UnitextPlaintext() { Text = Items[ii] };
                        fn.CustomMark = num.ToString();
                        return fn;
                    }
                }
                Pullenti.Unitext.UnitextContainer cnt = null;
                string txt = pl.Text;
                for (int i = 0; i < txt.Length; i++) 
                {
                    if (char.IsWhiteSpace(txt[i])) 
                        continue;
                    int j = i;
                    CorrFootnoteTag tag = CorrFootnoteTag.TryParse(txt, ref j, false, robust);
                    if (tag == null) 
                        continue;
                    if (tag.Typ != Typ) 
                        continue;
                    int ii = tag.Number - StartNumber;
                    if ((ii < 0) || ii >= Items.Count) 
                        continue;
                    if (cnt == null) 
                        cnt = new Pullenti.Unitext.UnitextContainer();
                    if (i > 0) 
                        cnt.Children.Add(new Pullenti.Unitext.UnitextPlaintext() { Text = txt.Substring(0, i) });
                    cnt.Children.Add(new Pullenti.Unitext.UnitextFootnote() { Content = new Pullenti.Unitext.UnitextPlaintext() { Text = Items[ii] } });
                    j++;
                    if (j >= txt.Length) 
                    {
                        txt = "";
                        break;
                    }
                    txt = txt.Substring(j);
                    i = -1;
                }
                if (!string.IsNullOrEmpty(txt) && cnt != null) 
                    cnt.Children.Add(new Pullenti.Unitext.UnitextPlaintext() { Text = txt });
                return cnt;
            }
            if (it is Pullenti.Unitext.UnitextContainer) 
            {
                Pullenti.Unitext.UnitextContainer cnt = it as Pullenti.Unitext.UnitextContainer;
                bool isCh = false;
                for (int i = 0; i < cnt.Children.Count; i++) 
                {
                    Pullenti.Unitext.UnitextItem ch = this.CorrectItem(cnt.Children[i], robust);
                    if (ch == null) 
                        continue;
                    cnt.Children[i] = ch;
                    isCh = true;
                }
                return (isCh ? cnt : null);
            }
            if (it is Pullenti.Unitext.UnitextTable) 
            {
                Pullenti.Unitext.UnitextTable tab = it as Pullenti.Unitext.UnitextTable;
                bool isCh = false;
                for (int r = 0; r < tab.RowsCount; r++) 
                {
                    for (int c = 0; c < tab.ColsCount; c++) 
                    {
                        Pullenti.Unitext.UnitextTablecell cel = tab.GetCell(r, c);
                        if (cel == null) 
                            continue;
                        if (cel.RowBegin != r || cel.ColBegin != c || cel.Content == null) 
                            continue;
                        Pullenti.Unitext.UnitextItem cnt = this.CorrectItem(cel.Content, robust);
                        if (cnt != null) 
                        {
                            cel.Content = cnt;
                            isCh = true;
                        }
                    }
                }
                return (isCh ? tab : null);
            }
            if (it is Pullenti.Unitext.UnitextList) 
            {
                Pullenti.Unitext.UnitextList li = it as Pullenti.Unitext.UnitextList;
                bool isCh = false;
                foreach (Pullenti.Unitext.UnitextListitem ii in li.Items) 
                {
                    Pullenti.Unitext.UnitextItem cnt = this.CorrectItem(ii.Content, robust);
                    if (cnt != null) 
                    {
                        isCh = true;
                        ii.Content = cnt;
                    }
                    Pullenti.Unitext.UnitextList sub = this.CorrectItem(ii.Sublist, robust) as Pullenti.Unitext.UnitextList;
                    if (sub != null) 
                    {
                        ii.Sublist = sub;
                        isCh = true;
                    }
                }
                return (isCh ? li : null);
            }
            return null;
        }
    }
}