/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    class EntryPatchData
    {
        public int SizePatchOffset
        {
            get
            {
                return sizePatchOffset_;
            }
            set
            {
                sizePatchOffset_ = value;
            }
        }
        public int CrcPatchOffset
        {
            get
            {
                return crcPatchOffset_;
            }
            set
            {
                crcPatchOffset_ = value;
            }
        }
        int sizePatchOffset_;
        int crcPatchOffset_;
    }
}