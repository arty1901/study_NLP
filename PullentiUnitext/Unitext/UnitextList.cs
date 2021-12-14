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
    /// Список
    /// </summary>
    public class UnitextList : UnitextItem
    {
        public override UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            foreach (UnitextListitem it in Items) 
            {
                it.Parent = this;
                it.Optimize(false, pars);
                if ((it.Content is UnitextList) && it.Sublist == null && (it.Content as UnitextList).Level == (Level + 1)) 
                {
                    it.Sublist = it.Content as UnitextList;
                    it.Content = null;
                    continue;
                }
                if (it.Sublist != null) 
                    continue;
                UnitextContainer cnt = it.Content as UnitextContainer;
                if (cnt != null && (cnt.Children[cnt.Children.Count - 1] is UnitextList) && (cnt.Children[cnt.Children.Count - 1] as UnitextList).Level == (Level + 1)) 
                {
                    it.Sublist = cnt.Children[cnt.Children.Count - 1] as UnitextList;
                    cnt.Children.RemoveAt(cnt.Children.Count - 1);
                    it.Content = it.Content.Optimize(true, pars);
                }
            }
            if (Items.Count == 0) 
                return null;
            if (UnorderPrefix == null) 
            {
                string pre = null;
                bool ok = true;
                foreach (UnitextListitem it in Items) 
                {
                    if (it.Prefix != null) 
                    {
                        if (pre == null) 
                            pre = Pullenti.Unitext.Internal.Uni.UnitextHelper.GetPlaintext(it.Prefix);
                        else if (pre != Pullenti.Unitext.Internal.Uni.UnitextHelper.GetPlaintext(it.Prefix)) 
                        {
                            ok = false;
                            break;
                        }
                    }
                }
                if ((ok && pre != null && pre.Length == 1) && !char.IsLetterOrDigit(pre[0])) 
                {
                    UnorderPrefix = pre;
                    foreach (UnitextListitem it in Items) 
                    {
                        it.Prefix = null;
                    }
                }
            }
            else 
                foreach (UnitextListitem it in Items) 
                {
                    it.Prefix = null;
                }
            if (Items.Count == 1 && Level == 0) 
            {
                if (Items[0].Sublist == null) 
                {
                    UnitextListitem it = Items[0];
                    if (it.Prefix == null) 
                        return it.Content;
                    if (it.Content == null) 
                        return it.Prefix;
                    UnitextContainer ccc = new UnitextContainer();
                    ccc.Children.Add(it.Prefix);
                    ccc.Children.Add(new UnitextPlaintext() { Text = " ", BeginChar = it.Prefix.EndChar, EndChar = it.Prefix.EndChar });
                    ccc.Children.Add(it.Content);
                    return ccc.Optimize(true, pars);
                }
            }
            return this;
        }
        /// <summary>
        /// Элементы списка
        /// </summary>
        public List<UnitextListitem> Items = new List<UnitextListitem>();
        /// <summary>
        /// Для ненумерованных списков это общий префикс
        /// </summary>
        public string UnorderPrefix;
        /// <summary>
        /// Уровень вложенности (0 - первый)
        /// </summary>
        public int Level = 0;
        public override string ToString()
        {
            return string.Format("{0}List ({1} items, {2} level)", (UnorderPrefix == null ? "" : string.Format("Unordered ({0})", UnorderPrefix)), Items.Count, Level);
        }
        public override UnitextItem Clone()
        {
            UnitextList res = new UnitextList();
            res._cloneFrom(this);
            foreach (UnitextListitem it in Items) 
            {
                res.Items.Add(it.Clone() as UnitextListitem);
            }
            res.UnorderPrefix = UnorderPrefix;
            res.Level = Level;
            return res;
        }
        public override void GetAllItems(List<UnitextItem> res, int lev)
        {
            if (res != null) 
                res.Add(this);
            foreach (UnitextListitem it in Items) 
            {
                it.Parent = this;
                it.GetAllItems(res, lev + 1);
            }
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
                return "lst";
            }
        }
        public override UnitextItem FindById(string id)
        {
            if (Id == id) 
                return this;
            foreach (UnitextListitem it in Items) 
            {
                UnitextItem res = it.FindById(id);
                if (res != null) 
                    return res;
            }
            return null;
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars = null)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            foreach (UnitextListitem it in Items) 
            {
                for (int ii = 0; ii < Level; ii++) 
                {
                    res.Append('\t');
                }
                if (it == Items[0] && pars != null && pars.SetPositions) 
                    BeginChar = res.Length;
                if (UnorderPrefix != null) 
                    res.AppendFormat("{0} ", UnorderPrefix);
                it.GetPlaintext(res, pars);
            }
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            if ((Parent is UnitextContainer) && (Parent as UnitextContainer).Typ == UnitextContainerType.Head && Items.Count == 1) 
            {
                UnitextListitem it = Items[0];
                if (it.Prefix != null) 
                {
                    it.Prefix.GetHtml(res, par);
                    res.Append("&nbsp;");
                }
                if (it.Content != null) 
                    it.Content.GetHtml(res, par);
                if (it.Sublist != null) 
                    it.Sublist.GetHtml(res, par);
            }
            else 
            {
                res.Append("\r\n<UL");
                if (Id != null) 
                {
                    res.AppendFormat(" id=\"{0}\"", Id);
                    if (par != null && par.Styles.ContainsKey(Id)) 
                        res.AppendFormat(" style=\"{0}\"", par.Styles[Id]);
                }
                res.Append(">");
                foreach (UnitextListitem it in Items) 
                {
                    it.GetHtml(res, par);
                }
                res.Append("\r\n</UL>");
            }
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("list");
            this._writeXmlAttrs(xml);
            if (UnorderPrefix != null) 
                xml.WriteAttributeString("pref", Pullenti.Util.MiscHelper.CorrectXmlValue(UnorderPrefix));
            if (Level > 0) 
                xml.WriteAttributeString("level", Level.ToString());
            foreach (UnitextListitem it in Items) 
            {
                it.GetXml(xml);
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
                    if (a.LocalName == "pref") 
                        UnorderPrefix = a.Value;
                    else if (a.LocalName == "level") 
                    {
                        int n;
                        if (int.TryParse(a.Value, out n)) 
                            Level = n;
                    }
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                UnitextItem it = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(x);
                if (it is UnitextListitem) 
                    Items.Add(it as UnitextListitem);
            }
        }
        internal override void AddPlainTextPos(int d)
        {
            base.AddPlainTextPos(d);
            foreach (UnitextListitem it in Items) 
            {
                it.AddPlainTextPos(d);
            }
        }
        internal override void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
            foreach (UnitextListitem it in Items) 
            {
                it.Correct(typ, data);
            }
        }
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = cp;
            foreach (UnitextListitem it in Items) 
            {
                it.SetDefaultTextPos(ref cp, res);
                it.Parent = this;
            }
            EndChar = cp - 1;
        }
    }
}