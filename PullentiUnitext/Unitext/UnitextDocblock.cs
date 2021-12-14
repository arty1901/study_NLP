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
    /// Cтруктурирующий блок из заголовочной части, тела, окончания и приложений. 
    /// Выделяется только для некоторых форматов, если задать LoadDocumentStructure = true в параметрах создания. 
    /// Но этот элемент активно используется на других этапах анализа, когда структура документа восстанавливается 
    /// по плоскому тексту, а затем их иерархия оформляется этими элементами. Например, для нормативных актов 
    /// это главы, статьи, части, пункты и подпункты.
    /// </summary>
    public class UnitextDocblock : UnitextItem
    {
        /// <summary>
        /// Заголовочная часть
        /// </summary>
        public UnitextContainer Head;
        /// <summary>
        /// Содержимое
        /// </summary>
        public UnitextItem Body;
        /// <summary>
        /// Подписная часть
        /// </summary>
        public UnitextContainer Tail;
        /// <summary>
        /// Приложения
        /// </summary>
        public UnitextItem Appendix;
        /// <summary>
        /// Тип блока (глава, статья, приложение и т.п. - если только ключевое слово явно указано)
        /// </summary>
        public string Typname;
        /// <summary>
        /// Выделенный номер (если есть) - относится к Head
        /// </summary>
        public string Number;
        /// <summary>
        /// Класс блока
        /// </summary>
        public UnitextDocblockType Typ = UnitextDocblockType.Undefined;
        /// <summary>
        /// Признак того, что фрагмент НПА утратил силу
        /// </summary>
        public bool Expired = false;
        /// <summary>
        /// Идентификатор внешнего блока, благодаря которому создан этот блок
        /// </summary>
        public string ExtBlockId;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Typname != null) 
                res.AppendFormat("{0} ", Typname);
            if (Number != null) 
                res.AppendFormat("{0} ", Number);
            if (Typ != UnitextDocblockType.Undefined) 
                res.AppendFormat("Typ:{0} ", Typ.ToString());
            return res.ToString().Trim();
        }
        public override UnitextItem Clone()
        {
            UnitextDocblock res = new UnitextDocblock();
            res._cloneFrom(this);
            if (Head != null) 
                res.Head = Head.Clone() as UnitextContainer;
            if (Body != null) 
                res.Body = Body.Clone();
            if (Tail != null) 
                res.Tail = Tail.Clone() as UnitextContainer;
            if (Appendix != null) 
                res.Appendix = Appendix.Clone();
            res.ExtBlockId = ExtBlockId;
            res.Typ = Typ;
            res.Typname = Typname;
            res.Number = Number;
            res.Expired = Expired;
            return res;
        }
        public override bool IsInline
        {
            get
            {
                return m_IsInline;
            }
            set
            {
                m_IsInline = value;
            }
        }
        bool m_IsInline = false;
        internal override string InnerTag
        {
            get
            {
                return "docblk";
            }
        }
        public override UnitextItem FindById(string id)
        {
            if (Id == id) 
                return this;
            UnitextItem res;
            if (Head != null) 
            {
                if ((((res = Head.FindById(id)))) != null) 
                    return res;
            }
            if (Tail != null) 
            {
                if ((((res = Tail.FindById(id)))) != null) 
                    return res;
            }
            if (Body != null) 
            {
                if ((((res = Body.FindById(id)))) != null) 
                    return res;
            }
            if (Appendix != null) 
            {
                if ((((res = Appendix.FindById(id)))) != null) 
                    return res;
            }
            return null;
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            if (Head != null) 
                Head.GetPlaintext(res, pars);
            if (Body != null) 
                Body.GetPlaintext(res, pars);
            if (Tail != null) 
                Tail.GetPlaintext(res, pars);
            if (Appendix != null) 
                Appendix.GetPlaintext(res, pars);
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            if (string.Compare(Typname ?? "", "Footnote", true) == 0) 
                return;
            string tag = "div";
            if (IsInline) 
                tag = "span";
            else if (string.Compare(Typname ?? "", "Indention", true) == 0 && (Parent is UnitextContainer) && (Parent as UnitextContainer).Children[0] == this) 
            {
                UnitextDocblock blk = Parent.Parent as UnitextDocblock;
                if (blk != null && blk.Head != null && blk.Head.IsInline) 
                    tag = "span";
            }
            if (string.Compare(Typname ?? "", "Part", true) == 0 && Head == null && Number != "1") 
                res.Append("\r\n<BR/>");
            res.AppendFormat("\r\n<{0}", tag);
            if (HtmlTitle != null) 
            {
                res.Append(" title=\"");
                Pullenti.Util.MiscHelper.CorrectHtmlValue(res, HtmlTitle, true, false);
                res.Append("\"");
            }
            int margleft = 0;
            if (Typname == "ITEM" || Typname == "SUBITEM") 
            {
                for (UnitextItem p = Parent; p != null; p = p.Parent) 
                {
                    if (p is UnitextDocblock) 
                    {
                        UnitextDocblock pb = p as UnitextDocblock;
                        if (pb.Typname == "ITEM" || pb.Typname == "SUBITEM" || pb.Typname == "CLAUSEPART") 
                            margleft += 10;
                    }
                }
            }
            if (string.Compare(Typname ?? "", "Footnote", true) == 0) 
                res.AppendFormat(" style=\"margin-left:{0}pt;margin-top:10pt;margin-bottom:10pt;font-size:smaller;text-align:left;font-weight:normal\"", 30 + margleft);
            else if (string.Compare(Typname ?? "", "Index", true) == 0) 
                res.Append(" style=\"border:1pt solid black; background:lightgray; text-align:left;font-weight:normal;font-style:italic\"");
            else if (Typname != null) 
            {
                int margtop = 0;
                if (string.Compare(Typname, "Chapter", true) == 0 || string.Compare(Typname, "Section", true) == 0) 
                    margtop = 20;
                else if (string.Compare(Typname, "Clause", true) == 0 || string.Compare(Typname, "Subsection", true) == 0) 
                    margtop = 10;
                if (margtop > 0 || Expired || margleft > 0) 
                {
                    res.AppendFormat(" style=\"");
                    if (margtop > 0) 
                        res.AppendFormat("margin-top:{0}pt;", margtop);
                    if (margleft > 0) 
                        res.AppendFormat("margin-left:{0}pt;", margleft);
                    if (Expired) 
                        res.AppendFormat("background-color:lightgray;");
                    res.Append("\"");
                }
            }
            if (Id != null) 
                res.AppendFormat(" id=\"{0}\"", Id);
            res.Append(">");
            if (Head != null) 
            {
                if (string.Compare(Typname ?? "", "Appendix", true) == 0) 
                {
                    int i = 0;
                    for (i = 0; i < Head.Children.Count; i++) 
                    {
                        if ((Head.Children[i] is UnitextContainer) && (Head.Children[i] as UnitextContainer).Typ == UnitextContainerType.Name) 
                            break;
                    }
                    if (i > 0) 
                    {
                        res.Append("\r\n<div style=\"text-align:right;font-style:italic\">");
                        for (int j = 0; j < i; j++) 
                        {
                            Head.Children[j].GetHtml(res, par);
                        }
                        res.Append("</div>");
                    }
                    if (i < Head.Children.Count) 
                    {
                        res.Append("\r\n<div style=\"text-align:center;font-weight:bold;font-style:normal\">");
                        for (; i < Head.Children.Count; i++) 
                        {
                            Head.Children[i].GetHtml(res, par);
                        }
                        res.Append("</div>");
                    }
                    res.Append("<div style=\"margin-bottom:20pt\" />");
                }
                else 
                {
                    if (string.Compare(Typname ?? "", "Mail", true) != 0) 
                        res.Append("<b>");
                    else 
                        res.Append("<i>");
                    Head.GetHtml(res, par);
                    if (string.Compare(Typname ?? "", "Mail", true) != 0) 
                        res.Append("</b>");
                    else 
                        res.Append("</i>");
                }
                if (par != null) 
                    par._outFootnotes(res);
            }
            if (Body != null) 
            {
                Body.GetHtml(res, par);
                if (par != null) 
                    par._outFootnotes(res);
            }
            if (Tail != null) 
            {
                res.Append("<i>");
                Tail.GetHtml(res, par);
                res.Append("</i>");
                if (par != null) 
                    par._outFootnotes(res);
            }
            if (Appendix != null) 
            {
                Appendix.GetHtml(res, par);
                if (par != null) 
                    par._outFootnotes(res);
            }
            res.AppendFormat("</{0}>", tag);
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("docblock");
            this._writeXmlAttrs(xml);
            if (Typname != null) 
                xml.WriteAttributeString("typname", Typname);
            if (Number != null) 
                xml.WriteAttributeString("num", Pullenti.Util.MiscHelper.CorrectXmlValue(Number));
            if (Typ != UnitextDocblockType.Undefined) 
                xml.WriteAttributeString("typ", Typ.ToString().ToLower());
            if (m_IsInline) 
                xml.WriteAttributeString("inline", "true");
            if (ExtBlockId != null) 
                xml.WriteAttributeString("extblockid", ExtBlockId);
            if (Expired) 
                xml.WriteAttributeString("expired", "true");
            if (Head != null) 
            {
                xml.WriteStartElement("head");
                Head.GetXml(xml);
                xml.WriteEndElement();
            }
            if (Body != null) 
            {
                xml.WriteStartElement("body");
                Body.GetXml(xml);
                xml.WriteEndElement();
            }
            if (Tail != null) 
            {
                xml.WriteStartElement("tail");
                Tail.GetXml(xml);
                xml.WriteEndElement();
            }
            if (Appendix != null) 
            {
                xml.WriteStartElement("appendix");
                Appendix.GetXml(xml);
                xml.WriteEndElement();
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
                    if (a.LocalName == "typname") 
                        Typname = a.Value;
                    else if (a.LocalName == "num") 
                        Number = a.Value;
                    else if (a.LocalName == "extblockid") 
                        ExtBlockId = a.Value;
                    else if (a.LocalName == "expired") 
                        Expired = a.Value == "true";
                    else if (a.LocalName == "inline") 
                        m_IsInline = a.Value == "true";
                    else if (a.LocalName == "typ") 
                    {
                        try 
                        {
                            Typ = (UnitextDocblockType)Enum.Parse(typeof(UnitextDocblockType), a.Value, true);
                        }
                        catch(Exception ex317) 
                        {
                        }
                    }
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "head") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        Head = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx) as UnitextContainer;
                        if (Head != null) 
                            Head.Typ = UnitextContainerType.Head;
                        break;
                    }
                }
                else if (x.LocalName == "body") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        Body = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx);
                        break;
                    }
                }
                else if (x.LocalName == "appendix") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        Appendix = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx);
                        break;
                    }
                }
                else if (x.LocalName == "tail") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        Tail = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx) as UnitextContainer;
                        if (Tail != null) 
                            Tail.Typ = UnitextContainerType.Tail;
                        break;
                    }
                }
            }
        }
        public override bool IsWhitespaces
        {
            get
            {
                return false;
            }
        }
        public override void GetAllItems(List<UnitextItem> res, int lev)
        {
            if (res != null) 
                res.Add(this);
            if (Head != null) 
            {
                if (Head.Children.Contains(Head)) 
                {
                }
                else if (Head.Children.Contains(this)) 
                {
                }
                else 
                {
                    Head.Parent = this;
                    Head.GetAllItems(res, lev + 1);
                }
            }
            if (Body != null) 
            {
                Body.Parent = this;
                Body.GetAllItems(res, lev + 1);
            }
            if (Tail != null) 
            {
                Tail.Parent = this;
                Tail.GetAllItems(res, lev + 1);
            }
            if (Appendix != null) 
            {
                Appendix.Parent = this;
                Appendix.GetAllItems(res, lev + 1);
            }
        }
        internal override void AddPlainTextPos(int d)
        {
            base.AddPlainTextPos(d);
            if (Head != null) 
                Head.AddPlainTextPos(d);
            if (Body != null) 
                Body.AddPlainTextPos(d);
            if (Tail != null) 
                Tail.AddPlainTextPos(d);
            if (Appendix != null) 
                Appendix.AddPlainTextPos(d);
        }
        internal override void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
            if (Head != null) 
                Head.Correct(typ, data);
            if (Body != null) 
                Body.Correct(typ, data);
            if (Tail != null) 
                Tail.Correct(typ, data);
            if (Appendix != null) 
                Appendix.Correct(typ, data);
        }
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = cp;
            if (Head != null) 
            {
                Head.SetDefaultTextPos(ref cp, res);
                Head.Parent = this;
            }
            if (Body != null) 
            {
                Body.SetDefaultTextPos(ref cp, res);
                Body.Parent = this;
            }
            if (Tail != null) 
            {
                Tail.SetDefaultTextPos(ref cp, res);
                Tail.Parent = this;
            }
            if (Appendix != null) 
            {
                Appendix.SetDefaultTextPos(ref cp, res);
                Appendix.Parent = this;
            }
            EndChar = cp - 1;
        }
        public override UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            if (Head != null) 
            {
                if (Head.Children.Contains(Head)) 
                {
                }
                Head = Head.Optimize(false, pars) as UnitextContainer;
            }
            if (Body != null) 
                Body = Body.Optimize(true, pars);
            if (Tail != null) 
                Tail = Tail.Optimize(false, pars) as UnitextContainer;
            if (Appendix != null) 
                Appendix = Appendix.Optimize(true, pars);
            return this;
        }
        internal bool _appendChild(UnitextItem it)
        {
            if (Appendix != null) 
            {
                UnitextContainer cnt = Appendix as UnitextContainer;
                if (cnt == null) 
                {
                    cnt = new UnitextContainer() { BeginChar = Body.BeginChar };
                    cnt.Children.Add(Body);
                    Appendix = cnt;
                }
                cnt.Children.Add(it);
                EndChar = (Body.EndChar = it.EndChar);
                return true;
            }
            if (Tail != null) 
            {
                Tail.Children.Add(it);
                EndChar = (Tail.EndChar = it.EndChar);
                return true;
            }
            if (Body != null) 
            {
                UnitextContainer cnt = Body as UnitextContainer;
                if (cnt == null) 
                {
                    cnt = new UnitextContainer() { BeginChar = Body.BeginChar };
                    cnt.Children.Add(Body);
                    Body = cnt;
                }
                cnt.Children.Add(it);
                EndChar = (Body.EndChar = it.EndChar);
                return true;
            }
            if (Head != null) 
            {
                Head.Children.Add(it);
                EndChar = (Head.EndChar = it.EndChar);
                return true;
            }
            return false;
        }
    }
}