/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfRect
    {
        public double X1;
        public double X2;
        public double Y1;
        public double Y2;
        public double Left
        {
            get
            {
                return X1;
            }
        }
        public double Right
        {
            get
            {
                return X2;
            }
        }
        public double Width
        {
            get
            {
                return X2 - X1;
            }
        }
        public double Top
        {
            get
            {
                return Y1;
            }
        }
        public double Bottom
        {
            get
            {
                return Y2;
            }
        }
        public double Height
        {
            get
            {
                return Y2 - Y1;
            }
        }
        public override string ToString()
        {
            return string.Format("[x1={0}, x2={1}, y1={2}, y2={3}", X1, X2, Y1, Y2);
        }
    }
}