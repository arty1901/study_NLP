/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    class InflaterDynHeader
    {
        const int LNUM = 0;
        const int DNUM = 1;
        const int BLNUM = 2;
        const int BLLENS = 3;
        const int LENS = 4;
        const int REPS = 5;
        static int[] repMin = new int[] {3, 3, 11};
        static int[] repBits = new int[] {2, 3, 7};
        static int[] BL_ORDER = new int[] {16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15};
        public InflaterDynHeader()
        {
        }
        public bool Decode(StreamManipulator input)
        {
            for (; ; ) 
            {
                switch (mode) { 
                case LNUM:
                    m_Lnum = input.PeekBits(5);
                    if (m_Lnum < 0) 
                        return false;
                    m_Lnum += 257;
                    input.DropBits(5);
                    mode = DNUM;
                    break;
                case DNUM:
                    m_Dnum = input.PeekBits(5);
                    if (m_Dnum < 0) 
                        return false;
                    m_Dnum++;
                    input.DropBits(5);
                    m_Num = m_Lnum + m_Dnum;
                    litdistLens = new byte[(int)m_Num];
                    mode = BLNUM;
                    break;
                case BLNUM:
                    m_Blnum = input.PeekBits(4);
                    if (m_Blnum < 0) 
                        return false;
                    m_Blnum += 4;
                    input.DropBits(4);
                    blLens = new byte[(int)19];
                    ptr = 0;
                    mode = BLLENS;
                    break;
                case BLLENS:
                    while (ptr < m_Blnum) 
                    {
                        int len = input.PeekBits(3);
                        if (len < 0) 
                            return false;
                        input.DropBits(3);
                        blLens[BL_ORDER[ptr]] = (byte)len;
                        ptr++;
                    }
                    blTree = new InflaterHuffmanTree(blLens);
                    blLens = null;
                    ptr = 0;
                    mode = LENS;
                    break;
                case LENS:
                        {
                            int symbol;
                            while ((((((symbol = blTree.GetSymbol(input)))) & (~15))) == 0) 
                            {
                                litdistLens[ptr++] = (lastLen = (byte)symbol);
                                if (ptr == m_Num) 
                                    return true;
                            }
                            if (symbol < 0) 
                                return false;
                            if (symbol >= 17) 
                                lastLen = 0;
                            else if (ptr == 0) 
                                throw new Exception();
                            repSymbol = symbol - 16;
                        }
                    mode = REPS;
                    break;
                case REPS:
                        {
                            int bits = repBits[repSymbol];
                            int count = input.PeekBits(bits);
                            if (count < 0) 
                                return false;
                            input.DropBits(bits);
                            count += repMin[repSymbol];
                            if ((ptr + count) > m_Num) 
                                throw new Exception();
                            while ((count--) > 0) 
                            {
                                litdistLens[ptr++] = lastLen;
                            }
                            if (ptr == m_Num) 
                                return true;
                        }
                    mode = LENS;
                    break;
                }
            }
        }
        public InflaterHuffmanTree BuildLitLenTree()
        {
            byte[] litlenLens = new byte[(int)m_Lnum];
            Array.Copy(litdistLens, 0, litlenLens, 0, m_Lnum);
            return new InflaterHuffmanTree(litlenLens);
        }
        public InflaterHuffmanTree BuildDistTree()
        {
            byte[] distLens = new byte[(int)m_Dnum];
            Array.Copy(litdistLens, m_Lnum, distLens, 0, m_Dnum);
            return new InflaterHuffmanTree(distLens);
        }
        byte[] blLens;
        byte[] litdistLens;
        InflaterHuffmanTree blTree;
        int mode;
        int m_Lnum;
        int m_Dnum;
        int m_Blnum;
        int m_Num;
        int repSymbol;
        byte lastLen;
        int ptr;
    }
}