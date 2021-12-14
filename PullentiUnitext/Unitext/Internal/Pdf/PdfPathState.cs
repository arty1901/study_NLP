/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Pdf
{
    class PdfPathState
    {
        PdfPage m_Page;
        public PdfPathState(PdfPage p)
        {
            m_Page = p;
            m_x = new List<double>();
            m_y = new List<double>();
        }
        List<double> m_x;
        List<double> m_y;
        public bool ParseOne(List<PdfObject> lex, int i)
        {
            if (i >= lex.Count) 
                return false;
            string nam = (lex[i] as PdfName).Name;
            if (nam == "m") 
            {
                m_x.Clear();
                m_y.Clear();
                m_x.Add(lex[i - 2].GetDouble());
                m_y.Add(m_Page.Height - lex[i - 1].GetDouble());
                return true;
            }
            if (nam == "l") 
            {
                m_x.Add(lex[i - 2].GetDouble());
                m_y.Add(m_Page.Height - lex[i - 1].GetDouble());
                return true;
            }
            if (nam == "h") 
            {
                if (m_x.Count > 0) 
                {
                    m_x.Add(m_x[0]);
                    m_y.Add(m_y[0]);
                }
                return true;
            }
            if (nam == "re") 
            {
                double x = lex[i - 4].GetDouble();
                double y = m_Page.Height - lex[i - 3].GetDouble();
                double w = lex[i - 2].GetDouble();
                double h = lex[i - 1].GetDouble();
                m_x.Add(x);
                m_y.Add(y);
                m_x.Add(x);
                m_y.Add(y - h);
                m_x.Add(x + w);
                m_y.Add(y - h);
                m_x.Add(x + w);
                m_y.Add(y);
                m_x.Add(x);
                m_y.Add(y);
                return true;
            }
            if (nam == "n") 
            {
                m_x.Clear();
                m_y.Clear();
                return true;
            }
            if (((((nam == "S" || nam == "s" || nam == "b") || nam == "B" || nam == "B*") || nam == "b*" || nam == "f") || nam == "F" || nam == "f*") || nam == "W" || nam == "W*") 
            {
                if (m_x.Count == 5 && m_x[0] == m_x[4] && m_y[0] == m_y[4]) 
                {
                    PdfFig re = null;
                    if (m_x[1] == m_x[0] && m_y[1] == m_y[2] && m_x[2] == m_x[3]) 
                    {
                        re = new PdfFig();
                        re.X1 = m_x[0];
                        re.X2 = m_x[2];
                        re.Y1 = m_y[0];
                        re.Y2 = m_y[1];
                    }
                    else if (m_y[0] == m_y[1] && m_x[1] == m_x[2] && m_y[2] == m_y[3]) 
                    {
                        re = new PdfFig();
                        re.X1 = m_x[0];
                        re.X2 = m_x[1];
                        re.Y1 = m_y[0];
                        re.Y2 = m_y[2];
                    }
                    if (re != null) 
                    {
                        if (re.X1 > re.X2) 
                        {
                            double d = re.X1;
                            re.X1 = re.X2;
                            re.X2 = d;
                        }
                        if (re.Y1 > re.Y2) 
                        {
                            double d = re.Y1;
                            re.Y1 = re.Y2;
                            re.Y2 = d;
                        }
                        m_Page.Items.Add(re);
                        return true;
                    }
                }
                for (int ii = 0; ii < (m_x.Count - 1); ii++) 
                {
                    if (m_x[ii] == m_x[ii + 1] || m_y[ii] == m_y[ii + 1]) 
                    {
                        PdfFig re = new PdfFig();
                        re.X1 = m_x[ii];
                        re.X2 = m_x[ii + 1];
                        re.Y1 = m_y[ii];
                        re.Y2 = m_y[ii + 1];
                        if (re.X1 > re.X2) 
                        {
                            double d = re.X1;
                            re.X1 = re.X2;
                            re.X2 = d;
                        }
                        if (re.Y1 > re.Y2) 
                        {
                            double d = re.Y1;
                            re.Y1 = re.Y2;
                            re.Y2 = d;
                        }
                        m_Page.Items.Add(re);
                    }
                }
                return true;
            }
            return false;
        }
    }
}