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
    /// Разрыв страниц
    /// </summary>
    public class UnitextPagebreak : UnitextItem
    {
        public UnitextPagebreak() : base()
        {
        }
        public override string ToString()
        {
            return "PageBreak";
        }
        public override UnitextItem Clone()
        {
            UnitextPagebreak res = new UnitextPagebreak();
            res._cloneFrom(this);
            return res;
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            res.Append(((pars ?? UnitextItem.m_DefParams)).NewLine ?? "");
            res.Append(((pars ?? UnitextItem.m_DefParams)).PageBreak ?? "");
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            int k = 0;
            for (UnitextItem p = Parent; p != null && (k < 10); p = p.Parent,k++) 
            {
                UnitextContainer cnt = p as UnitextContainer;
                if (cnt != null) 
                {
                    if (cnt.Typ != UnitextContainerType.Undefined && cnt.Typ != UnitextContainerType.Head) 
                    {
                        res.Append("\r\n<BR/>");
                        par.CallAfter(this, res);
                        return;
                    }
                }
                UnitextDocblock db = p as UnitextDocblock;
                if (db != null) 
                {
                    if (string.Compare(db.Typname ?? "", "Document", true) == 0 || string.Compare(db.Typname ?? "", "Subdocument", true) == 0 || string.Compare(db.Typname ?? "", "Appendix", true) == 0) 
                    {
                    }
                    else if (db.Typname != null) 
                    {
                        res.Append("\r\n<BR/>");
                        par.CallAfter(this, res);
                        return;
                    }
                }
            }
            if (par != null) 
                par._outFootnotes(res);
            res.Append("\r\n<HR/><br/></br>");
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("pagebreak");
            this._writeXmlAttrs(xml);
            xml.WriteEndElement();
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
        }
        public override bool IsWhitespaces
        {
            get
            {
                return true;
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
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = (EndChar = cp);
            cp++;
            if (res != null) 
                res.Append('\f');
        }
    }
}