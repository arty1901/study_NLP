/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Word
{
    class CFDirectoryEntry
    {
        internal char[] Name;
        internal ushort NameLength;
        internal byte ObjectType;
        internal byte ColorFlag;
        internal uint LeftSiblingID;
        internal uint RightSiblingID;
        internal uint ChildID;
        internal Guid CLSID;
        internal uint StateBits;
        internal uint StartingSectorLocation;
        internal uint StreamSize;
    }
}