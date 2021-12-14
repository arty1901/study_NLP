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
    /// Параметры корректировки (оптимизации) представления в параметрах создания CreateDocumentParam.CorrextParams.
    /// </summary>
    public class CorrectDocumentParam
    {
        /// <summary>
        /// Если есть разбивка на страницы, при этом идут нумерация или колонтитулы вверху или внизу страницы, 
        /// то они удаляются из документа. По умолчанию, true.
        /// </summary>
        public bool RemovePageBreakNumeration = true;
        /// <summary>
        /// Если документ принудительно отформатирован так, что переходы на новую строку 
        /// фактически не являтся окончаниями параграфов, то эта ситуация будет определена и 
        /// соответствующие переходы будут заменены на пробелы. По умолчанию, true.
        /// </summary>
        public bool RemoveFalseNewLines = true;
        /// <summary>
        /// Восстановить сноски, которые оформляются прямо в тексте в угловых скобках, 
        /// а сама сноска чуть ниже по тексту. По умолчанию, true.
        /// </summary>
        public bool RestoreTextFootnotes = true;
        /// <summary>
        /// Восстановить таблицы, которые в тексте оформляются псевдографикой. По умолчанию, true.
        /// </summary>
        public bool RestoreTables = true;
        /// <summary>
        /// Заменять неразрывные пробелы на обычные пробелы. По умолчанию, true.
        /// </summary>
        public bool ReplaceNbspBySpace = true;
        /// <summary>
        /// Оптимизировать структуру, удалив лишние элементы
        /// </summary>
        public bool OptimizeStructure = true;
        /// <summary>
        /// Установить все опции в true или false.
        /// </summary>
        public bool ChooseAll
        {
            get
            {
                return ((RemovePageBreakNumeration & RemoveFalseNewLines & RestoreTextFootnotes) & RestoreTables & ReplaceNbspBySpace) & OptimizeStructure;
            }
            set
            {
                RemovePageBreakNumeration = value;
                RemoveFalseNewLines = value;
                RestoreTextFootnotes = value;
                RestoreTables = value;
                ReplaceNbspBySpace = value;
                OptimizeStructure = value;
            }
        }
    }
}