/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Defines the contents of the general bit flags field for an archive entry.
    enum GeneralBitFlags : int
    {
        Encrypted = 0x0001,
        Method = 0x0006,
        Descriptor = 0x0008,
        ReservedPKware4 = 0x0010,
        Patched = 0x0020,
        StrongEncryption = 0x0040,
        Unused7 = 0x0080,
        Unused8 = 0x0100,
        Unused9 = 0x0200,
        Unused10 = 0x0400,
        UnicodeText = 0x0800,
        EnhancedCompress = 0x1000,
        HeaderMasked = 0x2000,
        ReservedPkware14 = 0x4000,
        ReservedPkware15 = 0x8000,
    }
}