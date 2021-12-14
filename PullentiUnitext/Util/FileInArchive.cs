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
    /// Файл в архиве, см. ArchiveHelper.GetFilesFromArchive
    /// </summary>
    public class FileInArchive
    {
        /// <summary>
        /// Ключ (имя файла в архиве)
        /// </summary>
        public string Key;
        /// <summary>
        /// Содержимое
        /// </summary>
        public byte[] Content;
        /// <summary>
        /// Используйте произвольным образом
        /// </summary>
        public object Tag;
        public override string ToString()
        {
            return Key;
        }
    }
}