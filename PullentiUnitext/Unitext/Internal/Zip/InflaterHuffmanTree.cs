/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Huffman tree used for inflation
    class InflaterHuffmanTree
    {
        const int MAX_BITLEN = 15;
        short[] tree;
        public static InflaterHuffmanTree defLitLenTree;
        public static InflaterHuffmanTree defDistTree;
        static InflaterHuffmanTree()
        {
            try 
            {
                byte[] codeLengths = new byte[(int)288];
                int i = 0;
                while (i < 144) 
                {
                    codeLengths[i++] = 8;
                }
                while (i < 256) 
                {
                    codeLengths[i++] = 9;
                }
                while (i < 280) 
                {
                    codeLengths[i++] = 7;
                }
                while (i < 288) 
                {
                    codeLengths[i++] = 8;
                }
                defLitLenTree = new InflaterHuffmanTree(codeLengths);
                codeLengths = new byte[(int)32];
                i = 0;
                while (i < 32) 
                {
                    codeLengths[i++] = 5;
                }
                defDistTree = new InflaterHuffmanTree(codeLengths);
            }
            catch(Exception ex) 
            {
            }
        }
        public InflaterHuffmanTree(byte[] codeLengths)
        {
            this.BuildTree(codeLengths);
        }
        void BuildTree(byte[] codeLengths)
        {
            int[] blCount = new int[(int)(MAX_BITLEN + 1)];
            int[] nextCode = new int[(int)(MAX_BITLEN + 1)];
            for (int i = 0; i < codeLengths.Length; i++) 
            {
                int bits = (int)codeLengths[i];
                if (bits > 0) 
                    blCount[bits]++;
            }
            int code = 0;
            int treeSize = 512;
            for (int bits = 1; bits <= MAX_BITLEN; bits++) 
            {
                nextCode[bits] = code;
                code += blCount[bits] << ((16 - bits));
                if (bits >= 10) 
                {
                    int start = nextCode[bits] & 0x1ff80;
                    int end = code & 0x1ff80;
                    treeSize += (((end - start)) >> ((16 - bits)));
                }
            }
            tree = new short[(int)treeSize];
            int treePtr = 512;
            for (int bits = MAX_BITLEN; bits >= 10; bits--) 
            {
                int end = code & 0x1ff80;
                code -= blCount[bits] << ((16 - bits));
                int start = code & 0x1ff80;
                for (int i = start; i < end; i += 1 << 7) 
                {
                    tree[(int)DeflaterHuffman.BitReverse(i)] = (short)((((-treePtr) << 4) | bits));
                    treePtr += 1 << ((bits - 9));
                }
            }
            for (int i = 0; i < codeLengths.Length; i++) 
            {
                int bits = (int)codeLengths[i];
                if (bits == 0) 
                    continue;
                code = nextCode[bits];
                int revcode = (int)DeflaterHuffman.BitReverse(code);
                if (bits <= 9) 
                {
                    do
                    {
                        tree[revcode] = (short)(((i << 4) | bits));
                        revcode += 1 << bits;
                    } while (revcode < 512); 
                }
                else 
                {
                    int subTree = (int)tree[revcode & 511];
                    int treeLen = 1 << ((subTree & 15));
                    subTree = -((subTree >> 4));
                    do
                    {
                        tree[subTree | ((revcode >> 9))] = (short)(((i << 4) | bits));
                        revcode += 1 << bits;
                    } while (revcode < treeLen); 
                }
                nextCode[bits] = code + (1 << ((16 - bits)));
            }
        }
        public int GetSymbol(StreamManipulator input)
        {
            int lookahead;
            int symbol;
            if ((((lookahead = input.PeekBits(9)))) >= 0) 
            {
                if ((((symbol = tree[lookahead]))) >= 0) 
                {
                    input.DropBits(symbol & 15);
                    return symbol >> 4;
                }
                int subtree = -((symbol >> 4));
                int bitlen = symbol & 15;
                if ((((lookahead = input.PeekBits(bitlen)))) >= 0) 
                {
                    symbol = tree[subtree | ((lookahead >> 9))];
                    input.DropBits(symbol & 15);
                    return symbol >> 4;
                }
                else 
                {
                    int bits = input.AvailableBits;
                    lookahead = input.PeekBits(bits);
                    symbol = tree[subtree | ((lookahead >> 9))];
                    if (((symbol & 15)) <= bits) 
                    {
                        input.DropBits(symbol & 15);
                        return symbol >> 4;
                    }
                    else 
                        return -1;
                }
            }
            else 
            {
                int bits = input.AvailableBits;
                lookahead = input.PeekBits(bits);
                symbol = tree[lookahead];
                if (symbol >= 0 && ((symbol & 15)) <= bits) 
                {
                    input.DropBits(symbol & 15);
                    return symbol >> 4;
                }
                else 
                    return -1;
            }
        }
    }
}