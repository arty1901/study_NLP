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
    // Используется для встраивания узлов в HTML-тексты
    public class HtmlCorrector
    {
        public HtmlCorrector(string html)
        {
            m_Html = html;
            m_Impls = new Dictionary<string, HtmlNode>();
            this._parse();
            if (m_Text == null) 
                m_Text = "";
        }
        string m_Html;
        HtmlNode m_Root;
        Dictionary<string, HtmlNode> m_Impls;
        void _parse()
        {
            StringBuilder tmp = new StringBuilder(m_Html);
            m_Root = HtmlParser.Parse(tmp, true);
            if (m_Root != null) 
            {
                if (m_Root.TagName == "HTML") 
                    m_Root.TagName = "DIV";
                if (!string.IsNullOrEmpty(m_Root.Text) && m_Root.TagName == null) 
                {
                    HtmlNode hh = new HtmlNode() { TagName = "span" };
                    hh.Children.Add(m_Root);
                    m_Root.Parent = hh;
                    m_Root = hh;
                }
                this._createText();
            }
        }
        void _createText()
        {
            Pullenti.Unitext.GetPlaintextParam pars = new Pullenti.Unitext.GetPlaintextParam() { NewLine = "\n", Tab = "  ", PageBreak = "\n\n" };
            m_Text = m_Root.GetPlaintext(pars);
        }
        public string PlainText
        {
            get
            {
                return m_Text;
            }
        }
        string m_Text;
        public string HtmlText
        {
            get
            {
                if (m_Root == null) 
                    return "";
                StringBuilder res = new StringBuilder();
                m_Root.GetHtml(res);
                return res.ToString();
            }
        }
        public bool SetText(int beginChar, int endChar, string newText)
        {
            if (m_Root == null || m_Text == null) 
                return false;
            if (beginChar > m_Text.Length || endChar > m_Text.Length || beginChar > endChar) 
                return false;
            HtmlNode nod = m_Root.ImplantateNode(beginChar, endChar, "span");
            if (nod == null) 
                return false;
            if (string.IsNullOrEmpty(newText)) 
            {
                if (nod.Parent != null && nod.Parent.Children.Contains(nod)) 
                {
                    nod.Parent.Children.Remove(nod);
                    this._createText();
                    return true;
                }
                return false;
            }
            if (nod.Children != null && nod.Children.Count > 0) 
                nod.Children.Clear();
            nod.Text = newText;
            nod.TagName = null;
            this._createText();
            return true;
        }
        public bool ImplantateNode(int beginChar, int endChar, string tagName, string id, Dictionary<string, string> attrs)
        {
            if ((m_Root == null || string.IsNullOrEmpty(tagName) || (beginChar < 0)) || beginChar > endChar || endChar >= m_Text.Length) 
                return false;
            if (id == null) 
                return false;
            if (m_Impls.ContainsKey(id)) 
                this.RemoveNode(id);
            for (; (beginChar < endChar) && (beginChar < m_Text.Length); beginChar++) 
            {
                if (!char.IsWhiteSpace(m_Text[beginChar])) 
                    break;
            }
            for (; endChar > beginChar; endChar--) 
            {
                if (!char.IsWhiteSpace(m_Text[endChar])) 
                    break;
            }
            HtmlNode nod = m_Root.ImplantateNode(beginChar, endChar, tagName);
            if (nod == null) 
                return false;
            nod.Attrs = new Dictionary<string, string>();
            nod.Attrs.Add("ID", id);
            if (attrs != null) 
            {
                foreach (KeyValuePair<string, string> kp in attrs) 
                {
                    if (!nod.Attrs.ContainsKey(kp.Key.ToUpper())) 
                        nod.Attrs.Add(kp.Key.ToUpper(), kp.Value);
                }
            }
            m_Impls.Add(id, nod);
            return true;
        }
        public bool RemoveNode(string id)
        {
            if (m_Impls.ContainsKey(id)) 
            {
                m_Impls[id].Remove();
                m_Impls.Remove(id);
                if (m_Impls.Count == 0) 
                    this._parse();
                return true;
            }
            else 
                return false;
        }
        public HtmlNode FindNode(string tagName, string attrName, string attrValue)
        {
            if (m_Root == null) 
                return null;
            return m_Root.FindSubnode(tagName, attrName, attrValue);
        }
        public void ClearAll()
        {
            foreach (KeyValuePair<string, HtmlNode> kp in m_Impls) 
            {
                kp.Value.Remove();
            }
            m_Impls.Clear();
            this._parse();
        }
    }
}