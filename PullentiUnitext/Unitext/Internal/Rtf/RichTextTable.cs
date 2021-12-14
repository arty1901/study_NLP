/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Rtf
{
    class RichTextTable : RichTextItem
    {
        internal List<int> ColumnIds = new List<int>();
        public override string ToString()
        {
            return string.Format("Table: {0} Columns, {1} Rows", ColumnIds.Count, Children.Count);
        }
        internal RichTextRow LastRow
        {
            get
            {
                if (Children == null || Children.Count == 0) 
                    return null;
                return Children[Children.Count - 1] as RichTextRow;
            }
        }
        internal RichTextCell LastCell
        {
            get
            {
                RichTextRow r = LastRow;
                if (r == null) 
                    return null;
                if (r.Children == null || r.Children.Count == 0) 
                    return null;
                return r.Children[r.Children.Count - 1] as RichTextCell;
            }
        }
        internal void Correct()
        {
            if (LastRow != null && !LastRow.EndOf && ((LastCell == null || !LastCell.EndOf))) 
                Children.RemoveAt(Children.Count - 1);
            foreach (RichTextItem r in Children) 
            {
                RichTextRow rr = r as RichTextRow;
                if (rr == null) 
                    continue;
                if (rr.Children != null && rr.Children.Count > 1) 
                {
                    RichTextCell la = rr.Children[rr.Children.Count - 1] as RichTextCell;
                    if (!la.EndOf && la.Text == null && la.Children == null) 
                        rr.Children.RemoveAt(rr.Children.Count - 1);
                }
                foreach (RichTextRow.CellInfo ci in rr.CellsInfo) 
                {
                    if (ci.Id > 0 && !ColumnIds.Contains(ci.Id)) 
                        ColumnIds.Add(ci.Id);
                }
                if (rr.CellsInfo.Count == rr.Children.Count) 
                {
                    for (int ii = 0; ii < rr.CellsInfo.Count; ii++) 
                    {
                        (rr.Children[ii] as RichTextCell).ColumnId = rr.CellsInfo[ii].Id;
                        (rr.Children[ii] as RichTextCell).MergeToTop = rr.CellsInfo[ii].VertMergeNext;
                        (rr.Children[ii] as RichTextCell).MergeBottoms = rr.CellsInfo[ii].VertMergeFirst;
                    }
                }
            }
            ColumnIds.Sort();
            Dictionary<int, RichTextCell> vertMerge = new Dictionary<int, RichTextCell>();
            foreach (RichTextItem r in Children) 
            {
                RichTextRow rr = r as RichTextRow;
                if (rr == null) 
                    continue;
                List<RichTextCell> cells = new List<RichTextCell>();
                foreach (RichTextItem c in rr.Children) 
                {
                    if (c is RichTextCell) 
                        cells.Add(c as RichTextCell);
                }
                foreach (RichTextCell cel in cells) 
                {
                    cel.LastGrid = ColumnIds.IndexOf(cel.ColumnId);
                }
                for (int ii = 0; ii < cells.Count; ii++) 
                {
                    if (ii == 0) 
                        cells[ii].FistGrid = 0;
                    else if (cells[ii - 1].LastGrid >= 0) 
                        cells[ii].FistGrid = cells[ii - 1].LastGrid + 1;
                    if (ii == (cells.Count - 1)) 
                        cells[ii].LastGrid = ColumnIds.Count - 1;
                }
                for (int ii = 0; ii < cells.Count; ii++) 
                {
                    RichTextCell cel = cells[ii];
                    if (cel.LastGrid > cel.FistGrid && cel.FistGrid >= 0) 
                        cel.ColsSpan = (cel.LastGrid - cel.FistGrid) + 1;
                    if (cel.MergeBottoms && cel.LastGrid >= 0) 
                    {
                        if (!vertMerge.ContainsKey(cel.LastGrid)) 
                            vertMerge.Add(cel.LastGrid, cel);
                        else 
                            vertMerge[cel.LastGrid] = cel;
                    }
                    else if (cel.MergeToTop && vertMerge.ContainsKey(cel.LastGrid)) 
                    {
                        vertMerge[cel.LastGrid].RowsSpan++;
                        rr.Children.Remove(cel);
                    }
                }
            }
        }
    }
}