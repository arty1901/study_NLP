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
    class HtmlParserNode
    {
        public int IndexFrom;
        public int IndexTo;
        public string TagName;
        public enum TagTypes : int
        {
            Undefined,
            Open,
            Close,
            OpenClose,
        }

        public TagTypes TagType;
        public HtmlTag Tag;
        public int CloseTagIndex;
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public string PureText;
        public bool WhitespacePreserve;
        internal bool Printed;
        public bool IsEmpty
        {
            get
            {
                if (TagName != null) 
                    return false;
                if (Attributes != null && Attributes.Count > 0) 
                    return false;
                if (!string.IsNullOrEmpty(PureText)) 
                {
                    if (WhitespacePreserve) 
                        return false;
                    for (int i = 0; i < PureText.Length; i++) 
                    {
                        if (!char.IsWhiteSpace(PureText[i])) 
                            return false;
                    }
                }
                return true;
            }
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            if (TagType != TagTypes.Undefined) 
            {
                if (TagType == TagTypes.Close) 
                    res.AppendFormat("</{0}>", TagName);
                else 
                {
                    res.AppendFormat("<{0}", TagName);
                    if (Attributes != null) 
                    {
                        foreach (KeyValuePair<string, string> kp in Attributes) 
                        {
                            res.AppendFormat(" {0}='{1}'", kp.Key, kp.Value);
                        }
                    }
                    if (TagType == TagTypes.Open) 
                        res.Append(">");
                    else 
                        res.Append("/>");
                }
            }
            else if (PureText != null) 
                res.Append(PureText);
            return res.ToString();
        }
        public bool Analize(string html, int indFrom, string endTag, bool whiteSpacePreserve)
        {
            if (indFrom >= html.Length) 
                return false;
            IndexFrom = indFrom;
            TagName = null;
            TagType = TagTypes.Undefined;
            if (Attributes == null) 
                Attributes = new Dictionary<string, string>();
            else 
                Attributes.Clear();
            PureText = null;
            int i;
            int j;
            char ch = html[indFrom];
            if (ch != '<' || ((endTag != null && !IsEndTag(html, indFrom, endTag)))) 
            {
                StringBuilder strTmp = null;
                bool comment = false;
                int lastNbsp = 0;
                for (i = indFrom; i < html.Length; i++) 
                {
                    bool nbr = false;
                    if (html[i] == '<') 
                    {
                        if (!comment) 
                        {
                            if (endTag == null || IsEndTag(html, i, endTag)) 
                                break;
                        }
                        if ((((i + 3) < html.Length) && html[i + 1] == '!' && html[i + 2] == '-') && html[i + 3] == '-') 
                        {
                            comment = true;
                            i += 3;
                            continue;
                        }
                        ch = html[i];
                        j = 1;
                    }
                    else 
                    {
                        if (comment && html[i] == '-') 
                        {
                            if (((i + 2) < html.Length) && html[i + 1] == '-' && html[i + 2] == '>') 
                            {
                                comment = false;
                                i += 2;
                                continue;
                            }
                        }
                        if ((((j = parseChar(html, i, out ch, out nbr)))) < 1) 
                            break;
                    }
                    if (char.IsWhiteSpace(ch)) 
                        ch = ' ';
                    if (ch == ' ' && !whiteSpacePreserve) 
                    {
                        bool prevSp = (strTmp == null ? true : strTmp[strTmp.Length - 1] == ' ');
                        if (!prevSp || nbr) 
                        {
                            if (strTmp == null) 
                                strTmp = new StringBuilder();
                            strTmp.Append(ch);
                        }
                        if (nbr) 
                            lastNbsp++;
                    }
                    else 
                    {
                        lastNbsp = 0;
                        if (strTmp == null) 
                            strTmp = new StringBuilder();
                        if (nbr && whiteSpacePreserve && ch == ' ') 
                            ch = (char)0xA0;
                        strTmp.Append(ch);
                    }
                    if (whiteSpacePreserve) 
                        WhitespacePreserve = true;
                    i += (j - 1);
                }
                if (lastNbsp == 0 && !whiteSpacePreserve && strTmp != null) 
                {
                    for (int jj = strTmp.Length - 1; jj > 0; jj--) 
                    {
                        if (strTmp[jj] != ' ') 
                            break;
                        else 
                            strTmp.Length--;
                    }
                }
                if (strTmp != null) 
                    PureText = strTmp.ToString();
                IndexTo = i - 1;
                return true;
            }
            if ((indFrom + 2) >= html.Length) 
                return false;
            ch = html[indFrom + 1];
            if (char.IsLetter(ch)) 
            {
                TagName = ReadLatinWord(html, indFrom + 1);
                if (TagName == null) 
                {
                    IndexTo = indFrom + 1;
                    return true;
                }
                if (TagName == "BR") 
                {
                }
                TagType = TagTypes.Open;
                StringBuilder strTmp = null;
                for (i = indFrom + 1 + TagName.Length; i < html.Length; i++) 
                {
                    ch = html[i];
                    if (char.IsWhiteSpace(ch)) 
                        continue;
                    if (ch == '>') 
                    {
                        i++;
                        break;
                    }
                    if (ch == '/') 
                    {
                        if ((i + 1) >= html.Length) 
                            return false;
                        if (html[i + 1] == '>') 
                            i++;
                        TagType = TagTypes.OpenClose;
                        i++;
                        break;
                    }
                    string attrName = ReadLatinWord(html, i);
                    if (attrName == null) 
                        continue;
                    i += attrName.Length;
                    if ((i + 2) >= html.Length || html[i] != '=') 
                    {
                        i--;
                        continue;
                    }
                    i++;
                    if (strTmp != null) 
                        strTmp.Length = 0;
                    char bracket = (char)0;
                    if (html[i] == '\'' || html[i] == '"') 
                    {
                        bracket = html[i];
                        i++;
                    }
                    for (; i < html.Length; i++) 
                    {
                        ch = html[i];
                        if (bracket != ((char)0)) 
                        {
                            if (ch == bracket) 
                            {
                                i++;
                                break;
                            }
                        }
                        else if (ch == '>') 
                            break;
                        else if (char.IsWhiteSpace(ch)) 
                            break;
                        else if (ch == '/' && ((i + 1) < html.Length) && html[i + 1] == '>') 
                        {
                        }
                        bool nbr;
                        if ((((j = parseChar(html, i, out ch, out nbr)))) < 1) 
                        {
                            ch = html[i];
                            j = 1;
                        }
                        if (strTmp == null) 
                            strTmp = new StringBuilder();
                        strTmp.Append(ch);
                        i += (j - 1);
                    }
                    i--;
                    if (!Attributes.ContainsKey(attrName) && strTmp != null && strTmp.Length > 0) 
                        Attributes.Add(attrName, strTmp.ToString());
                }
                IndexTo = i - 1;
                return true;
            }
            if (html[indFrom + 1] == '/') 
            {
                i = indFrom + 2;
                TagName = ReadLatinWord(html, i);
                if (TagName != null) 
                {
                    i += TagName.Length;
                    if ((i < html.Length) && html[i] == '>') 
                    {
                        i++;
                        TagType = TagTypes.Close;
                    }
                }
                IndexTo = i - 1;
                return true;
            }
            if ((((indFrom + 5) < html.Length) && html[indFrom + 1] == '!' && html[indFrom + 2] == '-') && html[indFrom + 3] == '-') 
            {
                for (i = indFrom + 4; i < (html.Length - 3); i++) 
                {
                    if (html[i] == '-' && html[i + 1] == '-' && html[i + 2] == '>') 
                    {
                        IndexTo = i + 2;
                        return true;
                    }
                }
                return false;
            }
            for (i = indFrom; i < html.Length; i++) 
            {
                if (html[i] == '>') 
                {
                    i++;
                    break;
                }
            }
            IndexTo = i - 1;
            return true;
        }
        static int parseChar(string txt, int ind, out char ch, out bool nbsp)
        {
            ch = (char)0;
            nbsp = false;
            if (ind >= txt.Length) 
                return -1;
            if (txt[ind] != '&') 
            {
                ch = txt[ind];
                return 1;
            }
            StringBuilder strTmp = new StringBuilder();
            int i;
            for (i = ind + 1; i < txt.Length; i++) 
            {
                if (txt[i] == ';') 
                    break;
                else 
                {
                    strTmp.Append(txt[i]);
                    if (strTmp.Length > 7) 
                        break;
                }
            }
            if (i >= txt.Length || txt[i] != ';') 
            {
                if (strTmp.ToString().StartsWith("nbsp")) 
                {
                    ch = ' ';
                    nbsp = true;
                    return 5;
                }
                ch = '&';
                return 1;
            }
            int ret = (i + 1) - ind;
            if (strTmp.Length == 0) 
                return ret;
            string s = strTmp.ToString();
            if (m_SpecChars.ContainsKey(s)) 
            {
                ch = m_SpecChars[s];
                if (s == "nbsp") 
                    nbsp = true;
                return ret;
            }
            if (s[0] != '#' || (strTmp.Length < 2)) 
            {
                ch = '&';
                return ret;
            }
            if (char.IsDigit(s[1])) 
            {
                try 
                {
                    ch = (char)int.Parse(s.Substring(1));
                    nbsp = char.IsWhiteSpace(ch);
                }
                catch(Exception ex14) 
                {
                }
                return ret;
            }
            if (s[1] == 'x' || s[1] == 'X') 
            {
                try 
                {
                    int code = 0;
                    s = s.ToUpper();
                    for (int ii = 2; ii < s.Length; ii++) 
                    {
                        if (char.IsDigit(s[ii])) 
                            code = (code << 4) + ((int)((s[ii] - '0')));
                        else if (s[ii] >= 'A' && s[ii] <= 'F') 
                            code = (code << 4) + ((int)(((s[ii] - 'A') + 10)));
                    }
                    ch = (char)code;
                    nbsp = char.IsWhiteSpace(ch);
                }
                catch(Exception ex15) 
                {
                }
                return ret;
            }
            ch = s[1];
            return ret;
        }
        static string ReadLatinWord(string str, int ind)
        {
            if ((ind + 1) >= str.Length) 
                return null;
            int i;
            for (i = ind; i < str.Length; i++) 
            {
                if (!char.IsLetter(str[i]) && str[i] != ':' && str[i] != '_') 
                {
                    if (i == 0) 
                        break;
                    if (!char.IsDigit(str[i]) && str[i] != '.' && str[i] != '-') 
                        break;
                }
                else 
                {
                    int cod = (int)str[i];
                    if (cod > 0x80) 
                        break;
                }
            }
            if (i <= ind) 
                return null;
            return str.Substring(ind, i - ind).ToUpper();
        }
        static bool IsEndTag(string html, int pos, string tagName)
        {
            if ((pos + 3) >= html.Length || tagName == null) 
                return false;
            if (html[pos] != '<' && html[pos + 1] != '/') 
                return false;
            if (ReadLatinWord(html, pos + 2) != tagName) 
                return false;
            return true;
        }
        static Dictionary<string, char> m_SpecChars;
        static HtmlParserNode()
        {
            m_SpecChars = new Dictionary<string, char>();
            m_SpecChars.Add("nbsp", ' ');
            m_SpecChars.Add("lt", '<');
            m_SpecChars.Add("gt", '>');
            m_SpecChars.Add("quot", '"');
            m_SpecChars.Add("apos", '\'');
            string data = "iexcl=161;cent=162;pound=163;curren=164;yen=165;brvbar=166;sect=167;uml=168;copy=169;ordf=170;laquo=171;not=172;shy=173;reg=174;macr=175;deg=176;plusmn=177;sup2=178;sup3=179;acute=180;micro=181;para=182;middot=183;cedil=184;sup1=185;ordm=186;raquo=187;frac14=188;frac12=189;frac34=190;iquest=191;Agrave=192;Aacute=193;Acirc=194;Atilde=195;Auml=196;Aring=197;AElig=198;Ccedil=199;Egrave=200;Eacute=201;Ecirc=202;Euml=203;Igrave=204;Iacute=205;Icirc=206;Iuml=207;ETH=208;Ntilde=209;Ograve=210;Oacute=211;Ocirc=212;Otilde=213;Ouml=214;times=215;Oslash=216;Ugrave=217;Uacute=218;Ucirc=219;Uuml=220;Yacute=221;THORN=222;szlig=223;agrave=224;aacute=225;acirc=226;atilde=227;auml=228;aring=229;aelig=230;ccedil=231;egrave=232;eacute=233;ecirc=234;euml=235;igrave=236;iacute=237;icirc=238;iuml=239;eth=240;ntilde=241;ograve=242;oacute=243;ocirc=244;otilde=245;ouml=246;divide=247;oslash=248;ugrave=249;uacute=250;ucirc=251;uuml=252;yacute=253;thorn=254;yuml=255;Alpha=913;Beta=914;Gamma=915;Delta=916;Epsilon=917;Zeta=918;Eta=919;Theta=920;Iota=921;Kappa=922;Lambda=923;Mu=924;Nu=925;Xi=926;Omicron=927;Pi=928;Rho=929;Sigma=931;Tau=932;Upsilon=933;Phi=934;Chi=935;Psi=936;Omega=937;alpha=945;beta=946;gamma=947;delta=948;epsilon=949;zeta=950;eta=951;theta=952;iota=953;kappa=954;lambda=955;mu=956;nu=957;xi=958;omicron=959;pi=960;rho=961;sigmaf=962;sigma=963;tau=964;upsilon=965;phi=966;chi=967;psi=968;omega=969;thetasym=977;upsih=978;piv=982;bull=8226;hellip=8230;prime=8242;Prime=8243;oline=8254;frasl=8260;weierp=8472;image=8465;real=8476;trade=8482;alefsym=8501;larr=8592;uarr=8593;rarr=8594;darr=8595;harr=8596;crarr=8629;lArr=8656;uArr=8657;rArr=8658;dArr=8659;hArr=8660;forall=8704;part=8706;exist=8707;empty=8709;nabla=8711;isin=8712;notin=8713;ni=8715;prod=8719;sum=8721;minus=8722;lowast=8727;radic=8730;prop=8733;infin=8734;ang=8736;and=8743;or=8744;cap=8745;cup=8746;int=8747;there4=8756;sim=8764;cong=8773;asymp=8776;ne=8800;equiv=8801;le=8804;ge=8805;sub=8834;sup=8835;nsub=8836;sube=8838;supe=8839;oplus=8853;otimes=8855;perp=8869;sdot=8901;lceil=8968;rceil=8969;lfloor=8970;rfloor=8971;lang=9001;rang=9002;loz=9674;spades=9824;clubs=9827;hearts=9829;diams=9830;amp=38;OElig=338;oelig=339;Scaron=352;scaron=353;Yuml=376;circ=710;tilde=732;ensp=8194;emsp=8195;thinsp=8201;zwnj=8204;zwj=8205;lrm=8206;rlm=8207;ndash=8211;mdash=8212;lsquo=8216;rsquo=8217;sbquo=8218;ldquo=8220;rdquo=8221;bdquo=8222;dagger=8224;Dagger=8225;permil=8240;lsaquo=8249;rsaquo=8250;euro=8364";
            foreach (string s in data.Split(';')) 
            {
                int i = s.IndexOf('=');
                string key = s.Substring(0, i);
                int cod = int.Parse(s.Substring(i + 1));
                m_SpecChars.Add(key, (char)cod);
            }
        }
    }
}