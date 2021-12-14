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
    class RichTextRow : RichTextItem
    {
        internal bool EndOf;
        internal bool LastRow;
        internal class CellInfo
        {
            public int Id;
            public bool HorMerge;
            public bool VertMergeFirst;
            public bool VertMergeNext;
            public int Width;
            public override string ToString()
            {
                return string.Format("{0} {1}{2}{3}", Id, (HorMerge ? "Merge " : ""), (VertMergeFirst ? "VertMergeFirst " : ""), (VertMergeNext ? "VertMergeNext " : ""));
            }
        }

        internal List<CellInfo> CellsInfo = new List<CellInfo>();
        internal bool _addCmd(string cmd)
        {
            if (cmd == null) 
                return false;
            if (cmd == "lastrow") 
            {
                LastRow = true;
                return true;
            }
            CellInfo last = (CellsInfo.Count == 0 ? null : CellsInfo[CellsInfo.Count - 1]);
            if (cmd.StartsWith("cellx")) 
            {
                int idd;
                if (int.TryParse(cmd.Substring(5), out idd)) 
                {
                    if (idd > 0) 
                    {
                        foreach (CellInfo ii in CellsInfo) 
                        {
                            if (ii.Id == idd) 
                            {
                                if (last.Id == 0) 
                                {
                                    if (last.HorMerge) 
                                        ii.HorMerge = true;
                                    if (last.VertMergeFirst) 
                                        ii.VertMergeFirst = true;
                                    if (last.VertMergeNext) 
                                        ii.VertMergeNext = true;
                                    CellsInfo.Remove(last);
                                }
                                return true;
                            }
                        }
                        if (last != null && last.Id == 0) 
                            last.Id = idd;
                        else 
                            CellsInfo.Add(new CellInfo() { Id = idd });
                    }
                }
                return true;
            }
            if (cmd.StartsWith("clwWidth")) 
            {
                int wi;
                if (int.TryParse(cmd.Substring(8), out wi)) 
                {
                    if (last == null || last.Id > 0) 
                        CellsInfo.Add((last = new CellInfo()));
                    last.Width = wi;
                    return true;
                }
            }
            if (cmd == "clmgf") 
            {
                if (last == null || last.Id > 0) 
                    CellsInfo.Add((last = new CellInfo()));
                last.HorMerge = true;
                return true;
            }
            if (cmd == "clvmgf") 
            {
                if (last == null || last.Id > 0) 
                    CellsInfo.Add((last = new CellInfo()));
                last.VertMergeFirst = true;
                return true;
            }
            if (cmd == "clvmrg") 
            {
                if (last == null || last.Id > 0) 
                    CellsInfo.Add((last = new CellInfo()));
                last.VertMergeNext = true;
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return string.Format("{0}{1}{2}", (LastRow ? "LastRow " : ""), (EndOf ? "HasEnd " : ""), base.ToString());
        }
    }
}