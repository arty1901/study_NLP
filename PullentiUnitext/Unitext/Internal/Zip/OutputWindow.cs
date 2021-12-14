/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Contains the output from the Inflation process.
    // We need to have a window so that we can refer backwards into the output stream
    // to repeat stuff.<br/>
    class OutputWindow
    {
        const int WindowSize = 1 << 15;
        const int WindowMask = WindowSize - 1;
        byte[] window = new byte[(int)WindowSize];
        int windowEnd;
        int windowFilled;
        public void Write(int value)
        {
            windowFilled++;
            if ((windowFilled - 1) == WindowSize) 
                throw new InvalidOperationException("Window full");
            window[windowEnd++] = (byte)value;
            windowEnd &= WindowMask;
        }
        private void SlowRepeat(int repStart, int length, int distance)
        {
            while ((length--) > 0) 
            {
                window[windowEnd++] = window[repStart++];
                windowEnd &= WindowMask;
                repStart &= WindowMask;
            }
        }
        public void Repeat(int length, int distance)
        {
            if (((windowFilled += length)) > WindowSize) 
                throw new InvalidOperationException("Window full");
            int repStart = ((windowEnd - distance)) & WindowMask;
            int border = WindowSize - length;
            if ((repStart <= border) && ((windowEnd < border))) 
            {
                if (length <= distance) 
                {
                    Array.Copy(window, repStart, window, windowEnd, length);
                    windowEnd += length;
                }
                else 
                    while ((length--) > 0) 
                    {
                        window[windowEnd++] = window[repStart++];
                    }
            }
            else 
                this.SlowRepeat(repStart, length, distance);
        }
        public int CopyStored(StreamManipulator input, int length)
        {
            length = Math.Min(Math.Min(length, WindowSize - windowFilled), input.AvailableBytes);
            int copied;
            int tailLen = WindowSize - windowEnd;
            if (length > tailLen) 
            {
                copied = input.CopyBytes(window, windowEnd, tailLen);
                if (copied == tailLen) 
                    copied += input.CopyBytes(window, 0, length - tailLen);
            }
            else 
                copied = input.CopyBytes(window, windowEnd, length);
            windowEnd = ((windowEnd + copied)) & WindowMask;
            windowFilled += copied;
            return copied;
        }
        public void CopyDict(byte[] dictionary, int offset, int length)
        {
            if (dictionary == null) 
                throw new ArgumentNullException("dictionary");
            if (windowFilled > 0) 
                throw new InvalidOperationException();
            if (length > WindowSize) 
            {
                offset += (length - WindowSize);
                length = WindowSize;
            }
            Array.Copy(dictionary, offset, window, 0, length);
            windowEnd = length & WindowMask;
        }
        public int GetFreeSpace()
        {
            return WindowSize - windowFilled;
        }
        public int GetAvailable()
        {
            return windowFilled;
        }
        public int CopyOutput(byte[] output, int offset, int len)
        {
            int copyEnd = windowEnd;
            if (len > windowFilled) 
                len = windowFilled;
            else 
                copyEnd = (((windowEnd - windowFilled) + len)) & WindowMask;
            int copied = len;
            int tailLen = len - copyEnd;
            if (tailLen > 0) 
            {
                Array.Copy(window, WindowSize - tailLen, output, offset, tailLen);
                offset += tailLen;
                len = copyEnd;
            }
            Array.Copy(window, copyEnd - len, output, offset, len);
            windowFilled -= copied;
            if (windowFilled < 0) 
                throw new InvalidOperationException();
            return copied;
        }
        public void Reset()
        {
            windowFilled = (windowEnd = 0);
        }
    }
}