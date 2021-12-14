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
    // Стиль нумерации
    public class UnitextGenNumStyle : IUnitextGenNumStyle
    {
        public string Id
        {
            get;
            set;
        }
        public List<UniTextGenNumLevel> Levels = new List<UniTextGenNumLevel>();
        public int Lvl
        {
            get;
            set;
        }
        public bool IsBullet
        {
            get;
            set;
        }
        public string Txt
        {
            get;
            set;
        }
        public override string ToString()
        {
            if (Txt != null) 
                return string.Format("Level: {0} Val: {1}", Lvl, Txt);
            return string.Format("Id: {0} Levels: {1} FirstLevel: {2}", Id, Levels.Count, (Levels.Count == 0 ? "" : Levels[0].ToString()));
        }
        List<int> nums = new List<int>();
        public UniTextGenNumLevel GetLevel(int lev)
        {
            if ((lev < 0) || lev >= Levels.Count) 
                return null;
            return Levels[lev];
        }
        public string Process(int lev)
        {
            if (Txt != null) 
                return Txt;
            if (lev >= Levels.Count) 
                return null;
            if (lev >= nums.Count) 
            {
                if (nums.Count == 0) 
                {
                    for (int ii = 0; ii <= lev; ii++) 
                    {
                        if (ii < Levels.Count) 
                            nums.Add(Levels[ii].Start);
                        else 
                            nums.Add(1);
                    }
                }
                else 
                    while (lev >= nums.Count) 
                    {
                        if (lev < Levels.Count) 
                            nums.Add(Levels[lev].Start);
                        else 
                            nums.Add(1);
                    }
            }
            else if (lev == (nums.Count - 1)) 
            {
                nums[lev]++;
                if (nums[lev] < Levels[lev].Start) 
                    nums[lev] = Levels[lev].Start;
            }
            else 
            {
                if ((lev + 1) < nums.Count) 
                {
                    nums.RemoveRange(lev + 1, nums.Count - lev - 1);
                    for (int k = lev + 1; k < Levels.Count; k++) 
                    {
                        Levels[k].Start = 1;
                    }
                }
                nums[lev]++;
            }
            string val = Levels[lev].Format ?? "";
            for (int ii = 0; ii <= lev; ii++) 
            {
                if (val.Contains("%" + ((ii + 1)))) 
                {
                    string nn = Levels[ii].GetValue(nums[ii]);
                    if (nn != null) 
                        val = val.Replace("%" + ((ii + 1)), nn);
                }
            }
            return val;
        }
    }
}