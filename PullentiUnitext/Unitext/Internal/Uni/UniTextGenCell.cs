/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Uni
{
    class UniTextGenCell
    {
        public int ColSpan = 1;
        public int RowSpan = 1;
        public Pullenti.Unitext.UnitextItem Content;
        public int Tag;
        public string Width;
        public override string ToString()
        {
            return string.Format("ColSpan:{0} RowSpan:{1} Content:{2}", ColSpan, RowSpan, (Content == null ? "null" : Content.ToString()));
        }
    }
}