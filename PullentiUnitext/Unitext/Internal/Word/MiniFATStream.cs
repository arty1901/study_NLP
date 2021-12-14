/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Word
{
    class MiniFATStream : CompoundFileStream
    {
        internal MiniFATStream(CompoundFileStorage storage) : base(storage, storage.System.GetMiniSectorSize())
        {
        }
        protected override byte[] GetPageData(int pageIndex)
        {
            byte[] page = new byte[(int)PageSize];
            uint sector = System.GetMiniStreamNextSector(Storage.Entry.StartingSectorLocation, pageIndex);
            return ReaderUtils.ReadFragment(System.BaseStream, System.GetMiniSectorOffset(sector), PageSize);
        }
    }
}