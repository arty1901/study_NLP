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
    static class Fb2Helper
    {
        public static Pullenti.Unitext.UnitextDocument CreateDocZip(string fileName, byte[] content, Pullenti.Unitext.CreateDocumentParam pars)
        {
            MyZipFile zip = new MyZipFile(fileName, content);
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFileName = fileName, SourceFormat = Pullenti.Unitext.FileFormat.Epub };
            doc.Content = new Pullenti.Unitext.UnitextContainer();
            Dictionary<string, Pullenti.Unitext.UnitextItem> footNotes = new Dictionary<string, Pullenti.Unitext.UnitextItem>();
            string coverImage = null;
            try 
            {
                foreach (MyZipEntry entry in zip.Entries) 
                {
                    if (entry.Name.EndsWith("body.xml", StringComparison.OrdinalIgnoreCase)) 
                    {
                        byte[] dat = entry.GetData();
                        if (dat == null) 
                            continue;
                        XmlDocument xml = new XmlDocument();
                        using (MemoryStream documentXml = new MemoryStream(dat)) 
                        {
                            xml.PreserveWhitespace = true;
                            xml.Load(documentXml);
                            documentXml.Dispose();
                        }
                        string img = _loadBody(xml.DocumentElement, doc, footNotes, pars);
                        if (img != null) 
                            coverImage = img;
                    }
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
        public static Pullenti.Unitext.UnitextDocument CreateDoc(string fileName, byte[] content, Pullenti.Unitext.CreateDocumentParam pars)
        {
            XmlDocument xml = new XmlDocument();
            if (content == null && File.Exists(fileName)) 
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) 
                {
                    xml.Load(fs);
                }
            }
            else if (content != null) 
            {
                using (MemoryStream mem = new MemoryStream(content)) 
                {
                    try 
                    {
                        xml.Load(mem);
                    }
                    catch(Exception ex) 
                    {
                        if (xml.DocumentElement == null) 
                            throw ex;
                    }
                }
            }
            else 
                return null;
            Dictionary<string, Pullenti.Unitext.UnitextItem> footNotes = new Dictionary<string, Pullenti.Unitext.UnitextItem>();
            foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
            {
                if (x.LocalName == "body") 
                {
                    string name = null;
                    if (x.Attributes != null) 
                    {
                        foreach (XmlAttribute a in x.Attributes) 
                        {
                            if (a.LocalName == "name") 
                                name = a.InnerText;
                        }
                    }
                    if (name != "notes") 
                        continue;
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        if (xx.LocalName == "section") 
                        {
                            string id = null;
                            if (xx.Attributes != null) 
                            {
                                foreach (XmlAttribute a in xx.Attributes) 
                                {
                                    if (a.LocalName == "id") 
                                        id = a.InnerText;
                                }
                            }
                            if (id == null || footNotes.ContainsKey(id)) 
                                continue;
                            Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            _GetUniText(xx, gen, null, null, pars);
                            footNotes.Add(id, gen.Finish(true, null));
                        }
                    }
                }
            }
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFileName = fileName, SourceFormat = Pullenti.Unitext.FileFormat.Fb2 };
            string coverImage = null;
            foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
            {
                if (x.LocalName == "body") 
                {
                    string img = _loadBody(x, doc, footNotes, pars);
                    if (img != null) 
                        coverImage = img;
                    break;
                }
                else if (x.LocalName == "description") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        if (xx.LocalName == "title-info") 
                        {
                            foreach (XmlNode xxx in xx.ChildNodes) 
                            {
                                if (xxx.LocalName == "book-title") 
                                {
                                    Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                    _GetUniText(xxx, gen, null, null, pars);
                                    if (!pars.LoadDocumentStructure) 
                                    {
                                        Pullenti.Unitext.UnitextItem fi = gen.Finish(true, null);
                                        if (fi != null) 
                                        {
                                            doc.Sections.Add(new Pullenti.Unitext.UnitextPagesection());
                                            doc.Sections[0].Header = fi;
                                        }
                                    }
                                    if (!doc.Attrs.ContainsKey("title")) 
                                        doc.Attrs.Add("title", xxx.InnerText);
                                }
                                else if (xxx.LocalName == "author") 
                                {
                                    foreach (XmlNode xxxx in xxx.ChildNodes) 
                                    {
                                        if (xxxx.LocalName == "first-name") 
                                        {
                                            if (!doc.Attrs.ContainsKey("author-firstname")) 
                                                doc.Attrs.Add("author-firstname", xxxx.InnerText);
                                        }
                                        else if (xxxx.LocalName == "last-name") 
                                        {
                                            if (!doc.Attrs.ContainsKey("author-lastname")) 
                                                doc.Attrs.Add("author-lastname", xxxx.InnerText);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    coverImage = LoadTitleInfo(doc.Attrs, x, null);
                }
            }
            if (!pars.OnlyForPureText) 
            {
                foreach (XmlNode x in xml.DocumentElement.ChildNodes) 
                {
                    if (x.LocalName == "binary") 
                    {
                        string id = null;
                        if (x.Attributes != null) 
                        {
                            foreach (XmlAttribute a in x.Attributes) 
                            {
                                if (a.LocalName == "id") 
                                    id = a.InnerText;
                            }
                        }
                        if (id == null) 
                            continue;
                        Pullenti.Unitext.UnitextImage img = doc.FindById(id) as Pullenti.Unitext.UnitextImage;
                        if (img != null) 
                        {
                            try 
                            {
                                img.Content = Convert.FromBase64String(x.InnerText);
                            }
                            catch(Exception ex53) 
                            {
                            }
                        }
                    }
                }
            }
            return doc;
        }
        static string _loadBody(XmlNode x, Pullenti.Unitext.UnitextDocument doc, Dictionary<string, Pullenti.Unitext.UnitextItem> footNotes, Pullenti.Unitext.CreateDocumentParam pars)
        {
            string coverImage = null;
            if (pars.LoadDocumentStructure) 
            {
                Pullenti.Unitext.UnitextDocblock dbl = new Pullenti.Unitext.UnitextDocblock();
                dbl.Typname = "Document";
                dbl.Body = new Pullenti.Unitext.UnitextContainer();
                Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                _GetUniText(x, gg, dbl, footNotes, pars);
                doc.Content = dbl;
                foreach (XmlNode xx in x.ChildNodes) 
                {
                    if (xx.LocalName == "title") 
                    {
                        Pullenti.Unitext.Internal.Uni.UnitextGen g = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        _GetUniText(xx, g, null, footNotes, pars);
                        dbl.Head = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Head };
                        Pullenti.Unitext.UnitextItem nn = g.Finish(true, null);
                        Pullenti.Unitext.UnitextContainer ccc = nn as Pullenti.Unitext.UnitextContainer;
                        if (ccc == null) 
                        {
                            ccc = new Pullenti.Unitext.UnitextContainer();
                            ccc.Children.Add(nn);
                        }
                        ccc.Typ = Pullenti.Unitext.UnitextContainerType.Name;
                        dbl.Head.Children.Add(ccc);
                        dbl.Head.Children.Add(new Pullenti.Unitext.UnitextNewline());
                        break;
                    }
                }
                if (coverImage != null) 
                {
                    if (coverImage.StartsWith("#")) 
                        coverImage = coverImage.Substring(1);
                    if (dbl.Head == null) 
                        dbl.Head = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Head };
                    dbl.Head.Children.Insert(0, new Pullenti.Unitext.UnitextImage() { Id = coverImage });
                    dbl.Head.Children.Insert(1, new Pullenti.Unitext.UnitextNewline());
                }
            }
            else 
            {
                Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                _GetUniText(x, gen, null, footNotes, pars);
                doc.Content = gen.Finish(true, null);
            }
            return coverImage;
        }
        static void _GetUniText(XmlNode xml, Pullenti.Unitext.Internal.Uni.UnitextGen gen, Pullenti.Unitext.UnitextDocblock dbl, Dictionary<string, Pullenti.Unitext.UnitextItem> footNotes, Pullenti.Unitext.CreateDocumentParam pars)
        {
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "#text") 
                {
                    if (gen != null) 
                        gen.AppendText(x.InnerText, false);
                }
                else if (x.LocalName == "p" || x.LocalName == "v") 
                {
                    gen.AppendNewline(true);
                    _GetUniText(x, gen, dbl, footNotes, pars);
                    gen.AppendNewline(false);
                }
                else if (x.LocalName == "title") 
                {
                    if (footNotes == null || pars.LoadDocumentStructure) 
                        continue;
                    gen.AppendNewline(false);
                    _GetUniText(x, gen, dbl, footNotes, pars);
                    gen.AppendNewline(false);
                    gen.AppendNewline(false);
                }
                else if (x.LocalName == "section") 
                {
                    if (!pars.LoadDocumentStructure) 
                    {
                        gen.AppendNewline(false);
                        _GetUniText(x, gen, null, footNotes, pars);
                        gen.AppendPagebreak();
                    }
                    else 
                    {
                        Pullenti.Unitext.UnitextDocblock db = new Pullenti.Unitext.UnitextDocblock() { Typname = "Section" };
                        if (dbl.Typname == "Section") 
                            db.Typname = "Subsection";
                        else if (dbl.Typname == "Subsection") 
                            db.Typname = "Chapter";
                        db.Body = new Pullenti.Unitext.UnitextContainer();
                        (dbl.Body as Pullenti.Unitext.UnitextContainer).Children.Add(db);
                        foreach (XmlNode xx in x.ChildNodes) 
                        {
                            if (xx.LocalName == "title") 
                            {
                                Pullenti.Unitext.Internal.Uni.UnitextGen g = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                _GetUniText(xx, g, null, footNotes, pars);
                                db.Head = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Head };
                                Pullenti.Unitext.UnitextItem nn = g.Finish(true, null);
                                Pullenti.Unitext.UnitextContainer ccc = nn as Pullenti.Unitext.UnitextContainer;
                                if (ccc == null) 
                                {
                                    ccc = new Pullenti.Unitext.UnitextContainer();
                                    ccc.Children.Add(nn);
                                }
                                ccc.Typ = Pullenti.Unitext.UnitextContainerType.Name;
                                db.Head.Children.Add(ccc);
                                db.Head.Children.Add(new Pullenti.Unitext.UnitextNewline());
                                break;
                            }
                        }
                        Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        _GetUniText(x, gg, db, footNotes, pars);
                        Pullenti.Unitext.UnitextItem bb = gg.Finish(false, null);
                        if (bb == null) 
                            continue;
                        if ((db.Body as Pullenti.Unitext.UnitextContainer).Children.Count == 0) 
                            db.Body = bb;
                        else 
                            (db.Body as Pullenti.Unitext.UnitextContainer).Children.Insert(0, bb);
                        Pullenti.Unitext.UnitextContainer cnt = db.Body as Pullenti.Unitext.UnitextContainer;
                        if (cnt != null && cnt.Children.Count > 0 && (cnt.Children[0] is Pullenti.Unitext.UnitextNewline)) 
                            cnt.Children.RemoveAt(0);
                    }
                }
                else if (x.LocalName == "empty-line") 
                    gen.AppendNewline(false);
                else if (x.LocalName == "image") 
                {
                    string id = null;
                    if (x.Attributes != null) 
                    {
                        foreach (XmlAttribute a in x.Attributes) 
                        {
                            if (a.LocalName == "href" && !string.IsNullOrEmpty(a.Value) && a.Value[0] == '#') 
                            {
                                id = a.Value.Substring(1);
                                break;
                            }
                        }
                    }
                    if (id != null) 
                        gen.Append(new Pullenti.Unitext.UnitextImage() { Id = id }, null, -1, false);
                }
                else if (x.LocalName == "a") 
                {
                    string id = null;
                    string typ = null;
                    if (x.Attributes != null) 
                    {
                        foreach (XmlAttribute a in x.Attributes) 
                        {
                            if (a.LocalName == "href" && !string.IsNullOrEmpty(a.Value) && a.Value[0] == '#') 
                                id = a.Value.Substring(1);
                            else if (a.LocalName == "type") 
                                typ = a.Value;
                        }
                    }
                    if ((id != null && typ == "note" && footNotes != null) && footNotes.ContainsKey(id)) 
                    {
                        Pullenti.Unitext.UnitextFootnote fn = new Pullenti.Unitext.UnitextFootnote() { CustomMark = x.InnerText };
                        fn.Content = footNotes[id];
                        gen.Append(fn, null, -1, false);
                        continue;
                    }
                    gen.AppendText(x.InnerText, false);
                }
                else if (x.LocalName == "subtitle") 
                    gen.AppendNewline(false);
                else if (x.LocalName == "epigraph") 
                {
                    Pullenti.Unitext.Internal.Uni.UnitextGen g = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    _GetUniText(x, g, null, null, pars);
                    Pullenti.Unitext.UnitextFootnote fn = new Pullenti.Unitext.UnitextFootnote();
                    fn.Content = g.Finish(true, null);
                    if (fn.Content != null) 
                        gen.Append(fn, null, -1, false);
                }
                else 
                    _GetUniText(x, gen, dbl, footNotes, pars);
            }
        }
        public static string LoadTitleInfo(Dictionary<string, string> attrs, XmlNode xml, string nam)
        {
            string ret = null;
            foreach (XmlNode ch in xml.ChildNodes) 
            {
                if (ch.LocalName == "image" && xml.LocalName == "coverpage") 
                {
                    foreach (XmlAttribute a in ch.Attributes) 
                    {
                        if (a.LocalName == "href") 
                            ret = a.Value;
                    }
                    continue;
                }
                string key = (nam == null ? ch.LocalName : (ch.LocalName == "#text" ? nam : string.Format("{0}/{1}", nam, ch.LocalName)));
                if (ch.ChildNodes.Count > 0) 
                {
                    string img = LoadTitleInfo(attrs, ch, key);
                    if (img != null) 
                        ret = img;
                    continue;
                }
                string val = ch.InnerText;
                if (string.IsNullOrEmpty(val)) 
                    continue;
                if (!attrs.ContainsKey(key)) 
                    attrs.Add(key, val);
                else 
                    attrs[key] = string.Format("{0}; {1}", attrs[key], val);
            }
            return ret;
        }
    }
}