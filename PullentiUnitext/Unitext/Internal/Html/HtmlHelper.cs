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
    static class HtmlHelper
    {
        public static HtmlNode CreateNode(string fileName, byte[] fileContent = null, string contentType = null)
        {
            if (fileContent == null) 
                fileContent = Pullenti.Unitext.Internal.Uni.UnitextHelper.LoadDataFromFile(fileName, 0);
            if (fileContent == null) 
                return null;
            string str = Pullenti.Util.TextHelper.ReadStringFromBytes(fileContent, false);
            if (str == null) 
                return null;
            for (int k = 0; k < 2; k++) 
            {
                string str0 = (k == 0 ? contentType : str);
                if (str0 == null) 
                    continue;
                int i = str0.IndexOf("charset=");
                if (i > 0) 
                {
                    i += 8;
                    if (str0[i] == '\'' || str0[i] == '"') 
                        i++;
                    int j;
                    for (j = i; j < str0.Length; j++) 
                    {
                        if (!char.IsLetterOrDigit(str0[j]) && str0[j] != '-') 
                            break;
                    }
                    if (j > i) 
                    {
                        string encod = str0.Substring(i, j - i);
                        Pullenti.Util.EncodingWrapper wr = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.Undefined, encod);
                        str = wr.GetString(fileContent, 0, -1);
                        break;
                    }
                }
            }
            StringBuilder tmp = new StringBuilder(str);
            HtmlNode nod = HtmlParser.Parse(tmp, false);
            return nod;
        }
        public static Pullenti.Unitext.UnitextDocument Create(HtmlNode nod, string dirName, Dictionary<string, byte[]> images, Pullenti.Unitext.CreateDocumentParam pars)
        {
            UnitextHtmlGen gen = new UnitextHtmlGen();
            return gen.Create(nod, dirName, images, pars);
        }
    }
}