/*
 * SDK Pullenti Lingvo, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Person
{
    /// <summary>
    /// Категории свойств персон
    /// </summary>
    public enum PersonPropertyKind : int
    {
        /// <summary>
        /// Неопределена
        /// </summary>
        Undefined,
        /// <summary>
        /// Начальник
        /// </summary>
        Boss,
        /// <summary>
        /// Вельможные и духовные особы
        /// </summary>
        King,
        /// <summary>
        /// Родственники
        /// </summary>
        Kin,
        /// <summary>
        /// Воинское звание
        /// </summary>
        MilitaryRank,
        /// <summary>
        /// Национальность
        /// </summary>
        Nationality,
    }
}