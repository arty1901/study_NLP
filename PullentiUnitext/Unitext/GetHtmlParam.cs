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

namespace Pullenti.Unitext
{
    /// <summary>
    /// Параметры генерации HTML функциями GetHtml и GetHtmlString
    /// </summary>
    public class GetHtmlParam
    {
        /// <summary>
        /// Дополнительные элементы стилей (ключ - Id элемента)
        /// </summary>
        public Dictionary<string, string> Styles = new Dictionary<string, string>();
        /// <summary>
        /// Генерировать ли для внутренних документов UnitextDocument.InnerDocuments, если они есть, по умолчанию true.
        /// </summary>
        public bool UseInnerDocuments = true;
        /// <summary>
        /// Как выводить сноски, по умолчанию, в конце параграфов (FootnoteManagement.EndOfUnit).
        /// </summary>
        public GetHtmlParamFootnoteOutType Footnotes = GetHtmlParamFootnoteOutType.EndOfUnit;
        /// <summary>
        /// Комментарии выводить в элементах del (иначе span)
        /// </summary>
        public bool OutCommentsWithDelTags = false;
        /// <summary>
        /// Выводить ли в HTML-теги значения BeginChar и EndChar в тегах bc=... ec=..., по умолчанию false.
        /// </summary>
        public bool OutBeginEndChars = false;
        /// <summary>
        /// Выводить ли стили UnitextStyle, оформляя их атрибутами style=...
        /// </summary>
        public bool OutStyles = true;
        /// <summary>
        /// Не выводить блоки редакций и комментариев (Container с типом Edition или Comment)
        /// </summary>
        public bool HideEditionsAndComments = false;
        /// <summary>
        /// Ставить для гиперссылок target="_blank", по умолчанию false.
        /// </summary>
        public bool HyperlinksTargetBlank = false;
        /// <summary>
        /// Вставлять в начало и конец теги HTML и BODY для формирования полноценного HTML, по умолчанию false.
        /// </summary>
        public bool OutHtmlAndBodyTags = false;
        /// <summary>
        /// Если формируется полноценный HTML, то в тег title вставить этот текст
        /// </summary>
        public string Title;
        /// <summary>
        /// Использовать произвольным образом
        /// </summary>
        public object Tag;
        /// <summary>
        /// Вызывается в самом начале перед генерацией для элемента
        /// </summary>
        /// <param name="it">текущий элемент</param>
        /// <param name="res">для результата</param>
        /// <return>если false, то сразу выход из функции - генерация для элемета не производится</return>
        public virtual bool CallBefore(UnitextItem it, StringBuilder res)
        {
            return true;
        }
        /// <summary>
        /// Вызывается в самом конце после генерации Html для элемента
        /// </summary>
        /// <param name="it">текущий элемент</param>
        /// <param name="res">для результата</param>
        public virtual void CallAfter(UnitextItem it, StringBuilder res)
        {
        }
        // Используется внутренним образом
        internal List<UnitextFootnote> m_Footnotes = new List<UnitextFootnote>();
        internal List<UnitextFootnote> m_Endnotes = new List<UnitextFootnote>();
        internal List<UnitextDocblock> m_FootnotesDb = new List<UnitextDocblock>();
        bool m_InFootregime = false;
        internal void _outEndnotes(StringBuilder res)
        {
            if (m_InFootregime) 
                return;
            m_InFootregime = true;
            if (m_Endnotes.Count > 0) 
            {
                res.Append("\r\n<hr/>");
                res.AppendFormat("\r\n<div style=\"border-left:2pt solid green;padding-left:5pt;margin-left:30pt;margin-top:10pt;margin-bottom:10pt;font-size:smaller;text-align:left;font-weight:normal\">");
                for (int i = 0; i < m_Endnotes.Count; i++) 
                {
                    if (i > 0) 
                        res.Append("<br/>");
                    res.AppendFormat("\r\n<b id=\"{0}\"><sup>E{1}</sup></b>", m_Endnotes[i].Id ?? "", i + 1);
                    if (m_Endnotes[i].Content != null) 
                        m_Endnotes[i].Content.GetHtml(res, this);
                }
                res.Append("\r\n</div>");
                m_Endnotes.Clear();
            }
            m_InFootregime = false;
        }
        // Не вызывать (это используется при специфических генерациях для вывода сносок)
        public void _outFootnotes(StringBuilder res)
        {
            if (m_InFootregime) 
                return;
            m_InFootregime = true;
            if (m_Footnotes.Count > 0) 
            {
                res.AppendFormat("\r\n<div style=\"border-left:2pt solid green;padding-left:5pt;border-left:2pt;margin-left:30pt;margin-top:10pt;margin-bottom:10pt;font-size:smaller;text-align:left;font-weight:normal\">");
                for (int i = 0; i < m_Footnotes.Count; i++) 
                {
                    if (i > 0) 
                        res.Append("<br/>");
                    StringBuilder tmp = new StringBuilder();
                    if (m_Footnotes[i].Content != null) 
                        m_Footnotes[i].Content.GetHtml(tmp, this);
                    string sss = tmp.ToString();
                    if (sss.StartsWith("<div")) 
                    {
                        int j = sss.IndexOf('>');
                        tmp.Insert(j + 1, string.Format("<b id=\"{0}\"><sup>{1}</sup></b>", m_Footnotes[i].Id ?? "", i + 1));
                        res.Append("\r\n");
                        res.Append(tmp.ToString());
                    }
                    else 
                    {
                        res.AppendFormat("\r\n<b id=\"{0}\"><sup>{1}</sup></b>", m_Footnotes[i].Id ?? "", i + 1);
                        res.Append(sss);
                    }
                }
                res.Append("\r\n</div>");
                m_Footnotes.Clear();
            }
            if (m_FootnotesDb.Count > 0) 
            {
                if (m_FootnotesDb.Count > 1) 
                {
                }
                res.AppendFormat("\r\n<div style=\"border-left:2pt solid green;padding-left:5pt;margin-left:30pt;margin-top:10pt;margin-bottom:10pt;font-size:smaller;text-align:left;font-weight:normal\">");
                for (int i = 0; i < m_FootnotesDb.Count; i++) 
                {
                    UnitextDocblock ff = m_FootnotesDb[i];
                    res.AppendFormat("\r\n  <div id=\"{0}\">", ff.Id ?? "?");
                    if (ff.Head != null) 
                        ff.Head.GetHtml(res, this);
                    if (ff.Body != null) 
                        ff.Body.GetHtml(res, this);
                    res.Append("</div>");
                }
                res.Append("\r\n</div>");
                m_FootnotesDb.Clear();
            }
            m_InFootregime = false;
        }
    }
}