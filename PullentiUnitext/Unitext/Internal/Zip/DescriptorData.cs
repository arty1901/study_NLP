/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Holds data pertinent to a data descriptor.
    class DescriptorData
    {
        public int CompressedSize
        {
            get
            {
                return m_CompressedSize;
            }
            set
            {
                m_CompressedSize = value;
            }
        }
        public int Size
        {
            get
            {
                return m_Size;
            }
            set
            {
                m_Size = value;
            }
        }
        public uint Crc
        {
            get
            {
                return m_Crc;
            }
            set
            {
                m_Crc = value;
            }
        }
        int m_Size;
        int m_CompressedSize;
        uint m_Crc;
    }
}