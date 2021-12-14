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
    /// An <see cref="IArchiveStorage"/> implementation suitable for in memory streams.
    /// </summary>
    class MemoryArchiveStorage : BaseArchiveStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryArchiveStorage"/> class.
        /// </summary>
        /// <param name="updateMode">The <see cref="FileUpdateMode"/> to use</param>
        public MemoryArchiveStorage(FileUpdateMode updateMode = FileUpdateMode.Direct) : base(updateMode)
        {
        }
        /// <summary>
        /// Get the stream returned by <see cref="ConvertTemporaryToFinal"/> if this was in fact called.
        /// </summary>
        public MemoryStream FinalStream
        {
            get
            {
                return finalStream_;
            }
        }
        /// <summary>
        /// Gets the temporary output <see cref="Stream"/>
        /// </summary>
        /// <return>Returns the temporary output stream.</return>
        public override Stream GetTemporaryOutput()
        {
            temporaryStream_ = new MemoryStream();
            return temporaryStream_;
        }
        /// <summary>
        /// Converts the temporary <see cref="Stream"/> to its final form.
        /// </summary>
        /// <return>Returns a <see cref="Stream"/> that can be used to read 
        /// the final storage for the archive.</return>
        public override Stream ConvertTemporaryToFinal()
        {
            if (temporaryStream_ == null) 
                throw new Exception("No temporary stream has been created");
            finalStream_ = new MemoryStream(temporaryStream_.ToArray());
            return finalStream_;
        }
        /// <summary>
        /// Make a temporary copy of the original stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to copy.</param>
        /// <return>Returns a temporary output <see cref="Stream"/> that is a copy of the input.</return>
        public override Stream MakeTemporaryCopy(Stream stream)
        {
            temporaryStream_ = new MemoryStream();
            stream.Position = 0;
            StreamUtils.Copy(stream, temporaryStream_, new byte[(int)4096]);
            return temporaryStream_;
        }
        /// <summary>
        /// Return a stream suitable for performing direct updates on the original source.
        /// </summary>
        /// <param name="stream">The original source stream</param>
        /// <return>Returns a stream suitable for direct updating.</return>
        public override Stream OpenForDirectUpdate(Stream stream)
        {
            if ((stream == null) || !stream.CanWrite) 
            {
                MemoryStream result = new MemoryStream();
                if (stream != null) 
                {
                    stream.Position = 0;
                    StreamUtils.Copy(stream, result, new byte[(int)4096]);
                    stream.Dispose();
                }
                return result;
            }
            else 
                return stream;
        }
        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public override void Dispose()
        {
            if (temporaryStream_ != null) 
            {
                temporaryStream_.Dispose();
                temporaryStream_ = null;
            }
        }
        MemoryStream temporaryStream_;
        MemoryStream finalStream_;
    }
}