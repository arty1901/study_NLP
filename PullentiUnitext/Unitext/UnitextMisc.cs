/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;
using System.Xml;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Разные нетекстовые элементы
    /// </summary>
    public class UnitextMisc : UnitextItem
    {
        /// <summary>
        /// Тип нетекстового элемента
        /// </summary>
        public UnitextMiscType Typ = UnitextMiscType.Undefined;
        public override string ToString()
        {
            return Typ.ToString();
        }
        internal override string InnerTag
        {
            get
            {
                return "misc";
            }
        }
        public override UnitextItem Clone()
        {
            UnitextMisc res = new UnitextMisc();
            res._cloneFrom(this);
            res.Typ = Typ;
            return res;
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            if (Id != null) 
                res.AppendFormat("<a name=\"{0}\"> </a>", Id);
            if (Typ == UnitextMiscType.HorizontalLine) 
                res.Append("\r\n<HR/>");
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("misc");
            this._writeXmlAttrs(xml);
            xml.WriteAttributeString("type", Typ.ToString().ToLower());
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
                            Typ = (UnitextMiscType)Enum.Parse(typeof(UnitextMiscType), a.Value, true);
                        }
                        catch(Exception ex335) 
                        {
                        }
                    }
                }
            }
        }
        public override bool IsWhitespaces
        {
            get
            {
                return true;
            }
        }
    }
}