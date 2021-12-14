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
    // This class assists with writing/reading from Zip files.
    class ZipHelperStream : Stream
    {
        public ZipHelperStream(string name, Stream stream = null)
        {
            if (stream == null) 
            {
                stream_ = new FileStream(name, FileMode.Open, FileAccess.ReadWrite);
                isOwner_ = true;
            }
            else 
                stream_ = stream;
        }
        public bool IsStreamOwner
        {
            get
            {
                return isOwner_;
            }
            set
            {
                isOwner_ = value;
            }
        }
        public override bool CanRead
        {
            get
            {
                return stream_.CanRead;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return stream_.CanSeek;
            }
        }
        public override long Length
        {
            get
            {
                return stream_.Length;
            }
        }
        public override long Position
        {
            get
            {
                return stream_.Position;
            }
            set
            {
                stream_.Position = value;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return stream_.CanWrite;
            }
        }
        public override void Flush()
        {
            stream_.Flush();
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream_.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            stream_.SetLength(value);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream_.Read(buffer, offset, count);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            stream_.Write(buffer, offset, count);
        }
        public override void Close()
        {
            Stream toClose = stream_;
            stream_ = null;
            if (isOwner_ && (toClose != null)) 
            {
                isOwner_ = false;
                toClose.Dispose();
            }
        }
        void WriteLocalHeader(ZipEntry entry, EntryPatchData patchData)
        {
            CompressionMethod method = entry._CompressionMethod;
            bool headerInfoAvailable = true;
            bool patchEntryHeader = false;
            this.WriteLEInt(ZipConstants.LocalHeaderSignature);
            this.WriteLEShort(entry.Version);
            this.WriteLEShort(entry.Flags);
            this.WriteLEShort((byte)method);
            this.WriteLEInt((int)entry.DosTime);
            if (headerInfoAvailable == true) 
            {
                this.WriteLEInt((int)entry.Crc);
                if (entry.LocalHeaderRequiresZip64) 
                {
                    this.WriteLEInt(-1);
                    this.WriteLEInt(-1);
                }
                else 
                {
                    this.WriteLEInt((entry.IsCrypted ? ((int)entry.CompressedSize) + ZipConstants.CryptoHeaderSize : (int)entry.CompressedSize));
                    this.WriteLEInt((int)entry.Size);
                }
            }
            else 
            {
                if (patchData != null) 
                    patchData.CrcPatchOffset = (int)stream_.Position;
                this.WriteLEInt(0);
                if (patchData != null) 
                    patchData.SizePatchOffset = (int)stream_.Position;
                if (entry.LocalHeaderRequiresZip64 && patchEntryHeader) 
                {
                    this.WriteLEInt(-1);
                    this.WriteLEInt(-1);
                }
                else 
                {
                    this.WriteLEInt(0);
                    this.WriteLEInt(0);
                }
            }
            byte[] name = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
            if (name.Length > 0xFFFF) 
                throw new Exception("Entry name too long.");
            ZipExtraData ed = new ZipExtraData(entry.ExtraData);
            if (entry.LocalHeaderRequiresZip64 && ((headerInfoAvailable || patchEntryHeader))) 
            {
                ed.StartNewEntry();
                if (headerInfoAvailable) 
                {
                    ed.AddLeLong(entry.Size, 0);
                    ed.AddLeLong(entry.CompressedSize, 0);
                }
                else 
                {
                    ed.AddLeLong(-1, -1);
                    ed.AddLeLong(-1, -1);
                }
                ed.AddNewEntry(1);
                if (!ed.Find(1)) 
                    throw new Exception("Internal error cant find extra data");
                if (patchData != null) 
                    patchData.SizePatchOffset = ed.CurrentReadIndex;
            }
            else 
                ed.Delete(1);
            byte[] extra = ed.GetEntryData();
            this.WriteLEShort(name.Length);
            this.WriteLEShort(extra.Length);
            if (name.Length > 0) 
                stream_.Write(name, 0, name.Length);
            if (entry.LocalHeaderRequiresZip64 && patchEntryHeader) 
                patchData.SizePatchOffset += ((int)stream_.Position);
            if (extra.Length > 0) 
                stream_.Write(extra, 0, extra.Length);
        }
        public int LocateBlockWithSignature(int signature, int endLocation, int minimumBlockSize, int maximumVariableData)
        {
            int pos = endLocation - minimumBlockSize;
            if (pos < 0) 
                return -1;
            int giveUpMarker = Math.Max(pos - maximumVariableData, 0);
            do
            {
                if (pos < giveUpMarker) 
                    return -1;
                this.Seek(pos--, SeekOrigin.Begin);
            } while (this.ReadLEInt() != signature); 
            return (int)Position;
        }
        public void WriteZip64EndOfCentralDirectory(long noOfEntries, long sizeEntries, long centralDirOffset)
        {
            long centralSignatureOffset = stream_.Position;
            this.WriteLEInt(ZipConstants.Zip64CentralFileHeaderSignature);
            this.WriteLELong(44);
            this.WriteLEShort(ZipConstants.VersionMadeBy);
            this.WriteLEShort(ZipConstants.VersionZip64);
            this.WriteLEInt(0);
            this.WriteLEInt(0);
            this.WriteLELong(noOfEntries);
            this.WriteLELong(noOfEntries);
            this.WriteLELong(sizeEntries);
            this.WriteLELong(centralDirOffset);
            this.WriteLEInt(ZipConstants.Zip64CentralDirLocatorSignature);
            this.WriteLEInt(0);
            this.WriteLELong(centralSignatureOffset);
            this.WriteLEInt(1);
        }
        public void WriteEndOfCentralDirectory(int noOfEntries, int sizeEntries, int startOfCentralDirectory, byte[] comment)
        {
            this.WriteLEInt(ZipConstants.EndOfCentralDirectorySignature);
            this.WriteLEShort(0);
            this.WriteLEShort(0);
            if (noOfEntries >= 0xffff) 
            {
                this.WriteLEUshort(0xffff);
                this.WriteLEUshort(0xffff);
            }
            else 
            {
                this.WriteLEShort((short)noOfEntries);
                this.WriteLEShort((short)noOfEntries);
            }
            this.WriteLEInt((int)sizeEntries);
            this.WriteLEInt((int)startOfCentralDirectory);
            int commentLength = (comment != null ? comment.Length : 0);
            if (commentLength > 0xffff) 
                throw new Exception(string.Format("Comment length({0}) is too long can only be 64K", commentLength));
            this.WriteLEShort(commentLength);
            if (commentLength > 0) 
                this.Write(comment, 0, comment.Length);
        }
        public int ReadLEShort()
        {
            int byteValue1 = stream_.ReadByte();
            if (byteValue1 < 0) 
                throw new EndOfStreamException();
            int byteValue2 = stream_.ReadByte();
            if (byteValue2 < 0) 
                throw new EndOfStreamException();
            return byteValue1 | (byteValue2 << 8);
        }
        public int ReadLEInt()
        {
            return this.ReadLEShort() | (this.ReadLEShort() << 16);
        }
        public void WriteLEShort(int value)
        {
            stream_.WriteByte((byte)((value & 0xff)));
            stream_.WriteByte((byte)((((value >> 8)) & 0xff)));
        }
        public void WriteLEUshort(ushort value)
        {
            stream_.WriteByte((byte)((value & 0xff)));
            stream_.WriteByte((byte)((value >> 8)));
        }
        public void WriteLEInt(int value)
        {
            this.WriteLEShort(value);
            this.WriteLEShort(value >> 16);
        }
        public void WriteLEUint(uint value)
        {
            this.WriteLEUshort((ushort)((value & 0xffff)));
            this.WriteLEUshort((ushort)((value >> 16)));
        }
        public void WriteLELong(long value)
        {
            this.WriteLEInt((int)value);
            this.WriteLEInt((int)((value >> 32)));
        }
        public void WriteLEUlong(ulong value)
        {
            this.WriteLEUint((uint)((value & 0xffffffff)));
            this.WriteLEUint((uint)((value >> 32)));
        }
        public int WriteDataDescriptor(ZipEntry entry)
        {
            if (entry == null) 
                throw new ArgumentNullException("entry");
            int result = 0;
            if (((entry.Flags & ((int)GeneralBitFlags.Descriptor))) != 0) 
            {
                this.WriteLEInt(ZipConstants.DataDescriptorSignature);
                this.WriteLEInt((int)entry.Crc);
                result += 8;
                if (entry.LocalHeaderRequiresZip64) 
                {
                    this.WriteLELong(entry.CompressedSize);
                    this.WriteLELong(entry.Size);
                    result += 16;
                }
                else 
                {
                    this.WriteLEInt((int)entry.CompressedSize);
                    this.WriteLEInt((int)entry.Size);
                    result += 8;
                }
            }
            return result;
        }
        public void ReadDataDescriptor(bool zip64, DescriptorData data)
        {
            int intValue = this.ReadLEInt();
            if (intValue != ZipConstants.DataDescriptorSignature) 
                throw new Exception("Data descriptor signature not found");
            data.Crc = (uint)this.ReadLEInt();
            if (zip64) 
            {
                data.CompressedSize = this.ReadLEInt();
                this.ReadLEInt();
                data.Size = this.ReadLEInt();
                this.ReadLEInt();
            }
            else 
            {
                data.CompressedSize = this.ReadLEInt();
                data.Size = this.ReadLEInt();
            }
        }
        bool isOwner_;
        Stream stream_;
    }
}