/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;

namespace Pullenti.Unitext.Internal.Zip
{
    // A special stream deflating or compressing the bytes that are
    // written to it.  It uses a Deflater to perform actual deflating.<br/>
    class DeflaterOutputStream : Stream
    {
        public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater = null, int bufferSize = 512)
        {
            if (deflater == null) 
                deflater = new Deflater();
            baseOutputStream_ = baseOutputStream;
            buffer_ = new byte[(int)bufferSize];
            deflater_ = deflater;
        }
        public virtual void Finish()
        {
            deflater_.Finish();
            while (!deflater_.IsFinished) 
            {
                int len = deflater_.DeflateEx(buffer_, 0, buffer_.Length);
                if (len <= 0) 
                    break;
                baseOutputStream_.Write(buffer_, 0, len);
            }
            if (!deflater_.IsFinished) 
                throw new Exception("Can't deflate all input?");
            baseOutputStream_.Flush();
        }
        public bool IsStreamOwner
        {
            get
            {
                return isStreamOwner_;
            }
            set
            {
                isStreamOwner_ = value;
            }
        }
        public bool CanPatchEntries
        {
            get
            {
                return baseOutputStream_.CanSeek;
            }
        }
        string password;
        protected void Deflate()
        {
            while (!deflater_.IsNeedingInput) 
            {
                int deflateCount = deflater_.DeflateEx(buffer_, 0, buffer_.Length);
                if (deflateCount <= 0) 
                    break;
                baseOutputStream_.Write(buffer_, 0, deflateCount);
            }
            if (!deflater_.IsNeedingInput) 
                throw new Exception("DeflaterOutputStream can't deflate all input?");
        }
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        public override bool CanWrite
        {
            get
            {
                try 
                {
                    return baseOutputStream_.CanWrite;
                }
                catch(Exception ex) 
                {
                    return false;
                }
            }
        }
        public override long Length
        {
            get
            {
                try 
                {
                    return baseOutputStream_.Length;
                }
                catch(Exception ex) 
                {
                    return -1;
                }
            }
        }
        public override long Position
        {
            get
            {
                try 
                {
                    return baseOutputStream_.Position;
                }
                catch(Exception ex) 
                {
                    return 0;
                }
            }
            set
            {
            }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }
        public override void SetLength(long value)
        {
        }
        public override int ReadByte()
        {
            return -1;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return -1;
        }
        public override void Flush()
        {
            try 
            {
                deflater_.Flush();
                this.Deflate();
                baseOutputStream_.Flush();
            }
            catch(Exception ex) 
            {
                throw new IOException(ex.Message, ex);
            }
        }
        public override void Close()
        {
            if (!isClosed_) 
            {
                isClosed_ = true;
                try 
                {
                    this.Finish();
                }
                catch(Exception ex) 
                {
                    throw new IOException(ex.Message, ex);
                }
                finally
                {
                    if (isStreamOwner_) 
                    {
                        try 
                        {
                            baseOutputStream_.Dispose();
                        }
                        catch(Exception ex) 
                        {
                        }
                    }
                }
            }
        }
        public override void WriteByte(byte value)
        {
            byte[] b = new byte[(int)1];
            b[0] = value;
            this.Write(b, 0, 1);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            try 
            {
                deflater_.SetInputEx(buffer, offset, count);
                this.Deflate();
            }
            catch(Exception ex) 
            {
                throw new IOException(ex.Message, ex);
            }
        }
        byte[] buffer_;
        protected Deflater deflater_;
        protected Stream baseOutputStream_;
        bool isClosed_;
        bool isStreamOwner_ = true;
    }
}