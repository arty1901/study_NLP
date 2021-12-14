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
    /// Типы стилевых фрагментов UnitextStyledFragment
    /// </summary>
    public enum UnitextStyledFragmentType : int
    {
        /// <summary>
        /// Не определён
        /// </summary>
        Undefined,
        /// <summary>
        /// Линейный фрагмент (например, слово)
        /// </summary>
        Inline,
        /// <summary>
        /// Абзац (аналог параграфа MS Word)
        /// </summary>
        Paragraph,
        /// <summary>
        /// Таблица
        /// </summary>
        Table,
        /// <summary>
        /// Ячейка таблицы
        /// </summary>
        Tablecell,
        /// <summary>
        /// Сноска
        /// </summary>
        Footnote,
    }
}