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
    /// Represents a source of data that can dynamically provide 
    /// multiple <see cref="Stream">data sources</see> based on the parameters passed.
    /// </summary>
    interface IDynamicDataSource
    {
        /// <summary>
        /// Get a data source.
        /// </summary>
        /// <param name="entry">The <see cref="ZipEntry"/> to get a source for.</param>
        /// <param name="name">The name for data if known.</param>
        /// <return>Returns a <see cref="Stream"/> to use for compression input.</return>
        Stream GetSource(ZipEntry entry, string name);
    }
}