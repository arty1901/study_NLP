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
    /// Сервис поддержки технологии Unitext
    /// </summary>
    public static class UnitextService
    {
        /// <summary>
        /// Текущая версия
        /// </summary>
        public static string Version = "4.10";
        /// <summary>
        /// Дата выхода версии
        /// </summary>
        public static string VersionDate = "2021.11.28";
        /// <summary>
        /// Создать документ из файла или байтового потока. 
        /// ВНИМАНИЕ! Функция всегда возвращает экземпляр UnitextDocument и не выбрасывает Exceptions, 
        /// а ошибки оформляются в свойстве ErrorMessage;
        /// </summary>
        /// <param name="fileName">имя файла (может быть null)</param>
        /// <param name="fileContent">содержимое файла (если null, то fileName есть имя файла в локальной файловой системе)</param>
        /// <param name="pars">дополнительные параметры выделения (может быть null)</param>
        /// <return>экземпляр документа</return>
        public static UnitextDocument CreateDocument(string fileName, byte[] fileContent = null, CreateDocumentParam pars = null)
        {
            if (pars == null) 
                pars = new CreateDocumentParam();
            UnitextDocument doc = Pullenti.Unitext.Internal.Uni.UnitextHelper.Create(fileName, fileContent, pars);
            bool opt = false;
            if (pars.CorrectParams != null) 
            {
                if (doc.Correct(pars.CorrectParams)) 
                    opt = true;
            }
            else if (doc.SourceFormat == FileFormat.Txt || Pullenti.Util.FileFormatsHelper.GetFormatClass(doc.SourceFormat) == FileFormatClass.Pagelayout) 
            {
                Pullenti.Unitext.Internal.Uni.UnitextCorrHelper.RemovePageBreakNumeration(doc);
                Pullenti.Unitext.Internal.Uni.UnitextCorrHelper.RemoveFalseNewLines(doc, true);
            }
            if (opt) 
                doc.Optimize(false, null);
            if (pars.DontGenerateItemsId) 
                doc.RefreshParents();
            else 
                doc.GenerateIds();
            if (doc.SourceFormat == FileFormat.Unknown) 
                doc.SourceFormat = FileFormat.Txt;
            return doc;
        }
        /// <summary>
        /// Создать документ из чистого текста, при этом позиции всех элементов 
        /// будут относительно именно его! GetPlaintext будет не генерировать, а возвращает именно его!
        /// </summary>
        /// <param name="text">текст</param>
        /// <return>экземпляр документа</return>
        public static UnitextDocument CreateDocumentFromText(string text)
        {
            if (string.IsNullOrEmpty(text)) 
                return null;
            UnitextDocument doc = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateDocFromText(text);
            doc.RefreshParents();
            return doc;
        }
        // Реализация извне дополнительного функцонала
        public static IUnitextExtension Extension = null;
    }
}