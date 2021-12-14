/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Util
{
    /// <summary>
    /// Подерживаемые стандартные кодировки для EncodingWrapper. Введены, чтобы избежать зависимости от 
    /// платформы и языка программирования.
    /// </summary>
    public enum EncodingStandard : int
    {
        /// <summary>
        /// Неизвестная
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Ascii
        /// </summary>
        ACSII,
        /// <summary>
        /// Utf-8, может быть префикс EF BB BF.
        /// </summary>
        UTF8,
        /// <summary>
        /// Utf-16, двухбайтовая, младший байт первый. Может быть префикс FF FE.
        /// </summary>
        UTF16LE,
        /// <summary>
        /// Utf-16, двухбайтовая, старшый байт первый (BigEndianUnicode). Может быть префикс FE FF.
        /// </summary>
        UTF16BE,
        /// <summary>
        /// Windows-1251
        /// </summary>
        CP1251,
        /// <summary>
        /// Windows-1252
        /// </summary>
        CP1252,
    }
}