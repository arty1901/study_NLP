/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Defines factory methods for creating new <see cref="ZipEntry"></see> values.
    interface IEntryFactory
    {
        ZipEntry MakeFileEntry(string fileName);
        ZipEntry MakeFileEntryEx(string fileName, bool useFileSystem);
        ZipEntry MakeDirectoryEntry(string directoryName);
        ZipEntry MakeDirectoryEntryEx(string directoryName, bool useFileSystem);
        INameTransform NameTransform { get; set; }
    }
}