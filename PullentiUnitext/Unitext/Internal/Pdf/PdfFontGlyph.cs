/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    class PdfFontGlyph
    {
        public double Width;
        public int Code;
        public char Char;
        public char Char2;
        public string UndefMnem;
        public override string ToString()
        {
            return string.Format("{0} -> '{1}' (w={2})", Code.ToString("X04"), (Char == 0 ? '?' : Char), Width);
        }
    }
}