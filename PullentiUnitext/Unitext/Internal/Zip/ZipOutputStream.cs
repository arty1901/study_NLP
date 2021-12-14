/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Pullenti.Unitext.Internal.Zip
{
    // This is a DeflaterOutputStream that writes the files into a zip
    // archive one after another.  It has a special method to start a new
    // zip entry.  The zip entries contains information about the file name
    // size, compressed size, CRC, etc.
    // It includes support for Stored and Deflated entries.
    // This class is not thread safe.
    class ZipOutputStream : DeflaterOutputStream
    {
        public ZipOutputStream(Stream baseOutputStream, int bufferSize = 512) : base(baseOutputStream, new Deflater(Deflater.DEFAULT_COMPRESSION, true), bufferSize)
        {
        }
        public bool IsFinished
        {
            get
            {
                return entries == null;
            }
        }
        public void SetComment(string comment)
        {
            byte[] commentBytes = ZipConstants.ConvertToArrayStr(comment);
            if (commentBytes.Length > 0xffff) 
                throw new ArgumentOutOfRangeException("comment");
            zipComment = commentBytes;
        }
        public void SetLevel(int level)
        {
            deflater_.SetLevel(level);
            defaultCompressionLevel = level;
        }
        public int GetLevel()
        {
            return deflater_.GetLevel();
        }
        public UseZip64 UseZip64
        {
            get
            {
                return useZip64_;
            }
            set
            {
                useZip64_ = value;
            }
        }
        private void WriteLeShort(int value)
        {
            unchecked 
            {
                baseOutputStream_.WriteByte((byte)((value & 0xff)));
                baseOutputStream_.WriteByte((byte)((((value >> 8)) & 0xff)));
            }
        }
        private void WriteLeInt(int value)
        {
            unchecked 
            {
                this.WriteLeShort(value);
                this.WriteLeShort(value >> 16);
            }
        }
        private void WriteLeLong(long value)
        {
            unchecked 
            {
                this.WriteLeInt((int)value);
                this.WriteLeInt((int)((value >> 32)));
            }
        }
        public void PutNextEntry(ZipEntry entry)
        {
            if (entry == null) 
                throw new ArgumentNullException("entry");
            if (entries == null) 
                throw new InvalidOperationException("ZipOutputStream was finished");
            if (curEntry != null) 
                this.CloseEntry();
            if (entries.Count == int.MaxValue) 
                throw new Exception("Too many entries for Zip file");
            CompressionMethod method = entry._CompressionMethod;
            int compressionLevel = defaultCompressionLevel;
            entry.Flags &= ((int)GeneralBitFlags.UnicodeText);
            patchEntryHeader = false;
            bool headerInfoAvailable;
            if (entry.Size == 0) 
            {
                entry.CompressedSize = entry.Size;
                entry.Crc = 0;
                method = CompressionMethod.Stored;
                headerInfoAvailable = true;
            }
            else 
            {
                headerInfoAvailable = (entry.Size >= 0) && entry.HasCrc;
                if (method == CompressionMethod.Stored) 
                {
                    if (!headerInfoAvailable) 
                    {
                        if (!CanPatchEntries) 
                        {
                            method = CompressionMethod.Deflated;
                            compressionLevel = 0;
                        }
                    }
                    else 
                    {
                        entry.CompressedSize = entry.Size;
                        headerInfoAvailable = entry.HasCrc;
                    }
                }
            }
            if (headerInfoAvailable == false) 
            {
                if (CanPatchEntries == false) 
                    entry.Flags |= 8;
                else 
                    patchEntryHeader = true;
            }
            entry.Offset = offset;
            entry._CompressionMethod = (CompressionMethod)method;
            curMethod = method;
            sizePatchPos = -1;
            if ((useZip64_ == UseZip64.On) || ((((entry.Size < 0)) && (useZip64_ == UseZip64.Dynamic)))) 
                entry.ForceZip64();
            this.WriteLeInt(ZipConstants.LocalHeaderSignature);
            this.WriteLeShort(entry.Version);
            this.WriteLeShort(entry.Flags);
            this.WriteLeShort((byte)entry.CompressionMethodForHeader);
            this.WriteLeInt((int)entry.DosTime);
            if (headerInfoAvailable == true) 
            {
                this.WriteLeInt((int)entry.Crc);
                if (entry.LocalHeaderRequiresZip64) 
                {
                    this.WriteLeInt(-1);
                    this.WriteLeInt(-1);
                }
                else 
                {
                    this.WriteLeInt((entry.IsCrypted ? ((int)entry.CompressedSize) + ZipConstants.CryptoHeaderSize : (int)entry.CompressedSize));
                    this.WriteLeInt((int)entry.Size);
                }
            }
            else 
            {
                if (patchEntryHeader) 
                    crcPatchPos = baseOutputStream_.Position;
                this.WriteLeInt(0);
                if (patchEntryHeader) 
                    sizePatchPos = baseOutputStream_.Position;
                if (entry.LocalHeaderRequiresZip64 || patchEntryHeader) 
                {
                    this.WriteLeInt(-1);
                    this.WriteLeInt(-1);
                }
                else 
                {
                    this.WriteLeInt(0);
                    this.WriteLeInt(0);
                }
            }
            byte[] name = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
            if (name.Length > 0xFFFF) 
                throw new Exception("Entry name too long.");
            ZipExtraData ed = new ZipExtraData(entry.ExtraData);
            if (entry.LocalHeaderRequiresZip64) 
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
                if (patchEntryHeader) 
                    sizePatchPos = ed.CurrentReadIndex;
            }
            else 
                ed.Delete(1);
            byte[] extra = ed.GetEntryData();
            this.WriteLeShort(name.Length);
            this.WriteLeShort(extra.Length);
            if (name.Length > 0) 
                baseOutputStream_.Write(name, 0, name.Length);
            if (entry.LocalHeaderRequiresZip64 && patchEntryHeader) 
                sizePatchPos += baseOutputStream_.Position;
            if (extra.Length > 0) 
                baseOutputStream_.Write(extra, 0, extra.Length);
            offset += (ZipConstants.LocalHeaderBaseSize + name.Length + extra.Length);
            if (entry.AESKeySize > 0) 
                offset += entry.AESOverheadSize;
            curEntry = entry;
            crc.Reset();
            if (method == CompressionMethod.Deflated) 
            {
                deflater_.Reset();
                deflater_.SetLevel(compressionLevel);
            }
            size = 0;
        }
        public void CloseEntry()
        {
            if (curEntry == null) 
                throw new InvalidOperationException("No open entry");
            int csize = size;
            if (curMethod == CompressionMethod.Deflated) 
            {
                if (size >= 0) 
                {
                    base.Finish();
                    csize = deflater_.TotalOut;
                }
                else 
                    deflater_.Reset();
            }
            if (curEntry.Size < 0) 
                curEntry.Size = size;
            else if (curEntry.Size != size) 
                throw new Exception(("size was " + size + ", but I expected ") + curEntry.Size);
            if (curEntry.CompressedSize < 0) 
                curEntry.CompressedSize = csize;
            else if (curEntry.CompressedSize != csize) 
                throw new Exception(("compressed size was " + csize + ", but I expected ") + curEntry.CompressedSize);
            if (!curEntry.CrcOk) 
                curEntry.Crc = crc.Value;
            else if (curEntry.Crc != crc.Value) 
                throw new Exception(("crc was " + crc.Value + ", but I expected ") + curEntry.Crc);
            offset += csize;
            if (curEntry.IsCrypted) 
            {
                if (curEntry.AESKeySize > 0) 
                    curEntry.CompressedSize += curEntry.AESOverheadSize;
                else 
                    curEntry.CompressedSize += ZipConstants.CryptoHeaderSize;
            }
            if (patchEntryHeader) 
            {
                patchEntryHeader = false;
                long curPos = baseOutputStream_.Position;
                baseOutputStream_.Seek(crcPatchPos, SeekOrigin.Begin);
                this.WriteLeInt((int)curEntry.Crc);
                if (curEntry.LocalHeaderRequiresZip64) 
                {
                    if (sizePatchPos == -1) 
                        throw new Exception("Entry requires zip64 but this has been turned off");
                    baseOutputStream_.Seek(sizePatchPos, SeekOrigin.Begin);
                    this.WriteLeLong(curEntry.Size);
                    this.WriteLeLong(curEntry.CompressedSize);
                }
                else 
                {
                    this.WriteLeInt((int)curEntry.CompressedSize);
                    this.WriteLeInt((int)curEntry.Size);
                }
                baseOutputStream_.Seek(curPos, SeekOrigin.Begin);
            }
            if (((curEntry.Flags & 8)) != 0) 
            {
                this.WriteLeInt(ZipConstants.DataDescriptorSignature);
                this.WriteLeInt((int)curEntry.Crc);
                if (curEntry.LocalHeaderRequiresZip64) 
                {
                    this.WriteLeLong(curEntry.CompressedSize);
                    this.WriteLeLong(curEntry.Size);
                    offset += ZipConstants.Zip64DataDescriptorSize;
                }
                else 
                {
                    this.WriteLeInt((int)curEntry.CompressedSize);
                    this.WriteLeInt((int)curEntry.Size);
                    offset += ZipConstants.DataDescriptorSize;
                }
            }
            entries.Add(curEntry);
            curEntry = null;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (curEntry == null) 
                throw new IOException("No open entry.");
            if (buffer == null) 
                throw new IOException("null buffer");
            if (offset < 0) 
                throw new Exception("offset out of range");
            if (count < 0) 
                throw new Exception("bad count");
            if (((buffer.Length - offset)) < count) 
                throw new IOException("Invalid offset/count combination");
            try 
            {
                crc.UpdateByBufEx(buffer, offset, count);
            }
            catch(Exception ex) 
            {
                throw new IOException(ex.Message, ex);
            }
            size += count;
            try 
            {
                switch (curMethod) { 
                case CompressionMethod.Deflated:
                    base.Write(buffer, offset, count);
                    break;
                case CompressionMethod.Stored:
                    baseOutputStream_.Write(buffer, offset, count);
                    break;
                }
            }
            catch(Exception ex) 
            {
                throw new IOException(ex.Message, ex);
            }
        }
        public override void Finish()
        {
            if (entries == null) 
                return;
            if (curEntry != null) 
                this.CloseEntry();
            int numEntries = entries.Count;
            int sizeEntries = 0;
            foreach (object en in entries) 
            {
                ZipEntry entry = en as ZipEntry;
                if (entry == null) 
                    continue;
                this.WriteLeInt(ZipConstants.CentralHeaderSignature);
                this.WriteLeShort(ZipConstants.VersionMadeBy);
                this.WriteLeShort(entry.Version);
                this.WriteLeShort(entry.Flags);
                this.WriteLeShort((short)entry.CompressionMethodForHeader);
                this.WriteLeInt((int)entry.DosTime);
                this.WriteLeInt((int)entry.Crc);
                if (entry.IsZip64Forced() || (((ulong)entry.CompressedSize) >= uint.MaxValue)) 
                    this.WriteLeInt(-1);
                else 
                    this.WriteLeInt((int)entry.CompressedSize);
                if (entry.IsZip64Forced() || (((ulong)entry.Size) >= uint.MaxValue)) 
                    this.WriteLeInt(-1);
                else 
                    this.WriteLeInt((int)entry.Size);
                byte[] name = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
                if (name.Length > 0xffff) 
                    throw new Exception("Name too long.");
                ZipExtraData ed = new ZipExtraData(entry.ExtraData);
                if (entry.CentralHeaderRequiresZip64) 
                {
                    ed.StartNewEntry();
                    if (entry.IsZip64Forced()) 
                        ed.AddLeLong(entry.Size, 0);
                    if (entry.IsZip64Forced()) 
                        ed.AddLeLong(entry.CompressedSize, 0);
                    ed.AddNewEntry(1);
                }
                else 
                    ed.Delete(1);
                byte[] extra = ed.GetEntryData();
                byte[] entryComment = (entry.Comment != null ? ZipConstants.ConvertToArray(entry.Flags, entry.Comment) : new byte[(int)0]);
                if (entryComment.Length > 0xffff) 
                    throw new Exception("Comment too long.");
                this.WriteLeShort(name.Length);
                this.WriteLeShort(extra.Length);
                this.WriteLeShort(entryComment.Length);
                this.WriteLeShort(0);
                this.WriteLeShort(0);
                if (entry.ExternalFileAttributes != -1) 
                    this.WriteLeInt(entry.ExternalFileAttributes);
                else if (entry.IsDirectory) 
                    this.WriteLeInt(16);
                else 
                    this.WriteLeInt(0);
                if (((ulong)entry.Offset) >= uint.MaxValue) 
                    this.WriteLeInt(-1);
                else 
                    this.WriteLeInt((int)entry.Offset);
                if (name.Length > 0) 
                    baseOutputStream_.Write(name, 0, name.Length);
                if (extra.Length > 0) 
                    baseOutputStream_.Write(extra, 0, extra.Length);
                if (entryComment.Length > 0) 
                    baseOutputStream_.Write(entryComment, 0, entryComment.Length);
                sizeEntries += ((ZipConstants.CentralHeaderBaseSize + name.Length + extra.Length) + entryComment.Length);
            }
            using (ZipHelperStream zhs = new ZipHelperStream(null, baseOutputStream_)) 
            {
                zhs.WriteEndOfCentralDirectory(numEntries, sizeEntries, offset, zipComment);
            }
            entries = null;
        }
        List<object> entries = new List<object>();
        Crc32 crc = new Crc32();
        ZipEntry curEntry;
        int defaultCompressionLevel = Deflater.DEFAULT_COMPRESSION;
        CompressionMethod curMethod = CompressionMethod.Deflated;
        int size;
        int offset;
        byte[] zipComment = new byte[(int)0];
        bool patchEntryHeader;
        long crcPatchPos = -1;
        long sizePatchPos = -1;
        UseZip64 useZip64_ = UseZip64.Dynamic;
    }
}