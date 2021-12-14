/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Uni
{
    static class UnitextImplantateHelper
    {
        public static Pullenti.Unitext.UnitextItem Clear(Pullenti.Unitext.UnitextItem content, string id, Type typeName, Pullenti.Unitext.UnitextContainerType cntTyp, Pullenti.Unitext.UnitextContainer own, ref int cou)
        {
            Pullenti.Unitext.UnitextTable tab = content as Pullenti.Unitext.UnitextTable;
            if (tab != null) 
            {
                for (int r = 0; r < tab.RowsCount; r++) 
                {
                    for (int c = 0; c < tab.ColsCount; c++) 
                    {
                        Pullenti.Unitext.UnitextTablecell cel = tab.GetCell(r, c);
                        if (cel != null && cel.Content != null) 
                            cel.Content = Clear(cel.Content, id, typeName, cntTyp, null, ref cou);
                    }
                }
                return tab;
            }
            Pullenti.Unitext.UnitextList li = content as Pullenti.Unitext.UnitextList;
            if (li != null) 
            {
                foreach (Pullenti.Unitext.UnitextListitem l in li.Items) 
                {
                    if (l.Prefix != null) 
                        l.Prefix = Clear(l.Prefix, id, typeName, cntTyp, null, ref cou);
                    if (l.Content != null) 
                        l.Content = Clear(l.Content, id, typeName, cntTyp, null, ref cou);
                    if (l.Sublist != null) 
                        Clear(l.Sublist, id, typeName, cntTyp, null, ref cou);
                }
                return li;
            }
            Pullenti.Unitext.UnitextFootnote foot = content as Pullenti.Unitext.UnitextFootnote;
            if (foot != null) 
            {
                if (foot.Content != null) 
                    foot.Content = Clear(foot.Content, id, typeName, cntTyp, null, ref cou);
                return foot;
            }
            Pullenti.Unitext.UnitextHyperlink hyp = content as Pullenti.Unitext.UnitextHyperlink;
            if (hyp != null) 
            {
                if (hyp.Content != null) 
                    hyp.Content = Clear(hyp.Content, id, typeName, cntTyp, null, ref cou);
                return hyp;
            }
            Pullenti.Unitext.UnitextDocblock dbl = content as Pullenti.Unitext.UnitextDocblock;
            if (dbl != null) 
            {
                if (dbl.Head != null) 
                    dbl.Head = Clear(dbl.Head, id, typeName, cntTyp, null, ref cou) as Pullenti.Unitext.UnitextContainer;
                if (dbl.Body != null) 
                    dbl.Body = Clear(dbl.Body, id, typeName, cntTyp, null, ref cou);
                if (dbl.Tail != null) 
                    dbl.Tail = Clear(dbl.Tail, id, typeName, cntTyp, null, ref cou) as Pullenti.Unitext.UnitextContainer;
                if (dbl.Appendix != null) 
                    dbl.Appendix = Clear(dbl.Appendix, id, typeName, cntTyp, null, ref cou) as Pullenti.Unitext.UnitextContainer;
                return dbl;
            }
            Pullenti.Unitext.UnitextContainer cnt = content as Pullenti.Unitext.UnitextContainer;
            if (cnt == null) 
                return content;
            for (int i = 0; i < cnt.Children.Count; i++) 
            {
                Pullenti.Unitext.UnitextItem cc = Clear(cnt.Children[i], id, typeName, cntTyp, cnt, ref cou);
                if (cc == null) 
                {
                }
                cnt.Children[i] = cc;
            }
            if (typeName != null) 
            {
                if (cnt.GetType() != typeName) 
                    return content;
                if (cntTyp != Pullenti.Unitext.UnitextContainerType.Undefined) 
                {
                    if (cnt.Typ != cntTyp) 
                        return content;
                }
            }
            else if (id != null) 
            {
                if (cnt.Id != id) 
                    return content;
            }
            if (cnt.Children.Count == 0) 
                return null;
            if (cnt.Children.Count == 1) 
            {
                cnt.Children[0].Parent = cnt.Parent;
                return cnt.Children[0];
            }
            if (own == null) 
                return content;
            int ii = own.GetChildIndexOf(cnt);
            if (ii < 0) 
                return content;
            own.Children.RemoveAt(ii);
            own.Children.InsertRange(ii, cnt.Children);
            foreach (Pullenti.Unitext.UnitextItem ch in own.Children) 
            {
                ch.Parent = own;
            }
            cou++;
            return own.Children[ii];
        }
        static Pullenti.Unitext.UnitextItem _splitTexts(Pullenti.Unitext.UnitextItem content, int begin, int end, Pullenti.Unitext.UnitextContainer own = null)
        {
            if (content == null) 
                return null;
            if (end < content.BeginChar) 
                return content;
            if (begin > content.EndChar) 
                return content;
            if (begin <= content.BeginChar && end >= content.EndChar) 
                return content;
            Pullenti.Unitext.UnitextTable tab = content as Pullenti.Unitext.UnitextTable;
            if (tab != null) 
            {
                Pullenti.Unitext.UnitextTablecell cel = tab._findCellByPos(begin);
                if (cel != null && cel.Content != null) 
                    cel.Content = _splitTexts(cel.Content, begin, end, null);
                return tab;
            }
            Pullenti.Unitext.UnitextList li = content as Pullenti.Unitext.UnitextList;
            if (li != null) 
            {
                foreach (Pullenti.Unitext.UnitextListitem l in li.Items) 
                {
                    if (l.Prefix != null) 
                        l.Prefix = _splitTexts(l.Prefix, begin, end, null);
                    if (l.Content != null) 
                        l.Content = _splitTexts(l.Content, begin, end, null);
                    if (l.Sublist != null) 
                        _splitTexts(l.Sublist, begin, end, null);
                }
                return li;
            }
            Pullenti.Unitext.UnitextFootnote foot = content as Pullenti.Unitext.UnitextFootnote;
            if (foot != null) 
            {
                if (foot.Content != null) 
                    foot.Content = _splitTexts(foot.Content, begin, end, null);
                return foot;
            }
            Pullenti.Unitext.UnitextHyperlink hyp = content as Pullenti.Unitext.UnitextHyperlink;
            if (hyp != null) 
            {
                if (hyp.Content != null) 
                    hyp.Content = _splitTexts(hyp.Content, begin, end, null);
                return hyp;
            }
            Pullenti.Unitext.UnitextDocblock dbl = content as Pullenti.Unitext.UnitextDocblock;
            if (dbl != null) 
            {
                if (dbl.Head != null) 
                {
                    Pullenti.Unitext.UnitextContainer hh = _splitTexts(dbl.Head, begin, end, null) as Pullenti.Unitext.UnitextContainer;
                    if (hh.Children.Contains(dbl)) 
                    {
                    }
                    dbl.Head = hh;
                }
                if (dbl.Body != null) 
                    dbl.Body = _splitTexts(dbl.Body, begin, end, null);
                if (dbl.Tail != null) 
                    dbl.Tail = _splitTexts(dbl.Tail, begin, end, null) as Pullenti.Unitext.UnitextContainer;
                if (dbl.Appendix != null) 
                    dbl.Appendix = _splitTexts(dbl.Appendix, begin, end, null);
                return dbl;
            }
            Pullenti.Unitext.UnitextContainer cnt = content as Pullenti.Unitext.UnitextContainer;
            if (cnt != null) 
            {
                for (int i = 0; i < cnt.Children.Count; i++) 
                {
                    Pullenti.Unitext.UnitextItem ch = cnt.Children[i];
                    if (ch.EndChar < begin) 
                        continue;
                    if (ch.BeginChar > end) 
                    {
                        if (end == (begin - 1) && ch.BeginChar <= begin) 
                        {
                        }
                        else 
                            break;
                    }
                    Pullenti.Unitext.UnitextItem cc = _splitTexts(ch, begin, end, cnt);
                    if (cc == null) 
                    {
                    }
                    cnt.Children[i] = cc;
                }
                return content;
            }
            Pullenti.Unitext.UnitextPlaintext txt = content as Pullenti.Unitext.UnitextPlaintext;
            if (txt != null) 
            {
                if (begin == txt.BeginChar && end == txt.EndChar) 
                    return txt;
                bool creat = false;
                if (own == null) 
                {
                    own = new Pullenti.Unitext.UnitextContainer() { Parent = txt.Parent, BeginChar = txt.BeginChar, EndChar = txt.EndChar, Id = string.Format("cnt0_{0}_{1}", txt.BeginChar, txt.EndChar) };
                    own.Children.Add(txt);
                    creat = true;
                }
                int ii = own.GetChildIndexOf(content);
                if (ii < 0) 
                    return content;
                int jj = ii;
                own.Children.RemoveAt(ii);
                if (begin > txt.BeginChar) 
                {
                    Pullenti.Unitext.UnitextPlaintext b = txt.RemoveStart(begin - txt.BeginChar);
                    if (b != null) 
                    {
                        own.Children.Insert(jj, b);
                        jj++;
                    }
                }
                if (end < txt.EndChar) 
                {
                    Pullenti.Unitext.UnitextPlaintext e = txt.RemoveEnd(txt.EndChar - end);
                    if (e != null) 
                        own.Children.Insert(jj, e);
                }
                own.Children.Insert(jj, txt);
                txt.Parent = own;
                if (creat) 
                    return own;
                else 
                    return own.Children[ii];
            }
            return content;
        }
        public static Pullenti.Unitext.UnitextItem Implantate(Pullenti.Unitext.UnitextItem content, Pullenti.Unitext.UnitextContainer impl, string text, Pullenti.Unitext.UnitextContainer own = null)
        {
            if (content == null || content == impl) 
                return content;
            if (impl.EndChar < content.BeginChar) 
                return content;
            if (impl.BeginChar > content.EndChar) 
                return content;
            if ((impl.BeginChar == content.BeginChar && impl.EndChar == content.EndChar && own == null) && !(content is Pullenti.Unitext.UnitextContainer)) 
            {
                impl.Parent = content.Parent;
                impl.Children.Add(content);
                content.Parent = impl;
                return impl;
            }
            content = _splitTexts(content, impl.BeginChar, impl.EndChar, own);
            if (content == null) 
                return null;
            if (content == impl) 
                return content;
            if (impl.BeginChar <= content.GetPlaintextNsPos(text) && content.GetPlaintextNsPos1(text) <= impl.EndChar) 
            {
                if ((content is Pullenti.Unitext.UnitextDocblock) && string.Compare((content as Pullenti.Unitext.UnitextDocblock).Typname ?? "", "Footnote", true) == 0) 
                {
                    Pullenti.Unitext.UnitextDocblock db = content as Pullenti.Unitext.UnitextDocblock;
                    if (db.Body != null) 
                    {
                        db.Body = Implantate(db.Body, impl, text, null);
                        return db.Body;
                    }
                }
                if (((impl.Typ == Pullenti.Unitext.UnitextContainerType.Number || impl.Typ == Pullenti.Unitext.UnitextContainerType.Name)) && (content is Pullenti.Unitext.UnitextContainer) && (content as Pullenti.Unitext.UnitextContainer).Typ == Pullenti.Unitext.UnitextContainerType.Head) 
                {
                    foreach (Pullenti.Unitext.UnitextItem ch in (content as Pullenti.Unitext.UnitextContainer).Children) 
                    {
                        ch.Parent = impl;
                        impl.Children.Add(ch);
                    }
                    impl.Parent = content.Parent;
                }
                else 
                {
                    impl.Children.Add(content);
                    impl.Parent = content.Parent;
                    content.Parent = impl;
                }
                if (impl.EndChar > content.EndChar || (impl.BeginChar < content.BeginChar)) 
                {
                    impl.BeginChar = content.BeginChar;
                    impl.EndChar = content.EndChar;
                }
                if (own != null) 
                {
                    int ii = own.GetChildIndexOf(content);
                    if (ii >= 0) 
                    {
                        own.Children[ii] = impl;
                        impl.Parent = own;
                    }
                    else 
                        return content;
                }
                return impl;
            }
            Pullenti.Unitext.UnitextTable tab = content as Pullenti.Unitext.UnitextTable;
            if (tab != null) 
            {
                Pullenti.Unitext.UnitextTablecell cel = tab._findCellByPos(impl.BeginChar);
                if (cel != null && cel.Content != null) 
                    cel.Content = Implantate(cel.Content, impl, text, null);
                return tab;
            }
            Pullenti.Unitext.UnitextList li = content as Pullenti.Unitext.UnitextList;
            if (li != null) 
            {
                foreach (Pullenti.Unitext.UnitextListitem l in li.Items) 
                {
                    if (l.BeginChar <= impl.BeginChar && impl.BeginChar <= l.EndChar) 
                    {
                        if (l.Prefix != null) 
                            l.Prefix = Implantate(l.Prefix, impl, text, null);
                        if (l.Content != null) 
                            l.Content = Implantate(l.Content, impl, text, null);
                        if (l.Sublist != null) 
                            Implantate(l.Sublist, impl, text, null);
                    }
                }
                return li;
            }
            Pullenti.Unitext.UnitextFootnote foot = content as Pullenti.Unitext.UnitextFootnote;
            if (foot != null) 
            {
                if (foot.Content != null) 
                    foot.Content = Implantate(foot.Content, impl, text, null);
                return foot;
            }
            Pullenti.Unitext.UnitextHyperlink hyp = content as Pullenti.Unitext.UnitextHyperlink;
            if (hyp != null) 
            {
                if (hyp.Content != null) 
                    hyp.Content = Implantate(hyp.Content, impl, text, null);
                return hyp;
            }
            Pullenti.Unitext.UnitextDocblock dbl = content as Pullenti.Unitext.UnitextDocblock;
            if (dbl != null) 
            {
                if (dbl.Head != null) 
                {
                    Pullenti.Unitext.UnitextContainer hh = Implantate(dbl.Head, impl, text, null) as Pullenti.Unitext.UnitextContainer;
                    if (hh.Children.Contains(dbl)) 
                    {
                    }
                    dbl.Head = hh;
                }
                if (dbl.Body != null) 
                    dbl.Body = Implantate(dbl.Body, impl, text, null);
                if (dbl.Tail != null) 
                    dbl.Tail = Implantate(dbl.Tail, impl, text, null) as Pullenti.Unitext.UnitextContainer;
                if (dbl.Appendix != null) 
                    dbl.Appendix = Implantate(dbl.Appendix, impl, text, null);
                return dbl;
            }
            Pullenti.Unitext.UnitextContainer cnt = content as Pullenti.Unitext.UnitextContainer;
            if (cnt != null) 
            {
                for (int i = 0; i < cnt.Children.Count; i++) 
                {
                    if (i == 49) 
                    {
                    }
                    Pullenti.Unitext.UnitextItem ch = cnt.Children[i];
                    if (impl.EndChar < ch.BeginChar) 
                    {
                        if (impl.EndChar == (impl.BeginChar - 1) && impl.BeginChar == ch.BeginChar) 
                        {
                        }
                        else 
                            continue;
                    }
                    if (impl.BeginChar > ch.EndChar) 
                        continue;
                    if (((ch is Pullenti.Unitext.UnitextFootnote) || (ch is Pullenti.Unitext.UnitextTable) || (ch is Pullenti.Unitext.UnitextList)) || (ch is Pullenti.Unitext.UnitextHyperlink) || (ch is Pullenti.Unitext.UnitextDocblock)) 
                    {
                        Implantate(ch, impl, text, cnt);
                        if (impl.Parent != null) 
                            break;
                        continue;
                    }
                    if (ch is Pullenti.Unitext.UnitextComment) 
                        continue;
                    if (ch.BeginChar > ch.EndChar) 
                        continue;
                    int i1 = impl.GetPlaintextNsPos(text);
                    int i2 = ch.GetPlaintextNsPos(text);
                    int e1 = impl.GetPlaintextNsPos1(text);
                    int e2 = ch.GetPlaintextNsPos1(text);
                    if (i1 > i2 || ((i1 == i2 && (e1 < e2)))) 
                    {
                        if (ch is Pullenti.Unitext.UnitextContainer) 
                        {
                            Pullenti.Unitext.UnitextItem cc = Implantate(ch, impl, text, null);
                            if (cc == null) 
                            {
                            }
                            cnt.Children[i] = cc;
                        }
                        continue;
                    }
                    int j;
                    for (j = i; j < cnt.Children.Count; j++) 
                    {
                        ch = cnt.Children[j];
                        if (impl.EndChar < ch.BeginChar) 
                        {
                            if (impl.EndChar == (impl.BeginChar - 1) && impl.BeginChar == ch.BeginChar) 
                            {
                                cnt.Children.Insert(j, impl);
                                impl.Parent = cnt;
                                return content;
                            }
                            break;
                        }
                        if (ch == impl) 
                            return content;
                        impl.Children.Add(ch);
                        ch.Parent = impl;
                    }
                    if (j > i) 
                    {
                        cnt.Children[i] = impl;
                        impl.Parent = cnt;
                        if (j > (i + 1)) 
                            cnt.Children.RemoveRange(i + 1, j - i - 1);
                        impl.EndChar = impl.Children[impl.Children.Count - 1].EndChar;
                    }
                    break;
                }
                return content;
            }
            impl.Children.Add(content);
            impl.Parent = content.Parent;
            content.Parent = impl;
            if (own != null) 
            {
                int ii = own.GetChildIndexOf(content);
                if (ii >= 0) 
                {
                    own.Children[ii] = impl;
                    impl.Parent = own;
                }
                else 
                    return content;
            }
            return impl;
        }
        public static bool _isTableChar(char ch)
        {
            return ch == 7 || ch == 0x1E || ch == 0x1F;
        }
        public static Pullenti.Unitext.UnitextItem ImplantateBlock(Pullenti.Unitext.UnitextItem content, int beginHead, int beginBody, int beginTail, int beginAppendix, int end, string text, Pullenti.Unitext.UnitextContainer own, ref Pullenti.Unitext.UnitextDocblock res)
        {
            if (content == null) 
                return null;
            if (end < content.BeginChar) 
                return content;
            if (beginHead > content.EndChar) 
                return content;
            if ((beginHead < beginBody) && (beginBody < end)) 
                content = _splitTexts(content, beginHead, beginBody - 1, own);
            Pullenti.Unitext.UnitextTable tab = content as Pullenti.Unitext.UnitextTable;
            if (tab != null) 
            {
                int bh = beginHead;
                for (; bh < text.Length; bh++) 
                {
                    if (!_isTableChar(text[bh])) 
                        break;
                }
                int bb = beginBody;
                if (bb < bh) 
                    bb = bh;
                for (int r = 0; r < tab.RowsCount; r++) 
                {
                    for (int c = 0; c < tab.ColsCount; c++) 
                    {
                        Pullenti.Unitext.UnitextTablecell cel = tab.GetCell(r, c);
                        if (cel == null || cel.Content == null) 
                            continue;
                        c = cel.ColEnd;
                        if (cel.BeginChar <= bh && bh <= cel.EndChar && cel.RowBegin == cel.RowEnd) 
                        {
                            if ((bh < bb) && cel.EndChar > bh && bb > cel.EndChar) 
                            {
                                Pullenti.Unitext.UnitextDocblock res0 = new Pullenti.Unitext.UnitextDocblock() { BeginChar = bh, EndChar = cel.EndChar };
                                res0.Head = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Head };
                                (res0.Head as Pullenti.Unitext.UnitextContainer).Children.Add(cel.Content);
                                res0.Head.BeginChar = bh;
                                res0.Head.EndChar = cel.EndChar;
                                cel.Content = res0;
                                if (res == null) 
                                    res = res0;
                                return tab;
                            }
                            cel.Content = ImplantateBlock(cel.Content, beginHead, beginBody, beginTail, beginAppendix, end, text, null, ref res);
                        }
                        else if (cel.BeginChar <= bb && bb <= cel.EndChar) 
                        {
                            if ((bh < bb) && cel.EndChar > bh && bb > cel.EndChar) 
                            {
                                Pullenti.Unitext.UnitextDocblock res0 = new Pullenti.Unitext.UnitextDocblock() { BeginChar = bh, EndChar = cel.EndChar };
                                res0.Body = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Head };
                                (res0.Body as Pullenti.Unitext.UnitextContainer).Children.Add(cel.Content);
                                res0.Body.BeginChar = bb;
                                res0.Body.EndChar = cel.EndChar;
                                cel.Content = res0;
                                if (res == null) 
                                    res = res0;
                                return tab;
                            }
                            cel.Content = ImplantateBlock(cel.Content, beginHead, beginBody, beginTail, beginAppendix, end, text, null, ref res);
                        }
                    }
                }
                return tab;
            }
            Pullenti.Unitext.UnitextList li = content as Pullenti.Unitext.UnitextList;
            if (li != null) 
            {
                foreach (Pullenti.Unitext.UnitextListitem l in li.Items) 
                {
                    if (l.BeginChar <= beginHead && beginHead <= l.EndChar) 
                    {
                        if (l.Content != null) 
                            l.Content = ImplantateBlock(l.Content, beginHead, beginBody, beginTail, beginAppendix, end, text, null, ref res);
                        if (l.Sublist != null) 
                            ImplantateBlock(l.Sublist, beginHead, beginBody, beginTail, beginAppendix, end, text, null, ref res);
                    }
                }
                return li;
            }
            Pullenti.Unitext.UnitextContainer cnt = content as Pullenti.Unitext.UnitextContainer;
            if (cnt != null) 
            {
                for (int i = 0; i < cnt.Children.Count; i++) 
                {
                    Pullenti.Unitext.UnitextItem ch = cnt.Children[i];
                    if (ch.IsWhitespaces) 
                        continue;
                    if (end < ch.BeginChar) 
                        break;
                    if ((ch is Pullenti.Unitext.UnitextTable) || (ch is Pullenti.Unitext.UnitextList) || (ch is Pullenti.Unitext.UnitextDocblock)) 
                    {
                    }
                    if (beginHead > ch.GetPlaintextNsPos(text)) 
                    {
                        if ((beginHead < ch.EndChar) && (ch is Pullenti.Unitext.UnitextTable)) 
                        {
                            ImplantateBlock(ch, beginHead, beginBody, beginTail, beginAppendix, end, text, null, ref res);
                            return content;
                        }
                        continue;
                    }
                    if (ch != cnt.Children[i]) 
                    {
                    }
                    ch = cnt.Children[i];
                    if (ch is Pullenti.Unitext.UnitextDocblock) 
                        continue;
                    if ((ch is Pullenti.Unitext.UnitextList) && (beginHead < beginBody) && ch.GetPlaintextNsPos(text) == beginHead) 
                    {
                        Pullenti.Unitext.UnitextList list = ch as Pullenti.Unitext.UnitextList;
                        int jj = i + 1;
                        foreach (Pullenti.Unitext.UnitextListitem it in list.Items) 
                        {
                            if (it.Prefix != null) 
                            {
                                it.Prefix.Parent = cnt;
                                cnt.Children.Insert(jj++, it.Prefix);
                                Pullenti.Unitext.UnitextPlaintext sp = new Pullenti.Unitext.UnitextPlaintext() { Text = " " };
                                sp.BeginChar = it.Prefix.EndChar + 1;
                                sp.EndChar = sp.BeginChar;
                                sp.Parent = cnt;
                                cnt.Children.Insert(jj++, sp);
                            }
                            if (it.Content != null) 
                            {
                                it.Content.Parent = cnt;
                                cnt.Children.Insert(jj++, it.Content);
                            }
                            Pullenti.Unitext.UnitextNewline nl = new Pullenti.Unitext.UnitextNewline();
                            nl.BeginChar = cnt.Children[jj - 1].EndChar + 1;
                            nl.EndChar = nl.BeginChar;
                            nl.Parent = cnt;
                            cnt.Children.Insert(jj++, nl);
                            if (it.Sublist != null) 
                            {
                                it.Sublist.Parent = cnt;
                                cnt.Children.Insert(jj++, it.Sublist);
                            }
                        }
                        cnt.Children.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if ((ch is Pullenti.Unitext.UnitextTable) && ch.BeginChar <= beginHead && (end < ch.EndChar)) 
                    {
                        ImplantateBlock(ch, beginHead, beginBody, beginTail, beginAppendix, end, text, null, ref res);
                        return content;
                    }
                    res = new Pullenti.Unitext.UnitextDocblock() { BeginChar = beginHead, EndChar = end };
                    Pullenti.Unitext.UnitextContainer head = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Head };
                    Pullenti.Unitext.UnitextContainer body = new Pullenti.Unitext.UnitextContainer();
                    Pullenti.Unitext.UnitextContainer tail = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Tail };
                    Pullenti.Unitext.UnitextContainer apps = new Pullenti.Unitext.UnitextContainer();
                    int j;
                    for (j = i; j < cnt.Children.Count; j++) 
                    {
                        if (beginHead == beginBody) 
                            break;
                        ch = cnt.Children[j];
                        if (!ch.IsWhitespaces) 
                        {
                            int ppp = ch.GetPlaintextNsPos1(text);
                            if (beginBody < ppp) 
                                break;
                            if (beginBody == ppp && (beginBody < end)) 
                                break;
                        }
                        head.Children.Add(ch);
                        ch.Parent = head;
                    }
                    for (; j < cnt.Children.Count; j++) 
                    {
                        if (beginBody == beginTail || beginBody == beginAppendix) 
                            break;
                        ch = cnt.Children[j];
                        if (!ch.IsWhitespaces) 
                        {
                            if (beginTail < ch.GetPlaintextNsPos1(text)) 
                                break;
                        }
                        body.Children.Add(ch);
                        ch.Parent = body;
                    }
                    for (; j < cnt.Children.Count; j++) 
                    {
                        if (beginTail == end) 
                            break;
                        ch = cnt.Children[j];
                        if (!ch.IsWhitespaces) 
                        {
                            if (beginAppendix < ch.GetPlaintextNsPos1(text)) 
                                break;
                        }
                        tail.Children.Add(ch);
                        ch.Parent = tail;
                    }
                    for (; j < cnt.Children.Count; j++) 
                    {
                        if (beginAppendix == end) 
                            break;
                        ch = cnt.Children[j];
                        if (!ch.IsWhitespaces) 
                        {
                            if (end < ch.GetPlaintextNsPos1(text)) 
                                break;
                        }
                        apps.Children.Add(ch);
                        ch.Parent = apps;
                    }
                    if (j > i) 
                    {
                        cnt.Children[i] = res;
                        res.Parent = cnt;
                        if (j > (i + 1)) 
                            cnt.Children.RemoveRange(i + 1, j - i - 1);
                        if (head.Children.Count > 0) 
                        {
                            res.Head = head;
                            head.Parent = res;
                            head.BeginChar = beginHead;
                            head.EndChar = head.Children[head.Children.Count - 1].EndChar;
                        }
                        if (body.Children.Count > 0) 
                        {
                            res.Body = body;
                            body.Parent = res;
                            body.BeginChar = beginBody;
                            body.EndChar = body.Children[body.Children.Count - 1].EndChar;
                        }
                        if (tail.Children.Count > 0) 
                        {
                            res.Tail = tail;
                            tail.Parent = res;
                            tail.BeginChar = beginTail;
                            tail.EndChar = tail.Children[tail.Children.Count - 1].EndChar;
                        }
                        if (apps.Children.Count > 0) 
                        {
                            res.Appendix = apps;
                            apps.Parent = res;
                            apps.BeginChar = beginAppendix;
                            apps.EndChar = apps.Children[apps.Children.Count - 1].EndChar;
                        }
                    }
                    break;
                }
                return content;
            }
            if ((content is Pullenti.Unitext.UnitextPlaintext) && (((content.Parent is Pullenti.Unitext.UnitextTablecell) || (content.Parent is Pullenti.Unitext.UnitextListitem)))) 
            {
                if ((content.BeginChar == beginBody && beginBody == beginHead && end == beginTail) && end == beginAppendix && end == content.EndChar) 
                {
                    res = new Pullenti.Unitext.UnitextDocblock() { BeginChar = beginHead, EndChar = end };
                    res.Body = content;
                    return res;
                }
            }
            return content;
        }
    }
}