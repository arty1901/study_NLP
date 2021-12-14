/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext
{
    // Интерфейс для расширения функционала сервиса
    public interface IUnitextExtension
    {
        UnitextDocument CreateDocument(FileFormat frm, string fileName, byte[] fileContent, CreateDocumentParam pars);
        Dictionary<string, int> GetFileNamesFromArchive(string fileName, byte[] content);
        List<Pullenti.Util.FileInArchive> GetFilesFromArchive(string fileName, byte[] content);
        byte[] GetFileFromArchive(string fileName, byte[] content, string internalName);
    }
}