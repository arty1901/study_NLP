/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Misc
{
    class PngHead : PngChunk
    {
        public int Width
        {
            get
            {
                int i0 = 0;
                int res = (int)Data[i0];
                res <<= 8;
                res |= Data[i0 + 1];
                res <<= 8;
                res |= Data[i0 + 2];
                res <<= 8;
                res |= Data[i0 + 3];
                return res;
            }
            set
            {
                int i0 = 0;
                Data[i0] = (byte)((((value >> 24)) & 0xFF));
                Data[i0 + 1] = (byte)((((value >> 16)) & 0xFF));
                Data[i0 + 2] = (byte)((((value >> 8)) & 0xFF));
                Data[i0 + 3] = (byte)(((value) & 0xFF));
            }
        }
        public int Height
        {
            get
            {
                int i0 = 4;
                int res = (int)Data[i0];
                res <<= 8;
                res |= Data[i0 + 1];
                res <<= 8;
                res |= Data[i0 + 2];
                res <<= 8;
                res |= Data[i0 + 3];
                return res;
            }
            set
            {
                int i0 = 4;
                Data[i0] = (byte)((((value >> 24)) & 0xFF));
                Data[i0 + 1] = (byte)((((value >> 16)) & 0xFF));
                Data[i0 + 2] = (byte)((((value >> 8)) & 0xFF));
                Data[i0 + 3] = (byte)(((value) & 0xFF));
            }
        }
        public byte BitDepth
        {
            get
            {
                return Data[8];
            }
            set
            {
                Data[8] = value;
            }
        }
        public byte ColorType
        {
            get
            {
                return Data[9];
            }
            set
            {
                Data[9] = value;
            }
        }
        public byte Compression
        {
            get
            {
                return Data[10];
            }
            set
            {
                Data[10] = value;
            }
        }
        public byte Filter
        {
            get
            {
                return Data[11];
            }
            set
            {
                Data[11] = value;
            }
        }
        public byte Interlace
        {
            get
            {
                return Data[12];
            }
            set
            {
                Data[12] = value;
            }
        }
        public PngHead() : base("IHDR")
        {
            Data = new byte[(int)13];
            BitDepth = 8;
        }
    }
}