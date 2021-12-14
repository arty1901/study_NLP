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

namespace Pullenti.Unitext.Internal.Html
{
    class HtmlSection
    {
        public List<HtmlNode> Nodes = new List<HtmlNode>();
        public HtmlNode Title;
        public int Level;
        public List<HtmlSection> Children = new List<HtmlSection>();
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.AppendFormat("H{0}: ", Level);
            if (Children.Count > 0) 
                tmp.AppendFormat("Childs={0} ", Children.Count);
            if (Nodes.Count > 0) 
                tmp.AppendFormat("Nodes={0} ", Nodes.Count);
            if (Title != null) 
                tmp.Append(Title);
            return tmp.ToString();
        }
        internal void _generate(Pullenti.Unitext.UnitextDocblock blk, int lev, UnitextHtmlGen hg)
        {
            Pullenti.Unitext.Internal.Uni.UnitextGen gen;
            if (Title != null) 
            {
                blk.Head = new Pullenti.Unitext.UnitextContainer();
                gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                hg.GetUniText(Title, gen, null, 0);
                Pullenti.Unitext.UnitextItem hh = gen.Finish(true, null);
                if (hh is Pullenti.Unitext.UnitextContainer) 
                    blk.Head = hh as Pullenti.Unitext.UnitextContainer;
                else if (hh != null) 
                    blk.Head.Children.Add(hh);
                blk.Head.Children.Add(new Pullenti.Unitext.UnitextNewline());
                blk.Head.Typ = Pullenti.Unitext.UnitextContainerType.Head;
            }
            blk.Body = new Pullenti.Unitext.UnitextContainer();
            if (Nodes.Count > 0) 
            {
                gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                foreach (HtmlNode ch in Nodes) 
                {
                    hg.GetUniText(ch, gen, null, 0);
                }
                Pullenti.Unitext.UnitextItem hh = gen.Finish(true, null);
                if (hh != null && Children.Count > 0) 
                {
                    Pullenti.Unitext.UnitextDocblock pr = new Pullenti.Unitext.UnitextDocblock() { Typname = "Preamble" };
                    pr.Body = hh;
                    (blk.Body as Pullenti.Unitext.UnitextContainer).Children.Add(pr);
                }
                else if (hh is Pullenti.Unitext.UnitextContainer) 
                    blk.Body = hh as Pullenti.Unitext.UnitextContainer;
                else if (hh != null) 
                    (blk.Body as Pullenti.Unitext.UnitextContainer).Children.Add(hh);
            }
            foreach (HtmlSection ch in Children) 
            {
                Pullenti.Unitext.UnitextDocblock chblk = new Pullenti.Unitext.UnitextDocblock();
                (blk.Body as Pullenti.Unitext.UnitextContainer).Children.Add(chblk);
                if (lev == 0) 
                    chblk.Typname = "Section";
                else if (lev == 1) 
                    chblk.Typname = "Subsection";
                else 
                    chblk.Typname = "Chapter";
                ch._generate(chblk, lev + 1, hg);
            }
        }
        public static HtmlSectionItem Create(HtmlNode n)
        {
            HtmlSectionItem ttt = new HtmlSectionItem();
            ttt.Stack.Add(new HtmlSection());
            _create(n, ttt);
            if (ttt.Stack.Count > 1) 
                ttt.Stack.RemoveRange(0, ttt.Stack.Count - 1);
            if (ttt.Stack[0].Children.Count == 0) 
                return null;
            if (ttt.Stack[0].Children.Count == 1) 
            {
                if (ttt.Stack[0].Nodes.Count > 0) 
                    ttt.Head.AddRange(ttt.Stack[0].Nodes);
                ttt.Stack[0] = ttt.Stack[0].Children[0];
            }
            if (ttt.Stack[0].Children.Count < 2) 
                return null;
            return ttt;
        }
        static void _create(HtmlNode n, HtmlSectionItem st)
        {
            int curLev = st.Stack.Count;
            foreach (HtmlNode ch in n.Children) 
            {
                if (!string.IsNullOrEmpty(ch.TagName) && ch.TagName[0] == 'H') 
                {
                    int le = 0;
                    if (int.TryParse(ch.TagName.Substring(1), out le)) 
                    {
                        if (le > 0) 
                        {
                            if (le < st.Stack.Count) 
                                st.Stack.RemoveRange(0, st.Stack.Count - le);
                            while (le > st.Stack.Count) 
                            {
                                if (st.Stack[0].Children.Count == 0) 
                                    break;
                                HtmlSection al = st.Stack[0].Children[st.Stack[0].Children.Count - 1];
                                st.Stack.Insert(0, al);
                            }
                            if (le == st.Stack.Count) 
                            {
                                HtmlSection s = new HtmlSection() { Level = le, Title = ch };
                                st.Stack[0].Children.Add(s);
                                st.Stack.Insert(0, s);
                                st.HasBody = true;
                                for (HtmlNode p = n; p != null; p = p.Parent) 
                                {
                                    p.Misc = p;
                                }
                                continue;
                            }
                            else 
                            {
                            }
                        }
                    }
                }
                if (ch.Text != null && ch.Text.StartsWith("Автор:")) 
                {
                }
                _create(ch, st);
                if (ch.Misc != null) 
                    continue;
                if (n.Misc != null) 
                {
                    if (st.Stack.Count > 1) 
                        st.Stack[0].Nodes.Add(ch);
                    else if (st.HasBody) 
                        st.Tail.Add(ch);
                    else 
                        st.Head.Add(ch);
                }
            }
            if (curLev < st.Stack.Count) 
                st.Stack.RemoveRange(0, st.Stack.Count - curLev);
        }
    }
}