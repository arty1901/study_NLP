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
    /// Базовый класс для всех элементов Unitext: 
    /// UnitextPlaintext, UnitextContainer, UnitextTable, UnitextTablecell, UnitextList, UnitextListitem, 
    /// UnitextNewline, UnitextPagebreak, UnitextFootnote, UnitextImage, UnitextHyperlink, UnitextComment, 
    /// UnitextDocblock и UnitextMisc.
    /// </summary>
    public class UnitextItem
    {
        /// <summary>
        /// Используется произвольным образом
        /// </summary>
        public object Tag;
        /// <summary>
        /// Начальная позиция в плоском тексте (формируется только после вызова 
        /// функции GetPlaintext, причём с SetPositions = true)
        /// </summary>
        public int BeginChar;
        /// <summary>
        /// Конечная позиция в плоском тексте (формируется только после вызова 
        /// функции GetPlaintext, причём с SetPositions = true) 
        /// Если EndChar = BeginChar - 1, то элемент занимает нулевую длину.
        /// </summary>
        public int EndChar;
        // Получить первую непробельную позицию
        internal int GetPlaintextNsPos(string txt)
        {
            if (txt != null) 
            {
                for (int p = BeginChar; p <= EndChar && (p < txt.Length); p++) 
                {
                    if (!char.IsWhiteSpace(txt[p])) 
                    {
                        if (txt[p] != ((char)7) && txt[p] != ((char)0x1E) && txt[p] != ((char)0x1F)) 
                            return p;
                    }
                }
            }
            return BeginChar;
        }
        // Получить последнюю непробельную позицию
        internal int GetPlaintextNsPos1(string txt)
        {
            if (txt != null) 
            {
                for (int p = EndChar; p >= 0 && (p < txt.Length); p--) 
                {
                    if (!char.IsWhiteSpace(txt[p])) 
                    {
                        if (txt[p] != ((char)7) && txt[p] != ((char)0x1E) && txt[p] != ((char)0x1F)) 
                            return p;
                    }
                }
            }
            return EndChar;
        }
        /// <summary>
        /// Ссылка на родительский элемент вверх по иерархии
        /// </summary>
        public UnitextItem Parent;
        /// <summary>
        /// Идентификатор секции (см. Section), если они поддержаны для формата
        /// </summary>
        public string PageSectionId;
        /// <summary>
        /// Страничная секция (параметры страницы и колонтитулы)
        /// </summary>
        public UnitextPagesection PageSection
        {
            get
            {
                string id = null;
                for (UnitextItem it = this; it != null; it = it.Parent) 
                {
                    if (it.PageSectionId != null && id == null) 
                        id = it.PageSectionId;
                    if (it is UnitextPagesection) 
                        return it as UnitextPagesection;
                    UnitextDocument doc = it as UnitextDocument;
                    if (doc != null) 
                    {
                        foreach (UnitextPagesection s in doc.Sections) 
                        {
                            if (s.Id == id) 
                                return s;
                        }
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Это некоторая дополнительная информация из исходного документа. 
        /// Сейчас реализовано для Excel (см. UnitextExcelSourceInfo)
        /// </summary>
        public object SourceInfo = null;
        public virtual UnitextItem Clone()
        {
            return null;
        }
        protected void _cloneFrom(UnitextItem it)
        {
            BeginChar = it.BeginChar;
            EndChar = it.EndChar;
            SourceInfo = it.SourceInfo;
            HtmlTitle = it.HtmlTitle;
            Id = it.Id;
            ExtData = it.ExtData;
            PageSectionId = it.PageSectionId;
        }
        /// <summary>
        /// Сгенерировать плоский текст
        /// </summary>
        /// <param name="res">результат запишет сюда</param>
        /// <param name="pars">параметры</param>
        public virtual void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
        }
        /// <summary>
        /// Сгенерировать плоский текст текущего элемента и всех его подэлементов
        /// </summary>
        /// <param name="pars">параметры генерации</param>
        /// <return>результат</return>
        public virtual string GetPlaintextString(GetPlaintextParam pars = null)
        {
            return Pullenti.Unitext.Internal.Uni.UnitextHelper.GetPlaintext(this);
        }
        /// <summary>
        /// Уникальный идентификатор элемента внутри документа 
        /// Если не null, то при генерации Html добавляется &lt;a name=Anchor > 
        /// для возможной навигации на этот элемент
        /// </summary>
        public string Id;
        /// <summary>
        /// Это устанавливайте для генерации хинтов в HTML
        /// </summary>
        public string HtmlTitle;
        /// <summary>
        /// Некоторые внешние данные, ассоциированные с элементом. Не сериализуется. 
        /// Устанавливается и используется в конечных приложениях произвольным образом.
        /// </summary>
        public object ExtData;
        /// <summary>
        /// Сгенерировать HTML
        /// </summary>
        /// <param name="res">результат</param>
        /// <param name="par">параметры генерации</param>
        public virtual void GetHtml(StringBuilder res, GetHtmlParam par)
        {
        }
        /// <summary>
        /// Сгенерировать HTML
        /// </summary>
        /// <param name="par">параметры генерации</param>
        /// <return>результат</return>
        public virtual string GetHtmlString(GetHtmlParam par = null)
        {
            StringBuilder res = new StringBuilder();
            this.GetHtml(res, par);
            return res.ToString();
        }
        public virtual void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("unknown");
            xml.WriteEndElement();
        }
        protected void _writeXmlAttrs(XmlWriter xml)
        {
            if (Id != null) 
                xml.WriteAttributeString("id", Pullenti.Util.MiscHelper.CorrectXmlValue(Id));
            if (BeginChar > 0 || EndChar > 0) 
            {
                xml.WriteAttributeString("b", BeginChar.ToString());
                xml.WriteAttributeString("e", EndChar.ToString());
            }
            if (HtmlTitle != null) 
                xml.WriteAttributeString("title", Pullenti.Util.MiscHelper.CorrectXmlValue(HtmlTitle));
            if (PageSectionId != null) 
                xml.WriteAttributeString("section", PageSectionId);
        }
        public virtual void FromXml(XmlNode xml)
        {
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "id") 
                        Id = a.Value;
                    else if (a.LocalName == "b") 
                        BeginChar = int.Parse(a.Value);
                    else if (a.LocalName == "e") 
                        EndChar = int.Parse(a.Value);
                    else if (a.LocalName == "l") 
                        EndChar = (BeginChar + int.Parse(a.Value)) - 1;
                    else if (a.LocalName == "t" || a.LocalName == "title") 
                        HtmlTitle = a.Value;
                    else if (a.LocalName == "section") 
                        PageSectionId = a.Value;
                }
            }
        }
        public virtual UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            return this;
        }
        /// <summary>
        /// Получить список всех элементов (этот и все нижележащий)
        /// </summary>
        public virtual void GetAllItems(List<UnitextItem> res, int lev)
        {
            if (lev > 40) 
            {
            }
            if (res != null) 
                res.Add(this);
        }
        /// <summary>
        /// Только из "пустых" символов и переходов на новую строку
        /// </summary>
        public virtual bool IsWhitespaces
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Объект не содержит блочных объектов и разрывов строк
        /// </summary>
        public virtual bool IsInline
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        protected static GetPlaintextParam m_DefParams = new GetPlaintextParam();
        internal virtual void AddPlainTextPos(int d)
        {
            BeginChar += d;
            EndChar += d;
        }
        internal virtual void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
        }
        // Используется внутренним образом
        internal virtual string InnerTag
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Поиск среди текущего элемента и его внутренних элементов
        /// </summary>
        /// <param name="id">идентификатор для поиска</param>
        /// <return>результат</return>
        public virtual UnitextItem FindById(string id)
        {
            if (Id == id) 
                return this;
            return null;
        }
        internal virtual bool ReplaceChild(UnitextItem old, UnitextItem ne)
        {
            return false;
        }
        internal virtual void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = cp;
            EndChar = cp - 1;
        }
        /// <summary>
        /// Получить ссылку на стилевой фрагмент для указанной текстовой позиции. 
        /// Отметим, что для элемента BeginChar..EndChar может покрывать или пересекаться 
        /// с несколькими стилевыми фрагментами.
        /// </summary>
        /// <param name="textPos">позиция текста в системе BeginChar..EndChar (-1 - для первой позиции текущего элемента)</param>
        /// <return>фрагмент, наиболее удалённый в иерархии фрагментов и содержащий указанную позицию</return>
        public virtual UnitextStyledFragment GetStyledFragment(int textPos = -1)
        {
            if ((textPos < 0) && m_StyledFrag != null) 
                return m_StyledFrag;
            if (textPos < 0) 
                textPos = BeginChar;
            for (UnitextItem it = this; it != null; it = it.Parent) 
            {
                if (it.m_StyledFrag != null) 
                {
                    UnitextStyledFragment res = it.m_StyledFrag.FindByCharPosition(textPos);
                    if (res != null) 
                    {
                        if (m_StyledFrag == null) 
                            m_StyledFrag = res;
                        return res;
                    }
                }
            }
            return null;
        }
        internal UnitextStyledFragment m_StyledFrag;
    }
}