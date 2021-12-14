/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Rtf
{
    class RichTextCell : RichTextBlock
    {
        public int ColsSpan = 1;
        public int RowsSpan = 1;
        internal int ColumnId;
        internal bool EndOf;
        internal bool MergeToTop;
        internal bool MergeBottoms;
        internal int FistGrid = -1;
        internal int LastGrid = -1;
        internal int ResColIndex = -1;
        public override string ToString()
        {
            string str = string.Format("ColId={0}, {1}", ColumnId, base.ToString());
            if (ColsSpan > 1) 
                str = string.Format("{0}, ColSpan={1}", str, ColsSpan);
            if (RowsSpan > 1) 
                str = string.Format("{0}, RowSpan={1}", str, RowsSpan);
            if (EndOf) 
                str += " EndOf";
            if (MergeToTop) 
                str += " MergeToTop";
            if (MergeBottoms) 
                str += " MergeBottoms";
            return str;
        }
    }
}