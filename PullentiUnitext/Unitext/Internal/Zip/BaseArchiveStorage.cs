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
    /// An abstract <see cref="IArchiveStorage"/> suitable for extension by inheritance.
    /// </summary>
    abstract class BaseArchiveStorage : IArchiveStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseArchiveStorage"/> class.
        /// </summary>
        /// <param name="updateMode">The update mode.</param>
        protected BaseArchiveStorage(FileUpdateMode updateMode)
        {
            updateMode_ = updateMode;
        }
        /// <summary>
        /// Gets a temporary output <see cref="Stream"/>
        /// </summary>
        /// <return>Returns the temporary output stream.</return>
        public abstract Stream GetTemporaryOutput();
        /// <summary>
        /// Converts the temporary <see cref="Stream"/> to its final form.
        /// </summary>
        /// <return>Returns a <see cref="Stream"/> that can be used to read 
        /// the final storage for the archive.</return>
        public abstract Stream ConvertTemporaryToFinal();
        /// <summary>
        /// Make a temporary copy of a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to make a copy of.</param>
        /// <return>Returns a temporary output <see cref="Stream"/> that is a copy of the input.</return>
        public abstract Stream MakeTemporaryCopy(Stream stream);
        /// <summary>
        /// Return a stream suitable for performing direct updates on the original source.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to open for direct update.</param>
        /// <return>Returns a stream suitable for direct updating.</return>
        public abstract Stream OpenForDirectUpdate(Stream stream);
        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public abstract void Dispose();
        /// <summary>
        /// Gets the update mode applicable.
        /// </summary>
        public FileUpdateMode UpdateMode
        {
            get
            {
                return updateMode_;
            }
        }
        FileUpdateMode updateMode_;
    }
}