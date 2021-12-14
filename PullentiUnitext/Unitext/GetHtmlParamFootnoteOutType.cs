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
    /// Тип вывода сносок при генерации HTML
    /// </summary>
    public enum GetHtmlParamFootnoteOutType : int
    {
        /// <summary>
        /// Хинтом к звёздочке, саму сноску не отображать.
        /// </summary>
        AsHint = 0,
        /// <summary>
        /// В скобках в месте вставки (...)
        /// </summary>
        InBrackets = 1,
        /// <summary>
        /// В конце ближайшего нумерованного раздела или главы UnitextDocblock 
        /// (если структура документа выделена).
        /// </summary>
        EndOfUnit = 2,
    }
}