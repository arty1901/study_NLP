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
    /// Прямоугольник тексто-графического слоя
    /// </summary>
    public class UnilayRectangle
    {
        /// <summary>
        /// Ссылка на страницу, начало координат в левом верхнем углу
        /// </summary>
        public UnilayPage Page;
        /// <summary>
        /// Отступ слева
        /// </summary>
        public double Left;
        /// <summary>
        /// Отступ сверху
        /// </summary>
        public double Top;
        /// <summary>
        /// Отступ слева правой границы
        /// </summary>
        public double Right;
        /// <summary>
        /// Отступ сверху нижней границы
        /// </summary>
        public double Bottom;
        /// <summary>
        /// Текст в прямоугольнике (если есть)
        /// </summary>
        public string Text;
        /// <summary>
        /// Картинка (если есть)
        /// </summary>
        public byte[] ImageContent;
        /// <summary>
        /// Этот признак выставляется для текстовых фрагментов, подлежащих игнорированию. 
        /// Например, повторяющийся на каждой странице текст колонтитулов или нумерация страниц.
        /// </summary>
        public bool Ignored;
        /// <summary>
        /// Номер линии (от 1), нумерация условна, для многоколоночных текстов нумерация сквозная
        /// </summary>
        public int LineNumber;
        // Качество распознавания (устанавливается после применения OCR)
        public UnilayOcrQuality Quality = UnilayOcrQuality.Undefined;
        /// <summary>
        /// Используется произвольным образом
        /// </summary>
        public object Tag;
        public override string ToString()
        {
            string res = string.Format("{0}..{1} x {2}..{3}", Math.Round(Left, 2), Math.Round(Right, 2), Math.Round(Top, 2), Math.Round(Bottom, 2));
            if (Text != null) 
                return string.Format("{0}: {1}", res, Text);
            if (ImageContent != null) 
                return string.Format("{0}: Image", res);
            return res;
        }
        public void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("rect");
            if (LineNumber > 0) 
                xml.WriteAttributeString("line", LineNumber.ToString());
            xml.WriteAttributeString("left", Pullenti.Util.MiscHelper.OutDouble(Left));
            xml.WriteAttributeString("top", Pullenti.Util.MiscHelper.OutDouble(Top));
            xml.WriteAttributeString("right", Pullenti.Util.MiscHelper.OutDouble(Right));
            xml.WriteAttributeString("bottom", Pullenti.Util.MiscHelper.OutDouble(Bottom));
            if (Quality != UnilayOcrQuality.Undefined) 
                xml.WriteAttributeString("q", Quality.ToString().ToLower());
            if (ImageContent != null) 
                xml.WriteElementString("image", Convert.ToBase64String(ImageContent));
            else if (Text != null) 
                xml.WriteElementString("text", Pullenti.Util.MiscHelper.CorrectXmlValue(Text) ?? "");
            xml.WriteEndElement();
        }
        public void Restore(XmlNode xml)
        {
            double d;
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "line") 
                    {
                        int i;
                        if (int.TryParse(a.Value, out i)) 
                            LineNumber = i;
                    }
                    else if (a.LocalName == "left") 
                    {
                        if (Pullenti.Util.MiscHelper.TryParseDouble(a.Value, out d)) 
                            Left = d;
                    }
                    else if (a.LocalName == "top") 
                    {
                        if (Pullenti.Util.MiscHelper.TryParseDouble(a.Value, out d)) 
                            Top = d;
                    }
                    else if (a.LocalName == "right") 
                    {
                        if (Pullenti.Util.MiscHelper.TryParseDouble(a.Value, out d)) 
                            Right = d;
                    }
                    else if (a.LocalName == "bottom") 
                    {
                        if (Pullenti.Util.MiscHelper.TryParseDouble(a.Value, out d)) 
                            Bottom = d;
                    }
                    else if (a.LocalName == "q") 
                    {
                        try 
                        {
                            Quality = (UnilayOcrQuality)Enum.Parse(typeof(UnilayOcrQuality), a.Value, true);
                        }
                        catch(Exception ex314) 
                        {
                        }
                    }
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "text") 
                    Text = x.InnerText;
                else if (x.LocalName == "image") 
                    ImageContent = Convert.FromBase64String(x.InnerText);
            }
        }
    }
}