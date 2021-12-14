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
    /// The operation in progress reported by a <see cref="ZipTestResultHandler"/> during testing.
    /// </summary>
    enum TestOperation : int
    {
        /// <summary>
        /// Setting up testing.
        /// </summary>
        Initialising,
        /// <summary>
        /// Testing an individual entries header
        /// </summary>
        EntryHeader,
        /// <summary>
        /// Testing an individual entries data
        /// </summary>
        EntryData,
        /// <summary>
        /// Testing an individual entry has completed.
        /// </summary>
        EntryComplete,
        /// <summary>
        /// Running miscellaneous tests
        /// </summary>
        MiscellaneousTests,
        /// <summary>
        /// Testing is complete
        /// </summary>
        Complete,
    }
}