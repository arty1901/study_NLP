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

namespace Pullenti.Unitext.Internal.Html
{
    class UnitextHtmlGen
    {
        public Pullenti.Unitext.UnitextDocument Create(HtmlNode nod, string dirName, Dictionary<string, byte[]> images, Pullenti.Unitext.CreateDocumentParam pars)
        {
            if (dirName != null && Directory.Exists(dirName)) 
                m_DirName = dirName;
            m_Images = images;
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Html };
            this._prepare(nod, false);
            if (pars.LoadDocumentStructure) 
            {
                HtmlSectionItem s = HtmlSection.Create(nod);
                if (s != null) 
                {
                    doc.Content = s.Generate(this);
                    if (s.Head.Count > 0 || s.Tail.Count > 0) 
                    {
                        doc.Sections.Add(new Pullenti.Unitext.UnitextPagesection());
                        if (s.Head.Count > 0) 
                        {
                            Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            foreach (HtmlNode n in s.Head) 
                            {
                                this.GetUniText(n, gen, null, 0);
                            }
                            doc.Sections[0].Header = gen.Finish(true, null);
                        }
                        if (s.Tail.Count > 0) 
                        {
                            Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            foreach (HtmlNode n in s.Tail) 
                            {
                                this.GetUniText(n, gen, null, 0);
                            }
                            doc.Sections[0].Footer = gen.Finish(true, null);
                        }
                    }
                }
            }
            if (doc.Content == null) 
            {
                Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                this.GetUniText(nod, gen, null, 0);
                doc.Content = gen.Finish(true, null);
            }
            HtmlNode h = nod.FindSubnode("HEAD", null, null);
            HtmlNode ti = (h != null ? h.FindSubnode("TITLE", null, null) : null);
            if (ti != null && ti.Text != null && !doc.Attrs.ContainsKey("title")) 
                doc.Attrs.Add("title", ti.Text.Trim());
            doc.Optimize(false, null);
            return doc;
        }
        class MsoListItem
        {
            public int Level;
            public string ListId;
            public Pullenti.Unitext.UnitextListitem Item;
        }

