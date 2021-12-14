/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Pdf
{
    class Matrix
    {
        public double A;
        public double B;
        public double C;
        public double D;
        public double E;
        public double F;
        public Matrix()
        {
            this.Init();
        }
        public void Set(double a, double b, double c, double d, double e, double f)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
        }
        public override string ToString()
        {
            return string.Format("[{0} {1} {2} {3} {4} {5}]", A, B, C, D, E, F);
        }
        public void Init()
        {
            A = 1;
            B = 0;
            C = 0;
            D = 1;
            E = 0;
            F = 0;
        }
        public void CopyFrom(Matrix m)
        {
            A = m.A;
            B = m.B;
            C = m.C;
            D = m.D;
            E = m.E;
            F = m.F;
        }
        public void Multiply(Matrix m)
        {
            double a = (A * m.A) + (B * m.C);
            double b = (A * m.B) + (B * m.D);
            double c = (C * m.A) + (D * m.C);
            double d = (C * m.B) + (D * m.D);
            double e = (E * m.A) + (F * m.C) + m.E;
            double f = (E * m.B) + (F * m.D) + m.F;
            this.Set(a, b, c, d, e, f);
        }
        public void Translate(double e, double f)
        {
            E = (A * e) + (C * f) + E;
            F = (B * e) + (D * f) + F;
        }
    }
}