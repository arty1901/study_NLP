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
    /// An <see cref="IArchiveStorage"/> implementation suitable for hard disks.
    /// </summary>
    class DiskArchiveStorage : BaseArchiveStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiskArchiveStorage"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="updateMode">The update mode.</param>
        public DiskArchiveStorage(ZipFile file, FileUpdateMode updateMode = FileUpdateMode.Safe) : base(updateMode)
        {
            if (file.Name == null) 
                throw new Exception("Cant handle non file archives");
            fileName_ = file.Name;
        }
        /// <summary>
        /// Gets a temporary output <see cref="Stream"/> for performing updates on.
        /// </summary>
        /// <return>Returns the temporary output stream.</return>
        public override Stream GetTemporaryOutput()
        {
            if (temporaryName_ != null) 
            {
                temporaryName_ = GetTempFileName(temporaryName_, true);
                temporaryStream_ = File.Open(temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            }
            else 
            {
                temporaryName_ = "temp.bin";
                temporaryStream_ = File.Open(temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            }
            return temporaryStream_;
        }
        /// <summary>
        /// Converts a temporary <see cref="Stream"/> to its final form.
        /// </summary>
        /// <return>Returns a <see cref="Stream"/> that can be used to read 
        /// the final storage for the archive.</return>
        public override Stream ConvertTemporaryToFinal()
        {
            if (temporaryStream_ == null) 
                throw new Exception("No temporary stream has been created");
            Stream result = null;
            string moveTempName = GetTempFileName(fileName_, false);
            bool newFileCreated = false;
            try 
            {
                temporaryStream_.Dispose();
                File.Move(fileName_, moveTempName);
                File.Move(temporaryName_, fileName_);
                newFileCreated = true;
                File.Delete(moveTempName);
                result = File.Open(fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch(Exception ex307) 
            {
                result = null;
                if (!newFileCreated) 
                {
                    File.Move(moveTempName, fileName_);
                    File.Delete(temporaryName_);
                }
                throw ex307;
            }
            return result;
        }
        /// <summary>
        /// Make a temporary copy of a stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to copy.</param>
        /// <return>Returns a temporary output <see cref="Stream"/> that is a copy of the input.</return>
        public override Stream MakeTemporaryCopy(Stream stream)
        {
            stream.Dispose();
            temporaryName_ = GetTempFileName(fileName_, true);
            File.Copy(fileName_, temporaryName_, true);
            temporaryStream_ = new FileStream(temporaryName_, FileMode.Open, FileAccess.ReadWrite);
            return temporaryStream_;
        }
        /// <summary>
        /// Return a stream suitable for performing direct updates on the original source.
        /// </summary>
        /// <param name="stream">The current stream.</param>
        /// <return>Returns a stream suitable for direct updating.</return>
        public override Stream OpenForDirectUpdate(Stream stream)
        {
            Stream result;
            if ((stream == null) || !stream.CanWrite) 
            {
                if (stream != null) 
                    stream.Dispose();
                return new FileStream(fileName_, FileMode.Open, FileAccess.ReadWrite);
            }
            else 
                result = stream;
            return result;
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
        static string GetTempFileName(string original, bool makeTempFile)
        {
            string result = null;
            if (original == null) 
                result = "temp.bin";
            else 
            {
                int counter = 0;
                int suffixSeed = DateTime.Now.Second;
                while (result == null) 
                {
                    counter += 1;
                    string newName = string.Format("{0}.{1}{2}.tmp", original, suffixSeed, counter);
                    if (!File.Exists(newName)) 
                    {
                        if (makeTempFile) 
                        {
                            try 
                            {
                                using (FileStream stream = File.Create(newName)) 
                                {
                                }
                                result = newName;
                            }
                            catch(Exception ex308) 
                            {
                                suffixSeed = DateTime.Now.Second;
                            }
                        }
                        else 
                            result = newName;
                    }
                }
            }
            return result;
        }
        Stream temporaryStream_;
        string fileName_;
        string temporaryName_;
    }
}