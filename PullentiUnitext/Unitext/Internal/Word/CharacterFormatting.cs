/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Word
{
    class CharacterFormatting
    {
        private WordDocument owner;
        public int Offset
        {
            get;
            set;
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
        public int Pos
        {
            get
            {
                return FileCharacterPosition.CharacterIndex + Offset;
            }
        }
        internal Chpx Chpx
        {
            get;
            set;
        }
        public override string ToString()
        {
            return string.Format("Pos:{0} Len:{1} Styles:{2}", Pos, Length, this.GetStyles(FormattingLevel.Character).ToString());
        }
        public StyleDefinition Style
        {
            get
            {
                byte[] istdData = this.GetProperty(SinglePropertyModifiers.sprmCIstd);
                if (istdData != null) 
                {
                    ushort istd = BitConverter.ToUInt16(istdData, 0);
                    if (owner.StyleDefinitionsMap.ContainsKey(istd)) 
                        return owner.StyleDefinitionsMap[istd];
                }
                return null;
            }
        }
        internal CharacterFormatting(WordDocument owner, int offset, int length, WordDocument.FileCharacterPosition fcp, Chpx chpx)
        {
            this.owner = owner;
            this.Offset = offset;
            this.Length = length;
            this.FileCharacterPosition = fcp;
            this.Chpx = chpx;
        }
        public byte[] GetProperty(ushort sprm)
        {
            foreach (Prl p in Chpx.grpprl) 
            {
                if (p.sprm.sprm == sprm) 
                    return p.operand;
            }
            foreach (Prl p in FileCharacterPosition.Prls) 
            {
                if (p.sprm.sprm == sprm) 
                    return p.operand;
            }
            return null;
        }
        public StyleCollection GetStyles(FormattingLevel level = FormattingLevel.Character)
        {
            List<Prl[]> prls = new List<Prl[]>();
            prls.Add(Chpx.grpprl);
            if (level == FormattingLevel.CharacterStyle) 
            {
                StyleDefinition def = this.Style;
                if (def != null) 
                    def.ExpandStyles(prls);
            }
            return new StyleCollection(prls);
        }
    }
}