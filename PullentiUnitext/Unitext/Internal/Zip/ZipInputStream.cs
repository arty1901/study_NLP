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
    // This is an InflaterInputStream that reads the files baseInputStream an zip archive
    // one after another.  It has a special method to get the zip entry of
    // the next file.  The zip entry contains information about the file name
    // size, compressed size, Crc, etc.
    // It includes support for Stored and Deflated entries.
    class ZipInputStream : InflaterInputStream
    {
        /// <summary>
        /// Delegate for reading bytes from a stream.
        /// </summary>
        delegate int ReadDataHandler(byte[] b, int offset, int length);

        ReadDataHandler internalReader;
        Crc32 m_Crc = new Crc32();
        ZipEntry m_Entry;
        int m_Size;
        int m_Method;
        int m_Flags;
        string m_Password;
        public ZipInputStream(Stream baseInputStream, int bufferSize = 4096) : base(baseInputStream, new Inflater(true), bufferSize)
        {
            internalReader = new ReadDataHandler(ReadingNotAvailable);
        }
        public string Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                m_Password = value;
            }
        }
        public bool CanDecompressEntry
        {
            get
            {
                return (m_Entry != null) && m_Entry.CanDecompress;
            }
        }
        public ZipEntry GetNextEntry()
        {
            if (m_Crc == null) 
                throw new InvalidOperationException("Closed.");
            if (m_Entry != null) 
                this.CloseEntry();
            int header = inputBuffer.ReadLeInt();
            if ((header == ZipConstants.CentralHeaderSignature || header == ZipConstants.EndOfCentralDirectorySignature || header == ZipConstants.CentralHeaderDigitalSignature) || header == ZipConstants.ArchiveExtraDataSignature || header == ZipConstants.Zip64CentralFileHeaderSignature) 
            {
                this.Dispose();
                return null;
            }
            if ((header == ZipConstants.SpanningTempSignature) || (header == ZipConstants.SpanningSignature)) 
                header = inputBuffer.ReadLeInt();
            if (header != ZipConstants.LocalHeaderSignature) 
                throw new Exception("Wrong Local header signature: 0x" + string.Format("{0}", header.ToString("X")));
            short versionRequiredToExtract = (short)inputBuffer.ReadLeShort();
            m_Flags = inputBuffer.ReadLeShort();
            m_Method = inputBuffer.ReadLeShort();
            uint dostime = (uint)inputBuffer.ReadLeInt();
            int crc2 = inputBuffer.ReadLeInt();
            csize = inputBuffer.ReadLeInt();
            m_Size = inputBuffer.ReadLeInt();
            int nameLen = inputBuffer.ReadLeShort();
            int extraLen = inputBuffer.ReadLeShort();
            bool isCrypted = ((m_Flags & 1)) == 1;
            byte[] buffer = new byte[(int)nameLen];
            inputBuffer.ReadRawBuffer(buffer);
            string name = ZipConstants.ConvertToStringExt0(m_Flags, buffer);
            m_Entry = new ZipEntry(name, versionRequiredToExtract);
            m_Entry.Flags = m_Flags;
            m_Entry._CompressionMethod = (CompressionMethod)m_Method;
            if (((m_Flags & 8)) == 0) 
            {
                m_Entry.Crc = (uint)crc2;
                m_Entry.Size = m_Size;
                m_Entry.CompressedSize = csize;
                m_Entry.CryptoCheckValue = (byte)((((crc2 >> 24)) & 0xff));
            }
            else 
            {
                if (crc2 != 0) 
                    m_Entry.Crc = (uint)crc2;
                if (m_Size != 0) 
                    m_Entry.Size = m_Size;
                if (csize != 0) 
                    m_Entry.CompressedSize = csize;
                m_Entry.CryptoCheckValue = (byte)((((dostime >> 8)) & 0xff));
            }
            m_Entry.DosTime = dostime;
            if (extraLen > 0) 
            {
                byte[] extra = new byte[(int)extraLen];
                inputBuffer.ReadRawBuffer(extra);
                m_Entry.ExtraData = extra;
            }
            m_Entry.ProcessExtraData(true);
            if (m_Entry.CompressedSize >= 0) 
                csize = m_Entry.CompressedSize;
            if (m_Entry.Size >= 0) 
                m_Size = m_Entry.Size;
            if (m_Method == ((int)CompressionMethod.Stored) && (((!isCrypted && csize != m_Size) || ((isCrypted && (csize - ZipConstants.CryptoHeaderSize) != m_Size))))) 
                throw new Exception("Stored, but compressed != uncompressed");
            if (m_Entry.IsCompressionMethodSupported()) 
                internalReader = new ReadDataHandler(InitialRead);
            else 
                internalReader = new ReadDataHandler(ReadingNotSupported);
            return m_Entry;
        }
        void ReadDataDescriptor()
        {
            if (inputBuffer.ReadLeInt() != ZipConstants.DataDescriptorSignature) 
                throw new Exception("Data descriptor signature not found");
            m_Entry.Crc = (uint)inputBuffer.ReadLeInt();
            if (m_Entry.LocalHeaderRequiresZip64) 
            {
                csize = (int)inputBuffer.ReadLeInt();
                inputBuffer.ReadLeInt();
                m_Size = (int)inputBuffer.ReadLeInt();
                inputBuffer.ReadLeInt();
            }
            else 
            {
                csize = inputBuffer.ReadLeInt();
                m_Size = inputBuffer.ReadLeInt();
            }
            m_Entry.CompressedSize = csize;
            m_Entry.Size = m_Size;
        }
        void CompleteCloseEntry(bool testCrc)
        {
            if (((m_Flags & 8)) != 0) 
                this.ReadDataDescriptor();
            m_Size = 0;
            if (testCrc && (m_Crc.Value != m_Entry.Crc)) 
                throw new Exception("CRC mismatch");
            m_Crc.Reset();
            if (m_Method == ((int)CompressionMethod.Deflated)) 
                inf.Reset();
            m_Entry = null;
        }
        public void CloseEntry()
        {
            if (m_Crc == null) 
                throw new InvalidOperationException("Closed");
            if (m_Entry == null) 
                return;
            if (m_Method == ((int)CompressionMethod.Deflated)) 
            {
                if (((m_Flags & 8)) != 0) 
                {
                    byte[] tmp = new byte[(int)4096];
                    while (this.Read(tmp, 0, tmp.Length) > 0) 
                    {
                    }
                    return;
                }
                csize -= inf.TotalIn;
                inputBuffer.Available += inf.RemainingInput;
            }
            if ((inputBuffer.Available > csize) && (csize >= 0)) 
                inputBuffer.Available = (int)((((long)inputBuffer.Available) - csize));
            else 
            {
                csize -= inputBuffer.Available;
                inputBuffer.Available = 0;
                while (csize != 0) 
                {
                    int skipped = base.Skip(csize);
                    if (skipped <= 0) 
                        throw new Exception("Zip archive ends early.");
                    csize -= skipped;
                }
            }
            this.CompleteCloseEntry(false);
        }
        public override int Available
        {
            get
            {
                return (m_Entry != null ? 1 : 0);
            }
        }
        public override long Length
        {
            get
            {
                if (m_Entry != null) 
                {
                    if (m_Entry.Size >= 0) 
                        return m_Entry.Size;
                    else 
                        return 0;
                }
                else 
                    return 0;
            }
        }
        public override int ReadByte()
        {
            byte[] b = new byte[(int)1];
            if (this.Read(b, 0, 1) <= 0) 
                return -1;
            return b[0] & 0xff;
        }
        int ReadingNotAvailable(byte[] destination, int offset, int count)
        {
            throw new InvalidOperationException("Unable to read from this stream");
        }
        int ReadingNotSupported(byte[] destination, int offset, int count)
        {
            throw new Exception("The compression method for this entry is not supported");
        }
        int InitialRead(byte[] destination, int offset, int count)
        {
            if (!CanDecompressEntry) 
                throw new Exception("Library cannot extract this entry. Version required is (" + m_Entry.Version + ")");
            if ((csize > 0) || (((m_Flags & ((int)GeneralBitFlags.Descriptor))) != 0)) 
            {
                if ((m_Method == ((int)CompressionMethod.Deflated)) && (inputBuffer.Available > 0)) 
                    inputBuffer.SetInflaterInput(inf);
                internalReader = new ReadDataHandler(BodyRead);
                return this.BodyRead(destination, offset, count);
            }
            else 
            {
                internalReader = new ReadDataHandler(ReadingNotAvailable);
                return 0;
            }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) 
                throw new IOException("null buffer");
            if (offset < 0) 
                throw new Exception("bad offset");
            if (count < 0) 
                throw new Exception("bad count");
            if (((buffer.Length - offset)) < count) 
                throw new IOException("Invalid offset/count combination");
            try 
            {
                return internalReader(buffer, offset, count) /* error */;
            }
            catch(Exception ex) 
            {
                throw new IOException(ex.Message, ex);
            }
        }
        int BodyRead(byte[] buffer, int offset, int count)
        {
            if (m_Crc == null) 
                throw new InvalidOperationException("Closed");
            if ((m_Entry == null) || (count <= 0)) 
                return 0;
            if ((offset + count) > buffer.Length) 
                throw new ArgumentException("Offset + count exceeds buffer size");
            bool finished = false;
            switch (m_Method) { 
            case (int)CompressionMethod.Deflated:
                count = base.Read(buffer, offset, count);
                if (count <= 0) 
                {
                    if (!inf.IsFinished) 
                        throw new Exception("Inflater not finished!");
                    inputBuffer.Available = inf.RemainingInput;
                    if (((m_Flags & 8)) == 0 && (((inf.TotalIn != csize && csize != 0xFFFFFFFF && csize != -1) || inf.TotalOut != m_Size))) 
                        throw new Exception(((("Size mismatch: " + csize + ";") + m_Size + " <-> ") + inf.TotalIn + ";") + inf.TotalOut);
                    inf.Reset();
                    finished = true;
                }
                break;
            case (int)CompressionMethod.Stored:
                if ((count > csize) && (csize >= 0)) 
                    count = (int)csize;
                if (count > 0) 
                {
                    count = inputBuffer.ReadClearTextBuffer(buffer, offset, count);
                    if (count > 0) 
                    {
                        csize -= count;
                        m_Size -= count;
                    }
                }
                if (csize == 0) 
                    finished = true;
                else if (count < 0) 
                    throw new Exception("EOF in stored block");
                break;
            }
            if (count > 0) 
                m_Crc.UpdateByBufEx(buffer, offset, count);
            if (finished) 
                this.CompleteCloseEntry(true);
            return count;
        }
        public override void Close()
        {
            internalReader = new ReadDataHandler(ReadingNotAvailable);
            m_Crc = null;
            m_Entry = null;
            base.Close();
        }
    }
}