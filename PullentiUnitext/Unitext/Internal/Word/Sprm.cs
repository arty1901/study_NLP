/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Word
{
    class Sprm
    {
        internal ushort ispmd
        {
            get
            {
                return (ushort)((sprm & 0x1FF));
            }
        }
        internal bool fSpec
        {
            get
            {
                return ((sprm & 0x200)) != 0;
            }
        }
        internal byte sgc
        {
            get
            {
                return (byte)((((sprm >> 10)) & 0x07));
            }
        }
        internal byte spra
        {
            get
            {
                return (byte)((((sprm >> 13)) & 0x07));
            }
        }
        internal ushort sprm;
        internal Sprm(ushort sprm = (ushort)0)
        {
            this.sprm = sprm;
        }
        public override string ToString()
        {
            string sprmName;
            if (SinglePropertyModifiers.map.TryGetValue(sprm, out sprmName)) 
                return sprmName;
            else 
                return "sprm: 0x" + sprm.ToString("X4");
        }
    }
}