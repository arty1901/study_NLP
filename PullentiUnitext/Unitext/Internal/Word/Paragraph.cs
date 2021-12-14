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
    class Paragraph : IComparable<Paragraph>
    {
        private WordDocument owner;
        public int Offset
        {
            get;
            set;
        }
        public int Pos
        {
            get
            {
                return FileCharacterPosition.CharacterIndex + Offset;
            }
        }
        public int Length
        {
            get;
            set;
        }
        internal WordDocument.FileCharacterPosition FileCharacterPosition
        {
            get;
            set;
        }
        internal PapxInFkps PapxInFkps
        {
            get;
            set;
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.AppendFormat("Pos:{0}, Len:{1}", FileCharacterPosition.CharacterIndex + Offset, Length);
            if (IsInTable) 
                res.AppendFormat(" InTable ({0})", TableDepth);
            if (IsList) 
                res.AppendFormat(" IsList ({0})", ListLevel);
            if (IsTableCellEnd) 
                res.Append(" TableCellEnd");
            if (IsTableRowEnd) 
                res.Append(" TableRowEnd");
            res.AppendFormat(" {0}", this.GetStyles(FormattingLevel.All));
            if (PapxInFkps != null && PapxInFkps.grpprlInPapx != null && PapxInFkps.grpprlInPapx.grpprl != null) 
            {
                foreach (Prl p in PapxInFkps.grpprlInPapx.grpprl) 
                {
                    res.AppendFormat(" {0}", SinglePropertyModifiers.GetSprmName(p.sprm.sprm) ?? "");
                }
            }
            foreach (Prl p in FileCharacterPosition.Prls) 
            {
                res.AppendFormat(" {0}", SinglePropertyModifiers.GetSprmName(p.sprm.sprm) ?? "");
            }
            return res.ToString();
        }
        public bool IsInTable
        {
            get
            {
                byte[] data = this.GetProperty(SinglePropertyModifiers.sprmPFInTable);
                return data != null && data[0] != 0;
            }
        }
        public int TableDepth
        {
            get
            {
                byte[] data = this.GetProperty(SinglePropertyModifiers.sprmPItap);
                if (data == null) 
                    return 0;
                else 
                    return (int)BitConverter.ToUInt32(data, 0);
            }
        }
        public bool IsTableRowEnd
        {
            get
            {
                int pos = (FileCharacterPosition.CharacterIndex + Offset + Length) - 1;
                if ((pos < owner.Text.Length) && owner.Text[pos] == '\u0007') 
                {
                    byte[] data = this.GetProperty(SinglePropertyModifiers.sprmPFTtp);
                    if (data != null && data[0] != 0) 
                        return true;
                }
                else 
                {
                    byte[] data = this.GetProperty(SinglePropertyModifiers.sprmPFInnerTtp);
                    if (data != null && data[0] != 0) 
                        return true;
                }
                return false;
            }
        }
        public bool IsTableCellEnd
        {
            get
            {
                int pos = (FileCharacterPosition.CharacterIndex + Offset + Length) - 1;
                if ((pos < owner.Text.Length) && owner.Text[pos] == '\u0007') 
                    return true;
                else 
                {
                    byte[] data = this.GetProperty(SinglePropertyModifiers.sprmPFInnerTableCell);
                    return data != null && data[0] != 0;
                }
            }
        }
        public bool IsList
        {
            get
            {
                byte[] data = this.GetProperty(SinglePropertyModifiers.sprmPIlfo);
                return data != null && BitConverter.ToInt16(data, 0) != 0;
            }
        }
        public int ListLevel
        {
            get
            {
                byte[] data = this.GetProperty(SinglePropertyModifiers.sprmPIlvl);
                return (data != null ? data[0] : (byte)0);
            }
        }
        public StyleDefinition Style
        {
            get
            {
                return owner.StyleDefinitionsMap[PapxInFkps.grpprlInPapx.istd];
            }
        }
        internal Paragraph(WordDocument owner, int offset, int length, WordDocument.FileCharacterPosition fcp, PapxInFkps papxInFkps)
        {
            this.owner = owner;
            this.Offset = offset;
            this.Length = length;
            this.FileCharacterPosition = fcp;
            this.PapxInFkps = papxInFkps;
        }
        public byte[] GetProperty(ushort sprm)
        {
            if (PapxInFkps != null && PapxInFkps.grpprlInPapx != null && PapxInFkps.grpprlInPapx.grpprl != null) 
            {
                foreach (Prl p in PapxInFkps.grpprlInPapx.grpprl) 
                {
                    if (p.sprm.sprm == sprm) 
                        return p.operand;
                }
            }
            foreach (Prl p in FileCharacterPosition.Prls) 
            {
                if (p.sprm.sprm == sprm) 
                    return p.operand;
            }
            return null;
        }
        public StyleCollection GetStyles(FormattingLevel level)
        {
            if (level != FormattingLevel.Paragraph && level != FormattingLevel.ParagraphStyle) 
                return null;
            List<Prl[]> prls = new List<Prl[]>();
            prls.Add(PapxInFkps.grpprlInPapx.grpprl);
            if (level == FormattingLevel.ParagraphStyle) 
                this.Style.ExpandStyles(prls);
            return new StyleCollection(prls);
        }
        public int CompareTo(Paragraph other)
        {
            if (Pos < other.Pos) 
                return -1;
            if (Pos > other.Pos) 
                return 1;
            if (Length < other.Length) 
                return -1;
            if (Length > other.Length) 
                return 1;
            return 0;
        }
    }
}