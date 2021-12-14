/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Pdf
{
    class PdfTextState
    {
        public PdfTextState()
        {
            CtmStack = new List<Matrix>();
            CtmStack.Add(new Matrix());
        }
        public List<Matrix> CtmStack;
        public double BoxHeight;
        public double FontSize;
        public PdfFont Font;
        public double CharSpace = 0;
        public double WordSpace = 0;
        public double WScale = 100;
        public double Leading = 0;
        public double Rise = 0;
        public int Render = 0;
        public Matrix Tlm = new Matrix();
        public Matrix Tm = new Matrix();
        public bool ParseOne(List<PdfObject> lex, int i)
        {
            string nam = (lex[i] as PdfName).Name;
            if (nam == "Tc") 
            {
                CharSpace = lex[i - 1].GetDouble();
                return true;
            }
            if (nam == "Tw") 
            {
                WordSpace = lex[i - 1].GetDouble();
                return true;
            }
            if (nam == "Tz") 
            {
                WScale = lex[i - 1].GetDouble();
                return true;
            }
            if (nam == "Ts") 
            {
                Rise = lex[i - 1].GetDouble();
                return true;
            }
            if (nam == "Tr") 
            {
                Render = (int)lex[i - 1].GetDouble();
                return true;
            }
            if (nam == "TL") 
            {
                Leading = lex[i - 1].GetDouble();
                return true;
            }
            if (nam == "Td" || nam == "TD") 
            {
                double x = lex[i - 2].GetDouble();
                double y = lex[i - 1].GetDouble();
                if (nam == "TD") 
                    Leading = -y;
                Tlm.Translate(x, y);
                Tm.CopyFrom(Tlm);
                return true;
            }
            if (nam == "T*") 
            {
                this.Newline();
                return true;
            }
            if (nam == "Tm") 
            {
                Tlm.Set(lex[i - 6].GetDouble(), lex[i - 5].GetDouble(), lex[i - 4].GetDouble(), lex[i - 3].GetDouble(), lex[i - 2].GetDouble(), lex[i - 1].GetDouble());
                Tm.CopyFrom(Tlm);
                return true;
            }
            if (nam == "cm") 
            {
                Matrix mmm = new Matrix();
                mmm.Set(lex[i - 6].GetDouble(), lex[i - 5].GetDouble(), lex[i - 4].GetDouble(), lex[i - 3].GetDouble(), lex[i - 2].GetDouble(), lex[i - 1].GetDouble());
                if (CtmStack.Count > 0) 
                {
                    mmm.Multiply(CtmStack[0]);
                    CtmStack[0] = mmm;
                }
                else 
                    CtmStack.Insert(0, mmm);
                return true;
            }
            if (nam == "q") 
            {
                Matrix mmm = new Matrix();
                if (CtmStack.Count > 0) 
                    mmm.CopyFrom(CtmStack[0]);
                CtmStack.Insert(0, mmm);
                return true;
            }
            if (nam == "Q") 
            {
                if (CtmStack.Count > 0) 
                    CtmStack.RemoveAt(0);
            }
            return false;
        }
        public void Newline()
        {
            Tlm.Translate(0, -Leading);
            Tm.CopyFrom(Tlm);
        }
    }
}