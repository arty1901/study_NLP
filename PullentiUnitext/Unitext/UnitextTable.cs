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
using System.Xml;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Таблица, представляет собой матрицу из клеток. 
    /// Ячейки могут заполнять прямоугольные области из клеток. 
    /// Ячейки не могут пересекаться друг с другом.
    /// </summary>
    public class UnitextTable : UnitextItem
    {
        public override UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            if (m_ColsCount == 3 && m_Cells.Count == 2) 
            {
            }
            foreach (List<UnitextTablecell> ro in m_Cells) 
            {
                foreach (UnitextTablecell c in ro) 
                {
                    if (c != null) 
                        c.Optimize(false, pars);
                }
            }
            if (m_Cells.Count == 0) 
                return null;
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    if (m_Cells[r][c] != null) 
                        continue;
                    int cc;
                    for (cc = c + 1; cc < m_Cells[r].Count; cc++) 
                    {
                        if (m_Cells[r][cc] != null) 
                            break;
                    }
                    this.AddCell(r, r, c, cc - 1, null);
                }
                if (m_Cells[r].Count < m_ColsCount) 
                    this.AddCell(r, r, m_Cells[r].Count, m_ColsCount - 1, null);
            }
            for (int i = m_ColsCount - 1; i >= 0; i--) 
            {
                int r;
                for (r = 0; r < m_Cells.Count; r++) 
                {
                    UnitextTablecell c = m_Cells[r][i];
                    if (c == null) 
                        continue;
                    if (c.Content != null) 
                    {
                        if (c.ColBegin == i) 
                            break;
                    }
                }
                if (r < m_Cells.Count) 
                    continue;
                for (r = 0; r < m_Cells.Count; r++) 
                {
                    for (int c = 0; c < m_Cells[r].Count; c++) 
                    {
                        UnitextTablecell ce = m_Cells[r][c];
                        if (ce == null) 
                            continue;
                        if (ce.ColBegin != c || ce.RowBegin != r) 
                            continue;
                        if (ce.ColBegin >= i) 
                            ce.ColBegin--;
                        if (ce.ColEnd >= i) 
                            ce.ColEnd--;
                    }
                }
                for (r = 0; r < m_Cells.Count; r++) 
                {
                    m_Cells[r].RemoveAt(i);
                }
                m_ColsCount--;
            }
            for (int r = m_Cells.Count - 1; r >= 0; r--) 
            {
                int i;
                for (i = 0; i < m_Cells[r].Count; i++) 
                {
                    UnitextTablecell cel = m_Cells[r][i];
                    if (cel == null) 
                        continue;
                    if (cel.RowBegin == r && cel.ColBegin == i) 
                    {
                        if (cel.Content != null) 
                            break;
                    }
                }
                if (i < m_Cells[r].Count) 
                    continue;
                for (int rr = 0; rr < m_Cells.Count; rr++) 
                {
                    for (int c = 0; c < m_Cells[r].Count; c++) 
                    {
                        UnitextTablecell ce = m_Cells[rr][c];
                        if (ce == null) 
                            continue;
                        if (ce.ColBegin != c || ce.RowBegin != rr) 
                            continue;
                        if (ce.RowBegin >= r) 
                            ce.RowBegin--;
                        if (ce.RowEnd >= r) 
                            ce.RowEnd--;
                    }
                }
                m_Cells.RemoveAt(r);
            }
            if (m_ColsCount == 1) 
            {
                UnitextContainer cnt = new UnitextContainer();
                cnt.IsInline = false;
                for (int r = 0; r < m_Cells.Count; r++) 
                {
                    if (m_Cells[r][0].Content != null && m_Cells[r][0].RowBegin == r) 
                    {
                        cnt.Children.Add(m_Cells[r][0].Content);
                        cnt.Children.Add(new UnitextNewline() { Tag = cnt });
                    }
                }
                cnt.Children.Insert(0, new UnitextNewline() { Tag = cnt });
                return cnt.Optimize(false, pars);
            }
            if (m_ColsCount == 0) 
                return null;
            if (pars != null && pars.SplitTableRows) 
            {
                for (int r = 0; r < RowsCount; r++) 
                {
                    int c;
                    int ppp = 0;
                    int cou = 0;
                    for (c = 0; c < ColsCount; c++) 
                    {
                        UnitextTablecell cel = this.GetCell(r, c);
                        if (cel == null) 
                            break;
                        if (cel.ColBegin != cel.ColEnd || cel.RowBegin != cel.RowEnd) 
                            break;
                        if (cel.Content == null) 
                            continue;
                        UnitextContainer cnt = cel.Content as UnitextContainer;
                        if (cnt == null) 
                            break;
                        int i;
                        int p = 0;
                        for (i = 0; i < cnt.Children.Count; i++) 
                        {
                            UnitextItem ch = cnt.Children[i];
                            if (((ch is UnitextPlaintext) || (ch is UnitextHyperlink) || (ch is UnitextImage)) || (ch is UnitextPagebreak)) 
                            {
                            }
                            else if (ch is UnitextNewline) 
                            {
                                if ((ch as UnitextNewline).Count > 1) 
                                    p++;
                            }
                            else 
                                break;
                        }
                        if (i < cnt.Children.Count) 
                            break;
                        if (p == 0) 
                            break;
                        if (ppp == 0) 
                            ppp = p;
                        else if (ppp != p) 
                            break;
                        cou++;
                    }
                    if ((c < ColsCount) || ppp == 0 || (cou < 2)) 
                        continue;
                    List<UnitextTablecell> nrow = new List<UnitextTablecell>();
                    for (int rr = r + 1; rr < m_Cells.Count; rr++) 
                    {
                        for (c = 0; c < ColsCount; c++) 
                        {
                            if (m_Cells[rr][c] != null) 
                                m_Cells[rr][c].Tag = null;
                        }
                    }
                    for (int rr = r + 1; rr < m_Cells.Count; rr++) 
                    {
                        for (c = 0; c < ColsCount; c++) 
                        {
                            if (m_Cells[rr][c] != null && m_Cells[rr][c].Tag == null) 
                            {
                                m_Cells[rr][c].Tag = m_Cells;
                                m_Cells[rr][c].RowBegin++;
                                m_Cells[rr][c].RowEnd++;
                            }
                        }
                    }
                    m_Cells.Insert(r + 1, nrow);
                    for (c = 0; c < ColsCount; c++) 
                    {
                        UnitextTablecell cel = this.GetCell(r, c);
                        UnitextTablecell cel1 = new UnitextTablecell() { IsPsevdo = true, ColBegin = c, ColEnd = c, RowBegin = r + 1, RowEnd = r + 1 };
                        nrow.Add(cel1);
                        if (cel.Content == null) 
                            continue;
                        UnitextContainer cnt = cel.Content as UnitextContainer;
                        int i;
                        for (i = cnt.Children.Count - 1; i >= 0; i--) 
                        {
                            if (cnt.Children[i] is UnitextPagebreak) 
                                cnt.Children.RemoveAt(i);
                        }
                        for (i = 0; i < cnt.Children.Count; i++) 
                        {
                            if ((cnt.Children[i] is UnitextNewline) && (cnt.Children[i] as UnitextNewline).Count > 1) 
                                break;
                        }
                        if (i >= cnt.Children.Count) 
                            continue;
                        if (i == 1) 
                        {
                            cel1.Content = cnt;
                            cel.Content = cnt.Children[0];
                            cnt.Children.RemoveAt(0);
                            cnt.Children.RemoveAt(0);
                            cel1.Content = cel1.Content.Optimize(true, pars);
                        }
                        else 
                        {
                            UnitextContainer cnt1 = new UnitextContainer();
                            for (int ii = i + 1; ii < cnt.Children.Count; ii++) 
                            {
                                cnt1.Children.Add(cnt.Children[ii]);
                            }
                            cnt.Children.RemoveRange(i, cnt.Children.Count - i);
                            cel1.Content = cnt1.Optimize(true, pars);
                        }
                    }
                }
            }
            return this;
        }
        /// <summary>
        /// Добавить ячейку
        /// </summary>
        /// <param name="rowBegin">начальная строка</param>
        /// <param name="rowEnd">конечная строка</param>
        /// <param name="colBegin">начальный столбец</param>
        /// <param name="colEnd">конечный столбец</param>
        /// <param name="content">возможное содержимое</param>
        /// <return>если null, то значит область пересекается с существующей ячейкой</return>
        public UnitextTablecell AddCell(int rowBegin, int rowEnd, int colBegin, int colEnd, UnitextItem content = null)
        {
            if ((rowBegin < 0) || rowBegin > rowEnd) 
                return null;
            if ((colBegin < 0) || colBegin > colEnd) 
                return null;
            if ((colEnd + 1) > m_ColsCount) 
                m_ColsCount = colEnd + 1;
            while (m_Cells.Count <= rowEnd) 
            {
                m_Cells.Add(new List<UnitextTablecell>());
            }
            for (int r = rowEnd; r >= rowBegin; r--) 
            {
                while (m_Cells[r].Count <= colEnd) 
                {
                    m_Cells[r].Add(null);
                }
                for (int c = colBegin; c <= colEnd; c++) 
                {
                    if (m_Cells[r][c] != null) 
                    {
                        if (r > rowBegin) 
                        {
                            rowEnd = r - 1;
                            break;
                        }
                        if (c > colBegin) 
                        {
                            colEnd = c - 1;
                            break;
                        }
                        return null;
                    }
                }
            }
            UnitextTablecell cel = new UnitextTablecell() { Content = content, ColBegin = colBegin, ColEnd = colEnd, RowBegin = rowBegin, RowEnd = rowEnd };
            for (int r = rowBegin; r <= rowEnd; r++) 
            {
                for (int c = colBegin; c <= colEnd; c++) 
                {
                    m_Cells[r][c] = cel;
                }
            }
            return cel;
        }
        List<List<UnitextTablecell>> m_Cells = new List<List<UnitextTablecell>>();
        List<string> m_ColWidth = new List<string>();
        /// <summary>
        /// Получить ячейку таблицы, накрывающую указанную клетку
        /// </summary>
        /// <param name="row">строка клетки</param>
        /// <param name="col">столбец клетки</param>
        /// <return>ячейка (null - отсутствует)</return>
        public UnitextTablecell GetCell(int row, int col)
        {
            if ((row < 0) || row >= m_Cells.Count) 
                return null;
            if ((col < 0) || col >= m_Cells[row].Count) 
                return null;
            return m_Cells[row][col];
        }
        /// <summary>
        /// Получить ширину столбца (если таковая была задана во входном файле)
        /// </summary>
        /// <param name="col">номер столбца</param>
        /// <return>ширина (как она была задана в Html или Doc) или null</return>
        public string GetColWidth(int col)
        {
            if (col >= 0 && (col < m_ColWidth.Count)) 
                return m_ColWidth[col];
            else 
                return null;
        }
        public void SetColWidth(int col, string val)
        {
            if (col < 0) 
                return;
            while (col >= m_ColWidth.Count) 
            {
                m_ColWidth.Add(null);
            }
            m_ColWidth[col] = val;
        }
        /// <summary>
        /// Количество столбцов
        /// </summary>
        public int ColsCount
        {
            get
            {
                return m_ColsCount;
            }
        }
        int m_ColsCount;
        /// <summary>
        /// Количество строк
        /// </summary>
        public int RowsCount
        {
            get
            {
                return m_Cells.Count;
            }
        }
        /// <summary>
        /// Ширина таблицы (если входной файл - Html или подобный). 
        /// Если не задана, то при выводе в HTML считается 100%
        /// </summary>
        public string Width;
        /// <summary>
        /// Признак того, что рамки таблицы и ячеек не прорисовывать
        /// </summary>
        public bool HideBorders = false;
        /// <summary>
        /// Признак того, что таблица может содержать ошибки (например, для Doc неправильно объединены ячейки по вертикали).
        /// </summary>
        public bool MayHasError;
        public override UnitextItem Clone()
        {
            UnitextTable res = new UnitextTable();
            res._cloneFrom(this);
            res.m_ColsCount = m_ColsCount;
            if (m_ColWidth != null) 
                res.m_ColWidth = new List<string>(m_ColWidth);
            res.Width = Width;
            res.HideBorders = HideBorders;
            res.MayHasError = MayHasError;
            foreach (List<UnitextTablecell> r in m_Cells) 
            {
                List<UnitextTablecell> rr = new List<UnitextTablecell>();
                res.m_Cells.Add(rr);
                for (int i = 0; i < r.Count; i++) 
                {
                    if (r[i] == null) 
                        rr.Add(null);
                    else 
                        rr.Add(r[i].Clone() as UnitextTablecell);
                }
            }
            return res;
        }
        internal void RemoveLastColumn()
        {
            m_ColsCount--;
            if (m_ColWidth != null && m_ColWidth.Count > m_ColsCount) 
                m_ColWidth.RemoveRange(m_ColsCount, m_ColWidth.Count - m_ColsCount);
            foreach (List<UnitextTablecell> r in m_Cells) 
            {
                if (r.Count > m_ColsCount) 
                    r.RemoveRange(m_ColsCount, r.Count - m_ColsCount);
            }
        }
        public override bool IsInline
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        internal override string InnerTag
        {
            get
            {
                return "tbl";
            }
        }
        public override UnitextItem FindById(string id)
        {
            if (Id == id) 
                return this;
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    if (m_Cells[r][c] != null) 
                    {
                        UnitextItem res = m_Cells[r][c].FindById(id);
                        if (res != null) 
                            return res;
                    }
                }
            }
            return null;
        }
        UnitextTablecell[] m_Map;
        internal UnitextTablecell _findCellByPos(int plainPos)
        {
            if (m_Map == null && (BeginChar < EndChar)) 
            {
                m_Map = new UnitextTablecell[(int)((EndChar + 1) - BeginChar)];
                for (int r = 0; r < m_Cells.Count; r++) 
                {
                    for (int c = 0; c < m_Cells[r].Count; c++) 
                    {
                        if (m_Cells[r][c] != null) 
                        {
                            UnitextTablecell cel = m_Cells[r][c];
                            for (int i = cel.BeginChar; i <= cel.EndChar; i++) 
                            {
                                if (i >= BeginChar && i <= EndChar) 
                                    m_Map[i - BeginChar] = cel;
                            }
                        }
                    }
                }
            }
            if (m_Map != null && plainPos >= BeginChar && plainPos <= EndChar) 
                return m_Map[plainPos - BeginChar];
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    if (m_Cells[r][c] != null) 
                    {
                        UnitextTablecell cel = m_Cells[r][c];
                        if (cel.BeginChar <= plainPos && plainPos <= cel.EndChar) 
                            return cel;
                    }
                }
            }
            return null;
        }
        internal bool TryAppend(UnitextTable tab)
        {
            if (tab.ColsCount != m_ColsCount) 
                return false;
            if (PageSectionId != tab.PageSectionId) 
                return false;
            if (tab.RowsCount < 1) 
                return false;
            int k;
            for (k = 0; k < 2; k++) 
            {
                int r = (k == 0 ? 0 : m_Cells.Count - 1);
                int c;
                for (c = 0; c < m_Cells[r].Count; c++) 
                {
                    if (m_Cells[r][c] == null) 
                        break;
                    UnitextTablecell cel = tab.GetCell(0, c);
                    if (cel == null) 
                        break;
                    if (cel.ColBegin != m_Cells[r][c].ColBegin || cel.ColEnd != m_Cells[r][c].ColEnd) 
                        break;
                }
                if (c >= m_Cells[r].Count) 
                    break;
            }
            if (k >= 2) 
                return false;
            int delt = m_Cells.Count;
            for (int r = tab.RowsCount - 1; r >= 0; r--) 
            {
                for (int c = 0; c < tab.ColsCount; c++) 
                {
                    UnitextTablecell cel = tab.GetCell(r, c);
                    if (cel == null) 
                        continue;
                    if (cel.ColBegin == c && cel.RowBegin == r) 
                    {
                        cel.RowBegin += delt;
                        cel.RowEnd += delt;
                    }
                }
            }
            m_Cells.AddRange(tab.m_Cells);
            tab.m_Cells.Clear();
            return true;
        }
        public override string ToString()
        {
            return string.Format("Table [{0}x{1}]{2}", RowsCount, ColsCount, (MayHasError ? " may be error" : ""));
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars = null)
        {
            if (pars != null && pars.SetPositions) 
                BeginChar = res.Length;
            if (pars == null) 
                pars = UnitextItem.m_DefParams;
            if (pars.TableStart != null) 
                res.Append(pars.TableStart);
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    UnitextTablecell cel = m_Cells[r][c];
                    if (cel == null) 
                        continue;
                    if (r == cel.RowBegin && c == cel.ColBegin) 
                        cel.GetPlaintext(res, pars);
                }
                if (pars == null) 
                    res.AppendFormat("\r\n{0}", (char)7);
                else 
                {
                    res.Append(pars.NewLine ?? "");
                    res.Append(pars.TableRowEnd ?? "");
                }
                if (pars != null && pars.MaxTextLength > 0 && res.Length > pars.MaxTextLength) 
                    break;
            }
            if (pars.TableEnd != null) 
                res.Append(pars.TableEnd);
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            res.AppendFormat("\r\n<table border=\"{1}\" width=\"{2}\"{0}", (MayHasError ? " style=\"border-color:red\"" : ""), (MayHasError ? 3 : (HideBorders ? 0 : 1)), Width ?? "100%");
            if (Id != null) 
            {
                res.AppendFormat(" id=\"{0}\"", Id);
                if (par != null && par.Styles.ContainsKey(Id)) 
                    res.AppendFormat(" style=\"{0}\"", par.Styles[Id]);
                UnitextStyledFragment fr = this.GetStyledFragment(-1);
                if (fr != null && fr.Typ != UnitextStyledFragmentType.Table) 
                    fr = fr.Parent;
                if ((fr != null && fr.Typ == UnitextStyledFragmentType.Table && fr.StyleId > 0) && fr.Style != null) 
                {
                    res.Append(" style=\"");
                    fr.Style.GetHtml(res);
                    res.Append("\"");
                }
            }
            res.Append(">");
            if (m_ColWidth.Count > 0) 
            {
                res.Append("<colgroup>");
                for (int i = 0; i < ColsCount; i++) 
                {
                    res.Append("<col");
                    if ((i < m_ColWidth.Count) && m_ColWidth[i] != null) 
                        res.AppendFormat(" width=\"{0}\"", m_ColWidth[i]);
                    res.Append("/>");
                }
                res.Append("</colgroup>");
            }
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                res.Append("\r\n <tr>");
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    UnitextTablecell cel = m_Cells[r][c];
                    if (cel == null) 
                        continue;
                    if (r == cel.RowBegin && c == cel.ColBegin) 
                    {
                        res.Append("\r\n   ");
                        cel.GetHtml(res, par);
                    }
                }
                res.Append("\r\n </tr>");
                if (res.Length > 10000000) 
                    break;
            }
            res.Append("\r\n</table>\r\n");
            if (par != null) 
                par._outFootnotes(res);
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("table");
            this._writeXmlAttrs(xml);
            xml.WriteAttributeString("rows", RowsCount.ToString());
            xml.WriteAttributeString("columns", ColsCount.ToString());
            if (Width != null) 
                xml.WriteAttributeString("width", Width);
            if (HideBorders) 
                xml.WriteAttributeString("border", "hide");
            for (int i = 0; i < m_ColWidth.Count; i++) 
            {
                if (m_ColWidth[i] != null) 
                {
                    xml.WriteStartElement("col");
                    xml.WriteAttributeString("num", i.ToString());
                    xml.WriteAttributeString("width", m_ColWidth[i]);
                    xml.WriteEndElement();
                }
            }
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                xml.WriteStartElement("row");
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    UnitextTablecell cel = m_Cells[r][c];
                    if (cel == null) 
                        continue;
                    if (r == cel.RowBegin && c == cel.ColBegin) 
                        cel.GetXml(xml);
                }
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
            int rows = 0;
            int cols = 0;
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "rows") 
                        rows = int.Parse(a.Value);
                    else if (a.LocalName == "columns") 
                        m_ColsCount = (cols = int.Parse(a.Value));
                    else if (a.LocalName == "width") 
                        Width = a.Value;
                    else if (a.LocalName == "border") 
                    {
                        if (a.Value == "hide") 
                            HideBorders = true;
                    }
                }
            }
            for (int r = 0; r < rows; r++) 
            {
                List<UnitextTablecell> row = new List<UnitextTablecell>();
                for (int c = 0; c < cols; c++) 
                {
                    row.Add(null);
                }
                m_Cells.Add(row);
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "row") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        UnitextTablecell cel = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx) as UnitextTablecell;
                        if (cel == null) 
                            continue;
                        if (cel.ColEnd >= cols || cel.RowEnd >= rows) 
                            continue;
                        for (int r = cel.RowBegin; r <= cel.RowEnd; r++) 
                        {
                            for (int c = cel.ColBegin; c <= cel.ColEnd; c++) 
                            {
                                m_Cells[r][c] = cel;
                            }
                        }
                    }
                }
                else if (x.LocalName == "col") 
                {
                    int num = 0;
                    string val = null;
                    if (x.Attributes != null) 
                    {
                        foreach (XmlAttribute a in x.Attributes) 
                        {
                            if (a.LocalName == "width") 
                                val = a.Value;
                            else if (a.LocalName == "num") 
                                int.TryParse(a.Value ?? "", out num);
                        }
                    }
                    if (val != null) 
                        this.SetColWidth(num, val);
                }
            }
        }
        public override void GetAllItems(List<UnitextItem> res, int lev)
        {
            if (res != null) 
                res.Add(this);
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    UnitextTablecell cel = m_Cells[r][c];
                    if (cel == null) 
                        continue;
                    if (r == cel.RowBegin && c == cel.ColBegin) 
                    {
                        cel.Parent = this;
                        cel.GetAllItems(res, lev + 1);
                    }
                }
            }
        }
        internal override void AddPlainTextPos(int d)
        {
            base.AddPlainTextPos(d);
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    UnitextTablecell cel = m_Cells[r][c];
                    if (cel == null) 
                        continue;
                    if (r == cel.RowBegin && c == cel.ColBegin) 
                        cel.AddPlainTextPos(d);
                }
            }
        }
        internal override void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    UnitextTablecell cel = m_Cells[r][c];
                    if (cel == null) 
                        continue;
                    if (r == cel.RowBegin && c == cel.ColBegin) 
                        cel.Correct(typ, data);
                }
            }
        }
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = cp;
            for (int r = 0; r < m_Cells.Count; r++) 
            {
                for (int c = 0; c < m_Cells[r].Count; c++) 
                {
                    UnitextTablecell cel = m_Cells[r][c];
                    if (cel == null) 
                        continue;
                    if (r == cel.RowBegin && c == cel.ColBegin) 
                    {
                        cel.SetDefaultTextPos(ref cp, res);
                        cel.Parent = this;
                    }
                }
            }
            EndChar = cp - 1;
        }
    }
}