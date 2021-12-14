/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfRealValue : PdfObject
    {
        public string Val;
        public override double GetDouble()
        {
            double res;
            if (!Pullenti.Util.MiscHelper.TryParseDouble(Val, out res)) 
                return 0;
            return res;
        }
        public override string ToString(int lev)
        {
            return Val;
        }
        public override bool IsSimple(int lev)
        {
            return true;
        }
    }
}