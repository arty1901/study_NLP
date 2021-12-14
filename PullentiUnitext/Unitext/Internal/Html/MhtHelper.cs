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

namespace Pullenti.Unitext.Internal.Html
{
    /// <summary>
    /// Поддержка формата MHT и EML
    /// </summary>
    static class MhtHelper
    {
        public static Pullenti.Unitext.UnitextDocument _createDoc(byte[] content, Pullenti.Unitext.CreateDocumentParam pars, bool isMht)
        {
            string txt = Pullenti.Util.MiscHelper.DecodeStringUtf8(content, 0, -1);
            return _createDocTxt(txt, pars, isMht, content);
        }
        public static Pullenti.Unitext.UnitextDocument _createDocTxt(string txt, Pullenti.Unitext.CreateDocumentParam pars, bool isMht, byte[] content)
        {
            int pos = 0;
            EmlPart p = EmlPart.TryParse(txt, ref pos);
            if (p == null) 
                return null;
            EmlPart p0 = p;
            HtmlNode nod = null;
            List<EmlPart> parts = new List<EmlPart>();
            parts.Add(p);
            Dictionary<string, byte[]> images = new Dictionary<string, byte[]>();
            Dictionary<string, string> imageTypes = new Dictionary<string, string>();
            while (pos < txt.Length) 
            {
                if (txt[pos] != '-') 
                {
                    pos++;
                    continue;
                }
                p = EmlPart.TryParse(txt, ref pos);
                if (p == null) 
                    break;
                if (p.ContentCharset != null && content != null) 
                {
                    try 
                    {
                        Pullenti.Util.EncodingWrapper ee = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, p.ContentCharset);
                        if (ee != null) 
                        {
                            txt = ee.GetString(content, 0, -1);
                            return _createDocTxt(txt, pars, isMht, null);
                        }
                    }
                    catch(Exception ex) 
                    {
                    }
                }
                if (p.Data != null && p.ContentType != null && p.ContentType.StartsWith("image")) 
                {
                    if (pars.OnlyForPureText) 
                        continue;
                    if (p.Filename == null) 
                    {
                        int ii = images.Count;
                        if (p.ContentLocation != null && !images.ContainsKey(p.ContentLocation)) 
                        {
                            images.Add(p.ContentLocation, p.Data);
                            if (p.ContentType != null) 
                                imageTypes.Add(p.ContentLocation, p.ContentType);
                        }
                        if (p.ContentId != null && !images.ContainsKey("cid:" + p.ContentId)) 
                        {
                            images.Add("cid:" + p.ContentId, p.Data);
                            if (p.ContentType != null) 
                                imageTypes.Add("cid:" + p.ContentId, p.ContentType);
                        }
                        if (images.Count > ii) 
                            continue;
                    }
                }
                parts.Add(p);
                if (p.ContentType == "text/html") 
                {
                    if (pars.IgnoreInnerDocuments) 
                        break;
                }
            }
            Pullenti.Unitext.UnitextDocument doc = null;
            for (int i = 0; i < parts.Count; i++) 
            {
                if (parts[i].ContentType == "text/html") 
                {
                    p = parts[i];
                    if (p.StringData != null) 
                    {
                        StringBuilder tmp = new StringBuilder(p.StringData);
                        nod = HtmlParser.Parse(tmp, false);
                    }
                    else if (p.Data != null) 
                        nod = HtmlHelper.CreateNode(null, p.Data, p.ContentCharset);
                    if (nod == null) 
                        continue;
                    doc = HtmlHelper.Create(nod, null, images, pars);
                    if (doc == null) 
                        continue;
                    parts.RemoveAt(i);
                    if (i > 0 && parts[i - 1].ContentType == "text/plain") 
                        parts.RemoveAt(i - 1);
                    break;
                }
            }
            if (doc == null) 
            {
                for (int i = 0; i < parts.Count; i++) 
                {
                    if (parts[i].ContentType == "text/plain") 
                    {
                        if (parts[i].StringData != null) 
                        {
                            doc = Pullenti.Unitext.UnitextService.CreateDocumentFromText(parts[i].StringData);
                            parts.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            if (doc == null) 
                doc = new Pullenti.Unitext.UnitextDocument();
            doc.SourceFormat = (isMht ? Pullenti.Unitext.FileFormat.Mht : Pullenti.Unitext.FileFormat.Eml);
            foreach (KeyValuePair<string, string> a in p0.Attrs) 
            {
                if (!doc.Attrs.ContainsKey(a.Key)) 
                    doc.Attrs.Add(a.Key, a.Value);
            }
            if (!isMht) 
            {
                int kk = 0;
                Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                foreach (string s in new string[] {"From", "To", "CC", "Date", "Subject"}) 
                {
                    if (doc.Attrs.ContainsKey(s)) 
                    {
                        gen.AppendText(string.Format("{0}: {1}", s, doc.Attrs[s]), false);
                        gen.AppendNewline(false);
                        kk++;
                    }
                }
                if (kk > 0) 
                {
                    gen.AppendNewline(false);
                    if (doc.Content != null) 
                        gen.Append(doc.Content, null, -1, false);
                    doc.Content = gen.Finish(true, null);
                    doc.Optimize(false, null);
                    doc.RefreshParents();
                }
            }
            if (pars.IgnoreInnerDocuments) 
                return doc;
            foreach (EmlPart pp in parts) 
            {
                if (pp.Data != null) 
                {
                    if (pp.Filename != null) 
                    {
                        try 
                        {
                            Pullenti.Unitext.UnitextDocument aa = Pullenti.Unitext.UnitextService.CreateDocument(pp.Filename, pp.Data, pars);
                            if (aa != null) 
                                doc.InnerDocuments.Add(aa);
                        }
                        catch(Exception ex) 
                        {
                        }
                    }
                }
            }
            return doc;
        }
        internal static string _decodeString(string val)
        {
            if (string.IsNullOrEmpty(val) || (val.Length < 3)) 
                return val;
            if (val.Length < 10) 
                return val;
            if (!val.Contains("=?")) 
                return val;
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < val.Length; i++) 
            {
                if (val[i] == '=' && ((i + 1) < val.Length) && val[i + 1] == '?') 
                {
                    int j;
                    i += 2;
                    for (j = i; j < val.Length; j++) 
                    {
                        if (val[j] == '?') 
                            break;
                    }
                    if (j >= val.Length) 
                        return val;
                    string charset = val.Substring(i, j - i).Trim();
                    Pullenti.Util.EncodingWrapper enc = null;
                    try 
                    {
                        enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, charset);
                    }
                    catch(Exception ex) 
                    {
                        return val;
                    }
                    i = j + 1;
                    char typ = '?';
                    for (++j; j < val.Length; j++) 
                    {
                        if (val[j] == '?') 
                            break;
                        else if (val[j] == 'Q' || val[j] == 'q') 
                            typ = 'Q';
                        else if (val[j] == 'B' || val[j] == 'b') 
                            typ = 'B';
                    }
                    if (typ != 'Q' && typ != 'B') 
                        return val;
                    i = j + 1;
                    for (++j; j < val.Length; j++) 
                    {
                        if (val[j] == '?' && ((j + 1) < val.Length) && val[j + 1] == '=') 
                            break;
                    }
                    string data = val.Substring(i, j - i).Trim();
                    byte[] bdata = null;
                    if (typ == 'B') 
                        bdata = Convert.FromBase64String(data);
                    else 
                    {
                        List<byte> bbb = new List<byte>();
                        for (int k = 0; k < data.Length; k++) 
                        {
                            if (data[k] != '=' || (k + 1) >= data.Length) 
                                bbb.Add((byte)data[k]);
                            else if ((k + 2) < data.Length) 
                            {
                                int v1 = EmlPart._toInt(data[k + 1]);
                                int v2 = EmlPart._toInt(data[k + 2]);
                                if (v1 >= 0 && v2 >= 0) 
                                {
                                    bbb.Add((byte)(((v1 << 4) | v2)));
                                    k += 2;
                                }
                                else 
                                    bbb.Add((byte)data[k]);
                            }
                        }
                        bdata = bbb.ToArray();
                    }
                    if (bdata == null) 
                        return val;
                    string ooo = enc.GetString(bdata, 0, -1);
                    res.Append(ooo);
                    i = j + 1;
                }
                else 
                    res.Append(val[i]);
            }
            return res.ToString();
        }
        internal static string _corrString(string val)
        {
            if (string.IsNullOrEmpty(val) || (val.Length < 3)) 
                return val;
            int i = val.IndexOf(';');
            if (i > 0) 
                val = val.Substring(0, i).Trim();
            if (val[0] == '"' && val[val.Length - 1] == '"') 
                val = val.Substring(1, val.Length - 2);
            return val;
        }
    }
}