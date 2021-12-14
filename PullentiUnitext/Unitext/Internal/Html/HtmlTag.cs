/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Html
{
    class HtmlTag
    {
        public static HtmlTag FindTag(string name)
        {
            if (name == null) 
                return null;
            HtmlTag res;
            if (!AllTags.TryGetValue(name, out res)) 
                return null;
            return res;
        }
        public string Name;
        public bool IsInline
        {
            get
            {
                return ((m_Attrs & Attrs.Inline)) != 0;
            }
        }
        public bool IsBlock
        {
            get
            {
                return ((m_Attrs & Attrs.Block)) != 0;
            }
        }
        public bool IsTable
        {
            get
            {
                return ((m_Attrs & Attrs.TableItem)) != 0;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return ((m_Attrs & Attrs.Empty)) != 0;
            }
        }
        public bool EndtagRequired
        {
            get
            {
                return ((m_Attrs & Attrs.EndtagReq)) != 0;
            }
        }
        public bool IgnoreWithContent
        {
            get
            {
                return ((m_Attrs & Attrs.IgnoreWithContent)) != 0;
            }
        }
        enum Attrs : int
        {
            Inline = 1,
            Block = 2,
            TableItem = 4,
            Control = 8,
            IgnoreThis = 0x10,
            IgnoreWithContent = 0x20,
            Empty = 0x40,
            EndtagReq = 0x80,
        }

        Attrs m_Attrs;
        HtmlTag(string name, Attrs typ)
        {
            Name = name;
            m_Attrs = typ;
            m_AllTags.Add(name, this);
        }
        static Dictionary<string, HtmlTag> m_AllTags;
        static Dictionary<string, HtmlTag> AllTags
        {
            get
            {
                if (m_AllTags == null) 
                {
                    m_AllTags = new Dictionary<string, HtmlTag>();
                    CreateAllTags();
                }
                return m_AllTags;
            }
        }
        static void CreateAllTags()
        {
            HtmlTag ht;
            ht = new HtmlTag("A", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("ABBR", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("ACRONYM", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("ADDRESS", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("APPLET", Attrs.Control | Attrs.IgnoreWithContent | Attrs.EndtagReq);
            ht = new HtmlTag("AREA", Attrs.Block | Attrs.Empty);
            ht = new HtmlTag("B", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("BASE", Attrs.Control | Attrs.IgnoreThis | Attrs.Empty);
            ht = new HtmlTag("BASEFONT", Attrs.Inline | Attrs.Empty);
            ht = new HtmlTag("BDO", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("BIG", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("BLOCKQUOTE", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("BODY", Attrs.Block);
            ht = new HtmlTag("BR", Attrs.Block | Attrs.Empty);
            ht = new HtmlTag("BUTTON", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("CAPTION", Attrs.TableItem | Attrs.EndtagReq);
            ht = new HtmlTag("CENTER", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("CITE", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("CODE", Attrs.Control | Attrs.IgnoreWithContent | Attrs.EndtagReq);
            ht = new HtmlTag("COL", Attrs.TableItem | Attrs.Empty);
            ht = new HtmlTag("COLGROUP", Attrs.TableItem);
            ht = new HtmlTag("DD", Attrs.Block);
            ht = new HtmlTag("DEL", Attrs.Inline | Attrs.IgnoreWithContent | Attrs.EndtagReq);
            ht = new HtmlTag("DFN", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("DIR", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("DIV", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("DL", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("DT", Attrs.Inline);
            ht = new HtmlTag("EM", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("FIELDSET", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("FONT", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("FORM", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("FRAME", Attrs.Block | Attrs.Empty);
            ht = new HtmlTag("FRAMESET", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("H1", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("H2", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("H3", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("H4", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("H5", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("H6", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("HEAD", Attrs.Block);
            ht = new HtmlTag("HR", Attrs.Block | Attrs.Empty);
            ht = new HtmlTag("HTML", Attrs.Block);
            ht = new HtmlTag("I", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("IFRAME", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("IMG", Attrs.Inline | Attrs.Empty);
            ht = new HtmlTag("INPUT", Attrs.Control | Attrs.IgnoreThis | Attrs.Empty);
            ht = new HtmlTag("INS", Attrs.Inline | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("ISINDEX", Attrs.Control | Attrs.Empty);
            ht = new HtmlTag("KBD", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("LABEL", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("LEGEND", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("LI", Attrs.Block);
            ht = new HtmlTag("LINK", Attrs.Control | Attrs.Empty);
            ht = new HtmlTag("MAP", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("MENU", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("META", Attrs.Control | Attrs.Empty);
            ht = new HtmlTag("NOFRAMES", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("NOSCRIPT", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("OBJECT", Attrs.Control | Attrs.IgnoreWithContent | Attrs.EndtagReq);
            ht = new HtmlTag("OL", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("OPTGROUP", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("OPTION", Attrs.Control | Attrs.IgnoreThis);
            ht = new HtmlTag("P", Attrs.Block);
            ht = new HtmlTag("PARAM", Attrs.Control | Attrs.IgnoreWithContent | Attrs.EndtagReq);
            ht = new HtmlTag("PRE", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("Q", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("S", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("SAMP", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("SCRIPT", Attrs.Control | Attrs.IgnoreWithContent | Attrs.EndtagReq);
            ht = new HtmlTag("SELECT", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("SMALL", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("SPAN", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("STRIKE", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("STRONG", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("STYLE", Attrs.Control | Attrs.IgnoreWithContent | Attrs.EndtagReq);
            ht = new HtmlTag("SUB", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("SUP", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("TABLE", Attrs.TableItem | Attrs.EndtagReq);
            ht = new HtmlTag("TBODY", Attrs.TableItem);
            ht = new HtmlTag("TD", Attrs.TableItem);
            ht = new HtmlTag("TFOOT", Attrs.TableItem);
            ht = new HtmlTag("TH", Attrs.TableItem);
            ht = new HtmlTag("THEAD", Attrs.TableItem);
            ht = new HtmlTag("TR", Attrs.TableItem);
            ht = new HtmlTag("TEXTAREA", Attrs.Control | Attrs.EndtagReq);
            ht = new HtmlTag("TITLE", Attrs.Control | Attrs.IgnoreThis | Attrs.EndtagReq);
            ht = new HtmlTag("TT", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("U", Attrs.Inline | Attrs.EndtagReq);
            ht = new HtmlTag("UL", Attrs.Block | Attrs.EndtagReq);
            ht = new HtmlTag("VAR", Attrs.Inline | Attrs.EndtagReq);
        }
    }
}