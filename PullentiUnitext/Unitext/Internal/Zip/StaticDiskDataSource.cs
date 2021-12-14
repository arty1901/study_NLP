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
    /// Default implementation of a <see cref="IStaticDataSource"/> for use with files stored on disk.
    /// </summary>
    class StaticDiskDataSource : IStaticDataSource
    {
        /// <summary>
        /// Initialise a new instnace of <see cref="StaticDiskDataSource"/>
        /// </summary>
        /// <param name="fileName">The name of the file to obtain data from.</param>
        public StaticDiskDataSource(string fileName)
        {
            fileName_ = fileName;
        }
        /// <summary>
        /// Get a <see cref="Stream"/> providing data.
        /// </summary>
        /// <return>Returns a <see cref="Stream"/> provising data.</return>
        public Stream GetSource()
        {
            return File.Open(fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        string fileName_;
    }
}