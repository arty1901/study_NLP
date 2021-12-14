/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;

namespace Pullenti.Unitext.Internal.Word
{
    // Base stream for retrival data from a Compound File.
    abstract class CompoundFileStream : Stream
    {
        CompoundFileStorage m_Storage;
        internal CompoundFileStorage Storage
        {
            get
            {
                return m_Storage;
            }
        }
        internal CompoundFileSystem System
        {
            get
            {
                return Storage.System;
            }
        }
        int m_Position;
        int m_Length;
        int m_PageSize;
        internal int PageSize
        {
            get
            {
                return m_PageSize;
            }
        }
        byte[] m_Page = null;
        int m_PageIndex = -1;
        internal CompoundFileStream(CompoundFileStorage storage, int pageSize)
        {
            this.m_Storage = storage;
            this.m_Position = 0;
            this.m_Length = storage.Length;
            this.m_PageSize = pageSize;
        }
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
        public override void Flush()
        {
        }
        public override long Length
        {
            get
            {
                return m_Length;
            }
        }
        public override long Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                this.Seek(value, SeekOrigin.Begin);
            }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count <= 0) 
                return count;
            if (m_Position >= m_Length) 
                return 0;
            int canRead = (count > ((m_Length - m_Position)) ? (int)((m_Length - m_Position)) : count);
            try 
            {
                int pageStartPosition = m_PageSize * m_PageIndex;
                int pageEndPosition = pageStartPosition;
                if (m_Page != null) 
                    pageEndPosition += m_Page.Length;
                if ((m_Position < pageStartPosition) || (m_Position + canRead) > pageEndPosition) 
                {
                    byte[] cachedPage = this.m_Page;
                    int cachedPageIndex = this.m_PageIndex;
                    int startPageIndex = (int)((m_Position / m_PageSize));
                    int lastPageIndex = (int)(((((m_Position + canRead) - 1)) / m_PageSize));
                    if (startPageIndex != cachedPageIndex) 
                    {
                        this.m_Page = this.GetPageData(startPageIndex);
                        this.m_PageIndex = startPageIndex;
                    }
                    int startPageOffset = (int)((m_Position - (startPageIndex * m_PageSize)));
                    int startPageTail = m_Page.Length - startPageOffset;
                    if (startPageTail >= canRead) 
                    {
                        Array.Copy(m_Page, startPageOffset, buffer, offset, canRead);
                        m_Position += canRead;
                    }
                    else 
                    {
                        Array.Copy(m_Page, startPageOffset, buffer, offset, startPageTail);
                        offset += startPageTail;
                        m_Position += startPageTail;
                        int leftToRead = canRead - startPageTail;
                        for (int i = startPageIndex + 1; i < lastPageIndex; i++) 
                        {
                            if (cachedPageIndex == i) 
                            {
                                this.m_Page = cachedPage;
                                this.m_PageIndex = cachedPageIndex;
                            }
                            else 
                            {
                                this.m_Page = this.GetPageData(i);
                                this.m_PageIndex = i;
                            }
                            Array.Copy(m_Page, 0, buffer, offset, m_Page.Length);
                            offset += m_Page.Length;
                            m_Position += m_Page.Length;
                            leftToRead -= m_Page.Length;
                        }
                        if (cachedPageIndex == lastPageIndex) 
                        {
                            this.m_Page = cachedPage;
                            this.m_PageIndex = cachedPageIndex;
                        }
                        else 
                        {
                            this.m_Page = this.GetPageData(lastPageIndex);
                            this.m_PageIndex = lastPageIndex;
                        }
                        Array.Copy(m_Page, 0, buffer, offset, leftToRead);
                        m_Position += leftToRead;
                    }
                }
                else 
                {
                    Array.Copy(m_Page, (int)((m_Position - pageStartPosition)), buffer, offset, canRead);
                    m_Position += canRead;
                }
            }
            catch(Exception ex) 
            {
                return -1;
            }
            return canRead;
        }
        protected abstract byte[] GetPageData(int pageIndex);
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin) { 
            case SeekOrigin.Begin:
                m_Position = (int)offset;
                break;
            case SeekOrigin.Current:
                m_Position += ((int)offset);
                break;
            case SeekOrigin.End:
                m_Position = (int)((Length + offset));
                break;
            }
            return m_Position;
        }
        public override void SetLength(long value)
        {
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) 
            {
                m_Storage = null;
                m_Page = null;
            }
        }
    }
}