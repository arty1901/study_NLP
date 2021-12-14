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
    static class ReaderUtils
    {
        internal const int HeaderDIFATSectorsCount = 109;
        internal const int DirectoryEntrySize = 128;
        internal const int HeaderSize = 512;
        internal static CFHeader ReadHeader(Stream s)
        {
            s.Position = 0;
            byte[] headerBytes = new byte[(int)HeaderSize];
            int headerRead = s.Read(headerBytes, 0, HeaderSize);
            if (headerRead != HeaderSize) 
                throw new Exception("Invalid header: eof of file");
            CFHeader header = new CFHeader();
            header.Signature1 = BitConverter.ToUInt32(headerBytes, 0);
            header.Signature2 = BitConverter.ToUInt32(headerBytes, 4);
            header.MinorVersion = BitConverter.ToUInt16(headerBytes, 24);
            header.MajorVersion = BitConverter.ToUInt16(headerBytes, 26);
            header.ByteOrder = BitConverter.ToUInt16(headerBytes, 28);
            header.SectorShift = BitConverter.ToUInt16(headerBytes, 30);
            header.MiniSectorShift = BitConverter.ToUInt16(headerBytes, 32);
            header.Reserved = GetByteArrayPortion(headerBytes, 34, 6);
            header.DirectorySectorsCount = BitConverter.ToUInt32(headerBytes, 40);
            header.FATSectorsCount = BitConverter.ToUInt32(headerBytes, 44);
            header.FirstDirectorySectorLocation = BitConverter.ToUInt32(headerBytes, 48);
            header.TransactionSignatureNumber = BitConverter.ToUInt32(headerBytes, 52);
            header.MiniStreamCutoffSize = BitConverter.ToUInt32(headerBytes, 56);
            header.FirstMiniFATSectorLocation = BitConverter.ToUInt32(headerBytes, 60);
            header.MiniFATSectorsCount = BitConverter.ToUInt32(headerBytes, 64);
            header.FirstDIFATSectorLocation = BitConverter.ToUInt32(headerBytes, 68);
            header.DIFATSectorsCount = BitConverter.ToUInt32(headerBytes, 72);
            header.DIFAT = new uint[(int)HeaderDIFATSectorsCount];
            for (int i = 0; i < HeaderDIFATSectorsCount; i++) 
            {
                header.DIFAT[i] = BitConverter.ToUInt32(headerBytes, 76 + (i * 4));
            }
            return header;
        }
        internal static byte[] ReadFragment(Stream s, int streamOffset, int length)
        {
            byte[] data = new byte[(int)length];
            s.Position = streamOffset;
            int read = s.Read(data, 0, length);
            if (read <= 0) 
                throw new Exception("Unexcpected eof of file");
            return data;
        }
        internal static uint ReadUInt32(Stream s, int streamOffset)
        {
            return BitConverter.ToUInt32(ReadFragment(s, streamOffset, 4), 0);
        }
        internal static uint[] ReadArrayOfUInt32(Stream s, int streamOffset, int count)
        {
            uint[] array = new uint[(int)count];
            byte[] data = ReadFragment(s, streamOffset, 4 * count);
            for (int i = 0; i < count; i++) 
            {
                array[i] = BitConverter.ToUInt32(data, 4 * i);
            }
            return array;
        }
        internal static CFDirectoryEntry ReadDirectoryEntry(Stream s, int streamOffset, bool isVersion3)
        {
            byte[] data = ReadFragment(s, streamOffset, DirectoryEntrySize);
            CFDirectoryEntry entry = new CFDirectoryEntry();
            entry.NameLength = BitConverter.ToUInt16(data, 64);
            int nameLength = Math.Min(32, ((int)entry.NameLength) / 2);
            entry.Name = new char[(int)nameLength];
            for (int i = 0; i < nameLength; i++) 
            {
                entry.Name[i] = (char)BitConverter.ToChar(data, 2 * i);
            }
            entry.ObjectType = data[66];
            entry.ColorFlag = data[67];
            entry.LeftSiblingID = BitConverter.ToUInt32(data, 68);
            entry.RightSiblingID = BitConverter.ToUInt32(data, 72);
            entry.ChildID = BitConverter.ToUInt32(data, 76);
            entry.StateBits = BitConverter.ToUInt32(data, 96);
            entry.StartingSectorLocation = BitConverter.ToUInt32(data, 116);
            entry.StreamSize = BitConverter.ToUInt32(data, 120);
            if (isVersion3) 
                entry.StreamSize &= 0xFFFFFFFF;
            return entry;
        }
        internal static void ValidateHeader(CFHeader header)
        {
            if (header.Signature1 != CFHeader.DefaultSignature1 || header.Signature2 != CFHeader.DefaultSignature2) 
                throw new Exception("Invalid header: signature");
            if (header.CLSID != Guid.Empty) 
            {
            }
            if (header.MinorVersion != 0x3E) 
            {
            }
            if (header.MajorVersion != 3 && header.MajorVersion != 4) 
                throw new Exception("Invalid header: Major version");
            if (header.ByteOrder != 0xFFFE) 
                throw new Exception("Invalid header: Byte order");
            if (header.MajorVersion == 3 && header.SectorShift != 0x0009) 
                throw new Exception("Invalid header: Sector shirt for v3");
            if (header.MajorVersion == 4 && header.SectorShift != 0x000c) 
                throw new Exception("Invalid header: Sector shirt for v4");
            if (header.MiniSectorShift != 0x0006) 
                throw new Exception("Invalid header: Mini sector shirt");
            if (((header.Reserved[0] != 0 || header.Reserved[1] != 0 || header.Reserved[2] != 0) || header.Reserved[3] != 0 || header.Reserved[4] != 0) || header.Reserved[5] != 0) 
                throw new Exception("Invalid header: Reserved");
            if (header.MajorVersion == 3 && header.DirectorySectorsCount != 0) 
                throw new Exception("Invalid header: Directory sectors for v3");
            if (header.MiniStreamCutoffSize != 0x1000) 
                throw new Exception("Invalid header: Mini stream cutoff size");
        }
        internal static void ValidateDirectoryEntry(CFDirectoryEntry entry)
        {
            int len = (int)entry.NameLength;
            if ((entry.Name.Length * 2) != len || len == 0) 
                throw new Exception("Invalid directory entry: name length");
            if (entry.Name[entry.Name.Length - 1] != 0) 
                throw new Exception("Invalid directory entry: name null termination");
            foreach (char ch in entry.Name) 
            {
                if ((ch == '/' || ch == '\\' || ch == ':') || ch == '!') 
                    throw new Exception("Invalid directory entry: illegal char in name");
            }
            if ((entry.ObjectType != DirectoryObjectTypes.Unknown && entry.ObjectType != DirectoryObjectTypes.Storage && entry.ObjectType != DirectoryObjectTypes.Stream) && entry.ObjectType != DirectoryObjectTypes.RootStorage) 
                throw new Exception("Invalid directory entry: object type");
            if (entry.ColorFlag != TreeColors.Red && entry.ColorFlag != TreeColors.Black) 
                throw new Exception("Invalid directory entry: color flag");
            if (entry.LeftSiblingID > DirectoryStreamIds.MAXREGSID && entry.LeftSiblingID != DirectoryStreamIds.NOSTREAM) 
                throw new Exception("Invalid directory entry: left sibling");
            if (entry.RightSiblingID > DirectoryStreamIds.MAXREGSID && entry.RightSiblingID != DirectoryStreamIds.NOSTREAM) 
                throw new Exception("Invalid directory entry: right sibling");
            if (entry.ChildID > DirectoryStreamIds.MAXREGSID && entry.ChildID != DirectoryStreamIds.NOSTREAM) 
                throw new Exception("Invalid directory entry: child");
        }
        static byte[] GetByteArrayPortion(byte[] source, int offset, int length)
        {
            byte[] bytes = new byte[(int)length];
            Array.Copy(source, offset, bytes, 0, length);
            return bytes;
        }
    }
}