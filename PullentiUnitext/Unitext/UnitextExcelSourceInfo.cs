/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Дополнительная информация при выделении из MsExcel
    /// </summary>
    public class UnitextExcelSourceInfo
    {
        /// <summary>
        /// Имя листа
        /// </summary>
        public string SheetName;
        /// <summary>
        /// Начальный номер строки (от 0)
        /// </summary>
        public int BeginRow;
        /// <summary>
        /// Конечный номер строки (от 0)
        /// </summary>
        public int EndRow;
        /// <summary>
        /// Начальный номер столбца (от 0)
        /// </summary>
        public int BeginColumn;
        /// <summary>
        /// Конечный номер столбца (от 0)
        /// </summary>
        public int EndColumn;
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (SheetName != null) 
                res.AppendFormat("'{0}': ", SheetName);
            if (BeginRow == EndRow) 
                res.AppendFormat("Row {0} ", BeginRow);
            else 
                res.AppendFormat("Rows {0}..{1}", BeginRow, EndRow);
            if (BeginColumn == EndColumn) 
                res.AppendFormat("Cell {0}", BeginColumn);
            else 
                res.AppendFormat("Cells {0}..{1}", BeginColumn, EndColumn);
            return res.ToString();
        }
    }
}