/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System.IO;

namespace Pullenti.Unitext.Internal.Zip
{
    /// <summary>
    /// Provides a static way to obtain a source of data for an entry.
    /// </summary>
    interface IStaticDataSource
    {
        /// <summary>
        /// Get a source of data by creating a new stream.
        /// </summary>
        /// <return>Returns a <see cref="Stream"/> to use for compression input.</return>
        Stream GetSource();
    }
}