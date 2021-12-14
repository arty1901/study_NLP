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

namespace Pullenti.Unitext.Internal.Word
{
    // Compound File Storage.
    class CompoundFileStorage
    {
        CompoundFileSystem m_System;
        uint m_StreamID;
        CFDirectoryEntry m_Entry;
        IEnumerable<uint> m_Ancestors;
        internal CompoundFileSystem System
        {
            get
            {
                return m_System;
            }
        }
        internal CFDirectoryEntry Entry
        {
            get
            {
                return m_Entry;
            }
        }
        public ExtendedName Name
        {
            get
            {
                return new ExtendedName(null, m_Entry.Name, 0, m_Entry.Name.Length - 1);
            }
        }
        public int Length
        {
            get
            {
                return (int)m_Entry.StreamSize;
            }
        }
        public CompoundFileObjectType ObjectType
        {
            get
            {
                return (CompoundFileObjectType)m_Entry.ObjectType;
            }
        }
        internal CompoundFileStorage(CompoundFileSystem system, uint streamID, IEnumerable<uint> ancestors)
        {
            this.m_System = system;
            this.m_StreamID = streamID;
            this.m_Ancestors = ancestors;
            this.Initialize();
        }
        private void Initialize()
        {
            m_Entry = ReaderUtils.ReadDirectoryEntry(System.BaseStream, this.GetStreamOffset(), System.IsVersion3);
            ReaderUtils.ValidateDirectoryEntry(m_Entry);
        }
        private int GetStreamOffset()
        {
            int offsetWithinLogicalStream = ((int)m_StreamID) * ReaderUtils.DirectoryEntrySize;
            if (!System.IsVersion3 && ((offsetWithinLogicalStream >> System.Header.SectorShift)) >= ((long)System.Header.DirectorySectorsCount)) 
                throw new Exception("Stream ID out of range");
            return System.ToPhysicalStreamOffset(System.Header.FirstDirectorySectorLocation, offsetWithinLogicalStream);
        }
        private void AppendChild(List<CompoundFileStorage> collection, uint streamID, Dictionary<uint, CompoundFileStorage> processed, IEnumerable<uint> ancestors)
        {
            if (processed.ContainsKey(streamID)) 
                throw new Exception("Circular structure: siblings");
            CompoundFileStorage item = new CompoundFileStorage(System, streamID, ancestors);
            processed.Add(streamID, item);
            if (item.Entry.LeftSiblingID != DirectoryStreamIds.NOSTREAM) 
                this.AppendChild(collection, item.Entry.LeftSiblingID, processed, ancestors);
            if (item.ObjectType != CompoundFileObjectType.Unknown) 
                collection.Add(item);
            if (item.Entry.RightSiblingID != DirectoryStreamIds.NOSTREAM) 
                this.AppendChild(collection, item.Entry.RightSiblingID, processed, ancestors);
        }
        public IEnumerable<CompoundFileStorage> GetStorages()
        {
            List<CompoundFileStorage> entries = new List<CompoundFileStorage>();
            Dictionary<uint, CompoundFileStorage> processed = new Dictionary<uint, CompoundFileStorage>();
            uint childID = m_Entry.ChildID;
            if (childID != DirectoryStreamIds.NOSTREAM) 
            {
                List<uint> newAncestors = new List<uint>();
                newAncestors.Add(m_StreamID);
                newAncestors.AddRange(m_Ancestors);
                if (newAncestors.Contains(childID)) 
                    throw new Exception("Circular structure: ancestors");
                this.AppendChild(entries, childID, processed, m_Ancestors);
            }
            return entries;
        }
        public CompoundFileStorage FindStorage(ExtendedName name)
        {
            foreach (CompoundFileStorage storage in this.GetStorages()) 
            {
                if (storage.Name == name) 
                    return storage;
            }
            return null;
        }
        public Stream CreateStream()
        {
            if (m_Entry.ObjectType == DirectoryObjectTypes.Stream) 
            {
                if (m_Entry.StreamSize > ((ulong)System.BaseStream.Length)) 
                    throw new Exception("Stream length");
                if (m_Entry.StreamSize < System.Header.MiniStreamCutoffSize) 
                    return new MiniFATStream(this);
                else 
                    return new FATStream(this);
            }
            else 
                throw new NotSupportedException();
        }
    }
}