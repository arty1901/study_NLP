/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // Represents a string from a <see cref="ZipFile"/> which is stored as an array of bytes.
    class ZipString
    {
        public ZipString(string comment)
        {
            comment_ = comment;
            isSourceString_ = true;
        }
        public bool IsSourceString
        {
            get
            {
                return isSourceString_;
            }
        }
        public int RawLength
        {
            get
            {
                this.MakeBytesAvailable();
                return rawComment_.Length;
            }
        }
        public byte[] RawComment
        {
            get
            {
                this.MakeBytesAvailable();
                return rawComment_;
            }
        }
        public void Reset()
        {
            if (isSourceString_) 
                rawComment_ = null;
            else 
                comment_ = null;
        }
        void MakeTextAvailable()
        {
            if (comment_ == null) 
                comment_ = ZipConstants.ConvertToString0(rawComment_);
        }
        void MakeBytesAvailable()
        {
            if (rawComment_ == null) 
                rawComment_ = ZipConstants.ConvertToArrayStr(comment_);
        }
        public static implicit operator string(ZipString zipString)
        {
            zipString.MakeTextAvailable();
            return zipString.comment_;
        }
        string comment_;
        byte[] rawComment_;
        bool isSourceString_;
    }
}