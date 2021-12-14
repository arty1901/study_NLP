﻿/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Тип плоского текста
    /// </summary>
    public enum UnitextPlaintextType : int
    {
        /// <summary>
        /// Обычный текст
        /// </summary>
        Simple,
        /// <summary>
        /// Верхний индекс
        /// </summary>
        Sup,
        /// <summary>
        /// Нижний индекс
        /// </summary>
        Sub,
        /// <summary>
        /// Текст сгенерирован (на основе автонумерации, например)
        /// </summary>
        Generated,
    }
}