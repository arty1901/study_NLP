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
    /// Сноска
    /// </summary>
    public class UnitextFootnote : UnitextItem
    {
        /// <summary>
        /// Содержимое сноски
        /// </summary>
        public UnitextItem Content;
        /// <summary>
        /// Признак концевой сноски
        /// </summary>
        public bool IsEndnote;
        /// <summary>
        /// Специфическая комбинация для сноски
        /// </summary>
        public string CustomMark = null;
        /// <summary>
        /// Ссылка на структурный элемент с содержимым текста сноски. 
        /// А текущея сноска - это просто метка в тексте на реальную сноску, которая расположена в другом месте.
        /// </summary>
        public string DocBlockId = null;
        public override string ToString()
        {
            if (DocBlockId != null) 
            {
                string ttt = ((HtmlTitle ?? "")).Replace('\r', ' ').Replace('\n', ' ');
                if (ttt.Length > 100) 
                    ttt = ttt.Substring(0, 100) + "...";
                return string.Format("<{0}> -> {1}", CustomMark ?? "", ttt);
            }
            return string.Format("{0}: {1}", (IsEndnote ? "Endnote" : "Footnote"), (Content == null ? "" : Content.ToString()));
        }
        public override UnitextItem Clone()
        {
            UnitextFootnote res = new UnitextFootnote();
            res._cloneFrom(this);
            if (Content != null) 
                res.Content = Content.Clone();
            res.IsEndnote = IsEndnote;
            res.CustomMark = CustomMark;
            res.DocBlockId = DocBlockId;
            return res;
        }
        public override UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            if (Content != null) 
                Content = Content.Optimize(true, pars);
            if (Content == null && HtmlTitle == null) 
                return null;
            return this;
        }
        public override void GetAllItems(List<UnitextItem> res, int lev)
        {
            if (res != null) 
                res.Add(this);
            if (Content != null) 
            {
                Content.Parent = this;
                Content.GetAllItems(res, lev + 1);
            }
        }
        internal override string InnerTag
        {
            get
            {
                return "footnote";
            }
        }
        public override UnitextItem FindById(string id)
        {
            if (Id == id) 
                return this;
            UnitextItem res;
            if (Content != null) 
            {
                if ((((res = Content.FindById(id)))) != null) 
                    return res;
            }
            return null;
        }
        static GetPlaintextParam m_FootnotesParam = new GetPlaintextParam() { SetPositions = true, NewLine = " ", PageBreak = " ", TableStart = " ", TableCellEnd = " ", TableEnd = " " };
        static GetPlaintextParam m_FootnotesParam1 = new GetPlaintextParam() { SetPositions = false, NewLine = " ", PageBreak = " ", TableStart = " ", TableCellEnd = " ", TableEnd = " " };
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            if (pars == null) 
                pars = UnitextItem.m_DefParams;
            if (!string.IsNullOrEmpty(pars.FootnotesTemplate) && Content != null) 
            {
                StringBuilder tmp = new StringBuilder();
                Content.GetPlaintext(tmp, (pars.SetPositions ? m_FootnotesParam : m_FootnotesParam1));
                if (tmp.Length > 0 && tmp[tmp.Length - 1] == '.') 
                    tmp.Length--;
                string txt = tmp.ToString();
                if ((CustomMark != null && (CustomMark.Length < txt.Length) && txt.StartsWith(CustomMark)) && char.IsWhiteSpace(txt[CustomMark.Length])) 
                {
                    tmp.Remove(0, CustomMark.Length + 1);
                    txt = tmp.ToString();
                }
                int d = pars.FootnotesTemplate.IndexOf("%1");
                if (d < 0) 
                {
                    if (pars.SetPositions) 
                        Content.AddPlainTextPos(res.Length);
                    res.Append(pars.FootnotesTemplate);
                    if (pars.SetPositions) 
                        Content.EndChar = (Content.BeginChar + ((res.Length - BeginChar))) - 1;
                }
                else 
                {
                    if (pars.SetPositions) 
                        Content.AddPlainTextPos(res.Length + ((d < 0 ? 0 : d)));
                    if (txt.Length > 0) 
                        res.Append(pars.FootnotesTemplate.Replace("%1", txt));
                }
            }
            if (pars != null && pars.SetPositions) 
                EndChar = (BeginChar + ((res.Length - BeginChar))) - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (Content == null && DocBlockId == null) 
                return;
            if (par.HideEditionsAndComments && (Content is UnitextContainer)) 
            {
                if ((Content as UnitextContainer).Typ == UnitextContainerType.Edition || (Content as UnitextContainer).Typ == UnitextContainerType.Comment) 
                    return;
            }
            if (!par.CallBefore(this, res)) 
                return;
            string id = "";
            if (Id != null) 
                id = string.Format(" id=\"{0}\"", Id);
            if (DocBlockId != null) 
            {
                bool aaa = false;
                if (par != null) 
                {
                    for (UnitextItem p = Parent; p != null; p = p.Parent) 
                    {
                        UnitextDocblock db = p.FindById(DocBlockId) as UnitextDocblock;
                        if (db != null) 
                        {
                            if (!par.m_FootnotesDb.Contains(db)) 
                                par.m_FootnotesDb.Add(db);
                            aaa = true;
                            break;
                        }
                    }
                }
                if (!aaa) 
                {
                }
                res.AppendFormat("<span title=\"");
                Pullenti.Util.MiscHelper.CorrectHtmlValue(res, HtmlTitle ?? "", true, false);
                res.AppendFormat("\" style=\"color:red\"><sup>{0}</sup></span>", CustomMark ?? "*");
                par.CallAfter(this, res);
                return;
            }
            if (par != null && par.Footnotes == GetHtmlParamFootnoteOutType.EndOfUnit) 
            {
                if (IsEndnote) 
                    par.m_Endnotes.Add(this);
                else 
                    par.m_Footnotes.Add(this);
                StringBuilder tmp = new StringBuilder();
                Content.GetPlaintext(tmp, m_FootnotesParam1);
                res.AppendFormat("<span title=\"");
                Pullenti.Util.MiscHelper.CorrectHtmlValue(res, tmp.ToString(), true, false);
                if (IsEndnote) 
                    res.AppendFormat("\" style=\"color:red\"><sup>&lt;E{0}&gt;</sup></span>", par.m_Endnotes.Count);
                else if ((Content is UnitextContainer) && (Content as UnitextContainer).Typ == UnitextContainerType.Edition) 
                    res.AppendFormat("\" style=\"color:lightgray;font-size:smaller\"><sup>&lt;{0}&gt;</sup></span>", par.m_Footnotes.Count);
                else 
                    res.AppendFormat("\" style=\"color:red\"><sup>&lt;{0}&gt;</sup></span>", par.m_Footnotes.Count);
                par.CallAfter(this, res);
                return;
            }
            if (par != null && par.Footnotes == GetHtmlParamFootnoteOutType.InBrackets) 
            {
                res.AppendFormat(" <i{0}><sub title=\"Сноска {1}\">(", id, CustomMark ?? "");
                Content.GetHtml(res, par);
                res.Append(")</sub></i>");
            }
            else 
            {
                StringBuilder tmp = new StringBuilder();
                Content.GetPlaintext(tmp, m_FootnotesParam1);
                string txt = tmp.ToString().Trim();
                res.AppendFormat("<span title=\"");
                Pullenti.Util.MiscHelper.CorrectHtmlValue(res, txt, true, false);
                res.AppendFormat("\" style=\"color:red\"{1}><sup>{0})</sup></span>", (IsEndnote ? "**" : "*"), id);
            }
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("footnote");
            this._writeXmlAttrs(xml);
            if (IsEndnote) 
                xml.WriteAttributeString("endnote", "true");
            if (CustomMark != null) 
                xml.WriteAttributeString("mark", Pullenti.Util.MiscHelper.CorrectXmlValue(CustomMark));
            if (DocBlockId != null) 
                xml.WriteAttributeString("docblockid", DocBlockId);
            if (Content != null) 
                Content.GetXml(xml);
            xml.WriteEndElement();
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "endnote") 
                        IsEndnote = a.Value == "true";
                    else if (a.LocalName == "mark") 
                        CustomMark = a.Value;
                    else if (a.LocalName == "docblockid") 
                        DocBlockId = a.Value;
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                Content = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(x);
                break;
            }
        }
        internal override void AddPlainTextPos(int d)
        {
            base.AddPlainTextPos(d);
            if (Content != null) 
                Content.AddPlainTextPos(d);
        }
        internal override void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
            if (Content != null) 
                Content.Correct(typ, data);
        }
        internal override bool ReplaceChild(UnitextItem old, UnitextItem ne)
        {
            if (Content != old) 
                return false;
            Content = ne;
            ne.Parent = this;
            return true;
        }
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = cp;
            cp++;
            if (res != null) 
                res.Append('<');
            if (Content != null) 
            {
                Content.SetDefaultTextPos(ref cp, res);
                Content.Parent = this;
            }
            cp++;
            if (res != null) 
                res.Append('>');
            EndChar = cp - 1;
        }
    }
}