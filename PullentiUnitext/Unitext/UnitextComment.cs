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
    /// Примечание (аннотация). Оформляется двумя такими объектами - 
    /// для начальной позиции и конечной позиции.
    /// </summary>
    public class UnitextComment : UnitextItem
    {
        /// <summary>
        /// Текст комментария
        /// </summary>
        public string Text;
        /// <summary>
        /// Автор (если есть)
        /// </summary>
        public string Author;
        /// <summary>
        /// Ссылка на Id "братского" элемента (начала или окончания аннотации)
        /// </summary>
        public string TwinId;
        /// <summary>
        /// Признак того, что это указатель конечной позиции 
        /// (при false - начальная позиция, при true - конечная)
        /// </summary>
        public bool IsEndOfComment;
        public override UnitextItem Clone()
        {
            UnitextComment res = new UnitextComment();
            res._cloneFrom(this);
            res.Text = Text;
            res.Author = Author;
            res.TwinId = TwinId;
            return res;
        }
        public override string ToString()
        {
            return string.Format("Comment: {0}{1}", Text, (IsEndOfComment ? " (end)" : ""));
        }
        internal override string InnerTag
        {
            get
            {
                return "comment";
            }
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
            {
                BeginChar = (EndChar = res.Length);
                if (IsEndOfComment && res.Length > 0) 
                    BeginChar = (EndChar = res.Length - 1);
            }
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (par.HideEditionsAndComments) 
                return;
            if (!par.CallBefore(this, res)) 
                return;
            if (Text != null && !IsEndOfComment) 
            {
                if (par.OutCommentsWithDelTags) 
                {
                    res.Append("<del");
                    if (Id != null) 
                        res.AppendFormat(" id=\"{0}\"", Id);
                    res.Append(">");
                    Pullenti.Util.MiscHelper.CorrectHtmlValue(res, Text, false, false);
                    res.Append("</del>");
                }
                else 
                {
                    res.AppendFormat(" <span id=\"{0}\" style=\"font-size:smaller;font-style:italic;color:blue;background:lightyellow\">[<b>Комментарий: </b>", Id);
                    Pullenti.Util.MiscHelper.CorrectHtmlValue(res, Text, false, false);
                    res.Append("]</span>");
                }
            }
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("comment");
            this._writeXmlAttrs(xml);
            if (Author != null) 
                xml.WriteAttributeString("author", Pullenti.Util.MiscHelper.CorrectXmlValue(Author));
            if (IsEndOfComment) 
                xml.WriteAttributeString("end", "true");
            if (TwinId != null) 
                xml.WriteAttributeString("twin", TwinId);
            xml.WriteString(Pullenti.Util.MiscHelper.CorrectXmlValue(Text));
            xml.WriteEndElement();
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "author") 
                        Author = a.Value;
                    else if (a.LocalName == "twin") 
                        TwinId = a.Value;
                    else if (a.LocalName == "end") 
                        IsEndOfComment = a.Value == "true";
                }
            }
            Text = xml.InnerText;
        }
    }
}