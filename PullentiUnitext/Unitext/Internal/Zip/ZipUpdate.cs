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
    class ZipUpdate
    {
        public ZipUpdate(ZipEntry entry, UpdateCommand command = UpdateCommand.Add, IStaticDataSource dataSource = null, string fileName = null, Stream str = null)
        {
            dataSource_ = dataSource;
            filename_ = fileName;
            command_ = command;
            _stream = str;
            entry_ = (ZipEntry)entry.Clone();
        }
        public ZipEntry Entry
        {
            get
            {
                return entry_;
            }
        }
        public ZipEntry OutEntry
        {
            get
            {
                if (outEntry_ == null) 
                    outEntry_ = (ZipEntry)entry_.Clone();
                return outEntry_;
            }
        }
        public UpdateCommand Command
        {
            get
            {
                return command_;
            }
        }
        public string Filename
        {
            get
            {
                return filename_;
            }
        }
        public int SizePatchOffset
        {
            get
            {
                return sizePatchOffset_;
            }
            set
            {
                sizePatchOffset_ = value;
            }
        }
        public int CrcPatchOffset
        {
            get
            {
                return crcPatchOffset_;
            }
            set
            {
                crcPatchOffset_ = value;
            }
        }
        public int OffsetBasedSize
        {
            get
            {
                return _offsetBasedSize;
            }
            set
            {
                _offsetBasedSize = value;
            }
        }
        public Stream GetSource()
        {
            if (_stream != null) 
                return _stream;
            Stream result = null;
            if (dataSource_ != null) 
                result = dataSource_.GetSource();
            return result;
        }
        ZipEntry entry_;
        ZipEntry outEntry_;
        UpdateCommand command_;
        IStaticDataSource dataSource_;
        Stream _stream;
        string filename_;
        int sizePatchOffset_ = -1;
        int crcPatchOffset_ = -1;
        int _offsetBasedSize = -1;
    }
}