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
    class DocNumStyles
    {
        Dictionary<string, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyleEx> m_NumStyles = new Dictionary<string, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyleEx>();
        public Pullenti.Unitext.Internal.Uni.UnitextGenNumStyleEx GetStyle(string id)
        {
            if (id == null) 
                return null;
            if (m_NumStyles.ContainsKey(id)) 
                return m_NumStyles[id];
            return null;
        }
        public void ReadAllStyles(XmlNode node)
        {
            Dictionary<string, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle> abstr = new Dictionary<string, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle>();
            foreach (XmlNode xnum in node.ChildNodes) 
            {
                if (xnum.LocalName == "abstractNum" && xnum.Attributes != null && xnum.Attributes.Count >= 1) 
                {
                    string id = null;
                    foreach (XmlAttribute a in xnum.Attributes) 
                    {
                        if (a.LocalName == "abstractNumId") 
                        {
                            id = a.Value;
                            break;
                        }
                    }
                    if (id == null || abstr.ContainsKey(id)) 
                        continue;
                    if (id == "14") 
                    {
                    }
                    Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle nsty = new Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle() { Id = id };
                    abstr.Add(id, nsty);
                    foreach (XmlNode xx in xnum.ChildNodes) 
                    {
                        if (xx.LocalName == "lvl") 
                        {
                            Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel nlev = new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel();
                            nsty.Levels.Add(nlev);
                            foreach (XmlNode xxx in xx.ChildNodes) 
                            {
                                if (xxx.Attributes != null) 
                                {
                                    if (xxx.LocalName == "numFmt" && xxx.Attributes.Count == 1) 
                                        nlev.Type = GetNumTyp(xxx.Attributes[0].Value);
                                    else if (xxx.LocalName == "lvlText" && xxx.Attributes.Count == 1) 
                                        nlev.Format = xxx.Attributes[0].Value;
                                    else if (xxx.LocalName == "start" && xxx.Attributes.Count == 1) 
                                    {
                                        int ii;
                                        if (int.TryParse(xxx.Attributes[0].Value, out ii)) 
                                            nlev.Start = ii;
                                    }
                                }
                            }
                        }
                        else if (xx.LocalName == "numStyleLink" && xx.Attributes != null && xx.Attributes.Count > 0) 
                        {
                            string lid = xx.Attributes[0].Value;
                            if (abstr.ContainsKey(lid)) 
                                nsty.Levels.AddRange(abstr[lid].Levels);
                        }
                    }
                }
                else if (xnum.LocalName == "num" && xnum.Attributes != null && xnum.Attributes.Count == 1) 
                {
                    string id = xnum.Attributes[0].Value;
                    string intId = null;
                    foreach (XmlNode xx in xnum.ChildNodes) 
                    {
                        if (xx.LocalName == "abstractNumId" && xx.Attributes != null && xx.Attributes.Count == 1) 
                            intId = xx.Attributes[0].Value;
                    }
                    if (intId == null) 
                        continue;
                    Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle num0;
                    if (!abstr.TryGetValue(intId, out num0)) 
                        continue;
                    Pullenti.Unitext.Internal.Uni.UnitextGenNumStyleEx num = new Pullenti.Unitext.Internal.Uni.UnitextGenNumStyleEx() { Src = num0 };
                    num.Id = id;
                    if (!m_NumStyles.ContainsKey(id)) 
                        m_NumStyles.Add(id, num);
                    foreach (XmlNode xx in xnum.ChildNodes) 
                    {
                        if (xx.LocalName == "lvlOverride" && xx.Attributes != null && xx.Attributes.Count == 1) 
                        {
                            int l;
                            if (!int.TryParse(xx.Attributes[0].Value ?? "", out l)) 
                                continue;
                            if ((l < 0) || l >= num.Src.Levels.Count) 
                                continue;
                            foreach (XmlNode xxx in xx.ChildNodes) 
                            {
                                if (xxx.LocalName == "startOverride" && xxx.Attributes != null) 
                                {
                                    foreach (XmlAttribute a in xxx.Attributes) 
                                    {
                                        if (a.LocalName == "val") 
                                        {
                                            int s;
                                            if (int.TryParse(a.Value, out s)) 
                                            {
                                                if (!num.OverrideStarts.ContainsKey(l)) 
                                                    num.OverrideStarts.Add(l, s);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        internal static Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle ReadNumberStyle(XmlNode xml)
        {
            Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle res = null;
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "ilvl") 
                {
                    res = new Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle();
                    try 
                    {
                        if (x.Attributes != null && x.Attributes.Count == 1) 
                            res.Lvl = int.Parse(x.Attributes[0].Value);
                    }
                    catch(Exception ee) 
                    {
                    }
                }
                else if (x.LocalName == "ilfo") 
                {
                    if (x.Attributes != null && x.Attributes.Count == 1) 
                    {
                        if (x.Attributes[0].Value == "2") 
                            res.IsBullet = true;
                    }
                }
                else if (x.LocalName == "t" && res != null) 
                {
                    if (x.Attributes != null && x.Attributes.Count == 1) 
                        res.Txt = x.Attributes[0].Value;
                }
            }
            return res;
        }
        internal static Pullenti.Unitext.Internal.Uni.UniTextGenNumType GetNumTyp(string ty)
        {
            if (ty == "bullet") 
                return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Bullet;
            if (ty.Contains("decimal")) 
                return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal;
            if (ty == "lowerLetter") 
                return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.LowerLetter;
            if (ty == "russianLower") 
                return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.LowerCyrLetter;
            if (ty == "russianUpper") 
                return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.UpperCyrLetter;
            if (ty == "upperLetter") 
                return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.UpperLetter;
            if (ty == "lowerRoman") 
                return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.LowerRoman;
            if (ty.Contains("oman")) 
                return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.UpperRoman;
            return Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal;
        }
    }
}