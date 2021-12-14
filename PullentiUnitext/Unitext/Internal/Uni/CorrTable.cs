/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Unitext.Internal.Uni
{
    class CorrTable
    {
        public int BeginInd;
        public int EndInd;
        public Pullenti.Unitext.UnitextTable Result;
        public static CorrTable TryParse(List<CorrLine> lines, int i, out int nexti)
        {
            nexti = i;
            if (i >= lines.Count) 
                return null;
            if (!lines[i].CanBeEmptyLineOfTable) 
                return null;
            int width = lines[i].Text.Length;
            int j;
            for (j = i + 1; j < lines.Count; j++) 
            {
                if (!lines[j].IsPureText) 
                    break;
                else if (lines[j].Length != lines[i].Length || lines[j].Text.Length != width) 
                    break;
            }
            int height = j - i;
            nexti = j - 1;
            if ((width < 30) || (height < 4)) 
                return null;
            List<int> verts = new List<int>();
            for (j = 0; j < width; j++) 
            {
                int r;
                for (r = 0; r < height; r++) 
                {
                    if (lines[i + r].CanHasHorLineOfTable) 
                        continue;
                    if (j >= lines[i + r].Text.Length) 
                        continue;
                    char ch = lines[i + r].Text[j];
                    if (CorrLine.IsTableChar(ch) && !CorrLine.IsHiphen(ch)) 
                    {
                        verts.Add(j);
                        break;
                    }
                }
            }
            if (verts.Count < 3) 
                return null;
            List<int> colBegs = new List<int>();
            List<int> colEnds = new List<int>();
            for (j = 0; j < verts.Count; j++) 
            {
                int b = 0;
                if (j > 0) 
                    b = verts[j - 1] + 1;
                if ((verts[j] - 1) >= b) 
                {
                    colBegs.Add(b);
                    colEnds.Add(verts[j] - 1);
                }
            }
            if (colBegs.Count < 2) 
                return null;
            List<int> rnums = new List<int>();
            int rnum = 0;
            for (j = 0; j < height; j++) 
            {
                rnums.Add(rnum);
                int w;
                for (w = 0; w < width; w++) 
                {
                    if (!verts.Contains(w)) 
                    {
                        if (w >= lines[i + j].Text.Length) 
                            continue;
                        char ch = lines[i + j].Text[w];
                        if (CorrLine.IsTableChar(ch)) 
                        {
                            if (CorrLine.IsHiphen(ch)) 
                            {
                                if (w > 0 && lines[i + j].Text[w - 1] == ch) 
                                {
                                }
                                else if (((w + 1) < width) && lines[i + j].Text[w + 1] == ch) 
                                {
                                }
                                else 
                                    continue;
                            }
                            rnum++;
                            break;
                        }
                    }
                }
            }
            if (rnum < 3) 
                return null;
            CorrTable tab = new CorrTable() { BeginInd = i, EndInd = (i + height) - 1 };
            tab.Result = new Pullenti.Unitext.UnitextTable();
            StringBuilder tmp = new StringBuilder();
            for (int h = 0; h < rnums.Count; h++) 
            {
                for (int c = 0; c < colBegs.Count; c++) 
                {
                    int w0 = colBegs[c];
                    int w1 = colEnds[c];
                    int h0 = h;
                    int ww;
                    for (; h0 < rnums.Count; h0++) 
                    {
                        for (ww = w0; ww <= w1; ww++) 
                        {
                            char ch = lines[i + h0].Text[ww];
                            if (!CorrLine.IsTableChar(ch)) 
                                break;
                        }
                        if (ww < w1) 
                            break;
                    }
                    if (h0 >= rnums.Count) 
                        continue;
                    Pullenti.Unitext.UnitextTablecell cel0 = tab.Result.GetCell(rnums[h0], c);
                    if (cel0 != null) 
                        continue;
                    int cc;
                    for (cc = c + 1; cc < colBegs.Count; cc++) 
                    {
                        for (ww = colEnds[cc - 1]; ww <= colEnds[cc]; ww++) 
                        {
                            char ch = lines[i + h0].Text[ww];
                            if (CorrLine.IsTableChar(ch) && !CorrLine.IsHiphen(ch)) 
                                break;
                        }
                        if (ww < colEnds[cc]) 
                            break;
                        w1 = colEnds[cc];
                    }
                    cc--;
                    int h1;
                    for (h1 = h0 + 1; h1 < rnums.Count; h1++) 
                    {
                        for (ww = w0; ww <= w1; ww++) 
                        {
                            char ch = lines[i + h1].Text[ww];
                            if (CorrLine.IsTableChar(ch)) 
                            {
                                if (CorrLine.IsHiphen(ch)) 
                                {
                                    if (ww > 0 && lines[i + h1].Text[ww - 1] == ch) 
                                        break;
                                    if (((ww + 1) < width) && lines[i + h1].Text[ww + 1] == ch) 
                                        break;
                                }
                                else 
                                    break;
                            }
                        }
                        if (ww < w1) 
                            break;
                    }
                    h1--;
                    tmp.Length = 0;
                    for (int hh = h0; hh <= h1; hh++) 
                    {
                        for (ww = w0 - 1; ww <= w1; ww++) 
                        {
                            if (ww < 0) 
                                continue;
                            char ch = lines[i + hh].Text[ww];
                            if (CorrLine.IsTableChar(ch)) 
                            {
                                if (!CorrLine.IsHiphen(ch)) 
                                    continue;
                            }
                            if (char.IsWhiteSpace(ch)) 
                            {
                                if (tmp.Length > 0 && tmp[tmp.Length - 1] == ' ') 
                                    continue;
                                ch = ' ';
                            }
                            tmp.Append(ch);
                        }
                        if ((hh < h1) && tmp.Length > 0) 
                            tmp.Append(' ');
                        for (int jj = tmp.Length - 1; jj >= 0; jj--) 
                        {
                            if (tmp[jj] != ' ') 
                            {
                                if (CorrLine.IsHiphen(tmp[jj])) 
                                    tmp.Length = jj;
                                break;
                            }
                        }
                    }
                    Pullenti.Unitext.UnitextTablecell cel = tab.Result.AddCell(rnums[h0], rnums[h1], c, cc, new Pullenti.Unitext.UnitextPlaintext() { Text = tmp.ToString().Trim() });
                    c = cc;
                }
            }
            tab.Result.Optimize(false, null);
            return tab;
        }
    }
}