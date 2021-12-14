/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfStringValue : PdfObject
    {
        public bool IsHex = false;
        public byte[] Val;
        public override string ToString(int lev)
        {
            string str = GetStringByBytes(Val);
            if (str.Length > 100) 
                str = str.Substring(0, 100) + "...";
            return string.Format("\"{0}\"", str);
        }
        public override bool IsSimple(int lev)
        {
            return true;
        }
        public static string GetStringByBytes(byte[] buf)
        {
            if (buf == null) 
                return null;
            if (buf.Length > 2 && buf[0] == 0xFF && buf[1] == 0xFE) 
                return Pullenti.Util.MiscHelper.DecodeStringUnicode(buf, 2, buf.Length - 2);
            if (buf.Length > 2 && buf[0] == 0xFE && buf[1] == 0xFF) 
                return Pullenti.Util.MiscHelper.DecodeStringUnicodeBE(buf, 2, buf.Length - 2);
            return Pullenti.Util.MiscHelper.DecodeStringAscii(buf, 0, -1);
        }
    }
}