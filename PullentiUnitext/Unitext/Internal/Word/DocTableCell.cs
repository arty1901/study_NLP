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
    class DocTableCell
    {
        public int GridNum;
        public int ColSpan = 1;
        public int RowSpan = 1;
        public int MergeVert = 0;
        public string Text;
        public Pullenti.Unitext.UnitextItem Uni;
        public Pullenti.Unitext.UnitextStyledFragment Frag;
        public override string ToString()
        {
            return string.Format("N={0}, PS={1}x{2}, V={3} : {4}", GridNum, ColSpan, RowSpan, MergeVert, Text ?? "");
        }
        public void Read(DocxToText own, Pullenti.Unitext.UnitextStyledFragment sfrag, List<XmlNode> stackNodes)
        {
            XmlNode xml = stackNodes[stackNodes.Count - 1];
            if (sfrag != null) 
            {
                Frag = new Pullenti.Unitext.UnitextStyledFragment() { Typ = Pullenti.Unitext.UnitextStyledFragmentType.Tablecell, Parent = sfrag };
                sfrag.Children.Add(Frag);
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "tcPr") 
                {
                    if (Frag != null) 
                    {
                        Pullenti.Unitext.UnitextStyle st0 = new Pullenti.Unitext.UnitextStyle();
                        own.m_Styles.ReadUnitextStyle(x, st0);
                        st0.RemoveInheritAttrs(Frag);
                        Frag.Style = own.m_Styles.RegisterStyle(st0);
                    }
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        if (xx.LocalName == "gridSpan" && xx.Attributes != null && xx.Attributes.Count == 1) 
                        {
                            int cou;
                            if (int.TryParse(xx.Attributes[0].InnerText, out cou)) 
                            {
                                if (cou > 1) 
                                    ColSpan = cou;
                            }
                        }
                        else if (xx.LocalName == "vMerge") 
                        {
                            MergeVert = 1;
                            if (xx.Attributes.Count > 0 && xx.Attributes != null && xx.Attributes[0].Value == "restart") 
                                MergeVert = 2;
                        }
                    }
                }
            }
            Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
            gen.SetStyle(Frag);
            stackNodes.Add(xml);
            own.ReadNode(stackNodes, gen, Frag, -1);
            stackNodes.RemoveAt(stackNodes.Count - 1);
            Uni = gen.Finish(true, null);
        }
    }
}