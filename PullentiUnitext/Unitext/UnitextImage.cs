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
    /// Картинка
    /// </summary>
    public class UnitextImage : UnitextItem
    {
        /// <summary>
        /// Содержимое картинки (может быть null)
        /// </summary>
        public byte[] Content;
        /// <summary>
        /// Ширина (в единицах исходного файла)
        /// </summary>
        public string Width;
        /// <summary>
        /// Высота (в единицах исходного файла)
        /// </summary>
        public string Height;
        /// <summary>
        /// Ссылка на слой тексто-графического представления (если есть)
        /// </summary>
        public UnilayRectangle Rect;
        /// <summary>
        /// Это извне устанавливаемый URI на картинку (для функции GetHtml)
        /// </summary>
        public string HtmlSrcUri;
        public override UnitextItem Clone()
        {
            UnitextImage res = new UnitextImage();
            res._cloneFrom(this);
            res.Content = Content;
            res.Width = Width;
            res.Height = Height;
            res.Rect = Rect;
            res.HtmlSrcUri = HtmlSrcUri;
            return res;
        }
        internal override string InnerTag
        {
            get
            {
                return "img";
            }
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append("Image");
            if (Width != null || Height != null) 
                tmp.AppendFormat(" {0} x {1}", Width ?? "?", Height ?? "?");
            if (Content != null) 
            {
                FileFormat frm = Pullenti.Util.FileFormatsHelper.AnalizeFormat(null, Content);
                if (frm != FileFormat.Unknown) 
                    tmp.AppendFormat(" {0}", frm.ToString().ToUpper());
                tmp.AppendFormat(" {0}bytes", Content.Length);
            }
            else if (HtmlSrcUri != null) 
                tmp.AppendFormat(" {0}", HtmlSrcUri);
            return tmp.ToString();
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            res.Append(' ');
            if (pars != null && pars.SetPositions) 
                EndChar = BeginChar;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            res.AppendFormat("<img");
            if (Id != null) 
                res.AppendFormat(" id=\"{0}\"", Id);
            res.AppendFormat(" style=\"max-width:100%;");
            if (Content == null || Content.Length > 10000000) 
                res.Append("border:2pt solid red;");
            int wi = 0;
            int hi = 0;
            if (Width != null) 
            {
                for (int i = 0; i < Width.Length; i++) 
                {
                    if (!char.IsDigit(Width[i])) 
                        break;
                    else 
                        wi = int.Parse(Width.Substring(0, i + 1));
                }
            }
            if (Height != null) 
            {
                for (int i = 0; i < Height.Length; i++) 
                {
                    if (!char.IsDigit(Height[i])) 
                        break;
                    else 
                        hi = int.Parse(Height.Substring(0, i + 1));
                }
            }
            if ((wi < 500) && (hi < 500)) 
            {
                if (Width != null) 
                    res.AppendFormat("width:{0};", Width);
                if (Height != null) 
                    res.AppendFormat("height:{0};", Height);
            }
            res.Append("\"");
            if (Content != null) 
            {
                if (Content.Length > 10000000) 
                    res.AppendFormat(" alt=\"Image too large to show here ({0}Kb)\"", Content.Length / 1024);
                else 
                {
                    FileFormat frm = Pullenti.Util.FileFormatsHelper.AnalizeFormat(null, Content);
                    if (frm == FileFormat.Jpg2000) 
                        res.AppendFormat(" title=\"Image format JPEG 2000 not supported\"");
                    else 
                    {
                        string base64 = Convert.ToBase64String(Content);
                        if (base64.IndexOf('\n') >= 0) 
                            base64 = base64.Replace("\n", "");
                        string str = null;
                        if (frm != FileFormat.Unknown) 
                        {
                            str = Pullenti.Util.FileFormatsHelper.GetFormatExt(frm);
                            if (str.StartsWith(".")) 
                                str = str.Substring(1);
                        }
                        if (str == null) 
                            str = "png";
                        if (str == "tif") 
                            str = "tiff";
                        string src = string.Format("data:image/{0};base64,", str) + base64;
                        res.AppendFormat(" src=\"{0}\"", src);
                    }
                }
            }
            else 
                res.AppendFormat(" src=\"{0}\"", HtmlSrcUri ?? "undefined");
            res.Append("/>");
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("image");
            this._writeXmlAttrs(xml);
            if (Width != null) 
                xml.WriteAttributeString("width", Width);
            if (Height != null) 
                xml.WriteAttributeString("height", Height);
            if (HtmlSrcUri != null) 
                xml.WriteAttributeString("uri", Pullenti.Util.MiscHelper.CorrectXmlValue(HtmlSrcUri));
            if (Content != null) 
            {
                try 
                {
                    string dat = Convert.ToBase64String(Content);
                    StringBuilder tmp = new StringBuilder(dat.Length + 100);
                    int i = 0;
                    foreach (char ch in dat) 
                    {
                        if (char.IsWhiteSpace(ch)) 
                            continue;
                        tmp.Append(ch);
                        if ((++i) >= 100) 
                        {
                            i = 0;
                            tmp.Append("\r\n");
                        }
                    }
                    xml.WriteString(tmp.ToString());
                }
                catch(Exception ex) 
                {
                    GC.Collect();
                }
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
                    if (a.LocalName == "width") 
                        Width = a.Value;
                    else if (a.LocalName == "height") 
                        Height = a.Value;
                    else if (a.LocalName == "uri") 
                        HtmlSrcUri = a.Value;
                }
            }
            try 
            {
                string txt = xml.InnerText;
                if (txt != null) 
                    Content = Convert.FromBase64String(txt);
            }
            catch(Exception ex332) 
            {
            }
        }
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = (EndChar = cp);
            cp++;
            if (res != null) 
                res.Append(' ');
        }
    }
}