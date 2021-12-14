/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfObject
    {
        public PdfFile SourceFile = null;
        public int Id = 0;
        public ushort Version = 0;
        public object Tag;
        public virtual bool IsSimple(int lev)
        {
            return false;
        }
        public virtual string ToString(int lev)
        {
            return "?";
        }
        public override string ToString()
        {
            return this.ToString(0);
        }
        public virtual double GetDouble()
        {
            return 0;
        }
    }
}