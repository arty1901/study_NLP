/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfText : PdfRect
    {
        public string Text;
        public bool FontType3;
        public double FontSize;
        public double SpaceWidth;
        public override string ToString()
        {
            return string.Format("{0}: \"{1}\"", base.ToString(), Text ?? "?");
        }
        public void MergeWith(PdfText p)
        {
            Text += p.Text;
            if (p.X1 < X1) 
                X1 = p.X1;
            if (p.X2 > X2) 
                X2 = p.X2;
            if (p.Y1 < Y1) 
                Y1 = p.Y1;
            if (p.Y2 > Y2) 
                Y2 = p.Y2;
        }
        public bool CanBeMergedWith(PdfText p)
        {
            if (FontSize != p.FontSize) 
                return false;
            double d = p.X1 - X2;
            if (d < -1) 
            {
                if (p.X2 > X2 && p.X1 > X1) 
                {
                }
                else 
                    return false;
            }
            if (d > SpaceWidth) 
                return false;
            if (d > (SpaceWidth / 2)) 
            {
                double wi = ((Width + p.Width)) / ((Text.Length + p.Text.Length));
                if (d > (wi / 3)) 
                    return false;
            }
            if (p.FontType3 || FontType3) 
                return false;
            if (Y1 <= p.Y1 && p.Y2 <= Y2) 
                return true;
            if (p.Y1 <= Y1 && Y2 <= p.Y2) 
                return true;
            if (p.Text == " " || ((p.Text.Length == 1 && ((int)p.Text[0]) == 0xA0))) 
            {
                if (Y1 <= p.Y1 && (p.Y1 < Y2)) 
                    return true;
            }
            d = Y1 - p.Y1;
            if (d < 0) 
                d = -d;
            if (d >= 1) 
            {
                if (p.Y1 <= Y1 && p.Y2 >= Y2) 
                {
                }
                return false;
            }
            d = Y2 - p.Y2;
            if (d < 0) 
                d = -d;
            if (d >= 1) 
                return false;
            return true;
        }
    }
}