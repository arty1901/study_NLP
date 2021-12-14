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
    /// Элемент списка
    /// </summary>
    public class UnitextListitem : UnitextItem
    {
        /// <summary>
        /// Префикс (может быть null)
        /// </summary>
        public UnitextItem Prefix;
        /// <summary>
        /// Содержимое списка
        /// </summary>
        public UnitextItem Content;
        /// <summary>
        /// Вложенный список
        /// </summary>
        public UnitextList Sublist;
        static GetPlaintextParam m_Pars = new GetPlaintextParam();
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Prefix != null) 
                res.AppendFormat("{0}: ", Prefix.ToString());
            if (Content != null) 
                res.Append(Content.ToString());
            if (Sublist != null) 
                res.AppendFormat(" + {0}", Sublist.ToString());
            return res.ToString();
        }
        public override UnitextItem Clone()
        {
            UnitextListitem res = new UnitextListitem();
            res._cloneFrom(this);
            if (Prefix != null) 
                res.Prefix = Prefix.Clone();
            if (Content != null) 
                res.Content = Content.Clone();
            if (Sublist != null) 
                res.Sublist = Sublist.Clone() as UnitextList;
            return res;
        }
        public override bool IsInline
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        internal override string InnerTag
        {
            get
            {
                return "litm";
            }
        }
        public override UnitextItem FindById(string id)
        {
            if (Id == id) 
                return this;
            UnitextItem res;
            if (Prefix != null) 
            {
                if ((((res = Prefix.FindById(id)))) != null) 
                    return res;
            }
            if (Content != null) 
            {
                if ((((res = Content.FindById(id)))) != null) 
                    return res;
            }
            if (Sublist != null) 
            {
                if ((((res = Sublist.FindById(id)))) != null) 
                    return res;
            }
            return null;
        }
        public override void GetAllItems(List<UnitextItem> res, int lev)
        {
            if (res != null) 
                res.Add(this);
            if (Prefix != null) 
            {
                Prefix.Parent = this;
                Prefix.GetAllItems(res, lev + 1);
            }
            if (Content != null) 
            {
                Content.Parent = this;
                Content.GetAllItems(res, lev + 1);
            }
            if (Sublist != null) 
            {
                Sublist.Parent = this;
                Sublist.GetAllItems(res, lev + 1);
            }
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            if (pars == null) 
                pars = UnitextItem.m_DefParams;
            if (Prefix != null) 
            {
                Prefix.GetPlaintext(res, pars);
                res.Append(' ');
            }
            if (Content != null) 
                Content.GetPlaintext(res, pars);
            res.Append(pars.NewLine ?? "");
            if (Sublist != null) 
                Sublist.GetPlaintext(res, pars);
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            res.Append("\r\n<LI");
            if (Id != null) 
            {
                res.AppendFormat(" id=\"{0}\"", Id);
                if (par != null && par.Styles.ContainsKey(Id)) 
                    res.AppendFormat(" style=\"{0}\"", par.Styles[Id]);
            }
            res.Append(">");
            if (Prefix != null) 
            {
                Prefix.GetHtml(res, par);
                res.Append(' ');
            }
            if (Content != null) 
                Content.GetHtml(res, par);
            if (Sublist != null) 
                Sublist.GetHtml(res, par);
            res.Append("</LI>");
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("listitem");
            this._writeXmlAttrs(xml);
            if (Prefix != null) 
            {
                xml.WriteStartElement("prefix");
                Prefix.GetXml(xml);
                xml.WriteEndElement();
            }
            if (Content != null) 
                Content.GetXml(xml);
            if (Sublist != null) 
                Sublist.GetXml(xml);
            xml.WriteEndElement();
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "prefix") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        Prefix = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx);
                        break;
                    }
                }
                else 
                {
                    UnitextItem it = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(x);
                    if (it is UnitextList) 
                        Sublist = it as UnitextList;
                    else if (it != null) 
                        Content = it;
                }
            }
        }
        public override UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            if (Prefix != null) 
                Prefix = Prefix.Optimize(true, pars);
            if (Content != null) 
                Content = Content.Optimize(true, pars);
            if (Content != null && Content.IsWhitespaces) 
                Content = null;
            if (Sublist != null) 
                Sublist = Sublist.Optimize(false, pars) as UnitextList;
            return this;
        }
        internal override void AddPlainTextPos(int d)
        {
            base.AddPlainTextPos(d);
            if (Prefix != null) 
                Prefix.AddPlainTextPos(d);
            if (Content != null) 
                Content.AddPlainTextPos(d);
            if (Sublist != null) 
                Sublist.AddPlainTextPos(d);
        }
        internal override void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
            if (Prefix != null) 
                Prefix.Correct(typ, data);
            if (Content != null) 
                Content.Correct(typ, data);
            if (Sublist != null) 
                Sublist.Correct(typ, data);
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
            if (Prefix != null) 
            {
                Prefix.SetDefaultTextPos(ref cp, res);
                Prefix.Parent = this;
            }
            if (Content != null) 
            {
                Content.SetDefaultTextPos(ref cp, res);
                Content.Parent = this;
            }
            cp++;
            if (res != null) 
                res.Append('\n');
            if (Sublist != null) 
            {
                Sublist.SetDefaultTextPos(ref cp, res);
                Sublist.Parent = this;
            }
            EndChar = cp - 1;
        }
    }
}