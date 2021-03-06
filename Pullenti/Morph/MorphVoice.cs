/*
 * SDK Pullenti Lingvo, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Morph
{
    /// <summary>
    /// Залог (для глаголов)
    /// </summary>
    public enum MorphVoice : short
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Действительный
        /// </summary>
        Active = 1,
        /// <summary>
        /// Страдательный
        /// </summary>
        Passive = 2,
        /// <summary>
        /// Средний
        /// </summary>
        Middle = 4,
    }
}