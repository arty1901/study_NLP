/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // This class allows us to retrieve a specified number of bits from
    // the input buffer, as well as copy big byte blocks.
    // It uses an int buffer to store up to 31 bits for direct
    // manipulation.  This guarantees that we can get at least 16 bits,
    // but we only need at most 15, so this is all safe.
    // There are some optimizations in this class, for example, you must
    // never peek more than 8 bits more than needed, and you must first
    // peek bits before you may drop them.  This is not a general purpose
    // class but optimized for the behaviour of the Inflater.
    class StreamManipulator
    {
        public StreamManipulator()
        {
        }
        public int PeekBits(int bitCount)
        {
            if (bitsInBuffer_ < bitCount) 
            {
                if (windowStart_ == windowEnd_) 
                    return -1;
                buffer_ |= ((uint)((((window_[windowStart_] & 0xff) | ((window_[windowStart_ + 1] & 0xff)) << 8)) << bitsInBuffer_));
                windowStart_ += 2;
                bitsInBuffer_ += 16;
            }
            return (int)((buffer_ & (((1 << bitCount) - 1))));
        }
        public void DropBits(int bitCount)
        {
            buffer_ >>= bitCount;
            bitsInBuffer_ -= bitCount;
        }
        public int GetBits(int bitCount)
        {
            int bits = this.PeekBits(bitCount);
            if (bits >= 0) 
                this.DropBits(bitCount);
            return bits;
        }
        public int AvailableBits
        {
            get
            {
                return bitsInBuffer_;
            }
        }
        public int AvailableBytes
        {
            get
            {
                return (windowEnd_ - windowStart_) + ((bitsInBuffer_ >> 3));
            }
        }
        public void SkipToByteBoundary()
        {
            buffer_ >>= ((bitsInBuffer_ & 7));
            bitsInBuffer_ &= (~7);
        }
        public bool IsNeedingInput
        {
            get
            {
                return windowStart_ == windowEnd_;
            }
        }
        public int CopyBytes(byte[] output, int offset, int length)
        {
            if (length < 0) 
                throw new ArgumentOutOfRangeException("length");
            if (((bitsInBuffer_ & 7)) != 0) 
                throw new InvalidOperationException("Bit buffer is not byte aligned!");
            int count = 0;
            while ((bitsInBuffer_ > 0) && (length > 0)) 
            {
                output[offset++] = (byte)buffer_;
                buffer_ >>= 8;
                bitsInBuffer_ -= 8;
                length--;
                count++;
            }
            if (length == 0) 
                return count;
            int avail = windowEnd_ - windowStart_;
            if (length > avail) 
                length = avail;
            Array.Copy(window_, windowStart_, output, offset, length);
            windowStart_ += length;
            if (((((windowStart_ - windowEnd_)) & 1)) != 0) 
            {
                buffer_ = (uint)((window_[windowStart_++] & 0xff));
                bitsInBuffer_ = 8;
            }
            return count + length;
        }
        public void Reset()
        {
            buffer_ = 0;
            windowStart_ = (windowEnd_ = (bitsInBuffer_ = 0));
        }
        public void SetInput(byte[] buffer, int offset, int count)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (offset < 0) 
                throw new ArgumentOutOfRangeException("offset", "Cannot be negative");
            if (count < 0) 
                throw new ArgumentOutOfRangeException("count", "Cannot be negative");
            if (windowStart_ < windowEnd_) 
                throw new InvalidOperationException("Old input was not completely processed");
            int end = offset + count;
            if ((offset > end) || (end > buffer.Length)) 
                throw new ArgumentOutOfRangeException("count");
            if (((count & 1)) != 0) 
            {
                buffer_ |= ((uint)(((buffer[offset++] & 0xff)) << bitsInBuffer_));
                bitsInBuffer_ += 8;
            }
            window_ = buffer;
            windowStart_ = offset;
            windowEnd_ = end;
        }
        private byte[] window_;
        private int windowStart_;
        private int windowEnd_;
        private uint buffer_;
        private int bitsInBuffer_;
    }
}