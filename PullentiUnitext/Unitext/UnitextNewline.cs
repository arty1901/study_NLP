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
    /// Переход на новую строку
    /// </summary>
    public class UnitextNewline : UnitextItem
    {
        /// <summary>
        /// Количество переходов
        /// </summary>
        public int Count = 1;
        public override string ToString()
        {
            return string.Format("NewLine ({0})", Count);
        }
        public override UnitextItem Clone()
        {
            UnitextNewline res = new UnitextNewline();
            res._cloneFrom(this);
            res.Count = Count;
            return res;
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            for (int i = 0; i < Count; i++) 
            {
                res.Append(((pars ?? UnitextItem.m_DefParams)).NewLine ?? "");
            }
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        static bool _isAllUpper(UnitextPlaintext it)
        {
            if (it == null) 
                return false;
            foreach (char ch in it.Text) 
            {
                if (char.IsLetter(ch)) 
                {
                    if (!char.IsUpper(ch)) 
                        return false;
                }
            }
            return true;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            bool isDummy = false;
            if (Parent is UnitextContainer) 
            {
                if ((Parent as UnitextContainer).Children.Count < 1000) 
                {
                    int ii = (Parent as UnitextContainer).GetChildIndexOf(this);
                    if (ii > 0 && (ii < ((Parent as UnitextContainer).Children.Count - 1))) 
                    {
                        UnitextContainer cnt = Parent as UnitextContainer;
                        if (cnt.Typ == UnitextContainerType.Name) 
                        {
                            isDummy = true;
                            if ((ii == 1 && (cnt.Children[ii - 1] is UnitextPlaintext) && ((ii + 1) < cnt.Children.Count)) && (cnt.Children[ii + 1] is UnitextPlaintext)) 
                            {
                                if (_isAllUpper(cnt.Children[ii - 1] as UnitextPlaintext) && !_isAllUpper(cnt.Children[ii + 1] as UnitextPlaintext)) 
                                    isDummy = false;
                            }
                        }
                        else if (Parent.Parent is UnitextDocblock) 
                        {
                            bool hasDb = false;
                            foreach (UnitextItem ch in (Parent as UnitextContainer).Children) 
                            {
                                if (ch is UnitextDocblock) 
                                    hasDb = true;
                            }
                            if (!hasDb) 
                            {
                            }
                        }
                    }
                }
            }
            if (isDummy) 
                res.Append(" ");
            else 
            {
                for (int i = 0; i < Count; i++) 
                {
                    res.Append("\r\n<BR/>");
                }
                if (par != null) 
                    par._outFootnotes(res);
            }
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("newline");
            this._writeXmlAttrs(xml);
            if (Count > 1) 
                xml.WriteAttributeString("count", Count.ToString());
            xml.WriteEndElement();
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
            if (xml.Attributes != null && xml.Attributes["count"] != null) 
                Count = int.Parse(xml.Attributes["count"].Value);
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
        internal override void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
            if (typ == Pullenti.Unitext.Internal.Uni.LocCorrTyp.MergeNewlines) 
            {
                if (Count == 2) 
                    Count = 1;
            }
        }
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = cp;
            cp += Count;
            EndChar = cp - 1;
            if (res != null) 
            {
                for (int i = 0; i < Count; i++) 
                {
                    res.Append('\n');
                }
            }
        }
    }
}