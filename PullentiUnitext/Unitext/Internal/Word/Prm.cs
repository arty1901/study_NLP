/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Word
{
    class Prm
    {
        internal ushort prm;
        internal bool fComplex
        {
            get
            {
                return ((prm & 1)) != 0;
            }
        }
        internal byte isprm
        {
            get
            {
                return (byte)((((prm >> 1)) & 0x7F));
            }
        }
        internal byte val
        {
            get
            {
                return (byte)((((prm >> 8)) & 0xFF));
            }
        }
        internal byte igrpprl
        {
            get
            {
                return (byte)((((prm >> 1)) & 0x7FFF));
            }
        }
    }
}