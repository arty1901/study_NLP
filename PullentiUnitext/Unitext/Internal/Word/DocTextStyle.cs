/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Word
{
    class DocTextStyle
    {
        public string Name;
        public string Aliases;
        public bool IsHeading
        {
            get
            {
                if (Name != null) 
                {
                    if (Name.StartsWith("head", StringComparison.OrdinalIgnoreCase)) 
                        return true;
                }
                if (Aliases != null) 
                {
                    if (Aliases.StartsWith("head", StringComparison.OrdinalIgnoreCase)) 
                        return true;
                    if (Aliases.Contains("аголовок")) 
                        return true;
                }
                return false;
            }
        }
        public int CalcHeadingLevel()
        {
            if (!IsHeading) 
                return 0;
            if (Aliases != null) 
            {
                for (int i = Aliases.Length - 1; i >= 0; i--) 
                {
                    if (!char.IsDigit(Aliases[i])) 
                    {
                        if ((i + 1) < Aliases.Length) 
                            return int.Parse(Aliases.Substring(i + 1));
                        break;
                    }
                }
            }
            if (Name != null) 
            {
                for (int i = Name.Length - 1; i >= 0; i--) 
                {
                    if (!char.IsDigit(Name[i])) 
                    {
                        if ((i + 1) < Name.Length) 
                            return int.Parse(Name.Substring(i + 1));
                        break;
                    }
                }
            }
            return 0;
        }
        public Pullenti.Unitext.UnitextStyle UStyle;
        public string NumId;
        public int NumLvl;
        public bool IsDefault;
        public override string ToString()
        {
            return Name;
        }
    }
}