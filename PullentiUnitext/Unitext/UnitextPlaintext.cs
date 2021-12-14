/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;
using System.Xml;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Фрагмент плоского текста.
    /// </summary>
    public class UnitextPlaintext : UnitextItem
    {
        /// <summary>
        /// Текст (без переходов на новую строку и спецсимволов, за исключением табуляции)
        /// </summary>
        public string Text;
        /// <summary>
        /// Тип текста
        /// </summary>
        public UnitextPlaintextType Typ = UnitextPlaintextType.Simple;
        /// <summary>
        /// Если есть нижележаший тексто-графический слой, то здесь ссылки для прямоугольники, для 
        /// каждого символа Text идёт ссылка на элемент, которому символ принадлежит. 
        /// То есть если Layout != null, то layout.Length = Text.Length.
        /// </summary>
        public UnilayRectangle[] Layout;
        public override UnitextItem Clone()
        {
            UnitextPlaintext res = new UnitextPlaintext();
            res._cloneFrom(this);
            res.Text = Text;
            res.Typ = Typ;
            res.Layout = Layout;
            return res;
        }
        public override bool IsWhitespaces
        {
            get
            {
                if (Text == null) 
                    return true;
                foreach (char ch in Text) 
                {
                    if (!char.IsWhiteSpace(ch)) 
                        return false;
                }
                return true;
            }
        }
        internal override string InnerTag
        {
            get
            {
                return "txt";
            }
        }
        internal void MergeWith(UnitextPlaintext pt)
        {
            if (pt.Text == null || Text == null) 
                return;
            if (pt.Layout != null || Layout != null) 
            {
                UnilayRectangle[] lay = new UnilayRectangle[(int)(pt.Text.Length + Text.Length)];
                if (Layout != null) 
                {
                    for (int i = 0; i < Layout.Length; i++) 
                    {
                        if (i < lay.Length) 
                            lay[i] = Layout[i];
                    }
                }
                if (pt.Layout != null) 
                {
                    for (int i = 0; i < pt.Layout.Length; i++) 
                    {
                        if ((Text.Length + i) < lay.Length) 
                            lay[Text.Length + i] = pt.Layout[i];
                    }
                }
                Layout = lay;
            }
            Text += pt.Text;
            if (pt.EndChar > EndChar) 
                EndChar = pt.EndChar;
        }
        internal UnitextPlaintext RemoveStart(int len)
        {
            if (Text.Length <= len) 
                return null;
            UnitextPlaintext b = new UnitextPlaintext();
            b.Parent = Parent;
            b.BeginChar = BeginChar;
            b.EndChar = (b.BeginChar + len) - 1;
            b.Text = Text.Substring(0, (b.EndChar - b.BeginChar) + 1);
            b.Typ = Typ;
            if (Id != null) 
                b.Id = string.Format("{0}_{1}", Id, EndChar);
            if (Layout != null) 
            {
                if (Layout.Length <= len) 
                    Layout = null;
                else 
                {
                    b.Layout = new UnilayRectangle[(int)len];
                    for (int i = 0; (i < len) && (i < Layout.Length); i++) 
                    {
                        b.Layout[i] = Layout[i];
                    }
                    UnilayRectangle[] lay = new UnilayRectangle[(int)(Layout.Length - len)];
                    for (int i = 0; i < lay.Length; i++) 
                    {
                        lay[i] = Layout[len + i];
                    }
                    Layout = lay;
                }
            }
            int le = (b.EndChar - b.BeginChar) + 1;
            Text = Text.Substring(le);
            BeginChar += le;
            return b;
        }
        internal UnitextPlaintext RemoveEnd(int len)
        {
            UnitextPlaintext e = new UnitextPlaintext();
            e.Parent = Parent;
            e.BeginChar = (EndChar + 1) - len;
            e.EndChar = (e.BeginChar + len) - 1;
            int dd = (((EndChar - BeginChar) + 1)) - (((e.EndChar - e.BeginChar) + 1));
            if (dd >= Text.Length) 
                return null;
            e.Text = Text.Substring(dd);
            e.Typ = Typ;
            if (Id != null) 
                e.Id = string.Format("{0}_{1}", Id, BeginChar);
            if (Layout != null) 
            {
                e.Layout = new UnilayRectangle[(int)len];
                for (int i = 0; i < len; i++) 
                {
                    int j = (Layout.Length - len) + i;
                    if (j >= 0 && (j < Layout.Length)) 
                        e.Layout[i] = Layout[j];
                }
                if ((Layout.Length - len) > 0) 
                {
                    UnilayRectangle[] lay = new UnilayRectangle[(int)(Layout.Length - len)];
                    for (int i = 0; i < lay.Length; i++) 
                    {
                        if (i < Layout.Length) 
                            lay[i] = Layout[i];
                    }
                    Layout = lay;
                }
                else 
                    Layout = null;
            }
            EndChar -= len;
            Text = Text.Substring(0, (EndChar - BeginChar) + 1);
            return e;
        }
        public override string ToString()
        {
            string ttt = ((Text ?? "")).TrimStart();
            if (ttt.Length > 100) 
                ttt = ttt.Substring(0, 100) + "...";
            if (Typ == UnitextPlaintextType.Simple) 
                return string.Format("'{0}'", ttt);
            else 
                return string.Format("<{0}>", ttt);
        }
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars)
        {
            if (pars != null && pars.SetPositions) 
            {
                BeginChar = res.Length;
                if (BeginChar == 793) 
                {
                }
            }
            if (pars != null && pars.SupTemplate != null && Typ == UnitextPlaintextType.Sup) 
            {
                string txt = ((Text ?? "")).Trim();
                if (!string.IsNullOrEmpty(txt)) 
                    res.Append(pars.SupTemplate.Replace("%1", txt));
            }
            else if (pars != null && pars.SubTemplate != null && Typ == UnitextPlaintextType.Sub) 
            {
                string txt = ((Text ?? "")).Trim();
                if (!string.IsNullOrEmpty(txt)) 
                    res.Append(pars.SubTemplate.Replace("%1", txt));
            }
            else if (Text != null) 
            {
                string txt = Text;
                if (pars != null && pars.PageBreak != null) 
                {
                    if (pars.PageBreak.Length == 1 && txt.IndexOf(pars.PageBreak[0]) >= 0) 
                        txt = txt.Replace(pars.PageBreak[0], ' ');
                }
                res.Append(txt);
            }
            if (pars != null && pars.SetPositions) 
                EndChar = res.Length - 1;
        }
        UnitextStyle _findStyle(int id)
        {
            for (UnitextItem it = Parent; it != null; it = it.Parent) 
            {
                UnitextDocument doc = it as UnitextDocument;
                if (doc != null) 
                {
                    if (id < doc.Styles.Count) 
                        return doc.Styles[id];
                }
            }
            return null;
        }
        public override void GetHtml(StringBuilder res, GetHtmlParam par)
        {
            if (!par.CallBefore(this, res)) 
                return;
            bool outSpan = false;
            if (par != null && Id != null && par.Styles.ContainsKey(Id)) 
            {
                res.AppendFormat("<span style=\"{0}\" id=\"{1}\"", par.Styles[Id], Id);
                outSpan = true;
            }
            else if (Id != null) 
            {
                res.AppendFormat("<span id=\"{0}\"", Id);
                outSpan = true;
            }
            if (par != null && par.OutBeginEndChars && BeginChar <= EndChar) 
            {
                if (!outSpan) 
                    res.Append("<span");
                outSpan = true;
                res.AppendFormat(" bc=\"{0}\" ec=\"{1}\"", BeginChar, EndChar);
            }
            if (outSpan) 
                res.Append('>');
            if (Typ == UnitextPlaintextType.Sub) 
                res.Append("<SUB>");
            else if (Typ == UnitextPlaintextType.Sup) 
                res.Append("<SUP>");
            bool textOuted = false;
            if ((par != null && par.OutStyles && this.GetStyledFragment(-1) != null) && EndChar >= BeginChar) 
            {
                UnitextStyledFragment[] map = new UnitextStyledFragment[(int)((EndChar + 1) - BeginChar)];
                bool hasInline = false;
                for (int i = BeginChar; i <= EndChar; i++) 
                {
                    UnitextStyledFragment ff = this.GetStyledFragment(i);
                    if (ff == null || (ff.StyleId < 0) || ff.Style == null) 
                        continue;
                    if (ff.Typ == UnitextStyledFragmentType.Inline) 
                    {
                    }
                    else if (ff.BeginChar >= BeginChar && ff.EndChar <= EndChar) 
                    {
                        if (Parent != null && Parent.GetStyledFragment(i) == ff) 
                            continue;
                    }
                    else 
                        continue;
                    hasInline = true;
                    map[i - BeginChar] = ff;
                }
                if (hasInline) 
                {
                    textOuted = true;
                    UnitextStyledFragment cur = null;
                    for (int i = 0; (i < map.Length) && (i < Text.Length); i++) 
                    {
                        if (map[i] == null) 
                        {
                            if (cur != null) 
                            {
                                cur = null;
                                res.Append("</span>");
                            }
                        }
                        else if (cur != map[i]) 
                        {
                            if (cur != null) 
                            {
                                cur = null;
                                res.Append("</span>");
                            }
                            cur = map[i];
                            res.Append("<span style=\"");
                            cur.Style.GetHtml(res);
                            res.Append("\">");
                        }
                        Pullenti.Util.MiscHelper.CorrectHtmlChar(res, Text[i]);
                    }
                    if (cur != null) 
                    {
                        cur = null;
                        res.Append("</span>");
                    }
                }
            }
            if (!textOuted) 
                Pullenti.Util.MiscHelper.CorrectHtmlValue(res, Text, false, false);
            if (Typ == UnitextPlaintextType.Sub) 
                res.Append("</SUB>");
            else if (Typ == UnitextPlaintextType.Sup) 
                res.Append("</SUP>");
            if (outSpan) 
                res.Append("</span>");
            par.CallAfter(this, res);
        }
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("text");
            this._writeXmlAttrs(xml);
            if (Typ != UnitextPlaintextType.Simple) 
                xml.WriteAttributeString("type", Typ.ToString().ToLower());
            string txt = Pullenti.Util.MiscHelper.CorrectXmlValue(Text);
            int sp = 0;
            foreach (char ch in txt) 
            {
                if (ch != ' ') 
                    break;
                else 
                    sp++;
            }
            if (sp > 0 && sp == txt.Length) 
                xml.WriteAttributeString("spaces", sp.ToString());
            try 
            {
                xml.WriteString(Pullenti.Util.MiscHelper.CorrectXmlValue(txt));
            }
            catch(Exception ex) 
            {
            }
            xml.WriteEndElement();
        }
        public override void FromXml(XmlNode xml)
        {
            base.FromXml(xml);
            int sp = 0;
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "type") 
                    {
                        try 
                        {
                            Typ = (UnitextPlaintextType)Enum.Parse(typeof(UnitextPlaintextType), a.Value, true);
                        }
                        catch(Exception ex336) 
                        {
                        }
                    }
                    else if (a.LocalName == "spaces") 
                        int.TryParse(a.Value, out sp);
                }
            }
            Text = xml.InnerText;
            if (sp == 1) 
                Text = " ";
            else if (sp == 2) 
                Text = "  ";
            else if (sp > 2) 
            {
                StringBuilder tmp = new StringBuilder(sp);
                for (int i = 0; i < sp; i++) 
                {
                    tmp.Append(' ');
                }
                Text = tmp.ToString();
            }
        }
        internal override void Correct(Pullenti.Unitext.Internal.Uni.LocCorrTyp typ, object data)
        {
            if (typ == Pullenti.Unitext.Internal.Uni.LocCorrTyp.Npsp2Sp) 
                Text = Pullenti.Unitext.Internal.Uni.UnitextCorrHelper.CorrNbsp(Text);
            else if (typ == Pullenti.Unitext.Internal.Uni.LocCorrTyp.TrimEnd) 
                Text = Text.TrimEnd();
        }
        internal override void SetDefaultTextPos(ref int cp, StringBuilder res)
        {
            BeginChar = cp;
            cp += Text.Length;
            EndChar = cp - 1;
            if (res != null) 
                res.Append(Text);
        }
    }
}