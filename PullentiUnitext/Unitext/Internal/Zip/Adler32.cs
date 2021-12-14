﻿/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Computes Adler32 checksum for a stream of data. An Adler32
    // checksum is not as reliable as a CRC32 checksum, but a lot faster to
    // compute.
    // The specification for Adler32 may be found in RFC 1950.
    // ZLIB Compressed Data Format Specification version 3.3)
    // From that document:
    // "ADLER32 (Adler-32 checksum)
    // This contains a checksum value of the uncompressed data
    // (excluding any dictionary data) computed according to Adler-32
    // algorithm. This algorithm is a 32-bit extension and improvement
    // of the Fletcher algorithm, used in the ITU-T X.224 / ISO 8073
    // standard.
    // Adler-32 is composed of two sums accumulated per byte: s1 is
    // the sum of all bytes, s2 is the sum of all s1 values. Both sums
    // are done modulo 65521. s1 is initialized to 1, s2 to zero.  The
    // Adler-32 checksum is stored as s2*65536 + s1 in most-
    // significant-byte first (network) order."
    // "8.2. The Adler-32 algorithm
    // The Adler-32 algorithm is much faster than the CRC32 algorithm yet
    // still provides an extremely low probability of undetected errors.
    // The modulo on unsigned long accumulators can be delayed for 5552
    // bytes, so the modulo operation time is negligible.  If the bytes
    // are a, b, c, the second sum is 3a + 2b + c + 3, and so is position
    // and order sensitive, unlike the first sum, which is just a
    // checksum.  That 65521 is prime is important to avoid a possible
    // large class of two-byte errors that leave the check unchanged.
    // (The Fletcher checksum uses 255, which is not prime and which also
    // makes the Fletcher check insensitive to single byte changes 0 -
    // 255.)
    // The sum s1 is initialized to 1 instead of zero to make the length
    // of the sequence part of s2, so that the length does not have to be
    // checked separately. (Any sequence of zeroes has a Fletcher
    // checksum of zero.)"
    class Adler32 : IChecksum
    {
        const uint BASE = 65521;
        public uint Value
        {
            get
            {
                return checksum;
            }
        }
        public Adler32()
        {
            this.Reset();
        }
        public void Reset()
        {
            checksum = 1;
        }
        public void UpdateByVal(int value)
        {
            uint s1 = (uint)(checksum & 0xFFFF);
            uint s2 = (uint)(checksum >> 16);
            s1 = ((s1 + ((((uint)value) & 0xFF)))) % BASE;
            s2 = ((s1 + s2)) % BASE;
            checksum = (s2 << 16) + s1;
        }
        public void UpdateByBuf(byte[] buffer)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            this.UpdateByBufEx(buffer, 0, buffer.Length);
        }
        public void UpdateByBufEx(byte[] buffer, int offset, int count)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (offset < 0) 
                throw new ArgumentOutOfRangeException("offset", "cannot be negative");
            if (count < 0) 
                throw new ArgumentOutOfRangeException("count", "cannot be negative");
            if (offset >= buffer.Length) 
                throw new ArgumentOutOfRangeException("offset", "not a valid index into buffer");
            if ((offset + count) > buffer.Length) 
                throw new ArgumentOutOfRangeException("count", "exceeds buffer size");
            uint s1 = (uint)(checksum & 0xFFFF);
            uint s2 = (uint)(checksum >> 16);
            while (count > 0) 
            {
                int n = 3800;
                if (n > count) 
                    n = count;
                count -= n;
                while ((--n) >= 0) 
                {
                    s1 = s1 + ((uint)((buffer[offset++] & 0xff)));
                    s2 = s2 + s1;
                }
                s1 %= BASE;
                s2 %= BASE;
            }
            checksum = (s2 << 16) | s1;
        }
        uint checksum;
    }
}