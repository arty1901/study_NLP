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
using System.Xml;

namespace Pullenti.Unitext.Internal.Misc
{
    class OdtHelper
    {
        Dictionary<string, OdtStyle> m_Styles = new Dictionary<string, OdtStyle>();
        Dictionary<string, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle> m_Nums = new Dictionary<string, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle>();
        Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle m_OutlineNum;
        class OdtStyle
        {
            public string Name;
            public string ParentName;
            public string FontFamily;
            public bool IsSuper;
            public bool IsSub;
            public bool IsWingdings;
            public string NumStyleId;
            public string TableWidth;
            public double TableColumnWidth;
        }

        public Pullenti.Unitext.UnitextDocument CreateUni(XmlNode xml, XmlNode styles)
        {
            if (xml.LocalName != "document-content") 
                return null;
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Odt };
            Pullenti.Unitext.UnitextPagesection sect = new Pullenti.Unitext.UnitextPagesection();
            if (styles != null) 
            {
                foreach (XmlNode x in styles.ChildNodes) 
                {
                    if (x.LocalName == "master-styles") 
                    {
                        foreach (XmlNode xx in x.ChildNodes) 
                        {
                            if (xx.LocalName == "master-page") 
                            {
                                foreach (XmlNode xxx in xx.ChildNodes) 
                                {
                                    if (xxx.LocalName == "header") 
                                    {
                                        Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                        this._readGen0(xxx, gg, true, null, 0);
                                        sect.Header = gg.Finish(true, null);
                                    }
                                    else if (xxx.LocalName == "footer") 
                                    {
                                        Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                        this._readGen0(xxx, gg, true, null, 0);
                                        sect.Footer = gg.Finish(true, null);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else if (x.LocalName == "styles") 
                    {
                        foreach (XmlNode xx in x.ChildNodes) 
                        {
                            if (xx.LocalName == "style") 
                                this._readStyle(xx);
                            else if (xx.LocalName == "list-style") 
                                this._readListStyle(xx);
                        }
                    }
                }
            }
            if (sect.Header != null || sect.Footer != null) 
                doc.Sections.Add(sect);
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "automatic-styles") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        if (xx.LocalName == "style") 
                            this._readStyle(xx);
                        else if (xx.LocalName == "list-style") 
                            this._readListStyle(xx);
                    }
                }
                else if (x.LocalName == "body") 
                {
                    Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    this._readGen0(x, gen, false, null, 0);
                    doc.Content = gen.Finish(true, null);
                }
            }
            doc.Optimize(false, null);
            if (doc.Content == null) 
                return null;
            return doc;
        }
        void _readStyle(XmlNode xx)
        {
            OdtStyle sty = new OdtStyle();
            if (xx.Attributes != null) 
            {
                foreach (XmlAttribute a in xx.Attributes) 
                {
                    if (a.LocalName == "name") 
                        sty.Name = a.Value;
                    else if (a.LocalName == "family") 
                        sty.FontFamily = a.Value;
                    else if (a.LocalName == "parent-style-name") 
                        sty.ParentName = a.Value;
                    else if (a.LocalName == "list-style-name") 
                        sty.NumStyleId = a.Value;
                }
            }
            if (sty.Name == null || m_Styles.ContainsKey(sty.Name)) 
                return;
            m_Styles.Add(sty.Name, sty);
            foreach (XmlNode p in xx.ChildNodes) 
            {
                if (p.LocalName == "text-properties") 
                {
                    if (p.Attributes != null) 
                    {
                        foreach (XmlAttribute a in p.Attributes) 
                        {
                            if (a.LocalName == "text-position") 
                            {
                                if (a.Value.StartsWith("super")) 
                                    sty.IsSuper = true;
                                else if (a.Value.StartsWith("sub")) 
                                    sty.IsSub = true;
                            }
                            else if (a.LocalName == "font-name") 
                            {
                                if (a.Value == "Wingdings") 
                                    sty.IsWingdings = true;
                            }
                        }
                    }
                }
                else if (p.LocalName == "table-column-properties") 
                {
                    if (p.Attributes != null) 
                    {
                        foreach (XmlAttribute a in p.Attributes) 
                        {
                            if (a.LocalName == "column-width") 
                            {
                                int dd = Pullenti.Unitext.Internal.Uni.UnitextGen.ConvertToMM(a.Value, null);
                                if (dd > 0) 
                                {
                                    sty.TableColumnWidth = dd;
                                    continue;
                                }
                                string vv = a.Value;
                                if (vv.EndsWith("in")) 
                                    vv = vv.Substring(0, vv.Length - 2);
                                double d;
                                if (Pullenti.Util.MiscHelper.TryParseDouble(vv, out d)) 
                                    sty.TableColumnWidth = d;
                            }
                        }
                    }
                }
                else if (p.LocalName == "table-properties") 
                {
                    if (p.Attributes != null) 
                    {
                        foreach (XmlAttribute a in p.Attributes) 
                        {
                            if (a.LocalName == "width") 
                                sty.TableWidth = a.Value;
                        }
                    }
                }
            }
        }
        void _readListStyle(XmlNode xx)
        {
            string nam = null;
            if (xx.Attributes != null) 
            {
                foreach (XmlAttribute a in xx.Attributes) 
                {
                    if (a.LocalName == "name") 
                        nam = a.Value;
                }
            }
            if (nam == null) 
                return;
            Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle num = new Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle() { Id = nam };
            this._readOdtNumStyle(num, xx);
            if (!m_Nums.ContainsKey(nam)) 
                m_Nums.Add(nam, num);
        }
        void _readOdtNumStyle(Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle num, XmlNode xml)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "list-level-style-number") 
                {
                    Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel lev = new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel();
                    num.Levels.Add(lev);
                    string pref = null;
                    string suf = null;
                    string frm = "1";
                    int cou = 1;
                    if (x.Attributes != null) 
                    {
                        foreach (XmlAttribute a in x.Attributes) 
                        {
                            if (a.LocalName == "num-suffix") 
                                suf = a.Value;
                            else if (a.LocalName == "num-prefix") 
                                pref = a.Value;
                            else if (a.LocalName == "num-format") 
                                frm = a.Value;
                            else if (a.LocalName == "display-levels") 
                                int.TryParse(a.Value, out cou);
                            else if (a.LocalName == "start-value") 
                            {
                                int nn;
                                if (int.TryParse(a.Value, out nn)) 
                                    lev.Start = nn;
                            }
                        }
                    }
                    if (frm == "1") 
                        lev.Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal;
                    else if (frm == "a") 
                        lev.Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.LowerLetter;
                    else if (frm == "A") 
                        lev.Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.UpperLetter;
                    else if (frm == "i") 
                        lev.Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.LowerRoman;
                    else if (frm == "I") 
                        lev.Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.UpperRoman;
                    else if (frm.StartsWith("А")) 
                        lev.Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.UpperCyrLetter;
                    else if (frm.StartsWith("а")) 
                        lev.Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.LowerCyrLetter;
                    StringBuilder tmp = new StringBuilder();
                    if (pref != null) 
                        tmp.Append(pref);
                    for (int i = (num.Levels.Count + 1) - cou; i <= num.Levels.Count; i++) 
                    {
                        string prevFrm = null;
                        if (i < num.Levels.Count) 
                            prevFrm = num.Levels[i - 1].Format;
                        if (!string.IsNullOrEmpty(prevFrm) && prevFrm[0] != '%') 
                            tmp.Append(prevFrm[0]);
                        tmp.AppendFormat("%{0}", i);
                        if (!string.IsNullOrEmpty(prevFrm) && !char.IsDigit(prevFrm[prevFrm.Length - 1])) 
                            tmp.Append(prevFrm[prevFrm.Length - 1]);
                    }
                    if (suf != null) 
                        tmp.Append(suf);
                    lev.Format = tmp.ToString();
                }
                else if (x.LocalName == "list-level-style-bullet") 
                {
                    Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel lev = new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Bullet };
                    if (x.Attributes != null) 
                    {
                        foreach (XmlAttribute a in x.Attributes) 
                        {
                            if (a.LocalName == "bullet-char") 
                                lev.Format = a.Value;
                        }
                    }
                    num.Levels.Add(lev);
                }
            }
        }
        void _readGen0(XmlNode xml, Pullenti.Unitext.Internal.Uni.UnitextGen gen, bool pure = false, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle numStyle = null, int numLevel = 0)
        {
            if (xml == null) 
                return;
            List<XmlNode> li = new List<XmlNode>();
            li.Add(xml);
            this._readGen(li, gen, pure, numStyle, numLevel);
        }
        void _readGen(List<XmlNode> xmlStack, Pullenti.Unitext.Internal.Uni.UnitextGen gen, bool pure = false, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle numStyle = null, int numLevel = 0)
        {
            XmlNode xml = xmlStack[xmlStack.Count - 1];
            if (!pure && xml.LocalName != "#text") 
            {
                if (xml.LocalName == "s") 
                {
                    gen.AppendText(" ", false);
                    if (xml.Attributes != null) 
                    {
                        foreach (XmlAttribute a in xml.Attributes) 
                        {
                            if (a.LocalName == "c") 
                            {
                                int cou;
                                if (int.TryParse(a.Value ?? "", out cou)) 
                                {
                                    for (; cou > 1; cou--) 
                                    {
                                        gen.AppendText(" ", false);
                                    }
                                }
                            }
                        }
                    }
                    return;
                }
                if (xml.LocalName == "soft-page-break") 
                {
                    gen.AppendPagebreak();
                    return;
                }
                if (xml.LocalName == "line-break") 
                {
                    gen.AppendNewline(false);
                    return;
                }
                if (xml.LocalName == "table") 
                {
                    Pullenti.Unitext.Internal.Uni.UnitextGenTable gt = new Pullenti.Unitext.Internal.Uni.UnitextGenTable();
                    List<double> colwi = new List<double>();
                    foreach (XmlNode r in xml.ChildNodes) 
                    {
                        if (r.LocalName == "table-row") 
                        {
                            List<Pullenti.Unitext.Internal.Uni.UniTextGenCell> row = new List<Pullenti.Unitext.Internal.Uni.UniTextGenCell>();
                            gt.Cells.Add(row);
                            foreach (XmlNode c in r.ChildNodes) 
                            {
                                if (c.LocalName == "table-cell") 
                                {
                                    Pullenti.Unitext.Internal.Uni.UniTextGenCell cel = new Pullenti.Unitext.Internal.Uni.UniTextGenCell();
                                    row.Add(cel);
                                    if (c.Attributes != null) 
                                    {
                                        foreach (XmlAttribute a in c.Attributes) 
                                        {
                                            if (a.LocalName == "number-rows-spanned") 
                                            {
                                                int nn;
                                                if (int.TryParse(a.Value, out nn)) 
                                                    cel.RowSpan = nn;
                                            }
                                            else if (a.LocalName == "number-columns-spanned") 
                                            {
                                                int nn;
                                                if (int.TryParse(a.Value, out nn)) 
                                                    cel.ColSpan = nn;
                                            }
                                        }
                                    }
                                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                    xmlStack.Add(c);
                                    this._readGen(xmlStack, gg, true, numStyle, numLevel);
                                    xmlStack.RemoveAt(xmlStack.Count - 1);
                                    cel.Content = gg.Finish(true, null);
                                }
                            }
                        }
                        else if (r.LocalName == "table-columns") 
                        {
                            foreach (XmlNode x in r.ChildNodes) 
                            {
                                if (x.LocalName == "table-column") 
                                {
                                    if (xml.Attributes != null) 
                                    {
                                        foreach (XmlAttribute a in x.Attributes) 
                                        {
                                            if (a.LocalName == "style-name") 
                                            {
                                                OdtStyle sty1;
                                                if (m_Styles.TryGetValue(a.Value, out sty1)) 
                                                {
                                                    if (sty1.TableColumnWidth > 0) 
                                                        colwi.Add(sty1.TableColumnWidth);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (r.LocalName == "table-column") 
                        {
                            if (r.Attributes != null) 
                            {
                                foreach (XmlAttribute a in r.Attributes) 
                                {
                                    if (a.LocalName == "style-name") 
                                    {
                                        OdtStyle sty1;
                                        if (m_Styles.TryGetValue(a.Value, out sty1)) 
                                        {
                                            if (sty1.TableColumnWidth > 0) 
                                                colwi.Add(sty1.TableColumnWidth);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    double sum = (double)0;
                    foreach (double w in colwi) 
                    {
                        sum += w;
                    }
                    if (sum > 0) 
                    {
                        foreach (double w in colwi) 
                        {
                            gt.m_ColWidth.Add(string.Format("{0}%", (int)(((w * 100) / sum))));
                        }
                    }
                    Pullenti.Unitext.UnitextTable tab = gt.Convert();
                    if (tab != null) 
                    {
                        gen.Append(tab, null, -1, false);
                        return;
                    }
                }
                if (xml.LocalName == "a") 
                {
                    string uri = null;
                    if (xml.Attributes != null) 
                    {
                        foreach (XmlAttribute a in xml.Attributes) 
                        {
                            if (a.LocalName == "href") 
                            {
                                uri = a.Value;
                                break;
                            }
                        }
                    }
                    if (uri != null) 
                    {
                        Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        xmlStack.Add(xml);
                        this._readGen(xmlStack, gg, true, null, 0);
                        xmlStack.RemoveAt(xmlStack.Count - 1);
                        Pullenti.Unitext.UnitextItem it = gg.Finish(true, null);
                        if (it != null) 
                        {
                            Pullenti.Unitext.UnitextHyperlink hr = new Pullenti.Unitext.UnitextHyperlink() { Href = uri };
                            hr.Content = it;
                            gen.Append(hr, null, -1, false);
                            return;
                        }
                    }
                }
                if (xml.LocalName == "note") 
                {
                    Pullenti.Unitext.UnitextFootnote fn = new Pullenti.Unitext.UnitextFootnote();
                    if (xml.Attributes != null) 
                    {
                        foreach (XmlAttribute a in xml.Attributes) 
                        {
                            if (a.LocalName == "note-class" && a.Value == "endnote") 
                                fn.IsEndnote = true;
                        }
                    }
                    foreach (XmlNode x in xml.ChildNodes) 
                    {
                        if (x.LocalName == "note-body") 
                        {
                            Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            xmlStack.Add(x);
                            this._readGen(xmlStack, gg, true, null, 0);
                            xmlStack.RemoveAt(xmlStack.Count - 1);
                            fn.Content = gg.Finish(true, null);
                            break;
                        }
                    }
                    if (fn.Content != null) 
                        gen.Append(fn, null, -1, false);
                    return;
                }
                if (xml.LocalName == "bookmark-ref") 
                {
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    xmlStack.Add(xml);
                    this._readGen(xmlStack, gg, true, null, 0);
                    xmlStack.RemoveAt(xmlStack.Count - 1);
                    Pullenti.Unitext.UnitextPlaintext pl = gg.Finish(true, null) as Pullenti.Unitext.UnitextPlaintext;
                    if (pl != null) 
                    {
                        if (!gen.LastText.EndsWith(pl.Text)) 
                            gen.AppendText(pl.Text, false);
                        return;
                    }
                }
                if (xml.LocalName == "annotation") 
                {
                    Pullenti.Unitext.UnitextComment cmt = new Pullenti.Unitext.UnitextComment();
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    foreach (XmlNode x in xml.ChildNodes) 
                    {
                        if (x.LocalName == "creator") 
                            cmt.Author = x.InnerText;
                        else if (x.LocalName == "date") 
                        {
                        }
                        else 
                        {
                            xmlStack.Add(x);
                            this._readGen(xmlStack, gg, false, null, 0);
                            xmlStack.RemoveAt(xmlStack.Count - 1);
                        }
                    }
                    Pullenti.Unitext.UnitextItem it = gg.Finish(true, null);
                    if (it != null) 
                    {
                        StringBuilder tmp = new StringBuilder();
                        it.GetPlaintext(tmp, null);
                        if (tmp.Length > 0) 
                        {
                            cmt.Text = tmp.ToString();
                            gen.Append(cmt, null, -1, false);
                        }
                    }
                    return;
                }
                int outLineLevel = 0;
                Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle numSty = null;
                if (xml.LocalName == "p" || xml.LocalName == "h") 
                {
                    if (xml.Attributes != null) 
                    {
                        foreach (XmlAttribute a in xml.Attributes) 
                        {
                            if (a.LocalName == "outline-level") 
                                int.TryParse(a.Value ?? "", out outLineLevel);
                            else if (a.LocalName == "style-name") 
                            {
                                OdtStyle sty1;
                                Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle num0 = null;
                                if (a.Value == "P5") 
                                {
                                }
                                if (!m_Styles.TryGetValue(a.Value, out sty1)) 
                                    continue;
                                for (int kk = 0; kk < 10; kk++) 
                                {
                                    if (sty1.NumStyleId != null) 
                                    {
                                        m_Nums.TryGetValue(sty1.NumStyleId, out num0);
                                        break;
                                    }
                                    if (sty1.ParentName == null) 
                                        break;
                                    if (!m_Styles.TryGetValue(sty1.ParentName, out sty1)) 
                                        break;
                                }
                                if (num0 == null) 
                                    continue;
                                numSty = num0;
                            }
                        }
                    }
                }
                if (outLineLevel > 0 || numSty != null) 
                {
                    if (m_OutlineNum == null && numSty == null) 
                    {
                        m_OutlineNum = new Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle();
                        m_OutlineNum.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1." });
                        m_OutlineNum.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2." });
                        m_OutlineNum.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2.%3." });
                        m_OutlineNum.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2.%3.%4." });
                        m_OutlineNum.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2.%3.%4.%5." });
                        m_OutlineNum.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2.%3.%4.%5.%6." });
                    }
                    Pullenti.Unitext.UnitextList li = new Pullenti.Unitext.UnitextList() { Level = outLineLevel - 1 };
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    foreach (XmlNode x in xml.ChildNodes) 
                    {
                        xmlStack.Add(x);
                        this._readGen(xmlStack, gg, false, (numSty == null ? m_OutlineNum : numStyle), numLevel);
                        xmlStack.RemoveAt(xmlStack.Count - 1);
                    }
                    gen.AppendNewline(true);
                    gen.Append(gg.Finish(true, null), (numSty == null ? m_OutlineNum : numSty), outLineLevel - 1, false);
                    gen.AppendNewline(false);
                    gen.Append(li, null, -1, false);
                    return;
                }
                if (xml.LocalName == "list") 
                {
                    Pullenti.Unitext.UnitextList li = new Pullenti.Unitext.UnitextList() { Level = numLevel };
                    Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle num0 = null;
                    bool contNum = false;
                    string styleName = null;
                    if (xml.Attributes != null) 
                    {
                        foreach (XmlAttribute a in xml.Attributes) 
                        {
                            if (a.LocalName == "style-name") 
                            {
                                OdtStyle sty1;
                                if (m_Styles.TryGetValue(a.Value, out sty1)) 
                                {
                                    if (sty1.NumStyleId != null) 
                                        m_Nums.TryGetValue((styleName = sty1.NumStyleId), out num0);
                                    else if (sty1.ParentName != null) 
                                    {
                                        OdtStyle sty2;
                                        if (m_Styles.TryGetValue(sty1.ParentName, out sty2)) 
                                        {
                                            if (sty2.NumStyleId != null) 
                                                m_Nums.TryGetValue((styleName = sty2.NumStyleId), out num0);
                                        }
                                    }
                                }
                                else 
                                {
                                    m_Nums.TryGetValue((styleName = a.Value), out num0);
                                    if (styleName == "L9") 
                                    {
                                    }
                                }
                            }
                            else if (a.LocalName == "continue-numbering") 
                                contNum = true;
                        }
                    }
                    if (numStyle == null || num0 != null) 
                    {
                        numStyle = num0;
                        numLevel = 0;
                    }
                    if (numStyle == null) 
                    {
                        numStyle = new Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle();
                        numStyle.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1." });
                        numStyle.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2." });
                        numStyle.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2.%3." });
                        numStyle.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2.%3.%4." });
                        numStyle.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2.%3.%4.%5." });
                        numStyle.Levels.Add(new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel() { Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal, Format = "%1.%2.%3.%4.%5.%6." });
                        numLevel = 0;
                        if (styleName != null && !m_Nums.ContainsKey(styleName)) 
                            m_Nums.Add(styleName, numStyle);
                    }
                    foreach (XmlNode x in xml.ChildNodes) 
                    {
                        if (x.LocalName == "list-item") 
                        {
                            int n0 = 0;
                            if (x.Attributes != null) 
                            {
                                foreach (XmlAttribute a in x.Attributes) 
                                {
                                    if (a.LocalName == "start-value") 
                                        int.TryParse(a.Value, out n0);
                                }
                            }
                            bool startSubList = false;
                            foreach (XmlNode xx in x.ChildNodes) 
                            {
                                if (xx.LocalName == "list") 
                                    startSubList = true;
                                break;
                            }
                            if (n0 > 0 && (numLevel < numStyle.Levels.Count)) 
                                numStyle.Levels[numLevel].Current = n0;
                            Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            xmlStack.Add(x);
                            this._readGen(xmlStack, gg, true, numStyle, numLevel + 1);
                            xmlStack.RemoveAt(xmlStack.Count - 1);
                            gen.AppendNewline(true);
                            if (startSubList) 
                                gen.Append(gg.Finish(true, null), null, 0, false);
                            else 
                                gen.Append(gg.Finish(true, null), numStyle, numLevel, false);
                        }
                        else if (x.LocalName == "list-header") 
                        {
                            Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            xmlStack.Add(x);
                            this._readGen(xmlStack, gg, true, numStyle, numLevel + 1);
                            xmlStack.RemoveAt(xmlStack.Count - 1);
                            gen.AppendNewline(true);
                            gen.Append(gg.Finish(true, null), null, 0, false);
                        }
                    }
                    gen.Append(li, null, -1, false);
                    return;
                }
                if (xml.LocalName == "custom-shape") 
                {
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    xmlStack.Add(xml);
                    this._readGen(xmlStack, gg, true, null, 0);
                    xmlStack.RemoveAt(xmlStack.Count - 1);
                    Pullenti.Unitext.UnitextItem it = gg.Finish(true, null);
                    if (it == null) 
                        return;
                    if (it is Pullenti.Unitext.UnitextContainer) 
                        (it as Pullenti.Unitext.UnitextContainer).Typ = Pullenti.Unitext.UnitextContainerType.Shape;
                    else 
                    {
                        Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Shape };
                        cnt.Children.Add(it);
                        it = cnt.Optimize(false, null);
                    }
                    if (it != null) 
                        gen.Append(it, null, -1, false);
                    return;
                }
                if (xml.LocalName == "image") 
                {
                    Pullenti.Unitext.UnitextImage img = new Pullenti.Unitext.UnitextImage();
                    if (xml.Attributes != null) 
                    {
                        foreach (XmlAttribute a in xml.Attributes) 
                        {
                            if (a.LocalName == "href") 
                            {
                                img.Id = a.Value;
                                break;
                            }
                        }
                    }
                    if (img.Id != null) 
                    {
                        gen.Append(img, null, -1, false);
                        xmlStack.Add(xml);
                        for (int jj = xmlStack.Count - 1; jj >= 0; jj--) 
                        {
                            XmlNode pp = xmlStack[jj];
                            if (pp.Attributes == null) 
                                continue;
                            foreach (XmlAttribute a in pp.Attributes) 
                            {
                                if (a.LocalName == "width" && img.Width == null) 
                                    img.Width = Pullenti.Unitext.Internal.Uni.UnitextGen.ConvertToPt(a.Value, null);
                                else if (a.LocalName == "height" && img.Height == null && a.Value != null) 
                                    img.Height = Pullenti.Unitext.Internal.Uni.UnitextGen.ConvertToPt(a.Value, null);
                            }
                            if (img.Width != null || img.Height != null) 
                                break;
                        }
                        xmlStack.RemoveAt(xmlStack.Count - 1);
                    }
                    return;
                }
                OdtStyle sty = null;
                if (xml.Attributes != null) 
                {
                    foreach (XmlAttribute a in xml.Attributes) 
                    {
                        if (a.LocalName == "style-name") 
                        {
                            m_Styles.TryGetValue(a.Value, out sty);
                            if (a.Value == "T53") 
                            {
                            }
                            if (sty != null && ((sty.IsSuper || sty.IsSub))) 
                            {
                                Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                xmlStack.Add(xml);
                                this._readGen(xmlStack, gg, true, null, 0);
                                xmlStack.RemoveAt(xmlStack.Count - 1);
                                Pullenti.Unitext.UnitextItem it = gg.Finish(true, null);
                                if (it == null) 
                                    continue;
                                StringBuilder tmp = new StringBuilder();
                                it.GetPlaintext(tmp, null);
                                string tt = tmp.ToString().Trim();
                                if (tt.Length > 0 && (tt.Length < 100)) 
                                {
                                    gen.Append(new Pullenti.Unitext.UnitextPlaintext() { Text = tt, Typ = (sty.IsSuper ? Pullenti.Unitext.UnitextPlaintextType.Sub : Pullenti.Unitext.UnitextPlaintextType.Sup) }, null, -1, false);
                                    return;
                                }
                            }
                            if (sty != null && sty.IsWingdings) 
                            {
                                Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                xmlStack.Add(xml);
                                this._readGen(xmlStack, gg, true, null, 0);
                                xmlStack.RemoveAt(xmlStack.Count - 1);
                                Pullenti.Unitext.UnitextItem it = gg.Finish(true, null);
                                if (it == null) 
                                    continue;
                                StringBuilder tmp = new StringBuilder();
                                it.GetPlaintext(tmp, null);
                                string tt = WingdingsHelper.GetUnicodeString(tmp.ToString().Trim());
                                if (tt.Length > 0) 
                                    gen.AppendText(tt, false);
                                return;
                            }
                        }
                    }
                }
            }
            if (xml.LocalName == "p") 
                gen.AppendNewline(true);
            else if (xml.LocalName == "h") 
                gen.AppendNewline(false);
            if (xml.ChildNodes.Count == 0 && xml.InnerText != null) 
            {
                string txt = xml.InnerText;
                if (txt.IndexOf((char)0xAD) >= 0) 
                    txt = txt.Replace(string.Format("{0}", (char)0xAD), "");
                if (txt.Length == 1 && ((int)txt[0]) >= 0xF000) 
                    gen.AppendText(string.Format("{0}", (char)((((int)txt[0]) - 0xF000))), false);
                else 
                    gen.AppendText(txt, false);
            }
            else 
                foreach (XmlNode x in xml.ChildNodes) 
                {
                    xmlStack.Add(x);
                    this._readGen(xmlStack, gen, false, numStyle, numLevel);
                    xmlStack.RemoveAt(xmlStack.Count - 1);
                }
            if (xml.LocalName == "p" || xml.LocalName == "h") 
            {
                gen.AppendNewline(false);
                if (xml.LocalName == "h") 
                    gen.AppendNewline(false);
            }
        }
    }
}