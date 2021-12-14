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
    // This class represents a Zip archive.  You can ask for the contained
    // entries, or get an input stream for a file entry.  The entry is
    // automatically decompressed.
    class ZipFile : IDisposable
    {
        public ZipFile(string name, Stream stream)
        {
            name_ = name;
            if (stream == null) 
            {
                m_BaseStream = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read);
                m_StreamOwner = true;
            }
            else 
            {
                m_BaseStream = stream;
                m_StreamOwner = false;
            }
            try 
            {
                this.ReadEntries();
            }
            catch(Exception ex311) 
            {
                this.DisposeInternal(true);
                throw ex311;
            }
        }
        public void Dispose()
        {
            this.DisposeInternal(true);
        }
        public static ZipFile CreateFile(string fileName)
        {
            if (fileName == null) 
                throw new ArgumentNullException("fileName");
            FileStream f = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            ZipFile result = new ZipFile(null, f);
            result.name_ = fileName;
            result.m_StreamOwner = true;
            return result;
        }
        public static ZipFile CreateStream(Stream outStream)
        {
            if (outStream == null) 
                throw new ArgumentNullException("outStream");
            if (!outStream.CanWrite) 
                throw new ArgumentException("Stream is not writeable", "outStream");
            if (!outStream.CanSeek) 
                throw new ArgumentException("Stream is not seekable", "outStream");
            ZipFile result = new ZipFile(null, outStream);
            return result;
        }
        public bool IsStreamOwner
        {
            get
            {
                return m_StreamOwner;
            }
            set
            {
                m_StreamOwner = value;
            }
        }
        public bool IsEmbeddedArchive
        {
            get
            {
                return offsetOfFirstEntry > 0;
            }
        }
        public bool IsNewArchive
        {
            get
            {
                return isNewArchive_;
            }
        }
        public string ZipFileComment
        {
            get
            {
                return comment_;
            }
        }
        public string Name
        {
            get
            {
                return name_;
            }
        }
        public int Size
        {
            get
            {
                return entries_.Length;
            }
        }
        public long Count
        {
            get
            {
                return entries_.Length;
            }
        }
        public ZipEntry this[int index]
        {
            get
            {
                return (ZipEntry)entries_[index].Clone();
            }
        }
        public List<ZipEntry> ZipEntries
        {
            get
            {
                return new List<ZipEntry>(entries_);
            }
        }
        public int FindEntry(string name, bool ignoreCase)
        {
            if (isDisposed_) 
                throw new ObjectDisposedException("ZipFile");
            for (int i = 0; i < entries_.Length; i++) 
            {
                if (string.Compare(name, entries_[i].Name, ignoreCase) == 0) 
                    return i;
            }
            return -1;
        }
        public ZipEntry GetEntry(string name)
        {
            if (isDisposed_) 
                throw new ObjectDisposedException("ZipFile");
            int index = this.FindEntry(name, true);
            return (index >= 0 ? (ZipEntry)entries_[index].Clone() : null);
        }
        public Stream GetInputStream(ZipEntry entry)
        {
            if (entry == null) 
                throw new ArgumentNullException("entry");
            if (isDisposed_) 
                throw new ObjectDisposedException("ZipFile");
            int index = entry.ZipFileIndex;
            if (((index < 0)) || (index >= entries_.Length) || (entries_[index].Name != entry.Name)) 
            {
                index = this.FindEntry(entry.Name, true);
                if (index < 0) 
                    throw new Exception("Entry cannot be found");
            }
            return this.GetInputStream0(index);
        }
        public Stream GetInputStream0(int entryIndex)
        {
            if (isDisposed_) 
                throw new ObjectDisposedException("ZipFile");
            int start = this.LocateEntry(entries_[entryIndex]);
            CompressionMethod method = entries_[entryIndex]._CompressionMethod;
            Stream result = (Stream)new PartialInputStream(this, start, entries_[entryIndex].CompressedSize);
            if (entries_[entryIndex].IsCrypted == true) 
                throw new Exception("Unable to decrypt this entry");
            switch (method) { 
            case CompressionMethod.Stored:
                break;
            case CompressionMethod.Deflated:
                result = new InflaterInputStream(result, new Inflater(true));
                break;
            default: 
                throw new Exception(string.Format("Unsupported compression method {0}", method));
            }
            return result;
        }
        public bool TestArchive(bool testData)
        {
            return this.TestArchiveEx(testData, TestStrategy.FindFirstError, null);
        }
        public bool TestArchiveEx(bool testData, TestStrategy strategy, ZipTestResultHandler resultHandler)
        {
            if (isDisposed_) 
                throw new ObjectDisposedException("ZipFile");
            TestStatus status = new TestStatus(this);
            if (resultHandler != null) 
                resultHandler(status, null) /* error */;
            HeaderTest test = (testData ? (HeaderTest.Header | HeaderTest.Extract) : HeaderTest.Header);
            bool testing = true;
            try 
            {
                int entryIndex = 0;
                while (testing && ((entryIndex < Count))) 
                {
                    if (resultHandler != null) 
                    {
                        status.SetEntry(this[entryIndex]);
                        status.SetOperation(TestOperation.EntryHeader);
                        resultHandler(status, null) /* error */;
                    }
                    try 
                    {
                        this.TestLocalHeader(this[entryIndex], test);
                    }
                    catch(Exception ex) 
                    {
                        status.AddError();
                        if (resultHandler != null) 
                            resultHandler(status, string.Format("Exception during test - '{0}'", ex.Message)) /* error */;
                        if (strategy == TestStrategy.FindFirstError) 
                            testing = false;
                    }
                    if (testing && testData && this[entryIndex].IsFile) 
                    {
                        if (resultHandler != null) 
                        {
                            status.SetOperation(TestOperation.EntryData);
                            resultHandler(status, null) /* error */;
                        }
                        Crc32 crc = new Crc32();
                        using (Stream entryStream = this.GetInputStream(this[entryIndex])) 
                        {
                            byte[] buffer = new byte[(int)4096];
                            int totalBytes = 0;
                            int bytesRead;
                            while ((((bytesRead = entryStream.Read(buffer, 0, buffer.Length)))) > 0) 
                            {
                                crc.UpdateByBufEx(buffer, 0, bytesRead);
                                if (resultHandler != null) 
                                {
                                    totalBytes += bytesRead;
                                    status.SetBytesTested(totalBytes);
                                    resultHandler(status, null) /* error */;
                                }
                            }
                        }
                        if (this[entryIndex].Crc != crc.Value) 
                        {
                            status.AddError();
                            if (resultHandler != null) 
                                resultHandler(status, "CRC mismatch") /* error */;
                            if (strategy == TestStrategy.FindFirstError) 
                                testing = false;
                        }
                        if (((this[entryIndex].Flags & ((int)GeneralBitFlags.Descriptor))) != 0) 
                        {
                            using (ZipHelperStream helper = new ZipHelperStream(null, m_BaseStream)) 
                            {
                                DescriptorData data = new DescriptorData();
                                helper.ReadDataDescriptor(this[entryIndex].LocalHeaderRequiresZip64, data);
                                if (this[entryIndex].Crc != data.Crc) 
                                    status.AddError();
                                if (this[entryIndex].CompressedSize != data.CompressedSize) 
                                    status.AddError();
                                if (this[entryIndex].Size != data.Size) 
                                    status.AddError();
                            }
                        }
                    }
                    if (resultHandler != null) 
                    {
                        status.SetOperation(TestOperation.EntryComplete);
                        resultHandler(status, null) /* error */;
                    }
                    entryIndex += 1;
                }
                if (resultHandler != null) 
                {
                    status.SetOperation(TestOperation.MiscellaneousTests);
                    resultHandler(status, null) /* error */;
                }
            }
            catch(Exception ex) 
            {
                status.AddError();
                if (resultHandler != null) 
                    resultHandler(status, string.Format("Exception during test - '{0}'", ex.Message)) /* error */;
            }
            if (resultHandler != null) 
            {
                status.SetOperation(TestOperation.Complete);
                status.SetEntry(null);
                resultHandler(status, null) /* error */;
            }
            return status.ErrorCount == 0;
        }
        enum HeaderTest : int
        {
            Extract = 0x01,
            Header = 0x02,
        }

        int TestLocalHeader(ZipEntry entry, HeaderTest tests)
        {
            bool testHeader = ((tests & HeaderTest.Header)) != 0;
            bool testData = ((tests & HeaderTest.Extract)) != 0;
            m_BaseStream.Seek(offsetOfFirstEntry + entry.Offset, SeekOrigin.Begin);
            if (((int)this.ReadLEUint()) != ZipConstants.LocalHeaderSignature) 
                throw new Exception(string.Format("Wrong local header signature @{0}", ((offsetOfFirstEntry + entry.Offset)).ToString("X")));
            short extractVersion = (short)this.ReadLEUshort();
            short localFlags = (short)this.ReadLEUshort();
            short compressionMethod = (short)this.ReadLEUshort();
            short fileTime = (short)this.ReadLEUshort();
            short fileDate = (short)this.ReadLEUshort();
            uint crcValue = this.ReadLEUint();
            int compressedSize = (int)this.ReadLEUint();
            int size = (int)this.ReadLEUint();
            int storedNameLength = (int)this.ReadLEUshort();
            int extraDataLength = (int)this.ReadLEUshort();
            byte[] nameData = new byte[(int)storedNameLength];
            StreamUtils.ReadFully(m_BaseStream, nameData);
            byte[] extraData = new byte[(int)extraDataLength];
            StreamUtils.ReadFully(m_BaseStream, extraData);
            ZipExtraData localExtraData = new ZipExtraData(extraData);
            if (localExtraData.Find(1)) 
            {
                size = localExtraData.ReadLong();
                compressedSize = localExtraData.ReadLong();
                if (((localFlags & ((int)GeneralBitFlags.Descriptor))) != 0) 
                {
                    if ((size != -1) && (size != entry.Size)) 
                        throw new Exception("Size invalid for descriptor");
                    if ((compressedSize != -1) && (compressedSize != entry.CompressedSize)) 
                        throw new Exception("Compressed size invalid for descriptor");
                }
            }
            else if ((extractVersion >= ZipConstants.VersionZip64) && (((((uint)size) == uint.MaxValue) || (((uint)compressedSize) == uint.MaxValue)))) 
                throw new Exception("Required Zip64 extended information missing");
            if (testData) 
            {
                if (entry.IsFile) 
                {
                    if (!entry.IsCompressionMethodSupported()) 
                        throw new Exception("Compression method not supported");
                    if ((extractVersion > ZipConstants.VersionMadeBy) || (((extractVersion > 20) && ((extractVersion < ZipConstants.VersionZip64))))) 
                        throw new Exception(string.Format("Version required to extract this entry not supported ({0})", extractVersion));
                    if (((localFlags & ((int)(((GeneralBitFlags.Patched | GeneralBitFlags.StrongEncryption | GeneralBitFlags.EnhancedCompress) | GeneralBitFlags.HeaderMasked))))) != 0) 
                        throw new Exception("The library does not support the zip version required to extract this entry");
                }
            }
            if (testHeader) 
            {
                if ((((((((extractVersion <= 63) && (extractVersion != 10) && (extractVersion != 11)) && (extractVersion != 20) && (extractVersion != 21)) && (extractVersion != 25) && (extractVersion != 27)) && (extractVersion != 45) && (extractVersion != 46)) && (extractVersion != 50) && (extractVersion != 51)) && (extractVersion != 52) && (extractVersion != 61)) && (extractVersion != 62) && (extractVersion != 63)) 
                    throw new Exception(string.Format("Version required to extract this entry is invalid ({0})", extractVersion));
                if (((localFlags & ((int)((GeneralBitFlags.ReservedPKware4 | GeneralBitFlags.ReservedPkware14 | GeneralBitFlags.ReservedPkware15))))) != 0) 
                    throw new Exception("Reserved bit flags cannot be set.");
                if ((((localFlags & ((int)GeneralBitFlags.Encrypted))) != 0) && ((extractVersion < 20))) 
                    throw new Exception(string.Format("Version required to extract this entry is too low for encryption ({0})", extractVersion));
                if (((localFlags & ((int)GeneralBitFlags.StrongEncryption))) != 0) 
                {
                    if (((localFlags & ((int)GeneralBitFlags.Encrypted))) == 0) 
                        throw new Exception("Strong encryption flag set but encryption flag is not set");
                    if (extractVersion < 50) 
                        throw new Exception(string.Format("Version required to extract this entry is too low for encryption ({0})", extractVersion));
                }
                if ((((localFlags & ((int)GeneralBitFlags.Patched))) != 0) && ((extractVersion < 27))) 
                    throw new Exception(string.Format("Patched data requires higher version than ({0})", extractVersion));
                if (localFlags != entry.Flags) 
                    throw new Exception("Central header/local header flags mismatch");
                if (entry._CompressionMethod != ((CompressionMethod)compressionMethod)) 
                    throw new Exception("Central header/local header compression method mismatch");
                if (entry.Version != extractVersion) 
                    throw new Exception("Extract version mismatch");
                if (((localFlags & ((int)GeneralBitFlags.StrongEncryption))) != 0) 
                {
                    if (extractVersion < 62) 
                        throw new Exception("Strong encryption flag set but version not high enough");
                }
                if (((localFlags & ((int)GeneralBitFlags.HeaderMasked))) != 0) 
                {
                    if ((fileTime != 0) || (fileDate != 0)) 
                        throw new Exception("Header masked set but date/time values non-zero");
                }
                if (((localFlags & ((int)GeneralBitFlags.Descriptor))) == 0) 
                {
                    if (crcValue != entry.Crc) 
                        throw new Exception("Central header/local header crc mismatch");
                }
                if ((size == 0) && (compressedSize == 0)) 
                {
                    if (crcValue != 0) 
                        throw new Exception("Invalid CRC for empty entry");
                }
                if (entry.Name.Length > storedNameLength) 
                    throw new Exception("File name length mismatch");
                string localName = ZipConstants.ConvertToStringExt0(localFlags, nameData);
                if (localName != entry.Name) 
                    throw new Exception("Central header and local header file name mismatch");
                if (entry.IsDirectory) 
                {
                    if (size > 0) 
                        throw new Exception("Directory cannot have size");
                    if (entry.IsCrypted) 
                    {
                        if (compressedSize > (ZipConstants.CryptoHeaderSize + 2)) 
                            throw new Exception("Directory compressed size invalid");
                    }
                    else if (compressedSize > 2) 
                        throw new Exception("Directory compressed size invalid");
                }
                if (!ZipNameTransform.IsValidNameEx(localName, true)) 
                    throw new Exception("Name is invalid");
            }
            if ((((localFlags & ((int)GeneralBitFlags.Descriptor))) == 0) || (((size > 0) || (compressedSize > 0)))) 
            {
            }
            int extraLength = storedNameLength + extraDataLength;
            return (offsetOfFirstEntry + entry.Offset + ZipConstants.LocalHeaderBaseSize) + extraLength;
        }
        const int DefaultBufferSize = 4096;
        public INameTransform NameTransform
        {
            get
            {
                return updateEntryFactory_.NameTransform;
            }
            set
            {
                updateEntryFactory_.NameTransform = value;
            }
        }
        public IEntryFactory EntryFactory
        {
            get
            {
                return updateEntryFactory_;
            }
            set
            {
                if (value == null) 
                    updateEntryFactory_ = new ZipEntryFactory();
                else 
                    updateEntryFactory_ = value;
            }
        }
        public int BufferSize
        {
            get
            {
                return bufferSize_;
            }
            set
            {
                if (value < 1024) 
                    throw new ArgumentOutOfRangeException("value", "cannot be below 1024");
                if (bufferSize_ != value) 
                {
                    bufferSize_ = value;
                    copyBuffer_ = null;
                }
            }
        }
        public bool IsUpdating
        {
            get
            {
                return m_Updates != null;
            }
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
        public void BeginUpdateEx(IArchiveStorage archiveStorage, IDynamicDataSource dataSource)
        {
            if (archiveStorage == null) 
                throw new ArgumentNullException("archiveStorage");
            if (dataSource == null) 
                throw new ArgumentNullException("dataSource");
            if (isDisposed_) 
                throw new ObjectDisposedException("ZipFile");
            if (IsEmbeddedArchive) 
                throw new Exception("Cannot update embedded/SFX archives");
            archiveStorage_ = archiveStorage;
            updateDataSource_ = dataSource;
            updateIndex_ = new Dictionary<object, object>();
            if (entries_ == null) 
                entries_ = new ZipEntry[(int)0];
            m_Updates = new List<object>(entries_.Length);
            foreach (ZipEntry entry in entries_) 
            {
                int index = m_Updates.Count;
                m_Updates.Add(new ZipUpdate(entry, UpdateCommand.Copy));
                updateIndex_.Add(entry.Name, index);
            }
            UpdateComparer cmp = new UpdateComparer();
            for (int ii = 0; ii < m_Updates.Count; ii++) 
            {
                bool ch = false;
                for (int jj = 0; jj < (m_Updates.Count - 1); jj++) 
                {
                    if (cmp.Compare(m_Updates[jj], m_Updates[jj + 1]) > 0) 
                    {
                        object u = m_Updates[jj];
                        m_Updates[jj] = m_Updates[jj + 1];
                        m_Updates[jj + 1] = u;
                        ch = true;
                    }
                }
                if (!ch) 
                    break;
            }
            int idx = 0;
            foreach (object up in m_Updates) 
            {
                ZipUpdate update = up as ZipUpdate;
                if (up == null) 
                    continue;
                if (idx == (m_Updates.Count - 1)) 
                    break;
                update.OffsetBasedSize = ((ZipUpdate)m_Updates[idx + 1]).Entry.Offset - update.Entry.Offset;
                idx++;
            }
            updateCount_ = m_Updates.Count;
            contentsEdited_ = false;
            commentEdited_ = false;
            newComment_ = null;
        }
        public void BeginUpdate(IArchiveStorage archiveStorage)
        {
            this.BeginUpdateEx(archiveStorage, new DynamicDiskDataSource());
        }
        public void BeginUpdate0()
        {
            if (Name == null) 
                this.BeginUpdateEx(new MemoryArchiveStorage(), new DynamicDiskDataSource());
            else 
                this.BeginUpdateEx(new DiskArchiveStorage(this), new DynamicDiskDataSource());
        }
        public void CommitUpdate()
        {
            if (isDisposed_) 
                throw new ObjectDisposedException("ZipFile");
            this.CheckUpdating();
            try 
            {
                updateIndex_.Clear();
                updateIndex_ = null;
                if (contentsEdited_) 
                    this.RunUpdates();
                else if (commentEdited_) 
                    this.UpdateCommentOnly();
                else if (entries_.Length == 0) 
                {
                    byte[] theComment = (newComment_ != null ? newComment_.RawComment : ZipConstants.ConvertToArrayStr(comment_));
                    using (ZipHelperStream zhs = new ZipHelperStream(null, m_BaseStream)) 
                    {
                        zhs.WriteEndOfCentralDirectory(0, 0, 0, theComment);
                    }
                }
            }
            finally
            {
                this.PostUpdateCleanup();
            }
        }
        public void AbortUpdate()
        {
            this.PostUpdateCleanup();
        }
        public void SetComment(string comment)
        {
            if (isDisposed_) 
                throw new ObjectDisposedException("ZipFile");
            this.CheckUpdating();
            newComment_ = new ZipString(comment);
            if (newComment_.RawLength > 0xffff) 
            {
                newComment_ = null;
                throw new Exception("Comment length exceeds maximum - 65535");
            }
            commentEdited_ = true;
        }
        void AddUpdate(ZipUpdate update)
        {
            contentsEdited_ = true;
            int index = this.FindExistingUpdate(update.Entry.Name);
            if (index >= 0) 
            {
                if (m_Updates[index] == null) 
                    updateCount_ += 1;
                m_Updates[index] = update;
            }
            else 
            {
                index = m_Updates.Count;
                m_Updates.Add(update);
                updateCount_ += 1;
                updateIndex_.Add(update.Entry.Name, index);
            }
        }
        public void Add(string fileName, string entryName)
        {
            if (fileName == null) 
                throw new ArgumentNullException("fileName");
            if (entryName == null) 
                throw new ArgumentNullException("entryName");
            this.CheckUpdating();
            this.AddUpdate(new ZipUpdate(EntryFactory.MakeFileEntryEx(entryName, false), UpdateCommand.Add, null, fileName));
        }
        public void AddStream(Stream str, string entryName)
        {
            if (entryName == null) 
                throw new ArgumentNullException("entryName");
            this.CheckUpdating();
            this.AddUpdate(new ZipUpdate(EntryFactory.MakeFileEntryEx(entryName, false), UpdateCommand.Add, null, null, str));
        }
        public void AddDirectory(string directoryName)
        {
            if (directoryName == null) 
                throw new ArgumentNullException("directoryName");
            this.CheckUpdating();
            ZipEntry dirEntry = EntryFactory.MakeDirectoryEntry(directoryName);
            this.AddUpdate(new ZipUpdate(dirEntry, UpdateCommand.Add));
        }
        public bool Delete(string fileName)
        {
            if (fileName == null) 
                throw new ArgumentNullException("fileName");
            this.CheckUpdating();
            bool result = false;
            int index = this.FindExistingUpdate(fileName);
            if ((index >= 0) && (m_Updates[index] != null)) 
            {
                result = true;
                contentsEdited_ = true;
                m_Updates[index] = null;
                updateCount_ -= 1;
            }
            else 
                throw new Exception("Cannot find entry to delete");
            return result;
        }
        void WriteLEShort(int value)
        {
            m_BaseStream.WriteByte((byte)((value & 0xff)));
            m_BaseStream.WriteByte((byte)((((value >> 8)) & 0xff)));
        }
        void WriteLEUshort(ushort value)
        {
            m_BaseStream.WriteByte((byte)((value & 0xff)));
            m_BaseStream.WriteByte((byte)((value >> 8)));
        }
        void WriteLEInt(int value)
        {
            this.WriteLEShort(value & 0xffff);
            this.WriteLEShort(value >> 16);
        }
        void WriteLEUint(uint value)
        {
            this.WriteLEUshort((ushort)((value & 0xffff)));
            this.WriteLEUshort((ushort)((value >> 16)));
        }
        void WriteLeLong(int value1, int value2)
        {
            this.WriteLEInt(value1);
            this.WriteLEInt(value2);
        }
        void WriteLEUlong(uint value1, uint value2)
        {
            this.WriteLEUint(value1);
            this.WriteLEUint(value2);
        }
        void WriteLocalEntryHeader(ZipUpdate update)
        {
            ZipEntry entry = update.OutEntry;
            entry.Offset = (int)m_BaseStream.Position;
            if (update.Command != UpdateCommand.Copy) 
            {
                if (entry._CompressionMethod == CompressionMethod.Deflated) 
                {
                    if (entry.Size == 0) 
                    {
                        entry.CompressedSize = entry.Size;
                        entry.Crc = 0;
                        entry._CompressionMethod = CompressionMethod.Stored;
                    }
                }
                else if (entry._CompressionMethod == CompressionMethod.Stored) 
                    entry.Flags &= (~((int)GeneralBitFlags.Descriptor));
                entry.IsCrypted = false;
                switch (useZip64_) { 
                case UseZip64.Dynamic:
                    if (entry.Size < 0) 
                        entry.ForceZip64();
                    break;
                case UseZip64.On:
                    entry.ForceZip64();
                    break;
                case UseZip64.Off:
                    break;
                }
            }
            this.WriteLEInt(ZipConstants.LocalHeaderSignature);
            this.WriteLEShort(entry.Version);
            this.WriteLEShort(entry.Flags);
            this.WriteLEShort((byte)entry._CompressionMethod);
            this.WriteLEInt((int)entry.DosTime);
            if (!entry.HasCrc) 
            {
                update.CrcPatchOffset = (int)m_BaseStream.Position;
                this.WriteLEInt((int)0);
            }
            else 
                this.WriteLEInt((int)entry.Crc);
            if (entry.LocalHeaderRequiresZip64) 
            {
                this.WriteLEInt(-1);
                this.WriteLEInt(-1);
            }
            else 
            {
                if (((entry.CompressedSize < 0)) || ((entry.Size < 0))) 
                    update.SizePatchOffset = (int)m_BaseStream.Position;
                this.WriteLEInt(entry.CompressedSize);
                this.WriteLEInt(entry.Size);
            }
            byte[] name = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
            if (name.Length > 0xFFFF) 
                throw new Exception("Entry name too long.");
            ZipExtraData ed = new ZipExtraData(entry.ExtraData);
            if (entry.LocalHeaderRequiresZip64) 
            {
                ed.StartNewEntry();
                ed.AddLeLong(entry.Size, 0);
                ed.AddLeLong(entry.CompressedSize, 0);
                ed.AddNewEntry(1);
            }
            else 
                ed.Delete(1);
            entry.ExtraData = ed.GetEntryData();
            this.WriteLEShort(name.Length);
            this.WriteLEShort(entry.ExtraData.Length);
            if (name.Length > 0) 
                m_BaseStream.Write(name, 0, name.Length);
            if (entry.LocalHeaderRequiresZip64) 
            {
                if (!ed.Find(1)) 
                    throw new Exception("Internal error cannot find extra data");
                update.SizePatchOffset = ((int)m_BaseStream.Position) + ed.CurrentReadIndex;
            }
            if (entry.ExtraData.Length > 0) 
                m_BaseStream.Write(entry.ExtraData, 0, entry.ExtraData.Length);
        }
        int WriteCentralDirectoryHeader(ZipEntry entry)
        {
            if (entry.CompressedSize < 0) 
                throw new Exception("Attempt to write central directory entry with unknown csize");
            if (entry.Size < 0) 
                throw new Exception("Attempt to write central directory entry with unknown size");
            this.WriteLEInt(ZipConstants.CentralHeaderSignature);
            this.WriteLEShort(ZipConstants.VersionMadeBy);
            this.WriteLEShort(entry.Version);
            this.WriteLEShort(entry.Flags);
            unchecked 
            {
                this.WriteLEShort((byte)entry._CompressionMethod);
                this.WriteLEInt((int)entry.DosTime);
                this.WriteLEInt((int)entry.Crc);
            }
            if (entry.IsZip64Forced()) 
                this.WriteLEInt(-1);
            else 
                this.WriteLEInt((int)((entry.CompressedSize & 0xffffffff)));
            if (entry.IsZip64Forced()) 
                this.WriteLEInt(-1);
            else 
                this.WriteLEInt((int)entry.Size);
            byte[] name = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
            if (name.Length > 0xFFFF) 
                throw new Exception("Entry name is too long.");
            this.WriteLEShort(name.Length);
            ZipExtraData ed = new ZipExtraData(entry.ExtraData);
            if (entry.CentralHeaderRequiresZip64) 
            {
                ed.StartNewEntry();
                if (useZip64_ == UseZip64.On) 
                    ed.AddLeLong(entry.Size, 0);
                if (useZip64_ == UseZip64.On) 
                    ed.AddLeLong(entry.CompressedSize, 0);
                ed.AddNewEntry(1);
            }
            else 
                ed.Delete(1);
            byte[] centralExtraData = ed.GetEntryData();
            this.WriteLEShort(centralExtraData.Length);
            this.WriteLEShort((entry.Comment != null ? entry.Comment.Length : 0));
            this.WriteLEShort(0);
            this.WriteLEShort(0);
            if (entry.ExternalFileAttributes != -1) 
                this.WriteLEInt(entry.ExternalFileAttributes);
            else if (entry.IsDirectory) 
                this.WriteLEUint(16);
            else 
                this.WriteLEUint(0);
            this.WriteLEUint((uint)entry.Offset);
            if (name.Length > 0) 
                m_BaseStream.Write(name, 0, name.Length);
            if (centralExtraData.Length > 0) 
                m_BaseStream.Write(centralExtraData, 0, centralExtraData.Length);
            byte[] rawComment = (entry.Comment != null ? Pullenti.Util.MiscHelper.EncodeStringAscii(entry.Comment) : new byte[(int)0]);
            if (rawComment.Length > 0) 
                m_BaseStream.Write(rawComment, 0, rawComment.Length);
            return (ZipConstants.CentralHeaderBaseSize + name.Length + centralExtraData.Length) + rawComment.Length;
        }
        void PostUpdateCleanup()
        {
            updateDataSource_ = null;
            m_Updates = null;
            updateIndex_ = null;
            if (archiveStorage_ != null) 
            {
                archiveStorage_.Dispose();
                archiveStorage_ = null;
            }
        }
        string GetTransformedFileName(string name)
        {
            INameTransform transform = NameTransform;
            return (transform != null ? transform.TransformFile(name) : name);
        }
        string GetTransformedDirectoryName(string name)
        {
            INameTransform transform = NameTransform;
            return (transform != null ? transform.TransformDirectory(name) : name);
        }
        byte[] GetBuffer()
        {
            if (copyBuffer_ == null) 
                copyBuffer_ = new byte[(int)bufferSize_];
            return copyBuffer_;
        }
        void CopyDescriptorBytes(ZipUpdate update, Stream dest, Stream source)
        {
            int bytesToCopy = this.GetDescriptorSize(update);
            if (bytesToCopy > 0) 
            {
                byte[] buffer = this.GetBuffer();
                while (bytesToCopy > 0) 
                {
                    int readSize = Math.Min(buffer.Length, bytesToCopy);
                    int bytesRead = source.Read(buffer, 0, readSize);
                    if (bytesRead > 0) 
                    {
                        dest.Write(buffer, 0, bytesRead);
                        bytesToCopy -= bytesRead;
                    }
                    else 
                        throw new Exception("Unxpected end of stream");
                }
            }
        }
        void CopyBytes(ZipUpdate update, Stream destination, Stream source, int bytesToCopy, bool updateCrc)
        {
            if (destination == source) 
                throw new InvalidOperationException("Destination and source are the same");
            Crc32 crc = new Crc32();
            byte[] buffer = this.GetBuffer();
            int targetBytes = bytesToCopy;
            int totalBytesRead = 0;
            int bytesRead;
            do
            {
                int readSize = buffer.Length;
                if (bytesToCopy < readSize) 
                    readSize = (int)bytesToCopy;
                bytesRead = source.Read(buffer, 0, readSize);
                if (bytesRead > 0) 
                {
                    if (updateCrc) 
                        crc.UpdateByBufEx(buffer, 0, bytesRead);
                    destination.Write(buffer, 0, bytesRead);
                    bytesToCopy -= bytesRead;
                    totalBytesRead += bytesRead;
                }
            } while ((bytesRead > 0) && (bytesToCopy > 0)); 
            if (totalBytesRead != targetBytes) 
                throw new Exception(string.Format("Failed to copy bytes expected {0} read {1}", targetBytes, totalBytesRead));
            if (updateCrc) 
                update.OutEntry.Crc = crc.Value;
        }
        int GetDescriptorSize(ZipUpdate update)
        {
            int result = 0;
            if (((update.Entry.Flags & ((int)GeneralBitFlags.Descriptor))) != 0) 
            {
                result = ZipConstants.DataDescriptorSize - 4;
                if (update.Entry.LocalHeaderRequiresZip64) 
                    result = ZipConstants.Zip64DataDescriptorSize - 4;
            }
            return result;
        }
        void CopyDescriptorBytesDirect(ZipUpdate update, Stream stream, ref int destinationPosition, int sourcePosition)
        {
            int bytesToCopy = this.GetDescriptorSize(update);
            while (bytesToCopy > 0) 
            {
                int readSize = (int)bytesToCopy;
                byte[] buffer = this.GetBuffer();
                stream.Position = sourcePosition;
                int bytesRead = stream.Read(buffer, 0, readSize);
                if (bytesRead > 0) 
                {
                    stream.Position = destinationPosition;
                    stream.Write(buffer, 0, bytesRead);
                    bytesToCopy -= bytesRead;
                    destinationPosition += bytesRead;
                    sourcePosition += bytesRead;
                }
                else 
                    throw new Exception("Unxpected end of stream");
            }
        }
        void CopyEntryDataDirect(ZipUpdate update, Stream stream, bool updateCrc, ref int destinationPosition, ref int sourcePosition)
        {
            int bytesToCopy = update.Entry.CompressedSize;
            Crc32 crc = new Crc32();
            byte[] buffer = this.GetBuffer();
            int targetBytes = bytesToCopy;
            int totalBytesRead = 0;
            int bytesRead;
            do
            {
                int readSize = buffer.Length;
                if (bytesToCopy < readSize) 
                    readSize = (int)bytesToCopy;
                stream.Position = sourcePosition;
                bytesRead = stream.Read(buffer, 0, readSize);
                if (bytesRead > 0) 
                {
                    if (updateCrc) 
                        crc.UpdateByBufEx(buffer, 0, bytesRead);
                    stream.Position = destinationPosition;
                    stream.Write(buffer, 0, bytesRead);
                    destinationPosition += bytesRead;
                    sourcePosition += bytesRead;
                    bytesToCopy -= bytesRead;
                    totalBytesRead += bytesRead;
                }
            } while ((bytesRead > 0) && (bytesToCopy > 0)); 
            if (totalBytesRead != targetBytes) 
                throw new Exception(string.Format("Failed to copy bytes expected {0} read {1}", targetBytes, totalBytesRead));
            if (updateCrc) 
                update.OutEntry.Crc = crc.Value;
        }
        int FindExistingUpdate(string fileName)
        {
            int result = -1;
            string convertedName = this.GetTransformedFileName(fileName);
            if (updateIndex_.ContainsKey(convertedName)) 
                result = (int)updateIndex_[convertedName];
            return result;
        }
        Stream GetOutputStream(ZipEntry entry)
        {
            Stream result = m_BaseStream;
            if (entry.IsCrypted == true) 
                throw new Exception("Encryption not supported");
            switch (entry._CompressionMethod) { 
            case CompressionMethod.Stored:
                return new UncompressedStream(result);
            case CompressionMethod.Deflated:
                DeflaterOutputStream dos = new DeflaterOutputStream(result, new Deflater(9, true));
                dos.IsStreamOwner = false;
                return dos;
            default: 
                throw new Exception(string.Format("Unknown compression method {0}", entry._CompressionMethod));
            }
        }
        void AddEntry(ZipFile workFile, ZipUpdate update)
        {
            Stream source = null;
            try 
            {
                if (update.Entry.IsFile) 
                {
                    source = update.GetSource();
                    if (source == null) 
                        source = updateDataSource_.GetSource(update.Entry, update.Filename);
                }
                if (source != null) 
                {
                    int sourceStreamLength = (int)source.Length;
                    if (update.OutEntry.Size < 0) 
                        update.OutEntry.Size = sourceStreamLength;
                    else if (update.OutEntry.Size != sourceStreamLength) 
                    {
                    }
                    workFile.WriteLocalEntryHeader(update);
                    int dataStart = (int)workFile.m_BaseStream.Position;
                    using (Stream output = workFile.GetOutputStream(update.OutEntry)) 
                    {
                        this.CopyBytes(update, output, source, sourceStreamLength, true);
                    }
                    int dataEnd = (int)workFile.m_BaseStream.Position;
                    update.OutEntry.CompressedSize = dataEnd - dataStart;
                    if (((update.OutEntry.Flags & ((int)GeneralBitFlags.Descriptor))) == ((int)GeneralBitFlags.Descriptor)) 
                    {
                        using (ZipHelperStream helper = new ZipHelperStream(null, workFile.m_BaseStream)) 
                        {
                            helper.WriteDataDescriptor(update.OutEntry);
                        }
                    }
                }
                else 
                {
                    workFile.WriteLocalEntryHeader(update);
                    update.OutEntry.CompressedSize = 0;
                }
            }
            finally
            {
                if (source != null) 
                    source.Dispose();
            }
        }
        void ModifyEntry(ZipFile workFile, ZipUpdate update)
        {
            workFile.WriteLocalEntryHeader(update);
            int dataStart = (int)workFile.m_BaseStream.Position;
            if (update.Entry.IsFile && (update.Filename != null)) 
            {
                using (Stream output = workFile.GetOutputStream(update.OutEntry)) 
                {
                    using (Stream source = this.GetInputStream(update.Entry)) 
                    {
                        this.CopyBytes(update, output, source, (int)source.Length, true);
                    }
                }
            }
            int dataEnd = (int)workFile.m_BaseStream.Position;
            update.Entry.CompressedSize = dataEnd - dataStart;
        }
        void CopyEntryDirect(ZipFile workFile, ZipUpdate update, ref int destinationPosition)
        {
            bool skipOver = false;
            if (update.Entry.Offset == destinationPosition) 
                skipOver = true;
            if (!skipOver) 
            {
                m_BaseStream.Position = destinationPosition;
                workFile.WriteLocalEntryHeader(update);
                destinationPosition = (int)m_BaseStream.Position;
            }
            int sourcePosition = 0;
            int NameLengthOffset = 26;
            int entryDataOffset = update.Entry.Offset + NameLengthOffset;
            m_BaseStream.Seek(entryDataOffset, SeekOrigin.Begin);
            uint nameLength = (uint)this.ReadLEUshort();
            uint extraLength = (uint)this.ReadLEUshort();
            sourcePosition = ((int)m_BaseStream.Position) + ((int)nameLength) + ((int)extraLength);
            if (skipOver) 
            {
                if (update.OffsetBasedSize != -1) 
                    destinationPosition += update.OffsetBasedSize;
                else 
                    destinationPosition += ((((sourcePosition - entryDataOffset)) + NameLengthOffset + update.Entry.CompressedSize) + this.GetDescriptorSize(update));
            }
            else 
            {
                if (update.Entry.CompressedSize > 0) 
                    this.CopyEntryDataDirect(update, m_BaseStream, false, ref destinationPosition, ref sourcePosition);
                this.CopyDescriptorBytesDirect(update, m_BaseStream, ref destinationPosition, sourcePosition);
            }
        }
        void CopyEntry(ZipFile workFile, ZipUpdate update)
        {
            workFile.WriteLocalEntryHeader(update);
            if (update.Entry.CompressedSize > 0) 
            {
                int NameLengthOffset = 26;
                int entryDataOffset = ((int)update.Entry.Offset) + NameLengthOffset;
                m_BaseStream.Seek(entryDataOffset, SeekOrigin.Begin);
                uint nameLength = (uint)this.ReadLEUshort();
                uint extraLength = (uint)this.ReadLEUshort();
                m_BaseStream.Seek(nameLength + extraLength, SeekOrigin.Current);
                this.CopyBytes(update, workFile.m_BaseStream, m_BaseStream, update.Entry.CompressedSize, false);
            }
            this.CopyDescriptorBytes(update, workFile.m_BaseStream, m_BaseStream);
        }
        void Reopen(Stream source)
        {
            if (source == null) 
                throw new Exception("Failed to reopen archive - no source");
            isNewArchive_ = false;
            m_BaseStream = source;
            this.ReadEntries();
        }
        void UpdateCommentOnly()
        {
            int baseLength = (int)m_BaseStream.Length;
            ZipHelperStream updateFile = null;
            if (archiveStorage_.UpdateMode == FileUpdateMode.Safe) 
            {
                using (Stream copyStream = archiveStorage_.MakeTemporaryCopy(m_BaseStream)) 
                {
                    updateFile = new ZipHelperStream(null, copyStream);
                    updateFile.IsStreamOwner = true;
                    m_BaseStream.Dispose();
                    m_BaseStream = null;
                }
            }
            else if (archiveStorage_.UpdateMode == FileUpdateMode.Direct) 
            {
                m_BaseStream = archiveStorage_.OpenForDirectUpdate(m_BaseStream);
                updateFile = new ZipHelperStream(null, m_BaseStream);
            }
            else 
            {
                m_BaseStream.Dispose();
                m_BaseStream = null;
                updateFile = new ZipHelperStream(Name);
            }
            try 
            {
                int locatedCentralDirOffset = updateFile.LocateBlockWithSignature(ZipConstants.EndOfCentralDirectorySignature, baseLength, ZipConstants.EndOfCentralRecordBaseSize, 0xffff);
                if (locatedCentralDirOffset < 0) 
                    throw new Exception("Cannot find central directory");
                int CentralHeaderCommentSizeOffset = 16;
                updateFile.Position += CentralHeaderCommentSizeOffset;
                byte[] rawComment = newComment_.RawComment;
                updateFile.WriteLEShort(rawComment.Length);
                updateFile.Write(rawComment, 0, rawComment.Length);
                updateFile.SetLength(updateFile.Position);
            }
            finally
            {
                updateFile.Dispose();
            }
            if (archiveStorage_.UpdateMode == FileUpdateMode.Safe) 
                this.Reopen(archiveStorage_.ConvertTemporaryToFinal());
            else 
                this.ReadEntries();
        }
        /// <summary>
        /// Class used to sort updates.
        /// </summary>
        class UpdateComparer : IComparer<object>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is 
            /// less than, equal to or greater than the other.
            /// </summary>
            /// <param name="x">First object to compare</param>
            /// <param name="y">Second object to compare.</param>
            /// <return>Compare result.</return>
            public int Compare(object x, object y)
            {
                Pullenti.Unitext.Internal.Zip.ZipUpdate zx = x as Pullenti.Unitext.Internal.Zip.ZipUpdate;
                Pullenti.Unitext.Internal.Zip.ZipUpdate zy = y as Pullenti.Unitext.Internal.Zip.ZipUpdate;
                int result;
                if (zx == null) 
                {
                    if (zy == null) 
                        result = 0;
                    else 
                        result = -1;
                }
                else if (zy == null) 
                    result = 1;
                else 
                {
                    int xCmdValue = (((zx.Command == Pullenti.Unitext.Internal.Zip.UpdateCommand.Copy) || (zx.Command == Pullenti.Unitext.Internal.Zip.UpdateCommand.Modify)) ? 0 : 1);
                    int yCmdValue = (((zy.Command == Pullenti.Unitext.Internal.Zip.UpdateCommand.Copy) || (zy.Command == Pullenti.Unitext.Internal.Zip.UpdateCommand.Modify)) ? 0 : 1);
                    result = xCmdValue - yCmdValue;
                    if (result == 0) 
                    {
                        int offsetDiff = zx.Entry.Offset - zy.Entry.Offset;
                        if (offsetDiff < 0) 
                            result = -1;
                        else if (offsetDiff == 0) 
                            result = 0;
                        else 
                            result = 1;
                    }
                }
                return result;
            }
        }

        void RunUpdates()
        {
            int sizeEntries = 0;
            int endOfStream = 0;
            bool directUpdate = false;
            int destinationPosition = 0;
            ZipFile workFile;
            if (IsNewArchive) 
            {
                workFile = this;
                workFile.m_BaseStream.Position = 0;
                directUpdate = true;
            }
            else if (archiveStorage_.UpdateMode == FileUpdateMode.Direct) 
            {
                workFile = this;
                workFile.m_BaseStream.Position = 0;
                directUpdate = true;
                UpdateComparer cmp = new UpdateComparer();
                for (int ii = 0; ii < m_Updates.Count; ii++) 
                {
                    bool ch = false;
                    for (int jj = 0; jj < (m_Updates.Count - 1); jj++) 
                    {
                        if (cmp.Compare(m_Updates[jj], m_Updates[jj + 1]) > 0) 
                        {
                            object u = m_Updates[jj];
                            m_Updates[jj] = m_Updates[jj + 1];
                            m_Updates[jj + 1] = u;
                            ch = true;
                        }
                    }
                    if (!ch) 
                        break;
                }
            }
            else 
            {
                workFile = ZipFile.CreateStream(archiveStorage_.GetTemporaryOutput());
                workFile.UseZip64 = UseZip64;
            }
            try 
            {
                foreach (object up in m_Updates) 
                {
                    ZipUpdate update = up as ZipUpdate;
                    if (update != null) 
                    {
                        switch (update.Command) { 
                        case UpdateCommand.Copy:
                            if (directUpdate) 
                                this.CopyEntryDirect(workFile, update, ref destinationPosition);
                            else 
                                this.CopyEntry(workFile, update);
                            break;
                        case UpdateCommand.Modify:
                            this.ModifyEntry(workFile, update);
                            break;
                        case UpdateCommand.Add:
                            if (!IsNewArchive && directUpdate) 
                                workFile.m_BaseStream.Position = destinationPosition;
                            this.AddEntry(workFile, update);
                            if (directUpdate) 
                                destinationPosition = (int)workFile.m_BaseStream.Position;
                            break;
                        }
                    }
                }
                if (!IsNewArchive && directUpdate) 
                    workFile.m_BaseStream.Position = destinationPosition;
                int centralDirOffset = (int)workFile.m_BaseStream.Position;
                foreach (object up in m_Updates) 
                {
                    ZipUpdate update = up as ZipUpdate;
                    if (update != null) 
                        sizeEntries += workFile.WriteCentralDirectoryHeader(update.OutEntry);
                }
                byte[] theComment = (newComment_ != null ? newComment_.RawComment : ZipConstants.ConvertToArrayStr(comment_));
                using (ZipHelperStream zhs = new ZipHelperStream(null, workFile.m_BaseStream)) 
                {
                    zhs.WriteEndOfCentralDirectory(updateCount_, sizeEntries, centralDirOffset, theComment);
                }
                endOfStream = (int)workFile.m_BaseStream.Position;
                foreach (object up in m_Updates) 
                {
                    ZipUpdate update = up as ZipUpdate;
                    if (update != null) 
                    {
                        if ((update.CrcPatchOffset > 0) && (update.OutEntry.CompressedSize > 0)) 
                        {
                            workFile.m_BaseStream.Position = update.CrcPatchOffset;
                            workFile.WriteLEInt((int)update.OutEntry.Crc);
                        }
                        if (update.SizePatchOffset > 0) 
                        {
                            workFile.m_BaseStream.Position = update.SizePatchOffset;
                            if (update.OutEntry.LocalHeaderRequiresZip64) 
                            {
                                workFile.WriteLeLong(update.OutEntry.Size, 0);
                                workFile.WriteLeLong(update.OutEntry.CompressedSize, 0);
                            }
                            else 
                            {
                                workFile.WriteLEInt((int)update.OutEntry.CompressedSize);
                                workFile.WriteLEInt((int)update.OutEntry.Size);
                            }
                        }
                    }
                }
            }
            catch(Exception ex312) 
            {
                if (!directUpdate && (workFile.Name != null)) 
                    File.Delete(workFile.Name);
                throw ex312;
            }
            finally
            {
                if (workFile != null) 
                    workFile.Dispose();
                workFile = null;
            }
            if (directUpdate) 
            {
                if (workFile != null) 
                {
                    workFile.m_BaseStream.SetLength(endOfStream);
                    workFile.m_BaseStream.Flush();
                }
                isNewArchive_ = false;
                this.ReadEntries();
            }
            else 
            {
                m_BaseStream.Dispose();
                this.Reopen(archiveStorage_.ConvertTemporaryToFinal());
            }
        }
        void CheckUpdating()
        {
            if (m_Updates == null) 
                throw new InvalidOperationException("BeginUpdate has not been called");
        }
        void DisposeInternal(bool disposing)
        {
            if (!isDisposed_) 
            {
                isDisposed_ = true;
                entries_ = new ZipEntry[(int)0];
                if (IsStreamOwner && (m_BaseStream != null)) 
                {
                    m_BaseStream.Dispose();
                    m_BaseStream = null;
                }
                this.PostUpdateCleanup();
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            this.DisposeInternal(disposing);
        }
        ushort ReadLEUshort()
        {
            int data1 = m_BaseStream.ReadByte();
            if (data1 < 0) 
                throw new EndOfStreamException("End of stream");
            int data2 = m_BaseStream.ReadByte();
            if (data2 < 0) 
                throw new EndOfStreamException("End of stream");
            return unchecked((ushort)((((ushort)data1) | ((ushort)(data2 << 8)))));
        }
        uint ReadLEUint()
        {
            return (uint)((this.ReadLEUshort() | (this.ReadLEUshort() << 16)));
        }
        int LocateBlockWithSignature(int signature, int endLocation, int minimumBlockSize, int maximumVariableData)
        {
            using (ZipHelperStream les = new ZipHelperStream(null, m_BaseStream)) 
            {
                return les.LocateBlockWithSignature(signature, endLocation, minimumBlockSize, maximumVariableData);
            }
        }
        void ReadEntries()
        {
            if (m_BaseStream == null) 
                return;
            if (m_BaseStream.Length == 0) 
                return;
            if (m_BaseStream.CanSeek == false) 
                throw new Exception("ZipFile stream must be seekable");
            int locatedEndOfCentralDir = this.LocateBlockWithSignature(ZipConstants.EndOfCentralDirectorySignature, (int)m_BaseStream.Length, ZipConstants.EndOfCentralRecordBaseSize, 0xffff);
            if (locatedEndOfCentralDir < 0) 
                throw new Exception("Cannot find central directory");
            ushort thisDiskNumber = this.ReadLEUshort();
            ushort startCentralDirDisk = this.ReadLEUshort();
            int entriesForThisDisk = (int)this.ReadLEUshort();
            int entriesForWholeCentralDir = (int)this.ReadLEUshort();
            int centralDirSize = (int)this.ReadLEUint();
            int offsetOfCentralDir = (int)this.ReadLEUint();
            uint commentSize = (uint)this.ReadLEUshort();
            if (commentSize > 0) 
            {
                byte[] comment = new byte[(int)commentSize];
                StreamUtils.ReadFully(m_BaseStream, comment);
                comment_ = ZipConstants.ConvertToString0(comment);
            }
            else 
                comment_ = string.Empty;
            bool isZip64 = false;
            if (thisDiskNumber == 0xffff) 
            {
                isZip64 = true;
                int offset = this.LocateBlockWithSignature(ZipConstants.Zip64CentralDirLocatorSignature, locatedEndOfCentralDir, 0, 0x1000);
                if (offset < 0) 
                    throw new Exception("Cannot find Zip64 locator");
                this.ReadLEUint();
                int offset64 = (int)this.ReadLEUint();
                this.ReadLEUint();
                uint totalDisks = this.ReadLEUint();
                m_BaseStream.Position = (long)offset64;
                int sig64 = (int)this.ReadLEUint();
                if (sig64 != ZipConstants.Zip64CentralFileHeaderSignature) 
                    throw new Exception(string.Format("Invalid Zip64 Central directory signature at {0}", offset64.ToString("X")));
                uint recordSize = this.ReadLEUint();
                this.ReadLEUint();
                int versionMadeBy = (int)this.ReadLEUshort();
                int versionToExtract = (int)this.ReadLEUshort();
                uint thisDisk = this.ReadLEUint();
                uint centralDirDisk = this.ReadLEUint();
                entriesForThisDisk = (int)this.ReadLEUint();
                this.ReadLEUint();
                entriesForWholeCentralDir = (int)this.ReadLEUint();
                this.ReadLEUint();
                centralDirSize = (int)this.ReadLEUint();
                this.ReadLEUint();
                offsetOfCentralDir = (int)this.ReadLEUint();
                this.ReadLEUint();
            }
            entries_ = new ZipEntry[(int)entriesForThisDisk];
            if (!isZip64 && ((offsetOfCentralDir < (locatedEndOfCentralDir - ((4 + ((long)centralDirSize))))))) 
            {
                offsetOfFirstEntry = locatedEndOfCentralDir - ((4 + centralDirSize + offsetOfCentralDir));
                if (offsetOfFirstEntry <= 0) 
                    throw new Exception("Invalid embedded zip archive");
            }
            m_BaseStream.Seek(offsetOfFirstEntry + offsetOfCentralDir, SeekOrigin.Begin);
            for (int i = 0; i < entriesForThisDisk; i++) 
            {
                if (((int)this.ReadLEUint()) != ZipConstants.CentralHeaderSignature) 
                    throw new Exception("Wrong Central Directory signature");
                int versionMadeBy = (int)this.ReadLEUshort();
                int versionToExtract = (int)this.ReadLEUshort();
                int bitFlags = (int)this.ReadLEUshort();
                int method = (int)this.ReadLEUshort();
                uint dostime = this.ReadLEUint();
                uint crc = this.ReadLEUint();
                int csize = (int)this.ReadLEUint();
                int size = (int)this.ReadLEUint();
                int nameLen = (int)this.ReadLEUshort();
                int extraLen = (int)this.ReadLEUshort();
                int commentLen = (int)this.ReadLEUshort();
                int diskStartNo = (int)this.ReadLEUshort();
                int internalAttributes = (int)this.ReadLEUshort();
                uint externalAttributes = this.ReadLEUint();
                int offset = (int)this.ReadLEUint();
                byte[] buffer = new byte[(int)Math.Max(nameLen, commentLen)];
                StreamUtils.ReadFullyEx(m_BaseStream, buffer, 0, nameLen);
                string name = ZipConstants.ConvertToStringExt(bitFlags, buffer, nameLen);
                ZipEntry entry = new ZipEntry(name, versionToExtract, versionMadeBy, (CompressionMethod)method);
                entry.Crc = crc;
                entry.Size = size;
                entry.CompressedSize = csize;
                entry.Flags = bitFlags;
                entry.DosTime = (uint)dostime;
                entry.ZipFileIndex = i;
                entry.Offset = offset;
                entry.ExternalFileAttributes = (int)externalAttributes;
                if (((bitFlags & 8)) == 0) 
                    entry.CryptoCheckValue = (byte)((crc >> 24));
                else 
                    entry.CryptoCheckValue = (byte)((((dostime >> 8)) & 0xff));
                if (extraLen > 0) 
                {
                    byte[] extra = new byte[(int)extraLen];
                    StreamUtils.ReadFully(m_BaseStream, extra);
                    entry.ExtraData = extra;
                }
                entry.ProcessExtraData(false);
                if (commentLen > 0) 
                {
                    StreamUtils.ReadFullyEx(m_BaseStream, buffer, 0, commentLen);
                    entry.Comment = ZipConstants.ConvertToStringExt(bitFlags, buffer, commentLen);
                }
                entries_[i] = entry;
            }
        }
        int LocateEntry(ZipEntry entry)
        {
            return this.TestLocalHeader(entry, HeaderTest.Extract);
        }
        bool isDisposed_;
        string name_;
        string comment_;
        internal Stream m_BaseStream;
        bool m_StreamOwner;
        int offsetOfFirstEntry;
        ZipEntry[] entries_;
        bool isNewArchive_;
        UseZip64 useZip64_ = UseZip64.Dynamic;
        List<object> m_Updates;
        int updateCount_;
        Dictionary<object, object> updateIndex_;
        IArchiveStorage archiveStorage_;
        IDynamicDataSource updateDataSource_;
        bool contentsEdited_;
        int bufferSize_ = DefaultBufferSize;
        byte[] copyBuffer_;
        ZipString newComment_;
        bool commentEdited_;
        IEntryFactory updateEntryFactory_ = new ZipEntryFactory();
    }
}