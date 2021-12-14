/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // The kind of compression used for an entry in an archive
    enum CompressionMethod : int
    {
        Stored = 0,
        Deflated = 8,
        Deflate64 = 9,
        BZip2 = 11,
        WinZipAES = 99,
    }
}