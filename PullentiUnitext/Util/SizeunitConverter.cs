/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Util
{
    /// <summary>
    /// Преобразователь единиц измерения размеров (Html, Css). 
    /// Поддерживаются: pt, cm, mm, in, px
    /// </summary>
    public class SizeunitConverter
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="numWithUnit">строковое представление значения и единицы измерения</param>
        public SizeunitConverter(string numWithUnit = null)
        {
            if (!string.IsNullOrEmpty(numWithUnit) && numWithUnit.Length > 2 && char.IsLetter(numWithUnit[numWithUnit.Length - 1])) 
            {
                Unit = numWithUnit.Substring(numWithUnit.Length - 2);
                double d;
                if (MiscHelper.TryParseDouble(numWithUnit.Substring(0, numWithUnit.Length - 2).Trim(), out d)) 
                    Val = d;
            }
            else 
            {
                double d;
                if (MiscHelper.TryParseDouble(numWithUnit, out d)) 
                    Val = d;
            }
        }
        /// <summary>
        /// Значение
        /// </summary>
        public double Val;
        /// <summary>
        /// Единица измерения (pt, cm, mm, in, px)
        /// </summary>
        public string Unit;
        public override string ToString()
        {
            return string.Format("{0}{1}", MiscHelper.OutDouble(Val), Unit ?? "?");
        }
        /// <summary>
        /// Преобразовать в новую единицу измерения
        /// </summary>
        /// <param name="unit">новая единица (pt, cm, mm, in, px)</param>
        /// <return>результат или null</return>
        public SizeunitConverter ConvertTo(string unit)
        {
            double pt = (double)0;
            if (Unit == "pt" || string.IsNullOrEmpty(Unit)) 
                pt = Val;
            else if (Unit == "cm") 
                pt = (Val * 2.54) / 72;
            else if (Unit == "mm") 
                pt = (Val * 0.254) / 72;
            else if (Unit == "in") 
                pt = Val / 72;
            else if (Unit == "px") 
                pt = (Val * 96) / 72;
            else 
                return null;
            double val = (double)0;
            if (unit == "pt") 
                val = pt;
            else if (unit == "cm") 
                val = (pt * 72) / 2.54;
            else if (unit == "mm") 
                val = (pt * 72) / 0.254;
            else if (unit == "in") 
                val = pt * 72;
            else if (unit == "px") 
                val = (pt * 72) / 96;
            else if (string.IsNullOrEmpty(unit) && string.IsNullOrEmpty(Unit)) 
                val = pt;
            else if (pt == 0) 
                val = 0;
            else 
                return null;
            SizeunitConverter res = new SizeunitConverter();
            res.Unit = unit;
            res.Val = Math.Round(val, 2);
            return res;
        }
        /// <summary>
        /// Преобразовать строку с числом и единицей измерения в другую единицу измерения
        /// </summary>
        /// <param name="numWithUnit">число с единицей измерения</param>
        /// <param name="newUnit">новая единица изменения (pt, cm, mm, in, px)</param>
        /// <return>результат или null</return>
        public static double? Convert(string numWithUnit, string newUnit)
        {
            SizeunitConverter cnv = new SizeunitConverter(numWithUnit);
            SizeunitConverter res = cnv.ConvertTo(newUnit);
            if (res == null) 
                return null;
            return res.Val;
        }
    }
}