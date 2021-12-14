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

namespace Pullenti.Unitext.Internal.Word
{
    class StyleDefinition
    {
        WordDocument owner;
        STD std;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.AppendFormat("'{0}'", Name);
            tmp.AppendFormat(this.GetStyles().ToString());
            return tmp.ToString();
        }
        public string Name
        {
            get
            {
                return std.xstzName.ToString();
            }
        }
        public bool IsTextStyle
        {
            get
            {
                return std.stdf.stdfBase.stk == ((byte)GrLPUpxSw.StkCharGRLPUPXStkValue);
            }
        }
        internal StyleDefinition(WordDocument owner, STD std)
        {
            this.owner = owner;
            this.std = std;
        }
        public StyleCollection GetStyles()
        {
            List<Prl[]> styles = new List<Prl[]>();
            this.ExpandStyles(styles);
            return new StyleCollection(styles);
        }
        internal void ExpandStyles(List<Prl[]> styles)
        {
            if (std.stdf.stdfBase.istdBase != StdfBase.IstdNull) 
                owner.StyleDefinitionsMap[std.stdf.stdfBase.istdBase].ExpandStyles(styles);
            switch (std.stdf.stdfBase.stk) { 
            case GrLPUpxSw.StkParaGRLPUPXStkValue:
                styles.Add(((StkParaGRLPUPX)std.grLPUpxSw).upxPapx.grpprlPapx);
                styles.Add(((StkParaGRLPUPX)std.grLPUpxSw).upxChpx.grpprlChpx);
                break;
            case GrLPUpxSw.StkCharGRLPUPXStkValue:
                styles.Add(((StkCharGRLPUPX)std.grLPUpxSw).upxChpx.grpprlChpx);
                break;
            case GrLPUpxSw.StkTableGRLPUPXStkValue:
                styles.Add(((StkTableGRLPUPX)std.grLPUpxSw).upxTapx.grpprlTapx);
                styles.Add(((StkTableGRLPUPX)std.grLPUpxSw).upxPapx.grpprlPapx);
                styles.Add(((StkTableGRLPUPX)std.grLPUpxSw).upxChpx.grpprlChpx);
                break;
            case GrLPUpxSw.StkListGRLPUPXStkValue:
                styles.Add(((StkListGRLPUPX)std.grLPUpxSw).upxPapx.grpprlPapx);
                break;
            }
        }
    }
}