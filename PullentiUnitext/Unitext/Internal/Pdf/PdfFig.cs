/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfFig : PdfRect
    {
        public override string ToString()
        {
            return string.Format("lines [x1={0}, x2={1}, y1={2}, y2={3}]", X1, X2, Y1, Y2);
        }
    }
}