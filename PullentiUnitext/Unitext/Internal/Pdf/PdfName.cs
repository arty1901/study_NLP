﻿/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfName : PdfObject
    {
        public string Name;
        public override string ToString(int lev)
        {
            return Name;
        }
        public override bool IsSimple(int lev)
        {
            return true;
        }
        internal bool HasSlash;
    }
}