/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    /// <summary>
    /// Status returned returned by <see cref="ZipTestResultHandler"/> during testing.
    /// </summary>
    class TestStatus
    {
        /// <summary>
        /// Initialise a new instance of <see cref="TestStatus"/>
        /// </summary>
        /// <param name="file">The <see cref="ZipFile"/> this status applies to.</param>
        public TestStatus(ZipFile file)
        {
            m_File = file;
        }
        /// <summary>
        /// Get the current <see cref="TestOperation"/> in progress.
        /// </summary>
        public TestOperation Operation
        {
            get
            {
                return m_Operation;
            }
        }
        /// <summary>
        /// Get the <see cref="ZipFile"/> this status is applicable to.
        /// </summary>
        public ZipFile File
        {
            get
            {
                return m_File;
            }
        }
        /// <summary>
        /// Get the current/last entry tested.
        /// </summary>
        public ZipEntry Entry
        {
            get
            {
                return m_Entry;
            }
        }
        /// <summary>
        /// Get the number of errors detected so far.
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return m_ErrorCount;
            }
        }
        /// <summary>
        /// Get the number of bytes tested so far for the current entry.
        /// </summary>
        public int BytesTested
        {
            get
            {
                return m_BytesTested;
            }
        }
        /// <summary>
        /// Get a value indicating wether the last entry test was valid.
        /// </summary>
        public bool EntryValid
        {
            get
            {
                return m_EntryValid;
            }
        }
        internal void AddError()
        {
            m_ErrorCount++;
            m_EntryValid = false;
        }
        internal void SetOperation(TestOperation operation)
        {
            m_Operation = operation;
        }
        internal void SetEntry(ZipEntry entry)
        {
            m_Entry = entry;
            m_EntryValid = true;
            m_BytesTested = 0;
        }
        internal void SetBytesTested(int value)
        {
            m_BytesTested = value;
        }
        ZipFile m_File;
        ZipEntry m_Entry;
        bool m_EntryValid;
        int m_ErrorCount;
        int m_BytesTested;
        TestOperation m_Operation;
    }
}