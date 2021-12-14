/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Pullenti.Unitext.Internal.Word
{
    class DocTextStyles
    {
        public Dictionary<string, DocTextStyle> m_TextStyles = new Dictionary<string, DocTextStyle>();
        public Pullenti.Unitext.UnitextStyle DefStyle;
        public List<Pullenti.Unitext.UnitextStyle> UStyles = new List<Pullenti.Unitext.UnitextStyle>();
        Dictionary<string, Pullenti.Unitext.UnitextStyle> m_UStyles = new Dictionary<string, Pullenti.Unitext.UnitextStyle>();
        public bool Ignore;
        public Pullenti.Unitext.UnitextStyle RegisterStyle(Pullenti.Unitext.UnitextStyle st)
        {
            if (Ignore) 
                return st;
            string key = st.ToString();
            if (string.IsNullOrEmpty(key)) 
                return null;
            Pullenti.Unitext.UnitextStyle res;
            if (m_UStyles.TryGetValue(key, out res)) 
                return res;
            m_UStyles.Add(key, st);
            st.Id = UStyles.Count;
            UStyles.Add(st);
            return st;
        }
        public DocTextStyle GetStyle(XmlNode x, string nam)
        {
            string id = _readAttrVal(x, nam, null);
            if (id == null) 
                return null;
            DocTextStyle res;
            if (!m_TextStyles.TryGetValue(id, out res)) 
                return null;
            return res;
        }
        string m_MajorFont;
        string m_MinorFont;
        static string _readAttrVal(XmlNode x, string nam, string nam2 = null)
        {
            if (x.Attributes != null) 
            {
                foreach (XmlAttribute a in x.Attributes) 
                {
                    if (a.LocalName == nam) 
                        return a.Value;
                    else if (nam2 != null && a.LocalName == nam2) 
                        return a.Value;
                }
            }
            return null;
        }
        static string _getSizeVal(string val)
        {
            if (val == null) 
                return null;
            foreach (char ch in val) 
            {
                if (char.IsLetter(ch)) 
                    return val;
            }
            double d;
            if (!Pullenti.Util.MiscHelper.TryParseDouble(val, out d)) 
                return val;
            d = Math.Round(d / 20, 2);
            return string.Format("{0}pt", Pullenti.Util.MiscHelper.OutDouble(d));
        }
        static string _corrColor(string val)
        {
            if (val == null) 
                return null;
            if (val.Length != 6) 
                return val;
            foreach (char ch in val) 
            {
                if (char.IsDigit(ch)) 
                {
                }
                else if (ch >= 'A' && ch <= 'F') 
                {
                }
                else 
                    return val;
            }
            return val = "#" + val;
        }
        public void ReadUnitextStyle(XmlNode node, Pullenti.Unitext.UnitextStyle st)
        {
            foreach (XmlNode x in node.ChildNodes) 
            {
                if (x.LocalName == "pStyle" || x.LocalName == "rStyle") 
                {
                    string id = _readAttrVal(x, "val", null);
                    if (m_TextStyles.ContainsKey(id)) 
                        m_TextStyles[id].UStyle.CopyTo(st);
                }
                else if (x.LocalName == "b") 
                {
                    string val = _readAttrVal(x, "val", null);
                    if (val == "0") 
                        val = "normal";
                    else 
                        val = "bold";
                    st.AddAttr("font-weight", val, false);
                }
                else if (x.LocalName == "i") 
                    st.AddAttr("font-style", "italic", false);
                else if (x.LocalName == "u") 
                    st.AddAttr("text-decoration", "underline", false);
                else if (x.LocalName == "strike") 
                {
                    string val = _readAttrVal(x, "val", null);
                    if (val != "0") 
                        st.AddAttr("text-decoration", "line-through", false);
                }
                else if (x.LocalName == "caps") 
                    st.AddAttr("upper-case", "true", false);
                else if (x.LocalName == "rFonts") 
                {
                    string nam = _readAttrVal(x, "cs", "ansii");
                    if (nam == null) 
                        nam = _readAttrVal(x, "ascii", "hAnsii");
                    if (nam == null) 
                    {
                        foreach (XmlAttribute a in x.Attributes) 
                        {
                            if (a.LocalName.EndsWith("theme", StringComparison.OrdinalIgnoreCase)) 
                            {
                                if (a.Value.StartsWith("major", StringComparison.OrdinalIgnoreCase)) 
                                    nam = m_MajorFont;
                                else if (a.Value.StartsWith("minor", StringComparison.OrdinalIgnoreCase)) 
                                    nam = m_MinorFont;
                            }
                        }
                    }
                    if (nam == null) 
                    {
                        foreach (XmlAttribute a in x.Attributes) 
                        {
                            if (!a.LocalName.EndsWith("theme", StringComparison.OrdinalIgnoreCase)) 
                            {
                                nam = a.Value;
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(nam)) 
                        st.AddAttr("font-name", nam, false);
                }
                else if (x.LocalName == "sz" || x.LocalName == "szCs") 
                {
                    double d;
                    if (Pullenti.Util.MiscHelper.TryParseDouble(_readAttrVal(x, "val", null), out d)) 
                        st.AddAttr("font-size", string.Format("{0}pt", Pullenti.Util.MiscHelper.OutDouble(d / 2)), false);
                }
                else if (x.LocalName == "highlight" || x.LocalName == "color") 
                {
                    string val = _corrColor(_readAttrVal(x, "val", null));
                    if (val == null || val == "auto") 
                        continue;
                    if (x.LocalName == "color") 
                        st.AddAttr("color", val, false);
                    else 
                        st.AddAttr("background-color", val, false);
                }
                else if (x.LocalName == "shd") 
                {
                    string val = _readAttrVal(x, "color", null);
                    if (val == "auto") 
                        val = _corrColor(_readAttrVal(x, "fill", null));
                    else 
                        val = _corrColor(val);
                    if (val != null && val != "auto") 
                        st.AddAttr("background-color", val, false);
                }
                else if (x.LocalName == "rPr") 
                    this.ReadUnitextStyle(x, st);
                else if (x.LocalName == "jc") 
                {
                    string val = _readAttrVal(x, "val", null);
                    if (val == "start") 
                        val = "left";
                    else if (val == "end") 
                        val = "right";
                    else if (val == "both") 
                        val = "justify";
                    st.AddAttr("text-align", val, false);
                }
                else if (x.LocalName == "spacing") 
                {
                    string rule = _readAttrVal(x, "lineRule", null);
                    foreach (XmlAttribute a in x.Attributes) 
                    {
                        if (a.LocalName == "before" || a.LocalName == "top") 
                            st.AddAttr("margin-top", _getSizeVal(a.Value), false);
                        else if (a.LocalName == "after" || a.LocalName == "bottom") 
                            st.AddAttr("margin-bottom", _getSizeVal(a.Value), false);
                        else if (a.LocalName == "line") 
                        {
                            double d;
                            if (Pullenti.Util.MiscHelper.TryParseDouble(a.Value, out d)) 
                            {
                                string val = null;
                                if (rule == "auto") 
                                {
                                    d = Math.Round(d / 240, 2);
                                    val = string.Format("{0}", Pullenti.Util.MiscHelper.OutDouble(d));
                                }
                                else if (rule == "exact") 
                                {
                                    d = Math.Round(d / 20, 2);
                                    val = string.Format("{0}pt", Pullenti.Util.MiscHelper.OutDouble(d));
                                    double? pt = Pullenti.Util.SizeunitConverter.Convert(st.GetAttr("font-size"), "pt");
                                    if (pt != null && pt.Value > 0) 
                                        val = Pullenti.Util.MiscHelper.OutDouble(Math.Round(d / pt.Value, 2));
                                }
                                else 
                                {
                                }
                                if (val != null) 
                                    st.AddAttr("line-height", val, false);
                            }
                        }
                    }
                }
                else if (x.LocalName == "tcW") 
                {
                    string val = _readAttrVal(x, "w", null);
                    if (val != null) 
                        st.AddAttr("width", _getSizeVal(val), false);
                }
                else if (x.LocalName == "ind") 
                {
                    foreach (XmlAttribute a in x.Attributes) 
                    {
                        if (a.LocalName == "left" || a.LocalName == "start") 
                            st.AddAttr("margin-left", _getSizeVal(a.Value), false);
                        else if (a.LocalName == "right" || a.LocalName == "left") 
                            st.AddAttr("margin-right", _getSizeVal(a.Value), false);
                        else if (a.LocalName == "firstLine") 
                            st.AddAttr("text-indent", _getSizeVal(a.Value), false);
                    }
                }
            }
        }
        public void ReadTheme(XmlNode node)
        {
            foreach (XmlNode x1 in node.ChildNodes) 
            {
                foreach (XmlNode x2 in x1.ChildNodes) 
                {
                    if (x2.LocalName == "fontScheme") 
                    {
                        foreach (XmlNode x3 in x2.ChildNodes) 
                        {
                            if (x3.LocalName == "majorFont" || x3.LocalName == "minorFont") 
                            {
                                foreach (XmlNode x4 in x3.ChildNodes) 
                                {
                                    if (x4.LocalName == "latin") 
                                    {
                                        if (x3.LocalName == "majorFont") 
                                            m_MajorFont = _readAttrVal(x4, "typeface", null);
                                        else 
                                            m_MinorFont = _readAttrVal(x4, "typeface", null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void ReadAllStyles(XmlNode node)
        {
            foreach (XmlNode xnum in node.ChildNodes) 
            {
                if (xnum.LocalName == "docDefaults") 
                {
                    foreach (XmlNode x in xnum.ChildNodes) 
                    {
                        if (x.LocalName == "rPrDefault") 
                        {
                            foreach (XmlNode xx in x.ChildNodes) 
                            {
                                if (xx.LocalName == "rPr") 
                                {
                                    DefStyle = new Pullenti.Unitext.UnitextStyle();
                                    this.ReadUnitextStyle(xx, DefStyle);
                                }
                            }
                        }
                    }
                }
                else if (xnum.LocalName == "style" && xnum.Attributes != null && xnum.Attributes.Count >= 1) 
                {
                    string id = _readAttrVal(xnum, "styleId", null);
                    if (id == null || m_TextStyles.ContainsKey(id)) 
                        continue;
                    DocTextStyle sty = new DocTextStyle();
                    if (DefStyle != null) 
                        sty.UStyle = DefStyle.Clone(false);
                    m_TextStyles.Add(id, sty);
                    foreach (XmlNode x in xnum.ChildNodes) 
                    {
                        if (x.LocalName == "name") 
                        {
                            if (x.Attributes != null && x.Attributes.Count > 0) 
                                sty.Name = x.Attributes[0].Value;
                        }
                        else if (x.LocalName == "aliases") 
                        {
                            if (x.Attributes != null && x.Attributes.Count > 0) 
                                sty.Aliases = x.Attributes[0].Value;
                        }
                        else if (x.LocalName == "basedOn") 
                        {
                            if (x.Attributes != null) 
                            {
                                foreach (XmlAttribute a in x.Attributes) 
                                {
                                    if (a.LocalName == "val" && m_TextStyles.ContainsKey(a.Value)) 
                                    {
                                        DocTextStyle st0 = m_TextStyles[a.Value];
                                        sty.NumId = st0.NumId;
                                        sty.NumLvl = st0.NumLvl;
                                        sty.UStyle = st0.UStyle.Clone(false);
                                    }
                                }
                            }
                        }
                        else if (x.LocalName == "rPr") 
                        {
                            this.ReadUnitextStyle(x, sty.UStyle);
                            if (_readAttrVal(xnum, "default", null) == "1") 
                            {
                                if (_readAttrVal(xnum, "type", null) == "paragraph") 
                                    DefStyle = sty.UStyle;
                            }
                        }
                        else if (x.LocalName == "pPr") 
                        {
                            this.ReadUnitextStyle(x, sty.UStyle);
                            foreach (XmlNode xx in x.ChildNodes) 
                            {
                                if (xx.LocalName == "numPr") 
                                {
                                    foreach (XmlNode xxx in xx.ChildNodes) 
                                    {
                                        if (xxx.LocalName == "numId" && xxx.Attributes != null) 
                                        {
                                            foreach (XmlAttribute a in xxx.Attributes) 
                                            {
                                                if (a.LocalName == "val") 
                                                    sty.NumId = a.Value;
                                            }
                                        }
                                        else if (xxx.LocalName == "ilvl" && xxx.Attributes != null) 
                                        {
                                            foreach (XmlAttribute a in xxx.Attributes) 
                                            {
                                                if (a.LocalName == "val") 
                                                {
                                                    int ll;
                                                    if (int.TryParse(a.Value, out ll)) 
                                                        sty.NumLvl = ll;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (sty.IsHeading) 
                        sty.UStyle.AddAttr("heading-level", sty.CalcHeadingLevel().ToString(), false);
                }
            }
        }
    }
}