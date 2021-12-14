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
    /// Гиперссылка
    /// </summary>
    public class UnitextHyperlink : UnitextItem
    {
        public UnitextHyperlink() : base()
        {
        }
        /// <summary>
        /// Сама ссылка
        /// </summary>
        public string Href;
        /// <summary>
        /// Содержимое ссылки
        /// </summary>
        public UnitextItem Content;
        /// <summary>
        /// Это внутренняя ссылка, Href - якорь на элемент СТП
        /// </summary>
        public bool IsInternal;
        /// <summary>
        /// Используйте произвольным образом, сериализуется
        /// </summary>
        public string Data;
        public override string ToString()
        {
            return string.Format("Hyperlink {0}", Href ?? "");
        }
        public override UnitextItem Clone()
        {
            UnitextHyperlink res = new UnitextHyperlink();
            res._cloneFrom(this);
            res.Href = Href;
            res.Data = Data;
            res.IsInternal = IsInternal;
            if (Content != null) 
                res.Content = Content.Clone();
            return res;
        }
        internal override string InnerTag
        {
            get
            {
                return "hyplnk";
            }
        }
        public override bool IsInline
        {
            get
            {
                if (Content == null) 
                    return true;
                return Content.IsInline;
            }
            set
            {
            }
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
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            if (Content != null) 
                Content.GetPlaintext(res, pars);
            if (pars == null) 
                pars = UnitextItem.m_DefParams;
            if (!string.IsNullOrEmpty(pars.HyperlinksTemplate) && Href != null && !IsInternal) 
            {
                StringBuilder tmp = new StringBuilder();
                for (int i = BeginChar; i < res.Length; i++) 
                {
                    tmp.Append(res[i]);
                }
                string txt = tmp.ToString();
                if (tmp.Length == 0) 
                    res.Append(Href);
                else if (string.Compare(Href, txt, true) != 0) 
                    res.Append(pars.HyperlinksTemplate.Replace("%1", Href));
            }
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            string tit = Href;
            if (HtmlTitle != null) 
            {
                StringBuilder tmp = new StringBuilder();
                Pullenti.Util.MiscHelper.CorrectHtmlValue(tmp, HtmlTitle, true, false);
                tit = tmp.ToString();
            }
            if (IsInternal) 
                res.Append("<i>");
            if (Href != null) 
                res.AppendFormat("<a href=\"{0}\" title=\"{1}\"{2}", Href, tit ?? "", (par.HyperlinksTargetBlank || Href.StartsWith("http") ? " target=\"_blank\"" : ""));
            else 
                res.AppendFormat("<span style=\"text-decoration:underline\" title=\"{0}\"", tit ?? "");
            if (Id != null) 
                res.AppendFormat(" id=\"{0}\"", Id);
            res.Append(">");
            if (Content != null) 
                Content.GetHtml(res, par);
            if (Href != null) 
                res.Append("</a>");
            else 
                res.Append("</span>");
            if (IsInternal) 
                res.Append("</i>");
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("hyperlink");
            this._writeXmlAttrs(xml);
            if (Href != null) 
            {
                xml.WriteAttributeString("href", Pullenti.Util.MiscHelper.CorrectXmlValue(Href));
                if (IsInternal) 
                    xml.WriteAttributeString("internal", "true");
            }
            if (Data != null) 
                xml.WriteAttributeString("data", Pullenti.Util.MiscHelper.CorrectXmlValue(Data));
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
                    if (a.LocalName == "href") 
                        Href = a.Value;
                    else if (a.LocalName == "internal") 
                        IsInternal = a.Value == "true";
                    else if (a.LocalName == "data") 
                        Data = a.Value;
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
        public override UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            if (Content != null) 
                Content = Content.Optimize(true, pars);
            if (pars != null && Href != null) 
            {
                if (((Href.Length > 2 && Href[0] == '<' && Href[2] == '>')) || ((Href.Length > 3 && Href[0] == '<' && Href[3] == '>'))) 
                {
                    string tag = Href.Substring(0, (Href[2] == '>' ? 3 : 4));
                    UnitextPlaintext pt = Content as UnitextPlaintext;
                    if (pt != null && pt.Text.StartsWith(tag)) 
                    {
                        UnitextFootnote foot = new UnitextFootnote();
                        if (Href[2] == '>') 
                            foot.CustomMark = Href.Substring(1, 1);
                        else 
                            foot.CustomMark = Href.Substring(1, 2);
                        string txt = Href.Substring(tag.Length).Trim();
                        if (txt.Contains("\\\"")) 
                            txt = txt.Replace("\\\"", "\"");
                        foot.Content = new UnitextPlaintext() { Text = txt };
                        return foot;
                    }
                    if (Content is UnitextFootnote) 
                        return Content;
                }
            }
            return this;
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
            if (Content != null) 
            {
                Content.SetDefaultTextPos(ref cp, res);
                Content.Parent = this;
            }
            EndChar = cp - 1;
        }
    }
}