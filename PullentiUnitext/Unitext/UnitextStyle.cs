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
    /// Представление стилей. Реализовано пока только для формата DOCX.
    /// </summary>
    public class UnitextStyle
    {
        /// <summary>
        /// Значения атрибутов (как в CSS). Составные атрибуты не поддерживаются, то есть 
        /// нет font, но есть по отдельности font-name, font-size, font-weight и т.д.
        /// </summary>
        public Dictionary<string, string> Attrs = new Dictionary<string, string>();
        /// <summary>
        /// Внутренний идентификатор (0 - используется по умолчанию)
        /// </summary>
        public int Id;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            foreach (KeyValuePair<string, string> at in Attrs) 
            {
                if (tmp.Length > 0) 
                    tmp.Append(" ");
                tmp.AppendFormat("{0}:{1}", at.Key, at.Value);
            }
            return tmp.ToString();
        }
        /// <summary>
        /// Получить значение атрибута
        /// </summary>
        /// <param name="name">имя</param>
        /// <return>значение</return>
        public string GetAttr(string name)
        {
            string res;
            if (Attrs.TryGetValue(name, out res)) 
                return res;
            return null;
        }
        public void AddAttr(string name, string val, bool appendVal = false)
        {
            if (string.IsNullOrEmpty(val)) 
                return;
            string val0;
            if (!Attrs.TryGetValue(name, out val0)) 
                Attrs.Add(name, val);
            else if (!appendVal || val0 == null) 
                Attrs[name] = val;
            else if (val0.IndexOf(';') < 0) 
                Attrs[name] = string.Format("{0};{1}", val0, val);
            else 
            {
                string[] vals = val0.Split(';');
                for (int i = 0; i < vals.Length; i++) 
                {
                    if (vals[i] == val) 
                        return;
                }
                Attrs[name] = string.Format("{0};{1}", val0, val);
            }
        }
        public void RemoveInheritAttrs(UnitextStyledFragment fr)
        {
            List<string> dels = null;
            foreach (KeyValuePair<string, string> kp in Attrs) 
            {
                string val = null;
                for (UnitextStyledFragment p = fr; p != null; p = p.Parent) 
                {
                    if (p.StyleId >= 0 && p.Style != null) 
                    {
                        if ((((val = p.Style.GetAttr(kp.Key)))) != null) 
                            break;
                    }
                }
                if (val == null || val != kp.Value) 
                    continue;
                if (dels == null) 
                    dels = new List<string>();
                dels.Add(kp.Key);
            }
            if (dels != null) 
            {
                foreach (string k in dels) 
                {
                    Attrs.Remove(k);
                }
            }
        }
        public UnitextStyle Clone(bool ignoreBlockAttrs = false)
        {
            UnitextStyle res = new UnitextStyle();
            this.CopyTo(res);
            return res;
        }
        public void CopyTo(UnitextStyle res)
        {
            res.Id = Id;
            foreach (KeyValuePair<string, string> kp in Attrs) 
            {
                res.AddAttr(kp.Key, kp.Value, false);
            }
        }
        public void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("style");
            xml.WriteAttributeString("id", Id.ToString());
            foreach (KeyValuePair<string, string> kp in Attrs) 
            {
                xml.WriteAttributeString(kp.Key, kp.Value);
            }
            xml.WriteEndElement();
        }
        public void FromXml(XmlNode xml)
        {
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "id") 
                        Id = int.Parse(a.Value);
                    else 
                        this.AddAttr(a.LocalName, a.Value, false);
                }
            }
        }
        /// <summary>
        /// Вывести в Html значение атрибута style="..."
        /// </summary>
        public void GetHtml(StringBuilder res)
        {
            foreach (KeyValuePair<string, string> kp in Attrs) 
            {
                if (kp.Key == "heading-level") 
                    continue;
                res.AppendFormat("{0}:{1};", kp.Key, kp.Value);
            }
        }
    }
}