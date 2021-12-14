/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Unitext.Internal.Uni
{
    static class StyleHelper
    {
        public static void ProcessDoc(Pullenti.Unitext.UnitextDocument doc)
        {
            ProcessContent(doc.Content);
            foreach (Pullenti.Unitext.UnitextPagesection s in doc.Sections) 
            {
                ProcessContent(s.Header);
                ProcessContent(s.Footer);
            }
        }
        static void ProcessContent(Pullenti.Unitext.UnitextItem cnt)
        {
            if (cnt == null || cnt.m_StyledFrag == null) 
                return;
            StringBuilder defTxt = new StringBuilder();
            int cp = 0;
            cnt.SetDefaultTextPos(ref cp, defTxt);
            List<Pullenti.Unitext.UnitextItem> its = new List<Pullenti.Unitext.UnitextItem>();
            cnt.GetAllItems(its, 0);
            for (int i = 0; i < (its.Count - 1); i++) 
            {
                if ((its[i] is Pullenti.Unitext.UnitextPlaintext) && (its[i] as Pullenti.Unitext.UnitextPlaintext).Typ == Pullenti.Unitext.UnitextPlaintextType.Generated) 
                {
                    if (its[i].Parent == its[i + 1].Parent && its[i].m_StyledFrag == null) 
                        its[i].m_StyledFrag = its[i + 1].m_StyledFrag;
                }
            }
            foreach (Pullenti.Unitext.UnitextItem it in its) 
            {
                if (it.m_StyledFrag != null) 
                {
                    Pullenti.Unitext.UnitextStyledFragment fr = it.m_StyledFrag;
                    if (fr.BeginChar < 0) 
                        fr.BeginChar = it.BeginChar;
                    if (it.EndChar > fr.EndChar) 
                        fr.EndChar = it.EndChar;
                }
            }
            _corrCp(cnt.m_StyledFrag);
            _setText(cnt.m_StyledFrag, defTxt.ToString());
            cnt.m_StyledFrag = _optimize(cnt.m_StyledFrag);
            foreach (Pullenti.Unitext.UnitextItem it in its) 
            {
                if (it.m_StyledFrag != null) 
                {
                    if (it.m_StyledFrag.Tag != null) 
                        it.m_StyledFrag = null;
                }
            }
        }
        static void _corrCp(Pullenti.Unitext.UnitextStyledFragment fr)
        {
            fr.Tag = null;
            for (int i = 0; i < fr.Children.Count; i++) 
            {
                Pullenti.Unitext.UnitextStyledFragment ch = fr.Children[i];
                ch.Parent = fr;
                _corrCp(ch);
                if (fr.BeginChar < 0) 
                    fr.BeginChar = ch.BeginChar;
                if (ch.EndChar > fr.EndChar) 
                    fr.EndChar = ch.EndChar;
            }
        }
        static void _setText(Pullenti.Unitext.UnitextStyledFragment fr, string txt)
        {
            bool outText = true;
            foreach (Pullenti.Unitext.UnitextStyledFragment ch in fr.Children) 
            {
                _setText(ch, txt);
                if (ch.Typ != Pullenti.Unitext.UnitextStyledFragmentType.Inline && ch.Typ != Pullenti.Unitext.UnitextStyledFragmentType.Undefined) 
                    outText = false;
            }
            if (outText && fr.EndChar >= 0 && (fr.EndChar < txt.Length)) 
                fr.Text = txt.Substring(fr.BeginChar, (fr.EndChar + 1) - fr.BeginChar);
        }
        static Pullenti.Unitext.UnitextStyledFragment _optimize(Pullenti.Unitext.UnitextStyledFragment fr)
        {
            if (fr.Parent != null && fr.Style == fr.Parent.Style) 
                fr.Style = null;
            for (int i = 0; i < fr.Children.Count; i++) 
            {
                Pullenti.Unitext.UnitextStyledFragment ch = fr.Children[i];
                ch = _optimize(ch);
                if (ch == null) 
                {
                    fr.Children.RemoveAt(i);
                    i--;
                    continue;
                }
                fr.Children[i] = ch;
                if (i > 0) 
                {
                    Pullenti.Unitext.UnitextStyledFragment ch0 = fr.Children[i - 1];
                    if (ch0.Style == ch.Style && ch0.Typ == ch.Typ && (ch0.EndChar + 1) == ch.BeginChar) 
                    {
                        ch0.Children.AddRange(ch.Children);
                        foreach (Pullenti.Unitext.UnitextStyledFragment ccc in ch.Children) 
                        {
                            ccc.Parent = ch0;
                        }
                        ch0.EndChar = ch.EndChar;
                        fr.Children.RemoveAt(i);
                        i--;
                        ch.Tag = ch0;
                        continue;
                    }
                }
            }
            if (fr.Children.Count == 0 && (fr.BeginChar < 0)) 
                return null;
            return fr;
        }
    }
}