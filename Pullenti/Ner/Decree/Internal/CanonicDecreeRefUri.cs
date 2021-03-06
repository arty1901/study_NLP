/*
 * SDK Pullenti Lingvo, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Decree.Internal
{
    public class CanonicDecreeRefUri
    {
        public CanonicDecreeRefUri(string txt)
        {
            Text = txt;
        }
        public Pullenti.Ner.Referent Ref;
        public int BeginChar;
        public int EndChar;
        public bool IsDiap = false;
        public bool IsAdopted = false;
        public string TypeWithGeo;
        public string Text;
        public override string ToString()
        {
            return (Text == null ? "?" : Text.Substring(BeginChar, (EndChar + 1) - BeginChar));
        }
    }
}