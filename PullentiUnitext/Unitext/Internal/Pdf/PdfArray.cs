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

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfArray : PdfObject
    {
        List<PdfObject> m_Items = new List<PdfObject>();
        public int ItemsCount
        {
            get
            {
                return m_Items.Count;
            }
        }
        public PdfObject GetItem(int ind)
        {
            if ((ind < 0) || ind >= m_Items.Count) 
                return null;
            if (m_Items[ind] is PdfReference) 
            {
                PdfObject obj = SourceFile.GetObject(m_Items[ind].Id);
                if (obj == null) 
                    return null;
                m_Items[ind] = obj;
            }
            return m_Items[ind];
        }
        public override bool IsSimple(int lev)
        {
            if (m_Items.Count > 10 || lev > 5) 
                return false;
            for (int i = 0; i < m_Items.Count; i++) 
            {
                PdfObject it = this.GetItem(i);
                if (it != null && !it.IsSimple(lev + 1)) 
                    return false;
            }
            return true;
        }
        internal void Add(PdfObject obj)
        {
            m_Items.Add(obj);
        }
        public override string ToString(int lev)
        {
            if (lev > 10) 
                return string.Format("[...{0}]", ItemsCount);
            StringBuilder res = new StringBuilder();
            res.Append("[");
            int i;
            for (i = 0; i < m_Items.Count; i++) 
            {
                if (i > 0) 
                    res.Append(", ");
                if (res.Length > 100) 
                    break;
                PdfObject it = this.GetItem(i);
                if (it == null) 
                    res.Append("NULL");
                else if (!it.IsSimple(lev + 1)) 
                    break;
                else 
                {
                    string str = it.ToString(lev + 1);
                    if (str.Length < 20) 
                    {
                        res.Append(str);
                        continue;
                    }
                    else 
                        break;
                }
            }
            if (i < m_Items.Count) 
                res.AppendFormat("... {0}", m_Items.Count);
            res.Append("]");
            return res.ToString();
        }
        internal void PostParse(PdfStream stream)
        {
            m_Items.Clear();
            int p0 = (int)stream.Position;
            byte ch = (byte)stream.ReadByte();
            if (ch != '[') 
                return;
            while (true) 
            {
                int i = stream.PeekSolidByte();
                if (i < 0) 
                    break;
                ch = (byte)i;
                if (ch == ']') 
                {
                    stream.Position += 1;
                    return;
                }
                PdfObject obj = stream.ParseObject(SourceFile, false);
                if (obj == null) 
                    break;
                m_Items.Add(obj);
            }
        }
        public override double GetDouble()
        {
            if (m_Items.Count < 1) 
                return 0;
            PdfObject it = this.GetItem(0);
            if (it == null) 
                return 0;
            return it.GetDouble();
        }
    }
}