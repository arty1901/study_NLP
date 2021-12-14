/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Pullenti.Unitext.Internal.Misc
{
    class EpubHelper
    {
        public static Pullenti.Unitext.UnitextDocument CreateDoc(string fileName, byte[] content, Pullenti.Unitext.CreateDocumentParam pars)
        {
            MyZipFile zip = new MyZipFile(fileName, content);
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFileName = fileName, SourceFormat = Pullenti.Unitext.FileFormat.Epub };
            doc.Content = new Pullenti.Unitext.UnitextContainer();
            try 
            {
                Dictionary<string, Pullenti.Unitext.UnitextItem> textEntries = new Dictionary<string, Pullenti.Unitext.UnitextItem>();
                List<string> itemsOrder = new List<string>();
                Dictionary<string, string> imagesEntries = new Dictionary<string, string>();
                foreach (MyZipEntry entry in zip.Entries) 
                {
                    if (entry.Name.EndsWith("content.opf", StringComparison.OrdinalIgnoreCase)) 
                    {
                        XmlDocument xml = new XmlDocument();
                        byte[] dat = entry.GetData();
                        if (dat == null) 
                            continue;
                        using (MemoryStream documentXml = new MemoryStream(dat)) 
                        {
                            xml.PreserveWhitespace = true;
                            xml.Load(documentXml);
                            documentXml.Dispose();
                        }
                        foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                        {
                            if (x.LocalName == "manifest") 
                            {
                                foreach (XmlNode xx in x.ChildNodes) 
                                {
                                    if (xx.LocalName == "item") 
                                    {
                                        string typ = null;
                                        string id = null;
                                        string href = null;
                                        if (xx.Attributes != null) 
                                        {
                                            foreach (XmlAttribute a in xx.Attributes) 
                                            {
                                                if (a.LocalName == "id") 
                                                    id = a.Value;
                                                else if (a.LocalName == "href") 
                                                    href = a.Value;
                                                else if (a.LocalName == "media-type") 
                                                    typ = a.Value;
                                            }
                                        }
                                        if (typ == null || href == null) 
                                            continue;
                                        if (typ.StartsWith("application") && typ.Contains("xhtml")) 
                                        {
                                            if (!textEntries.ContainsKey(href)) 
                                                textEntries.Add(href, null);
                                            itemsOrder.Add(href);
                                        }
                                        else if (typ.StartsWith("image/")) 
                                        {
                                            if (!imagesEntries.ContainsKey(href)) 
                                                imagesEntries.Add(href, null);
                                        }
                                    }
                                }
                            }
                            else if (x.LocalName == "metadata") 
                            {
                                foreach (XmlNode xxx in x.ChildNodes) 
                                {
                                    if (xxx.LocalName == "title") 
                                    {
                                        if (!doc.Attrs.ContainsKey("title")) 
                                            doc.Attrs.Add("title", xxx.InnerText);
                                    }
                                    else if (xxx.LocalName == "creator") 
                                    {
                                        bool isAuth = false;
                                        if (xxx.Attributes != null) 
                                        {
                                            foreach (XmlAttribute a in xxx.Attributes) 
                                            {
                                                if (a.LocalName == "role") 
                                                {
                                                    if (a.Value == "aut") 
                                                        isAuth = true;
                                                }
                                            }
                                        }
                                        if (isAuth) 
                                        {
                                            if (!doc.Attrs.ContainsKey("author")) 
                                                doc.Attrs.Add("author", xxx.InnerText);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                byte[] buf = new byte[(int)100000];
                foreach (MyZipEntry entry in zip.Entries) 
                {
                    string txt = null;
                    foreach (KeyValuePair<string, Pullenti.Unitext.UnitextItem> kp in textEntries) 
                    {
                        if (entry.Name.EndsWith(kp.Key)) 
                        {
                            txt = kp.Key;
                            break;
                        }
                    }
                    if (txt == null) 
                        continue;
                    if (txt.Contains("annotation") || txt.Contains("about") || txt.Contains("info")) 
                        continue;
                    try 
                    {
                        byte[] dat = entry.GetData();
                        Pullenti.Unitext.Internal.Html.HtmlNode nod = Pullenti.Unitext.Internal.Html.HtmlHelper.CreateNode(null, dat, null);
                        if (nod == null) 
                            continue;
                        Pullenti.Unitext.UnitextDocument doc0 = Pullenti.Unitext.Internal.Html.HtmlHelper.Create(nod, null, null, new Pullenti.Unitext.CreateDocumentParam());
                        if (doc0 != null && doc0.Content != null) 
                        {
                            if (txt.Contains("title")) 
                            {
                                doc.Sections.Add(new Pullenti.Unitext.UnitextPagesection());
                                doc.Sections[0].Header = doc0.Content;
                            }
                            else 
                                textEntries[txt] = doc0.Content;
                        }
                    }
                    catch(Exception ex) 
                    {
                    }
                }
                Pullenti.Unitext.UnitextContainer cnt = doc.Content as Pullenti.Unitext.UnitextContainer;
                foreach (string key in itemsOrder) 
                {
                    if (textEntries.ContainsKey(key)) 
                    {
                        Pullenti.Unitext.UnitextItem ccc = textEntries[key];
                        if (ccc == null) 
                            continue;
                        if (key.EndsWith("TOC.NCX", StringComparison.OrdinalIgnoreCase)) 
                            continue;
                        if (key.ToUpper().Contains("CONTENT.")) 
                            continue;
                        if (cnt.Children.Count > 0) 
                            cnt.Children.Add(new Pullenti.Unitext.UnitextPagebreak());
                        if (ccc is Pullenti.Unitext.UnitextContainer) 
                            cnt.Children.AddRange((ccc as Pullenti.Unitext.UnitextContainer).Children);
                        else 
                            cnt.Children.Add(ccc);
                    }
                }
                List<Pullenti.Unitext.UnitextItem> its = new List<Pullenti.Unitext.UnitextItem>();
                doc.GetAllItems(its, 0);
                foreach (MyZipEntry entry in zip.Entries) 
                {
                    if (pars.OnlyForPureText) 
                        break;
                    Pullenti.Unitext.UnitextImage img = null;
                    foreach (Pullenti.Unitext.UnitextItem it in its) 
                    {
                        if (it is Pullenti.Unitext.UnitextImage) 
                        {
                            if (entry.Name.EndsWith(it.Id)) 
                            {
                                img = it as Pullenti.Unitext.UnitextImage;
                                break;
                            }
                        }
                    }
                    if (img == null) 
                        continue;
                    img.Content = entry.GetData();
                }
            }
            finally
            {
                if (zip != null) 
                    zip.Dispose();
            }
            doc.Optimize(false, null);
            return doc;
        }
    }
}