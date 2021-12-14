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
using System.Threading;
using System.Xml;

namespace Pullenti.Unitext.Internal.Uni
{
    static class UnitextHelper
    {
        public static Pullenti.Unitext.UnitextDocument Create(string fileName, byte[] fileContent, Pullenti.Unitext.CreateDocumentParam pars)
        {
            if (fileContent == null) 
            {
                if (fileName == null) 
                    return null;
                fileName = Path.GetFullPath(fileName);
                if (!File.Exists(fileName)) 
                    return null;
            }
            Pullenti.Unitext.FileFormat frm = Pullenti.Util.FileFormatsHelper.AnalizeFileFormat(fileName, fileContent);
            Pullenti.Unitext.UnitextDocument doc = null;
            try 
            {
                if (Pullenti.Unitext.UnitextService.Extension != null) 
                    doc = Pullenti.Unitext.UnitextService.Extension.CreateDocument(frm, fileName, fileContent, pars);
                if (doc == null) 
                    doc = _create(frm, fileName, fileContent, pars);
                if (doc == null) 
                    doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = frm, ErrorMessage = (frm == Pullenti.Unitext.FileFormat.Unknown ? "Format unknown" : "Can't create document") };
            }
            catch(Exception ex) 
            {
                doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = frm, ErrorMessage = ex.ToString() };
            }
            if (doc.SourceFileName == null) 
                doc.SourceFileName = fileName;
            List<Pullenti.Unitext.UnitextItem> all = new List<Pullenti.Unitext.UnitextItem>();
            doc.GetAllItems(all, 0);
            foreach (Pullenti.Unitext.UnitextItem it in all) 
            {
                Pullenti.Unitext.UnitextImage img = it as Pullenti.Unitext.UnitextImage;
                if (img != null) 
                {
                    if (img.Content == null && img.Id != null) 
                    {
                        if (img.Id.StartsWith("data:image")) 
                        {
                            int ii = img.Id.IndexOf("base64,");
                            if (ii > 0) 
                            {
                                try 
                                {
                                    img.Content = Convert.FromBase64String(img.Id.Substring(ii + 7));
                                    img.Id = null;
                                }
                                catch(Exception ex) 
                                {
                                }
                            }
                        }
                        else if (img.Id.StartsWith("http") || img.Id.StartsWith("www")) 
                        {
                            img.HtmlSrcUri = img.Id;
                            continue;
                        }
                    }
                    if (img.Content != null && img.Content.Length > 10) 
                    {
                        if (img.Content[0] == 0x1F && img.Content[1] == 0x8B) 
                        {
                            try 
                            {
                                img.Content = Pullenti.Util.ArchiveHelper.DecompressGZip(img.Content);
                            }
                            catch(Exception ex230) 
                            {
                            }
                        }
                    }
                    if (img.Content == null && img.Id != null && (img.Id.Length < 30)) 
                        continue;
                }
            }
            return doc;
        }
        public static byte[] LoadDataFromFile(string fileName, int attampts = 0)
        {
            FileStream fStr = null;
            byte[] buf = null;
            try 
            {
                Exception ex = null;
                for (int i = 0; i <= attampts; i++) 
                {
                    try 
                    {
                        fStr = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        break;
                    }
                    catch(Exception e) 
                    {
                        ex = e;
                    }
                    if (i == 0 && !File.Exists(fileName)) 
                        break;
                    Thread.Sleep(500);
                }
                if (fStr == null) 
                    throw ex;
                if (fStr.Length == 0) 
                    return null;
                buf = new byte[(int)fStr.Length];
                fStr.Read(buf, 0, (int)fStr.Length);
            }
            finally
            {
                if (fStr != null) 
                    fStr.Dispose();
            }
            return buf;
        }
        static Pullenti.Unitext.UnitextDocument _create(Pullenti.Unitext.FileFormat frm, string fileName, byte[] fileContent, Pullenti.Unitext.CreateDocumentParam pars)
        {
            Pullenti.Unitext.FileFormatClass frmcl = Pullenti.Util.FileFormatsHelper.GetFormatClass(frm);
            if (frmcl == Pullenti.Unitext.FileFormatClass.Archive) 
            {
                Pullenti.Unitext.UnitextDocument res = new Pullenti.Unitext.UnitextDocument() { SourceFileName = fileName, SourceFormat = frm };
                if (pars.IgnoreInnerDocuments) 
                    return res;
                Dictionary<string, int> files = null;
                try 
                {
                    files = Pullenti.Util.ArchiveHelper.GetFileNamesFromArchive(fileName, fileContent);
                }
                catch(Exception ex) 
                {
                    res.ErrorMessage = ex.Message;
                    return res;
                }
                if (files == null) 
                {
                    res.ErrorMessage = string.Format("Archive format {0} not supported", frm.ToString());
                    return res;
                }
                if (files.Count == 0) 
                    return res;
                foreach (KeyValuePair<string, int> f in files) 
                {
                    string ext = Path.GetExtension(f.Key);
                    if (ext == ".exe" || ext == ".dll") 
                        continue;
                    try 
                    {
                        byte[] cnt = Pullenti.Util.ArchiveHelper.GetFileFromArchive(fileName, fileContent, f.Key);
                        if (cnt == null || cnt.Length == 0) 
                            continue;
                        Pullenti.Unitext.UnitextDocument d = Pullenti.Unitext.UnitextService.CreateDocument(f.Key, cnt, pars);
                        res.InnerDocuments.Add(d);
                    }
                    catch(Exception ex) 
                    {
                        Pullenti.Unitext.UnitextDocument d = new Pullenti.Unitext.UnitextDocument() { SourceFileName = f.Key, SourceFormat = Pullenti.Util.FileFormatsHelper.AnalizeFormat(ext, null) };
                        d.ErrorMessage = ex.Message;
                        res.InnerDocuments.Add(d);
                    }
                }
                return res;
            }
            if (fileContent == null) 
            {
                FileInfo fi = new FileInfo(fileName);
                if (fi.Length < 10000000) 
                    fileContent = LoadDataFromFile(fileName, 0);
            }
            if (frm == Pullenti.Unitext.FileFormat.Rtf) 
            {
                if (fileContent != null) 
                {
                    using (MemoryStream mem = new MemoryStream(fileContent)) 
                    {
                        return Pullenti.Unitext.Internal.Rtf.RtfHelper.CreateUniDoc(mem, pars);
                    }
                }
                else 
                {
                    using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
                    {
                        return Pullenti.Unitext.Internal.Rtf.RtfHelper.CreateUniDoc(f, pars);
                    }
                }
            }
            if ((frm == Pullenti.Unitext.FileFormat.Docx || frm == Pullenti.Unitext.FileFormat.Xlsx || frm == Pullenti.Unitext.FileFormat.Pptx) || frm == Pullenti.Unitext.FileFormat.Odt || frm == Pullenti.Unitext.FileFormat.Docxml) 
            {
                using (Pullenti.Unitext.Internal.Word.DocxToText d2t = new Pullenti.Unitext.Internal.Word.DocxToText(fileName, fileContent, frm == Pullenti.Unitext.FileFormat.Docxml)) 
                {
                    Pullenti.Unitext.UnitextDocument doc = d2t.CreateUniDoc(pars.OnlyForPureText, frm, pars);
                    if (doc != null) 
                    {
                        StyleHelper.ProcessDoc(doc);
                        doc.Optimize(false, pars);
                    }
                    return doc;
                }
            }
            if (frm == Pullenti.Unitext.FileFormat.Xlsxml) 
            {
                Pullenti.Unitext.Internal.Misc.MyXmlReader xm = null;
                if (fileContent != null) 
                {
                    xm = Pullenti.Unitext.Internal.Misc.MyXmlReader.Create(fileContent);
                    return Pullenti.Unitext.Internal.Misc.ExcelHelper.CreateDocForXml(xm);
                }
                else 
                {
                    xm = Pullenti.Unitext.Internal.Misc.MyXmlReader.Create(LoadDataFromFile(fileName, 0));
                    return Pullenti.Unitext.Internal.Misc.ExcelHelper.CreateDocForXml(xm);
                }
            }
            if (frm == Pullenti.Unitext.FileFormat.Doc) 
            {
                Pullenti.Unitext.UnitextDocument doc = null;
                if (fileContent != null) 
                    doc = Pullenti.Unitext.Internal.Word.MSOfficeHelper._uniFromWord97Arr(fileContent);
                else 
                    doc = Pullenti.Unitext.Internal.Word.MSOfficeHelper._uniFromWord97File(fileName);
                if (doc == null) 
                    return null;
                if (doc.Attrs.ContainsKey("word") && doc.Attrs["word"] == "6") 
                {
                    if (pars.IgnoreWord6) 
                    {
                        doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Doc };
                        doc.ErrorMessage = "Word6 not supported";
                        return doc;
                    }
                }
                doc.Optimize(false, pars);
                return doc;
            }
            if (frm == Pullenti.Unitext.FileFormat.Pdf) 
                return Pullenti.Unitext.Internal.Pdf.PdfHelper.CreateUni(fileName, fileContent, pars);
            if (frm == Pullenti.Unitext.FileFormat.Html) 
            {
                Pullenti.Unitext.Internal.Html.HtmlNode nod = Pullenti.Unitext.Internal.Html.HtmlHelper.CreateNode(fileName, fileContent, null);
                if (nod == null) 
                    return null;
                string dirName = null;
                if (!pars.OnlyForPureText && fileName != null && File.Exists(fileName)) 
                {
                    string dir = Path.GetDirectoryName(Path.GetFullPath(fileName));
                    if (Directory.Exists(dir)) 
                        dirName = dir;
                }
                Pullenti.Unitext.UnitextDocument doc = Pullenti.Unitext.Internal.Html.HtmlHelper.Create(nod, dirName, null, pars);
                return doc;
            }
            if (frm == Pullenti.Unitext.FileFormat.Mht || frm == Pullenti.Unitext.FileFormat.Eml) 
            {
                FileStream fs = null;
                if (fileContent == null) 
                    fileContent = LoadDataFromFile(fileName, 0);
                try 
                {
                    Pullenti.Unitext.UnitextDocument doc = Pullenti.Unitext.Internal.Html.MhtHelper._createDoc(fileContent, pars, frm == Pullenti.Unitext.FileFormat.Mht);
                    if (doc != null) 
                    {
                        doc.Optimize(false, pars);
                        doc.SourceFileName = fileName;
                        doc.SourceFormat = frm;
                    }
                    else 
                        doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = frm, ErrorMessage = "Не удалось сформировать письмо" };
                    return doc;
                }
                finally
                {
                    if (fs != null) 
                        fs.Dispose();
                }
            }
            if (frm == Pullenti.Unitext.FileFormat.Fb2) 
                return Pullenti.Unitext.Internal.Misc.Fb2Helper.CreateDoc(fileName, fileContent, pars);
            if (frm == Pullenti.Unitext.FileFormat.Fb3) 
                return Pullenti.Unitext.Internal.Misc.Fb2Helper.CreateDocZip(fileName, fileContent, pars);
            if (frm == Pullenti.Unitext.FileFormat.Epub) 
                return Pullenti.Unitext.Internal.Misc.EpubHelper.CreateDoc(fileName, fileContent, pars);
            if (frm == Pullenti.Unitext.FileFormat.Txt || frm == Pullenti.Unitext.FileFormat.Csv) 
            {
                string txt = null;
                if (fileContent != null) 
                    txt = Pullenti.Util.TextHelper.ReadStringFromBytes(fileContent, false);
                else 
                    txt = Pullenti.Util.TextHelper.ReadStringFromFile(fileName, false);
                if (string.IsNullOrEmpty(txt)) 
                    return null;
                int len = txt.Length;
                if (frm == Pullenti.Unitext.FileFormat.Csv) 
                {
                    Pullenti.Unitext.UnitextDocument csv = Pullenti.Unitext.Internal.Misc.CsvHelper.Create(txt);
                    if (csv != null) 
                        return csv;
                }
                UnitextGen gen = new UnitextGen();
                gen.AppendText(txt, true);
                Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = Pullenti.Unitext.FileFormat.Txt };
                doc.Content = gen.Finish(true, null);
                if (doc.Content == null) 
                    return null;
                return doc;
            }
            if (frm == Pullenti.Unitext.FileFormat.Tif) 
            {
                if (fileContent == null) 
                    fileContent = LoadDataFromFile(fileName, 0);
                Pullenti.Unitext.UnitextDocument doc = Pullenti.Unitext.Internal.Misc.TiffHelper.CreateDoc(fileContent);
                doc.RefreshContentByPages();
                return doc;
            }
            if (frmcl == Pullenti.Unitext.FileFormatClass.Image) 
            {
                Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourceFormat = frm, SourceFileName = fileName };
                Pullenti.Unitext.UnilayPage page = new Pullenti.Unitext.UnilayPage();
                page.ImageContent = fileContent ?? UnitextHelper.LoadDataFromFile(fileName, 0);
                doc.Pages.Add(page);
                doc.RefreshContentByPages();
                return doc;
            }
            Pullenti.Unitext.UnitextDocument err = new Pullenti.Unitext.UnitextDocument() { SourceFormat = frm, ErrorMessage = "Unsupported format" };
            if (frm != Pullenti.Unitext.FileFormat.Unknown) 
                err.ErrorMessage = string.Format("{0} {1}", err.ErrorMessage, frm.ToString());
            return err;
        }
        public static Pullenti.Unitext.UnitextItem CreateItem(XmlNode xml)
        {
            Pullenti.Unitext.UnitextItem res = null;
            switch (xml.LocalName) { 
            case "container":
                res = new Pullenti.Unitext.UnitextContainer();
                break;
            case "text":
                res = new Pullenti.Unitext.UnitextPlaintext();
                break;
            case "newline":
                res = new Pullenti.Unitext.UnitextNewline();
                break;
            case "pagebreak":
                res = new Pullenti.Unitext.UnitextPagebreak();
                break;
            case "table":
                res = new Pullenti.Unitext.UnitextTable();
                break;
            case "cell":
                res = new Pullenti.Unitext.UnitextTablecell();
                break;
            case "hyperlink":
                res = new Pullenti.Unitext.UnitextHyperlink();
                break;
            case "footnote":
                res = new Pullenti.Unitext.UnitextFootnote();
                break;
            case "list":
                res = new Pullenti.Unitext.UnitextList();
                break;
            case "listitem":
                res = new Pullenti.Unitext.UnitextListitem();
                break;
            case "image":
                res = new Pullenti.Unitext.UnitextImage();
                break;
            case "comment":
                res = new Pullenti.Unitext.UnitextComment();
                break;
            case "docblock":
                res = new Pullenti.Unitext.UnitextDocblock();
                break;
            case "misc":
                res = new Pullenti.Unitext.UnitextMisc();
                break;
            }
            if (res != null) 
                res.FromXml(xml);
            return res;
        }
        public static Pullenti.Unitext.UnitextDocument CreateDocFromText(string text)
        {
            Pullenti.Unitext.UnitextDocument doc = new Pullenti.Unitext.UnitextDocument() { SourcePlainText = text ?? "" };
            if (string.IsNullOrEmpty(text)) 
                return doc;
            Pullenti.Unitext.UnitextItem cur = null;
            Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer() { EndChar = text.Length - 1 };
            doc.Content = cnt;
            cnt.Parent = doc;
            doc.EndChar = text.Length - 1;
            for (int i = 0; i < text.Length; i++) 
            {
                char ch = text[i];
                if (ch == '\r' || ch == '\n') 
                {
                    if (!(cur is Pullenti.Unitext.UnitextNewline)) 
                    {
                        cur = new Pullenti.Unitext.UnitextNewline() { Parent = cnt };
                        cnt.Children.Add(cur);
                        cur.BeginChar = i;
                    }
                    cur.EndChar = i;
                    if (ch == '\r') 
                        (cur as Pullenti.Unitext.UnitextNewline).Count++;
                    else if (ch == '\n') 
                    {
                        if (i == 0 || text[i - 1] != '\r') 
                            (cur as Pullenti.Unitext.UnitextNewline).Count++;
                    }
                }
                else if (ch == 0xC) 
                {
                    cur = new Pullenti.Unitext.UnitextPagebreak() { Parent = cnt };
                    cnt.Children.Add(cur);
                    cur.BeginChar = i;
                    cur.EndChar = i;
                }
                else 
                {
                    if (!(cur is Pullenti.Unitext.UnitextPlaintext)) 
                    {
                        cur = new Pullenti.Unitext.UnitextPlaintext() { Parent = cnt };
                        cnt.Children.Add(cur);
                        cur.BeginChar = i;
                    }
                    cur.EndChar = i;
                }
            }
            foreach (Pullenti.Unitext.UnitextItem ch in cnt.Children) 
            {
                if ((ch is Pullenti.Unitext.UnitextPlaintext) && ch.BeginChar <= ch.EndChar) 
                    (ch as Pullenti.Unitext.UnitextPlaintext).Text = text.Substring(ch.BeginChar, (ch.EndChar - ch.BeginChar) + 1);
            }
            doc.GenerateIds();
            return doc;
        }
        static Pullenti.Unitext.GetPlaintextParam m_StdParams = new Pullenti.Unitext.GetPlaintextParam() { SetPositions = false };
        public static string GetPlaintext(Pullenti.Unitext.UnitextItem it)
        {
            if (it == null) 
                return null;
            StringBuilder res = new StringBuilder();
            it.GetPlaintext(res, m_StdParams);
            if (res.Length == 0) 
                return null;
            return res.ToString();
        }
        internal static string[] m_Romans = new string[] {"I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX", "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI", "XXVII", "XXVIII", "XXIX", "XXX"};
    }
}