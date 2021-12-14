/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Pullenti.Unitext.Internal.Word
{
    class DocTable
    {
        public int Cols = 0;
        public List<DocTableRow> Rows = new List<DocTableRow>();
        public List<int> ColWidth = new List<int>();
        public bool HideBorders = false;
        Pullenti.Unitext.UnitextStyledFragment m_Style;
        public void Read(DocxToText own, Pullenti.Unitext.UnitextStyledFragment sfrag, List<XmlNode> stackNodes)
        {
            if (sfrag != null) 
            {
                m_Style = new Pullenti.Unitext.UnitextStyledFragment() { Typ = Pullenti.Unitext.UnitextStyledFragmentType.Table, Parent = sfrag };
                sfrag.Children.Add(m_Style);
            }
            XmlNode xml = stackNodes[stackNodes.Count - 1];
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "tblGrid") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        if (xx.LocalName == "gridCol") 
                        {
                            Cols++;
                            if (xx.Attributes != null) 
                            {
                                foreach (XmlAttribute a in xx.Attributes) 
                                {
                                    if (a.LocalName == "w") 
                                    {
                                        int n;
                                        if (int.TryParse(a.Value ?? "", out n)) 
                                            ColWidth.Add(n);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (x.LocalName == "tblPr") 
                {
                    if (m_Style != null) 
                    {
                        Pullenti.Unitext.UnitextStyle st0 = new Pullenti.Unitext.UnitextStyle();
                        own.m_Styles.ReadUnitextStyle(x, st0);
                        st0.RemoveInheritAttrs(m_Style);
                        m_Style.Style = own.m_Styles.RegisterStyle(st0);
                    }
                    int cou = 0;
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        if (xx.LocalName == "tblBorders") 
                        {
                            foreach (XmlNode xxx in xx.ChildNodes) 
                            {
                                if (((xxx.LocalName == "top" || xxx.LocalName == "left" || xxx.LocalName == "right") || xxx.LocalName == "bottom" || xxx.LocalName == "start") || xxx.LocalName == "end") 
                                {
                                    if (xxx.Attributes != null) 
                                    {
                                        foreach (XmlAttribute a in xxx.Attributes) 
                                        {
                                            if (a.LocalName == "color") 
                                            {
                                                if (a.Value == "FFFFFF") 
                                                    cou++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (cou == 4) 
                        HideBorders = true;
                }
                else if (x.LocalName == "tr") 
                {
                    DocTableRow tr = new DocTableRow();
                    Rows.Add(tr);
                    int firstSpan = 0;
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        if (xx.LocalName == "tc") 
                        {
                            DocTableCell tc = new DocTableCell();
                            tr.Cells.Add(tc);
                            stackNodes.Add(xx);
                            tc.Read(own, m_Style, stackNodes);
                            stackNodes.RemoveAt(stackNodes.Count - 1);
                            if (firstSpan > 0) 
                            {
                                tc.ColSpan += firstSpan;
                                firstSpan = 0;
                            }
                        }
                        else if (xx.LocalName == "sdt") 
                        {
                            foreach (XmlNode xxx in xx.ChildNodes) 
                            {
                                if (xxx.LocalName == "sdtContent") 
                                {
                                    foreach (XmlNode xxxx in xxx.ChildNodes) 
                                    {
                                        if (xxxx.LocalName == "tc") 
                                        {
                                            DocTableCell tc = new DocTableCell();
                                            tr.Cells.Add(tc);
                                            stackNodes.Add(xxxx);
                                            tc.Read(own, m_Style, stackNodes);
                                            stackNodes.RemoveAt(stackNodes.Count - 1);
                                            if (firstSpan > 0) 
                                            {
                                                tc.ColSpan += firstSpan;
                                                firstSpan = 0;
                                            }
                                            if (tc.Uni is Pullenti.Unitext.UnitextContainer) 
                                                (tc.Uni as Pullenti.Unitext.UnitextContainer).Typ = Pullenti.Unitext.UnitextContainerType.ContentControl;
                                            else 
                                            {
                                                Pullenti.Unitext.UnitextContainer ccc = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.ContentControl, m_StyledFrag = m_Style };
                                                ccc.Children.Add(tc.Uni);
                                                tc.Uni.Parent = ccc;
                                                tc.Uni = ccc;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (xx.LocalName == "trPr") 
                        {
                            foreach (XmlNode xxx in xx.ChildNodes) 
                            {
                                if (xxx.LocalName == "gridBefore" && xxx.Attributes != null && xxx.Attributes.Count == 1) 
                                {
                                    int cou;
                                    if (int.TryParse(xxx.Attributes[0].InnerText, out cou)) 
                                    {
                                        if (cou > 0) 
                                            firstSpan = cou;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (ColWidth.Count == Cols) 
            {
                int nn = 0;
                foreach (int w in ColWidth) 
                {
                    nn += w;
                }
                if (nn > 0) 
                {
                    for (int ii = 0; ii < ColWidth.Count; ii++) 
                    {
                        ColWidth[ii] = (ColWidth[ii] * 100) / nn;
                        if (ColWidth[ii] == 0) 
                            ColWidth[ii] = 1;
                    }
                }
                else 
                    ColWidth.Clear();
            }
            else 
                ColWidth.Clear();
            for (int i = 0; i < Rows.Count; i++) 
            {
                DocTableRow r = Rows[i];
                int grNum = 1;
                for (int j = 0; j < r.Cells.Count; j++) 
                {
                    DocTableCell c = r.Cells[j];
                    c.GridNum = grNum;
                    grNum += c.ColSpan;
                    if (c.MergeVert != 1) 
                        continue;
                    DocTableCell spanCell = null;
                    for (int ii = i - 1; ii >= 0; ii--) 
                    {
                        DocTableRow rr = Rows[ii];
                        int grrNum = 1;
                        for (int jj = 0; jj < rr.Cells.Count; jj++) 
                        {
                            DocTableCell cc = rr.Cells[jj];
                            if (cc.MergeVert == 2 && ((c.GridNum == cc.GridNum || c.GridNum == grrNum))) 
                            {
                                cc.RowSpan++;
                                spanCell = cc;
                                break;
                            }
                            grrNum += cc.ColSpan;
                        }
                        if (spanCell != null) 
                            break;
                    }
                    if (spanCell == null) 
                    {
                    }
                }
            }
        }
        public Pullenti.Unitext.UnitextTable CreateUni()
        {
            if (Rows.Count == 0) 
                return null;
            Pullenti.Unitext.UnitextTable tab = new Pullenti.Unitext.UnitextTable();
            tab.m_StyledFrag = m_Style;
            tab.HideBorders = HideBorders;
            for (int ii = 0; ii < ColWidth.Count; ii++) 
            {
                tab.SetColWidth(ii, string.Format("{0}%", ColWidth[ii]));
            }
            int rn = 0;
            foreach (DocTableRow r in Rows) 
            {
                int cn = 0;
                foreach (DocTableCell c in r.Cells) 
                {
                    if (c.MergeVert == 1) 
                        continue;
                    for (; ; cn++) 
                    {
                        if (tab.GetCell(rn, cn) == null) 
                            break;
                    }
                    Pullenti.Unitext.UnitextTablecell cel = tab.AddCell(rn, (rn + c.RowSpan) - 1, cn, (cn + c.ColSpan) - 1, c.Uni);
                    cel.m_StyledFrag = c.Frag;
                    if (c.Uni != null && c.Uni.m_StyledFrag == c.Frag) 
                        c.Uni.m_StyledFrag = null;
                }
                rn++;
            }
            return tab;
        }
    }
}