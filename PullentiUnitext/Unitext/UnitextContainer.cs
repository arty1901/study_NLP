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

namespace Pullenti.Unitext
{
    /// <summary>
    /// Контейнер других элементов
    /// </summary>
    public class UnitextContainer : UnitextItem
    {
        static int _getBoxChars(UnitextItem it)
        {
            UnitextPlaintext pl = it as UnitextPlaintext;
            if (pl == null) 
                return 0;
            int co = 0;
            foreach (char ch in pl.Text) 
            {
                if (((int)ch) >= 0x2500 && ((int)ch) <= 0x257F) 
                    co++;
            }
            return co;
        }
        static int _getUnderlineChars(UnitextItem it)
        {
            UnitextPlaintext pl = it as UnitextPlaintext;
            if (pl == null) 
                return 0;
            int co = 0;
            foreach (char ch in pl.Text) 
            {
                if (ch == '_' || ch == '_') 
                    co++;
            }
            return co;
        }
        public override UnitextItem Optimize(bool isContainer, CreateDocumentParam pars)
        {
            if (Children == null || Children.Count == 0) 
                return null;
            for (int i = 0; i < Children.Count; i++) 
            {
                Children[i].Parent = this;
                UnitextItem ch = Children[i].Optimize(false, pars);
                if (ch == null) 
                {
                    Children.RemoveAt(i);
                    i--;
                }
                else 
                    Children[i] = ch;
            }
            if (Children.Count >= 30) 
            {
            }
            for (int i = Children.Count - 1; i >= 0; i--) 
            {
                UnitextContainer cnt = Children[i] as UnitextContainer;
                if (cnt == null) 
                    continue;
                if (i == 0) 
                {
                }
                if (cnt.Typ != UnitextContainerType.Undefined) 
                    continue;
                if (cnt.PageSectionId != null && PageSectionId != null && cnt.PageSectionId != PageSectionId) 
                    continue;
                Children.RemoveAt(i);
                Children.InsertRange(i, cnt.Children);
            }
            for (int i = 0; i < (Children.Count - 1); i++) 
            {
                UnitextItem ch = Children[i];
                UnitextItem ch1 = Children[i + 1];
                if ((ch is UnitextPagebreak) && (ch1 is UnitextPagebreak)) 
                {
                    Children.RemoveAt(i + 1);
                    i--;
                    continue;
                }
                if (ch is UnitextNewline) 
                {
                    if (ch1 is UnitextNewline) 
                    {
                        int ii;
                        for (ii = i + 1; ii < Children.Count; ii++) 
                        {
                            if (!(Children[ii] is UnitextNewline)) 
                                break;
                            else 
                                (ch as UnitextNewline).Count += (Children[ii] as UnitextNewline).Count;
                        }
                        Children.RemoveRange(i + 1, ii - i - 1);
                        i--;
                    }
                    else if (ch1 is UnitextPagebreak) 
                    {
                        Children.RemoveAt(i);
                        i--;
                    }
                    continue;
                }
                if ((ch is UnitextPlaintext) && ch.IsWhitespaces) 
                {
                    if ((ch1 is UnitextNewline) || (ch1 is UnitextPagebreak)) 
                    {
                        Children.RemoveAt(i);
                        i--;
                        if (i >= 0) 
                            i--;
                        continue;
                    }
                }
                if ((ch is UnitextContainer) && !ch.IsInline && (ch1 is UnitextNewline)) 
                {
                    (ch as UnitextContainer).Children.Add(ch1);
                    Children.RemoveAt(i + 1);
                    i--;
                    continue;
                }
                if ((ch is UnitextContainer) && (ch1 is UnitextContainer)) 
                {
                    UnitextContainer cnt = ch as UnitextContainer;
                    UnitextContainer cnt1 = ch1 as UnitextContainer;
                    if (cnt.Typ == cnt1.Typ && cnt.PageSectionId == cnt1.PageSectionId) 
                    {
                        cnt.Children.AddRange(cnt1.Children);
                        Children.RemoveAt(i + 1);
                        i--;
                        continue;
                    }
                }
                if ((ch is UnitextPlaintext) && (ch1 is UnitextPlaintext)) 
                {
                    if ((ch as UnitextPlaintext).Typ == (ch1 as UnitextPlaintext).Typ) 
                    {
                        (ch as UnitextPlaintext).MergeWith(ch1 as UnitextPlaintext);
                        Children.RemoveAt(i + 1);
                        i--;
                        continue;
                    }
                }
                if ((ch is UnitextTable) && (ch1 is UnitextTable)) 
                {
                    if ((ch as UnitextTable).TryAppend(ch1 as UnitextTable)) 
                    {
                        Children.RemoveAt(i + 1);
                        i--;
                        continue;
                    }
                }
                if (((((i + 2) < Children.Count) && (ch is UnitextTable) && (Children[i + 2] is UnitextTable)) && (ch1 is UnitextNewline) && ((ch1 as UnitextNewline).Count < 3)) && (ch as UnitextTable).TryAppend(Children[i + 2] as UnitextTable)) 
                {
                    Children.RemoveAt(i + 1);
                    Children.RemoveAt(i + 1);
                    i--;
                    continue;
                }
                if ((ch is UnitextPlaintext) && (((ch1 is UnitextNewline) || (ch1 is UnitextPagebreak) || (ch1 is UnitextFootnote)))) 
                    ch.Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp.TrimEnd, null);
            }
            for (int i = 0; i < (Children.Count - 1); i++) 
            {
                if ((Children[i] is UnitextDocblock) && (((Children[i + 1] is UnitextPagebreak) || (Children[i + 1] is UnitextNewline)))) 
                {
                    if ((Children[i] as UnitextDocblock)._appendChild(Children[i + 1])) 
                    {
                        Children.RemoveAt(i + 1);
                        Children[i].Optimize(false, pars);
                        i--;
                    }
                }
            }
            if (isContainer) 
            {
                for (int i = Children.Count - 1; i >= 0; i--) 
                {
                    if (Children[i].IsWhitespaces) 
                        Children.RemoveAt(i);
                    else 
                        break;
                }
                for (int i = 0; i < Children.Count; i++) 
                {
                    if (Children[i].IsWhitespaces) 
                    {
                        Children.RemoveAt(i);
                        i--;
                    }
                    else 
                        break;
                }
            }
            for (int i = 0; i < Children.Count; i++) 
            {
                int co = _getBoxChars(Children[i]);
                if (co == 0) 
                    continue;
                int i1 = i;
                int no = 1;
                int cou = 1;
                for (int j = i + 1; j < Children.Count; j++) 
                {
                    if (Children[j].IsWhitespaces) 
                        continue;
                    co = _getBoxChars(Children[j]);
                    if (co > 0) 
                    {
                        no = 1;
                        i1 = j;
                        cou++;
                        if (cou > 3) 
                            no++;
                        if (cou > 10) 
                            no++;
                    }
                    else 
                    {
                        co = _getUnderlineChars(Children[j]);
                        if (co > 10) 
                        {
                            no = 1;
                            i1 = j;
                            cou++;
                            if (cou > 3) 
                                no++;
                            if (cou > 10) 
                                no++;
                        }
                        if (co > 3) 
                            cou++;
                        else if ((--no) == 0) 
                            break;
                    }
                }
                for (; (i1 + 1) < Children.Count; i1++) 
                {
                    if (!Children[i1 + 1].IsWhitespaces && (_getUnderlineChars(Children[i1 + 1]) < 3)) 
                        break;
                }
                for (int j = i - 1; j >= 0; j--) 
                {
                    if (Children[j].IsWhitespaces) 
                        continue;
                    if (_getUnderlineChars(Children[j]) < 3) 
                        break;
                    i = j;
                }
                if (i == 0 && i1 == (Children.Count - 1) && Typ == UnitextContainerType.Undefined) 
                {
                    Typ = UnitextContainerType.Monospace;
                    break;
                }
                UnitextContainer cnt = new UnitextContainer() { Typ = UnitextContainerType.Monospace, Parent = this };
                for (int j = i; j <= i1; j++) 
                {
                    cnt.Children.Add(Children[j]);
                    Children[j].Parent = cnt;
                }
                Children.RemoveRange(i, (i1 - i) + 1);
                Children.Insert(i, cnt);
            }
            if (Children.Count == 0) 
                return null;
            if (Children.Count == 1) 
            {
                if (!IsInline && Children[0].IsInline) 
                    return this;
                if (Children[0] is UnitextContainer) 
                {
                    UnitextContainer ch = Children[0] as UnitextContainer;
                    if (ch.Typ == Typ) 
                    {
                        if (Children[0].PageSectionId == null) 
                            Children[0].PageSectionId = PageSectionId;
                        return Children[0];
                    }
                    if (Typ == UnitextContainerType.Head || Typ == UnitextContainerType.Tail) 
                    {
                        if (ch.Typ == UnitextContainerType.Undefined) 
                        {
                            if (PageSectionId == null) 
                                PageSectionId = Children[0].PageSectionId;
                            Children.Clear();
                            Children.AddRange(ch.Children);
                        }
                        return this;
                    }
                }
                if (Typ == UnitextContainerType.Undefined) 
                {
                    if (Children[0].PageSectionId == null) 
                        Children[0].PageSectionId = PageSectionId;
                    return Children[0];
                }
            }
            return this;
        }
        /// <summary>
        /// Внутренние элементы
        /// </summary>
        public List<UnitextItem> Children = new List<UnitextItem>();
        /// <summary>
        /// Специфический тип контейнера
        /// </summary>
        public UnitextContainerType Typ = UnitextContainerType.Undefined;
        /// <summary>
        /// Используйте произвольным образом, сериализуется
        /// </summary>
        public string Data;
        /// <summary>
        /// Используйте произвольным образом, несериализуется
        /// </summary>
        public object UserData;
        /// <summary>
        /// При генерации HTML если не null, то явлется дополнительным стилем вывода
        /// </summary>
        public string HtmlStyle;
        internal override string InnerTag
        {
            get
            {
                return "cnt";
            }
        }
        public override UnitextItem Clone()
        {
            UnitextContainer res = new UnitextContainer();
            res._cloneFrom2(this);
            return res;
        }
        void _cloneFrom2(UnitextContainer src)
        {
            this._cloneFrom(src);
            Typ = src.Typ;
            HtmlStyle = src.HtmlStyle;
            Data = src.Data;
            UserData = src.UserData;
            m_IsInline = src.m_IsInline;
            foreach (UnitextItem ch in src.Children) 
            {
                Children.Add(ch.Clone());
            }
        }
        public override UnitextItem FindById(string id)
        {
            if (Id == id) 
                return this;
            foreach (UnitextItem ch in Children) 
            {
                UnitextItem res = ch.FindById(id);
                if (res != null) 
                    return res;
            }
            return null;
        }
        /// <summary>
        /// Получить индекс дочернего элемента из списка Children (работает быстро)
        /// </summary>
        /// <param name="it">исколмый элемент</param>
        /// <return>индекс или -1</return>
        public int GetChildIndexOf(UnitextItem it)
        {
            if ((it.BeginChar > 0 && Children.Count > 20 && BeginChar <= it.BeginChar) && it.EndChar <= EndChar) 
            {
                int i = Children.Count / 2;
                int d = Children.Count / 4;
                if (d == 0) 
                    d = 1;
                int k = d + 2;
                while (k > 0) 
                {
                    if (i >= Children.Count || (i < 0)) 
                        break;
                    if (Children[i] == it) 
                        return i;
                    if (Children[i].BeginChar < it.BeginChar) 
                        i += d;
                    else if (Children[i].EndChar > it.BeginChar) 
                        i -= d;
                    else 
                        i += d;
                    d /= 2;
                    if (d == 0) 
                    {
                        d = 1;
                        k--;
                    }
                }
            }
            return Children.IndexOf(it);
        }
        public override string ToString()
        {
            string res = string.Format("{0} {1} items", (Typ == UnitextContainerType.Undefined ? "Container" : Typ.ToString()), (Children == null ? 0 : Children.Count));
            if (HtmlTitle != null) 
                res = string.Format("{0} {1}", res, HtmlTitle);
            return res;
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            if (Children == null || Children.Count == 0) 
            {
            }
            else if (Typ == UnitextContainerType.Shape && pars != null && pars.IgnoreShapes) 
            {
            }
            else 
            {
                if (Typ == UnitextContainerType.Shape && pars != null && res.Length > 0) 
                {
                    char ch = res[res.Length - 1];
                    if (char.IsLetterOrDigit(ch)) 
                        res.Append(pars.NewLine);
                }
                foreach (UnitextItem ch in Children) 
                {
                    ch.GetPlaintext(res, pars ?? UnitextItem.m_DefParams);
                    if (pars != null && pars.MaxTextLength > 0 && res.Length > pars.MaxTextLength) 
                        break;
                }
                if (Typ == UnitextContainerType.Shape && pars != null) 
                    res.Append(pars.NewLine);
            }
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (par.HideEditionsAndComments) 
            {
                if (Typ == UnitextContainerType.Edition || Typ == UnitextContainerType.Comment) 
                    return;
            }
            if (!par.CallBefore(this, res)) 
                return;
            string id = "";
            if (Id != null) 
                id = string.Format(" id=\"{0}\"", Id);
            if (par != null && par.OutBeginEndChars && BeginChar <= EndChar) 
                id = string.Format("{0} bc=\"{1}\" ec=\"{2}\"", id, BeginChar, EndChar);
            if (Children == null || Children.Count == 0) 
            {
                par.CallAfter(this, res);
                return;
            }
            bool isDiv = false;
            bool isSpan = false;
            bool isPre = false;
            string tit = (Typ == UnitextContainerType.Undefined ? "" : Typ.ToString().ToLower());
            if (HtmlTitle != null) 
            {
                StringBuilder tmp = new StringBuilder();
                Pullenti.Util.MiscHelper.CorrectHtmlValue(tmp, HtmlTitle, true, false);
                tit = tmp.ToString();
            }
            else 
                for (UnitextItem p = Parent; p != null; p = p.Parent) 
                {
                    if (p.HtmlTitle != null) 
                    {
                        tit = "";
                        break;
                    }
                }
            if (!string.IsNullOrEmpty(tit)) 
                tit = string.Format(" title=\"{0}\"", tit);
            string style = null;
            if (par != null && Id != null && par.Styles.ContainsKey(Id)) 
                style = par.Styles[Id];
            if (style == null) 
                style = HtmlStyle;
            if (style != null) 
            {
                isDiv = !IsInline;
                isSpan = !isDiv;
                if (isDiv) 
                    res.Append("\r\n");
                res.AppendFormat("<{2} style=\"{3}\" {1}{0}>", tit, id, (isDiv ? "div" : "span"), style);
            }
            else if (Typ == UnitextContainerType.Shape) 
            {
                res.AppendFormat("\r\n<div style=\"border-width:2pt;border-color:green;border-style:dotted;margin:10pt;padding:5pt\" {1}{0}>", tit, id);
                isDiv = true;
            }
            else if (Typ == UnitextContainerType.Monospace) 
            {
                res.AppendFormat("\r\n<div style=\"font-family:monospace\" {1}{0}>", tit, id);
                res.Append("<pre>");
                isDiv = true;
                isPre = true;
            }
            else if (Typ == UnitextContainerType.ContentControl) 
            {
                isDiv = !IsInline;
                isSpan = !isDiv;
                if (isDiv) 
                    res.Append("\r\n");
                res.AppendFormat("<{2} style=\"background-color:lightyellow;border:1pt solid black\" {1}{0}>", tit, id, (isDiv ? "div" : "span"));
            }
            else if (Typ == UnitextContainerType.RightAlign) 
            {
                res.AppendFormat("\r\n<div style=\"font-weight:normal;font-style:italic;text-align:right\" {1}{0}>", tit, id);
                isDiv = true;
            }
            else if (Typ == UnitextContainerType.Edition || Typ == UnitextContainerType.Comment) 
            {
                if (Typ == UnitextContainerType.Comment && IsInline) 
                    isSpan = true;
                else 
                {
                    isDiv = true;
                    res.Append("\r\n");
                }
                res.AppendFormat("<{0} {2} style=\"font-weight: normal;font-style: italic;{3}\"{1}>", (isSpan ? "span" : "div"), tit, id, "font-size: smaller;color: gray");
            }
            else if (((Typ == UnitextContainerType.Head || ((Typ == UnitextContainerType.Name && (Parent is UnitextDocblock))))) && !IsInline) 
            {
                string ali = "center";
                int marg = 10;
                UnitextDocblock db = Parent as UnitextDocblock;
                if (db != null && db.Typname != null) 
                {
                    if (((string.Compare(db.Typname, "Clause", true) == 0 || string.Compare(db.Typname, "Mail", true) == 0 || string.Compare(db.Typname, "Chapter", true) == 0) || string.Compare(db.Typname, "Paragraph", true) == 0 || string.Compare(db.Typname, "SubParagraph", true) == 0) || string.Compare(db.Typname, "Section", true) == 0 || string.Compare(db.Typname, "Subsection", true) == 0) 
                    {
                        ali = "left";
                        marg = 10;
                    }
                }
                res.AppendFormat("\r\n<div style=\"text-align:{2};margin-bottom:{3}pt\" {1}{0}>", tit, id, ali, marg);
                isDiv = true;
            }
            else if (Typ == UnitextContainerType.Directive) 
            {
                res.AppendFormat("\r\n<div style=\"text-align:center;text-decoration:underline\" {1}{0}>", tit, id);
                isDiv = true;
            }
            else if (Typ == UnitextContainerType.Tail) 
            {
                res.AppendFormat("\r\n<div style=\"text-align:right\" {1}{0}>", tit, id);
                isDiv = true;
            }
            else if (Typ == UnitextContainerType.Highlighting) 
            {
                res.AppendFormat("<span style=\"background-color:yellow\" {1}{0}>", tit, id);
                isSpan = true;
            }
            else if (Typ != UnitextContainerType.Undefined) 
            {
                if (Parent != null && (Parent.Parent is UnitextDocblock) && string.Compare((Parent.Parent as UnitextDocblock).Typname ?? "", "INDEXITEM", true) == 0) 
                {
                    res.AppendFormat("<span{0}>", id);
                    isSpan = true;
                }
                else 
                    res.AppendFormat("<b {1}{0}>", tit, id);
            }
            else if (((Parent is UnitextDocument) && par != null && par.OutStyles) && m_StyledFrag != null && m_StyledFrag.Style != null) 
            {
                res.AppendFormat("\r\n<div{0} style=\"", id);
                m_StyledFrag.Style.GetHtml(res);
                res.Append("\">");
                isDiv = true;
            }
            else if (!string.IsNullOrEmpty(id)) 
            {
                res.AppendFormat("<span{0}>", id);
                isSpan = true;
            }
            bool outStyles = false;
            if (par != null && par.OutStyles && !IsInline) 
            {
                if (this.GetStyledFragment(-1) != null) 
                    outStyles = true;
            }
            UnitextStyledFragment curStFr = null;
            for (int i = 0; i < Children.Count; i++) 
            {
                UnitextItem ch = Children[i];
                if (i == 29) 
                {
                }
                if (ch is UnitextNewline) 
                {
                    if ((ch as UnitextNewline).Count == 1) 
                    {
                        if (((i + 1) < Children.Count) && !Children[i + 1].IsInline) 
                            continue;
                        if ((i + 1) == Children.Count && Parent != null && !Parent.IsInline) 
                            continue;
                    }
                }
                if (outStyles) 
                {
                    UnitextStyledFragment fr = ch.GetStyledFragment(-1);
                    if (fr != null && fr.Typ == UnitextStyledFragmentType.Inline) 
                        fr = fr.Parent;
                    if (fr == null) 
                    {
                        if (curStFr != null) 
                        {
                            curStFr = null;
                            res.Append("</div>");
                        }
                    }
                    else if (fr != curStFr) 
                    {
                        if (curStFr != null) 
                            res.Append("</div>");
                        curStFr = null;
                        if (fr.StyleId > 0 && fr.Style != null) 
                        {
                            curStFr = fr;
                            res.Append("\r\n<div style=\"");
                            fr.Style.GetHtml(res);
                            res.Append("\">");
                        }
                    }
                }
                ch.GetHtml(res, par);
                if (res.Length > 20000000) 
                    break;
            }
            if (curStFr != null) 
                res.Append("</div>");
            if (isPre) 
                res.Append("</pre>");
            if (isDiv) 
            {
                res.Append("</div>\r\n");
                if (par != null) 
                    par._outFootnotes(res);
            }
            else if (isSpan) 
                res.Append("</span>");
            else if (Typ != UnitextContainerType.Undefined) 
                res.Append("</b>");
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("container");
            this._writeXmlAttrs(xml);
            if (Typ != UnitextContainerType.Undefined) 
                xml.WriteAttributeString("type", Typ.ToString().ToLower());
            if (Data != null) 
                xml.WriteAttributeString("data", Pullenti.Util.MiscHelper.CorrectXmlValue(Data));
            if (!string.IsNullOrEmpty(HtmlStyle)) 
                xml.WriteAttributeString("style", Pullenti.Util.MiscHelper.CorrectXmlValue(HtmlStyle));
            if (m_IsInline >= 0) 
                xml.WriteAttributeString("inline", (m_IsInline > 0 ? "true" : "false"));
            foreach (UnitextItem ch in Children) 
            {
                ch.GetXml(xml);
            }
            xml.WriteEndElement();
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "type") 
                    {
                        try 
                        {
                            Typ = (UnitextContainerType)Enum.Parse(typeof(UnitextContainerType), a.Value, true);
                        }
                        catch(Exception ex316) 
                        {
                        }
                    }
                    else if (a.LocalName == "data") 
                        Data = a.Value;
                    else if (a.LocalName == "style") 
                        HtmlStyle = a.Value;
                    else if (a.LocalName == "inline") 
                        m_IsInline = (a.Value == "true" ? 1 : 0);
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                UnitextItem it = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(x);
                if (it != null) 
                    Children.Add(it);
            }
        }
        public override bool IsWhitespaces
        {
            get
            {
                foreach (UnitextItem ch in Children) 
                {
                    if (!ch.IsWhitespaces) 
                        return false;
                }
                return true;
            }
        }
        public override bool IsInline
        {
            get
            {
                if (m_IsInline >= 0) 
                    return m_IsInline > 0;
                if (Children != null) 
                {
                    foreach (UnitextItem ch in Children) 
                    {
                        if (!ch.IsInline) 
                            return false;
                    }
                }
                if (Parent == null || (Parent is UnitextDocument)) 
                    return false;
                return true;
            }
            set
            {
                m_IsInline = (value ? 1 : 0);
            }
        }
        int m_IsInline = -1;
        public override void GetAllItems(List<UnitextItem> res, int lev)
        {
            if (lev > 20) 
            {
            }
            if (res != null) 
                res.Add(this);
            if (Children != null) 
            {
                foreach (UnitextItem ch in Children) 
                {
                    ch.Parent = this;
                    ch.GetAllItems(res, lev + 1);
                }
            }
        }
        internal override void AddPlainTextPos(int d)
        {
            base.AddPlainTextPos(d);
            foreach (UnitextItem ch in Children) 
            {
                ch.AddPlainTextPos(d);
            }
        }
        internal override void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
            foreach (UnitextItem ch in Children) 
            {
                ch.Correct(typ, data);
            }
        }
        internal override bool ReplaceChild(UnitextItem old, UnitextItem ne)
        {
            int i = this.GetChildIndexOf(old);
            if (i < 0) 
                return false;
            Children[i] = ne;
            ne.Parent = this;
            return true;
        }
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = cp;
            foreach (UnitextItem ch in Children) 
            {
                ch.SetDefaultTextPos(ref cp, res);
                ch.Parent = this;
            }
            EndChar = cp - 1;
        }
    }
}