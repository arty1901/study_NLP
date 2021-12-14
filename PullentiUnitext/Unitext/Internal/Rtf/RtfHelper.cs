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

namespace Pullenti.Unitext.Internal.Rtf
{
    /// <summary>
    /// Поддержка формата RTF
    /// </summary>
    class RtfHelper
    {
        static RtfHelper()
        {
            foreach (int i in new int[] {1052, 1050, 1029, 1038, 1045, 1048, 2074, 1051, 1060}) 
            {
                m_LangIdToCodepage.Add(i, 1250);
            }
            foreach (int i in new int[] {2092, 1059, 1026, 1071, 1087, 1088, 1104, 1049, 3098, 1092, 1058, 2115}) 
            {
                m_LangIdToCodepage.Add(i, 1251);
            }
            foreach (int i in new int[] {1078, 1069, 1027, 1030, 2067, 1043, 3081, 10249, 4105, 9225, 2057, 6153, 8201, 5129, 13321, 7177, 11273, 1033, 12297, 1080, 1035, 2060, 3084, 1036, 5132, 6156, 4108, 1110, 3079, 1031, 5127, 4103, 2055, 1039, 1057, 1040, 2064, 2110, 1086, 1044, 2068, 1046, 2070, 11274, 16394, 13322, 9226, 5130, 7178, 12298, 17418, 4106, 18442, 2058, 19466, 6154, 15370, 10250, 20490, 1034, 14346, 8202, 1089, 2077, 1053}) 
            {
                m_LangIdToCodepage.Add(i, 1252);
            }
            m_LangIdToCodepage.Add(1032, 1253);
            foreach (int i in new int[] {1068, 1055, 1091}) 
            {
                m_LangIdToCodepage.Add(i, 1254);
            }
            m_LangIdToCodepage.Add(1037, 1255);
            foreach (int i in new int[] {5121, 15361, 3073, 2049, 11265, 13313, 12289, 4097, 6145, 8193, 16385, 1025, 10241, 7169, 14337, 9217, 1065, 1056}) 
            {
                m_LangIdToCodepage.Add(i, 1256);
            }
            foreach (int i in new int[] {1061, 1062, 1063, 1066}) 
            {
                m_LangIdToCodepage.Add(i, 1257);
            }
        }
        static Dictionary<int, int> m_LangIdToCodepage = new Dictionary<int, int>();
        static void _manageEncodings(List<RtfItem> items)
        {
            Dictionary<string, int> fontCharsets = new Dictionary<string, int>();
            Dictionary<string, Pullenti.Util.EncodingWrapper> fontEncodings = new Dictionary<string, Pullenti.Util.EncodingWrapper>();
            string fontName = null;
            List<RtfEncoder> defs = new List<RtfEncoder>();
            List<RtfEncoder> curs = new List<RtfEncoder>();
            bool isAnsi = false;
            for (int i = 0; i < items.Count; i++) 
            {
                RtfItem it = items[i];
                while (curs.Count > 0) 
                {
                    if (curs[0].Level > it.Level) 
                        curs.RemoveAt(0);
                    else 
                        break;
                }
                while (defs.Count > 0) 
                {
                    if (defs[0].Level > it.Level) 
                        defs.RemoveAt(0);
                    else 
                        break;
                }
                if (it.Typ == RftItemTyp.Text) 
                {
                    if (it.Text == null && it.Codes != null && it.Codes.Length > 0) 
                    {
                        if (it.Codes.Length > 5) 
                        {
                        }
                        RtfEncoder cur = (curs.Count > 0 ? curs[0] : null);
                        if (cur != null && cur.Level > it.Level) 
                            cur = null;
                        RtfEncoder def = (defs.Count > 0 ? defs[0] : null);
                        if (def != null && def.Level > it.Level) 
                            def = null;
                        Pullenti.Util.EncodingWrapper enc = (cur == null ? null : cur.Encoding);
                        if (fontName != null && fontEncodings.ContainsKey(fontName)) 
                            enc = fontEncodings[fontName];
                        if (enc == null && def != null) 
                            enc = def.Encoding;
                        if (enc == null) 
                            enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.CP1252);
                        try 
                        {
                            string str = enc.GetString(it.Codes, 0, -1);
                            it.Text = str;
                        }
                        catch(Exception ex125) 
                        {
                        }
                    }
                    else if (i > 0 && items[i - 1].Typ == RftItemTyp.Command && items[i - 1].Text[0] == 'f') 
                    {
                        if (it.Text != null && it.Text.EndsWith(" Cyr;") && fontName != null) 
                        {
                            if (!fontEncodings.ContainsKey(fontName)) 
                                fontEncodings.Add(fontName, new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.CP1251));
                        }
                    }
                    continue;
                }
                if (it.Typ != RftItemTyp.Command) 
                    continue;
                string cmd = it.Text;
                if (cmd == null) 
                    continue;
                if (cmd == "plain") 
                {
                    if (curs.Count > 0) 
                        curs.RemoveAt(0);
                    continue;
                }
                if (cmd == "ansi") 
                {
                    isAnsi = true;
                    continue;
                }
                if (cmd.StartsWith("ansicpg")) 
                {
                    int cp;
                    if (!int.TryParse(cmd.Substring(7), out cp)) 
                        continue;
                    defs.Insert(0, new RtfEncoder() { Level = it.Level, Encoding = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, null, cp) });
                    continue;
                }
                if (cmd.StartsWith("deflangfe")) 
                {
                    int cp;
                    if (!int.TryParse(cmd.Substring(9), out cp)) 
                        continue;
                    if (!m_LangIdToCodepage.TryGetValue(cp, out cp)) 
                        continue;
                    defs.Insert(0, new RtfEncoder() { Level = it.Level, Encoding = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, null, cp) });
                    continue;
                }
                if (cmd.StartsWith("deflang")) 
                {
                    int cp;
                    if (!int.TryParse(cmd.Substring(7), out cp)) 
                        continue;
                    if (!m_LangIdToCodepage.TryGetValue(cp, out cp)) 
                        continue;
                    defs.Insert(0, new RtfEncoder() { Level = it.Level, Encoding = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, null, cp) });
                    continue;
                }
                if (cmd.StartsWith("langfe")) 
                    continue;
                if (cmd.StartsWith("lang")) 
                {
                    int cp;
                    if (!int.TryParse(cmd.Substring(4), out cp)) 
                        continue;
                    if (!m_LangIdToCodepage.TryGetValue(cp, out cp)) 
                        continue;
                    if ((i > 0 && items[i - 1].Typ == RftItemTyp.Command && items[i - 1].Text.StartsWith("f")) && curs.Count > 0 && curs[0].Level == it.Level) 
                        continue;
                    curs.Insert(0, new RtfEncoder() { Level = it.Level, Encoding = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, null, cp) });
                    continue;
                }
                if (cmd.Length >= 2 && cmd[0] == 'f' && char.IsDigit(cmd[1])) 
                {
                    if (cmd == "f674") 
                    {
                    }
                    if (fontCharsets.ContainsKey(cmd)) 
                    {
                        int cs = fontCharsets[cmd];
                        Pullenti.Util.EncodingWrapper enc = null;
                        switch (cs) { 
                        case 204:
                            enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.CP1251);
                            break;
                        case 238:
                            enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, null, 1250);
                            break;
                        case 161:
                            enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, null, 1253);
                            break;
                        case 162:
                            enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, null, 1254);
                            break;
                        }
                        if (enc != null) 
                            curs.Insert(0, new RtfEncoder() { Level = it.Level, Encoding = enc });
                    }
                    else 
                        fontName = cmd;
                    continue;
                }
                if (cmd.StartsWith("fcharset") && fontName != null) 
                {
                    int cod;
                    if (int.TryParse(cmd.Substring(8), out cod)) 
                    {
                        if (!fontCharsets.ContainsKey(fontName)) 
                            fontCharsets.Add(fontName, cod);
                    }
                    continue;
                }
            }
            StringBuilder tmp = new StringBuilder();
            for (int i = 0; i < (items.Count - 1); i++) 
            {
                RtfItem it = items[i];
                RtfItem it1 = items[i + 1];
                if ((it1.Typ == RftItemTyp.Text && it.Typ == RftItemTyp.Text && it1.Text != null) && it.Text != null) 
                {
                    tmp.Length = 0;
                    tmp.Append(it.Text);
                    for (int j = i + 1; j < items.Count; j++) 
                    {
                        it1 = items[j];
                        if (it1.Typ != RftItemTyp.Text || it1.Text == null) 
                            break;
                        tmp.Append(it1.Text);
                        items[j] = null;
                        i = j;
                    }
                    it.Text = tmp.ToString();
                }
            }
            int k = 0;
            for (int i = 0; i < items.Count; i++) 
            {
                if (items[i] != null) 
                {
                    items[k] = items[i];
                    k++;
                }
            }
            if (k < items.Count) 
                items.RemoveRange(k, items.Count - k);
        }
        class RtfEncoder
        {
            public int Level;
            public Pullenti.Util.EncodingWrapper Encoding;
            public override string ToString()
            {
                return string.Format("{0}: {1}", Level, Encoding.ToString());
            }
        }

        internal static Pullenti.Unitext.UnitextDocument CreateUniDoc(Stream stream, Pullenti.Unitext.CreateDocumentParam pars)
        {
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Rtf };
            stream.Position = 0;
            List<RtfItem> items = RtfItem.ParseList(stream);
            _manageEncodings(items);
            Pullenti.Unitext.Internal.Uni.UnitextGen cntGen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
            Pullenti.Unitext.Internal.Uni.UnitextGen headGen = null;
            Pullenti.Unitext.Internal.Uni.UnitextGen footGen = null;
            StringBuilder html = null;
            int i = 0;
            if (items.Count > 100 && items[1].Typ == RftItemTyp.Command && items[1].Text == "rtf1") 
            {
                for (int ii = 2; ii < 20; ii++) 
                {
                    if (items[ii].Typ == RftItemTyp.Command && items[ii].Text == "fromhtml1") 
                    {
                        html = new StringBuilder();
                        cntGen = null;
                        for (; (ii < 500) && ((ii + 1) < items.Count); ii++) 
                        {
                            if ((items[ii].Typ == RftItemTyp.Command && items[ii].Text == "*" && items[ii + 1].Typ == RftItemTyp.Command) && items[ii + 1].Text.StartsWith("htmltag")) 
                            {
                                i = ii;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            for (; i < items.Count; i++) 
            {
                RtfItem it = items[i];
                if (it.Typ == RftItemTyp.Command) 
                {
                    string cmd = it.Text;
                    if (cmd == null) 
                        continue;
                    if (cmd.StartsWith("header") || cmd.StartsWith("footer")) 
                    {
                        if (!cmd.StartsWith("headery") && !cmd.StartsWith("footery")) 
                        {
                            int ii;
                            for (ii = i + 1; ii < items.Count; ii++) 
                            {
                                if (items[ii].Level < it.Level) 
                                    break;
                            }
                            if (headGen == null && cmd.StartsWith("head")) 
                            {
                                headGen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                _manageUni(items, html, headGen, i + 1, ii, false, 0);
                            }
                            else if (footGen == null && cmd.StartsWith("foot")) 
                            {
                                footGen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                _manageUni(items, html, footGen, i + 1, ii, false, 0);
                            }
                            i = ii - 1;
                            continue;
                        }
                    }
                }
                if (items[i] is RtfTablerow) 
                {
                }
                int j = _manageUni(items, html, cntGen, i, i + 1, false, 0);
                i = j - 1;
            }
            if (html == null) 
            {
                doc.Content = cntGen.Finish(true, null);
                Pullenti.Unitext.UnitextPagesection sect = new Pullenti.Unitext.UnitextPagesection();
                if (headGen != null) 
                    sect.Header = headGen.Finish(true, null);
                if (footGen != null) 
                    sect.Footer = footGen.Finish(true, null);
                if (sect.Header != null || sect.Footer != null) 
                    doc.Sections.Add(sect);
            }
            else 
            {
                string txt = html.ToString();
                Pullenti.Unitext.Internal.Html.HtmlNode nod = Pullenti.Unitext.Internal.Html.HtmlParser.Parse(html, false);
                if (nod == null) 
                    return null;
                doc = Pullenti.Unitext.Internal.Html.HtmlHelper.Create(nod, null, null, pars);
                doc.SourceFormat = Pullenti.Unitext.FileFormat.Rtf;
            }
            doc.Optimize(false, null);
            return doc;
        }
        class RtfTablerow : Pullenti.Unitext.Internal.Rtf.RtfItem
        {
            public List<string> Props = new List<string>();
            public bool IsLast
            {
                get
                {
                    return Props.Contains("lastrow");
                }
            }
            public override string ToString()
            {
                return string.Format("{0}: TableRow{1}", Level, (IsLast ? " Last" : ""));
            }
        }

        public class RtfListinf : Pullenti.Unitext.Internal.Rtf.RtfItem
        {
            public List<string> Props = new List<string>();
            public override string ToString()
            {
                return string.Format("{0}: Listitem {1}", Level, Text);
            }
        }

        static int _manageUni(List<RtfItem> items, StringBuilder html, Pullenti.Unitext.Internal.Uni.UnitextGen gen, int i0, int i1, bool inTab, int lev)
        {
            int i;
            for (i = i0; i < i1; i++) 
            {
                RtfItem it = items[i];
                if (it.Typ == RftItemTyp.BracketOpen || it.Typ == RftItemTyp.BracketClose) 
                    continue;
                if (it.Typ == RftItemTyp.Proxy) 
                {
                    gen.Append((it as RtfItemProxy).Uni, null, -1, false);
                    continue;
                }
                if (it.Typ == RftItemTyp.Text) 
                {
                    if (it.Text != null) 
                    {
                        if (html != null) 
                            Pullenti.Util.MiscHelper.CorrectHtmlValue(html, it.Text, false, false);
                        else 
                            gen.AppendText(it.Text, false);
                    }
                    continue;
                }
                if (it is RtfItemImage) 
                {
                    RtfItemImage im = it as RtfItemImage;
                    Pullenti.Unitext.UnitextImage img = new Pullenti.Unitext.UnitextImage() { Content = im.Codes };
                    if (im.Width > 0 && im.Height > 0) 
                    {
                        img.Width = string.Format("{0}pt", im.Width);
                        img.Height = string.Format("{0}pt", im.Height);
                    }
                    gen.Append(img, null, -1, false);
                    continue;
                }
                if (it.Typ != RftItemTyp.Command) 
                    continue;
                string cmd = it.Text;
                if (cmd == null) 
                    continue;
                if ((cmd == "*" || cmd.StartsWith("fonttbl") || cmd == "stylesheet") || cmd.StartsWith("nonshppict") || cmd == "deleted") 
                {
                    int htmlOutLev = -1;
                    int lastOutInd = -1;
                    for (++i; i < items.Count; i++) 
                    {
                        if (items[i].Level < it.Level) 
                        {
                            i--;
                            break;
                        }
                        else if (items[i] is RtfItemImage) 
                        {
                            if (cmd == "*") 
                            {
                                RtfItemImage im = items[i] as RtfItemImage;
                                Pullenti.Unitext.UnitextImage img = new Pullenti.Unitext.UnitextImage() { Content = items[i].Codes };
                                if (im.Width > 0 && im.Height > 0) 
                                {
                                    img.Width = string.Format("{0}pt", im.Width);
                                    img.Height = string.Format("{0}pt", im.Height);
                                }
                                gen.Append(img, null, -1, false);
                            }
                        }
                        else if (items[i].Typ == RftItemTyp.Command) 
                        {
                            if (items[i].Text == "footnote") 
                            {
                                i--;
                                break;
                            }
                            if (items[i].Text == "annotation") 
                            {
                                i--;
                                break;
                            }
                            if (items[i].Text == "shptxt") 
                            {
                                i--;
                                break;
                            }
                            if (items[i].Text == "fldinst") 
                            {
                                i--;
                                break;
                            }
                            if (html != null && items[i].Text.StartsWith("htmltag")) 
                                htmlOutLev = items[i].Level;
                        }
                        else if (items[i].Typ == RftItemTyp.Text && html != null) 
                        {
                            html.Append(items[i].Text);
                            lastOutInd = i;
                        }
                    }
                    continue;
                }
                if (cmd == "fldinst") 
                {
                    List<RtfItem> iii = new List<RtfItem>();
                    int ii;
                    for (ii = i + 1; (ii < items.Count) && (ii < i1); ii++) 
                    {
                        if (items[ii].Level < it.Level) 
                            break;
                        else 
                        {
                            iii.Add(items[ii]);
                            if (items[ii].Text != null && items[ii].Text.StartsWith("SYMBOL")) 
                            {
                                string sss = items[ii].Text.Substring(6).Trim();
                                int jj = sss.IndexOf(' ');
                                if (jj > 0) 
                                    sss = sss.Substring(0, jj);
                                if (int.TryParse(sss, out jj)) 
                                {
                                    if (items[ii].Text.Contains("Wingdings")) 
                                    {
                                        char chu = Pullenti.Unitext.Internal.Misc.WingdingsHelper.GetUnicode(jj);
                                        if (chu == ((char)0)) 
                                            chu = '?';
                                        gen.AppendText(string.Format("{0}", chu), false);
                                    }
                                    else 
                                        gen.AppendText(string.Format("{0}", (char)jj), false);
                                }
                            }
                        }
                    }
                    i = ii - 1;
                    continue;
                }
                if (cmd == "field") 
                {
                    List<RtfItem> iii = new List<RtfItem>();
                    int ii;
                    string ur = null;
                    for (ii = i + 1; ii < items.Count; ii++) 
                    {
                        if (items[ii].Level < it.Level) 
                            break;
                        else 
                        {
                            iii.Add(items[ii]);
                            if (items[ii].Typ == RftItemTyp.Text && items[ii].Text != null && items[ii].Text.Trim().StartsWith("HYPERLINK")) 
                            {
                                string fff = items[ii].Text;
                                int jj0 = fff.IndexOf('"');
                                int jj1 = fff.LastIndexOf('"');
                                if (jj0 > 0 && jj1 > jj0) 
                                {
                                    string hy = fff.Substring(jj0 + 1, jj1 - jj0 - 1);
                                    ur = hy;
                                }
                                else 
                                {
                                    fff = fff.Substring(10).Trim();
                                    if (fff.StartsWith("http")) 
                                        ur = fff;
                                    else 
                                    {
                                    }
                                }
                                if (ur == null) 
                                {
                                }
                            }
                        }
                    }
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    _manageUni(iii, null, gg, 0, iii.Count, false, lev + 1);
                    Pullenti.Unitext.UnitextItem cnt = gg.Finish(true, null);
                    if (cnt != null) 
                    {
                        StringBuilder tmp = new StringBuilder();
                        cnt.GetPlaintext(tmp, new Pullenti.Unitext.GetPlaintextParam());
                        if (ur == null || tmp.Length > 300) 
                            gen.Append(cnt, null, -1, false);
                        else 
                        {
                            Pullenti.Unitext.UnitextHyperlink hr = new Pullenti.Unitext.UnitextHyperlink() { Href = ur };
                            hr.Content = cnt;
                            if (gen != null) 
                                gen.Append(hr, null, -1, false);
                            else if (html != null) 
                                cnt.GetPlaintext(html, null);
                        }
                        i = ii - 1;
                    }
                    continue;
                }
                if (cmd == "annotation") 
                {
                    List<RtfItem> iii = new List<RtfItem>();
                    int ii;
                    for (ii = i; (ii < items.Count) && (ii < i1); ii++) 
                    {
                        if (items[ii].Level < (it.Level - 1)) 
                            break;
                        else 
                            iii.Add(items[ii]);
                    }
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    _manageUni(items, html, gg, i + 1, ii, false, lev);
                    Pullenti.Unitext.UnitextItem cnt = gg.Finish(false, null);
                    if (cnt != null) 
                    {
                        StringBuilder tmp = new StringBuilder();
                        cnt.GetPlaintext(tmp, null);
                        if (tmp.Length > 0) 
                            gen.Append(new Pullenti.Unitext.UnitextComment() { Text = tmp.ToString() }, null, -1, false);
                    }
                    i = ii - 1;
                    continue;
                }
                if (cmd == "shptxt") 
                {
                    List<RtfItem> iii = new List<RtfItem>();
                    int ii;
                    for (ii = i; (ii < items.Count) && (ii < i1); ii++) 
                    {
                        if (items[ii].Level < (it.Level - 1)) 
                            break;
                        else 
                            iii.Add(items[ii]);
                    }
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    _manageUni(items, html, gg, i + 1, ii, false, lev);
                    Pullenti.Unitext.UnitextItem cnt = gg.Finish(true, null);
                    if (cnt is Pullenti.Unitext.UnitextContainer) 
                        (cnt as Pullenti.Unitext.UnitextContainer).Typ = Pullenti.Unitext.UnitextContainerType.Shape;
                    else if (cnt != null) 
                    {
                        Pullenti.Unitext.UnitextContainer cnt2 = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Shape };
                        cnt2.Children.Add(cnt);
                        cnt = cnt2.Optimize(false, null);
                    }
                    if (cnt != null) 
                        gen.Append(cnt, null, -1, false);
                    i = ii - 1;
                    continue;
                }
                if (cmd == "footnote") 
                {
                    List<RtfItem> iii = new List<RtfItem>();
                    int ii;
                    bool endnote = false;
                    for (ii = i; (ii < items.Count) && (ii < i1); ii++) 
                    {
                        if (items[ii].Level <= (it.Level - 1)) 
                            break;
                        else 
                        {
                            iii.Add(items[ii]);
                            if (items[ii].Typ == RftItemTyp.Command && items[ii].Text == "ftnalt") 
                                endnote = true;
                        }
                    }
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    _manageUni(items, html, gg, i + 1, ii, false, lev);
                    Pullenti.Unitext.UnitextItem cnt = gg.Finish(false, null);
                    if (cnt != null) 
                    {
                        Pullenti.Unitext.UnitextFootnote fn = new Pullenti.Unitext.UnitextFootnote() { Content = cnt, IsEndnote = endnote };
                        if (cnt is Pullenti.Unitext.UnitextContainer) 
                        {
                            Pullenti.Unitext.UnitextContainer cc = cnt as Pullenti.Unitext.UnitextContainer;
                            if (cc.Children.Count > 1 && (cc.Children[0] is Pullenti.Unitext.UnitextPlaintext) && (cc.Children[0] as Pullenti.Unitext.UnitextPlaintext).Typ == Pullenti.Unitext.UnitextPlaintextType.Sup) 
                            {
                                fn.CustomMark = (cc.Children[0] as Pullenti.Unitext.UnitextPlaintext).Text;
                                cc.Children.RemoveAt(0);
                                fn.Content = cc.Optimize(true, null);
                            }
                        }
                        gen.Append(fn, null, -1, false);
                    }
                    i = ii - 1;
                    continue;
                }
                if (cmd.StartsWith("listtext")) 
                {
                    string listId = null;
                    int listLevel = 0;
                    int ii;
                    bool ok = false;
                    bool endOfNum = false;
                    StringBuilder num = new StringBuilder();
                    List<RtfItem> tmp = new List<RtfItem>();
                    for (ii = i + 1; ii < items.Count; ii++) 
                    {
                        RtfItem itt = items[ii];
                        tmp.Add(itt);
                        if (itt.Typ == RftItemTyp.Text) 
                        {
                            if (listId != null || endOfNum) 
                            {
                                ok = true;
                                break;
                            }
                            num.Append(itt.Text ?? "");
                            continue;
                        }
                        if (itt.Typ == RftItemTyp.BracketClose && num.Length > 0) 
                            endOfNum = true;
                        if (itt.Typ != RftItemTyp.Command) 
                            continue;
                        if (inTab) 
                        {
                            if ((itt.Text == "cell" || itt.Text == "row" || itt.Text == "trowd") || itt.Text == "intbl" || itt.Text == "nestcell") 
                                break;
                        }
                        if (itt.Text.StartsWith("ls") && itt.Text.Length > 2 && char.IsDigit(itt.Text[2])) 
                        {
                            if (listId != null) 
                                break;
                            listId = itt.Text.Substring(2);
                            continue;
                        }
                        if (itt.Text.StartsWith("ilvl") && itt.Text.Length > 4 && char.IsDigit(itt.Text[4])) 
                        {
                            int.TryParse(itt.Text.Substring(4), out listLevel);
                            continue;
                        }
                        if (itt.Text == "listtext") 
                            break;
                    }
                    if (ok) 
                    {
                        int jj;
                        for (jj = ii + 1; jj < items.Count; jj++) 
                        {
                            RtfItem itt = items[jj];
                            tmp.Add(itt);
                            if (itt.Typ != RftItemTyp.Command) 
                                continue;
                            if (itt.Text == "par") 
                            {
                                jj++;
                                break;
                            }
                            if (itt.Text == "sect") 
                                break;
                            if (itt.Text == "listtext" || itt.Text == "trowd") 
                                break;
                            if (itt.Text.StartsWith("ls")) 
                                break;
                        }
                        if ((ii > 2 && items[ii - 1].Typ == RftItemTyp.Command && items[ii - 2].Typ == RftItemTyp.Command) && items[ii - 2].Text == "*") 
                            ii -= 2;
                        Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        _manageUni(items, html, gg, ii, jj, false, lev);
                        gen.AppendListItem(gg.Finish(true, null), num.ToString().Trim(), listId, listLevel, null);
                        i = jj - 1;
                        continue;
                    }
                }
                if (cmd == "line") 
                {
                    if (html != null) 
                        html.Append("\r\n");
                    else 
                        gen.AppendNewline(false);
                    continue;
                }
                if (cmd == "page" || cmd == "sect") 
                {
                    gen.AppendPagebreak();
                    continue;
                }
                if (cmd == "par") 
                {
                    if (html != null) 
                        html.Append("\r\n");
                    else 
                        gen.AppendNewline(false);
                    continue;
                }
                if (cmd == "brdrt" || cmd == "brdrb") 
                {
                    if (html != null) 
                        html.Append("\r\n<hr/>");
                    else 
                        gen.Append(new Pullenti.Unitext.UnitextMisc() { Typ = Pullenti.Unitext.UnitextMiscType.HorizontalLine }, null, -1, false);
                    continue;
                }
                if (cmd == "intbl" && !inTab) 
                {
                }
                if (((cmd == "trowd" && !inTab)) || ((cmd == "intbl" && !inTab))) 
                {
                    if (lev < 5) 
                    {
                        if (lev > 10) 
                        {
                        }
                        RichTextTable rtfTab = new RichTextTable();
                        RichTextRow rtfRow = new RichTextRow();
                        int ii = _manageTableRow(items, i, rtfRow, lev + 1);
                        if (ii > i) 
                        {
                            rtfTab.AddChild(rtfRow);
                            i = ii;
                            while (i < items.Count) 
                            {
                                if (rtfRow.LastRow) 
                                    break;
                                rtfRow = new RichTextRow();
                                ii = _manageTableRow(items, i, rtfRow, lev + 1);
                                if (ii < 0) 
                                    break;
                                rtfTab.AddChild(rtfRow);
                                if (ii <= i) 
                                    break;
                                i = ii;
                            }
                            rtfTab.Correct();
                            Pullenti.Unitext.UnitextTable tab = _createTable(rtfTab);
                            if (gen != null) 
                                gen.Append(tab, null, -1, false);
                            continue;
                        }
                    }
                }
                int sub = -1;
                if (cmd.StartsWith("super") || cmd.StartsWith("up")) 
                    sub = 0;
                else if (cmd.StartsWith("sub")) 
                    sub = 1;
                if (sub >= 0) 
                {
                    int ii;
                    bool ok = false;
                    StringBuilder tmp = new StringBuilder();
                    for (ii = i + 1; ii < items.Count; ii++) 
                    {
                        if (items[ii].Typ == RftItemTyp.BracketClose || items[ii].Typ == RftItemTyp.BracketOpen) 
                        {
                            ok = true;
                            break;
                        }
                        else if (items[ii].Typ == RftItemTyp.Text) 
                            tmp.Append(items[ii].Text ?? "");
                        else if (items[ii].Typ != RftItemTyp.Command) 
                            break;
                        else if (items[ii].Text == "pard") 
                        {
                            ok = true;
                            break;
                        }
                        else if (items[ii].Text == "footnote") 
                            break;
                    }
                    if (ok) 
                    {
                        string tt = tmp.ToString().Trim();
                        if (tt.Length > 0 && (tt.Length < 10)) 
                        {
                            gen.Append(new Pullenti.Unitext.UnitextPlaintext() { Text = tt, Typ = (sub > 0 ? Pullenti.Unitext.UnitextPlaintextType.Sub : Pullenti.Unitext.UnitextPlaintextType.Sup) }, null, -1, false);
                            i = ii - 1;
                        }
                    }
                    continue;
                }
            }
            return i;
        }
        static Pullenti.Unitext.UnitextTable _createTable(RichTextTable tbl)
        {
            Pullenti.Unitext.UnitextTable res = new Pullenti.Unitext.UnitextTable();
            int rn = 0;
            foreach (RichTextItem rr in tbl.Children) 
            {
                int cn = 0;
                foreach (RichTextItem c in rr.Children) 
                {
                    for (; ; cn++) 
                    {
                        if (res.GetCell(rn, cn) == null) 
                            break;
                    }
                    Pullenti.Unitext.UnitextTablecell cel = res.AddCell(rn, (rn + (c as RichTextCell).RowsSpan) - 1, cn, (cn + (c as RichTextCell).ColsSpan) - 1, c.Tag as Pullenti.Unitext.UnitextItem);
                    if (cel != null) 
                        (c as RichTextCell).ResColIndex = cel.ColBegin;
                }
                rn++;
            }
            if (res.ColsCount > 1) 
            {
                int[] wi = new int[(int)res.ColsCount];
                for (int kk = 0; kk < 3; kk++) 
                {
                    foreach (RichTextItem rr in tbl.Children) 
                    {
                        RichTextRow r = rr as RichTextRow;
                        if (r == null) 
                            continue;
                        if (r.CellsInfo.Count != r.Children.Count) 
                            continue;
                        int sum = 0;
                        foreach (RichTextRow.CellInfo ci in r.CellsInfo) 
                        {
                            sum += ci.Width;
                        }
                        if (sum <= 0) 
                            continue;
                        for (int i = 0; i < r.Children.Count; i++) 
                        {
                            RichTextCell c = r.Children[i] as RichTextCell;
                            if ((c.ResColIndex < 0) || c.ResColIndex >= wi.Length) 
                                continue;
                            int w = (r.CellsInfo[i].Width * 100) / sum;
                            int co = 0;
                            for (int j = c.ResColIndex; (j < (c.ResColIndex + c.ColsSpan)) && (j < wi.Length); j++) 
                            {
                                if (wi[j] > 0) 
                                    w -= wi[j];
                                else 
                                    co++;
                            }
                            if (co == 1 && w > 0) 
                            {
                                for (int j = c.ResColIndex; (j < (c.ResColIndex + c.ColsSpan)) && (j < wi.Length); j++) 
                                {
                                    if (wi[j] == 0) 
                                    {
                                        wi[j] = w;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < wi.Length; i++) 
                {
                    if (wi[i] > 0) 
                        res.SetColWidth(i, string.Format("{0}%", wi[i]));
                }
            }
            return res;
        }
        static int _manageTableRow(List<RtfItem> items, int i, RichTextRow rtfRow, int lev)
        {
            bool ok = false;
            for (; i < items.Count; i++) 
            {
                RtfItem it = items[i];
                if (it.Typ == RftItemTyp.Command && ((it.Text == "trowd" || it.Text == "intbl"))) 
                {
                    ok = true;
                    break;
                }
                if (it.Typ == RftItemTyp.Text || it.Typ == RftItemTyp.Image) 
                    break;
            }
            if (!ok) 
                return -1;
            List<int> colWidth = new List<int>();
            for (; i < items.Count; i++) 
            {
                RtfItem it = items[i];
                if (it.Typ == RftItemTyp.Command && it.Text == "row") 
                    return i + 1;
                int j;
                ok = false;
                for (j = i; j < items.Count; j++) 
                {
                    RtfItem itt = items[j];
                    if (itt.Typ != RftItemTyp.Command) 
                        continue;
                    if (itt.Text == "cell") 
                    {
                        ok = true;
                        break;
                    }
                    if (itt.Text == "row") 
                    {
                        j--;
                        break;
                    }
                    if (itt.Text == "nestcell") 
                    {
                        RichTextRow nRow = new RichTextRow();
                        int jj = _manageNestedTableRow(items, i, nRow, lev + 1);
                        if (jj > 0) 
                        {
                            RichTextTable nTab = new RichTextTable();
                            nTab.AddChild(nRow);
                            while (true) 
                            {
                                nRow = new RichTextRow();
                                int jjj = _manageNestedTableRow(items, jj, nRow, lev + 1);
                                if (jjj < 0) 
                                    break;
                                nTab.AddChild(nRow);
                                jj = jjj;
                            }
                            nTab.Correct();
                            RtfItemProxy ppp = new RtfItemProxy() { Typ = RftItemTyp.Proxy };
                            ppp.Uni = _createTable(nTab);
                            items.RemoveRange(i + 1, jj - i - 1);
                            items[i] = ppp;
                            continue;
                        }
                    }
                    if (rtfRow != null) 
                        rtfRow._addCmd(itt.Text);
                }
                if (ok && rtfRow != null) 
                {
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    _manageUni(items, null, gg, i, j, true, lev);
                    RichTextCell cel = new RichTextCell();
                    cel.Tag = gg.Finish(true, null);
                    cel.EndOf = true;
                    rtfRow.AddChild(cel);
                }
                i = j;
            }
            return -1;
        }
        class RtfItemProxy : Pullenti.Unitext.Internal.Rtf.RtfItem
        {
            public Pullenti.Unitext.UnitextItem Uni;
        }

        static int _manageNestedTableRow(List<RtfItem> items, int i, RichTextRow rtfRow, int lev)
        {
            bool endOfRow = false;
            for (; i < items.Count; i++) 
            {
                RtfItem it = items[i];
                if (it.Typ == RftItemTyp.Command && ((it.Text == "row" || it.Text == "cell"))) 
                    return -1;
                int j;
                bool ok = false;
                for (j = i; j < items.Count; j++) 
                {
                    RtfItem itt = items[j];
                    if (itt.Typ != RftItemTyp.Command) 
                        continue;
                    if (itt.Text == "nestcell") 
                    {
                        ok = true;
                        break;
                    }
                    if (itt.Text == "row" || itt.Text == "cell") 
                        return -1;
                    if (itt.Text == "nesttableprops" || itt.Text == "nestrow") 
                    {
                        endOfRow = true;
                        break;
                    }
                }
                if (ok) 
                {
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    _manageUni(items, null, gg, i, j, true, lev + 1);
                    RichTextCell cel = new RichTextCell();
                    cel.Tag = gg.Finish(true, null);
                    cel.EndOf = true;
                    rtfRow.AddChild(cel);
                }
                i = j;
                if (endOfRow) 
                    break;
            }
            if (!endOfRow) 
                return -1;
            for (; i < items.Count; i++) 
            {
                RtfItem it = items[i];
                if (it.Typ != RftItemTyp.Command) 
                    continue;
                if (it.Text == "row" || it.Text == "cell") 
                    return -1;
                if (it.Text == "nestrow") 
                    return i + 1;
                rtfRow._addCmd(it.Text);
            }
            return -1;
        }
    }
}