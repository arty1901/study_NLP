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
    /// Фрагмент, с которым связаны стили UnitextStyle (если их выделение реализовано для входного формата). 
    /// Представляет собой иерархию.
    /// </summary>
    public class UnitextStyledFragment
    {
        /// <summary>
        /// Ссылка на родителя в иерархии
        /// </summary>
        public UnitextStyledFragment Parent;
        /// <summary>
        /// Дочерние фрагменты
        /// </summary>
        public List<UnitextStyledFragment> Children = new List<UnitextStyledFragment>();
        /// <summary>
        /// Ссылка на документ
        /// </summary>
        public UnitextDocument Doc
        {
            get
            {
                if (m_Doc != null) 
                    return m_Doc;
                if (Parent != null) 
                    return Parent.Doc;
                return null;
            }
            set
            {
                m_Doc = value;
            }
        }
        UnitextDocument m_Doc;
        /// <summary>
        /// Тип фрагмента
        /// </summary>
        public UnitextStyledFragmentType Typ = UnitextStyledFragmentType.Undefined;
        /// <summary>
        /// Идентификатор стиля (если -1, то явно для фрагмента не задан)
        /// </summary>
        public int StyleId = -1;
        /// <summary>
        /// Стиль фрагмента (если явно не задан, то берётся от родителя)
        /// </summary>
        public UnitextStyle Style
        {
            get
            {
                if (m_Style != null) 
                    return m_Style;
                UnitextDocument doc = Doc;
                if (doc == null || doc.Styles == null) 
                    return null;
                if (StyleId >= 0 && (StyleId < doc.Styles.Count)) 
                    return doc.Styles[StyleId];
                return null;
            }
            set
            {
                m_Style = value;
                if (value == null) 
                    StyleId = -1;
                else 
                    StyleId = value.Id;
            }
        }
        UnitextStyle m_Style;
        /// <summary>
        /// Начальная позиция в плоском тексте
        /// </summary>
        public int BeginChar = -1;
        /// <summary>
        /// Конечная позиция в плоском тексте
        /// </summary>
        public int EndChar = -1;
        /// <summary>
        /// Фрагмент текста. Внимание! Он может отличаться от сгенерированного плоского текста. 
        /// Это поле добавлено исключительно для наглядности при визуализации, но сами эти данные 
        /// не использовать при обработке.
        /// </summary>
        public string Text;
        /// <summary>
        /// Используйте произвольным образом (не сериализуется)
        /// </summary>
        public object Tag;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (Typ != UnitextStyledFragmentType.Undefined) 
            {
                res.AppendFormat("{0} ", Typ);
                res.AppendFormat("[{0}..{1}] ", BeginChar, EndChar);
            }
            if (Style != null) 
                res.AppendFormat("{0}", Style);
            if (Text != null) 
            {
                string txt = (Text.Length < 100 ? Text : (Text.Substring(0, 100) + "..."));
                res.AppendFormat(" \"{0}\"", txt.Replace("\n", "\\n"));
            }
            return res.ToString();
        }
        /// <summary>
        /// Найти значение атрибута стиля (от текущего вверх по иерархии, пока не найдём)
        /// </summary>
        /// <param name="name">имя атрибута</param>
        /// <param name="defvalue">значение, если не нашли</param>
        /// <return>значение</return>
        public string GetStyleAttr(string name, string defvalue = null)
        {
            for (UnitextStyledFragment fr = this; fr != null; fr = fr.Parent) 
            {
                if (fr.StyleId >= 0) 
                {
                    UnitextStyle sty = fr.Style;
                    if (sty != null) 
                    {
                        string res = sty.GetAttr(name);
                        if (res != null) 
                            return res;
                    }
                }
            }
            return defvalue;
        }
        public UnitextStyledFragment Clone()
        {
            UnitextStyledFragment res = new UnitextStyledFragment();
            res.Typ = Typ;
            res.StyleId = StyleId;
            res.Doc = Doc;
            res.Parent = Parent;
            res.BeginChar = BeginChar;
            res.EndChar = EndChar;
            res.Text = Text;
            foreach (UnitextStyledFragment ch in Children) 
            {
                UnitextStyledFragment chh = ch.Clone();
                chh.Parent = res;
                res.Children.Add(chh);
            }
            return res;
        }
        public void AddChild(UnitextStyledFragment ch)
        {
            ch.Parent = this;
            if (Children.Count > 0 && (ch.EndChar < Children[0].BeginChar)) 
            {
                Children.Insert(0, ch);
                return;
            }
            for (int i = 0; i < (Children.Count - 1); i++) 
            {
                if ((Children[i].EndChar < ch.BeginChar) && (ch.EndChar < Children[i + 1].BeginChar)) 
                {
                    Children.Insert(i + 1, ch);
                    return;
                }
            }
            Children.Add(ch);
        }
        public void GetXml(XmlWriter xml, string tag = null, bool outStyleFull = false)
        {
            xml.WriteStartElement(tag ?? "stylefrag");
            if (Typ != UnitextStyledFragmentType.Undefined) 
                xml.WriteAttributeString("typ", Typ.ToString().ToLower());
            if (StyleId >= 0) 
                xml.WriteAttributeString("style", StyleId.ToString());
            xml.WriteAttributeString("b", BeginChar.ToString());
            xml.WriteAttributeString("e", EndChar.ToString());
            if (StyleId >= 0 && Style != null) 
            {
                foreach (KeyValuePair<string, string> kp in Style.Attrs) 
                {
                    xml.WriteAttributeString(kp.Key, kp.Value);
                }
            }
            if (Text != null) 
                xml.WriteElementString("text", Pullenti.Util.MiscHelper.CorrectXmlValue(Text));
            if (!outStyleFull) 
            {
                foreach (UnitextStyledFragment ch in Children) 
                {
                    ch.GetXml(xml, null, false);
                }
            }
            xml.WriteEndElement();
        }
        public void FromXml(XmlNode xml)
        {
            foreach (XmlAttribute a in xml.Attributes) 
            {
                if (a.LocalName == "typ") 
                {
                    try 
                    {
                        Typ = (UnitextStyledFragmentType)Enum.Parse(typeof(UnitextStyledFragmentType), a.Value, true);
                    }
                    catch(Exception ex337) 
                    {
                    }
                }
                else if (a.LocalName == "style") 
                    StyleId = int.Parse(a.Value);
                else if (a.LocalName == "b") 
                    BeginChar = int.Parse(a.Value);
                else if (a.LocalName == "e") 
                    EndChar = int.Parse(a.Value);
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "stylefrag") 
                {
                    UnitextStyledFragment fr = new UnitextStyledFragment();
                    fr.Parent = this;
                    fr.FromXml(x);
                    Children.Add(fr);
                }
                else if (x.LocalName == "text") 
                    Text = x.InnerText;
            }
        }
        /// <summary>
        /// Найти самый мелкий в дереве фрагмент (удалённный от корня), 
        /// содержащий указанную позицию плоского текста.
        /// </summary>
        /// <param name="cp">позиция символа плоского текста</param>
        /// <return>фрагмент или null</return>
        public UnitextStyledFragment FindByCharPosition(int cp)
        {
            if ((cp < BeginChar) || cp > EndChar) 
                return null;
            if (Children.Count > 10) 
            {
                int i = Children.Count / 2;
                int d = Children.Count / 4;
                if (d == 0) 
                    d = 1;
                int k = d + 2;
                while (k > 0) 
                {
                    if (i >= Children.Count || (i < 0)) 
                        break;
                    UnitextStyledFragment ch = Children[i];
                    if (ch.BeginChar <= cp && cp <= ch.EndChar) 
                    {
                        UnitextStyledFragment res = ch.FindByCharPosition(cp);
                        if (res != null) 
                            return res;
                        return this;
                    }
                    if (ch.BeginChar < cp) 
                        i += d;
                    else if (ch.EndChar > cp) 
                        i -= d;
                    else 
                        i += d;
                    d /= 2;
                    if (d == 0) 
                    {
                        d = 1;
                        k--;
                    }
                }
            }
            foreach (UnitextStyledFragment ch in Children) 
            {
                if (ch.BeginChar <= cp && cp <= ch.EndChar) 
                {
                    UnitextStyledFragment res = ch.FindByCharPosition(cp);
                    if (res != null) 
                        return res;
                    return this;
                }
            }
            return this;
        }
    }
}