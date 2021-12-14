/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Word
{
    class CFHeader
    {
        internal const uint DefaultSignature1 = 0xE011CFD0;
        internal const uint DefaultSignature2 = 0xE11AB1A1;
        internal uint Signature1;
        internal uint Signature2;
        internal Guid CLSID;
        internal ushort MinorVersion;
        internal ushort MajorVersion;
        internal ushort ByteOrder;
        internal ushort SectorShift;
        internal ushort MiniSectorShift;
        internal byte[] Reserved;
        internal uint DirectorySectorsCount;
        internal uint FATSectorsCount;
        internal uint FirstDirectorySectorLocation;
        internal uint TransactionSignatureNumber;
        internal uint MiniStreamCutoffSize;
        internal uint FirstMiniFATSectorLocation;
        internal uint MiniFATSectorsCount;
        internal uint FirstDIFATSectorLocation;
        internal uint DIFATSectorsCount;
        internal uint[] DIFAT;
    }
}