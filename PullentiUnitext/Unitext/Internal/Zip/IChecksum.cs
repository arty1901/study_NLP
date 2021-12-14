/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // A data checksum can be updated by one byte or with a byte array. After each
    // update the value of the current checksum can be returned by calling
    // <code>getValue</code>. The complete checksum object can also be reset
    // so it can be used again with new data.
    interface IChecksum
    {
        uint Value { get; }
        void Reset();
        void UpdateByVal(int value);
        void UpdateByBuf(byte[] buffer);
        void UpdateByBufEx(byte[] buffer, int offset, int count);
    }
}