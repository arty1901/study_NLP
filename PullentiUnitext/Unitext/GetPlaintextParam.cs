/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Параметры генерации плоского текста функциями GetPlaintext и GetPlaintextString
    /// </summary>
    public class GetPlaintextParam
    {
        /// <summary>
        /// Устанавливать значения BeginChar и EndChar у элементов, по умолчанию true.
        /// </summary>
        public bool SetPositions = true;
        /// <summary>
        /// Переход на новую строку, по умолчанию "\r\n".
        /// </summary>
        public string NewLine = "\r\n";
        /// <summary>
        /// Табуляция, по умолчанию "\t".
        /// </summary>
        public string Tab = "\t";
        /// <summary>
        /// Разрыв страниц, по умолчанию "\f".
        /// </summary>
        public string PageBreak = "\f";
        /// <summary>
        /// Начало таблицы, по умолчанию "\x001e".
        /// </summary>
        public string TableStart = string.Format("{0}", (char)0x1E);
        /// <summary>
        /// Окончание таблицы, по умолчанию "\x001f".
        /// </summary>
        public string TableEnd = string.Format("{0}", (char)0x1F);
        /// <summary>
        /// Окончание ячейки таблицы, по умолчанию "\x0007".
        /// </summary>
        public string TableCellEnd = string.Format("{0}", (char)7);
        /// <summary>
        /// Окончание строки таблицы, по умолчанию "\x0007".
        /// </summary>
        public string TableRowEnd = string.Format("{0}", (char)7);
        /// <summary>
        /// Шаблон вывода сносок (текст сноски в шаблоне - это %1), по умолчанию " (%1)". 
        /// Если null, то не выводится.
        /// </summary>
        public string FootnotesTemplate = " (%1)";
        /// <summary>
        /// Шаблон вывода значений гиперссылок (Uri в шаблоне - это %1), 
        /// если null, то не выводится - по умолчанию. Гиперссылка также не выводится, если она совпадает 
        /// с фрагментом текста (то есть в тексте в явном виде).
        /// </summary>
        public string HyperlinksTemplate = null;
        /// <summary>
        /// Шаблон вывода участков текста, которые представлены в верхнем индексе (тип UnitextPlaintext.Typ = UnitextPlaintextType.Sup). 
        /// По умолчанию, "&lt;%1&gt;", если null, то просто текст.
        /// </summary>
        public string SupTemplate = "<%1>";
        /// <summary>
        /// Шаблон вывода участков текста, которые представлены в нижнем индексе (тип UnitextPlaintext.Typ = UnitextPlaintextType.Sub). 
        /// По умолчанию, "&lt;%1&gt;", если null, то просто текст.
        /// </summary>
        public string SubTemplate = "<%1>";
        /// <summary>
        /// Игнорировать текст в формах
        /// </summary>
        public bool IgnoreShapes = false;
        /// <summary>
        /// Ограничение на размер результирующего текста (0 - нет ограничения)
        /// </summary>
        public int MaxTextLength = 0;
        /// <summary>
        /// Генерировать ли текст для внутренних документов UnitextDocument.InnerDocuments, если они есть, по умолчанию true.
        /// </summary>
        public bool UseInnerDocuments = true;
    }
}