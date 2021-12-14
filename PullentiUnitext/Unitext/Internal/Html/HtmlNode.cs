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

namespace Pullenti.Unitext.Internal.Html
{
    public class HtmlNode
    {
        public string TagName;
        public Dictionary<string, string> Attrs;
        public string Text;
        public bool WhitespacePreserve;
        public HtmlNode Parent;
        public List<HtmlNode> Children = new List<HtmlNode>();
        public int SourceHtmlPosition;
        public int SourceHtmlLength;
        public int SourceHtmlEndPosition;
        public object Misc;
        public int TotalChildren
        {
            get
            {
                int res = Children.Count;
                foreach (HtmlNode ch in Children) 
                {
                    res += ch.TotalChildren;
                }
                return res;
            }
        }
        public void GetHtml(StringBuilder res)
        {
            HtmlTag tag = HtmlTag.FindTag(TagName);
            if (tag != null && ((tag.IsBlock || tag.IsTable)) && res.Length > 0) 
                res.Append("\r\n");
            if (TagName != null) 
            {
                res.AppendFormat("<{0}", TagName.ToLower());
                if (Attrs != null) 
                {
                    foreach (KeyValuePair<string, string> kp in Attrs) 
                    {
                        res.AppendFormat(" {0}=\"", kp.Key.ToLower());
                        Pullenti.Util.MiscHelper.CorrectHtmlValue(res, kp.Value, true, false);
                        res.Append("\"");
                    }
                }
                if (string.IsNullOrEmpty(Text) && Children.Count == 0) 
                {
                    res.Append("/>");
                    return;
                }
                res.Append(">");
            }
            if (Children.Count > 0) 
            {
                foreach (HtmlNode ch in Children) 
                {
                    ch.GetHtml(res);
                }
            }
            else if (!string.IsNullOrEmpty(Text)) 
                Pullenti.Util.MiscHelper.CorrectHtmlValue(res, Text, false, false);
            if (TagName != null) 
                res.AppendFormat("</{0}>", TagName.ToLower());
        }
        public int PlainTextBegin;
        public int PlainTextEnd;
        public string GetPlaintext(Pullenti.Unitext.GetPlaintextParam pars)
        {
            StringBuilder res = new StringBuilder();
            this._getFullText(res, pars, null);
            return res.ToString();
        }
        internal void _getFullText(StringBuilder res, Pullenti.Unitext.GetPlaintextParam pars, string liNumStr = null)
        {
            PlainTextBegin = res.Length;
            PlainTextEnd = PlainTextBegin - 1;
            if ((TagName == "SCRIPT" || TagName == "STYLE" || TagName == "META") || TagName == "TITLE") 
                return;
            HtmlTag tag = HtmlTag.FindTag(TagName);
            bool isBlock = false;
            if (tag != null && ((tag.IsBlock || tag.IsTable))) 
                isBlock = true;
            bool isTab = tag != null && tag.Name == "TABLE";
            bool isTd = tag != null && ((tag.Name == "TD" || tag.Name == "TH"));
            bool isTr = tag != null && tag.Name == "TR";
            bool isLi = tag != null && tag.Name == "LI";
            bool isOl = tag != null && tag.Name == "OL";
            if (tag != null && tag.Name == "BR") 
            {
                if (WhitespacePreserve) 
                {
                    res.Append(' ');
                    PlainTextEnd = res.Length - 1;
                    return;
                }
                res.Append((pars == null ? "\r\n" : pars.NewLine));
                PlainTextEnd = res.Length - 1;
                return;
            }
            if (isTab && pars != null) 
            {
                if (pars.TableStart != null) 
                    res.Append(pars.TableStart);
            }
            if ((isBlock && !isTd && !isTr) && res.Length > 0) 
                res.Append((pars == null ? "\r\n" : pars.NewLine));
            if (isLi) 
            {
                if (liNumStr != null) 
                    res.Append(liNumStr);
                else 
                    res.Append('');
            }
            int liNum = 0;
            string liTyp = null;
            if (isOl) 
            {
                if (!int.TryParse(this.GetAttribute("START") ?? "", out liNum)) 
                    liNum = 1;
                liTyp = this.GetAttribute("TYPE") ?? "1";
            }
            if (!string.IsNullOrEmpty(Text)) 
                res.Append(Text);
            else if (Children.Count == 1) 
                (Children[0] as HtmlNode)._getFullText(res, pars, _getLiNum(liNum, liTyp));
            else 
            {
                bool first = true;
                for (int i = 0; i < Children.Count; i++) 
                {
                    HtmlNode ch = Children[i];
                    HtmlNode hn = ch as HtmlNode;
                    if (first) 
                        first = false;
                    else if (!ch.WhitespacePreserve && !isTr) 
                    {
                        if (ch.TagName == null || string.Compare(ch.TagName ?? "", "SPAN", true) == 0) 
                        {
                        }
                        else if (i > 0 && ((Children[i - 1].TagName == null || string.Compare(Children[i - 1].TagName ?? "", "SPAN", true) == 0))) 
                        {
                        }
                        else 
                            res.Append(' ');
                    }
                    hn._getFullText(res, pars, _getLiNum(liNum, liTyp));
                    if (liNum > 0 && hn.TagName == "LI") 
                        liNum++;
                }
            }
            if (isTd) 
            {
                if (this.GetAttribute("ROWSPAN") != null) 
                {
                    int span;
                    if (int.TryParse(this.GetAttribute("ROWSPAN"), out span)) 
                    {
                        for (; span > 1; span--) 
                        {
                            res.Append(pars.PageBreak);
                        }
                    }
                }
                if (this.GetAttribute("COLSPAN") != null) 
                {
                    int span;
                    if (int.TryParse(this.GetAttribute("COLSPAN"), out span)) 
                    {
                        for (; span > 1; span--) 
                        {
                            res.Append(pars.Tab);
                        }
                    }
                }
                res.Append(pars.TableCellEnd);
            }
            else if (isTr) 
                res.Append(pars.TableRowEnd ?? pars.NewLine);
            else if (isTab) 
            {
            }
            if (isTab && pars != null) 
            {
                if (pars.TableEnd != null) 
                    res.Append(pars.TableEnd);
            }
            PlainTextEnd = res.Length - 1;
        }
        static string _trimTabText(string txt)
        {
            if (string.IsNullOrEmpty(txt)) 
                return "";
            int i;
            for (i = txt.Length - 1; i >= 0; i--) 
            {
                if (txt[i] == ' ' || txt[i] == 0xD || txt[i] == 0xA) 
                {
                }
                else 
                    break;
            }
            if (i < 0) 
                return "";
            if (i < (txt.Length - 1)) 
                txt = txt.Substring(0, i + 1);
            for (i = 0; i < txt.Length; i++) 
            {
                if (txt[i] == ' ' || txt[i] == 0xD || txt[i] == 0xA) 
                {
                }
                else 
                    break;
            }
            if (i > 0) 
                txt = txt.Substring(i);
            return txt;
        }
        internal static string _getLiNum(int num, string typ)
        {
            if (num <= 0) 
                return null;
            char ty = (string.IsNullOrEmpty(typ) ? '1' : typ[0]);
            if (ty == '1') 
                return string.Format("{0}. ", num);
            if (ty == 'A') 
                return string.Format("{0}) ", (char)(('A' + ((num - 1)))));
            if (ty == 'a') 
                return string.Format("{0}) ", (char)(('a' + ((num - 1)))));
            if (ty == 'i' && ((num - 1) < Pullenti.Unitext.Internal.Uni.UnitextHelper.m_Romans.Length)) 
                return string.Format("{0}. ", Pullenti.Unitext.Internal.Uni.UnitextHelper.m_Romans[num - 1].ToLower());
            if (ty == 'I' && ((num - 1) < Pullenti.Unitext.Internal.Uni.UnitextHelper.m_Romans.Length)) 
                return string.Format("{0}. ", Pullenti.Unitext.Internal.Uni.UnitextHelper.m_Romans[num - 1]);
            return string.Format("{0}. ", num);
        }
        internal void CorrectRusTexts(int corr = 0)
        {
            string attr = this.GetAttribute("STYLE");
            if (attr != null) 
            {
                if (attr.Contains("small-caps")) 
                    corr = 1;
            }
            foreach (HtmlNode ch in Children) 
            {
                ch.CorrectRusTexts(corr);
            }
            if (corr > 0 && !string.IsNullOrEmpty(Text)) 
                Text = Text.ToUpper();
        }
        public string GetAttribute(string name)
        {
            if (Attrs == null) 
                return null;
            if (Attrs.ContainsKey(name)) 
                return Attrs[name];
            return null;
        }
        public string GetStyleValue(string name)
        {
            if (Attrs == null || name == null) 
                return null;
            string val;
            if (!Attrs.TryGetValue("STYLE", out val)) 
                return null;
            foreach (string vv in val.Split(';')) 
            {
                string vvv = vv.Trim();
                int ii = vvv.IndexOf(':');
                if (ii < 0) 
                    continue;
                if (string.Compare(name, vvv.Substring(0, ii).Trim(), true) == 0) 
                    return vv.Substring(ii + 1).Trim();
            }
            return null;
        }
        public override string ToString()
        {
            if (TagName == null) 
            {
                if (Text == null) 
                    return "";
                return Text;
            }
            StringBuilder tmp = new StringBuilder();
            tmp.AppendFormat("<{0}", TagName);
            if (Attrs != null) 
            {
                foreach (KeyValuePair<string, string> kp in Attrs) 
                {
                    tmp.AppendFormat(" {0}='{1}'", kp.Key, kp.Value ?? "");
                }
            }
            if (Text != null) 
                tmp.AppendFormat(">{0}</{1}>", Text, TagName);
            else if (Children.Count > 0) 
                tmp.AppendFormat(">...</{0}>", TagName);
            else 
                tmp.Append("/>");
            return tmp.ToString();
        }
        public bool HasText
        {
            get
            {
                if (Text != null) 
                {
                    for (int i = 0; i < Text.Length; i++) 
                    {
                        if (!char.IsWhiteSpace(Text[i])) 
                            return true;
                    }
                }
                for (int j = 0; j < Children.Count; j++) 
                {
                    if ((Children[j] as HtmlNode).HasText) 
                        return true;
                }
                return false;
            }
        }
        public bool HasTag(string tag)
        {
            if (TagName != null && string.Compare(TagName, tag, true) == 0) 
                return true;
            foreach (HtmlNode ch in Children) 
            {
                if (ch.HasTag(tag)) 
                    return true;
            }
            return false;
        }
        public bool HasBlockTag()
        {
            if (TagName != null) 
            {
                HtmlTag ti = HtmlTag.FindTag(TagName);
                if (ti != null && ti.IsBlock) 
                    return true;
            }
            foreach (HtmlNode ch in Children) 
            {
                if (ch.HasBlockTag()) 
                    return true;
            }
            return false;
        }
        public void Remove()
        {
            if (Parent == null) 
                return;
            int i = Parent.Children.IndexOf(this);
            if (i < 0) 
                return;
            Parent.Children.Remove(this);
            if (Children.Count > 0) 
            {
                Parent.Children.InsertRange(i, Children);
                Children.Clear();
            }
            Parent = null;
        }
        public HtmlNode FindSubnode(string tagName, string attrName, string attrValue)
        {
            if (attrName != null) 
                attrName = attrName.ToUpper();
            if (tagName != null) 
                tagName = tagName.ToUpper();
            return this._FindSubnode(tagName, attrName, attrValue);
        }
        HtmlNode _FindSubnode(string tagName, string attrName, string attrValue)
        {
            if (tagName != null && TagName == tagName) 
            {
                if (attrName != null) 
                {
                    string val = this.GetAttribute(attrName);
                    if (val != null) 
                    {
                        if (attrValue == null) 
                            return this;
                        if (string.Compare(attrValue, val, true) == 0) 
                            return this;
                    }
                }
                else 
                    return this;
            }
            foreach (HtmlNode ch in Children) 
            {
                HtmlNode res = ch._FindSubnode(tagName, attrName, attrValue);
                if (res != null) 
                    return res;
            }
            return null;
        }
        public HtmlNode ImplantateNode(int beginChar, int endChar, string tagName)
        {
            if (beginChar >= PlainTextEnd || (endChar < PlainTextBegin)) 
                return null;
            int ind = -1;
            if (Parent != null) 
                ind = Parent.Children.IndexOf(this);
            if (Children.Count == 0) 
            {
                if (string.IsNullOrEmpty(Text)) 
                    return null;
                if (beginChar < PlainTextBegin) 
                    beginChar = PlainTextBegin;
                if (endChar > PlainTextEnd) 
                    endChar = PlainTextEnd;
                if (beginChar == PlainTextBegin && endChar == PlainTextEnd) 
                {
                    HtmlNode res = new HtmlNode() { TagName = tagName, PlainTextBegin = beginChar, PlainTextEnd = endChar };
                    if (ind >= 0) 
                    {
                        Parent.Children[ind] = res;
                        res.Parent = Parent;
                        res.Children.Add(this);
                        this.Parent = res;
                        return res;
                    }
                    else 
                    {
                        HtmlNode txt = new HtmlNode() { PlainTextBegin = beginChar, PlainTextEnd = endChar };
                        txt.Text = Text;
                        Text = null;
                        res.Children.Add(txt);
                        txt.Parent = res;
                        Children.Add(res);
                        res.Parent = this;
                        return res;
                    }
                }
                HtmlNode res1 = new HtmlNode() { TagName = tagName, PlainTextBegin = beginChar, PlainTextEnd = endChar };
                if (beginChar > PlainTextBegin) 
                {
                    int d = beginChar - PlainTextBegin;
                    HtmlNode txt = new HtmlNode() { PlainTextEnd = beginChar - 1 };
                    txt.PlainTextBegin = PlainTextBegin;
                    txt.Text = Text.Substring(0, d);
                    Children.Add(txt);
                    txt.Parent = this;
                    Text = Text.Substring(d);
                }
                if (beginChar <= endChar) 
                {
                    HtmlNode txt = new HtmlNode() { PlainTextBegin = beginChar, PlainTextEnd = endChar };
                    txt.Text = Text;
                    if (endChar < PlainTextEnd) 
                    {
                        txt.Text = Text.Substring(0, (endChar - beginChar) + 1);
                        Text = Text.Substring((endChar - beginChar) + 1);
                    }
                    else 
                        Text = "";
                    Children.Add(res1);
                    res1.Parent = this;
                    res1.Children.Add(txt);
                    txt.Parent = res1;
                }
                if (endChar < PlainTextEnd) 
                {
                    HtmlNode txt = new HtmlNode() { PlainTextBegin = endChar + 1 };
                    txt.PlainTextEnd = PlainTextEnd;
                    txt.Text = Text;
                    Children.Add(txt);
                    txt.Parent = this;
                    Text = "";
                }
                Text = null;
                return res1;
            }
            if (Children.Count == 0) 
                return null;
            for (int i = 0; i < Children.Count; i++) 
            {
                if (Children[i].PlainTextBegin <= beginChar && beginChar <= Children[i].PlainTextEnd) 
                {
                    for (int j = i; j < Children.Count; j++) 
                    {
                        string tag = Children[j].TagName;
                        if ((tag == "TD" || tag == "TR" || tag == "TH") || ((Children[j].PlainTextBegin <= endChar && endChar <= Children[j].PlainTextEnd))) 
                        {
                            if (i == j) 
                            {
                                HtmlNode res1 = Children[i].ImplantateNode(beginChar, endChar, tagName);
                                if (res1 != null) 
                                    return res1;
                            }
                            HtmlNode head = null;
                            HtmlNode tail = null;
                            HtmlNode ch = Children[i];
                            if (beginChar > ch.PlainTextBegin && ch.Text != null) 
                            {
                                head = new HtmlNode() { TagName = ch.TagName, PlainTextBegin = ch.PlainTextBegin, PlainTextEnd = beginChar - 1 };
                                head.Parent = Parent;
                                head.Text = ch.Text.Substring(0, beginChar - ch.PlainTextBegin);
                                ch.Text = ch.Text.Substring(beginChar - ch.PlainTextBegin);
                                ch.PlainTextBegin = beginChar;
                            }
                            if ((endChar < Children[j].PlainTextEnd) && Children[j].Text != null) 
                            {
                                HtmlNode ch1 = Children[j];
                                tail = new HtmlNode() { TagName = ch1.TagName, PlainTextBegin = endChar + 1, PlainTextEnd = ch1.PlainTextEnd };
                                tail.Parent = Parent;
                                tail.Text = ch1.Text.Substring(endChar - ch1.PlainTextBegin);
                                ch1.Text = ch1.Text.Substring(0, endChar - ch1.PlainTextBegin);
                                ch1.PlainTextEnd = endChar;
                            }
                            beginChar = ch.PlainTextBegin;
                            endChar = Children[j].PlainTextEnd;
                            HtmlNode res = new HtmlNode() { TagName = tagName, PlainTextBegin = beginChar, PlainTextEnd = endChar };
                            if (((tag == "TD" || tag == "TR" || tag == "TH")) && i == j) 
                            {
                                foreach (HtmlNode chh in ch.Children) 
                                {
                                    res.Children.Add(chh);
                                    ch.Parent = res;
                                }
                                Children[i].Children.Clear();
                                Children[i].Children.Add(res);
                                res.Parent = Children[i];
                                return res;
                            }
                            for (int k = i; k <= j; k++) 
                            {
                                res.Children.Add(Children[k]);
                                Children[k].Parent = res;
                            }
                            Children.RemoveRange(i, (j + 1) - i);
                            if (tail != null) 
                                Children.Insert(i, tail);
                            Children.Insert(i, res);
                            res.Parent = this;
                            if (head != null) 
                                Children.Insert(i, head);
                            return res;
                        }
                    }
                }
            }
            return null;
        }
    }
}