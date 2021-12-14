/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Xml;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Информация о страницах и колонтитулах.
    /// </summary>
    public class UnitextPagesection : UnitextItem
    {
        /// <summary>
        /// Ширина страницы (в см)
        /// </summary>
        public double Width;
        /// <summary>
        /// Высота страницы (в см)
        /// </summary>
        public double Height;
        /// <summary>
        /// Левый отступ (в см)
        /// </summary>
        public double Left;
        /// <summary>
        /// Верхний отступ (в см)
        /// </summary>
        public double Top;
        /// <summary>
        /// Правый отступ (в см)
        /// </summary>
        public double Right;
        /// <summary>
        /// Нижний отступ (в см)
        /// </summary>
        public double Bottom;
        /// <summary>
        /// Высота верхнего колонтитула (в см)
        /// </summary>
        public double HeaderHeight;
        /// <summary>
        /// Высота нижнего колонтитула (в см)
        /// </summary>
        public double FooterHeight;
        /// <summary>
        /// Верхний колонтитул
        /// </summary>
        public UnitextItem Header;
        /// <summary>
        /// Нижний колонтитул
        /// </summary>
        public UnitextItem Footer;
        public override UnitextItem Clone()
        {
            UnitextPagesection res = new UnitextPagesection();
            res._cloneFrom(this);
            res.Width = Width;
            res.Height = Height;
            res.Left = Left;
            res.Right = Right;
            res.Top = Top;
            res.Bottom = Bottom;
            res.HeaderHeight = HeaderHeight;
            res.FooterHeight = FooterHeight;
            if (Header != null) 
                res.Header = Header.Clone();
            if (Footer != null) 
                res.Footer = Footer.Clone();
            if (m_StyledFrag != null) 
                res.m_StyledFrag = m_StyledFrag.Clone();
            return res;
        }
        public override string ToString()
        {
            return string.Format("Section {0}: {1}x{2}", Id, Pullenti.Util.MiscHelper.OutDouble(Width), Pullenti.Util.MiscHelper.OutDouble(Height));
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("section");
            this._writeXmlAttrs(xml);
            if (Width > 0) 
                xml.WriteAttributeString("width", Pullenti.Util.MiscHelper.OutDouble(Width));
            if (Height > 0) 
                xml.WriteAttributeString("height", Pullenti.Util.MiscHelper.OutDouble(Height));
            if (Left > 0) 
                xml.WriteAttributeString("left", Pullenti.Util.MiscHelper.OutDouble(Left));
            if (Top > 0) 
                xml.WriteAttributeString("top", Pullenti.Util.MiscHelper.OutDouble(Top));
            if (Right > 0) 
                xml.WriteAttributeString("right", Pullenti.Util.MiscHelper.OutDouble(Right));
            if (Bottom > 0) 
                xml.WriteAttributeString("bottom", Pullenti.Util.MiscHelper.OutDouble(Bottom));
            if (HeaderHeight > 0) 
                xml.WriteAttributeString("header", Pullenti.Util.MiscHelper.OutDouble(HeaderHeight));
            if (FooterHeight > 0) 
                xml.WriteAttributeString("footer", Pullenti.Util.MiscHelper.OutDouble(FooterHeight));
            if (Header != null) 
            {
                xml.WriteStartElement("header");
                Header.GetXml(xml);
                xml.WriteEndElement();
                if (Header.m_StyledFrag != null) 
                    Header.m_StyledFrag.GetXml(xml, "headerstylefrag", false);
            }
            if (Footer != null) 
            {
                xml.WriteStartElement("footer");
                Footer.GetXml(xml);
                xml.WriteEndElement();
                if (Footer.m_StyledFrag != null) 
                    Footer.m_StyledFrag.GetXml(xml, "footerstylefrag", false);
            }
            xml.WriteEndElement();
        }
        double _parseDouble(string str)
        {
            double d;
            if (Pullenti.Util.MiscHelper.TryParseDouble(str, out d)) 
                return d;
            return 0;
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
            foreach (XmlAttribute a in xml.Attributes) 
            {
                if (a.LocalName == "width") 
                    Width = this._parseDouble(a.Value);
                else if (a.LocalName == "height") 
                    Height = this._parseDouble(a.Value);
                else if (a.LocalName == "left") 
                    Left = this._parseDouble(a.Value);
                else if (a.LocalName == "top") 
                    Top = this._parseDouble(a.Value);
                else if (a.LocalName == "right") 
                    Right = this._parseDouble(a.Value);
                else if (a.LocalName == "bottom") 
                    Bottom = this._parseDouble(a.Value);
                else if (a.LocalName == "header") 
                    HeaderHeight = this._parseDouble(a.Value);
                else if (a.LocalName == "footer") 
                    FooterHeight = this._parseDouble(a.Value);
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "header") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        Header = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx);
                        break;
                    }
                }
                else if (x.LocalName == "footer") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        Footer = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx);
                        break;
                    }
                }
                else if (x.LocalName == "headerstylefrag") 
                {
                    if (Header == null) 
                        continue;
                    Header.m_StyledFrag = new UnitextStyledFragment();
                    Header.m_StyledFrag.FromXml(x);
                }
                else if (x.LocalName == "footerstylefrag") 
                {
                    if (Footer == null) 
                        continue;
                    Footer.m_StyledFrag = new UnitextStyledFragment();
                    Footer.m_StyledFrag.FromXml(x);
                }
            }
        }
    }
}