        string m_DirName = null;
        Dictionary<string, byte[]> m_Images = null;
        Dictionary<string, Pullenti.Unitext.UnitextFootnote> m_Foots = new Dictionary<string, Pullenti.Unitext.UnitextFootnote>();
        Dictionary<string, Pullenti.Unitext.UnitextComment> m_Anno = new Dictionary<string, Pullenti.Unitext.UnitextComment>();
        Dictionary<string, Pullenti.Unitext.UnitextContainer> m_Shapes = new Dictionary<string, Pullenti.Unitext.UnitextContainer>();
        List<HtmlNode> m_Ignored = new List<HtmlNode>();
        void _prepare(HtmlNode nod, bool okFoots)
        {
            string val = nod.GetAttribute("STYLE");
            if (val != null && ((val.Contains("footnote-list") || val.Contains("endnote-list") || val.Contains("comment-list")))) 
            {
                okFoots = true;
                m_Ignored.Add(nod);
            }
            if (okFoots && nod.TagName == "P") 
            {
                val = nod.GetAttribute("CLASS");
                if (val == "MsoFootnoteText" || val == "MsoEndnoteText" || val == "MsoCommentText") 
                {
                    string nam = null;
                    string val1;
                    if ((((val1 = nod.GetAttribute("NAME")))) != null) 
                        nam = val1;
                    else 
                    {
                        HtmlNode ch = nod.FindSubnode("A", "NAME", null);
                        if (ch == null && nod.Parent != null && val == "MsoCommentText") 
                            ch = (nod.Parent as HtmlNode).FindSubnode("A", "NAME", null);
                        if (ch != null) 
                        {
                            nam = (val1 = ch.GetAttribute("NAME"));
                            if (nam != null) 
                                m_Ignored.Add(ch);
                        }
                    }
                    if (nam != null) 
                    {
                        if (val.Contains("Comment")) 
                        {
                            if (!m_Anno.ContainsKey(nam)) 
                            {
                                Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                this.GetUniText(nod, gg, null, 0);
                                Pullenti.Unitext.UnitextItem it = gg.Finish(true, null);
                                if (it != null) 
                                {
                                    StringBuilder tmp = new StringBuilder();
                                    it.GetPlaintext(tmp, null);
                                    if (tmp.Length > 0) 
                                    {
                                        if (tmp.ToString().IndexOf('[') >= 0) 
                                        {
                                            int ii = tmp.ToString().IndexOf(']');
                                            if (ii > 0) 
                                                tmp.Remove(0, ii + 1);
                                        }
                                        m_Anno.Add(nam, new Pullenti.Unitext.UnitextComment() { Text = tmp.ToString().Trim() });
                                    }
                                }
                            }
                        }
                        else 
                        {
                            Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            this.GetUniText(nod, gg, null, 0);
                            Pullenti.Unitext.UnitextFootnote fn = new Pullenti.Unitext.UnitextFootnote();
                            fn.IsEndnote = val.Contains("Endnote");
                            fn.Content = gg.Finish(true, null);
                            if (!m_Foots.ContainsKey(nam)) 
                                m_Foots.Add(nam, fn);
                        }
                    }
                    return;
                }
            }
            for (int ii = 0; ii < nod.Children.Count; ii++) 
            {
                this._prepare(nod.Children[ii], okFoots);
            }
        }
        public void GetUniText(HtmlNode nod, Pullenti.Unitext.Internal.Uni.UnitextGen res, object blk, int lev)
        {
            if (lev > 100) 
                return;
            string TagName = nod.TagName;
            if ((TagName == "SCRIPT" || TagName == "NOSCRIPT" || TagName == "STYLE") || TagName == "META" || TagName == "TITLE") 
                return;
            if (TagName == "DEL") 
                return;
            if (m_Ignored.Contains(nod)) 
                return;
            HtmlTag tag = HtmlTag.FindTag(TagName);
            string st = nod.GetAttribute("STYLE");
            if (st != null && st.ToLower().Contains("page-break-before")) 
                res.AppendPagebreak();
            if (tag != null && tag.Name == "BR") 
            {
                    {
                        if (res != null) 
                            res.AppendNewline(false);
                        return;
                    }
            }
            if (tag != null && tag.Name == "TABLE") 
            {
                Pullenti.Unitext.Internal.Uni.UnitextGenTable tbl = new Pullenti.Unitext.Internal.Uni.UnitextGenTable();
                for (int ii = 0; ii < nod.Children.Count; ii++) 
                {
                    this.GetUniText(nod.Children[ii], null, tbl, lev + 1);
                }
                if (res != null) 
                {
                    Pullenti.Unitext.UnitextTable tab = tbl.Convert();
                    if (tab != null) 
                    {
                        if ((((tab.Width = nod.GetStyleValue("width")))) == null) 
                            tab.Width = nod.GetAttribute("WIDTH");
                        string vv = nod.GetAttribute("BORDER");
                        if (vv != null) 
                        {
                            if (vv.StartsWith("0")) 
                                tab.HideBorders = true;
                        }
                        else if ((((vv = nod.GetStyleValue("border")))) != null) 
                        {
                            if (vv.StartsWith("0")) 
                                tab.HideBorders = true;
                        }
                        else if ((((vv = nod.GetStyleValue("border-width")))) != null) 
                        {
                            if (vv.StartsWith("0")) 
                                tab.HideBorders = true;
                        }
                        if (nod.GetStyleValue("BORDER") == "none") 
                        {
                            if (nod.GetStyleValue("MSO-BORDER-ALT") == null) 
                                tab.HideBorders = true;
                        }
                        if (res.LastChar != '\r') 
                            res.AppendNewline(false);
                        res.Append(tab, null, -1, false);
                    }
                    res.AppendNewline(false);
                }
                return;
            }
            if (tag != null && tag.Name == "TR") 
            {
                Pullenti.Unitext.Internal.Uni.UnitextGenTable tab = blk as Pullenti.Unitext.Internal.Uni.UnitextGenTable;
                if (tab == null) 
                    return;
                List<Pullenti.Unitext.Internal.Uni.UniTextGenCell> row = new List<Pullenti.Unitext.Internal.Uni.UniTextGenCell>();
                tab.Cells.Add(row);
                for (int ii = 0; ii < nod.Children.Count; ii++) 
                {
                    HtmlNode ch = nod.Children[ii];
                    HtmlTag tag2 = HtmlTag.FindTag(ch.TagName);
                    if (tag2 == null || ((tag2.Name != "TD" && tag2.Name != "TH"))) 
                        continue;
                    Pullenti.Unitext.Internal.Uni.UniTextGenCell cel = new Pullenti.Unitext.Internal.Uni.UniTextGenCell();
                    row.Add(cel);
                    int colsp = 1;
                    int rowsp = 1;
                    if (ch.GetAttribute("ROWSPAN") != null) 
                    {
                        if (int.TryParse(ch.GetAttribute("ROWSPAN"), out rowsp)) 
                        {
                            if (rowsp > 65534) 
                                rowsp = 65534;
                            cel.RowSpan = rowsp;
                        }
                    }
                    if (ch.GetAttribute("COLSPAN") != null) 
                    {
                        if (int.TryParse(ch.GetAttribute("COLSPAN"), out colsp)) 
                        {
                            if (colsp < 100) 
                                cel.ColSpan = colsp;
                        }
                    }
                    if (colsp <= 1) 
                    {
                        string wi = ch.GetStyleValue("width") ?? ch.GetAttribute("WIDTH");
                        if (wi != null) 
                            cel.Width = wi;
                    }
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    this.GetUniText(ch, gg, null, lev + 1);
                    cel.Content = gg.Finish(true, null);
                }
                return;
            }
            if (tag != null && tag.Name == "COLGROUP") 
            {
                Pullenti.Unitext.Internal.Uni.UnitextGenTable tab = blk as Pullenti.Unitext.Internal.Uni.UnitextGenTable;
                if (tab == null) 
                    return;
                for (int ii = 0; ii < nod.Children.Count; ii++) 
                {
                    HtmlNode ch = nod.Children[ii];
                    HtmlTag tag2 = HtmlTag.FindTag(ch.TagName);
                    if (tag2 == null || tag2.Name != "COL") 
                        continue;
                    string wi = ch.GetStyleValue("width") ?? ch.GetAttribute("WIDTH");
                    tab.m_ColWidth.Add(wi);
                }
                return;
            }
            if (tag != null && ((tag.Name == "OL" || tag.Name == "UL"))) 
            {
                Pullenti.Unitext.UnitextList li = new Pullenti.Unitext.UnitextList();
                int liNum = 0;
                string liTyp = null;
                if (tag.Name == "OL") 
                {
                    if (!int.TryParse(nod.GetAttribute("START") ?? "", out liNum)) 
                        liNum = 1;
                    liTyp = nod.GetAttribute("TYPE") ?? "1";
                }
                else 
                    li.UnorderPrefix = "";
                for (int ii = 0; ii < nod.Children.Count; ii++) 
                {
                    HtmlNode ch = nod.Children[ii];
                    HtmlTag tag2 = HtmlTag.FindTag(ch.TagName);
                    if (tag2 == null || tag2.Name != "LI") 
                        continue;
                    Pullenti.Unitext.UnitextListitem it = new Pullenti.Unitext.UnitextListitem();
                    li.Items.Add(it);
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    this.GetUniText(ch, gg, null, lev + 1);
                    it.Content = gg.Finish(true, null);
                    if (liNum > 0 && liTyp != null && li.UnorderPrefix == null) 
                    {
                        string pref = HtmlNode._getLiNum(liNum, liTyp);
                        if (pref != null) 
                            it.Prefix = new Pullenti.Unitext.UnitextPlaintext() { Text = pref };
                        liNum++;
                    }
                }
                if (res != null) 
                    res.Append(li, null, -1, false);
                return;
            }
            if (tag != null && ((tag.Name == "SUP" || tag.Name == "SUB"))) 
            {
                StringBuilder tmp = new StringBuilder();
                if (nod.Children.Count == 0) 
                    tmp.Append(nod.Text);
                else 
                    for (int ii = 0; ii < nod.Children.Count; ii++) 
                    {
                        nod.Children[ii]._getFullText(tmp, null, null);
                    }
                string tt = tmp.ToString().Trim();
                if (tt.Length > 0 && (tt.Length < 10)) 
                {
                    res.Append(new Pullenti.Unitext.UnitextPlaintext() { Text = tt, Typ = (tag.Name == "SUB" ? Pullenti.Unitext.UnitextPlaintextType.Sub : Pullenti.Unitext.UnitextPlaintextType.Sup) }, null, -1, false);
                    return;
                }
            }
            if (res == null) 
            {
                for (int ii = 0; ii < nod.Children.Count; ii++) 
                {
                    this.GetUniText(nod.Children[ii], res, blk, lev + 1);
                }
                return;
            }
            if (TagName == "A") 
            {
                string val = nod.GetAttribute("HREF");
                if (val != null) 
                {
                    Pullenti.Unitext.UnitextFootnote fn;
                    if (val.StartsWith("#") && m_Foots.TryGetValue(val.Substring(1), out fn)) 
                    {
                        res.Append(fn, null, -1, false);
                        return;
                    }
                    if (val.StartsWith("#") && m_Anno.ContainsKey(val.Substring(1))) 
                    {
                        res.Append(m_Anno[val.Substring(1)], null, -1, false);
                        return;
                    }
                    if (nod.Children.Count > 0) 
                    {
                        bool noTag = false;
                        if (nod.Children.Count > 20) 
                            noTag = true;
                        else 
                            foreach (HtmlNode ch in nod.Children) 
                            {
                                if (ch.HasTag("A") || ch.HasBlockTag()) 
                                {
                                    noTag = true;
                                    break;
                                }
                            }
                        if (noTag && (nod.Parent is HtmlNode)) 
                        {
                            int i = (nod.Parent as HtmlNode).Children.IndexOf(nod);
                            if (i >= 0) 
                                (nod.Parent as HtmlNode).Children.InsertRange(i + 1, nod.Children);
                            nod.Children.Clear();
                        }
                    }
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    if (nod.Text != null) 
                        gg.AppendText(nod.Text, false);
                    foreach (HtmlNode ch in nod.Children) 
                    {
                        this.GetUniText(ch, gg, null, lev + 1);
                    }
                    Pullenti.Unitext.UnitextItem it = gg.Finish(true, null);
                    if (it != null) 
                    {
                        Pullenti.Unitext.UnitextHyperlink hr = new Pullenti.Unitext.UnitextHyperlink() { Href = val };
                        hr.Content = it;
                        res.Append(hr, null, -1, false);
                        return;
                    }
                    val = nod.GetAttribute("CLASS");
                    if (string.Compare(val ?? "", "MSOCOMANCHOR", true) == 0) 
                        return;
                }
            }
            if (TagName == "V:TEXTBOX") 
            {
            }
            if (TagName == "V:IMAGEDATA" || TagName == "IMG") 
            {
                string shapes = nod.GetAttribute("V:SHAPES");
                if (shapes != null) 
                {
                    string[] ids = shapes.Split(' ');
                }
                Pullenti.Unitext.UnitextImage img = new Pullenti.Unitext.UnitextImage();
                if ((((img.Id = nod.GetAttribute("SRC")))) != null) 
                {
                    try 
                    {
                        if (m_DirName != null) 
                        {
                            if (img.Id != null && !img.Id.StartsWith("http")) 
                            {
                                string fname = Path.Combine(m_DirName, (img.Id ?? ""));
                                if (!File.Exists(fname)) 
                                    fname = fname.Replace('/', '\\');
                                if (File.Exists(fname)) 
                                    img.Content = Pullenti.Unitext.Internal.Uni.UnitextHelper.LoadDataFromFile(fname, 0);
                            }
                        }
                        else if (m_Images != null) 
                        {
                            foreach (KeyValuePair<string, byte[]> kp in m_Images) 
                            {
                                if (kp.Key.EndsWith(img.Id)) 
                                {
                                    img.Content = kp.Value;
                                    break;
                                }
                            }
                        }
                        res.Append(img, null, -1, false);
                        for (HtmlNode hh = nod; hh != null; hh = hh.Parent as HtmlNode) 
                        {
                            string val;
                            if ((((val = hh.GetStyleValue("WIDTH")))) != null) 
                                img.Width = Pullenti.Unitext.Internal.Uni.UnitextGen.ConvertToPt(val, "px");
                            if ((((val = hh.GetStyleValue("HEIGHT")))) != null) 
                                img.Height = Pullenti.Unitext.Internal.Uni.UnitextGen.ConvertToPt(val, "px");
                            if (img.Width != null || img.Height != null) 
                                break;
                            if ((((val = hh.GetAttribute("WIDTH")))) != null) 
                                img.Width = Pullenti.Unitext.Internal.Uni.UnitextGen.ConvertToPt(val, "px");
                            if ((((val = hh.GetAttribute("HEIGHT")))) != null) 
                                img.Height = Pullenti.Unitext.Internal.Uni.UnitextGen.ConvertToPt(val, "px");
                        }
                        if (img.Width == null || img.Height == null) 
                            img.Width = (img.Height = null);
                    }
                    catch(Exception ex27) 
                    {
                    }
                    return;
                }
            }
            bool isBlock = false;
            if (tag != null && ((tag.IsBlock || tag.IsTable))) 
                isBlock = true;
            bool isH = false;
            if (isBlock) 
            {
                res.AppendNewline(false);
                if (((TagName ?? "")).Length == 2) 
                {
                    if (((TagName[0] == 'H' || TagName[0] == 'h')) && char.IsDigit(TagName[1])) 
                    {
                        isH = true;
                        res.AppendNewline(false);
                        if (TagName[0] == '1') 
                            res.AppendNewline(false);
                    }
                }
            }
            if (!string.IsNullOrEmpty(nod.Text)) 
            {
                int cou = 0;
                bool wingdings = false;
                bool webdings = false;
                for (HtmlNode pp = nod; pp != null && (cou < 2); pp = pp.Parent as HtmlNode,cou++) 
                {
                    string ff = pp.GetStyleValue("font-family") ?? pp.GetStyleValue("font");
                    if (ff != null) 
                    {
                        if (ff.ToUpper().Contains("WINGDINGS")) 
                            wingdings = true;
                        else if (ff.ToUpper().Contains("WEBDINGS")) 
                            webdings = true;
                        break;
                    }
                }
                if (wingdings) 
                    res.AppendText(Pullenti.Unitext.Internal.Misc.WingdingsHelper.GetUnicodeString(nod.Text), false);
                else if (webdings) 
                    res.AppendText(Pullenti.Unitext.Internal.Misc.WebdingsHelper.GetUnicodeString(nod.Text), false);
                else 
                    res.AppendText(nod.Text, false);
            }
            else if (nod.Children.Count == 1) 
            {
                int cou = 0;
                HtmlNode nn = nod;
                for (; nn != null; ) 
                {
                    if (nn.Children.Count != 1) 
                        break;
                    if (nn.Children[0].Text != null) 
                        break;
                    cou++;
                    nn = nn.Children[0];
                }
                if (cou > 100 && nn != null) 
                    nod = nn;
                else 
                    nod = nod.Children[0];
                this.GetUniText(nod, res, null, lev + 1);
            }
            else 
            {
                bool first = true;
                List<Pullenti.Unitext.UnitextList> lists = null;
                string listId = null;
                for (int ii = 0; ii < nod.Children.Count; ii++) 
                {
                    HtmlNode ch = nod.Children[ii];
                    MsoListItem li = this.TryParseList(ch, lev + 1);
                    if (li != null && li.Item.Content == null && nod.TagName == "TD") 
                        li = null;
                    if (li == null) 
                    {
                        if (lists != null) 
                        {
                            if (ch.Children.Count == 0 && ((ch.Text ?? "")).Trim().Length == 0) 
                                continue;
                            lists = null;
                            listId = null;
                        }
                    }
                    else if (li.ListId != listId && listId != null) 
                    {
                        listId = null;
                        lists = null;
                    }
                    if (li != null) 
                    {
                        if (lists == null) 
                        {
                            Pullenti.Unitext.UnitextList list = new Pullenti.Unitext.UnitextList();
                            if (res.LastChar != '\r') 
                                res.AppendNewline(false);
                            res.Append(list, null, -1, false);
                            res.AppendNewline(false);
                            lists = new List<Pullenti.Unitext.UnitextList>();
                            lists.Add(list);
                            listId = li.ListId;
                        }
                        if (li.Level == 3) 
                        {
                        }
                        if (li.Level == lists.Count) 
                        {
                            lists[0].Items.Add(li.Item);
                            continue;
                        }
                        if (li.Level < lists.Count) 
                        {
                            while (li.Level < lists.Count) 
                            {
                                lists.RemoveAt(0);
                            }
                            lists[0].Items.Add(li.Item);
                            continue;
                        }
                        if (li.Level == (lists.Count + 1) && lists[0].Items.Count > 0) 
                        {
                            Pullenti.Unitext.UnitextListitem last = lists[0].Items[lists[0].Items.Count - 1];
                            if (last.Sublist == null) 
                                last.Sublist = new Pullenti.Unitext.UnitextList() { Level = li.Level - 1 };
                            lists.Insert(0, last.Sublist);
                            last.Sublist.Items.Add(li.Item);
                            continue;
                        }
                    }
                    if (first) 
                        first = false;
                    else if (!ch.WhitespacePreserve) 
                        res.AppendText(" ", false);
                    this.GetUniText(ch, res, null, lev + 1);
                }
            }
            if (isH) 
            {
                res.AppendNewline(false);
                if (TagName[0] == '1') 
                    res.AppendNewline(false);
            }
        }
        MsoListItem TryParseList(HtmlNode nod, int lev)
        {
            if (nod.TagName != "P") 
                return null;
            string val = nod.GetAttribute("STYLE");
            if (val == null) 
                return null;
            int i = val.IndexOf("mso-list:");
            if (i < 0) 
                return null;
            val = val.Substring(i + 9).Trim();
            string[] fi = val.Split(' ');
            if (fi.Length < 3) 
                return null;
            if (!fi[2].StartsWith("LFO", StringComparison.OrdinalIgnoreCase)) 
                return null;
            MsoListItem res = new MsoListItem() { ListId = fi[2] };
            if (!fi[1].StartsWith("level", StringComparison.OrdinalIgnoreCase) || !int.TryParse(fi[1].Substring(5), out i)) 
                return null;
            res.Level = i;
            if (i < 1) 
                return null;
            if (nod.Children.Count == 1 && nod.Children[0].TagName == "A") 
                nod = nod.Children[0];
            for (i = 0; i < nod.Children.Count; i++) 
            {
                HtmlNode ch = nod.Children[i];
                if (ch.TagName != "SPAN") 
                    continue;
                if ((((val = ch.GetAttribute("STYLE")))) != null && val.Contains("Ignore")) 
                {
                }
                else if (ch.FindSubnode("SPAN", "STYLE", "mso-list:Ignore") != null) 
                {
                }
                else if (i == 0) 
                {
                }
                else 
                    continue;
                StringBuilder tmp = new StringBuilder();
                ch._getFullText(tmp, new Pullenti.Unitext.GetPlaintextParam(), null);
                res.Item = new Pullenti.Unitext.UnitextListitem();
                if (tmp.Length > 0) 
                    res.Item.Prefix = new Pullenti.Unitext.UnitextPlaintext() { Text = tmp.ToString().Trim() };
                Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                nod.Children[i] = new HtmlNode();
                nod.TagName = "PPP";
                this.GetUniText(nod, gg, null, lev + 1);
                nod.Children[i] = ch;
                nod.TagName = "P";
                res.Item.Content = gg.Finish(true, null);
                return res;
            }
            return null;
        }
    }
}