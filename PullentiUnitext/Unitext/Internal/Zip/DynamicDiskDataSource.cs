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
    /// <summary>
    /// Default implementation of <see cref="IDynamicDataSource"/> for files stored on disk.
    /// </summary>
    class DynamicDiskDataSource : IDynamicDataSource
    {
        /// <summary>
        /// Initialise a default instance of <see cref="DynamicDiskDataSource"/>.
        /// </summary>
        public DynamicDiskDataSource()
        {
        }
        /// <summary>
        /// Get a <see cref="Stream"/> providing data for an entry.
        /// </summary>
        /// <param name="entry">The entry to provide data for.</param>
        /// <param name="name">The file name for data if known.</param>
        /// <return>Returns a stream providing data; or null if not available</return>
        public Stream GetSource(ZipEntry entry, string name)
        {
            Stream result = null;
            if (name != null) 
                result = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read);
            return result;
        }
    }
}