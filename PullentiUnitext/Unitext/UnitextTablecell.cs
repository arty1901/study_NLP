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
    /// Ячейка таблицы
    /// </summary>
    public class UnitextTablecell : UnitextItem
    {
        /// <summary>
        /// Начальный столбец ячейки
        /// </summary>
        public int ColBegin
        {
            get;
            set;
        }
        /// <summary>
        /// Конечный столбец ячейки
        /// </summary>
        public int ColEnd
        {
            get;
            set;
        }
        /// <summary>
        /// Начальная строка ячейки
        /// </summary>
        public int RowBegin
        {
            get;
            set;
        }
        /// <summary>
        /// Конечная строка ячейки
        /// </summary>
        public int RowEnd
        {
            get;
            set;
        }
        /// <summary>
        /// Содержимое (может быть null)
        /// </summary>
        public UnitextItem Content;
        /// <summary>
        /// Это признак восстановления слипшихся строк таблицы
        /// </summary>
        public bool IsPsevdo = false;
        public override UnitextItem Clone()
        {
            UnitextTablecell res = new UnitextTablecell();
            res._cloneFrom(this);
            res.ColBegin = ColBegin;
            res.ColEnd = ColEnd;
            res.RowBegin = RowBegin;
            res.RowEnd = RowEnd;
            if (Content != null) 
                res.Content = Content.Clone();
            res.IsPsevdo = IsPsevdo;
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
                return "tcell";
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
        public override UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            if (Content != null) 
                Content = Content.Optimize(true, pars);
            if (Content != null && Content.IsWhitespaces) 
                Content = null;
            UnitextList li = Content as UnitextList;
            if ((li != null && li.Items.Count == 1 && li.Items[0].Content == null) && li.Items[0].Prefix != null) 
                Content = li.Items[0].Prefix;
            return this;
        }
        public override string ToString()
        {
            return string.Format("Cell{0} {1}{2} {3} {4}", (IsPsevdo ? "?" : ""), ColBegin, (ColBegin < ColEnd ? string.Format("-{0}", ColEnd) : ""), (RowBegin < RowEnd ? string.Format(" (rows {0}-{1})", RowBegin, RowEnd) : ""), (Content == null ? "" : Content.ToString()));
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            if (pars == null) 
                pars = UnitextItem.m_DefParams;
            if (Content != null) 
            {
                int i0 = res.Length;
                Content.GetPlaintext(res, pars);
                for (int i = res.Length - 1; i >= i0; i--) 
                {
                    if ((char.IsWhiteSpace(res[i]) || res[i] == ((char)7) || res[i] == '\t') || res[i] == '\f') 
                        res.Length = i;
                    else 
                        break;
                }
            }
            if (string.IsNullOrEmpty(pars.TableCellEnd)) 
                res.Append(pars.NewLine ?? "");
            else 
            {
                if (ColBegin < ColEnd) 
                {
                    for (int i = 0; i < ((ColEnd - ColBegin)); i++) 
                    {
                        res.Append('\t');
                    }
                }
                if (RowBegin < RowEnd) 
                {
                    for (int i = 0; i < ((RowEnd - RowBegin)); i++) 
                    {
                        res.Append('\f');
                    }
                }
                if (pars == null) 
                    res.Append((char)7);
                else 
                    res.Append(pars.TableCellEnd ?? "");
            }
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            res.Append("<td valign=\"top\"");
            if (RowEnd > RowBegin) 
                res.AppendFormat(" rowspan=\"{0}\"", (RowEnd - RowBegin) + 1);
            if (ColEnd > ColBegin) 
                res.AppendFormat(" colspan=\"{0}\"", (ColEnd - ColBegin) + 1);
            if (Id != null) 
            {
                res.AppendFormat(" id=\"{0}\"", Id);
                if (par != null && par.Styles.ContainsKey(Id)) 
                    res.AppendFormat(" style=\"{0}\"", par.Styles[Id]);
                else if (par != null && par.OutStyles && this.GetStyledFragment(-1) != null) 
                {
                    UnitextStyledFragment fr = this.GetStyledFragment(-1);
                    if (fr != null && fr.Typ != UnitextStyledFragmentType.Tablecell) 
                        fr = fr.Parent;
                    if ((fr != null && fr.Typ == UnitextStyledFragmentType.Tablecell && fr.StyleId > 0) && fr.Style != null) 
                    {
                        res.Append(" style=\"");
                        fr.Style.GetHtml(res);
                        res.Append("\"");
                    }
                }
            }
            res.Append(">");
            if (Content == null) 
                res.Append("&nbsp;");
            else 
                Content.GetHtml(res, par);
            res.Append("</td>");
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("cell");
            this._writeXmlAttrs(xml);
            if (IsPsevdo) 
                xml.WriteAttributeString("psevdo", "true");
            xml.WriteAttributeString("row", RowBegin.ToString());
            if (RowEnd > RowBegin) 
                xml.WriteAttributeString("rowspan", (((RowEnd - RowBegin) + 1)).ToString());
            xml.WriteAttributeString("col", ColBegin.ToString());
            if (ColEnd > ColBegin) 
                xml.WriteAttributeString("colspan", (((ColEnd - ColBegin) + 1)).ToString());
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
                    if (a.LocalName == "row") 
                        RowBegin = (RowEnd = int.Parse(a.Value));
                    else if (a.LocalName == "col") 
                        ColBegin = (ColEnd = int.Parse(a.Value));
                    else if (a.LocalName == "rowspan") 
                        RowEnd = (RowBegin + int.Parse(a.Value)) - 1;
                    else if (a.LocalName == "colspan") 
                        ColEnd = (ColBegin + int.Parse(a.Value)) - 1;
                    else if (a.LocalName == "psevdo") 
                        IsPsevdo = true;
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                Content = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(x);
                if (Content != null) 
                    break;
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
            if (Content != null) 
            {
                Content.SetDefaultTextPos(ref cp, res);
                Content.Parent = this;
            }
            EndChar = cp;
            cp++;
            if (res != null) 
                res.AppendFormat("{0}", (char)7);
        }
    }
}