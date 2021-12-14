/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Misc
{
    public static class ExcelHelper
    {
        static List<List<Pullenti.Unitext.UnitextTablecell>> _getRows(MyXmlReader xml, List<Pullenti.Unitext.UnitextItem> sharedStrings, Dictionary<string, BorderInfo> cellBorders, List<int> hiddenCols)
        {
            List<List<Pullenti.Unitext.UnitextTablecell>> rows = new List<List<Pullenti.Unitext.UnitextTablecell>>();
            int cols = 0;
            int allCols = 0;
            while (xml.Read()) 
            {
                if (xml.NodeType != MyXmlNodeType.Element) 
                    continue;
                if (xml.LocalName == "col" || xml.LocalName == "Column") 
                {
                    if (xml.GetAttribute("hidden") == "1") 
                        hiddenCols.Add(allCols);
                    allCols++;
                    continue;
                }
                if (string.Compare(xml.LocalName, "row", true) == 0) 
                {
                    int rn = rows.Count;
                    int nnn;
                    if (int.TryParse(xml.GetAttribute("r") ?? "", out nnn)) 
                        rn = nnn - 1;
                    while (rows.Count <= rn) 
                    {
                        rows.Add(new List<Pullenti.Unitext.UnitextTablecell>());
                    }
                    List<Pullenti.Unitext.UnitextTablecell> row = rows[rn];
                    if (xml.IsEmptyElement) 
                        continue;
                    while (xml.Read()) 
                    {
                        if (xml.NodeType == MyXmlNodeType.EndElement && string.Compare(xml.LocalName, "row", true) == 0) 
                            break;
                        if (xml.NodeType == MyXmlNodeType.Element && ((xml.LocalName == "c" || xml.LocalName == "Cell"))) 
                        {
                            int num = -1;
                            bool isExt = false;
                            BorderInfo brd = null;
                            Pullenti.Unitext.UnitextItem val = null;
                            string v = xml.GetAttribute("r");
                            if (v != null && v.Length >= 2) 
                                num = (int)((v[0] - 'A'));
                            else if ((((v = xml.GetAttribute("ss:Index")))) != null) 
                            {
                                if (int.TryParse(v, out num)) 
                                    num--;
                            }
                            if ((((v = xml.GetAttribute("t")))) == "s") 
                                isExt = true;
                            if ((((v = xml.GetAttribute("s")))) != null) 
                                cellBorders.TryGetValue(v, out brd);
                            int mergeAcross = -1;
                            int mergeDown = -1;
                            if ((((v = xml.GetAttribute("ss:MergeAcross")))) != null) 
                                int.TryParse(v, out mergeAcross);
                            if ((((v = xml.GetAttribute("ss:MergeDown")))) != null) 
                                int.TryParse(v, out mergeDown);
                            if (!xml.IsEmptyElement) 
                            {
                                while (xml.Read()) 
                                {
                                    if (xml.NodeType == MyXmlNodeType.Element && xml.LocalName == "v") 
                                    {
                                        if (xml.IsEmptyElement) 
                                            break;
                                        if (!xml.Read()) 
                                            break;
                                        if (xml.NodeType == MyXmlNodeType.Text) 
                                        {
                                            if (!isExt) 
                                            {
                                                val = new Pullenti.Unitext.UnitextPlaintext() { Text = xml.Value };
                                                continue;
                                            }
                                            int nu;
                                            if (int.TryParse(xml.Value, out nu)) 
                                            {
                                                if (nu >= 0 && (nu < sharedStrings.Count)) 
                                                {
                                                    Pullenti.Unitext.UnitextItem ss = sharedStrings[nu];
                                                    if (ss is Pullenti.Unitext.UnitextPlaintext) 
                                                        val = new Pullenti.Unitext.UnitextPlaintext() { Text = (ss as Pullenti.Unitext.UnitextPlaintext).Text };
                                                    else 
                                                        val = ss;
                                                }
                                            }
                                        }
                                    }
                                    else if (xml.NodeType == MyXmlNodeType.Element && xml.LocalName == "Data") 
                                    {
                                        if (!xml.IsEmptyElement) 
                                        {
                                            xml.Read();
                                            try 
                                            {
                                                if (xml.NodeType == MyXmlNodeType.Text) 
                                                    val = new Pullenti.Unitext.UnitextPlaintext() { Text = xml.Value };
                                            }
                                            catch(Exception eee) 
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (xml.NodeType == MyXmlNodeType.EndElement && ((xml.LocalName == "c" || xml.LocalName == "Cell"))) 
                                        break;
                                }
                            }
                            if (num < 0) 
                                num = row.Count;
                            if ((num + 1) > cols) 
                                cols = num + 1;
                            while (row.Count <= num) 
                            {
                                row.Add(null);
                            }
                            row[num] = new Pullenti.Unitext.UnitextTablecell() { ColBegin = num, ColEnd = num, RowBegin = rn, RowEnd = rn, Tag = brd };
                            if (val != null) 
                                row[num].Content = val;
                            if (mergeAcross > 0) 
                                row[num].ColEnd += mergeAcross;
                            if (mergeDown > 0) 
                                row[num].RowEnd += mergeDown;
                        }
                    }
                }
                else if (xml.LocalName == "mergeCell") 
                {
                    string val = xml.GetAttribute("ref");
                    if (val == null) 
                        continue;
                    int ii = val.IndexOf(':');
                    if (ii < 2) 
                        continue;
                    string v1 = val.Substring(0, ii);
                    string v2 = val.Substring(ii + 1);
                    if (v2.Length < 2) 
                        continue;
                    int col1 = (int)((v1[0] - 'A'));
                    int col2 = (int)((v2[0] - 'A'));
                    if (((col1 < 0) || (col2 < 0) || col1 >= cols) || col2 >= cols) 
                        continue;
                    int row1;
                    int row2;
                    if (!int.TryParse(v1.Substring(1), out row1) || !int.TryParse(v2.Substring(1), out row2)) 
                        continue;
                    row1--;
                    row2--;
                    if (((row1 < 0) || (row2 < 0) || row1 >= rows.Count) || row2 >= rows.Count) 
                        continue;
                    if (col2 > col1) 
                    {
                        if ((col1 < rows[row1].Count) && rows[row1][col1] != null) 
                            rows[row1][col1].ColEnd = col2;
                        for (int jj = row1; jj <= row2; jj++) 
                        {
                            for (ii = col1 + 1; ii <= col2 && (ii < rows[jj].Count); ii++) 
                            {
                                rows[jj][ii] = null;
                            }
                        }
                    }
                    if (row2 > row1) 
                    {
                        if ((col1 < rows[row1].Count) && rows[row1][col1] != null) 
                            rows[row1][col1].RowEnd = row2;
                        for (int jj = row1 + 1; jj <= row2; jj++) 
                        {
                            for (int kk = col1; kk <= col2; kk++) 
                            {
                                if (kk < rows[jj].Count) 
                                    rows[jj][kk] = null;
                            }
                        }
                    }
                }
            }
            return rows;
        }
        internal static Pullenti.Unitext.UnitextItem ReadSheet(MyXmlReader xml, List<Pullenti.Unitext.UnitextItem> sharedStrings, Dictionary<string, BorderInfo> cellBorders, string sheetName)
        {
            List<int> hiddenCols = new List<int>();
            List<List<Pullenti.Unitext.UnitextTablecell>> rows = _getRows(xml, sharedStrings, cellBorders, hiddenCols);
            hiddenCols.Clear();
            return CreateTable(rows, hiddenCols, sheetName);
        }
        public static Pullenti.Unitext.UnitextItem CreateTable(List<List<Pullenti.Unitext.UnitextTablecell>> rows, List<int> hiddenCols, string sheetName)
        {
            Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer();
            Pullenti.Unitext.UnitextTable tab = null;
            int rnu = 0;
            int rnu0 = 0;
            int remptyMax = 0;
            bool allRowsNoBorder = true;
            for (int rn = 0; rn < rows.Count; rn++) 
            {
                List<Pullenti.Unitext.UnitextTablecell> row = new List<Pullenti.Unitext.UnitextTablecell>();
                for (int cn = 0; cn < rows[rn].Count; cn++) 
                {
                    if (rows[rn][cn] != null) 
                    {
                        if (hiddenCols.Contains(cn)) 
                            continue;
                        Pullenti.Unitext.UnitextTablecell c = rows[rn][cn];
                        row.Add(c);
                        if (c.Tag is BorderInfo) 
                        {
                            if (!(c.Tag as BorderInfo).IsEmpty) 
                            {
                                allRowsNoBorder = false;
                                break;
                            }
                        }
                    }
                }
                if (!allRowsNoBorder) 
                    break;
            }
            for (int rn = 0; rn < rows.Count; rn++) 
            {
                List<Pullenti.Unitext.UnitextTablecell> row = new List<Pullenti.Unitext.UnitextTablecell>();
                bool hasBorder = false;
                bool hasData = false;
                for (int cn = 0; cn < rows[rn].Count; cn++) 
                {
                    if (rows[rn][cn] != null) 
                    {
                        if (hiddenCols.Contains(cn)) 
                            continue;
                        Pullenti.Unitext.UnitextTablecell c = rows[rn][cn];
                        row.Add(c);
                        if (c.Tag is BorderInfo) 
                        {
                            if (!(c.Tag as BorderInfo).IsEmpty) 
                                hasBorder = true;
                        }
                        if (c.Content != null) 
                            hasData = true;
                    }
                }
                if (!hasData) 
                {
                    if (rn <= remptyMax) 
                        continue;
                    if (!hasBorder || tab == null) 
                    {
                        tab = null;
                        if (cnt.Children.Count > 0 && (cnt.Children[cnt.Children.Count - 1] is Pullenti.Unitext.UnitextNewline)) 
                            (cnt.Children[cnt.Children.Count - 1] as Pullenti.Unitext.UnitextNewline).Count++;
                        else 
                            cnt.Children.Add(new Pullenti.Unitext.UnitextNewline());
                        continue;
                    }
                    continue;
                }
                if (!hasBorder && !allRowsNoBorder) 
                {
                    if ((row.Count < 3) || tab == null) 
                    {
                        tab = null;
                        bool fi = true;
                        for (int cn = 0; cn < row.Count; cn++) 
                        {
                            if (row[cn] != null && row[cn].Content != null && !hiddenCols.Contains(cn)) 
                            {
                                if (fi) 
                                    fi = false;
                                else 
                                    cnt.Children.Add(new Pullenti.Unitext.UnitextPlaintext() { Text = " " });
                                cnt.Children.Add(row[cn].Content);
                                row[cn].Content.SourceInfo = new Pullenti.Unitext.UnitextExcelSourceInfo() { SheetName = sheetName, BeginRow = rn, EndRow = rn, BeginColumn = cn, EndColumn = cn };
                            }
                        }
                        if (cnt.Children.Count > 0 && (cnt.Children[cnt.Children.Count - 1] is Pullenti.Unitext.UnitextNewline)) 
                            (cnt.Children[cnt.Children.Count - 1] as Pullenti.Unitext.UnitextNewline).Count++;
                        else 
                            cnt.Children.Add(new Pullenti.Unitext.UnitextNewline());
                        continue;
                    }
                }
                if (tab == null) 
                {
                    cnt.Children.Add(new Pullenti.Unitext.UnitextNewline());
                    tab = new Pullenti.Unitext.UnitextTable();
                    cnt.Children.Add(tab);
                    rnu = 0;
                    rnu0 = rn;
                    remptyMax = rn;
                }
                for (int cn = 0; cn < row.Count; cn++) 
                {
                    if (row[cn] != null && !hiddenCols.Contains(cn)) 
                    {
                        Pullenti.Unitext.UnitextTablecell rr = row[cn];
                        if (rr.RowEnd > remptyMax) 
                            remptyMax = rr.RowEnd;
                        Pullenti.Unitext.UnitextExcelSourceInfo si = new Pullenti.Unitext.UnitextExcelSourceInfo() { SheetName = sheetName, BeginRow = rr.RowBegin, EndRow = rr.RowEnd, BeginColumn = rr.ColBegin, EndColumn = rr.ColEnd };
                        rr.RowBegin = rr.RowBegin - rnu0;
                        rr.RowEnd = rr.RowEnd - rnu0;
                        Pullenti.Unitext.UnitextTablecell cc = tab.AddCell(rr.RowBegin, rr.RowEnd, rr.ColBegin, rr.ColEnd, rr.Content);
                        if (cc != null && cc.SourceInfo == null) 
                        {
                            cc.SourceInfo = si;
                            if (rr.Content != null) 
                                rr.Content.SourceInfo = si;
                        }
                    }
                }
                rnu++;
            }
            return cnt.Optimize(true, null);
        }
        internal static Pullenti.Unitext.UnitextDocument CreateDocForXml(MyXmlReader xml)
        {
            Pullenti.Unitext.UnitextDocument res = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Xlsx };
            Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer();
            Dictionary<string, BorderInfo> borders = new Dictionary<string, BorderInfo>();
            string sheetName = null;
            while (xml.Read()) 
            {
                if (xml.NodeType != MyXmlNodeType.Element) 
                    continue;
                if (xml.LocalName == "Worksheet") 
                {
                    if (cnt.Children.Count > 0) 
                        cnt.Children.Add(new Pullenti.Unitext.UnitextPagebreak());
                    if (xml.Attributes.Count > 0) 
                    {
                        foreach (KeyValuePair<string, string> a in xml.Attributes) 
                        {
                            string name = a.Value;
                            sheetName = name;
                            cnt.Children.Add(new Pullenti.Unitext.UnitextPlaintext() { Text = name, Tag = "SheetName" });
                            cnt.Children.Add(new Pullenti.Unitext.UnitextNewline());
                            break;
                        }
                    }
                    continue;
                }
                if (xml.LocalName == "Table") 
                {
                    Pullenti.Unitext.UnitextItem tab = ReadSheet(xml, null, borders, sheetName);
                    if (tab != null) 
                        cnt.Children.Add(tab);
                }
            }
            res.Content = cnt;
            res.Optimize(false, null);
            return res;
        }
    }
}