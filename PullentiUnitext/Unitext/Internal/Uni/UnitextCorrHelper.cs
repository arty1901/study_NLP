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
    static class UnitextCorrHelper
    {
        public static void RemovePageBreakNumeration(Pullenti.Unitext.UnitextDocument doc)
        {
            Pullenti.Unitext.UnitextContainer cnt = doc.Content as Pullenti.Unitext.UnitextContainer;
            if (cnt == null) 
                return;
            StringBuilder tmp = new StringBuilder();
            Pullenti.Unitext.UnitextContainer tmpCnt = new Pullenti.Unitext.UnitextContainer();
            bool corr = false;
            for (int k = 0; k < 2; k++) 
            {
                List<CorrPageText> pages = new List<CorrPageText>();
                for (int i = 0; i < cnt.Children.Count; i++) 
                {
                    int i0;
                    int i1;
                    tmpCnt.Children.Clear();
                    if (k == 0) 
                    {
                        if (i == 0) 
                        {
                        }
                        else if ((cnt.Children[i - 1] is Pullenti.Unitext.UnitextPagebreak) && (cnt.Children[i] is Pullenti.Unitext.UnitextPlaintext)) 
                        {
                        }
                        else 
                            continue;
                        i0 = i;
                        for (i1 = i0; i1 < cnt.Children.Count; i1++) 
                        {
                            Pullenti.Unitext.UnitextItem chh = cnt.Children[i1];
                            if (chh is Pullenti.Unitext.UnitextPlaintext) 
                            {
                            }
                            else if (chh is Pullenti.Unitext.UnitextHyperlink) 
                            {
                            }
                            else 
                                break;
                            tmpCnt.Children.Add(chh);
                        }
                        i1--;
                    }
                    else 
                    {
                        if (i == (cnt.Children.Count - 1)) 
                            i1 = i;
                        else if (cnt.Children[i] is Pullenti.Unitext.UnitextPagebreak) 
                            i1 = i - 1;
                        else 
                            continue;
                        for (i0 = i1; i0 >= 0; i0--) 
                        {
                            Pullenti.Unitext.UnitextItem chh = cnt.Children[i0];
                            if (chh is Pullenti.Unitext.UnitextPlaintext) 
                            {
                            }
                            else if (chh is Pullenti.Unitext.UnitextHyperlink) 
                            {
                            }
                            else 
                                break;
                            tmpCnt.Children.Add(chh);
                        }
                        i0++;
                    }
                    if (tmpCnt.Children.Count == 0) 
                        continue;
                    tmp.Length = 0;
                    tmpCnt.GetPlaintext(tmp, new Pullenti.Unitext.GetPlaintextParam() { SetPositions = false });
                    if (tmp.Length == 0) 
                        continue;
                    CorrPageText page = new CorrPageText(tmp.ToString(), k == 0) { I0 = i0, I1 = i1 };
                    pages.Add(page);
                }
                if (pages.Count < 2) 
                    continue;
                string tittext = CorrPageText.Process(pages);
                int k1 = k;
                for (int i = pages.Count - 1; i >= 0; i--) 
                {
                    if (pages[i].Corr) 
                    {
                        if (i == 0) 
                        {
                        }
                        for (int j = pages[i].I0; j <= pages[i].I1; j++) 
                        {
                            Pullenti.Unitext.UnitextPlaintext pl = cnt.Children[j] as Pullenti.Unitext.UnitextPlaintext;
                            if (pl != null && pl.Layout != null) 
                            {
                                for (int jj = 0; jj < pl.Layout.Length; jj++) 
                                {
                                    if (pl.Layout[jj] != null) 
                                        pl.Layout[jj].Ignored = true;
                                }
                            }
                        }
                        cnt.Children.RemoveRange(pages[i].I0, (pages[i].I1 + 1) - pages[i].I0);
                        corr = true;
                        k1 = k - 1;
                    }
                }
                if (tittext != null) 
                {
                }
                k = k1;
            }
            for (int i = cnt.Children.Count - 1; i >= 0; i--) 
            {
                if (i >= cnt.Children.Count) 
                    continue;
                Pullenti.Unitext.UnitextPlaintext pt = cnt.Children[i] as Pullenti.Unitext.UnitextPlaintext;
                if (pt == null) 
                    continue;
                int n;
                if (!int.TryParse(pt.Text.Trim(), out n)) 
                    continue;
                if (i == 0 || (cnt.Children[i - 1] is Pullenti.Unitext.UnitextPagebreak) || ((i > 1 && (cnt.Children[i - 1] is Pullenti.Unitext.UnitextNewline) && (cnt.Children[i - 2] is Pullenti.Unitext.UnitextPagebreak)))) 
                {
                    if (((i + 1) < cnt.Children.Count) && (cnt.Children[i + 1] is Pullenti.Unitext.UnitextNewline)) 
                    {
                        if (pt.Layout != null) 
                        {
                            foreach (Pullenti.Unitext.UnilayRectangle la in pt.Layout) 
                            {
                                if (la != null) 
                                {
                                    la.Ignored = true;
                                    la.Page.TopNumber = n;
                                }
                            }
                        }
                        cnt.Children.RemoveRange(i, 2);
                        continue;
                    }
                }
                else if ((i + 1) == cnt.Children.Count || (cnt.Children[i + 1] is Pullenti.Unitext.UnitextPagebreak) || ((((i + 2) < cnt.Children.Count) && (cnt.Children[i + 1] is Pullenti.Unitext.UnitextNewline) && (cnt.Children[i + 2] is Pullenti.Unitext.UnitextPagebreak)))) 
                {
                    if (i > 0 && (cnt.Children[i - 1] is Pullenti.Unitext.UnitextNewline)) 
                    {
                        if (pt.Layout != null) 
                        {
                            foreach (Pullenti.Unitext.UnilayRectangle la in pt.Layout) 
                            {
                                if (la != null) 
                                {
                                    la.Ignored = true;
                                    la.Page.BottomNumber = n;
                                }
                            }
                        }
                        cnt.Children.RemoveRange(i - 1, 2);
                        continue;
                    }
                }
            }
            if (corr) 
                doc.Optimize(false, null);
        }
        const int MinLineLen = 50;
        const int MaxLineLen = 100;
        public static void RemoveFalseNewLines(Pullenti.Unitext.UnitextDocument doc, bool replaceNbsp)
        {
            if (doc == null || doc.Content == null) 
                return;
            Pullenti.Unitext.UnitextContainer cnt = doc.Content as Pullenti.Unitext.UnitextContainer;
            if (cnt == null) 
                return;
            int cr1 = 0;
            int cr2 = 0;
            int crA = 0;
            foreach (Pullenti.Unitext.UnitextItem v in cnt.Children) 
            {
                if (v is Pullenti.Unitext.UnitextNewline) 
                {
                    int co = (v as Pullenti.Unitext.UnitextNewline).Count;
                    if (co == 1) 
                        cr1++;
                    else if (co == 2) 
                        cr2++;
                    else 
                        crA++;
                }
            }
            if (cr2 > (((cr1 + crA)) * 3)) 
                cnt.Correct(LocCorrTyp.MergeNewlines, null);
            List<CorrLine> lines = CorrLine.ParseList(cnt);
            int cou = 0;
            int len = 0;
            int nbspCount = 0;
            int spCount = 0;
            foreach (CorrLine l in lines) 
            {
                if (l.IsPureText) 
                {
                    foreach (char ch in l.Text) 
                    {
                        if (ch == ' ') 
                            spCount++;
                        else if (ch == ((char)0xA0)) 
                            nbspCount++;
                    }
                }
                if ((l.IsPureText && l.NewLines == 1) || l.PageBreakAfter) 
                {
                    cou++;
                    len += l.Length;
                }
            }
            if (replaceNbsp && nbspCount > 100 && nbspCount > (spCount * 2)) 
            {
                doc.Content.Correct(LocCorrTyp.Npsp2Sp, null);
                foreach (Pullenti.Unitext.UnitextPagesection s in doc.Sections) 
                {
                    if (s.Header != null) 
                        s.Header.Correct(LocCorrTyp.Npsp2Sp, null);
                    if (s.Footer != null) 
                        s.Footer.Correct(LocCorrTyp.Npsp2Sp, null);
                }
            }
            if (cou == 0) 
                return;
            len /= cou;
            if (cou < 3) 
                return;
            if ((len < MinLineLen) || len > MaxLineLen) 
                return;
            int minLen = (int)((0.80 * len));
            int maxLen = (int)((1.20 * len));
            for (int i = 0; i < (lines.Count - 1); i++) 
            {
                CorrLine l1 = lines[i];
                CorrLine l2 = lines[i + 1];
                if (!l1.IsPureText || !l2.IsPureText) 
                    continue;
                if (l1.Length < minLen) 
                    continue;
                if (l1.CanFollowed(l2)) 
                    l2.Merge = true;
            }
            bool changed = false;
            for (int i = lines.Count - 1; i > 0; i--) 
            {
                if (lines[i].Merge) 
                {
                    if (lines[i - 1].PageBreakAfter) 
                        continue;
                    int i0 = lines[i - 1].LastInd + 1;
                    int i1 = lines[i].FirstInd - 1;
                    if (i0 <= i1) 
                    {
                        changed = true;
                        if (i0 < i1) 
                            cnt.Children.RemoveRange(i0 + 1, i1 - i0);
                        cnt.Children[i0] = new Pullenti.Unitext.UnitextPlaintext() { Text = " " };
                    }
                }
            }
            if (changed) 
                doc.Optimize(false, null);
        }
        public static void RestoreTextFootnotes(Pullenti.Unitext.UnitextDocument doc)
        {
            if (doc == null || doc.Content == null) 
                return;
            if (doc.Content is Pullenti.Unitext.UnitextTable) 
            {
                RestoreTextFootnotesInTable(doc.Content as Pullenti.Unitext.UnitextTable);
                return;
            }
            Pullenti.Unitext.UnitextContainer cnt = doc.Content as Pullenti.Unitext.UnitextContainer;
            if (cnt == null) 
                return;
            List<CorrFootnoteContainer> fcl = new List<CorrFootnoteContainer>();
            int numStart = 1;
            for (int i = 0; i < cnt.Children.Count; i++) 
            {
                if (cnt.Children[i] is Pullenti.Unitext.UnitextTable) 
                {
                    RestoreTextFootnotesInTable(cnt.Children[i] as Pullenti.Unitext.UnitextTable);
                    continue;
                }
                CorrFootnoteContainer fc = CorrFootnoteContainer.TryParse(cnt.Children, i, numStart);
                if (fc == null) 
                    continue;
                fcl.Add(fc);
                if (fcl.Count == 19) 
                {
                }
                i = fc.EndInd;
                numStart += fc.Items.Count;
            }
            if (fcl.Count == 0) 
                return;
            bool isChanged = false;
            for (int i = fcl.Count - 1; i >= 0; i--) 
            {
                int ind0 = 0;
                int ind1 = fcl[i].BeginInd - 1;
                if (i > 0) 
                    ind0 = fcl[i - 1].EndInd + 1;
                if (i == 8) 
                {
                }
                bool corr = false;
                for (int k = 0; k < 2; k++) 
                {
                    for (int j = ind0; j <= ind1; j++) 
                    {
                        Pullenti.Unitext.UnitextItem it = fcl[i].CorrectItem(cnt.Children[j], k == 1);
                        if (it != null) 
                        {
                            cnt.Children[j] = it;
                            corr = true;
                        }
                    }
                    if (corr || fcl[i].Typ != CorrFootnoteTyps.Stars) 
                        break;
                }
                if (corr) 
                {
                    isChanged = true;
                    cnt.Children.RemoveRange(fcl[i].BeginInd, (fcl[i].EndInd + 1) - fcl[i].BeginInd);
                    if (((fcl[i].BeginInd < cnt.Children.Count) && (cnt.Children[fcl[i].BeginInd] is Pullenti.Unitext.UnitextNewline) && fcl[i].BeginInd > 0) && (cnt.Children[fcl[i].BeginInd - 1] is Pullenti.Unitext.UnitextNewline)) 
                        cnt.Children.RemoveAt(fcl[i].BeginInd);
                }
            }
            if (isChanged) 
                doc.Optimize(false, null);
        }
        static void RestoreTextFootnotesInTable(Pullenti.Unitext.UnitextTable tab)
        {
            bool isChanged = false;
            for (int r = 0; r < tab.RowsCount; r++) 
            {
                for (int c = 0; c < tab.ColsCount; c++) 
                {
                    Pullenti.Unitext.UnitextTablecell cel = tab.GetCell(r, c);
                    if (cel == null) 
                        continue;
                    if (cel.ColBegin != c || cel.RowBegin != r) 
                        continue;
                    if (cel.Content == null) 
                        continue;
                    CorrFootnoteContainer fn = CorrFootnoteContainer.TryParse1(cel.Content);
                    if (fn == null) 
                        continue;
                    for (int cc = cel.ColBegin; cc <= cel.ColEnd; cc++) 
                    {
                        Pullenti.Unitext.UnitextTablecell cel0 = tab.GetCell(r - 1, cc);
                        if (cel0 == null) 
                            continue;
                        if (cel0.Content == null) 
                            continue;
                        Pullenti.Unitext.UnitextItem re = fn.CorrectItem(cel0.Content, false);
                        if (re == null) 
                            continue;
                        cel0.Content = re;
                        cel.Content = null;
                        isChanged = true;
                    }
                }
            }
            if (isChanged) 
                tab.Optimize(false, null);
        }
        public static bool RestoreTables(Pullenti.Unitext.UnitextDocument doc)
        {
            Pullenti.Unitext.UnitextContainer cnt = doc.Content as Pullenti.Unitext.UnitextContainer;
            if (cnt != null) 
                return _restoreTables(cnt);
            return false;
        }
        static bool _restoreTables(Pullenti.Unitext.UnitextContainer cnt)
        {
            if (cnt == null) 
                return false;
            bool ret = false;
            foreach (Pullenti.Unitext.UnitextItem ch in cnt.Children) 
            {
                if (ch is Pullenti.Unitext.UnitextContainer) 
                {
                    if (_restoreTables(ch as Pullenti.Unitext.UnitextContainer)) 
                        ret = true;
                }
            }
            List<CorrLine> lines = CorrLine.ParseList(cnt);
            List<CorrTable> tabs = new List<CorrTable>();
            for (int i = 0; i < lines.Count; i++) 
            {
                int ii;
                CorrTable tab = CorrTable.TryParse(lines, i, out ii);
                if (tab != null) 
                {
                    tabs.Add(tab);
                    i = tab.EndInd;
                }
                else 
                    i = ii;
            }
            if (tabs.Count == 0) 
                return ret;
            for (int i = tabs.Count - 1; i >= 0; i--) 
            {
                int i0 = lines[tabs[i].BeginInd].FirstInd;
                int i1 = lines[tabs[i].EndInd].LastInd;
                if (i0 < i1) 
                {
                    cnt.Children.RemoveRange(i0 + 1, i1 - i0);
                    cnt.Children[i0] = tabs[i].Result;
                }
            }
            if (cnt.Typ == Pullenti.Unitext.UnitextContainerType.Monospace) 
                cnt.Typ = Pullenti.Unitext.UnitextContainerType.Undefined;
            return true;
        }
        internal static string CorrNbsp(string text)
        {
            if (text.IndexOf((char)0xA0) < 0) 
                return text;
            return text.Replace((char)0xA0, ' ');
        }
    }
}