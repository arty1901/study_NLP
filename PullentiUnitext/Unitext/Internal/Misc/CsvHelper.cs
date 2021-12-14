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

namespace Pullenti.Unitext.Internal.Misc
{
    public static class CsvHelper
    {
        public static char CheckDelim(string text)
        {
            char[] delims = new char[] {',', ';', ' ', '\t', '|'};
            int[] stat = new int[(int)delims.Length];
            int[] rows = new int[(int)delims.Length];
            int[] columns = new int[(int)delims.Length];
            List<string> cells = new List<string>();
            StringBuilder tmp = new StringBuilder();
            for (int i = 0; (i < delims.Length) && (i < 100000); i++) 
            {
                rows[i] = 0;
                int cols = 0;
                for (int j = 0; j < text.Length; ) 
                {
                    cells.Clear();
                    int jj = ReadRecord(text, j, delims[i], cells, null);
                    if (jj < 0) 
                        break;
                    if (cells.Count > 1) 
                    {
                        if (cols == 0) 
                            cols = cells.Count;
                        else if (cols == cells.Count) 
                            stat[i]++;
                        else 
                            stat[i]--;
                        rows[i]++;
                    }
                    if (jj > j) 
                        j = jj;
                    else 
                        break;
                }
                columns[i] = cols;
            }
            char delim = (char)0;
            int bestcou = 0;
            for (int i = 0; i < delims.Length; i++) 
            {
                if (stat[i] > 0 && (stat[i] + 1) >= ((int)((rows[i] * 0.96)))) 
                {
                    if (bestcou == 0 || columns[i] > bestcou) 
                    {
                        bestcou = columns[i];
                        delim = delims[i];
                    }
                }
            }
            return delim;
        }
        internal static Pullenti.Unitext.UnitextDocument Create(string text)
        {
            char delim = CheckDelim(text);
            List<string> cells = new List<string>();
            StringBuilder tmp = new StringBuilder();
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Csv };
            Pullenti.Unitext.UnitextTable tab = new Pullenti.Unitext.UnitextTable();
            Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
            doc.Content = tab;
            int row = 0;
            for (int j = 0; j < text.Length; ) 
            {
                cells.Clear();
                int jj = ReadRecord(text, j, delim, cells, tmp);
                if (jj < 0) 
                    break;
                if (cells.Count > 0) 
                {
                    for (int i = 0; i < cells.Count; i++) 
                    {
                        Pullenti.Unitext.UnitextTablecell cel = tab.AddCell(row, row, i, i, null);
                        string val = cells[i];
                        if (string.IsNullOrEmpty(val)) 
                            continue;
                        gen.ClearAll();
                        gen.AppendText(val, false);
                        cel.Content = gen.Finish(true, null);
                    }
                    row++;
                }
                if (jj > j) 
                    j = jj;
                else 
                    break;
            }
            int r;
            for (r = 0; r < tab.RowsCount; r++) 
            {
                Pullenti.Unitext.UnitextTablecell cel = tab.GetCell(r, tab.ColsCount - 1);
                if (cel != null && cel.Content != null) 
                    break;
            }
            if (r >= tab.RowsCount) 
                tab.RemoveLastColumn();
            return doc;
        }
        public static int ReadRecord(string text, int pos, char delim, List<string> res, StringBuilder tmp)
        {
            if (pos >= text.Length) 
                return -1;
            if (tmp != null) 
                tmp.Length = 0;
            bool isQu = false;
            bool newCell = false;
            int len = 0;
            int i;
            for (i = pos; i < text.Length; i++) 
            {
                char ch = text[i];
                if (isQu) 
                {
                    if (ch == '"') 
                    {
                        if (((i + 1) < text.Length) && text[i + 1] == '"') 
                        {
                            if (tmp != null) 
                                tmp.Append('"');
                            i++;
                            len++;
                            continue;
                        }
                        isQu = false;
                        continue;
                    }
                    if (tmp != null) 
                        tmp.Append(ch);
                    len++;
                    continue;
                }
                if (ch == delim && delim != 0) 
                {
                    if (tmp != null) 
                    {
                        res.Add(tmp.ToString().Trim());
                        tmp.Length = 0;
                    }
                    else 
                        res.Add(null);
                    newCell = true;
                    len = 0;
                    continue;
                }
                if (ch == 0xD || ch == 0xA) 
                {
                    i++;
                    if (i < text.Length) 
                    {
                        ch = text[i];
                        if (ch == 0xD || ch == 0xA) 
                            i++;
                    }
                    break;
                }
                if (ch == '"' && len == 0) 
                {
                    if (((i + 1) < text.Length) && text[i + 1] == '"') 
                        i++;
                    else 
                        isQu = true;
                }
                else 
                {
                    if (tmp != null) 
                        tmp.Append(ch);
                    len++;
                }
            }
            if (len > 0 || newCell) 
            {
                if (tmp != null) 
                    res.Add(tmp.ToString().Trim());
                else 
                    res.Add(null);
            }
            return i;
        }
    }
}