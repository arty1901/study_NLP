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

namespace Pullenti.Unitext.Internal.Rtf
{
    class RichTextItem
    {
        public string Text;
        public List<RichTextItem> Children = new List<RichTextItem>();
        public RichTextItem Owner;
        public object Tag;
        public int BeginChar;
        public int EndChar;
        public bool IsPageBreak = false;
        public int NewLines = 0;
        public void AddChild(RichTextItem it)
        {
            if (Children == null) 
                Children = new List<RichTextItem>();
            Children.Add(it);
            it.Owner = this;
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.Append(this.GetType().Name.Substring(8));
            if (Text != null) 
                res.AppendFormat(" '{0}'", Text);
            else if (Children != null) 
                res.AppendFormat(" {0} items", Children.Count);
            return res.ToString();
        }
    }
}