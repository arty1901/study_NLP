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
    // Basic implementation of <see cref="IEntryFactory"></see>
    class ZipEntryFactory : IEntryFactory
    {
        /// <summary>
        /// Defines the possible values to be used for the <see cref="ZipEntry.DateTime"/>.
        /// </summary>
        internal enum TimeSetting : int
        {
            /// <summary>
            /// Use the recorded LastWriteTime value for the file.
            /// </summary>
            LastWriteTime,
            /// <summary>
            /// Use the recorded LastWriteTimeUtc value for the file
            /// </summary>
            LastWriteTimeUtc,
            /// <summary>
            /// Use the recorded CreateTime value for the file.
            /// </summary>
            CreateTime,
            /// <summary>
            /// Use the recorded CreateTimeUtc value for the file.
            /// </summary>
            CreateTimeUtc,
            /// <summary>
            /// Use the recorded LastAccessTime value for the file.
            /// </summary>
            LastAccessTime,
            /// <summary>
            /// Use the recorded LastAccessTimeUtc value for the file.
            /// </summary>
            LastAccessTimeUtc,
            /// <summary>
            /// Use a fixed value.
            /// </summary>
            Fixed,
        }

        public ZipEntryFactory(TimeSetting timeSetting = TimeSetting.LastWriteTime)
        {
            timeSetting_ = timeSetting;
            nameTransform_ = new ZipNameTransform();
        }
        public INameTransform NameTransform
        {
            get
            {
                return nameTransform_;
            }
            set
            {
                if (value == null) 
                    nameTransform_ = new ZipNameTransform();
                else 
                    nameTransform_ = value;
            }
        }
        public TimeSetting Setting
        {
            get
            {
                return timeSetting_;
            }
            set
            {
                timeSetting_ = value;
            }
        }
        public DateTime FixedDateTime
        {
            get
            {
                return fixedDateTime_;
            }
            set
            {
                if (value.Year < 1970) 
                    throw new ArgumentException("Value is too old to be valid", "value");
                fixedDateTime_ = value;
            }
        }
        public int GetAttributes
        {
            get
            {
                return getAttributes_;
            }
            set
            {
                getAttributes_ = value;
            }
        }
        public int SetAttributes
        {
            get
            {
                return setAttributes_;
            }
            set
            {
                setAttributes_ = value;
            }
        }
        public bool IsUnicodeText
        {
            get
            {
                return isUnicodeText_;
            }
            set
            {
                isUnicodeText_ = value;
            }
        }
        public ZipEntry MakeFileEntry(string fileName)
        {
            return this.MakeFileEntryEx(fileName, true);
        }
        public ZipEntry MakeFileEntryEx(string fileName, bool useFileSystem)
        {
            ZipEntry result = new ZipEntry(nameTransform_.TransformFile(fileName));
            result.IsUnicodeText = isUnicodeText_;
            int externalAttributes = 0;
            bool useAttributes = setAttributes_ != 0;
            FileInfo fi = null;
            if (useFileSystem) 
                fi = new FileInfo(fileName);
            if ((fi != null) && fi.Exists) 
            {
                result.Size = (int)fi.Length;
                useAttributes = true;
            }
            else if (timeSetting_ == TimeSetting.Fixed) 
                result.DateTime = fixedDateTime_;
            if (useAttributes) 
            {
                externalAttributes |= setAttributes_;
                result.ExternalFileAttributes = externalAttributes;
            }
            return result;
        }
        public ZipEntry MakeDirectoryEntry(string directoryName)
        {
            return this.MakeDirectoryEntryEx(directoryName, true);
        }
        public ZipEntry MakeDirectoryEntryEx(string directoryName, bool useFileSystem)
        {
            ZipEntry result = new ZipEntry(nameTransform_.TransformDirectory(directoryName));
            result.IsUnicodeText = isUnicodeText_;
            result.Size = 0;
            int externalAttributes = 0;
            DirectoryInfo di = null;
            if (useFileSystem) 
                di = new DirectoryInfo(directoryName);
            externalAttributes |= ((setAttributes_ | 16));
            result.ExternalFileAttributes = externalAttributes;
            return result;
        }
        INameTransform nameTransform_;
        DateTime fixedDateTime_ = DateTime.Now;
        TimeSetting timeSetting_;
        bool isUnicodeText_ = true;
        int getAttributes_ = -1;
        int setAttributes_;
    }
}