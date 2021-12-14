/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Uni
{
    public class UnitextGenNumStyleEx : IUnitextGenNumStyle
    {
        public UnitextGenNumStyle Src;
        public Dictionary<int, int> OverrideStarts = new Dictionary<int, int>();
        bool m_Overs = false;
        public string Id
        {
            get;
            set;
        }
        public string Txt
        {
            get
            {
                return Src.Txt;
            }
        }
        public int Lvl
        {
            get
            {
                return Src.Lvl;
            }
        }
        public bool IsBullet
        {
            get
            {
                return Src.IsBullet;
            }
        }
        public override string ToString()
        {
            string res = string.Format("Id={0} => {1}", Id, Src.ToString());
            foreach (KeyValuePair<int, int> kp in OverrideStarts) 
            {
                res = string.Format("{0} Strrt[{1}]={2}", res, kp.Key, kp.Value);
            }
            return res;
        }
        void _prep()
        {
            if (!m_Overs) 
            {
                foreach (KeyValuePair<int, int> kp in OverrideStarts) 
                {
                    if (kp.Key >= 0 && (kp.Key < Src.Levels.Count)) 
                        Src.Levels[kp.Key].Start = kp.Value;
                }
                m_Overs = true;
            }
        }
        public UniTextGenNumLevel GetLevel(int lev)
        {
            this._prep();
            return Src.GetLevel(lev);
        }
        public string Process(int lev)
        {
            this._prep();
            return Src.Process(lev);
        }
    }
}