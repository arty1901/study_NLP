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
    /// Параметры создания документа UnitextDocument функцией CreateDocument
    /// </summary>
    public class CreateDocumentParam
    {
        /// <summary>
        /// Признак того, что создание предназначено только для последующего 
        /// выделения плоского текста. То есть картинки и пр. можно игнорировать.
        /// </summary>
        public bool OnlyForPureText = false;
        /// <summary>
        /// Для архивов не извлекать файлы, для почтовых форматов игнорировать вложения
        /// </summary>
        public bool IgnoreInnerDocuments = false;
        /// <summary>
        /// Извлекать ли картинки для страниц тексто-графических форматов (Pdf, DjVu). 
        /// Используется для OCR распознаваний
        /// </summary>
        public bool ExtractPageImageContent = false;
        /// <summary>
        /// Разбивать ли слипшиеся строки таблиц на отдельные строки (когда в ячейках таблицы строки 
        /// моделируются переходами на новую строку)
        /// </summary>
        public bool SplitTableRows = false;
        /// <summary>
        /// Загружать ли структуру документа, если есть, оформляя её через UnitextDocblock. 
        /// Сейчас поддержано только для HTML и FB2.
        /// </summary>
        public bool LoadDocumentStructure = false;
        /// <summary>
        /// Параметры корректировки результата
        /// </summary>
        public CorrectDocumentParam CorrectParams = null;
        /// <summary>
        /// Не генерировать Id для элементов. По умолчанию false, то есть генерировать.
        /// </summary>
        public bool DontGenerateItemsId = false;
        /// <summary>
        /// Не разбирать документы формата Word6 и ранее (иначе выделяет из него только текст, и то не всегда правильно)
        /// </summary>
        public bool IgnoreWord6 = false;
    }
}