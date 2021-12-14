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
    /// Defines facilities for data storage when updating Zip Archives.
    /// </summary>
    interface IArchiveStorage : IDisposable
    {
        /// <summary>
        /// Get the <see cref="FileUpdateMode"/> to apply during updates.
        /// </summary>
        FileUpdateMode UpdateMode { get; }
        /// <summary>
        /// Get an empty <see cref="Stream"/> that can be used for temporary output.
        /// </summary>
        /// <return>Returns a temporary output <see cref="Stream"/></return>
        Stream GetTemporaryOutput();
        /// <summary>
        /// Convert a temporary output stream to a final stream.
        /// </summary>
        /// <return>The resulting final <see cref="Stream"/></return>
        Stream ConvertTemporaryToFinal();
        /// <summary>
        /// Make a temporary copy of the original stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to copy.</param>
        /// <return>Returns a temporary output <see cref="Stream"/> that is a copy of the input.</return>
        Stream MakeTemporaryCopy(Stream stream);
        /// <summary>
        /// Return a stream suitable for performing direct updates on the original source.
        /// </summary>
        /// <param name="stream">The current stream.</param>
        /// <return>Returns a stream suitable for direct updating.</return>
        Stream OpenForDirectUpdate(Stream stream);
    }
}