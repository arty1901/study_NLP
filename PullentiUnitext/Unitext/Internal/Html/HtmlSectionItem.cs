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
    class HtmlSectionItem
    {
        public List<HtmlNode> Head = new List<HtmlNode>();
        public List<HtmlNode> Tail = new List<HtmlNode>();
        public bool HasBody;
        public List<HtmlSection> Stack = new List<HtmlSection>();
        public Pullenti.Unitext.UnitextItem Generate(UnitextHtmlGen hg)
        {
            Pullenti.Unitext.UnitextDocblock doc = new Pullenti.Unitext.UnitextDocblock() { Typname = "Document" };
            Stack[0]._generate(doc, 0, hg);
            return doc;
        }
    }
}