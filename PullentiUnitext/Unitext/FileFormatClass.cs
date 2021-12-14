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
    /// Класс формата
    /// </summary>
    public enum FileFormatClass : int
    {
        /// <summary>
        /// Неопределён
        /// </summary>
        Undefined,
        /// <summary>
        /// Архивы (zip, rar ...)
        /// </summary>
        Archive,
        /// <summary>
        /// Картинки
        /// </summary>
        Image,
        /// <summary>
        /// Документы MS Office и Open Office
        /// </summary>
        Office,
        /// <summary>
        /// Тексто-графические (PDF, DjVu)
        /// </summary>
        Pagelayout,
    }
}