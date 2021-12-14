/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Misc
{
    class MyZipEntry
    {
        public MyZipFile Zip;
        public MyZipEntry(MyZipFile zip)
        {
            Zip = zip;
        }
        public string Name;
        public byte[] Data;
        public int UncompressDataSize;
        public int Pos;
        public int CompressDataSize;
        public bool Encrypted;
        public bool IsDirectory;
        public int Method;
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, UncompressDataSize);
        }
        public byte[] GetData()
        {
            if (Data != null) 
                return Data;
            return Zip.Unzip(this);
        }
    }
}