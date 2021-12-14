/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Страница тексто-графического слоя
    /// </summary>
    public class UnilayPage
    {
        /// <summary>
        /// Порядковый номер (от 1)
        /// </summary>
        public int Number;
        /// <summary>
        /// Ширина в условных единицах
        /// </summary>
        public int Width;
        /// <summary>
        /// Высота в условных единицах
        /// </summary>
        public int Height;
        /// <summary>
        /// Содержимое картинки всей страницы (если есть)
        /// </summary>
        public byte[] ImageContent;
        /// <summary>
        /// Прямоугольники с текстами и картинками - список UnilayRectangle.
        /// </summary>
        public List<UnilayRectangle> Rects = new List<UnilayRectangle>();
        /// <summary>
        /// Номер с верхнего колонтитула (если есть)
        /// </summary>
        public int TopNumber;
        /// <summary>
        /// Номер с нижнего колонтитула (если есть)
        /// </summary>
        public int BottomNumber;
        /// <summary>
        /// Используется произвольным образом
        /// </summary>
        public object Tag;
        public override string ToString()
        {
            return string.Format("Page {0} ({1}x{2}){3}", Number, Width, Height, (ImageContent == null ? "" : " has image"));
        }
        public void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("page");
            if (Number > 0) 
                xml.WriteAttributeString("number", Number.ToString());
            if (TopNumber > 0) 
                xml.WriteAttributeString("topnumber", TopNumber.ToString());
            if (BottomNumber > 0) 
                xml.WriteAttributeString("bottomnumber", BottomNumber.ToString());
            if (Width > 0) 
                xml.WriteAttributeString("width", Width.ToString());
            if (Height > 0) 
                xml.WriteAttributeString("height", Height.ToString());
            if (ImageContent != null) 
                xml.WriteElementString("image", Convert.ToBase64String(ImageContent));
            foreach (UnilayRectangle r in Rects) 
            {
                r.GetXml(xml);
            }
            xml.WriteEndElement();
        }
        public void Restore(XmlNode xml)
        {
            double d;
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "number") 
                    {
                        int i;
                        if (int.TryParse(a.Value, out i)) 
                            Number = i;
                    }
                    else if (a.LocalName == "topnumber") 
                    {
                        int i;
                        if (int.TryParse(a.Value, out i)) 
                            TopNumber = i;
                    }
                    else if (a.LocalName == "bottomnumber") 
                    {
                        int i;
                        if (int.TryParse(a.Value, out i)) 
                            BottomNumber = i;
                    }
                    else if (a.LocalName == "width") 
                    {
                        int i;
                        if (int.TryParse(a.Value, out i)) 
                            Width = i;
                    }
                    else if (a.LocalName == "height") 
                    {
                        int i;
                        if (int.TryParse(a.Value, out i)) 
                            Height = i;
                    }
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "rect") 
                {
                    UnilayRectangle r = new UnilayRectangle() { Page = this };
                    r.Restore(x);
                    Rects.Add(r);
                }
                else if (x.LocalName == "image") 
                    ImageContent = Convert.FromBase64String(x.InnerText);
            }
        }
        /// <summary>
        /// Найти по координате накрывающий её прямоугольник
        /// </summary>
        public UnilayRectangle FindRect(double x, double y)
        {
            foreach (UnilayRectangle r in Rects) 
            {
                if ((r.Left <= x && x <= r.Right && r.Top <= y) && y <= r.Bottom) 
                    return r;
            }
            return null;
        }
        // Объединить результат распознавания с распознаванием страницы другим движком,
        // взяв наилучшие Rects по их качеству Quality.
        // page - страница с таким же изображением</param>
        public void MergeRectsByQuality(UnilayPage page)
        {
            foreach (UnilayRectangle r in page.Rects) 
            {
                r.Tag = null;
            }
            int maxLine = 0;
            foreach (UnilayRectangle r in Rects) 
            {
                if (r.LineNumber > maxLine) 
                    maxLine = r.LineNumber;
                UnilayRectangle rr = page.FindRect(((r.Left + r.Right)) / 2, ((r.Top + r.Bottom)) / 2);
                if (rr == null) 
                    continue;
                rr.Tag = r;
                if (((int)r.Quality) >= ((int)rr.Quality)) 
                    continue;
                r.Text = rr.Text;
                r.Quality = rr.Quality;
                r.Left = rr.Left;
                r.Top = rr.Top;
                r.Right = rr.Right;
                r.Bottom = rr.Bottom;
            }
        }
    }
}