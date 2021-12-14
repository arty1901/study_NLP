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
    // File System that base on Compound File structure. The class
    // provides methods read the structure and the data of the
    // Compound File.
    // See Microsoft's [MS-CFB] "Compound File Binary File Format"
    // reference for more details.
    class CompoundFileSystem : IDisposable
    {
        string m_Filename;
        Stream m_BaseStream;
        bool m_Disposed;
        CompoundFileStorage m_RootStorage = null;
        CFHeader m_Header;
        public Stream BaseStream
        {
            get
            {
                return m_BaseStream;
            }
        }
        internal CFHeader Header
        {
            get
            {
                return m_Header;
            }
        }
        internal bool IsVersion3
        {
            get
            {
                return Header.MajorVersion == 3;
            }
        }
        public DateTime Created
        {
            get
            {
                return (string.IsNullOrEmpty(m_Filename) ? DateTime.MinValue : File.GetCreationTimeUtc(m_Filename));
            }
        }
        public DateTime Modified
        {
            get
            {
                return (string.IsNullOrEmpty(m_Filename) ? DateTime.MinValue : File.GetLastWriteTimeUtc(m_Filename));
            }
        }
        public CompoundFileSystem(string filename, byte[] data = null, Stream stream = null)
        {
            if (stream != null) 
                this.m_BaseStream = stream;
            else if (data != null) 
                this.m_BaseStream = new MemoryStream(data);
            else 
            {
                this.m_Filename = filename;
                this.m_BaseStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            this.Initialize();
        }
        ~CompoundFileSystem()
        {
            this.Dispose(false);
        }
        private void Initialize()
        {
            m_Header = ReaderUtils.ReadHeader(BaseStream);
            ReaderUtils.ValidateHeader(m_Header);
        }
        public void Dispose()
        {
            try 
            {
                this.Dispose(true);
            }
            catch(Exception ex) 
            {
            }
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed) 
                return;
            m_Disposed = true;
            m_BaseStream.Dispose();
        }
        public CompoundFileStorage GetRootStorage()
        {
            if (m_RootStorage == null) 
            {
                int RootDirectoryStreamID = 0;
                m_RootStorage = new CompoundFileStorage(this, (uint)RootDirectoryStreamID, new uint[(int)0]);
                this.ValidateRootStorage();
            }
            return m_RootStorage;
        }
        private void ValidateRootStorage()
        {
            if (m_RootStorage.ObjectType != CompoundFileObjectType.Root) 
            {
                m_RootStorage = null;
                throw new Exception("Invalid root storage");
            }
        }
        internal int GetSectorOffset(uint sectorNumber)
        {
            return (int)(((sectorNumber + 1)) << m_Header.SectorShift);
        }
        internal int GetSectorSize()
        {
            return 1 << m_Header.SectorShift;
        }
        internal int GetMiniSectorSize()
        {
            return 1 << m_Header.MiniSectorShift;
        }
        internal int GetMiniSectorOffset(uint sectorNumber)
        {
            return this.ToPhysicalStreamOffset(this.GetRootStorage().Entry.StartingSectorLocation, (int)(sectorNumber << m_Header.MiniSectorShift));
        }
        internal int ToPhysicalStreamOffset(uint firstSector, int offsetWithinLogicalStream)
        {
            int sectorIndex = (int)((offsetWithinLogicalStream >> m_Header.SectorShift));
            if (sectorIndex == 0) 
                return this.GetSectorOffset(firstSector) + offsetWithinLogicalStream;
            uint sector = this.GetStreamNextSector(firstSector, sectorIndex);
            int mask = (1 << Header.SectorShift) - 1;
            return this.GetSectorOffset(sector) + ((offsetWithinLogicalStream & mask));
        }
        internal uint GetStreamNextSector(uint sector, int iterations)
        {
            for (int i = 0; i < iterations; i++) 
            {
                int fatSectorIndex = (int)(((sector >> ((Header.SectorShift - 2)))));
                int mask = (1 << ((Header.SectorShift - 2))) - 1;
                int entryIndex = (int)((sector & mask));
                uint fatSector = this.GetFATSector(fatSectorIndex);
                sector = ReaderUtils.ReadUInt32(BaseStream, this.GetSectorOffset(fatSector) + (entryIndex << 2));
                if (sector > FATSectorIds.MAXREGSECT) 
                    throw new Exception("Short chain");
            }
            return sector;
        }
        internal uint GetMiniStreamNextSector(uint sector, int iterations)
        {
            for (int i = 0; i < iterations; i++) 
            {
                int entryOffset = ((int)sector) << 2;
                if ((entryOffset >> Header.SectorShift) >= ((int)Header.MiniFATSectorsCount)) 
                    throw new Exception("Mini FAT sector index out of range");
                int offset = this.ToPhysicalStreamOffset(Header.FirstMiniFATSectorLocation, entryOffset);
                sector = ReaderUtils.ReadUInt32(BaseStream, offset);
                if (sector > FATSectorIds.MAXREGSECT) 
                    throw new Exception("Short chain");
            }
            return sector;
        }
        private uint GetFATSector(int fatSectorIndex)
        {
            if (fatSectorIndex < ReaderUtils.HeaderDIFATSectorsCount) 
                return Header.DIFAT[fatSectorIndex];
            else 
            {
                int itemsPerPage = ((this.GetSectorSize() >> 2)) - 1;
                int pageIndex = ((fatSectorIndex - ReaderUtils.HeaderDIFATSectorsCount)) / itemsPerPage;
                if (pageIndex >= ((int)Header.FATSectorsCount)) 
                    throw new Exception("FAT sector index out of range");
                uint currentSector = Header.FirstDIFATSectorLocation;
                for (int i = 0; i < pageIndex; i++) 
                {
                    int offset = this.GetSectorOffset(currentSector) + (itemsPerPage << 2);
                    currentSector = ReaderUtils.ReadUInt32(BaseStream, offset);
                }
                int entryIndex = ((fatSectorIndex - ReaderUtils.HeaderDIFATSectorsCount)) % itemsPerPage;
                return ReaderUtils.ReadUInt32(BaseStream, this.GetSectorOffset(currentSector) + (entryIndex << 2));
            }
        }
    }
}