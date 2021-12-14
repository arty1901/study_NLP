/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Uni
{
    // Уровень стиля нумерации
    public class UniTextGenNumLevel
    {
        public UniTextGenNumType Type;
        public string Format;
        public int Start = 1;
        public int Current = 0;
        public UniTextGenNumLevel Clone()
        {
            UniTextGenNumLevel res = new UniTextGenNumLevel();
            res.Type = Type;
            res.Format = Format;
            res.Start = Start;
            res.Current = Current;
            return res;
        }
        public override string ToString()
        {
            return string.Format("{0} '{1}' from {2}", Type, Format, Start);
        }
        public string GetValue(int cur)
        {
            if (Type == UniTextGenNumType.Bullet) 
                return Format;
            if (Type == UniTextGenNumType.Decimal) 
                return cur.ToString();
            if (Type == UniTextGenNumType.LowerLetter) 
                return string.Format("{0}", (char)(((((int)'a') + cur) - 1)));
            if (Type == UniTextGenNumType.LowerCyrLetter) 
            {
                if (cur >= 10) 
                    cur++;
                return string.Format("{0}", (char)(((((int)'а') + cur) - 1)));
            }
            if (Type == UniTextGenNumType.UpperCyrLetter) 
            {
                if (cur >= 10) 
                    cur++;
                return string.Format("{0}", (char)(((((int)'А') + cur) - 1)));
            }
            if (Type == UniTextGenNumType.UpperLetter) 
                return string.Format("{0}", (char)(((((int)'A') + cur) - 1)));
            if (Type == UniTextGenNumType.LowerRoman) 
            {
                if (cur > 0 && ((cur - 1) < UnitextHelper.m_Romans.Length)) 
                    return UnitextHelper.m_Romans[cur - 1].ToLower();
            }
            if (Type == UniTextGenNumType.UpperRoman) 
            {
                if (cur > 0 && ((cur - 1) < UnitextHelper.m_Romans.Length)) 
                    return UnitextHelper.m_Romans[cur - 1];
            }
            return cur.ToString();
        }
    }
}