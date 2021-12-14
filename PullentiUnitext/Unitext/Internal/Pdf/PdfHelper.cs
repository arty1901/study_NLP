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
using System.Xml;

namespace Pullenti.Unitext.Internal.Pdf
{
    /// <summary>
    /// Работа с PDF
    /// </summary>
    static class PdfHelper
    {
        internal static Pullenti.Unitext.UnitextDocument CreateUni(string pdfFileName, byte[] fileContent, Pullenti.Unitext.CreateDocumentParam pars)
        {
            List<Pullenti.Unitext.UnilayPage> pages = new List<Pullenti.Unitext.UnilayPage>();
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Pdf };
            try 
            {
                using (PdfFile file = new PdfFile()) 
                {
                    file.Open(pdfFileName, fileContent);
                    if (file.Encrypt != null) 
                        doc.ErrorMessage = "Can't extract pages from encrypted pdf";
                    if (file.RootObject != null) 
                    {
                        PdfDictionary mt = file.RootObject.GetDictionary("Metadata", null);
                        if (mt != null) 
                        {
                            byte[] dat = mt.ExtractData();
                            if (dat != null) 
                            {
                                try 
                                {
                                    string str = Encoding.UTF8.GetString(dat);
                                    int i = str.IndexOf("?>");
                                    if (i > 0) 
                                        str = str.Substring(i + 2).Trim();
                                    XmlDocument xml = new XmlDocument();
                                    xml.LoadXml(str);
                                    _readMetadata0(doc, xml.DocumentElement, null);
                                }
                                catch(Exception ex119) 
                                {
                                }
                            }
                        }
                    }
                    if (file.Info != null) 
                    {
                        string str = file.Info.GetStringItem("Title");
                        if (!string.IsNullOrEmpty(str) && !doc.Attrs.ContainsKey("title")) 
                            doc.Attrs.Add("title", str);
                        str = file.Info.GetStringItem("Author");
                        if (!string.IsNullOrEmpty(str) && !doc.Attrs.ContainsKey("author")) 
                            doc.Attrs.Add("author", str);
                    }
                    foreach (PdfDictionary pdic in file.Pages) 
                    {
                        PdfPage pPage = new PdfPage(pdic);
                        Pullenti.Unitext.UnilayPage up = new Pullenti.Unitext.UnilayPage();
                        up.Width = (int)pPage.Width;
                        up.Height = (int)pPage.Height;
                        pages.Add(up);
                        up.Number = pages.Count;
                        foreach (PdfRect it in pPage.Items) 
                        {
                            if (it is PdfFig) 
                                continue;
                            Pullenti.Unitext.UnilayRectangle r = new Pullenti.Unitext.UnilayRectangle();
                            r.Left = it.Left;
                            r.Top = it.Top;
                            r.Right = it.Right;
                            r.Bottom = it.Bottom;
                            r.Page = up;
                            if (it is PdfText) 
                                r.Text = (it as PdfText).Text;
                            else if (it is PdfImage) 
                            {
                                r.ImageContent = (it as PdfImage).Content;
                                r.Tag = it;
                            }
                            up.Rects.Add(r);
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                doc.ErrorMessage = ex.Message;
                return doc;
            }
            if (pages == null || pages.Count == 0 || doc.ErrorMessage != null) 
            {
                if (doc.ErrorMessage == null) 
                    doc.ErrorMessage = "Can't extract pages from pdf-file";
                return doc;
            }
            doc.Pages = pages;
            Pullenti.Unitext.Internal.Uni.UnilayoutHelper.CreateContentFromPages(doc, false);
            Pullenti.Unitext.UnitextContainer cnt = doc.Content as Pullenti.Unitext.UnitextContainer;
            if (cnt == null) 
                return doc;
            for (int i = 0; i < cnt.Children.Count; i++) 
            {
                Pullenti.Unitext.UnitextPlaintext pt = cnt.Children[i] as Pullenti.Unitext.UnitextPlaintext;
                if (pt == null) 
                    continue;
                if (!pt.IsWhitespaces) 
                    continue;
                if (i == 0 || (cnt.Children[i - 1] is Pullenti.Unitext.UnitextNewline) || (cnt.Children[i - 1] is Pullenti.Unitext.UnitextPagebreak)) 
                {
                }
                else 
                    continue;
                if ((i + 1) == cnt.Children.Count || (cnt.Children[i + 1] is Pullenti.Unitext.UnitextNewline) || (cnt.Children[i + 1] is Pullenti.Unitext.UnitextPagebreak)) 
                {
                }
                else 
                    continue;
                cnt.Children.RemoveAt(i);
                i--;
            }
            doc.Content = doc.Content.Optimize(true, null);
            return doc;
        }
        static void _readMetadata0(Pullenti.Unitext.UnitextDocument doc, XmlNode xml, string typ)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "title") 
                    _readMetadata0(doc, x, "title");
                else if (x.LocalName == "creator") 
                    _readMetadata0(doc, x, "author");
                else if (x.LocalName == "subject") 
                    _readMetadata0(doc, x, "subject");
                else if (x.LocalName == "Keywords") 
                {
                    string val = x.InnerText;
                    if (!string.IsNullOrEmpty(val) && !doc.Attrs.ContainsKey("keywords")) 
                        doc.Attrs.Add("keywords", val);
                }
                else if (typ != null && x.LocalName == "li" && !string.IsNullOrEmpty(x.InnerText)) 
                {
                    string txt = x.InnerText;
                    if (!doc.Attrs.ContainsKey(typ)) 
                        doc.Attrs.Add(typ, txt);
                    else 
                        doc.Attrs[typ] = string.Format("{0}; {1}", doc.Attrs[typ], txt);
                }
                else 
                    _readMetadata0(doc, x, typ);
            }
        }
    }
}