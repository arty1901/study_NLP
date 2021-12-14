/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Low level compression engine for deflate algorithm which uses a 32K sliding window
    // with secondary compression from Huffman/Shannon-Fano codes.
    class DeflaterEngine : DeflaterConstants
    {
        const int TooFar = 4096;
        public DeflaterEngine(DeflaterPending pending)
        {
            this.pending = pending;
            huffman = new DeflaterHuffman(pending);
            m_Adler = new Adler32();
            window = new byte[(int)(2 * DeflaterConstants.WSIZE)];
            head = new short[(int)DeflaterConstants.HASH_SIZE];
            prev = new short[(int)DeflaterConstants.WSIZE];
            blockStart = (strstart = 1);
        }
        public bool Deflate(bool flush, bool finish)
        {
            bool progress;
            while (true) 
            {
                this.FillWindow();
                bool canFlush = flush && (inputOff == inputEnd);
                switch (compressionFunction) { 
                case DeflaterConstants.DEFLATE_STORED:
                    progress = this.DeflateStored(canFlush, finish);
                    break;
                case DeflaterConstants.DEFLATE_FAST:
                    progress = this.DeflateFast(canFlush, finish);
                    break;
                case DeflaterConstants.DEFLATE_SLOW:
                    progress = this.DeflateSlow(canFlush, finish);
                    break;
                default: 
                    throw new InvalidOperationException("unknown compressionFunction");
                }
                if (pending.IsFlushed && progress) 
                {
                }
                else 
                    break;
            }
            return progress;
        }
        public void SetInput(byte[] buffer, int offset, int count)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (offset < 0) 
                throw new ArgumentOutOfRangeException("offset");
            if (count < 0) 
                throw new ArgumentOutOfRangeException("count");
            if (inputOff < inputEnd) 
                throw new InvalidOperationException("Old input was not completely processed");
            int end = offset + count;
            if ((offset > end) || (end > buffer.Length)) 
                throw new ArgumentOutOfRangeException("count");
            inputBuf = buffer;
            inputOff = offset;
            inputEnd = end;
        }
        public bool NeedsInput()
        {
            return inputEnd == inputOff;
        }
        public void SetDictionary(byte[] buffer, int offset, int length)
        {
            m_Adler.UpdateByBufEx(buffer, offset, length);
            if (length < DeflaterConstants.MIN_MATCH) 
                return;
            if (length > DeflaterConstants.MAX_DIST) 
            {
                offset += (length - DeflaterConstants.MAX_DIST);
                length = DeflaterConstants.MAX_DIST;
            }
            Array.Copy(buffer, offset, window, strstart, length);
            this.UpdateHash();
            --length;
            while ((--length) > 0) 
            {
                this.InsertString();
                strstart++;
            }
            strstart += 2;
            blockStart = strstart;
        }
        public void Reset()
        {
            huffman.Reset();
            m_Adler.Reset();
            blockStart = (strstart = 1);
            lookahead = 0;
            m_TotalIn = 0;
            prevAvailable = false;
            matchLen = DeflaterConstants.MIN_MATCH - 1;
            for (int i = 0; i < DeflaterConstants.HASH_SIZE; i++) 
            {
                head[i] = 0;
            }
            for (int i = 0; i < DeflaterConstants.WSIZE; i++) 
            {
                prev[i] = 0;
            }
        }
        public void ResetAdler()
        {
            m_Adler.Reset();
        }
        public int Adler
        {
            get
            {
                return unchecked((int)m_Adler.Value);
            }
        }
        public int TotalIn
        {
            get
            {
                return m_TotalIn;
            }
        }
        public DeflateStrategy Strategy
        {
            get
            {
                return m_Strategy;
            }
            set
            {
                m_Strategy = value;
            }
        }
        public void SetLevel(int level)
        {
            if (((level < 0)) || (level > 9)) 
                throw new ArgumentOutOfRangeException("level");
            goodLength = DeflaterConstants.GOOD_LENGTH[level];
            max_lazy = DeflaterConstants.MAX_LAZY[level];
            niceLength = DeflaterConstants.NICE_LENGTH[level];
            max_chain = DeflaterConstants.MAX_CHAIN[level];
            if (DeflaterConstants.COMPR_FUNC[level] != compressionFunction) 
            {
                switch (compressionFunction) { 
                case DeflaterConstants.DEFLATE_STORED:
                    if (strstart > blockStart) 
                    {
                        huffman.FlushStoredBlock(window, blockStart, strstart - blockStart, false);
                        blockStart = strstart;
                    }
                    this.UpdateHash();
                    break;
                case DeflaterConstants.DEFLATE_FAST:
                    if (strstart > blockStart) 
                    {
                        huffman.FlushBlock(window, blockStart, strstart - blockStart, false);
                        blockStart = strstart;
                    }
                    break;
                case DeflaterConstants.DEFLATE_SLOW:
                    if (prevAvailable) 
                        huffman.TallyLit(window[strstart - 1] & 0xff);
                    if (strstart > blockStart) 
                    {
                        huffman.FlushBlock(window, blockStart, strstart - blockStart, false);
                        blockStart = strstart;
                    }
                    prevAvailable = false;
                    matchLen = DeflaterConstants.MIN_MATCH - 1;
                    break;
                }
                compressionFunction = DeflaterConstants.COMPR_FUNC[level];
            }
        }
        public void FillWindow()
        {
            if (strstart >= (DeflaterConstants.WSIZE + DeflaterConstants.MAX_DIST)) 
                this.SlideWindow();
            while ((lookahead < DeflaterConstants.MIN_LOOKAHEAD) && (inputOff < inputEnd)) 
            {
                int more = (2 * DeflaterConstants.WSIZE) - lookahead - strstart;
                if (more > (inputEnd - inputOff)) 
                    more = inputEnd - inputOff;
                Array.Copy(inputBuf, inputOff, window, strstart + lookahead, more);
                m_Adler.UpdateByBufEx(inputBuf, inputOff, more);
                inputOff += more;
                m_TotalIn += more;
                lookahead += more;
            }
            if (lookahead >= DeflaterConstants.MIN_MATCH) 
                this.UpdateHash();
        }
        void UpdateHash()
        {
            ins_h = (window[strstart] << DeflaterConstants.HASH_SHIFT) ^ window[strstart + 1];
        }
        int InsertString()
        {
            short match;
            int hash = (((ins_h << DeflaterConstants.HASH_SHIFT) ^ window[strstart + ((DeflaterConstants.MIN_MATCH - 1))])) & DeflaterConstants.HASH_MASK;
            prev[strstart & DeflaterConstants.WMASK] = (match = head[hash]);
            head[hash] = unchecked((short)strstart);
            ins_h = hash;
            return match & 0xffff;
        }
        void SlideWindow()
        {
            Array.Copy(window, DeflaterConstants.WSIZE, window, 0, DeflaterConstants.WSIZE);
            matchStart -= DeflaterConstants.WSIZE;
            strstart -= DeflaterConstants.WSIZE;
            blockStart -= DeflaterConstants.WSIZE;
            for (int i = 0; i < DeflaterConstants.HASH_SIZE; ++i) 
            {
                int m = head[i] & 0xffff;
                head[i] = (short)((m >= DeflaterConstants.WSIZE ? (m - DeflaterConstants.WSIZE) : 0));
            }
            for (int i = 0; i < DeflaterConstants.WSIZE; i++) 
            {
                int m = prev[i] & 0xffff;
                prev[i] = (short)((m >= DeflaterConstants.WSIZE ? (m - DeflaterConstants.WSIZE) : 0));
            }
        }
        bool FindLongestMatch(int curMatch)
        {
            int chainLength = this.max_chain;
            int niceLength = this.niceLength;
            short[] prev = this.prev;
            int scan = this.strstart;
            int match;
            int best_end = this.strstart + matchLen;
            int best_len = Math.Max(matchLen, DeflaterConstants.MIN_MATCH - 1);
            int limit = Math.Max(strstart - DeflaterConstants.MAX_DIST, 0);
            int strend = (strstart + DeflaterConstants.MAX_MATCH) - 1;
            byte scan_end1 = window[best_end - 1];
            byte scan_end = window[best_end];
            if (best_len >= this.goodLength) 
                chainLength >>= 2;
            if (niceLength > lookahead) 
                niceLength = lookahead;
            while (true) 
            {
                if ((window[curMatch + best_len] != scan_end || window[(curMatch + best_len) - 1] != scan_end1 || window[curMatch] != window[scan]) || window[curMatch + 1] != window[scan + 1]) 
                {
                    curMatch = (prev[curMatch & DeflaterConstants.WMASK] & 0xffff);
                    chainLength--;
                    if (curMatch > limit && chainLength != 0) 
                    {
                    }
                    else 
                        break;
                    continue;
                }
                match = curMatch + 2;
                scan += 2;
                while ((((window[++scan] == window[++match] && window[++scan] == window[++match] && window[++scan] == window[++match]) && window[++scan] == window[++match] && window[++scan] == window[++match]) && window[++scan] == window[++match] && window[++scan] == window[++match]) && window[++scan] == window[++match] && ((scan < strend))) 
                {
                }
                if (scan > best_end) 
                {
                    matchStart = curMatch;
                    best_end = scan;
                    best_len = scan - strstart;
                    if (best_len >= niceLength) 
                        break;
                    scan_end1 = window[best_end - 1];
                    scan_end = window[best_end];
                }
                scan = strstart;
                curMatch = (prev[curMatch & DeflaterConstants.WMASK] & 0xffff);
                chainLength--;
                if (curMatch > limit && chainLength != 0) 
                {
                }
                else 
                    break;
            }
            matchLen = Math.Min(best_len, lookahead);
            return matchLen >= DeflaterConstants.MIN_MATCH;
        }
        bool DeflateStored(bool flush, bool finish)
        {
            if (!flush && (lookahead == 0)) 
                return false;
            strstart += lookahead;
            lookahead = 0;
            int storedLength = strstart - blockStart;
            if ((storedLength >= DeflaterConstants.MAX_BLOCK_SIZE) || (((blockStart < DeflaterConstants.WSIZE) && storedLength >= DeflaterConstants.MAX_DIST)) || flush) 
            {
                bool lastBlock = finish;
                if (storedLength > DeflaterConstants.MAX_BLOCK_SIZE) 
                {
                    storedLength = DeflaterConstants.MAX_BLOCK_SIZE;
                    lastBlock = false;
                }
                huffman.FlushStoredBlock(window, blockStart, storedLength, lastBlock);
                blockStart += storedLength;
                return !lastBlock;
            }
            return true;
        }
        bool DeflateFast(bool flush, bool finish)
        {
            if ((lookahead < DeflaterConstants.MIN_LOOKAHEAD) && !flush) 
                return false;
            while (lookahead >= DeflaterConstants.MIN_LOOKAHEAD || flush) 
            {
                if (lookahead == 0) 
                {
                    huffman.FlushBlock(window, blockStart, strstart - blockStart, finish);
                    blockStart = strstart;
                    return false;
                }
                if (strstart > ((2 * DeflaterConstants.WSIZE) - DeflaterConstants.MIN_LOOKAHEAD)) 
                    this.SlideWindow();
                int hashHead;
                if ((lookahead >= DeflaterConstants.MIN_MATCH && (((hashHead = this.InsertString()))) != 0 && m_Strategy != DeflateStrategy.HuffmanOnly) && (strstart - hashHead) <= DeflaterConstants.MAX_DIST && this.FindLongestMatch(hashHead)) 
                {
                    bool full = huffman.TallyDist(strstart - matchStart, matchLen);
                    lookahead -= matchLen;
                    if (matchLen <= max_lazy && lookahead >= DeflaterConstants.MIN_MATCH) 
                    {
                        while ((--matchLen) > 0) 
                        {
                            ++strstart;
                            this.InsertString();
                        }
                        ++strstart;
                    }
                    else 
                    {
                        strstart += matchLen;
                        if (lookahead >= (DeflaterConstants.MIN_MATCH - 1)) 
                            this.UpdateHash();
                    }
                    matchLen = DeflaterConstants.MIN_MATCH - 1;
                    if (!full) 
                        continue;
                }
                else 
                {
                    huffman.TallyLit(window[strstart] & 0xff);
                    ++strstart;
                    --lookahead;
                }
                if (huffman.IsFull()) 
                {
                    bool lastBlock = finish && (lookahead == 0);
                    huffman.FlushBlock(window, blockStart, strstart - blockStart, lastBlock);
                    blockStart = strstart;
                    return !lastBlock;
                }
            }
            return true;
        }
        bool DeflateSlow(bool flush, bool finish)
        {
            if ((lookahead < DeflaterConstants.MIN_LOOKAHEAD) && !flush) 
                return false;
            while (lookahead >= DeflaterConstants.MIN_LOOKAHEAD || flush) 
            {
                if (lookahead == 0) 
                {
                    if (prevAvailable) 
                        huffman.TallyLit(window[strstart - 1] & 0xff);
                    prevAvailable = false;
                    huffman.FlushBlock(window, blockStart, strstart - blockStart, finish);
                    blockStart = strstart;
                    return false;
                }
                if (strstart >= ((2 * DeflaterConstants.WSIZE) - DeflaterConstants.MIN_LOOKAHEAD)) 
                    this.SlideWindow();
                int prevMatch = matchStart;
                int prevLen = matchLen;
                if (lookahead >= DeflaterConstants.MIN_MATCH) 
                {
                    int hashHead = this.InsertString();
                    if ((m_Strategy != DeflateStrategy.HuffmanOnly && hashHead != 0 && (strstart - hashHead) <= DeflaterConstants.MAX_DIST) && this.FindLongestMatch(hashHead)) 
                    {
                        if (matchLen <= 5 && ((m_Strategy == DeflateStrategy.Filtered || ((matchLen == DeflaterConstants.MIN_MATCH && (strstart - matchStart) > TooFar))))) 
                            matchLen = DeflaterConstants.MIN_MATCH - 1;
                    }
                }
                if ((prevLen >= DeflaterConstants.MIN_MATCH) && (matchLen <= prevLen)) 
                {
                    huffman.TallyDist(strstart - 1 - prevMatch, prevLen);
                    prevLen -= 2;
                    do
                    {
                        strstart++;
                        lookahead--;
                        if (lookahead >= DeflaterConstants.MIN_MATCH) 
                            this.InsertString();
                        --prevLen;
                    } while (prevLen > 0); 
                    strstart++;
                    lookahead--;
                    prevAvailable = false;
                    matchLen = DeflaterConstants.MIN_MATCH - 1;
                }
                else 
                {
                    if (prevAvailable) 
                        huffman.TallyLit(window[strstart - 1] & 0xff);
                    prevAvailable = true;
                    strstart++;
                    lookahead--;
                }
                if (huffman.IsFull()) 
                {
                    int len = strstart - blockStart;
                    if (prevAvailable) 
                        len--;
                    bool lastBlock = (finish && (lookahead == 0) && !prevAvailable);
                    huffman.FlushBlock(window, blockStart, len, lastBlock);
                    blockStart += len;
                    return !lastBlock;
                }
            }
            return true;
        }
        int ins_h;
        short[] head;
        short[] prev;
        int matchStart;
        int matchLen;
        bool prevAvailable;
        int blockStart;
        int strstart;
        int lookahead;
        byte[] window;
        DeflateStrategy m_Strategy;
        int max_chain;
        int max_lazy;
        int niceLength;
        int goodLength;
        int compressionFunction;
        byte[] inputBuf;
        int m_TotalIn;
        int inputOff;
        int inputEnd;
        DeflaterPending pending;
        DeflaterHuffman huffman;
        Adler32 m_Adler;
    }
}