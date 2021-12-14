/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;

namespace Pullenti.Util
{
    /// <summary>
    /// Хелпер работы с форматами файлов
    /// </summary>
    public static class FileFormatsHelper
    {
        /// <summary>
        /// Проанализировать формат файла по расширению и небольшому начальному фрагменту
        /// </summary>
        /// <param name="fileExt">расширение файла (может быть null)</param>
        /// <param name="fileContext">начальный фрагмент файла (может быть null), но можно и весь файл целиком, если он уже в памяти.</param>
        /// <return>формат</return>
        public static Pullenti.Unitext.FileFormat AnalizeFormat(string fileExt, byte[] fileContext)
        {
            if (fileExt != null) 
            {
                fileExt = fileExt.ToLower();
                if (!fileExt.StartsWith(".")) 
                    fileExt = "." + fileExt;
            }
            if (fileContext != null && fileContext.Length > 6) 
            {
                if (fileContext[0] == 0xD0 && fileContext[1] == 0xCF) 
                {
                    if ((fileExt == ".doc" || fileExt == ".docx" || fileExt == ".rtf") || fileExt == ".txt" || fileExt == ".dot") 
                        return Pullenti.Unitext.FileFormat.Doc;
                    if (fileExt == ".xls" || fileExt == ".xlsx") 
                        return Pullenti.Unitext.FileFormat.Xls;
                    if (fileExt == ".msg") 
                        return Pullenti.Unitext.FileFormat.Msg;
                    if (fileExt == null || fileExt == ".") 
                        return Pullenti.Unitext.FileFormat.Doc;
                }
                if ((fileContext[0] == '0' && fileContext[1] == 'M' && fileContext[2] == '8') && fileContext[3] == 'R') 
                {
                    if (fileExt == ".msg") 
                        return Pullenti.Unitext.FileFormat.Msg;
                }
                if (fileContext[0] == 1 && fileContext[1] == 0 && fileContext.Length > 0x30) 
                {
                    if ((fileContext[0x28] == ' ' && fileContext[0x29] == 'E' && fileContext[0x2A] == 'M') && fileContext[0x2B] == 'F') 
                        return Pullenti.Unitext.FileFormat.Emf;
                }
                if (fileContext[0] == 0x50 && fileContext[1] == 0x4B) 
                {
                    if ((fileExt == ".doc" || fileExt == ".docx" || fileExt == ".rtf") || fileExt == ".dotm") 
                        return Pullenti.Unitext.FileFormat.Docx;
                    else if (fileExt == ".pptx") 
                        return Pullenti.Unitext.FileFormat.Pptx;
                    else if (fileExt == ".vsdx") 
                        return Pullenti.Unitext.FileFormat.Unknown;
                    else if (fileExt == ".xls" || fileExt == ".xlsx" || fileExt == ".xlsb") 
                        return Pullenti.Unitext.FileFormat.Xlsx;
                    else if (fileExt == ".odt") 
                        return Pullenti.Unitext.FileFormat.Odt;
                    else if (fileExt == ".ods") 
                        return Pullenti.Unitext.FileFormat.Ods;
                    else if (fileExt == ".epub") 
                        return Pullenti.Unitext.FileFormat.Epub;
                    else if (fileExt == ".fb3") 
                        return Pullenti.Unitext.FileFormat.Fb3;
                    else if (fileExt == ".xps") 
                        return Pullenti.Unitext.FileFormat.Xps;
                    else 
                    {
                        for (int k = 0; k < 3; k++) 
                        {
                            string tag = (k == 0 ? "[Content_Types].xml" : (k == 1 ? "word/document.xml" : "word/"));
                            for (int i = 0; i < (fileContext.Length - tag.Length); i++) 
                            {
                                int j;
                                for (j = 0; j < tag.Length; j++) 
                                {
                                    if (fileContext[i + j] != tag[j]) 
                                        break;
                                }
                                if (j >= tag.Length) 
                                    return Pullenti.Unitext.FileFormat.Docx;
                            }
                        }
                        string odtTag = "oasis.opendocument.text";
                        for (int i = 0; i < (fileContext.Length - odtTag.Length); i++) 
                        {
                            int j;
                            for (j = 0; j < odtTag.Length; j++) 
                            {
                                if (fileContext[i + j] != odtTag[j]) 
                                    break;
                            }
                            if (j >= odtTag.Length) 
                                return Pullenti.Unitext.FileFormat.Odt;
                        }
                        return Pullenti.Unitext.FileFormat.Zip;
                    }
                }
                if (fileContext[0] == 0x52 && fileContext[1] == 0x61 && fileContext[2] == 0x72) 
                    return Pullenti.Unitext.FileFormat.Rar;
                if (fileContext[0] == 0x37 && fileContext[1] == 0x7A && fileContext[2] == 0xDC) 
                    return Pullenti.Unitext.FileFormat.Zip7;
                if ((fileContext[0] == 0x25 && fileContext[1] == 0x50 && fileContext[2] == 0x44) && fileContext[3] == 0x46 && fileContext[4] == 0x2D) 
                    return Pullenti.Unitext.FileFormat.Pdf;
                if ((fileContext[0] == 0x7B && fileContext[1] == 0x5C && fileContext[2] == 0x72) && fileContext[3] == 0x74 && fileContext[4] == 0x66) 
                    return Pullenti.Unitext.FileFormat.Rtf;
                if (fileContext[0] == 'B' && fileContext[1] == 'M') 
                {
                    if (fileExt == ".bmp" || fileExt == "." || fileExt == null) 
                        return Pullenti.Unitext.FileFormat.Bmp;
                }
                if (fileContext[0] == 'I' && fileContext[1] == 'I') 
                {
                    if ((fileExt == ".tif" || fileExt == ".tiff" || fileExt == ".") || fileExt == null) 
                        return Pullenti.Unitext.FileFormat.Tif;
                }
                if (fileContext[1] == 'P' && fileContext[2] == 'N' && fileContext[3] == 'G') 
                {
                    if (fileExt == ".png" || fileExt == "." || fileExt == null) 
                        return Pullenti.Unitext.FileFormat.Png;
                }
                if (fileContext[0] == 'G' && fileContext[1] == 'I' && fileContext[2] == 'F') 
                {
                    if (fileExt == ".gif" || fileExt == "." || fileExt == null) 
                        return Pullenti.Unitext.FileFormat.Gif;
                }
                if ((fileContext.Length > 20 && fileContext[6] == 'J' && fileContext[7] == 'F') && fileContext[8] == 'I') 
                {
                    if ((fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".") || fileExt == null) 
                        return Pullenti.Unitext.FileFormat.Jpg;
                }
                if (((fileContext.Length > 10 && fileContext[0] == 0 && fileContext[1] == 0) && fileContext[2] == 0 && fileContext[3] == 0xC) && fileContext[4] == 0x6A && fileContext[5] == 0x50) 
                    return Pullenti.Unitext.FileFormat.Jpg2000;
                string enc = MiscHelper.DecodeStringAscii(fileContext, 0, (fileContext.Length < 1000 ? fileContext.Length : 1000));
                if (fileContext[0] == 0xEF && fileContext[1] == 0xBB && fileContext[2] == 0xBF) 
                    enc = enc.Substring(3);
                enc = enc.ToUpper().Trim();
                if (enc.StartsWith("<HTML") || enc.StartsWith("<!DOCTYPE HTML") || enc.StartsWith("<DIV>")) 
                    return Pullenti.Unitext.FileFormat.Html;
                if (enc.StartsWith("AT&TFORM")) 
                    return Pullenti.Unitext.FileFormat.DjVu;
                if (enc.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)) 
                {
                    if (enc.Contains("<FUNCTIONBOOK") || enc.Contains("<FICTIONBOOK")) 
                        return Pullenti.Unitext.FileFormat.Fb2;
                    if (enc.Contains("<WORKBOOK ") || enc.Contains("<?mso-application progid=\"Excel.Sheet\"".ToUpper())) 
                        return Pullenti.Unitext.FileFormat.Xlsxml;
                    if (enc.Contains("<?mso-application progid=\"Word.Document\"".ToUpper())) 
                        return Pullenti.Unitext.FileFormat.Docxml;
                    if (enc.Contains("<HTML")) 
                        return Pullenti.Unitext.FileFormat.Html;
                }
                if ((enc.StartsWith("DELIVERED-TO:") || enc.StartsWith("RECEIVED:") || enc.StartsWith("RECEIVED-FROM:")) || enc.StartsWith("DELIVERED:")) 
                    return Pullenti.Unitext.FileFormat.Eml;
                int ii = enc.IndexOf("CONTENT-TYPE:");
                if (ii > 0) 
                {
                    if (fileExt == ".eml") 
                        return Pullenti.Unitext.FileFormat.Eml;
                    if (enc.IndexOf("MULTIPART/RELATED") > ii) 
                        return Pullenti.Unitext.FileFormat.Mht;
                }
                if (fileContext[0] == 0xFF && fileContext[1] == 0xFE) 
                {
                    string txt = MiscHelper.DecodeStringUnicode(fileContext, 0, -1);
                    if (txt.Contains("<htm")) 
                        return Pullenti.Unitext.FileFormat.Html;
                    return Pullenti.Unitext.FileFormat.Txt;
                }
                if (fileContext[0] == 0xFE && fileContext[1] == 0xFF) 
                {
                    string txt = MiscHelper.DecodeStringUnicodeBE(fileContext, 0, -1);
                    if (txt.Contains("<htm")) 
                        return Pullenti.Unitext.FileFormat.Html;
                    return Pullenti.Unitext.FileFormat.Txt;
                }
                if (enc.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)) 
                    return Pullenti.Unitext.FileFormat.Xml;
                if (fileExt == ".") 
                {
                    string txt = TextHelper.ReadStringFromBytes(fileContext, false);
                    if (txt != null) 
                        return Pullenti.Unitext.FileFormat.Txt;
                }
            }
            else 
            {
                if (fileExt == ".doc" || fileExt == ".dot") 
                    return Pullenti.Unitext.FileFormat.Doc;
                if (fileExt == ".odt") 
                    return Pullenti.Unitext.FileFormat.Odt;
                if (fileExt == ".docx" || fileExt == ".dotx") 
                    return Pullenti.Unitext.FileFormat.Docx;
                if (fileExt == ".pptx") 
                    return Pullenti.Unitext.FileFormat.Pptx;
                if (fileExt == ".zip") 
                    return Pullenti.Unitext.FileFormat.Zip;
                if (fileExt == ".rar") 
                    return Pullenti.Unitext.FileFormat.Rar;
                if (fileExt == ".xlsx" || fileExt == ".xlsb") 
                    return Pullenti.Unitext.FileFormat.Xlsx;
                if (fileExt == ".xls") 
                    return Pullenti.Unitext.FileFormat.Xls;
                if (fileExt == ".msg") 
                    return Pullenti.Unitext.FileFormat.Msg;
                if (fileExt == ".eml") 
                    return Pullenti.Unitext.FileFormat.Eml;
                if (fileExt == ".djvu") 
                    return Pullenti.Unitext.FileFormat.DjVu;
            }
            if (fileExt == ".tar") 
                return Pullenti.Unitext.FileFormat.Tar;
            if (fileExt == ".gzip" || fileExt == ".gz") 
                return Pullenti.Unitext.FileFormat.Gzip;
            if (fileExt == ".7z") 
                return Pullenti.Unitext.FileFormat.Zip7;
            if (fileExt == ".xml") 
                return Pullenti.Unitext.FileFormat.Xml;
            if (fileExt == ".pdf") 
                return Pullenti.Unitext.FileFormat.Pdf;
            if (fileExt == ".djvu") 
                return Pullenti.Unitext.FileFormat.DjVu;
            if (fileExt == ".xls") 
                return Pullenti.Unitext.FileFormat.Xls;
            if (fileExt == ".txt") 
                return Pullenti.Unitext.FileFormat.Txt;
            if (fileExt == ".csv") 
                return Pullenti.Unitext.FileFormat.Csv;
            if (fileExt == ".rtf") 
                return Pullenti.Unitext.FileFormat.Rtf;
            if (fileExt == ".htm" || fileExt == ".html") 
                return Pullenti.Unitext.FileFormat.Html;
            if (fileExt == ".doc") 
                return Pullenti.Unitext.FileFormat.Txt;
            if (fileExt == ".mht" || fileExt == ".mhtml") 
                return Pullenti.Unitext.FileFormat.Mht;
            if (fileExt == ".fb2") 
                return Pullenti.Unitext.FileFormat.Fb2;
            if (fileExt == ".fb3") 
                return Pullenti.Unitext.FileFormat.Fb3;
            if (fileExt == ".epub") 
                return Pullenti.Unitext.FileFormat.Epub;
            if (fileExt == ".bmp") 
                return Pullenti.Unitext.FileFormat.Bmp;
            if (fileExt == ".jpg" || fileExt == ".jpeg") 
                return Pullenti.Unitext.FileFormat.Jpg;
            if (fileExt == ".jp2") 
                return Pullenti.Unitext.FileFormat.Jpg2000;
            if (fileExt == ".gif") 
                return Pullenti.Unitext.FileFormat.Gif;
            if (fileExt == ".png") 
                return Pullenti.Unitext.FileFormat.Png;
            if (fileExt == ".emf") 
                return Pullenti.Unitext.FileFormat.Emf;
            if (fileExt == ".tif" || fileExt == ".tiff") 
                return Pullenti.Unitext.FileFormat.Tif;
            return Pullenti.Unitext.FileFormat.Unknown;
        }
        /// <summary>
        /// Получить класс формата. Пригодится для определения типа файла.
        /// </summary>
        /// <param name="frm">формат</param>
        /// <return>класс формата</return>
        public static Pullenti.Unitext.FileFormatClass GetFormatClass(Pullenti.Unitext.FileFormat frm)
        {
            if ((frm == Pullenti.Unitext.FileFormat.Zip || frm == Pullenti.Unitext.FileFormat.Zip7 || frm == Pullenti.Unitext.FileFormat.Rar) || frm == Pullenti.Unitext.FileFormat.Tar || frm == Pullenti.Unitext.FileFormat.Gzip) 
                return Pullenti.Unitext.FileFormatClass.Archive;
            if (((frm == Pullenti.Unitext.FileFormat.Bmp || frm == Pullenti.Unitext.FileFormat.Jpg || frm == Pullenti.Unitext.FileFormat.Jpg2000) || frm == Pullenti.Unitext.FileFormat.Png || frm == Pullenti.Unitext.FileFormat.Tif) || frm == Pullenti.Unitext.FileFormat.Gif || frm == Pullenti.Unitext.FileFormat.Emf) 
                return Pullenti.Unitext.FileFormatClass.Image;
            if ((((frm == Pullenti.Unitext.FileFormat.Doc || frm == Pullenti.Unitext.FileFormat.Docx || frm == Pullenti.Unitext.FileFormat.Xls) || frm == Pullenti.Unitext.FileFormat.Xlsx || frm == Pullenti.Unitext.FileFormat.Pptx) || frm == Pullenti.Unitext.FileFormat.Rtf || frm == Pullenti.Unitext.FileFormat.Odt) || frm == Pullenti.Unitext.FileFormat.Docxml || frm == Pullenti.Unitext.FileFormat.Xlsxml) 
                return Pullenti.Unitext.FileFormatClass.Office;
            if (frm == Pullenti.Unitext.FileFormat.Pdf || frm == Pullenti.Unitext.FileFormat.DjVu || frm == Pullenti.Unitext.FileFormat.Xps) 
                return Pullenti.Unitext.FileFormatClass.Pagelayout;
            return Pullenti.Unitext.FileFormatClass.Undefined;
        }
        /// <summary>
        /// Получить расширение для формата
        /// </summary>
        /// <param name="frm">формат</param>
        /// <return>расширение с точкой (null - если не знает)</return>
        public static string GetFormatExt(Pullenti.Unitext.FileFormat frm)
        {
            if (frm == Pullenti.Unitext.FileFormat.Doc) 
                return ".doc";
            if (frm == Pullenti.Unitext.FileFormat.Docx) 
                return ".docx";
            if (frm == Pullenti.Unitext.FileFormat.Docxml) 
                return ".doc";
            if (frm == Pullenti.Unitext.FileFormat.Pptx) 
                return ".pptx";
            if (frm == Pullenti.Unitext.FileFormat.Html) 
                return ".htm";
            if (frm == Pullenti.Unitext.FileFormat.Pdf) 
                return ".pdf";
            if (frm == Pullenti.Unitext.FileFormat.Rtf) 
                return ".rtf";
            if (frm == Pullenti.Unitext.FileFormat.Txt) 
                return ".txt";
            if (frm == Pullenti.Unitext.FileFormat.Xls || frm == Pullenti.Unitext.FileFormat.Xlsxml) 
                return ".xls";
            if (frm == Pullenti.Unitext.FileFormat.Xlsx) 
                return ".xlsx";
            if (frm == Pullenti.Unitext.FileFormat.Epub) 
                return ".epub";
            if (frm == Pullenti.Unitext.FileFormat.Fb2) 
                return ".fb2";
            if (frm == Pullenti.Unitext.FileFormat.Fb3) 
                return ".fb3";
            if (frm == Pullenti.Unitext.FileFormat.Csv) 
                return ".csv";
            if (frm == Pullenti.Unitext.FileFormat.Msg) 
                return ".msg";
            if (frm == Pullenti.Unitext.FileFormat.Eml) 
                return ".eml";
            if (frm == Pullenti.Unitext.FileFormat.Jpg) 
                return ".jpg";
            if (frm == Pullenti.Unitext.FileFormat.Jpg2000) 
                return ".jp2";
            if (frm == Pullenti.Unitext.FileFormat.Bmp) 
                return ".bmp";
            if (frm == Pullenti.Unitext.FileFormat.Gif) 
                return ".gif";
            if (frm == Pullenti.Unitext.FileFormat.Tif) 
                return ".tif";
            if (frm == Pullenti.Unitext.FileFormat.Png) 
                return ".png";
            if (frm == Pullenti.Unitext.FileFormat.Emf) 
                return ".emf";
            if (frm == Pullenti.Unitext.FileFormat.Xml) 
                return ".xml";
            if (frm == Pullenti.Unitext.FileFormat.Zip) 
                return ".zip";
            if (frm == Pullenti.Unitext.FileFormat.Rar) 
                return ".rar";
            if (frm == Pullenti.Unitext.FileFormat.Tar) 
                return ".tar";
            if (frm == Pullenti.Unitext.FileFormat.Gzip) 
                return ".gz";
            if (frm == Pullenti.Unitext.FileFormat.Zip7) 
                return ".7z";
            if (frm == Pullenti.Unitext.FileFormat.Xps) 
                return ".xps";
            if (frm == Pullenti.Unitext.FileFormat.DjVu) 
                return ".djvu";
            return null;
        }
        /// <summary>
        /// Проанализировать формат файла или байтового потока.
        /// </summary>
        /// <param name="fileName">имя файла, может быть null, если есть содержимое</param>
        /// <param name="content">содержимое, может быть null, тогда файл указывает на файл локальной файловой системы</param>
        public static Pullenti.Unitext.FileFormat AnalizeFileFormat(string fileName, byte[] content = null)
        {
            byte[] head = new byte[(int)2048];
            if (content != null) 
            {
                if (content.Length < 2) 
                    return Pullenti.Unitext.FileFormat.Unknown;
                for (int i = 0; ((i < head.Length) && (i < content.Length)); i++) 
                {
                    head[i] = content[i];
                }
            }
            else 
            {
                if (!File.Exists(fileName)) 
                    return Pullenti.Unitext.FileFormat.Unknown;
                using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
                {
                    if (f.Read(head, 0, head.Length) < 2) 
                        return Pullenti.Unitext.FileFormat.Unknown;
                }
            }
            string ext = null;
            try 
            {
                ext = (fileName == null ? "." : Path.GetExtension(fileName));
            }
            catch(Exception ex) 
            {
                ext = ".";
            }
            return AnalizeFormat(ext, head);
        }
    }
}