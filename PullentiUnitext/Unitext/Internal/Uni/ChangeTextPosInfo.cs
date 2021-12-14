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
    // для корректировки beginchar и endchar у стилевых фрагментов
    class ChangeTextPosInfo
    {
        Pullenti.Unitext.UnitextItem[] bMap = null;
        Pullenti.Unitext.UnitextItem[] eMap = null;
        Pullenti.Unitext.UnitextStyledFragment m_Root;
        public ChangeTextPosInfo(Pullenti.Unitext.UnitextStyledFragment fr, Pullenti.Unitext.UnitextItem root)
        {
            if (fr.EndChar == 0) 
                return;
            bMap = new Pullenti.Unitext.UnitextItem[(int)(fr.EndChar + 1)];
            eMap = new Pullenti.Unitext.UnitextItem[(int)(fr.EndChar + 1)];
            List<Pullenti.Unitext.UnitextItem> its = new List<Pullenti.Unitext.UnitextItem>();
            root.GetAllItems(its, 0);
            foreach (Pullenti.Unitext.UnitextItem it in its) 
            {
                if (it.EndChar > 0 && (it.EndChar < bMap.Length)) 
                {
                    bMap[it.BeginChar] = it;
                    eMap[it.EndChar] = it;
                }
            }
            m_Root = fr;
        }
        int[] bPos;
        int[] ePos;
        public void Restore(int newLen)
        {
            if (bMap == null) 
                return;
            bPos = new int[(int)bMap.Length];
            ePos = new int[(int)bMap.Length];
            int p = 0;
            for (int i = 0; i < bMap.Length; i++) 
            {
                Pullenti.Unitext.UnitextItem it = bMap[i];
                if (it != null) 
                {
                    if (it.BeginChar < p) 
                    {
                    }
                    p = it.BeginChar;
                }
                bPos[i] = p;
                p++;
            }
            p = newLen - 1;
            for (int i = eMap.Length - 1; i >= 0; i--) 
            {
                Pullenti.Unitext.UnitextItem it = eMap[i];
                if (it != null) 
                {
                    if (p > 0 && it.EndChar > p) 
                    {
                    }
                    p = it.EndChar;
                }
                ePos[i] = p;
                p--;
            }
            this._restore(m_Root);
        }
        void _restore(Pullenti.Unitext.UnitextStyledFragment fr)
        {
            if (fr.BeginChar >= 0) 
            {
                if (bPos[fr.BeginChar] < 0) 
                {
                }
                fr.BeginChar = bPos[fr.BeginChar];
            }
            if (fr.EndChar >= 0) 
            {
                if (ePos[fr.EndChar] < 0) 
                {
                }
                fr.EndChar = ePos[fr.EndChar];
            }
            foreach (Pullenti.Unitext.UnitextStyledFragment ch in fr.Children) 
            {
                this._restore(ch);
            }
        }
    }
}