/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Pullenti.Unitext.Internal.Word
{
    class DocSection
    {
        public Pullenti.Unitext.UnitextPagesection USect = new Pullenti.Unitext.UnitextPagesection();
        public bool Loaded = false;
        public List<string> HeadIds = new List<string>();
        public List<string> FootIds = new List<string>();
        static double _readDoubleCm(string str)
        {
            double d;
            if (!Pullenti.Util.MiscHelper.TryParseDouble(str, out d)) 
                return 0;
            d /= 20;
            d *= 2.54;
            d /= 72;
            return Math.Round(d, 2);
        }
        static string _readAttrVal(XmlNode x, string nam, string nam2 = null)
        {
            if (x.Attributes != null) 
            {
                foreach (XmlAttribute a in x.Attributes) 
                {
                    if (a.LocalName == nam) 
                        return a.Value;
                    else if (nam2 != null && a.LocalName == nam2) 
                        return a.Value;
                }
            }
            return null;
        }
        public void Load(XmlNode xml)
        {
            Loaded = true;
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "pgSz") 
                {
                    USect.Width = _readDoubleCm(_readAttrVal(x, "w", null));
                    USect.Height = _readDoubleCm(_readAttrVal(x, "h", null));
                }
                else if (x.LocalName == "pgMar") 
                {
                    USect.Left = _readDoubleCm(_readAttrVal(x, "left", null));
                    USect.Right = _readDoubleCm(_readAttrVal(x, "right", null));
                    USect.Top = _readDoubleCm(_readAttrVal(x, "top", null));
                    USect.Bottom = _readDoubleCm(_readAttrVal(x, "bottom", null));
                    USect.HeaderHeight = _readDoubleCm(_readAttrVal(x, "header", null));
                    USect.FooterHeight = _readDoubleCm(_readAttrVal(x, "footer", null));
                }
                else if (x.LocalName == "headerReference") 
                    HeadIds.Add(_readAttrVal(x, "id", null));
                else if (x.LocalName == "footerReference") 
                    FootIds.Add(_readAttrVal(x, "id", null));
            }
        }
    }
}