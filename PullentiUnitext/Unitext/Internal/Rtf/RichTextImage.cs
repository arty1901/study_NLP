/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Rtf
{
    class RichTextImage : RichTextItem
    {
        public byte[] Image;
        public int UniqueId;
        public override string ToString()
        {
            return string.Format("Image: {0} bytes ", (Image == null ? 0 : Image.Length));
        }
    }
}