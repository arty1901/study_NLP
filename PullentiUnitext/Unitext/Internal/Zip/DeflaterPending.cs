/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */


namespace Pullenti.Unitext.Internal.Zip
{
    // This class stores the pending output of the Deflater.
    class DeflaterPending : PendingBuffer
    {
        public DeflaterPending() : base(DeflaterConstants.PENDING_BUF_SIZE)
        {
        }
    }
}