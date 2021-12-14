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
    /// Тип контейнера элементов
    /// </summary>
    public enum UnitextContainerType : int
    {
        /// <summary>
        /// Обычный контейнер
        /// </summary>
        Undefined,
        /// <summary>
        /// Контейнер является полотном с визуальными формами 
        /// (прямоугольник, форма, круг ...), которые содержат тексты
        /// </summary>
        Shape,
        /// <summary>
        /// Это для MSWord элемент управления содержимым
        /// </summary>
        ContentControl,
        /// <summary>
        /// Содержимое рекомендуется выводиться моноширинным шрифтом, 
        /// так как содержит разную псевдографику
        /// </summary>
        Monospace,
        /// <summary>
        /// Временная подсветка
        /// </summary>
        Highlighting,
        /// <summary>
        /// Ключевое слово (например, Статья или Глава)
        /// </summary>
        Keyword,
        /// <summary>
        /// Нумерация раздела (блока)
        /// </summary>
        Number,
        /// <summary>
        /// Наименование раздела
        /// </summary>
        Name,
        /// <summary>
        /// Указания редакций (например,  п.1 в ред. ФЗ-123)
        /// </summary>
        Edition,
        /// <summary>
        /// Это некоторый внешний комментарий
        /// </summary>
        Comment,
        /// <summary>
        /// Это ключевое слово типа "Приказываю:" ...
        /// </summary>
        Directive,
        /// <summary>
        /// Заголовок блока текста (см. UnitextDocblock)
        /// </summary>
        Head,
        /// <summary>
        /// Подпись блока текста (см. UnitextDocblock)
        /// </summary>
        Tail,
        /// <summary>
        /// Это некоторый участок, выравниваемый вправо 
        /// (например, в записке в заголовке - кому и от кого)
        /// </summary>
        RightAlign,
        /// <summary>
        /// Некоторая сущность
        /// </summary>
        Entity,
        /// <summary>
        /// Некоторый пользовательский тип
        /// </summary>
        UserType,
    }
}