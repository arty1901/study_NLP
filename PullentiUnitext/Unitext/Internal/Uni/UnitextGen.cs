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
    public class UnitextGen
    {
        StringBuilder m_Text = new StringBuilder();
        Pullenti.Unitext.UnitextContainer m_Res = new Pullenti.Unitext.UnitextContainer();
        Pullenti.Unitext.UnitextStyledFragment m_CurStyle;
        string m_CurSection = null;
        bool m_NewRes = false;
        public void ClearAll()
        {
            m_Text.Length = 0;
            m_Stack.Clear();
            m_CurStyle = null;
            if (m_NewRes) 
                m_Res = new Pullenti.Unitext.UnitextContainer();
            else 
                m_Res.Children.Clear();
        }
        public void SetStyle(Pullenti.Unitext.UnitextStyledFragment st)
        {
            if (st == null) 
                return;
            if (m_CurStyle == null) 
            {
                m_CurStyle = st;
                m_Res.m_StyledFrag = st;
            }
            else if (m_CurStyle != st) 
            {
                this.FlushText();
                m_CurStyle = st;
            }
        }
        public Pullenti.Unitext.UnitextItem Finish(bool isContent, Pullenti.Unitext.CreateDocumentParam pars = null)
        {
            this.FlushText();
            Pullenti.Unitext.UnitextItem res = (Pullenti.Unitext.UnitextItem)m_Res;
            if (m_CurStyle != null) 
            {
                if (m_Res.Children.Count == 0) 
                    return null;
            }
            else 
                res = m_Res.Optimize(isContent, pars);
            m_NewRes = res == m_Res;
            return res;
        }
        List<Pullenti.Unitext.UnitextItem> m_Stack = new List<Pullenti.Unitext.UnitextItem>();
        public char LastNotSpaceChar = (char)0;
        public char LastChar = (char)0;
        public string LastText
        {
            get
            {
                return m_Text.ToString();
            }
        }
        void FlushText()
        {
            if (m_Text.Length > 0) 
            {
                Pullenti.Unitext.UnitextPlaintext t = new Pullenti.Unitext.UnitextPlaintext() { Text = m_Text.ToString() };
                t.m_StyledFrag = m_CurStyle;
                m_Text.Length = 0;
                this.Append(t, null, -1, false);
            }
        }
        public void AppendNewline(bool ifNotPrevNewline = false)
        {
            if (ifNotPrevNewline) 
            {
                if (m_Res.Children.Count > 0 && m_Text.Length == 0) 
                {
                    if (m_Res.Children[m_Res.Children.Count - 1] is Pullenti.Unitext.UnitextNewline) 
                        return;
                }
            }
            this.Append(new Pullenti.Unitext.UnitextNewline() { PageSectionId = m_CurSection }, null, -1, false);
            LastChar = '\r';
        }
        public void AppendPagebreak()
        {
            this.Append(new Pullenti.Unitext.UnitextPagebreak() { PageSectionId = m_CurSection }, null, -1, false);
            LastChar = '\f';
        }
        public void AppendPagesection(Pullenti.Unitext.UnitextPagesection s)
        {
            if (s != null && s.Id == m_CurSection) 
                return;
            if (m_Res.Children.Count > 0) 
            {
                if (m_Res.Children.Count == 1 && (m_Res.Children[0] is Pullenti.Unitext.UnitextNewline)) 
                {
                }
                else if (!(m_Res.Children[m_Res.Children.Count - 1] is Pullenti.Unitext.UnitextPagebreak)) 
                    this.AppendPagebreak();
            }
            if (s != null) 
                m_CurSection = s.Id;
        }
        public void Append(Pullenti.Unitext.UnitextItem blk, IUnitextGenNumStyle numStyl = null, int numLevel = -1, bool doNumStyleAsText = false)
        {
            this.FlushText();
            if (blk != null && m_CurStyle != null && blk.m_StyledFrag == null) 
                blk.m_StyledFrag = m_CurStyle;
            if (blk != null && blk.PageSectionId == null) 
                blk.PageSectionId = m_CurSection;
            if (numStyl != null && numStyl.Txt != null) 
            {
                Pullenti.Unitext.UnitextList list0 = this.AppendListItem(blk, numStyl.Txt, null, numStyl.Lvl, numStyl);
                return;
            }
            if (numStyl == null || (numLevel < 0)) 
            {
                if (blk != null) 
                {
                    if ((blk is Pullenti.Unitext.UnitextFootnote) && m_Res.Children.Count > 0) 
                    {
                        Pullenti.Unitext.UnitextPlaintext txt = m_Res.Children[m_Res.Children.Count - 1] as Pullenti.Unitext.UnitextPlaintext;
                        if (txt != null && txt.Typ == Pullenti.Unitext.UnitextPlaintextType.Sup) 
                        {
                            if (txt.Text == (blk as Pullenti.Unitext.UnitextFootnote).CustomMark) 
                                m_Res.Children.RemoveAt(m_Res.Children.Count - 1);
                        }
                    }
                    if (numStyl != null) 
                        this.AppendNewline(true);
                    m_Res.Children.Add(blk);
                    if (numStyl != null) 
                        this.AppendNewline(false);
                }
                return;
            }
            string num = numStyl.Process(numLevel);
            if (num == null) 
            {
                if (blk != null) 
                    m_Res.Children.Add(blk);
                return;
            }
            if (doNumStyleAsText) 
            {
                this.AppendNewline(true);
                m_Res.Children.Add(new Pullenti.Unitext.UnitextPlaintext() { Text = num + " ", Typ = Pullenti.Unitext.UnitextPlaintextType.Generated });
                if (blk != null) 
                    m_Res.Children.Add(blk);
                this.AppendNewline(false);
                return;
            }
            UniTextGenNumLevel numlev = numStyl.GetLevel(numLevel);
            Pullenti.Unitext.UnitextList list = this.AppendListItem(blk, num, numStyl.Id, numLevel, null);
            if (list != null && numlev.Type == UniTextGenNumType.Bullet) 
                list.UnorderPrefix = numlev.Format ?? "";
        }
        public Pullenti.Unitext.UnitextList AppendListItem(Pullenti.Unitext.UnitextItem content, string pref, string numStyleId, int numLevel, IUnitextGenNumStyle numStyl = null)
        {
            this.FlushText();
            Pullenti.Unitext.UnitextList list = null;
            for (int i = m_Res.Children.Count - 1; i >= 0; i--) 
            {
                if (m_Res.Children[i] is Pullenti.Unitext.UnitextNewline) 
                {
                }
                else 
                {
                    list = m_Res.Children[i] as Pullenti.Unitext.UnitextList;
                    break;
                }
            }
            if (list != null && (((list.Tag as string) == numStyleId || numStyleId == null))) 
            {
                if (numStyleId == null && numStyl == null) 
                {
                    numStyleId = list.Tag as string;
                    numLevel = -1;
                }
                while (true) 
                {
                    if (list.Level == numLevel) 
                    {
                        Pullenti.Unitext.UnitextListitem it0 = new Pullenti.Unitext.UnitextListitem() { Content = content };
                        if (m_CurStyle != null) 
                            it0.m_StyledFrag = m_CurStyle;
                        if (pref != null) 
                            it0.Prefix = new Pullenti.Unitext.UnitextPlaintext() { Text = pref, Typ = Pullenti.Unitext.UnitextPlaintextType.Generated };
                        list.Items.Add(it0);
                        return list;
                    }
                    if ((numLevel < 0) || (list.Level < numLevel)) 
                    {
                        if (list.Items.Count == 0) 
                            break;
                        Pullenti.Unitext.UnitextListitem last = list.Items[list.Items.Count - 1];
                        if (last.Sublist != null) 
                        {
                            list = last.Sublist;
                            continue;
                        }
                        if (numLevel < 0) 
                        {
                            numLevel = list.Level;
                            continue;
                        }
                        if (list.Level < (numLevel - 1)) 
                            break;
                        last.Sublist = new Pullenti.Unitext.UnitextList() { Level = numLevel, Tag = numStyleId };
                        if (m_CurStyle != null) 
                            last.Sublist.m_StyledFrag = m_CurStyle;
                        list = last.Sublist;
                        continue;
                    }
                    break;
                }
            }
            if (numLevel < 0) 
                numLevel = 0;
            list = new Pullenti.Unitext.UnitextList() { Level = numLevel, Tag = numStyleId, PageSectionId = m_CurSection };
            if (m_CurStyle != null) 
                list.m_StyledFrag = m_CurStyle;
            if (numStyl != null && numStyl.IsBullet) 
                list.UnorderPrefix = numStyl.Txt;
            m_Res.Children.Add(list);
            m_Res.Children.Add(new Pullenti.Unitext.UnitextNewline());
            Pullenti.Unitext.UnitextListitem it = new Pullenti.Unitext.UnitextListitem() { Content = content };
            if (m_CurStyle != null) 
                it.m_StyledFrag = m_CurStyle;
            if (pref != null && list.UnorderPrefix == null) 
                it.Prefix = new Pullenti.Unitext.UnitextPlaintext() { Text = pref, Typ = Pullenti.Unitext.UnitextPlaintextType.Generated };
            list.Items.Add(it);
            return list;
        }
        public void AppendText(string txt, bool checkTable = false)
        {
            int inTableLev = 0;
            for (int i = 0; i < txt.Length; i++) 
            {
                char ch = txt[i];
                LastChar = ch;
                if (ch == 0xD || ch == 0xA) 
                {
                    if (ch == 0xD && ((i + 1) < txt.Length) && txt[i + 1] == 0xA) 
                        i++;
                    this.AppendNewline(false);
                    continue;
                }
                if (ch == '\f' && inTableLev == 0) 
                {
                    this.AppendPagebreak();
                    continue;
                }
                if (ch == 0x1E && checkTable) 
                    inTableLev++;
                if (ch == 0x1F && checkTable) 
                    inTableLev--;
                if (ch == 0xAD) 
                    continue;
                if ((ch != 9 && ch != 7 && ch != '\f') && (((int)ch) < 0x1E)) 
                    ch = ' ';
                m_Text.Append(ch);
                if (!char.IsWhiteSpace(ch)) 
                    LastNotSpaceChar = ch;
            }
        }
        internal static string ConvertToPt(string val, string detTyp = null)
        {
            int vv = ConvertToMM(val, detTyp);
            if (vv == 0) 
                return val;
            else 
                return string.Format("{0}pt", vv);
        }
        internal static int ConvertToMM(string val, string detTyp = null)
        {
            if (val == null) 
                return 0;
            float f = (float)0;
            int i;
            string typ = null;
            for (i = 0; i < val.Length; i++) 
            {
                if (char.IsLetter(val[i]) || val[i] == ';') 
                {
                    typ = val.Substring(i).ToLower();
                    val = val.Substring(0, i);
                    break;
                }
            }
            if (string.IsNullOrEmpty(typ)) 
                typ = detTyp;
            if (string.IsNullOrEmpty(typ)) 
                return (int)f;
            if (Pullenti.Util.MiscHelper.TryParseFloat(val, out f)) 
            {
            }
            else 
                return 0;
            if (typ.StartsWith("pt")) 
            {
            }
            else if (typ.StartsWith("pc")) 
                f = (f * 72) / 6;
            else if (typ.StartsWith("cm")) 
                f = (f * 72) / 2.54F;
            else if (typ.StartsWith("mm")) 
                f = (f * 72) / 25.4F;
            else if (typ.StartsWith("px")) 
                f = f * 0.75F;
            else if (typ.StartsWith("in")) 
                f = f * 72F;
            else 
                return 0;
            return (int)f;
        }
    }
}