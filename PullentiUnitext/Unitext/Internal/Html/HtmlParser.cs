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
    // Парсинг Html-текста
    public static class HtmlParser
    {
        public static HtmlNode Parse(StringBuilder sHtml, bool preserveWhitespaces = false)
        {
            int i;
            int j;
            int k;
            HtmlNode par;
            HtmlNode nod;
            string tmpHtml = sHtml.ToString();
            sHtml.Length = 0;
            for (i = 0; i < tmpHtml.Length; i++) 
            {
                if (!char.IsWhiteSpace(tmpHtml[i])) 
                    break;
            }
            string newLine = "\r";
            for (; i < tmpHtml.Length; i++) 
            {
                char ch = tmpHtml[i];
                if (ch == '\t') 
                    sHtml.Append("    ");
                else if (ch != '\r' && ch != '\n') 
                    sHtml.Append(ch);
                else if (ch == '\n') 
                    sHtml.Append(newLine);
                else if (ch == '\r') 
                {
                    if (((i + 1) < tmpHtml.Length) && tmpHtml[i + 1] == '\n') 
                        i++;
                    sHtml.Append(newLine);
                }
            }
            string html = sHtml.ToString();
            List<HtmlParserNode> items = new List<HtmlParserNode>();
            HtmlParserNode pn = new HtmlParserNode();
            int preserve = 0;
            for (i = 0; i < html.Length; i++) 
            {
                string tag = null;
                for (j = items.Count - 1; j >= 0; j--) 
                {
                    if (items[j].TagName != null) 
                    {
                        if (items[j].TagName == "SCRIPT" && items[j].TagType == HtmlParserNode.TagTypes.Open) 
                            tag = "SCRIPT";
                        break;
                    }
                }
                if (items.Count == 47) 
                {
                }
                if (!pn.Analize(html, i, tag, preserve > 0 || preserveWhitespaces)) 
                    break;
                i = pn.IndexTo;
                if (pn.TagName == "PRE") 
                {
                    if (pn.TagType == HtmlParserNode.TagTypes.Open) 
                        preserve++;
                    else if (pn.TagType == HtmlParserNode.TagTypes.Close) 
                        preserve--;
                }
                if (pn.TagName == "META") 
                {
                    string val = null;
                    if (pn.Attributes != null && pn.Attributes.TryGetValue("CONTENT", out val)) 
                    {
                        if (val == "Word.Document") 
                            preserve = 1;
                        else if (val.Contains("Microsoft Word")) 
                            preserve = 1;
                    }
                }
                pn.WhitespacePreserve = preserve > 0 || preserveWhitespaces;
                if (pn.IsEmpty) 
                    continue;
                items.Add(pn);
                if (pn.TagName != null) 
                    pn.Tag = HtmlTag.FindTag(pn.TagName);
                pn.CloseTagIndex = -1;
                pn = new HtmlParserNode();
            }
            List<HtmlParserNode> stack = new List<HtmlParserNode>();
            bool error = false;
            for (i = 0; i < items.Count; i++) 
            {
                pn = items[i];
                if (pn.Tag == null) 
                    continue;
                if (pn.TagType == HtmlParserNode.TagTypes.Close) 
                {
                    if (!pn.Tag.EndtagRequired || stack.Count == 0) 
                        continue;
                    HtmlParserNode pp = stack[stack.Count - 1];
                    if (pp.Tag == pn.Tag) 
                    {
                        stack.RemoveAt(stack.Count - 1);
                        pp.CloseTagIndex = i;
                        continue;
                    }
                    for (j = stack.Count - 1; j >= 0; j--) 
                    {
                        if (stack[j].Tag == pn.Tag) 
                            break;
                    }
                    if (j < 0) 
                        continue;
                    stack[j].CloseTagIndex = i;
                    stack.RemoveRange(j, stack.Count - j);
                    continue;
                }
                if (pn.TagType == HtmlParserNode.TagTypes.OpenClose) 
                {
                    pn.CloseTagIndex = i;
                    continue;
                }
                if (pn.TagType != HtmlParserNode.TagTypes.Open) 
                    continue;
                if (pn.Tag.IsEmpty) 
                {
                    pn.CloseTagIndex = i;
                    continue;
                }
                if (pn.Tag.EndtagRequired) 
                    stack.Add(pn);
            }
            if (stack.Count > 0) 
                error = true;
            if (error) 
            {
            }
            for (i = 0; i < items.Count; i++) 
            {
                pn = items[i];
                if (pn.TagName == "BR") 
                {
                }
                if (pn.CloseTagIndex >= 0 || pn.TagName == null || pn.TagType != HtmlParserNode.TagTypes.Open) 
                    continue;
                int maxi = -1;
                for (j = i - 1; j >= 0; j--) 
                {
                    if (items[j].CloseTagIndex > i) 
                    {
                        maxi = items[j].CloseTagIndex;
                        break;
                    }
                }
                if (maxi < 0) 
                    maxi = items.Count - 1;
                int firstItemWithSameTag = -1;
                k = 1;
                for (j = i + 1; j <= maxi; j++) 
                {
                    if (items[j].CloseTagIndex >= j) 
                        j = items[j].CloseTagIndex;
                    else if (items[j].TagName == pn.TagName) 
                    {
                        if (items[j].TagType != HtmlParserNode.TagTypes.Close && (firstItemWithSameTag < 0)) 
                            firstItemWithSameTag = j;
                        if (items[j].TagType == HtmlParserNode.TagTypes.Open) 
                            k++;
                        else if (items[j].TagType == HtmlParserNode.TagTypes.Close) 
                        {
                            k--;
                            if (k == 0) 
                            {
                                pn.CloseTagIndex = j;
                                break;
                            }
                        }
                    }
                }
                if (pn.CloseTagIndex >= 0) 
                    continue;
                if (pn.TagName == "HTML") 
                {
                    pn.CloseTagIndex = items.Count - 1;
                    continue;
                }
                if (firstItemWithSameTag >= 0) 
                    pn.CloseTagIndex = firstItemWithSameTag - 1;
                else 
                {
                    if (items[maxi].TagName != null) 
                        pn.CloseTagIndex = maxi - 1;
                    else 
                        pn.CloseTagIndex = maxi;
                    if (pn.TagName == "HEAD") 
                    {
                        for (j = i + 1; j < items.Count; j++) 
                        {
                            if (items[j].TagName == "BODY") 
                            {
                                pn.CloseTagIndex = j - 1;
                                break;
                            }
                        }
                    }
                    else if (pn.TagName == "P") 
                    {
                        for (j = i + 1; j < items.Count; j++) 
                        {
                            if (items[j].Tag != null && !items[j].Tag.IsInline) 
                            {
                                pn.CloseTagIndex = j - 1;
                                break;
                            }
                        }
                    }
                }
            }
            List<HtmlNode> resList = new List<HtmlNode>();
            for (i = 0; i < items.Count; i++) 
            {
                items[i].Printed = false;
            }
            for (i = 0; i < items.Count; i++) 
            {
                if (items[i].Printed) 
                    continue;
                nod = CreateNode(items, i, out j);
                if (j < 0) 
                    break;
                if (j < i) 
                {
                }
                if (nod == null) 
                {
                    i = j;
                    continue;
                }
                if (i == 0 && j == (items.Count - 1)) 
                {
                    nod.CorrectRusTexts(0);
                    return nod;
                }
                resList.Add(nod);
                i = j;
            }
            if (resList.Count == 0) 
                return null;
            foreach (HtmlNode li in resList) 
            {
                li.CorrectRusTexts(0);
            }
            if (resList.Count == 1) 
                return resList[0];
            HtmlNode res = new HtmlNode();
            res.TagName = "HTML";
            res.SourceHtmlPosition = 0;
            res.SourceHtmlLength = html.Length;
            foreach (HtmlNode ch in resList) 
            {
                res.Children.Add(ch);
                ch.Parent = res;
            }
            return res;
        }
        static HtmlNode CreateNode(List<HtmlParserNode> list, int indFrom, out int indTo)
        {
            indTo = indFrom;
            if ((indFrom < 0) || indFrom >= list.Count) 
                return null;
            HtmlParserNode pn = list[indFrom];
            if (pn.TagType == HtmlParserNode.TagTypes.Close) 
            {
                pn.Printed = true;
                indTo = indFrom;
                return null;
            }
            if (pn.Printed) 
                return null;
            HtmlNode res = new HtmlNode();
            res.SourceHtmlPosition = pn.IndexFrom;
            res.SourceHtmlEndPosition = pn.IndexTo;
            res.WhitespacePreserve = pn.WhitespacePreserve;
            if (pn.TagName == null) 
            {
                if (pn.PureText == null) 
                    return null;
                res.Text = pn.PureText;
                pn.Printed = true;
                return res;
            }
            res.TagName = pn.TagName;
            res.Attrs = pn.Attributes;
            res.Text = pn.PureText;
            pn.Printed = true;
            if (res.TagName == "SPAN") 
            {
            }
            if (pn.CloseTagIndex <= indFrom) 
                return res;
            indTo = pn.CloseTagIndex;
            HtmlParserNode cl = list[pn.CloseTagIndex];
            if (cl.TagType == HtmlParserNode.TagTypes.Close && cl.TagName == pn.TagName) 
            {
                res.SourceHtmlEndPosition = cl.IndexTo;
                if ((indFrom + 2) == pn.CloseTagIndex && list[indFrom + 1].TagName == null) 
                {
                    res.Text = list[indFrom + 1].PureText;
                    return res;
                }
            }
            else if (pn.CloseTagIndex > indFrom) 
            {
                res.SourceHtmlEndPosition = list[pn.CloseTagIndex].IndexTo;
                if ((indFrom + 1) == pn.CloseTagIndex && list[indFrom + 1].TagName == null) 
                {
                    res.Text = list[indFrom + 1].PureText;
                    indTo = indFrom + 1;
                    return res;
                }
            }
            else 
            {
            }
            for (int i = indFrom + 1; i <= pn.CloseTagIndex; i++) 
            {
                if (list[i].Printed) 
                    continue;
                int j;
                HtmlNode chi = CreateNode(list, i, out j);
                if (j < i) 
                    break;
                if (chi != null) 
                {
                    if (chi.TotalChildren > 100) 
                    {
                    }
                    res.Children.Add(chi);
                    chi.Parent = res;
                    if (chi.TagName == "UL") 
                    {
                    }
                }
                i = j;
            }
            return res;
        }
        static HtmlParser()
        {
            m_TableTags = new Dictionary<string, int>();
            m_TableTags.Add("TABLE", 0);
            m_TableTags.Add("TBODY", 1);
            m_TableTags.Add("THEAD", 1);
            m_TableTags.Add("TFOOT", 1);
            m_TableTags.Add("TR", 2);
            m_TableTags.Add("TH", 3);
            m_TableTags.Add("TD", 3);
            m_TableTags.Add("UL", 0);
            m_TableTags.Add("OL", 0);
            m_TableTags.Add("DL", 0);
            m_TableTags.Add("LI", 1);
            m_TableTags.Add("DT", 1);
            m_TableTags.Add("DD", 1);
        }
        static Dictionary<string, int> m_TableTags;
    }
}