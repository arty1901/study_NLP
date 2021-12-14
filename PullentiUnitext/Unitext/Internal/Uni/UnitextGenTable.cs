/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Uni
{
    /// <summary>
    /// Это для поддержки генерации таблиц из модели COLSPAN + ROWSPAN
    /// </summary>
    class UnitextGenTable
    {
        public List<List<UniTextGenCell>> Cells = new List<List<UniTextGenCell>>();
        public List<string> m_ColWidth = new List<string>();
        public Pullenti.Unitext.UnitextTable Convert()
        {
            Pullenti.Unitext.UnitextTable res = new Pullenti.Unitext.UnitextTable();
            res.MayHasError = MayHasError;
            for (int c = 0; c < m_ColWidth.Count; c++) 
            {
                res.SetColWidth(c, m_ColWidth[c]);
            }
            for (int r = 0; r < Cells.Count; r++) 
            {
                int cn = 0;
                foreach (UniTextGenCell cc in Cells[r]) 
                {
                    for (; ; cn++) 
                    {
                        if (res.GetCell(r, cn) == null) 
                            break;
                    }
                    res.AddCell(r, (r + cc.RowSpan) - 1, cn, (cn + cc.ColSpan) - 1, cc.Content);
                    if (cc.Width != null && cc.ColSpan <= 1 && res.GetColWidth(cn) == null) 
                    {
                        if (cc.Width != null) 
                            res.SetColWidth(cn, cc.Width);
                        else if ((cc.Content is Pullenti.Unitext.UnitextTable) && (cc.Content as Pullenti.Unitext.UnitextTable).Width != null) 
                            res.SetColWidth(cn, (cc.Content as Pullenti.Unitext.UnitextTable).Width);
                    }
                    cn += cc.ColSpan;
                }
            }
            return res;
        }
        public bool MayHasError;
    }
}