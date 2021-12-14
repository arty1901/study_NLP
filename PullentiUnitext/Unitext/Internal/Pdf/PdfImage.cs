/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfImage : PdfRect
    {
        public PdfImage(PdfDictionary xobj)
        {
            PdfObject wi = xobj.GetObject("Width", false);
            if (wi != null) 
                ImageWidth = (int)wi.GetDouble();
            PdfObject hi = xobj.GetObject("Height", false);
            if (hi != null) 
                ImageHeight = (int)hi.GetDouble();
            PdfName sub = xobj.GetObject("Filter", false) as PdfName;
            byte[] data = xobj.Data;
            if (sub == null) 
            {
                PdfArray aaa = xobj.GetObject("Filter", false) as PdfArray;
                if (aaa != null && aaa.ItemsCount == 1) 
                    sub = aaa.GetItem(0) as PdfName;
                else if ((aaa != null && aaa.ItemsCount == 2 && (aaa.GetItem(0) is PdfName)) && (aaa.GetItem(0) as PdfName).Name == "FlateDecode") 
                {
                    data = xobj.ExtractData();
                    sub = aaa.GetItem(1) as PdfName;
                }
            }
            if (sub != null && sub.Name == "DCTDecode") 
            {
                Content = data;
                return;
            }
            if (sub != null && sub.Name == "JPXDecode") 
            {
                Content = data;
                return;
            }
            if (sub != null && sub.Name == "CCITTFaxDecode") 
            {
                PdfDictionary parms = xobj.GetDictionary("DecodeParms", null);
                if (parms != null) 
                {
                    int k = (int)parms.GetIntItem("K");
                    int width = (int)parms.GetIntItem("Columns");
                    if (width < 10) 
                        width = (int)xobj.GetIntItem("Width");
                    int height = (int)parms.GetIntItem("Rows");
                    if (height < 10) 
                        height = (int)xobj.GetIntItem("Height");
                    bool invert = false;
                    string str = parms.GetStringItem("BlackIs1");
                    if (str == null) 
                        str = xobj.GetStringItem("ImageMask");
                    if (str != null) 
                    {
                        if (str == "true") 
                            invert = true;
                    }
                    if (width > 0 && height > 0) 
                    {
                        using (MemoryStream mem = new MemoryStream()) 
                        {
                            Pullenti.Unitext.Internal.Misc.TiffHelper.Tiff tif = new Pullenti.Unitext.Internal.Misc.TiffHelper.Tiff(null);
                            tif.WriteTiff(mem, width, height, (k < 0 ? Pullenti.Unitext.Internal.Misc.TiffHelper.CCITT_Types.G4 : (k == 0 ? Pullenti.Unitext.Internal.Misc.TiffHelper.CCITT_Types.G3_2D : Pullenti.Unitext.Internal.Misc.TiffHelper.CCITT_Types.G3_1D)), 300, 300, invert, data);
                            Content = mem.ToArray();
                        }
                    }
                }
                return;
            }
            if (((sub == null || sub.Name == "FlateDecode")) && ImageWidth > 0 && ImageHeight > 0) 
            {
                data = xobj.ExtractData();
                PdfObject bpi = xobj.GetObject("BitsPerComponent", false);
                PdfDictionary dparms = xobj.GetDictionary("DecodeParms", null);
                int predict = 0;
                if (dparms != null) 
                {
                    PdfIntValue pred = dparms.GetObject("Predictor", false) as PdfIntValue;
                    if (pred != null && pred.Val > 1) 
                    {
                        predict = pred.Val;
                        if (predict >= 10 && (predict < 15)) 
                        {
                        }
                    }
                }
                PdfObject colorsp = xobj.GetObject("ColorSpace", false);
                byte[] colorMap = null;
                if (bpi != null && ((bpi.GetDouble() == 8 || bpi.GetDouble() == 4 || bpi.GetDouble() == 1)) && colorsp != null) 
                {
                    string colorTyp = null;
                    int bpiVal = (int)bpi.GetDouble();
                    bool isIccBased = false;
                    if (colorsp is PdfName) 
                        colorTyp = (colorsp as PdfName).Name;
                    else if (colorsp is PdfArray) 
                    {
                        PdfArray arr = colorsp as PdfArray;
                        PdfName it0 = arr.GetItem(0) as PdfName;
                        if (it0 != null && it0.Name == "ICCBased") 
                            isIccBased = true;
                        if (it0 != null && it0.Name == "Indexed") 
                        {
                            PdfStringValue sss = arr.GetItem(3) as PdfStringValue;
                            if (sss != null) 
                                colorMap = sss.Val;
                            else 
                            {
                                PdfDictionary ddd = arr.GetItem(3) as PdfDictionary;
                                if (ddd != null) 
                                    colorMap = ddd.ExtractData();
                            }
                        }
                        PdfDictionary dii = arr.GetItem(3) as PdfDictionary;
                        if (dii == null && arr.ItemsCount == 2) 
                        {
                            dii = arr.GetItem(1) as PdfDictionary;
                            PdfDictionary ddd = arr.GetItem(1) as PdfDictionary;
                            if (ddd != null && (ddd.GetObject("Alternate", false) is PdfName)) 
                                colorTyp = (ddd.GetObject("Alternate", false) as PdfName).Name;
                        }
                        if (arr.ItemsCount > 2 && (arr.GetItem(1) is PdfArray)) 
                        {
                            arr = arr.GetItem(1) as PdfArray;
                            if (arr.ItemsCount == 2) 
                            {
                                PdfDictionary ddd = arr.GetItem(1) as PdfDictionary;
                                if (ddd != null && (ddd.GetObject("Alternate", false) is PdfName)) 
                                    colorTyp = (ddd.GetObject("Alternate", false) as PdfName).Name;
                            }
                        }
                        else if (arr.ItemsCount == 4 && (arr.GetItem(1) is PdfName)) 
                            colorTyp = (arr.GetItem(1) as PdfName).Name;
                    }
                    if (colorTyp == "DeviceRGB" || colorTyp == "DeviceGray" || colorTyp == "DeviceCMYK") 
                    {
                        if (data != null) 
                        {
                            int rowWidth = data.Length / ImageHeight;
                            int mmm = data.Length % ImageHeight;
                            if (mmm != 0) 
                            {
                            }
                            int nn = rowWidth / ImageWidth;
                            if (nn == 0 && (bpiVal < 8)) 
                                nn = 1;
                            bool isGray = colorTyp == "DeviceGray";
                            bool isCmyk = colorTyp == "DeviceCMYK";
                            if ((nn == 3 || ((nn == 4 && isCmyk)) || ((isCmyk && colorMap != null))) || bpiVal == 4 || (((nn == 1 && ((colorMap != null || colorTyp == "DeviceGray"))) || ((bpiVal == 1 && colorMap != null))))) 
                            {
                                if (colorTyp == "DeviceGray") 
                                {
                                }
                                if (colorTyp == "DeviceRGB" && colorMap != null && ((colorMap.Length % 3)) == 0) 
                                {
                                }
                                if (isCmyk && colorMap != null) 
                                {
                                    byte[] rgb = new byte[(int)(((colorMap.Length / 4)) * 3)];
                                    int j = 0;
                                    for (int i = 0; (i + 3) < colorMap.Length; i += 4) 
                                    {
                                        rgb[j] = _corrCMYK(colorMap[i], colorMap[i + 3]);
                                        rgb[j + 1] = _corrCMYK(colorMap[i + 1], colorMap[i + 3]);
                                        rgb[j + 2] = _corrCMYK(colorMap[i + 2], colorMap[i + 3]);
                                        j += 3;
                                    }
                                    colorMap = rgb;
                                }
                                Pullenti.Unitext.Internal.Misc.PngWrapper img = new Pullenti.Unitext.Internal.Misc.PngWrapper(ImageWidth, ImageHeight, isGray, colorMap);
                                for (int row = 0; row < ImageHeight; row++) 
                                {
                                    int pp = row * rowWidth;
                                    img.BeginRow();
                                    if (bpiVal == 1) 
                                    {
                                        for (int ii = 0; ii < rowWidth; ii++) 
                                        {
                                            if ((pp + ii) >= data.Length) 
                                                break;
                                            byte b = data[pp + ii];
                                            for (int jj = 7; jj >= 0; jj--) 
                                            {
                                                img.AddPixelIndex((byte)((((b >> jj)) & 1)));
                                            }
                                        }
                                    }
                                    else 
                                        for (int w = 0; w < ImageWidth; w++) 
                                        {
                                            if (nn == 3) 
                                            {
                                                if ((pp + 2) >= data.Length) 
                                                    break;
                                                img.AddPixelRgb(data[pp + 2], data[pp], data[pp + 1]);
                                                pp += 3;
                                            }
                                            else if (nn == 4) 
                                            {
                                                if ((pp + 3) >= data.Length) 
                                                    break;
                                                byte r = _corrCMYK(data[pp], data[pp + 3]);
                                                byte g = _corrCMYK(data[pp + 1], data[pp + 3]);
                                                byte b = _corrCMYK(data[pp + 2], data[pp + 3]);
                                                img.AddPixelRgb(b, r, g);
                                                pp += 4;
                                            }
                                            else if (colorTyp == "DeviceGray" && colorMap == null) 
                                            {
                                                if (pp < data.Length) 
                                                {
                                                    byte bb = data[pp];
                                                    img.AddPixelGray(bb);
                                                    pp += 1;
                                                }
                                            }
                                            else if (pp < data.Length) 
                                            {
                                                int b1 = (int)data[pp];
                                                if (bpiVal == 4) 
                                                {
                                                    if (colorMap != null) 
                                                    {
                                                        img.AddPixelIndex((byte)((((b1 >> 4)) & 0xF)));
                                                        img.AddPixelIndex((byte)((b1 & 0xF)));
                                                        pp++;
                                                    }
                                                    else if ((pp + 2) < data.Length) 
                                                    {
                                                        int b2 = (int)data[pp + 1];
                                                        int b3 = (int)data[pp + 2];
                                                        img.AddPixelRgb((byte)b3, (byte)b1, (byte)b2);
                                                        img.AddPixelRgb((byte)b3, (byte)b1, (byte)b2);
                                                        pp += 3;
                                                    }
                                                }
                                                else 
                                                {
                                                    img.AddPixelIndex((byte)b1);
                                                    pp += 1;
                                                }
                                            }
                                        }
                                    img.EndRow();
                                }
                                img.Commit();
                                Content = img.GetBytes();
                                return;
                            }
                        }
                    }
                }
            }
        }
        static byte _corrCMYK(byte b1, byte b2)
        {
            int i = (int)b1;
            i += ((int)b2);
            if (i >= 256) 
                i = 256;
            return (byte)((256 - i));
        }
        public byte[] Content;
        public int ImageWidth;
        public int ImageHeight;
        public override string ToString()
        {
            return string.Format("{0}: Image {1} bytes", base.ToString(), (Content == null ? "?" : Content.Length.ToString()));
        }
    }
}