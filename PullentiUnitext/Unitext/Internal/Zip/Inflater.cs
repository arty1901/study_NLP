/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Inflater is used to decompress data that has been compressed according
    // to the "deflate" standard described in rfc1951.
    // By default Zlib (rfc1950) headers and footers are expected in the input.
    // You can use constructor <code> public Inflater(bool noHeader)</code> passing true
    // if there is no Zlib header information
    // The usage is as following.  First you have to set some input with
    // <code>SetInput()</code>, then Inflate() it.  If inflate doesn't
    // inflate any bytes there may be three reasons:
    // <ul>
    // <li>IsNeedingInput() returns true because the input buffer is empty.
    // You have to provide more input with <code>SetInput()</code>.
    // NOTE: IsNeedingInput() also returns true when, the stream is finished.
    // </li>
    // <li>IsNeedingDictionary() returns true, you have to provide a preset
    // dictionary with <code>SetDictionary()</code>.</li>
    // <li>IsFinished returns true, the inflater has finished.</li>
    // </ul>
    // Once the first output byte is produced, a dictionary will not be
    // needed at a later stage.
    class Inflater
    {
        static int[] CPLENS = new int[] {3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258};
        static int[] CPLEXT = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0};
        static int[] CPDIST = new int[] {1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577};
        static int[] CPDEXT = new int[] {0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13};
        const int DECODE_HEADER = 0;
        const int DECODE_DICT = 1;
        const int DECODE_BLOCKS = 2;
        const int DECODE_STORED_LEN1 = 3;
        const int DECODE_STORED_LEN2 = 4;
        const int DECODE_STORED = 5;
        const int DECODE_DYN_HEADER = 6;
        const int DECODE_HUFFMAN = 7;
        const int DECODE_HUFFMAN_LENBITS = 8;
        const int DECODE_HUFFMAN_DIST = 9;
        const int DECODE_HUFFMAN_DISTBITS = 10;
        const int DECODE_CHKSUM = 11;
        const int FINISHED = 12;
        int mode;
        int readAdler;
        int neededBits;
        int repLength;
        int repDist;
        int uncomprLen;
        bool isLastBlock;
        int m_TotalOut;
        int m_TotalIn;
        bool noHeader;
        StreamManipulator input;
        OutputWindow outputWindow;
        InflaterDynHeader dynHeader;
        InflaterHuffmanTree litlenTree;
        InflaterHuffmanTree distTree;
        Adler32 m_Adler;
        public Inflater(bool noHeader = false)
        {
            this.noHeader = noHeader;
            this.m_Adler = new Adler32();
            input = new StreamManipulator();
            outputWindow = new OutputWindow();
            mode = (noHeader ? DECODE_BLOCKS : DECODE_HEADER);
        }
        public void Reset()
        {
            mode = (noHeader ? DECODE_BLOCKS : DECODE_HEADER);
            m_TotalIn = 0;
            m_TotalOut = 0;
            input.Reset();
            outputWindow.Reset();
            dynHeader = null;
            litlenTree = null;
            distTree = null;
            isLastBlock = false;
            m_Adler.Reset();
        }
        private bool DecodeHeaderEx()
        {
            int header = input.PeekBits(16);
            if (header < 0) 
                return false;
            input.DropBits(16);
            header = (((header << 8) | ((header >> 8)))) & 0xffff;
            if ((header % 31) != 0) 
                throw new Exception("Header checksum illegal");
            if (((header & 0x0f00)) != (Deflater.DEFLATED << 8)) 
                throw new Exception("Compression Method unknown");
            if (((header & 0x0020)) == 0) 
                mode = DECODE_BLOCKS;
            else 
            {
                mode = DECODE_DICT;
                neededBits = 32;
            }
            return true;
        }
        private bool DecodeDictEx()
        {
            while (neededBits > 0) 
            {
                int dictByte = input.PeekBits(8);
                if (dictByte < 0) 
                    return false;
                input.DropBits(8);
                readAdler = (readAdler << 8) | dictByte;
                neededBits -= 8;
            }
            return false;
        }
        private bool DecodeHuffmanEx()
        {
            int free = outputWindow.GetFreeSpace();
            int mode0 = mode;
            while (free >= 258) 
            {
                int symbol;
                switch (mode0) { 
                case DECODE_HUFFMAN:
                    while (true) 
                    {
                        symbol = litlenTree.GetSymbol(input);
                        if (((symbol & (~0xff))) != 0) 
                            break;
                        outputWindow.Write(symbol);
                        if ((--free) < 258) 
                            return true;
                    }
                    if (symbol < 257) 
                    {
                        if (symbol < 0) 
                            return false;
                        else 
                        {
                            distTree = null;
                            litlenTree = null;
                            mode = (mode0 = DECODE_BLOCKS);
                            return true;
                        }
                    }
                    try 
                    {
                        repLength = CPLENS[symbol - 257];
                        neededBits = CPLEXT[symbol - 257];
                    }
                    catch(Exception ex309) 
                    {
                        throw new Exception("Illegal rep length code");
                    }
                    mode0 = DECODE_HUFFMAN_LENBITS;
                    break;
                case DECODE_HUFFMAN_LENBITS:
                    if (neededBits > 0) 
                    {
                        mode = (mode0 = DECODE_HUFFMAN_LENBITS);
                        int i = input.PeekBits(neededBits);
                        if (i < 0) 
                            return false;
                        input.DropBits(neededBits);
                        repLength += i;
                    }
                    mode = (mode0 = DECODE_HUFFMAN_DIST);
                    break;
                case DECODE_HUFFMAN_DIST:
                    symbol = distTree.GetSymbol(input);
                    if (symbol < 0) 
                        return false;
                    try 
                    {
                        repDist = CPDIST[symbol];
                        neededBits = CPDEXT[symbol];
                    }
                    catch(Exception ex310) 
                    {
                        throw new Exception("Illegal rep dist code");
                    }
                    mode0 = DECODE_HUFFMAN_DISTBITS;
                    break;
                case DECODE_HUFFMAN_DISTBITS:
                    if (neededBits > 0) 
                    {
                        mode = (mode0 = DECODE_HUFFMAN_DISTBITS);
                        int i = input.PeekBits(neededBits);
                        if (i < 0) 
                            return false;
                        input.DropBits(neededBits);
                        repDist += i;
                    }
                    outputWindow.Repeat(repLength, repDist);
                    free -= repLength;
                    mode = (mode0 = DECODE_HUFFMAN);
                    break;
                default: 
                    throw new Exception("Inflater unknown mode");
                }
            }
            return true;
        }
        private bool DecodeChksumEx()
        {
            while (neededBits > 0) 
            {
                int chkByte = input.PeekBits(8);
                if (chkByte < 0) 
                    return false;
                input.DropBits(8);
                readAdler = (readAdler << 8) | chkByte;
                neededBits -= 8;
            }
            if (((int)m_Adler.Value) != readAdler) 
                throw new Exception(("Adler chksum doesn't match: " + ((int)m_Adler.Value) + " vs. ") + readAdler);
            mode = FINISHED;
            return false;
        }
        private bool Decode()
        {
            while (true) 
            {
                switch (mode) { 
                case DECODE_HEADER:
                    return this.DecodeHeaderEx();
                case DECODE_DICT:
                    return this.DecodeDictEx();
                case DECODE_CHKSUM:
                    return this.DecodeChksumEx();
                case DECODE_BLOCKS:
                    if (isLastBlock) 
                    {
                        if (noHeader) 
                        {
                            mode = FINISHED;
                            return false;
                        }
                        else 
                        {
                            input.SkipToByteBoundary();
                            neededBits = 32;
                            mode = DECODE_CHKSUM;
                            return true;
                        }
                    }
                    int type = input.PeekBits(3);
                    if (type < 0) 
                        return false;
                    input.DropBits(3);
                    if (((type & 1)) != 0) 
                        isLastBlock = true;
                    switch (type >> 1) { 
                    case DeflaterConstants.STORED_BLOCK:
                        input.SkipToByteBoundary();
                        mode = DECODE_STORED_LEN1;
                        break;
                    case DeflaterConstants.STATIC_TREES:
                        litlenTree = InflaterHuffmanTree.defLitLenTree;
                        distTree = InflaterHuffmanTree.defDistTree;
                        mode = DECODE_HUFFMAN;
                        break;
                    case DeflaterConstants.DYN_TREES:
                        dynHeader = new InflaterDynHeader();
                        mode = DECODE_DYN_HEADER;
                        break;
                    default: 
                        throw new Exception("Unknown block type " + type);
                    }
                    return true;
                case DECODE_STORED_LEN1:
                        {
                            if ((((uncomprLen = input.PeekBits(16)))) < 0) 
                                return false;
                            input.DropBits(16);
                            mode = DECODE_STORED_LEN2;
                        }
                    break;
                case DECODE_STORED_LEN2:
                        {
                            int nlen = input.PeekBits(16);
                            if (nlen < 0) 
                                return false;
                            input.DropBits(16);
                            if (nlen != ((uncomprLen ^ 0xffff))) 
                                throw new Exception("broken uncompressed block");
                            mode = DECODE_STORED;
                        }
                    break;
                case DECODE_STORED:
                        {
                            int more = outputWindow.CopyStored(input, uncomprLen);
                            uncomprLen -= more;
                            if (uncomprLen == 0) 
                            {
                                mode = DECODE_BLOCKS;
                                return true;
                            }
                            return !input.IsNeedingInput;
                        }
                case DECODE_DYN_HEADER:
                    if (!dynHeader.Decode(input)) 
                        return false;
                    litlenTree = dynHeader.BuildLitLenTree();
                    distTree = dynHeader.BuildDistTree();
                    mode = DECODE_HUFFMAN;
                    break;
                case DECODE_HUFFMAN:
                case DECODE_HUFFMAN_LENBITS:
                case DECODE_HUFFMAN_DIST:
                case DECODE_HUFFMAN_DISTBITS:
                    return this.DecodeHuffmanEx();
                case FINISHED:
                    return false;
                default: 
                    throw new Exception("Inflater.Decode unknown mode");
                }
            }
        }
        public void SetDictionary(byte[] buffer)
        {
            this.SetDictionaryEx(buffer, 0, buffer.Length);
        }
        public void SetDictionaryEx(byte[] buffer, int index, int count)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (index < 0) 
                throw new ArgumentOutOfRangeException("index");
            if (count < 0) 
                throw new ArgumentOutOfRangeException("count");
            if (!IsNeedingDictionary) 
                throw new InvalidOperationException("Dictionary is not needed");
            m_Adler.UpdateByBufEx(buffer, index, count);
            if (((int)m_Adler.Value) != readAdler) 
                throw new Exception("Wrong adler checksum");
            m_Adler.Reset();
            outputWindow.CopyDict(buffer, index, count);
            mode = DECODE_BLOCKS;
        }
        public void SetInput(byte[] buffer)
        {
            this.SetInputEx(buffer, 0, buffer.Length);
        }
        public void SetInputEx(byte[] buffer, int index, int count)
        {
            input.SetInput(buffer, index, count);
            m_TotalIn += count;
        }
        public int Inflate(byte[] buffer)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            return this.InflateEx(buffer, 0, buffer.Length);
        }
        public int InflateEx(byte[] buffer, int offset, int count)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (count < 0) 
                throw new ArgumentOutOfRangeException("count", "count cannot be negative");
            if (offset < 0) 
                throw new ArgumentOutOfRangeException("offset", "offset cannot be negative");
            if ((offset + count) > buffer.Length) 
                throw new ArgumentException("count exceeds buffer bounds");
            if (count == 0) 
            {
                if (!IsFinished) 
                    this.Decode();
                return 0;
            }
            int bytesCopied = 0;
            do
            {
                if (mode != DECODE_CHKSUM) 
                {
                    int more = outputWindow.CopyOutput(buffer, offset, count);
                    if (more > 0) 
                    {
                        m_Adler.UpdateByBufEx(buffer, offset, more);
                        offset += more;
                        bytesCopied += more;
                        m_TotalOut += more;
                        count -= more;
                        if (count == 0) 
                            return bytesCopied;
                    }
                }
            } while (this.Decode() || (((outputWindow.GetAvailable() > 0) && (mode != DECODE_CHKSUM)))); 
            return bytesCopied;
        }
        public bool IsNeedingInput
        {
            get
            {
                return input.IsNeedingInput;
            }
        }
        public bool IsNeedingDictionary
        {
            get
            {
                return mode == DECODE_DICT && neededBits == 0;
            }
        }
        public bool IsFinished
        {
            get
            {
                return mode == FINISHED && outputWindow.GetAvailable() == 0;
            }
        }
        public int Adler
        {
            get
            {
                return (IsNeedingDictionary ? readAdler : (int)m_Adler.Value);
            }
        }
        public int TotalOut
        {
            get
            {
                return m_TotalOut;
            }
        }
        public int TotalIn
        {
            get
            {
                return m_TotalIn - RemainingInput;
            }
        }
        public int RemainingInput
        {
            get
            {
                return input.AvailableBytes;
            }
        }
    }
}