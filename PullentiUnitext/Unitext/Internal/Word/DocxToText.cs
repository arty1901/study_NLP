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
using System.Text;
using System.Xml;

namespace Pullenti.Unitext.Internal.Word
{
    class DocxToText : IDisposable
    {
        public DocxToText(string fileName, byte[] content, bool isXml)
        {
            if (isXml) 
            {
                xmlFile = new XmlDocument();
                if (content != null) 
                {
                    using (MemoryStream mem = new MemoryStream(content)) 
                    {
                        xmlFile.Load(mem);
                    }
                }
                else 
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) 
                    {
                        xmlFile.Load(fs);
                    }
                }
                this._prepareXml();
            }
            else 
            {
                zipFile = new Pullenti.Unitext.Internal.Misc.MyZipFile(fileName, content);
                this._prepareZip();
            }
        }
        void _prepareZip()
        {
            foreach (Pullenti.Unitext.Internal.Misc.MyZipEntry o in zipFile.Entries) 
            {
                if (o.IsDirectory || o.Encrypted || o.UncompressDataSize == 0) 
                    continue;
                Parts.Add(new DocxPart() { Name = o.Name, ZipEntry = o });
            }
        }
        void _prepareXml()
        {
            if (xmlFile.DocumentElement.LocalName == "wordDocument") 
            {
                Parts.Add(new DocxPart() { Name = "word/document.xml", Xml = xmlFile.DocumentElement });
                return;
            }
            foreach (XmlNode xml in xmlFile.DocumentElement.ChildNodes) 
            {
                if (xml.LocalName == "part") 
                {
                    DocxPart p = new DocxPart();
                    if (xml.Attributes != null) 
                    {
                        foreach (XmlAttribute a in xml.Attributes) 
                        {
                            if (a.LocalName == "name") 
                            {
                                p.Name = a.Value;
                                if (p.Name.StartsWith("/")) 
                                    p.Name = p.Name.Substring(1);
                                break;
                            }
                        }
                    }
                    foreach (XmlNode x in xml.ChildNodes) 
                    {
                        if (x.LocalName == "xmlData") 
                        {
                            foreach (XmlNode xx in x.ChildNodes) 
                            {
                                p.Xml = xx;
                                break;
                            }
                            break;
                        }
                        else if (x.LocalName == "binaryData") 
                        {
                            try 
                            {
                                p.Data = Convert.FromBase64String(x.InnerText);
                            }
                            catch(Exception ex) 
                            {
                            }
                            break;
                        }
                    }
                    if (p.Name != null && ((p.Xml != null || p.Data != null))) 
                        Parts.Add(p);
                }
            }
        }
        public void Dispose()
        {
            try 
            {
                if (zipFile != null) 
                {
                    zipFile.Dispose();
                    zipFile = null;
                }
            }
            catch(Exception ex) 
            {
            }
        }
        Pullenti.Unitext.Internal.Misc.MyZipFile zipFile = null;
        XmlDocument xmlFile = null;
        List<DocxPart> Parts = new List<DocxPart>();
        Dictionary<string, string> m_Hyperlinks = new Dictionary<string, string>();
        Dictionary<string, string> m_DataControls = new Dictionary<string, string>();
        public DocTextStyles m_Styles = new DocTextStyles();
        public DocNumStyles m_NumStyles = new DocNumStyles();
        List<DocSection> m_Sections = new List<DocSection>();
        Pullenti.Unitext.UnitextPagesection m_CurSection;
        Dictionary<string, string> m_Collontituls = new Dictionary<string, string>();
        public Pullenti.Unitext.UnitextDocument CreateUniDoc(bool onlyForPureText, Pullenti.Unitext.FileFormat frm, Pullenti.Unitext.CreateDocumentParam pars)
        {
            Dictionary<string, string> sheets = new Dictionary<string, string>();
            Dictionary<string, string> idImages = new Dictionary<string, string>();
            Dictionary<string, string> idEmbeds = new Dictionary<string, string>();
            List<Pullenti.Unitext.UnitextItem> sharedStrings = new List<Pullenti.Unitext.UnitextItem>();
            Dictionary<string, Pullenti.Unitext.Internal.Misc.BorderInfo> cellBorders = new Dictionary<string, Pullenti.Unitext.Internal.Misc.BorderInfo>();
            Dictionary<int, Pullenti.Unitext.UnitextItem> pptSlides = new Dictionary<int, Pullenti.Unitext.UnitextItem>();
            Dictionary<int, Dictionary<string, string>> pptImages = new Dictionary<int, Dictionary<string, string>>();
            XmlNode xmlDoc = null;
            XmlNode xmlBook = null;
            XmlNode xmlComments = null;
            XmlNode xmlFootnotes = null;
            XmlNode xmlEndnotes = null;
            XmlNode xmlOdtContent = null;
            XmlNode xmlOdtStyle = null;
            foreach (DocxPart p in Parts) 
            {
                if (p.Name != null && p.Name.StartsWith("word/theme/")) 
                    m_Styles.ReadTheme(p.GetXmlNode(false));
            }
            foreach (DocxPart p in Parts) 
            {
                if (p.IsName("word/styles.xml")) 
                    m_Styles.ReadAllStyles(p.GetXmlNode(false));
            }
            foreach (DocxPart p in Parts) 
            {
                if (p.IsName("word/document.xml")) 
                    xmlDoc = p.GetXmlNode(false);
                else if (p.IsName("word/footnotes.xml")) 
                    xmlFootnotes = p.GetXmlNode(false);
                else if (p.IsName("word/endnotes.xml")) 
                    xmlEndnotes = p.GetXmlNode(false);
                else if (p.IsName("word/styles.xml")) 
                {
                }
                else if (p.IsName("word/comments.xml")) 
                {
                    xmlComments = p.GetXmlNode(false);
                    if (xmlComments != null) 
                    {
                        foreach (XmlNode x in xmlComments.ChildNodes) 
                        {
                            if (x.LocalName == "comment") 
                            {
                                Pullenti.Unitext.UnitextComment cmt = new Pullenti.Unitext.UnitextComment();
                                string id = null;
                                if (x.Attributes != null) 
                                {
                                    foreach (XmlAttribute a in x.Attributes) 
                                    {
                                        if (a.LocalName == "id") 
                                            id = a.Value;
                                        else if (a.LocalName == "author") 
                                            cmt.Author = a.Value;
                                    }
                                }
                                if (id == null || m_Comments.ContainsKey(id)) 
                                    continue;
                                Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                List<XmlNode> xxx = new List<XmlNode>();
                                xxx.Add(x);
                                m_Styles.Ignore = true;
                                this.ReadNode(xxx, gen, null, -1);
                                m_Styles.Ignore = false;
                                Pullenti.Unitext.UnitextItem it = gen.Finish(true, null);
                                if (it != null) 
                                {
                                    StringBuilder tmp = new StringBuilder();
                                    it.GetPlaintext(tmp, null);
                                    if (tmp.Length > 0) 
                                    {
                                        cmt.Text = tmp.ToString();
                                        m_Comments.Add(id, cmt);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (p.IsName("content.xml")) 
                    xmlOdtContent = p.GetXmlNode(true);
                else if (p.IsName("styles.xml")) 
                    xmlOdtStyle = p.GetXmlNode(true);
                else if (p.IsName("xl/workbook.xml")) 
                    xmlBook = p.GetXmlNode(false);
                else if (p.IsName("xl/sharedStrings.xml")) 
                {
                    XmlNode xml = p.GetXmlNode(false);
                    if (xml != null) 
                    {
                        foreach (XmlNode xx in xml.ChildNodes) 
                        {
                            if (xx.LocalName == "si") 
                            {
                                Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                List<XmlNode> xxx = new List<XmlNode>();
                                xxx.Add(xx);
                                m_Styles.Ignore = true;
                                this.ReadNode(xxx, gg, null, -1);
                                m_Styles.Ignore = false;
                                sharedStrings.Add(gg.Finish(true, null));
                            }
                        }
                    }
                }
                else if (p.IsName("xl/styles.xml")) 
                {
                    XmlNode xml = p.GetXmlNode(false);
                    if (xml != null) 
                    {
                        List<Pullenti.Unitext.Internal.Misc.BorderInfo> brdr = new List<Pullenti.Unitext.Internal.Misc.BorderInfo>();
                        foreach (XmlNode xx in xml.ChildNodes) 
                        {
                            if (xx.LocalName == "borders") 
                            {
                                foreach (XmlNode xxx in xx.ChildNodes) 
                                {
                                    if (xxx.LocalName == "border") 
                                    {
                                        Pullenti.Unitext.Internal.Misc.BorderInfo brd = new Pullenti.Unitext.Internal.Misc.BorderInfo();
                                        brdr.Add(brd);
                                        foreach (XmlNode y in xxx.ChildNodes) 
                                        {
                                            if (y.Attributes.Count > 0 || y.ChildNodes.Count > 0) 
                                            {
                                                if (y.LocalName == "left") 
                                                    brd.Left = true;
                                                else if (y.LocalName == "right") 
                                                    brd.Right = true;
                                                else if (y.LocalName == "top") 
                                                    brd.Top = true;
                                                else if (y.LocalName == "bottom") 
                                                    brd.Bottom = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (xx.LocalName == "cellXfs") 
                            {
                                int nu = 0;
                                foreach (XmlNode xxx in xx.ChildNodes) 
                                {
                                    if (xxx.LocalName == "xf") 
                                    {
                                        int ind = 0;
                                        if (xxx.Attributes != null) 
                                        {
                                            foreach (XmlAttribute a in xxx.Attributes) 
                                            {
                                                if (a.LocalName == "borderId") 
                                                {
                                                    int.TryParse(a.Value, out ind);
                                                    break;
                                                }
                                            }
                                        }
                                        if (ind >= 0 && (ind < brdr.Count)) 
                                            cellBorders.Add(nu.ToString(), brdr[ind]);
                                        nu++;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (((p.IsNameStarts("word/_rels/") || p.IsNameStarts("xl/_rels/") || p.IsNameStarts("ppt/slides/_rels/"))) && ((p.ZipEntry != null && !p.ZipEntry.IsDirectory))) 
                {
                    XmlNode xmlRels = p.GetXmlNode(false);
                    if (xmlRels != null) 
                    {
                        Dictionary<string, string> pptImgs = null;
                        if (p.Name.StartsWith("ppt")) 
                        {
                            int ii = p.Name.IndexOf("rels/slide");
                            if (ii < 0) 
                                continue;
                            string nam = p.Name.Substring(ii + 10);
                            ii = nam.IndexOf('.');
                            if (ii < 0) 
                                continue;
                            if (!int.TryParse(nam.Substring(0, ii), out ii)) 
                                continue;
                            if (pptImages.ContainsKey(ii)) 
                                continue;
                            pptImgs = new Dictionary<string, string>();
                            pptImages.Add(ii, pptImgs);
                        }
                        foreach (XmlNode xx in xmlRels.ChildNodes) 
                        {
                            if (xx.Attributes != null) 
                            {
                                if (xx.Attributes["Id"] != null && xx.Attributes["Target"] != null && xx.Attributes["Type"] != null) 
                                {
                                    string id = xx.Attributes["Id"].Value;
                                    string val = xx.Attributes["Target"].Value;
                                    string typ = xx.Attributes["Type"].Value;
                                    if (typ.EndsWith("/header")) 
                                        m_Collontituls.Add(id, val);
                                    else if (typ.EndsWith("/footer")) 
                                        m_Collontituls.Add(id, val);
                                    else if (typ.EndsWith("/worksheet")) 
                                        sheets.Add(id, val);
                                    else if (typ.EndsWith("/image")) 
                                    {
                                        if (pptImgs != null) 
                                        {
                                            if (!pptImgs.ContainsKey(id)) 
                                                pptImgs.Add(id, val);
                                        }
                                        else if (!idImages.ContainsKey(id)) 
                                            idImages.Add(id, val);
                                    }
                                    else if (typ.EndsWith("/package")) 
                                    {
                                        if (!idEmbeds.ContainsKey(id)) 
                                            idEmbeds.Add(id, val);
                                    }
                                    else if (typ.EndsWith("/hyperlink")) 
                                    {
                                        if (!string.IsNullOrEmpty(val) && !m_Hyperlinks.ContainsKey(id)) 
                                            m_Hyperlinks.Add(id, val);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (p.IsName("word/numbering.xml")) 
                {
                    XmlNode xmlNum = p.GetXmlNode(false);
                    if (xmlNum != null) 
                        m_NumStyles.ReadAllStyles(xmlNum);
                }
                else if (p.IsNameStarts("ppt/slides/slide") && p.Name.EndsWith(".xml")) 
                {
                    xmlDoc = p.GetXmlNode(false);
                    if (xmlDoc == null) 
                        continue;
                    Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    List<XmlNode> xxx = new List<XmlNode>();
                    xxx.Add(xmlDoc);
                    this.ReadNode(xxx, gen, null, -1);
                    Pullenti.Unitext.UnitextItem slide = gen.Finish(true, null);
                    if (slide == null) 
                        continue;
                    string nam = p.Name.Substring("ppt/slides/slide".Length);
                    int ii = nam.IndexOf('.');
                    if (ii < 0) 
                        continue;
                    if (int.TryParse(nam.Substring(0, ii), out ii)) 
                    {
                    }
                    else 
                        ii = pptSlides.Count + 1;
                    for (; ; ii++) 
                    {
                        if (!pptSlides.ContainsKey(ii)) 
                        {
                            pptSlides.Add(ii, slide);
                            break;
                        }
                    }
                }
            }
            if (xmlOdtContent != null) 
            {
                Pullenti.Unitext.Internal.Misc.OdtHelper dh = new Pullenti.Unitext.Internal.Misc.OdtHelper();
                Pullenti.Unitext.UnitextDocument res = dh.CreateUni(xmlOdtContent, (xmlOdtStyle == null ? null : xmlOdtStyle));
                if (res == null) 
                    return null;
                List<Pullenti.Unitext.UnitextItem> its = new List<Pullenti.Unitext.UnitextItem>();
                List<string> keys = new List<string>();
                res.GetAllItems(its, 0);
                foreach (Pullenti.Unitext.UnitextItem it in its) 
                {
                    if ((it is Pullenti.Unitext.UnitextImage) && it.Id != null && !keys.Contains(it.Id.ToLower())) 
                        keys.Add(it.Id.ToLower());
                }
                if (its.Count > 0 && !onlyForPureText) 
                {
                    foreach (Pullenti.Unitext.Internal.Misc.MyZipEntry o in zipFile.Entries) 
                    {
                        string kkk = o.Name.ToLower();
                        if (!keys.Contains(kkk)) 
                            continue;
                        byte[] dat = o.GetData();
                        if (dat != null && dat.Length > 0) 
                        {
                            foreach (Pullenti.Unitext.UnitextItem it in its) 
                            {
                                if ((it is Pullenti.Unitext.UnitextImage) && it.Id != null && it.Id.ToLower() == kkk) 
                                    (it as Pullenti.Unitext.UnitextImage).Content = dat;
                            }
                        }
                    }
                }
                return res;
            }
            if (pptSlides.Count > 0) 
            {
                Pullenti.Unitext.UnitextDocument res = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Pptx, Attrs = m_DataControls };
                Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer();
                res.Content = cnt;
                foreach (KeyValuePair<int, Pullenti.Unitext.UnitextItem> kp in pptSlides) 
                {
                    if (cnt.Children.Count > 0) 
                        cnt.Children.Add(new Pullenti.Unitext.UnitextPagebreak());
                    cnt.Children.Add(kp.Value);
                    if (pptImages.ContainsKey(kp.Key)) 
                    {
                        List<Pullenti.Unitext.UnitextItem> imgs = new List<Pullenti.Unitext.UnitextItem>();
                        kp.Value.GetAllItems(imgs, 0);
                        foreach (Pullenti.Unitext.UnitextItem it in imgs) 
                        {
                            Pullenti.Unitext.UnitextImage im = it as Pullenti.Unitext.UnitextImage;
                            if (im != null && pptImages[kp.Key].ContainsKey(im.Id ?? "")) 
                            {
                                im.Id = pptImages[kp.Key][im.Id];
                                if (im.Id.StartsWith("../")) 
                                    im.Id = im.Id.Substring(3);
                            }
                        }
                    }
                }
                if (!onlyForPureText) 
                {
                    List<Pullenti.Unitext.UnitextItem> ims = new List<Pullenti.Unitext.UnitextItem>();
                    res.GetAllItems(ims, 0);
                    foreach (DocxPart p in Parts) 
                    {
                        string kkk = p.Name.ToLower();
                        byte[] dat = null;
                        foreach (Pullenti.Unitext.UnitextItem im in ims) 
                        {
                            if ((im is Pullenti.Unitext.UnitextImage) && im.Id != null) 
                            {
                                if (kkk.EndsWith(im.Id)) 
                                {
                                    if (dat == null) 
                                        dat = p.GetBytes();
                                    (im as Pullenti.Unitext.UnitextImage).Content = dat;
                                }
                            }
                        }
                    }
                }
                return res;
            }
            if (idEmbeds.Count > 0) 
            {
                foreach (DocxPart p in Parts) 
                {
                    foreach (KeyValuePair<string, string> kp in idEmbeds) 
                    {
                        if (!m_Embeds.ContainsKey(kp.Key)) 
                        {
                            if (p.Name.ToLower().EndsWith(kp.Value.ToLower())) 
                            {
                                byte[] dat = p.GetBytes();
                                if (dat != null && dat.Length > 0) 
                                {
                                    Pullenti.Unitext.UnitextDocument doc1 = Pullenti.Unitext.UnitextService.CreateDocument(p.Name, dat, null);
                                    if (doc1 != null && doc1.Content != null) 
                                        m_Embeds.Add(kp.Key, doc1.Content);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (xmlDoc != null) 
            {
                Pullenti.Unitext.UnitextDocument res = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Docx, Attrs = m_DataControls };
                Pullenti.Unitext.UnitextStyledFragment rootStyle = new Pullenti.Unitext.UnitextStyledFragment();
                rootStyle.Doc = res;
                if (m_Styles.DefStyle != null) 
                {
                    m_Styles.DefStyle = m_Styles.RegisterStyle(m_Styles.DefStyle);
                    rootStyle.Style = m_Styles.DefStyle;
                }
                m_Sections.Add(new DocSection());
                m_CurSection = m_Sections[0].USect;
                m_CurSection.Id = "ps1";
                if (xmlFootnotes != null) 
                {
                    List<XmlNode> li = new List<XmlNode>();
                    li.Add(xmlFootnotes);
                    this.ReadFootnotes(li, false, rootStyle);
                }
                if (xmlEndnotes != null) 
                {
                    List<XmlNode> li = new List<XmlNode>();
                    li.Add(xmlEndnotes);
                    this.ReadFootnotes(li, true, rootStyle);
                }
                Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                gen.SetStyle(rootStyle);
                List<XmlNode> xxx = new List<XmlNode>();
                xxx.Add(xmlDoc);
                this.ReadNode(xxx, gen, rootStyle, -1);
                Pullenti.Unitext.UnitextItem body = gen.Finish(true, null);
                if (body == null) 
                    return null;
                res.Content = body;
                res.Content.m_StyledFrag = rootStyle;
                if (m_Sections.Count > 0 && !m_Sections[m_Sections.Count - 1].Loaded) 
                    m_Sections.RemoveAt(m_Sections.Count - 1);
                m_CurSection = null;
                foreach (DocSection s in m_Sections) 
                {
                    res.Sections.Add(s.USect);
                    for (int k = 0; k < 2; k++) 
                    {
                        Pullenti.Unitext.UnitextStyledFragment sectRootStyle = new Pullenti.Unitext.UnitextStyledFragment();
                        sectRootStyle.Doc = res;
                        Pullenti.Unitext.Internal.Uni.UnitextGen g = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        g.SetStyle(sectRootStyle);
                        List<string> fnams = new List<string>();
                        List<string> nnn = (k == 0 ? s.HeadIds : s.FootIds);
                        foreach (string ii in nnn) 
                        {
                            if (m_Collontituls.ContainsKey(ii)) 
                                fnams.Add(m_Collontituls[ii]);
                        }
                        if (fnams.Count == 0) 
                            continue;
                        foreach (DocxPart p in Parts) 
                        {
                            string kkk = p.Name;
                            int ii = kkk.IndexOf('/');
                            if (ii < 0) 
                                continue;
                            kkk = kkk.Substring(ii + 1);
                            if (!fnams.Contains(kkk)) 
                                continue;
                            XmlNode xml = p.GetXmlNode(false);
                            if (xml != null) 
                            {
                                xxx.Clear();
                                xxx.Add(xml);
                                this.ReadNode(xxx, g, sectRootStyle, -1);
                            }
                        }
                        Pullenti.Unitext.UnitextItem fi = g.Finish(true, null);
                        if (fi != null) 
                        {
                            if (k == 0) 
                                s.USect.Header = fi;
                            else 
                                s.USect.Footer = fi;
                        }
                    }
                }
                res.Styles = m_Styles.UStyles;
                if (!onlyForPureText) 
                {
                    List<Pullenti.Unitext.UnitextItem> its = new List<Pullenti.Unitext.UnitextItem>();
                    res.GetAllItems(its, 0);
                    List<Pullenti.Unitext.UnitextImage> iii = new List<Pullenti.Unitext.UnitextImage>();
                    List<string> iiids = new List<string>();
                    foreach (Pullenti.Unitext.UnitextItem i in its) 
                    {
                        if ((i is Pullenti.Unitext.UnitextImage) && i.Id != null && idImages.ContainsKey(i.Id)) 
                        {
                            iii.Add(i as Pullenti.Unitext.UnitextImage);
                            i.Tag = idImages[i.Id];
                            iiids.Add(i.Tag as string);
                        }
                    }
                    if (iii.Count > 0) 
                    {
                        foreach (DocxPart p in Parts) 
                        {
                            string kkk = p.Name;
                            if (!iiids.Contains(kkk)) 
                            {
                                int ii = kkk.IndexOf('/');
                                if (ii < 0) 
                                    continue;
                                kkk = kkk.Substring(ii + 1);
                                if (!iiids.Contains(kkk)) 
                                    continue;
                            }
                            byte[] dat = p.GetBytes();
                            if (dat != null) 
                            {
                                foreach (Pullenti.Unitext.UnitextImage kp in iii) 
                                {
                                    if ((kp.Tag as string) == kkk) 
                                        kp.Content = dat;
                                }
                            }
                        }
                    }
                }
                return res;
            }
            if (xmlBook != null && zipFile != null) 
            {
                Pullenti.Unitext.UnitextDocument res = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Xlsx, Attrs = m_DataControls };
                Dictionary<string, string> books = new Dictionary<string, string>();
                foreach (XmlNode xml in xmlBook.ChildNodes) 
                {
                    if (xml.LocalName == "sheets") 
                    {
                        foreach (XmlNode xx in xml.ChildNodes) 
                        {
                            string id = null;
                            string nams = null;
                            if (xx.Attributes != null) 
                            {
                                foreach (XmlAttribute a in xx.Attributes) 
                                {
                                    if (a.LocalName == "name") 
                                        nams = a.Value;
                                    else if (a.LocalName == "id") 
                                        id = a.Value;
                                }
                            }
                            if (id != null && nams != null && !books.ContainsKey(nams)) 
                                books.Add(id, nams);
                        }
                    }
                }
                Dictionary<string, Pullenti.Unitext.UnitextItem> sss = new Dictionary<string, Pullenti.Unitext.UnitextItem>();
                foreach (Pullenti.Unitext.Internal.Misc.MyZipEntry o in zipFile.Entries) 
                {
                    string kkk = o.Name;
                    int ii = kkk.IndexOf('/');
                    if (ii < 0) 
                        continue;
                    kkk = kkk.Substring(ii + 1);
                    string id = null;
                    foreach (KeyValuePair<string, string> kp in sheets) 
                    {
                        if (kp.Value == kkk) 
                        {
                            id = kp.Key;
                            break;
                        }
                    }
                    if (id == null) 
                        continue;
                    string sheetName = null;
                    books.TryGetValue(id, out sheetName);
                    byte[] dat = o.GetData();
                    if (dat == null) 
                        continue;
                    Pullenti.Unitext.Internal.Misc.MyXmlReader xr = Pullenti.Unitext.Internal.Misc.MyXmlReader.Create(dat);
                    Pullenti.Unitext.UnitextItem cnt = Pullenti.Unitext.Internal.Misc.ExcelHelper.ReadSheet(xr, sharedStrings, cellBorders, sheetName);
                    if (cnt == null) 
                        continue;
                    sss.Add(id, cnt);
                }
                List<Pullenti.Unitext.UnitextItem> ss = new List<Pullenti.Unitext.UnitextItem>();
                foreach (KeyValuePair<string, string> kp in books) 
                {
                    if (sss.ContainsKey(kp.Key)) 
                    {
                        if (sss[kp.Key] != null) 
                            ss.Add(sss[kp.Key]);
                    }
                }
                if (ss.Count == 0) 
                    return null;
                if (ss.Count == 1) 
                    res.Content = ss[0];
                else 
                {
                    Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer();
                    for (int ii = 0; ii < ss.Count; ii++) 
                    {
                        if (ii > 0) 
                            cnt.Children.Add(new Pullenti.Unitext.UnitextPagebreak());
                        cnt.Children.Add(ss[ii]);
                    }
                    res.Content = cnt;
                }
                res.Optimize(false, pars);
                return res;
            }
            return null;
        }
        Dictionary<string, Pullenti.Unitext.UnitextComment> m_Comments = new Dictionary<string, Pullenti.Unitext.UnitextComment>();
        Dictionary<string, Pullenti.Unitext.UnitextItem> m_Embeds = new Dictionary<string, Pullenti.Unitext.UnitextItem>();
        Dictionary<string, Pullenti.Unitext.UnitextItem> m_Footnotes = new Dictionary<string, Pullenti.Unitext.UnitextItem>();
        private void ReadFootnotes(List<XmlNode> stackList, bool end, Pullenti.Unitext.UnitextStyledFragment rootStyle)
        {
            XmlNode node = stackList[stackList.Count - 1];
            foreach (XmlNode xml in node.ChildNodes) 
            {
                if (xml.LocalName == "footnote" || xml.LocalName == "endnote") 
                {
                    string id = null;
                    if (xml.Attributes != null) 
                    {
                        foreach (XmlAttribute a in xml.Attributes) 
                        {
                            if (a.LocalName == "id") 
                            {
                                id = a.Value;
                                if (end) 
                                    id = "end" + id;
                                break;
                            }
                        }
                    }
                    if (id == null || m_Footnotes.ContainsKey(id)) 
                        continue;
                    Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    gen.SetStyle(rootStyle);
                    stackList.Add(xml);
                    this.ReadNode(stackList, gen, rootStyle, -1);
                    stackList.RemoveAt(stackList.Count - 1);
                    Pullenti.Unitext.UnitextItem fi = gen.Finish(true, null);
                    if (fi != null) 
                        m_Footnotes.Add(id, fi);
                }
            }
        }
        private string m_lastRSID = "";
        private char m_lastChar = ' ';
        internal void ReadNode(List<XmlNode> stackNodes, Pullenti.Unitext.Internal.Uni.UnitextGen gen, Pullenti.Unitext.UnitextStyledFragment sfrag, int pictTop)
        {
            if (stackNodes.Count == 0) 
                return;
            XmlNode node = stackNodes[stackNodes.Count - 1];
            foreach (XmlNode child in node.ChildNodes) 
            {
                if (child.LocalName == "Fallback") 
                {
                    stackNodes.Add(child);
                    this.ReadNode(stackNodes, gen, sfrag, pictTop);
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    return;
                }
            }
            string delText = null;
            bool proofErr = false;
            for (int i = 0; i < node.ChildNodes.Count; i++) 
            {
                XmlNode child = node.ChildNodes[i];
                string locname = child.LocalName;
                if (string.IsNullOrEmpty(locname)) 
                    continue;
                if (locname == "#text") 
                    continue;
                if (proofErr) 
                    continue;
                switch (locname) { 
                case "del":
                    break;
                case "delText":
                    break;
                case "t":
                    string text = child.InnerText;
                    if (string.IsNullOrEmpty(text) || text == delText) 
                        m_lastChar = ' ';
                    else 
                    {
                        if (delText != null) 
                        {
                            if (text.StartsWith(delText)) 
                                text = text.Substring(delText.Length);
                        }
                        if (sfrag != null && sfrag.Style != null && sfrag.Style.GetAttr("upper-case") == "true") 
                            text = text.ToUpper();
                        m_lastChar = text[text.Length - 1];
                        gen.AppendText(text, false);
                        if (text.StartsWith("Дого")) 
                        {
                        }
                    }
                    delText = null;
                    break;
                case "sym":
                    string ch = null;
                    if (child.Attributes != null) 
                    {
                        string font = null;
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "font") 
                            {
                                font = a.Value;
                                break;
                            }
                        }
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "char") 
                            {
                                int nn = 0;
                                for (int jj = 0; jj < a.Value.Length; jj++) 
                                {
                                    int dig = (int)a.Value[jj];
                                    if (dig >= 0x30 && dig <= 0x39) 
                                        nn = ((nn * 16) + dig) - 0x30;
                                    else if (dig >= 0x41 && dig <= 0x46) 
                                        nn = (((nn * 16) + dig) - 0x41) + 10;
                                    else if (dig >= 0x61 && dig <= 0x66) 
                                        nn = (((nn * 16) + dig) - 0x61) + 10;
                                }
                                if (a.Value[0] == 'F') 
                                    nn -= 0xF000;
                                char uch = (char)0;
                                if (string.Compare(font ?? "", "Symbol", true) == 0) 
                                    uch = (char)nn;
                                else 
                                    uch = Pullenti.Unitext.Internal.Misc.WingdingsHelper.GetUnicode(nn);
                                if (uch == ((char)0)) 
                                    ch = " ";
                                else 
                                    ch = string.Format("{0}", uch);
                            }
                        }
                    }
                    if (ch != null) 
                    {
                        gen.AppendText(ch, false);
                        m_lastChar = ch[0];
                    }
                    break;
                case "cr":
                    gen.AppendNewline(false);
                    break;
                case "commentRangeStart":
                    if (child.Attributes != null) 
                    {
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "id") 
                            {
                                Pullenti.Unitext.UnitextComment cmt;
                                if (m_Comments.TryGetValue(a.Value ?? "", out cmt)) 
                                {
                                    if (cmt.Id == null) 
                                        cmt.Id = "comment" + a.Value;
                                    gen.Append(cmt, null, -1, false);
                                }
                            }
                        }
                    }
                    break;
                case "commentRangeEnd":
                    if (child.Attributes != null) 
                    {
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "id") 
                            {
                                Pullenti.Unitext.UnitextComment cmt;
                                if (m_Comments.TryGetValue(a.Value ?? "", out cmt)) 
                                {
                                    Pullenti.Unitext.UnitextComment ecmt = new Pullenti.Unitext.UnitextComment() { Id = cmt.Id + "_end", TwinId = cmt.Id, IsEndOfComment = true };
                                    cmt.TwinId = ecmt.Id;
                                    ecmt.Text = cmt.Text;
                                    ecmt.Author = cmt.Author;
                                    gen.Append(ecmt, null, -1, false);
                                    m_Comments.Remove(a.Value);
                                }
                            }
                        }
                    }
                    break;
                case "br":
                    string val = null;
                    if (child.Attributes != null && child.Attributes.Count > 0 && child.Attributes[0].Name.Contains("type")) 
                        val = child.Attributes[0].Value;
                    if (val == "page") 
                        gen.AppendPagebreak();
                    else 
                    {
                        bool isNewLine = false;
                        if (gen.LastNotSpaceChar == ':' || gen.LastNotSpaceChar == '.') 
                            isNewLine = true;
                        if (isNewLine) 
                            gen.AppendNewline(false);
                        else 
                            gen.AppendNewline(false);
                    }
                    break;
                case "pict":
                    Pullenti.Unitext.UnitextImage img1 = null;
                    foreach (XmlNode x in child.ChildNodes) 
                    {
                        if (x.LocalName == "binData") 
                        {
                            if (img1 == null) 
                                img1 = new Pullenti.Unitext.UnitextImage();
                            try 
                            {
                                img1.Content = Convert.FromBase64String(x.InnerText);
                            }
                            catch(Exception ex) 
                            {
                            }
                            gen.Append(img1, null, -1, false);
                        }
                        else if (x.LocalName == "shape" && x.Attributes != null) 
                        {
                            foreach (XmlAttribute a in x.Attributes) 
                            {
                                if (a.LocalName == "style" && a.Value != null) 
                                {
                                    if (img1 == null) 
                                        img1 = new Pullenti.Unitext.UnitextImage();
                                    _setImageSize(img1, a.Value);
                                }
                            }
                        }
                    }
                    if (img1 != null && img1.Content != null) 
                        return;
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    gg.AppendPagesection(m_CurSection);
                    stackNodes.Add(child);
                    this.ReadNode(stackNodes, gg, sfrag, pictTop);
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    Pullenti.Unitext.UnitextItem it = gg.Finish(true, null);
                    if (it == null) 
                        break;
                    if (it is Pullenti.Unitext.UnitextContainer) 
                        (it as Pullenti.Unitext.UnitextContainer).Typ = Pullenti.Unitext.UnitextContainerType.Shape;
                    else 
                    {
                        Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Shape };
                        cnt.Children.Add(it);
                        it = cnt.Optimize(false, null);
                    }
                    if (it != null) 
                        gen.Append(it, null, -1, false);
                    break;
                case "tab":
                    if (node.LocalName == "tabs") 
                    {
                    }
                    else 
                        gen.AppendText("\t", false);
                    break;
                case "sectPr":
                    m_Sections[m_Sections.Count - 1].Load(child);
                    DocSection se = new DocSection();
                    m_Sections.Add(se);
                    se.USect.Id = string.Format("ps{0}", m_Sections.Count);
                    m_CurSection = se.USect;
                    gen.AppendPagesection(m_CurSection);
                    break;
                case "tbl":
                    DocTable tbl = new DocTable();
                    stackNodes.Add(child);
                    tbl.Read(this, sfrag, stackNodes);
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    Pullenti.Unitext.UnitextTable tab = tbl.CreateUni();
                    if (tab != null) 
                        gen.Append(tab, null, -1, false);
                    break;
                case "fldSimple":
                    Pullenti.Unitext.Internal.Uni.UnitextGen ggg1 = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    stackNodes.Add(child);
                    this.ReadNode(stackNodes, ggg1, sfrag, pictTop);
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    Pullenti.Unitext.UnitextItem ggt = ggg1.Finish(true, null);
                    string txt = (ggt == null ? null : Pullenti.Unitext.Internal.Uni.UnitextHelper.GetPlaintext(ggt));
                    if (!string.IsNullOrEmpty(txt) && !gen.LastText.EndsWith(txt)) 
                        gen.AppendText(txt, false);
                    break;
                case "hyperlink":
                case "hlink":
                    bool ok = false;
                    if (child.Attributes != null) 
                    {
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "id") 
                            {
                                if (m_Hyperlinks.ContainsKey(a.Value)) 
                                {
                                    Pullenti.Unitext.Internal.Uni.UnitextGen ggg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                    stackNodes.Add(child);
                                    this.ReadNode(stackNodes, ggg, sfrag, pictTop);
                                    stackNodes.RemoveAt(stackNodes.Count - 1);
                                    Pullenti.Unitext.UnitextItem cnt = ggg.Finish(true, null);
                                    if (cnt != null) 
                                    {
                                        if (m_Hyperlinks.ContainsKey(a.Value ?? "")) 
                                        {
                                            try 
                                            {
                                                Pullenti.Unitext.UnitextHyperlink hr = new Pullenti.Unitext.UnitextHyperlink() { Href = m_Hyperlinks[a.Value].ToString() };
                                                hr.Content = cnt;
                                                gen.Append(hr, null, -1, false);
                                            }
                                            catch(Exception xx) 
                                            {
                                                gen.Append(cnt, null, -1, false);
                                            }
                                        }
                                        else 
                                            gen.Append(cnt, null, -1, false);
                                        ok = true;
                                        break;
                                    }
                                }
                            }
                            else if (a.LocalName == "dest") 
                            {
                                Pullenti.Unitext.Internal.Uni.UnitextGen ggg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                                ggg.AppendPagesection(m_CurSection);
                                stackNodes.Add(child);
                                this.ReadNode(stackNodes, ggg, sfrag, pictTop);
                                stackNodes.RemoveAt(stackNodes.Count - 1);
                                Pullenti.Unitext.UnitextItem cnt = ggg.Finish(true, null);
                                if (cnt != null) 
                                {
                                    Pullenti.Unitext.UnitextHyperlink hr = new Pullenti.Unitext.UnitextHyperlink() { Href = a.Value };
                                    hr.Content = cnt;
                                    gen.Append(hr, null, -1, false);
                                    ok = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!ok) 
                    {
                        stackNodes.Add(child);
                        this.ReadNode(stackNodes, gen, sfrag, pictTop);
                        stackNodes.RemoveAt(stackNodes.Count - 1);
                    }
                    break;
                case "p":
                    Pullenti.Unitext.Internal.Uni.IUnitextGenNumStyle numStyle = null;
                    int lev = 0;
                    DocTextStyle numTxtStyle = null;
                    Pullenti.Unitext.UnitextStyledFragment para = null;
                    if (sfrag != null) 
                    {
                        para = new Pullenti.Unitext.UnitextStyledFragment() { Parent = sfrag, Typ = Pullenti.Unitext.UnitextStyledFragmentType.Paragraph };
                        sfrag.Children.Add(para);
                    }
                    Pullenti.Unitext.Internal.Uni.UnitextGen pgen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    pgen.SetStyle(para);
                    pgen.AppendPagesection(m_CurSection);
                    foreach (XmlNode xx in child.ChildNodes) 
                    {
                        if (xx.LocalName == "pPr") 
                        {
                            if (para != null) 
                            {
                                Pullenti.Unitext.UnitextStyle ust0 = new Pullenti.Unitext.UnitextStyle();
                                m_Styles.ReadUnitextStyle(xx, ust0);
                                ust0.RemoveInheritAttrs(para.Parent);
                                para.Style = m_Styles.RegisterStyle(ust0);
                            }
                            string id = null;
                            foreach (XmlNode xxx in xx.ChildNodes) 
                            {
                                if (xxx.LocalName == "numPr") 
                                {
                                    foreach (XmlNode chh in xxx.ChildNodes) 
                                    {
                                        if (chh.LocalName == "numId" && chh.Attributes != null && chh.Attributes.Count == 1) 
                                            id = chh.Attributes[0].Value;
                                        else if (chh.LocalName == "ilvl" && chh.Attributes != null && chh.Attributes.Count == 1) 
                                            int.TryParse(chh.Attributes[0].Value ?? "", out lev);
                                    }
                                }
                                else if (xxx.LocalName == "listPr") 
                                {
                                    numStyle = DocNumStyles.ReadNumberStyle(xxx);
                                    lev = numStyle.Lvl;
                                }
                                else if (xxx.LocalName == "pStyle") 
                                {
                                    numTxtStyle = m_Styles.GetStyle(xxx, "val");
                                    if (numTxtStyle != null && numTxtStyle.NumId != null) 
                                    {
                                        if (id != null || lev > 0) 
                                        {
                                        }
                                        else 
                                        {
                                            id = numTxtStyle.NumId;
                                            lev = numTxtStyle.NumLvl;
                                        }
                                    }
                                }
                            }
                            numStyle = m_NumStyles.GetStyle(id);
                        }
                    }
                    gen.AppendPagesection(m_CurSection);
                    stackNodes.Add(child);
                    this.ReadNode(stackNodes, pgen, para, pictTop);
                    if (pictTop < 0) 
                    {
                        pgen.AppendNewline(false);
                        if (para != null && para.Style != null && para.Style.GetAttr("heading-level") != null) 
                            pgen.AppendNewline(false);
                    }
                    Pullenti.Unitext.UnitextItem fi = pgen.Finish(false, null);
                    if (fi != null) 
                        gen.Append(fi, numStyle, lev, false);
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    break;
                case "r":
                    if (child.Attributes != null) 
                    {
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "rsidRPr") 
                            {
                                if (!string.IsNullOrEmpty(m_lastRSID) && string.Compare(a.Value, m_lastRSID, true) != 0) 
                                {
                                    m_lastRSID = a.LocalName;
                                    if (gen.LastChar != 0 && !char.IsWhiteSpace(gen.LastChar)) 
                                        gen.AppendText(" ", false);
                                }
                            }
                        }
                    }
                    Pullenti.Unitext.UnitextStyle ust = null;
                    int isSup = -1;
                    foreach (XmlNode rpr in child.ChildNodes) 
                    {
                        if (rpr.LocalName == "rPr") 
                        {
                            ust = new Pullenti.Unitext.UnitextStyle();
                            m_Styles.ReadUnitextStyle(rpr, ust);
                            if (sfrag != null) 
                                ust.RemoveInheritAttrs(sfrag);
                            ust = m_Styles.RegisterStyle(ust);
                            foreach (XmlNode xxx in rpr.ChildNodes) 
                            {
                                if (xxx.LocalName == "vertAlign" && xxx.Attributes != null && xxx.Attributes.Count > 0) 
                                {
                                    if (xxx.Attributes[0].Value == "superscript") 
                                        isSup = 1;
                                    else if (xxx.Attributes[0].Value == "subscript") 
                                        isSup = 0;
                                }
                            }
                        }
                    }
                    if (isSup >= 0) 
                    {
                        Pullenti.Unitext.Internal.Uni.UnitextGen gg1 = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        stackNodes.Add(child);
                        this.ReadNode(stackNodes, gg1, sfrag, pictTop);
                        stackNodes.RemoveAt(stackNodes.Count - 1);
                        Pullenti.Unitext.UnitextItem tmp = gg1.Finish(true, null);
                        if (tmp == null) 
                            break;
                        string tt = Pullenti.Unitext.Internal.Uni.UnitextHelper.GetPlaintext(tmp);
                        if (tt == null) 
                            tt = "";
                        tt = tt.Trim();
                        if (tt.StartsWith("<") && tt.EndsWith(">")) 
                            tt = tt.Substring(1, tt.Length - 2);
                        if (tt.Length > 0 && (tt.Length < 10)) 
                        {
                            Pullenti.Unitext.UnitextPlaintext tp = new Pullenti.Unitext.UnitextPlaintext() { Text = tt, Typ = (isSup == 0 ? Pullenti.Unitext.UnitextPlaintextType.Sub : Pullenti.Unitext.UnitextPlaintextType.Sup) };
                            gen.Append(tp, null, -1, false);
                            break;
                        }
                    }
                    stackNodes.Add(child);
                    if (ust == null || sfrag == null) 
                        this.ReadNode(stackNodes, gen, sfrag, pictTop);
                    else 
                    {
                        Pullenti.Unitext.UnitextStyledFragment ifr = new Pullenti.Unitext.UnitextStyledFragment() { Parent = sfrag, Typ = Pullenti.Unitext.UnitextStyledFragmentType.Inline };
                        ifr.Style = ust;
                        Pullenti.Unitext.Internal.Uni.UnitextGen gg1 = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        gg1.SetStyle(ifr);
                        this.ReadNode(stackNodes, gg1, ifr, pictTop);
                        Pullenti.Unitext.UnitextItem v = gg1.Finish(false, null);
                        if (v != null) 
                        {
                            gen.Append(v, null, -1, false);
                            sfrag.Children.Add(ifr);
                        }
                    }
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    break;
                case "footnoteReference":
                    delText = null;
                    if (child.Attributes == null) 
                        break;
                    foreach (XmlAttribute a in child.Attributes) 
                    {
                        if (a.LocalName == "customMarkFollows" && a.Value == "1" && ((i + 1) < node.ChildNodes.Count)) 
                        {
                            XmlNode ch1 = node.ChildNodes[i + 1];
                            if (ch1.LocalName == "t") 
                            {
                                delText = ch1.InnerText;
                                i++;
                                break;
                            }
                        }
                    }
                    foreach (XmlAttribute a in child.Attributes) 
                    {
                        if (a.LocalName == "id" && m_Footnotes.ContainsKey(a.Value)) 
                        {
                            Pullenti.Unitext.UnitextItem cnt = m_Footnotes[a.Value];
                            if (!string.IsNullOrEmpty(delText)) 
                            {
                                Pullenti.Unitext.UnitextPlaintext pl = cnt as Pullenti.Unitext.UnitextPlaintext;
                                if (pl != null && pl.Text.StartsWith(delText)) 
                                    pl.Text = pl.Text.Substring(delText.Length).Trim();
                            }
                            if ((cnt.m_StyledFrag != null && cnt.m_StyledFrag.Parent == null && (cnt is Pullenti.Unitext.UnitextContainer)) && sfrag != null) 
                            {
                                Pullenti.Unitext.UnitextContainer cc = cnt as Pullenti.Unitext.UnitextContainer;
                                if (cc.Children.Count == 1 && (cc.Children[0] is Pullenti.Unitext.UnitextContainer)) 
                                {
                                    cnt = cc.Children[0];
                                    if (cnt.m_StyledFrag != null && cnt.m_StyledFrag.Parent != null) 
                                    {
                                        cnt.m_StyledFrag.Typ = Pullenti.Unitext.UnitextStyledFragmentType.Footnote;
                                        cnt.m_StyledFrag.Parent.Children.Remove(cnt.m_StyledFrag);
                                        sfrag.Children.Add(cnt.m_StyledFrag);
                                        cnt.m_StyledFrag.Parent = sfrag;
                                    }
                                }
                            }
                            gen.Append(new Pullenti.Unitext.UnitextFootnote() { Content = cnt, CustomMark = delText }, null, -1, false);
                            break;
                        }
                    }
                    break;
                case "endnoteReference":
                    if (child.Attributes != null) 
                    {
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "id" && m_Footnotes.ContainsKey("end" + a.Value)) 
                            {
                                Pullenti.Unitext.UnitextItem cnt = m_Footnotes["end" + a.Value];
                                if ((cnt.m_StyledFrag != null && cnt.m_StyledFrag.Parent == null && (cnt is Pullenti.Unitext.UnitextContainer)) && sfrag != null) 
                                {
                                    Pullenti.Unitext.UnitextContainer cc = cnt as Pullenti.Unitext.UnitextContainer;
                                    if (cc.Children.Count == 1 && (cc.Children[0] is Pullenti.Unitext.UnitextContainer)) 
                                    {
                                        cnt = cc.Children[0];
                                        if (cnt.m_StyledFrag != null && cnt.m_StyledFrag.Parent != null) 
                                        {
                                            cnt.m_StyledFrag.Typ = Pullenti.Unitext.UnitextStyledFragmentType.Footnote;
                                            cnt.m_StyledFrag.Parent.Children.Remove(cnt.m_StyledFrag);
                                            sfrag.Children.Add(cnt.m_StyledFrag);
                                            cnt.m_StyledFrag.Parent = sfrag;
                                        }
                                    }
                                }
                                gen.Append(new Pullenti.Unitext.UnitextFootnote() { Content = cnt, IsEndnote = true }, null, -1, false);
                                break;
                            }
                        }
                    }
                    break;
                case "footnote":
                case "endnote":
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg2 = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    stackNodes.Add(child);
                    this.ReadNode(stackNodes, gg2, sfrag, pictTop);
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    Pullenti.Unitext.UnitextItem tmp2 = gg2.Finish(true, null);
                    if (tmp2 == null) 
                        break;
                    Pullenti.Unitext.UnitextFootnote fn = new Pullenti.Unitext.UnitextFootnote() { IsEndnote = locname == "endnote", Content = tmp2 };
                    gen.Append(fn, null, -1, false);
                    break;
                case "blip":
                    if (child.Attributes != null) 
                    {
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "embed") 
                            {
                                Pullenti.Unitext.UnitextImage img = new Pullenti.Unitext.UnitextImage() { Id = a.Value };
                                gen.Append(img, null, -1, false);
                                for (int ii = stackNodes.Count - 1; ii >= 0; ii--) 
                                {
                                    foreach (XmlNode xxx in stackNodes[ii].ChildNodes) 
                                    {
                                        if (xxx.LocalName == "extent" && xxx.Attributes != null) 
                                        {
                                            int xw = 0;
                                            int yh = 0;
                                            foreach (XmlAttribute aa in xxx.Attributes) 
                                            {
                                                if (aa.LocalName == "cx") 
                                                    int.TryParse(aa.Value, out xw);
                                                else if (aa.LocalName == "cy") 
                                                    int.TryParse(aa.Value, out yh);
                                            }
                                            if (xw > 0 && yh > 0) 
                                            {
                                                img.Width = string.Format("{0}pt", (xw * 72) / 914400);
                                                img.Height = string.Format("{0}pt", (yh * 72) / 914400);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    break;
                case "imagedata":
                    if (child.Attributes != null) 
                    {
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "id") 
                            {
                                Pullenti.Unitext.UnitextImage img = new Pullenti.Unitext.UnitextImage() { Id = a.Value };
                                gen.Append(img, null, -1, false);
                                if (node.LocalName == "shape" && node.Attributes != null) 
                                {
                                    foreach (XmlAttribute aa in node.Attributes) 
                                    {
                                        if (aa.LocalName == "style") 
                                        {
                                            _setImageSize(img, aa.Value);
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    break;
                case "OLEObject":
                    if (child.Attributes != null) 
                    {
                        foreach (XmlAttribute a in child.Attributes) 
                        {
                            if (a.LocalName == "id") 
                            {
                                if (m_Embeds.ContainsKey(a.Value)) 
                                {
                                    gen.Append(m_Embeds[a.Value], null, -1, false);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case "sdt":
                    string tag = null;
                    string valu = null;
                    Pullenti.Unitext.UnitextStyle ust1 = null;
                    foreach (XmlNode x in child.ChildNodes) 
                    {
                        if (x.LocalName == "sdtPr") 
                        {
                            foreach (XmlNode xx in x.ChildNodes) 
                            {
                                if (xx.LocalName == "tag") 
                                    tag = _getAttrValue(xx, "val");
                                else if (xx.LocalName == "date") 
                                    valu = _getAttrValue(xx, "fullDate");
                            }
                            ust1 = new Pullenti.Unitext.UnitextStyle();
                            m_Styles.ReadUnitextStyle(x, ust1);
                            if (sfrag != null) 
                                ust1.RemoveInheritAttrs(sfrag);
                            ust1 = m_Styles.RegisterStyle(ust1);
                        }
                    }
                    foreach (XmlNode x in child.ChildNodes) 
                    {
                        if (x.LocalName == "sdtContent") 
                        {
                            Pullenti.Unitext.UnitextStyledFragment sfrag1 = sfrag;
                            if (ust1 != null) 
                            {
                                sfrag1 = new Pullenti.Unitext.UnitextStyledFragment() { Typ = Pullenti.Unitext.UnitextStyledFragmentType.Inline, Style = ust1, Parent = sfrag1 };
                                sfrag.Children.Add(sfrag1);
                            }
                            Pullenti.Unitext.Internal.Uni.UnitextGen gg3 = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            gg3.SetStyle(sfrag1);
                            stackNodes.Add(x);
                            this.ReadNode(stackNodes, gg3, sfrag1, pictTop);
                            stackNodes.RemoveAt(stackNodes.Count - 1);
                            Pullenti.Unitext.UnitextItem tmp3 = gg3.Finish(true, null);
                            if (tmp3 == null) 
                                break;
                            if (valu == null) 
                                valu = Pullenti.Unitext.Internal.Uni.UnitextHelper.GetPlaintext(tmp3);
                            if (tmp3 is Pullenti.Unitext.UnitextContainer) 
                                (tmp3 as Pullenti.Unitext.UnitextContainer).Typ = Pullenti.Unitext.UnitextContainerType.ContentControl;
                            else 
                            {
                                Pullenti.Unitext.UnitextContainer ccc = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.ContentControl, m_StyledFrag = sfrag1 };
                                ccc.Children.Add(tmp3);
                                tmp3.Parent = ccc;
                                tmp3 = ccc;
                            }
                            tmp3.HtmlTitle = string.Format("Content control: {0}", tag ?? "?");
                            (tmp3 as Pullenti.Unitext.UnitextContainer).Data = valu;
                            tmp3.Id = tag;
                            gen.Append(tmp3, null, -1, false);
                        }
                    }
                    if (tag != null && valu != null) 
                    {
                        if (!m_DataControls.ContainsKey(tag)) 
                            m_DataControls.Add(tag, valu);
                    }
                    break;
                case "ffData":
                    string nam1 = null;
                    foreach (XmlNode x in child.ChildNodes) 
                    {
                        if (x.LocalName == "name") 
                            nam1 = _getAttrValue(x, "val");
                        else if (x.LocalName == "textInput") 
                        {
                            foreach (XmlNode xx in x.ChildNodes) 
                            {
                                if (xx.LocalName == "default") 
                                {
                                    string val1 = _getAttrValue(xx, "val");
                                    if (val1 != null && nam1 != null) 
                                    {
                                        if (!m_DataControls.ContainsKey(nam1)) 
                                            m_DataControls.Add(nam1, val1);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "rect":
                    foreach (XmlAttribute a in child.Attributes) 
                    {
                        if (a.LocalName == "style") 
                        {
                            if (pictTop < 0) 
                                pictTop = 0;
                            foreach (string s in a.Value.Split(';')) 
                            {
                                int ii = s.IndexOf(':');
                                if (ii <= 0) 
                                    continue;
                                string key = s.Substring(0, ii).Trim().ToLower();
                                string va = s.Substring(ii + 1).Trim();
                                if (key == "position") 
                                {
                                    if (va != "absolute") 
                                        break;
                                }
                                if (key != "top") 
                                    continue;
                                for (int j = va.Length - 1; j > 0; j--) 
                                {
                                    if (char.IsDigit(va[j])) 
                                    {
                                        if (!int.TryParse(va.Substring(0, j + 1), out ii)) 
                                            break;
                                        if (ii > pictTop) 
                                        {
                                            gen.AppendNewline(true);
                                            pictTop = ii;
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    stackNodes.Add(child);
                    this.ReadNode(stackNodes, gen, sfrag, pictTop);
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    break;
                default: 
                    if ((pictTop < 0) && child.LocalName == "txbxContent") 
                        gen.AppendText(" ", false);
                    stackNodes.Add(child);
                    this.ReadNode(stackNodes, gen, sfrag, pictTop);
                    stackNodes.RemoveAt(stackNodes.Count - 1);
                    if ((pictTop < 0) && child.LocalName == "txbxContent") 
                        gen.AppendText(" ", false);
                    break;
                }
            }
        }
        static void _setImageSize(Pullenti.Unitext.UnitextImage img, string style)
        {
            int ii = style.IndexOf("width:");
            if (ii >= 0) 
            {
                img.Width = style.Substring(ii + 6).Trim();
                if ((((ii = img.Width.IndexOf(';')))) > 0) 
                    img.Width = img.Width.Substring(0, ii).Trim();
            }
            ii = style.IndexOf("height:");
            if (ii >= 0) 
            {
                img.Height = style.Substring(ii + 7).Trim();
                if ((((ii = img.Height.IndexOf(';')))) > 0) 
                    img.Height = img.Height.Substring(0, ii).Trim();
            }
        }
        static string _getAttrValue(XmlNode n, string attrName)
        {
            if (n.Attributes != null) 
            {
                foreach (XmlAttribute a in n.Attributes) 
                {
                    if (a.LocalName == attrName) 
                        return a.Value;
                }
            }
            return null;
        }
    }
}