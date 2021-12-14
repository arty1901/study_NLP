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
    static class UnilayoutHelper
    {
        internal static void CorrectPdfSpacings(List<Pullenti.Unitext.UnilayPage> pages)
        {
            int sp0 = 0;
            int sp1 = 0;
            foreach (Pullenti.Unitext.UnilayPage p in pages) 
            {
                foreach (Pullenti.Unitext.UnilayRectangle r in p.Rects) 
                {
                    if (r.Text != null) 
                    {
                        for (int i = 0; i < (r.Text.Length - 1); i++) 
                        {
                            char ch1 = r.Text[i];
                            char ch2 = r.Text[i + 1];
                            if (!char.IsWhiteSpace(ch1)) 
                            {
                                if (!char.IsWhiteSpace(ch2)) 
                                    sp0++;
                                else if (((i + 2) < r.Text.Length) && !char.IsWhiteSpace(r.Text[i + 2])) 
                                    sp1++;
                            }
                        }
                    }
                }
            }
            if (sp0 > (sp1 / 100)) 
                return;
            StringBuilder tmp = new StringBuilder();
            foreach (Pullenti.Unitext.UnilayPage p in pages) 
            {
                foreach (Pullenti.Unitext.UnilayRectangle r in p.Rects) 
                {
                    if (r.Text != null) 
                    {
                        tmp.Length = 0;
                        for (int i = 0; i < r.Text.Length; i++) 
                        {
                            char ch1 = r.Text[i];
                            if (char.IsWhiteSpace(ch1)) 
                                continue;
                            tmp.Append(ch1);
                            int j;
                            int k = 0;
                            for (j = i + 1; j < r.Text.Length; j++) 
                            {
                                if (!char.IsWhiteSpace(r.Text[j])) 
                                    break;
                                else 
                                    k++;
                            }
                            k /= 2;
                            for (; k > 0; k--) 
                            {
                                tmp.Append(' ');
                            }
                        }
                        r.Text = tmp.ToString();
                    }
                }
            }
        }
        internal static void CorrectPdfEncoding(List<Pullenti.Unitext.UnilayPage> pages)
        {
            int tot = 0;
            int err = 0;
            int lat = 0;
            int rus = 0;
            foreach (Pullenti.Unitext.UnilayPage p in pages) 
            {
                foreach (Pullenti.Unitext.UnilayRectangle r in p.Rects) 
                {
                    if (r.Text != null) 
                    {
                        foreach (char ch in r.Text) 
                        {
                            if (char.IsLetter(ch)) 
                            {
                                tot++;
                                int cod = (int)ch;
                                if (cod < 0x80) 
                                    lat++;
                                else if (cod < 0x100) 
                                    err++;
                                else if (cod >= 0x400 && (cod < 0x500)) 
                                    rus++;
                            }
                        }
                    }
                }
            }
            if (err > 10 && err > (tot / 2) && (rus < (err / 2))) 
            {
            }
            else 
                return;
            StringBuilder tmp = new StringBuilder();
            byte[] b1 = new byte[(int)1];
            foreach (Pullenti.Unitext.UnilayPage p in pages) 
            {
                foreach (Pullenti.Unitext.UnilayRectangle r in p.Rects) 
                {
                    if (r.Text != null) 
                    {
                        tmp.Length = 0;
                        foreach (char ch in r.Text) 
                        {
                            int cod = (int)ch;
                            char resCh = ch;
                            if (cod > 0x80 && (cod < 0x100)) 
                            {
                                b1[0] = (byte)cod;
                                string s = Pullenti.Util.MiscHelper.DecodeString1251(b1, 0, -1);
                                if ((s != null && s.Length == 1 && ((int)s[0]) >= 0x400) && (((int)s[0]) < 0x500)) 
                                {
                                    resCh = s[0];
                                    if (((resCh >= 'А' && resCh <= 'Я')) || ((resCh >= 'а' && resCh <= 'я'))) 
                                    {
                                    }
                                    else if (resCh == 'њ') 
                                        resCh = 'Ё';
                                    else if (cod == 188) 
                                        resCh = 'ё';
                                    else if (cod == 190) 
                                        resCh = '«';
                                    else if (cod == 191) 
                                        resCh = '»';
                                    else 
                                    {
                                    }
                                }
                                else 
                                {
                                }
                            }
                            tmp.Append(resCh);
                        }
                        r.Text = tmp.ToString();
                    }
                }
            }
        }
        static double _cmFromPt(int pt)
        {
            double v = (double)pt;
            v = (v * 2.54) / 72;
            return Math.Round(v, 1);
        }
        public static void CreateContentFromPages(Pullenti.Unitext.UnitextDocument doc, bool afterOcr)
        {
            if (doc.Pages.Count == 0) 
                return;
            if (doc.SourceFormat == Pullenti.Unitext.FileFormat.Pdf && !afterOcr) 
                CorrectPdfEncoding(doc.Pages);
            Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer();
            doc.Content = cnt;
            List<List<LayLine>> plines = new List<List<LayLine>>();
            foreach (Pullenti.Unitext.UnilayPage p in doc.Pages) 
            {
                plines.Add(CorrectPageLines(p));
            }
            List<LayLine> headTitle = new List<LayLine>();
            Pullenti.Unitext.UnitextItem header = null;
            Pullenti.Unitext.UnitextItem footer = null;
            if (doc.Pages.Count > 1) 
            {
                while (plines[0].Count > 0) 
                {
                    if (headTitle.Count > 0) 
                    {
                        if (plines[0][0].Y > (headTitle[0].Y + headTitle[0].Height)) 
                            break;
                    }
                    int pn;
                    int eq = 0;
                    for (pn = 1; pn < plines.Count; pn++) 
                    {
                        if (plines[pn].Count > 0) 
                        {
                            if (plines[pn][0].CanBeEqualsExceptPageNumber(plines[0][0])) 
                                eq++;
                            else 
                                break;
                        }
                    }
                    if (eq > 0 && pn >= plines.Count) 
                    {
                        headTitle.Add(plines[0][0]);
                        foreach (List<LayLine> pp in plines) 
                        {
                            if (pp.Count > 0) 
                                pp.RemoveAt(0);
                        }
                        continue;
                    }
                    break;
                }
                if (headTitle.Count > 0) 
                {
                    Pullenti.Unitext.UnitextContainer fcnt = new Pullenti.Unitext.UnitextContainer();
                    foreach (LayLine ht in headTitle) 
                    {
                        foreach (Pullenti.Unitext.UnilayRectangle b in ht.Boxes) 
                        {
                            b.Ignored = true;
                            if (b.Text != null) 
                                fcnt.Children.Add(new Pullenti.Unitext.UnitextPlaintext() { Text = b.Text });
                            else if (b.ImageContent != null) 
                                fcnt.Children.Add(new Pullenti.Unitext.UnitextImage() { Content = b.ImageContent });
                        }
                    }
                    string ttt = fcnt.GetPlaintextString(null);
                    if (ttt != null && ttt.Length > 5) 
                        header = fcnt;
                }
            }
            Pullenti.Unitext.UnitextPagesection cur = new Pullenti.Unitext.UnitextPagesection();
            cur.Width = _cmFromPt(doc.Pages[0].Width);
            cur.Height = _cmFromPt(doc.Pages[0].Height);
            doc.Sections.Add(cur);
            for (int pn = 0; pn < doc.Pages.Count; pn++) 
            {
                Pullenti.Unitext.UnilayPage p = doc.Pages[pn];
                if (pn > 0) 
                    cnt.Children.Add(new Pullenti.Unitext.UnitextPagebreak() { PageSectionId = cur.Id });
                double pw = _cmFromPt(p.Width);
                double ph = _cmFromPt(p.Height);
                if (Math.Abs(pw - cur.Width) > 1 || Math.Abs(ph - cur.Height) > 1) 
                {
                    cur = new Pullenti.Unitext.UnitextPagesection();
                    doc.Sections.Add(cur);
                    cur.Id = string.Format("ps{0}", doc.Sections.Count);
                    cur.Width = pw;
                    cur.Height = ph;
                }
                List<LayLine> lines = plines[pn];
                if (lines == null) 
                    continue;
                double charWidth = (double)0;
                int cou = 0;
                foreach (LayLine l in lines) 
                {
                    foreach (Pullenti.Unitext.UnilayRectangle r in l.Boxes) 
                    {
                        if (r.Text != null) 
                        {
                            cou += r.Text.Length;
                            charWidth += (r.Right - r.Left);
                        }
                    }
                }
                if (cou > 0) 
                    charWidth /= cou;
                for (int i = 0; i < lines.Count; i++) 
                {
                    if (i > 0) 
                    {
                        cou = 1;
                        double h1 = lines[i - 1].Height;
                        double h2 = lines[i].Height;
                        double y1 = lines[i - 1].Y;
                        double y2 = lines[i].Y;
                        if (((h1 < (h2 * 0.75F))) || ((h1 * 0.75F) >= h2)) 
                            cou = 2;
                        else if ((y1 + h1) <= y2 && h1 > 0) 
                        {
                            cou = (int)((((y2 - y1)) / h1));
                            if (cou < 1) 
                                cou = 1;
                            else if (cou > 5) 
                                cou = 5;
                        }
                        else if (y1 > y2) 
                            cou = 3;
                        cnt.Children.Add(new Pullenti.Unitext.UnitextNewline() { Count = cou, PageSectionId = cur.Id });
                    }
                    LayLine l = lines[i];
                    foreach (Pullenti.Unitext.UnilayRectangle b in l.Boxes) 
                    {
                        b.Tag = null;
                    }
                    if (i == 13) 
                    {
                    }
                    for (int j = 0; j < l.Boxes.Count; j++) 
                    {
                        Pullenti.Unitext.UnilayRectangle r = l.Boxes[j];
                        r.LineNumber = j + 1;
                        if (j > 0) 
                        {
                            double d = r.Left - l.Boxes[j - 1].Right;
                            if (d > (charWidth / 3)) 
                            {
                                int sps = (charWidth > 0 ? (int)((d / charWidth)) : 1);
                                if (sps < 1) 
                                    sps = 1;
                                string str = new string(' ', sps);
                                cnt.Children.Add(new Pullenti.Unitext.UnitextPlaintext() { Text = str, PageSectionId = cur.Id });
                            }
                        }
                        if (r.Text != null) 
                        {
                            Pullenti.Unitext.UnitextPlaintext txt = new Pullenti.Unitext.UnitextPlaintext() { Text = r.Text, PageSectionId = cur.Id };
                            txt.Layout = new Pullenti.Unitext.UnilayRectangle[(int)r.Text.Length];
                            for (int k = 0; k < txt.Layout.Length; k++) 
                            {
                                txt.Layout[k] = r;
                            }
                            cnt.Children.Add(txt);
                            if (j > 0 && _hasLetterOrDigit(txt.Text)) 
                            {
                                Pullenti.Unitext.UnilayRectangle r0 = l.Boxes[j - 1];
                                if (r0.Tag == null && (r0.Right + 3) >= r.Left) 
                                {
                                    Pullenti.Unitext.UnitextPlaintext prev = (cnt.Children.Count > 1 ? cnt.Children[cnt.Children.Count - 2] as Pullenti.Unitext.UnitextPlaintext : null);
                                    if (prev != null && _hasLetterOrDigit(prev.Text)) 
                                    {
                                        if (r.Bottom > ((((r0.Bottom + r0.Top)) / 2)) && r.Bottom > r0.Top && (r.Top < r0.Top)) 
                                        {
                                            if ((prev.Text.Length < 3) && txt.Text.Length > 4) 
                                                prev.Typ = Pullenti.Unitext.UnitextPlaintextType.Sub;
                                            else if (txt.Text.Length < 5) 
                                            {
                                                r.Tag = txt;
                                                txt.Typ = Pullenti.Unitext.UnitextPlaintextType.Sup;
                                            }
                                        }
                                        else if ((r.Top < ((((r0.Bottom + r0.Top)) / 2))) && (r.Top < r0.Bottom)) 
                                        {
                                            if (r.Bottom > ((r0.Bottom + (((r.Bottom - r0.Top)) / 6)))) 
                                            {
                                                if (prev != null && (prev.Text.Length < 3) && txt.Text.Length > 4) 
                                                    prev.Typ = Pullenti.Unitext.UnitextPlaintextType.Sup;
                                                else if (txt.Text.Length < 5) 
                                                {
                                                    r.Tag = txt;
                                                    txt.Typ = Pullenti.Unitext.UnitextPlaintextType.Sub;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (r.ImageContent != null) 
                        {
                            Pullenti.Unitext.UnitextImage img = new Pullenti.Unitext.UnitextImage() { Content = r.ImageContent, Rect = r, PageSectionId = cur.Id };
                            img.Width = string.Format("{0}pt", (int)((r.Right - r.Left)));
                            img.Height = string.Format("{0}pt", (int)((r.Bottom - r.Top)));
                            cnt.Children.Add(img);
                        }
                    }
                }
                if (p.ImageContent != null) 
                {
                    Pullenti.Unitext.UnitextImage img = new Pullenti.Unitext.UnitextImage() { PageSectionId = cur.Id };
                    img.Content = p.ImageContent;
                    if (p.Width > 0) 
                        img.Width = string.Format("{0}pt", (int)p.Width);
                    if (p.Height > 0) 
                        img.Height = string.Format("{0}pt", (int)p.Height);
                    cnt.Children.Add(img);
                }
            }
        }
        static bool _hasLetterOrDigit(string txt)
        {
            if (txt != null) 
            {
                foreach (char ch in txt) 
                {
                    if ((char.IsLetterOrDigit(ch) || ch == '*' || ch == '<') || ch == '>') 
                        return true;
                }
            }
            return false;
        }
        static List<LayLine> CorrectPageLines(Pullenti.Unitext.UnilayPage p)
        {
            List<LayLine> lines = new List<LayLine>();
            foreach (Pullenti.Unitext.UnilayRectangle r in p.Rects) 
            {
                if (lines.Count > 0 && lines[lines.Count - 1].TryAppend(r)) 
                {
                }
                else 
                {
                    LayLine l = new LayLine();
                    l.Boxes.Add(r);
                    lines.Add(l);
                    if (lines.Count == 14) 
                    {
                    }
                }
            }
            if (lines.Count < 1) 
                return lines;
            double height = (double)0;
            double x = (double)10000000;
            foreach (LayLine l in lines) 
            {
                height += l.Height;
                if (l.X < x) 
                    x = l.X;
            }
            height /= lines.Count;
            int errs = 0;
            int max = (lines.Count * ((lines.Count - 1))) / 2;
            for (int i = 0; i < (lines.Count - 1); i++) 
            {
                for (int j = i + 1; j < lines.Count; j++) 
                {
                    if (lines[i].Y > (lines[j].Y + height)) 
                        errs++;
                }
            }
            if (errs > (max / 2)) 
            {
                for (int ii = 0; ii < lines.Count; ii++) 
                {
                    bool ch = false;
                    for (int jj = 0; jj < (lines.Count - 1); jj++) 
                    {
                        if (lines[jj].CompareTo(lines[jj + 1]) > 0) 
                        {
                            LayLine v = lines[jj];
                            lines[jj] = lines[jj + 1];
                            lines[jj + 1] = v;
                            ch = true;
                        }
                    }
                    if (!ch) 
                        break;
                }
            }
            return lines;
        }
        class LayLine : IComparable
        {
            public List<Pullenti.Unitext.UnilayRectangle> Boxes = new List<Pullenti.Unitext.UnilayRectangle>();
            public bool CanBeEqualsExceptPageNumber(LayLine ll)
            {
                if (ll.Boxes.Count != Boxes.Count) 
                    return false;
                for (int i = 0; i < ll.Boxes.Count; i++) 
                {
                    Pullenti.Unitext.UnilayRectangle b0 = Boxes[i];
                    Pullenti.Unitext.UnilayRectangle b1 = ll.Boxes[i];
                    if (b0.Text == null && b1.Text == null) 
                    {
                        if (b0.ImageContent != null && b1.ImageContent != null) 
                        {
                            if (b0.ImageContent.Length != b1.ImageContent.Length) 
                                return false;
                            for (int j = 0; j < b0.ImageContent.Length; j++) 
                            {
                                if (b0.ImageContent[j] != b1.ImageContent[j]) 
                                    return false;
                            }
                        }
                        else 
                            return false;
                        continue;
                    }
                    if (b0.Text == null || b1.Text == null) 
                        return false;
                    if (b0.Text == b1.Text) 
                        continue;
                    int i0 = 0;
                    int i1 = 0;
                    while ((i0 < b0.Text.Length) && (i1 < b1.Text.Length)) 
                    {
                        char ch0 = b0.Text[i0];
                        char ch1 = b1.Text[i1];
                        if (ch0 == ch1) 
                        {
                            i0++;
                            i1++;
                            continue;
                        }
                        bool oo = false;
                        while ((i0 < b0.Text.Length) && char.IsDigit(b0.Text[i0])) 
                        {
                            i0++;
                            oo = true;
                        }
                        while ((i1 < b1.Text.Length) && char.IsDigit(b1.Text[i1])) 
                        {
                            i1++;
                            oo = true;
                        }
                        if (!oo) 
                            return false;
                    }
                    if ((i0 < b0.Text.Length) || (i1 < b1.Text.Length)) 
                        return false;
                }
                return true;
            }
            public override string ToString()
            {
                StringBuilder txt = new StringBuilder();
                foreach (Pullenti.Unitext.UnilayRectangle b in Boxes) 
                {
                    if (b.Text != null) 
                    {
                        if (txt.Length > 0) 
                            txt.Append(' ');
                        txt.Append(b.Text);
                    }
                }
                return string.Format("{0} X={1} X1={2} Y={3} Y2={4}", txt.ToString(), X, X + Width, Y, Y + Height);
            }
            public double X
            {
                get
                {
                    return (Boxes.Count == 0 ? 0 : (int)Boxes[0].Left);
                }
            }
            public double Width
            {
                get
                {
                    if (Boxes.Count == 0) 
                        return 0;
                    return (Boxes[Boxes.Count - 1].Right - Boxes[0].Left);
                }
            }
            public double Y
            {
                get
                {
                    double y = (double)0;
                    foreach (Pullenti.Unitext.UnilayRectangle w in Boxes) 
                    {
                        y += w.Top;
                    }
                    if (Boxes.Count > 0) 
                        y /= Boxes.Count;
                    return y;
                }
            }
            public double Height
            {
                get
                {
                    double y = (double)0;
                    double h = (double)0;
                    foreach (Pullenti.Unitext.UnilayRectangle w in Boxes) 
                    {
                        y += w.Top;
                        h += w.Bottom;
                    }
                    if (Boxes.Count > 0) 
                    {
                        y /= Boxes.Count;
                        h /= Boxes.Count;
                        h -= y;
                    }
                    return h;
                }
            }
            public bool TryAppend(Pullenti.Unitext.UnilayRectangle r)
            {
                if (Boxes.Count == 0) 
                {
                    Boxes.Add(r);
                    return true;
                }
                if ((((r.Top + r.Bottom)) / 2) < Y) 
                    return false;
                if (r.Top > ((Y + ((Height / 2))))) 
                    return false;
                if (r.Left >= ((X + Width) - 2)) 
                {
                    Boxes.Add(r);
                    return true;
                }
                for (int i = 0; i < Boxes.Count; i++) 
                {
                    if (r.Right <= Boxes[i].Left) 
                    {
                        Boxes.Insert(i, r);
                        return true;
                    }
                }
                return false;
            }
            public int CompareTo(object obj)
            {
                LayLine l = obj as LayLine;
                if (Y < l.Y) 
                    return -1;
                if (Y > l.Y) 
                    return 1;
                if (X < l.X) 
                    return -1;
                if (X > l.X) 
                    return 1;
                return 0;
            }
        }

    }
}