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

namespace Pullenti.Util
{
    public class StructExpressBlock
    {
        public List<StructExpressLine> Lines = new List<StructExpressLine>();
        public bool IsApp;
        public bool HasStructure;
        public int BeginChar
        {
            get
            {
                if (Lines.Count > 0) 
                    return Lines[0].BeginChar;
                return 0;
            }
        }
        public int EndChar
        {
            get
            {
                if (Lines.Count > 0) 
                    return Lines[Lines.Count - 1].EndChar;
                return 0;
            }
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.AppendFormat("[{0}..{1}]", BeginChar, EndChar);
            if (IsApp) 
                res.Append(" Appendix");
            if (HasStructure) 
                res.Append(" Structured");
            return res.ToString();
        }
        public static List<StructExpressBlock> Parse(string text)
        {
            List<StructExpressLine> lines = new List<StructExpressLine>();
            for (int i = 0; i < text.Length; ) 
            {
                StructExpressLine li = StructExpressLine.TryParse(text, ref i);
                if (li == null) 
                    break;
                lines.Add(li);
            }
            if (lines.Count == 0) 
                return null;
            for (int i = 0; i < lines.Count; i++) 
            {
                if (lines[i].IsStartOfApp && lines[i + 1].IsStartOfApp) 
                {
                    for (; i < lines.Count; i++) 
                    {
                        if (lines[i].IsStartOfApp) 
                            lines[i].IsStartOfApp = false;
                        else 
                            break;
                    }
                }
            }
            List<StructExpressBlock> res = new List<StructExpressBlock>();
            StructExpressBlock cur = new StructExpressBlock();
            res.Add(cur);
            for (int i = 0; i < lines.Count; i++) 
            {
                StructExpressLine li = lines[i];
                if (li.IsStartOfApp) 
                {
                    if (cur.Lines.Count > 0) 
                    {
                        cur = new StructExpressBlock();
                        res.Add(cur);
                    }
                    cur.IsApp = true;
                }
                cur.Lines.Add(li);
            }
            foreach (StructExpressBlock b in res) 
            {
                b._analyze();
            }
            return res;
        }
        void _analyze()
        {
            int clauses = 0;
            int numbered = 0;
            int illegals = 0;
            foreach (StructExpressLine li in Lines) 
            {
                if (li.Keyword == "СТАТЬЯ" || li.Keyword == "СТАТТЯ") 
                    clauses++;
                if (li.Number != null) 
                    numbered++;
                if (li.IllegalChars >= 2) 
                    illegals++;
            }
            if (clauses > 1) 
                HasStructure = true;
            else if (clauses == 1 && numbered > 3) 
                HasStructure = true;
            else if (numbered > 5 && numbered > (Lines.Count / 2) && (illegals < 2)) 
                HasStructure = true;
        }
    }
}