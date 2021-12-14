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
    public class PdfPage : PdfRect
    {
        public PdfDictionary Dic;
        public PdfPage(PdfDictionary dic, object state0 = null)
        {
            Dic = dic;
            PdfArray crop = dic.GetObject("CropBox", false) as PdfArray;
            if (crop == null) 
                crop = dic.GetObject("TrimBox", false) as PdfArray;
            if (crop == null) 
                crop = dic.GetObject("MediaBox", false) as PdfArray;
            if (crop == null) 
                crop = dic.GetObject("BBox", false) as PdfArray;
            if (crop == null) 
            {
                if (dic.SourceFile.RootObject != null) 
                {
                    PdfDictionary pages = dic.SourceFile.RootObject.GetDictionary("Pages", null);
                    if (pages != null) 
                    {
                        crop = pages.GetObject("MediaBox", false) as PdfArray;
                        if (crop == null) 
                            crop = pages.GetObject("TrimBox", false) as PdfArray;
                        if (crop == null) 
                            crop = pages.GetObject("CropBox", false) as PdfArray;
                    }
                }
            }
            if (crop != null && crop.ItemsCount == 4) 
            {
                X2 = crop.GetItem(2).GetDouble();
                Y2 = crop.GetItem(3).GetDouble();
                PdfObject rot = dic.GetObject("Rotate", false);
                if (rot != null && ((rot.GetDouble() == 90 || rot.GetDouble() == 270))) 
                {
                    double xx = X2;
                    X2 = Y2;
                    Y2 = xx;
                }
            }
            PdfDictionary res = Dic.GetDictionary("Resources", null);
            PdfDictionary fnts = (res == null ? null : res.GetDictionary("Font", null));
            PdfDictionary imgs = (res == null ? null : res.GetDictionary("XObject", null));
            byte[] val = Dic.GetTotalDataStream("Contents");
            if (val == null && state0 != null) 
                val = Dic.ExtractData();
            if (val == null || (val.Length < 1)) 
                return;
            List<PdfObject> lex = null;
            using (PdfStream pstr = new PdfStream(null, val)) 
            {
                lex = pstr.ParseContent();
            }
            Dictionary<string, PdfFont> allFonts = new Dictionary<string, PdfFont>();
            PdfTextState state = state0 as PdfTextState;
            if (state == null) 
            {
                state = new PdfTextState();
                state.BoxHeight = Y2;
            }
            else 
            {
                PdfArray mat = dic.GetObject("Matrix", false) as PdfArray;
                if (mat != null && mat.ItemsCount == 6) 
                {
                    Matrix m = new Matrix();
                    m.Set(mat.GetItem(0).GetDouble(), mat.GetItem(1).GetDouble(), mat.GetItem(2).GetDouble(), mat.GetItem(3).GetDouble(), mat.GetItem(4).GetDouble(), mat.GetItem(5).GetDouble());
                    if (state.CtmStack.Count > 0) 
                    {
                        m.Multiply(state.CtmStack[0]);
                        state.CtmStack[0] = m;
                    }
                }
            }
            bool textRegime = false;
            StringBuilder buf = new StringBuilder();
            PdfPathState path = new PdfPathState(this);
            for (int i = 0; i < lex.Count; i++) 
            {
                if (!(lex[i] is PdfName)) 
                    continue;
                if ((lex[i] as PdfName).HasSlash) 
                    continue;
                string nam = (lex[i] as PdfName).Name;
                if (nam == "ET") 
                {
                    textRegime = false;
                    continue;
                }
                if (nam == "BT") 
                {
                    textRegime = true;
                    state.Tm.Init();
                    state.Tlm.Init();
                    continue;
                }
                if (nam == "Do" && (lex[i - 1] is PdfName)) 
                {
                    nam = (lex[i - 1] as PdfName).Name;
                    PdfDictionary xobjs = (res == null ? null : res.GetDictionary("XObject", null));
                    PdfDictionary xobj = (xobjs == null ? null : xobjs.GetDictionary(nam, null));
                    if (xobj == null) 
                    {
                        foreach (PdfDictionary rr in dic.SourceFile.AllResources) 
                        {
                            if (rr.GetStringItem("Name") == nam) 
                            {
                                xobj = rr;
                                break;
                            }
                        }
                    }
                    if (xobj != null) 
                    {
                        PdfName sub = xobj.GetObject("Subtype", false) as PdfName;
                        if (sub != null && sub.Name == "Image") 
                        {
                            PdfBoolValue im = xobj.GetObject("ImageMask", false) as PdfBoolValue;
                            if (im != null && im.Val) 
                            {
                            }
                            else 
                            {
                                PdfImage img = new PdfImage(xobj);
                                if (state.CtmStack.Count > 0) 
                                {
                                    img.X1 = (img.X2 = state.CtmStack[0].E);
                                    img.X2 += state.CtmStack[0].A;
                                    img.Y1 = (img.Y2 = state.BoxHeight - state.CtmStack[0].F);
                                    img.Y1 -= state.CtmStack[0].D;
                                    if (((img.Y1 < -0.1) || img.Y2 > Y2 || (img.X1 < -0.1)) || img.X2 > X2) 
                                    {
                                    }
                                }
                                Items.Add(img);
                            }
                        }
                        else 
                        {
                            PdfPage ppp = new PdfPage(xobj, state);
                            Items.AddRange(ppp.Items);
                        }
                    }
                    continue;
                }
                if (state.ParseOne(lex, i)) 
                    continue;
                if (path.ParseOne(lex, i)) 
                    continue;
                if (nam == "Tf" && (lex[i - 2] is PdfName)) 
                {
                    state.FontSize = lex[i - 1].GetDouble();
                    string fnam = (lex[i - 2] as PdfName).Name;
                    if (!allFonts.ContainsKey(fnam)) 
                    {
                        PdfDictionary fff = null;
                        if (fnts != null) 
                            fff = fnts.GetDictionary(fnam, null);
                        if (fff != null) 
                        {
                            if (fff.Tag is PdfFont) 
                                state.Font = fff.Tag as PdfFont;
                            else 
                            {
                                state.Font = new PdfFont(fff);
                                state.Font.Alias = fnam;
                            }
                            fff.Tag = state.Font;
                            allFonts.Add(fnam, state.Font);
                        }
                    }
                    else 
                        state.Font = allFonts[fnam];
                    continue;
                }
                if (textRegime && (((nam == "Tj" || nam == "TJ" || nam == "\"") || nam == "'"))) 
                {
                    if (state.Font == null) 
                        continue;
                    if (nam == "\"") 
                    {
                        state.CharSpace = lex[i - 2].GetDouble();
                        state.WordSpace = lex[i - 3].GetDouble();
                    }
                    if (nam == "\"" || nam == "'") 
                        state.Newline();
                    PdfText txt = state.Font.ExtractText(lex[i - 1], state);
                    if (txt != null) 
                        Items.Add(txt);
                    continue;
                }
            }
            if (Y2 > 0) 
            {
                foreach (PdfRect it in Items) 
                {
                    while (it.Y1 > Y2) 
                    {
                        it.Y1 -= Y2;
                        it.Y2 -= Y2;
                    }
                }
            }
            for (int i = 0; i < (Items.Count - 1); i++) 
            {
                if ((Items[i] is PdfText) && (Items[i + 1] is PdfText)) 
                {
                    if ((Items[i] as PdfText).CanBeMergedWith(Items[i + 1] as PdfText)) 
                    {
                        (Items[i] as PdfText).MergeWith(Items[i + 1] as PdfText);
                        Items.RemoveAt(i + 1);
                        i--;
                    }
                }
            }
            double maxx = (double)0;
            foreach (PdfRect it in Items) 
            {
                if (it.Right > maxx) 
                    maxx = it.Right;
            }
            if (maxx > X2) 
            {
                double ratio = ((X2 - 3)) / maxx;
                foreach (PdfRect it in Items) 
                {
                    it.X1 *= ratio;
                    it.X2 *= ratio;
                }
            }
        }
        public List<PdfRect> Items = new List<PdfRect>();
    }
}