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

namespace Pullenti.Unitext.Internal.Misc
{
    class MyXmlReader
    {
        public static MyXmlReader Create(byte[] data)
        {
            MyXmlReader res = new MyXmlReader();
            int hlen = 0;
            Pullenti.Util.EncodingWrapper enc = Pullenti.Util.EncodingWrapper.CheckEncoding(data, out hlen);
            if (enc == null) 
                enc = new Pullenti.Util.EncodingWrapper(Pullenti.Util.EncodingStandard.ACSII);
            res.m_Text = enc.GetString(data, hlen, data.Length - hlen);
            res.m_Pos = 0;
            while (true) 
            {
                if (!res._readHead()) 
                    break;
            }
            return res;
        }
        void _gotoNonsp()
        {
            for (; m_Pos < m_Text.Length; m_Pos++) 
            {
                if (!char.IsWhiteSpace(m_Text[m_Pos])) 
                    break;
            }
        }
        bool _readHead()
        {
            this._gotoNonsp();
            if (((m_Pos + 10) < m_Text.Length) && m_Text[m_Pos] == '<' && m_Text[m_Pos + 1] == '?') 
            {
                m_Pos += 2;
                for (; m_Pos < m_Text.Length; m_Pos++) 
                {
                    if (m_Text[m_Pos] == '>' && m_Text[m_Pos - 1] == '?') 
                    {
                        m_Pos++;
                        return true;
                    }
                }
            }
            return false;
        }
        StringBuilder m_Tmp = new StringBuilder();
        StringBuilder m_Tmp2;
        static char m_Char0 = (char)0;
        string _readName()
        {
            this._gotoNonsp();
            m_Tmp.Length = 0;
            for (; m_Pos < m_Text.Length; m_Pos++) 
            {
                char ch = m_Text[m_Pos];
                if (char.IsLetter(ch) || ch == '_') 
                    m_Tmp.Append(ch);
                else if (m_Tmp.Length > 0 && ((char.IsDigit(ch) || ch == '-' || ch == ':'))) 
                    m_Tmp.Append(ch);
                else 
                    break;
            }
            if (m_Tmp.Length > 0) 
                return m_Tmp.ToString();
            return null;
        }
        char _readChar()
        {
            if (m_Pos >= m_Text.Length) 
                return m_Char0;
            char ch = m_Text[m_Pos];
            m_Pos++;
            if (ch != '&') 
                return ch;
            if (m_Pos >= m_Text.Length) 
                return ch;
            if (m_Tmp2 == null) 
                m_Tmp2 = new StringBuilder();
            m_Tmp2.Length = 0;
            for (; m_Pos < m_Text.Length; m_Pos++) 
            {
                if (m_Text[m_Pos] == ';') 
                {
                    m_Pos++;
                    break;
                }
                else 
                    m_Tmp2.Append(m_Text[m_Pos]);
            }
            string txt = m_Tmp2.ToString().ToUpper();
            if (txt == "LT") 
                return '<';
            if (txt == "GT") 
                return '>';
            if (txt == "AMP") 
                return '&';
            if (txt == "QUOT") 
                return '"';
            if (txt == "APOS") 
                return '\'';
            if (txt == "NBSP") 
                return (char)0xA0;
            if (txt.Length == 0) 
                return m_Char0;
            if (txt[0] == 'X') 
            {
                int cod = 0;
                for (int i = 1; i < txt.Length; i++) 
                {
                    if (char.IsDigit(txt[i])) 
                        cod = ((cod * 16) + ((int)txt[i])) - 0x30;
                    else if (((int)txt[i]) >= 0x41 && ((int)txt[i]) <= 0x46) 
                        cod = (((cod * 16) + ((int)txt[i])) - 0x41) + 10;
                }
                return (char)cod;
            }
            if (char.IsDigit(txt[0])) 
            {
                int cod = 0;
                for (int i = 1; i < txt.Length; i++) 
                {
                    if (char.IsDigit(txt[i])) 
                        cod = ((cod * 10) + ((int)txt[i])) - 0x30;
                }
                return (char)cod;
            }
            return txt[0];
        }
        string _readValue(char attrChar)
        {
            m_Tmp.Length = 0;
            for (; m_Pos < m_Text.Length; ) 
            {
                char ch = m_Text[m_Pos];
                if (attrChar == ch) 
                {
                    m_Pos++;
                    break;
                }
                if (attrChar == m_Char0) 
                {
                    if (ch == '<') 
                        break;
                }
                ch = this._readChar();
                if (ch != m_Char0) 
                    m_Tmp.Append(ch);
                else 
                    break;
            }
            return m_Tmp.ToString();
        }
        bool _readAttr(out string nam, out string val)
        {
            val = null;
            int pos = m_Pos;
            nam = this._readName();
            if (nam == null) 
                return false;
            this._gotoNonsp();
            if (m_Pos >= m_Text.Length || m_Text[m_Pos] != '=') 
                return false;
            m_Pos++;
            this._gotoNonsp();
            if (m_Pos < m_Text.Length) 
            {
                char ch = m_Text[m_Pos];
                if (ch == '"' || ch == '\'') 
                {
                    m_Pos++;
                    val = this._readValue(ch);
                    if (val != null) 
                    {
                        if ((m_Pos < m_Text.Length) && m_Text[m_Pos] == ch) 
                            m_Pos++;
                        return true;
                    }
                }
            }
            m_Pos = pos;
            return false;
        }
        string m_Text;
        int m_Pos;
        public bool Read()
        {
            Attributes.Clear();
            IsEmptyElement = false;
            LocalName = null;
            Value = null;
            NodeType = MyXmlNodeType.None;
            if (m_Pos >= m_Text.Length) 
                return false;
            char ch = m_Text[m_Pos];
            if (ch != '<') 
            {
                Value = this._readValue(m_Char0);
                if (Value != null) 
                {
                    NodeType = MyXmlNodeType.Text;
                    LocalName = "#text";
                    return true;
                }
                return false;
            }
            m_Pos++;
            if ((m_Pos + 3) > m_Text.Length) 
                return false;
            ch = m_Text[m_Pos];
            if (ch == '/') 
            {
                m_Pos++;
                LocalName = this._readName();
                this._gotoNonsp();
                if (LocalName != null && (m_Pos < m_Text.Length) && m_Text[m_Pos] == '>') 
                {
                    NodeType = MyXmlNodeType.EndElement;
                    m_Pos++;
                    return true;
                }
                return false;
            }
            LocalName = this._readName();
            if (LocalName == null) 
                return false;
            NodeType = MyXmlNodeType.Element;
            for (; m_Pos < m_Text.Length; ) 
            {
                this._gotoNonsp();
                if (m_Pos >= m_Text.Length) 
                    break;
                ch = m_Text[m_Pos];
                if (ch == '>') 
                {
                    m_Pos++;
                    break;
                }
                if (ch == '/' && ((m_Pos + 1) < m_Text.Length) && m_Text[m_Pos + 1] == '>') 
                {
                    IsEmptyElement = true;
                    m_Pos += 2;
                    break;
                }
                string nam;
                string val;
                if (this._readAttr(out nam, out val)) 
                {
                    if (!Attributes.ContainsKey(nam)) 
                        Attributes.Add(nam, val);
                }
                else 
                    break;
            }
            return true;
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (NodeType == MyXmlNodeType.Element) 
            {
                tmp.AppendFormat("<{0}", LocalName);
                foreach (KeyValuePair<string, string> kp in Attributes) 
                {
                    tmp.AppendFormat(" {0}=\"{1}\"", kp.Key, kp.Value);
                }
                if (IsEmptyElement) 
                    tmp.Append(" />");
                else 
                    tmp.Append(">");
            }
            else if (NodeType == MyXmlNodeType.EndElement) 
                tmp.AppendFormat("</{0}>", LocalName);
            else if (NodeType == MyXmlNodeType.Text) 
                tmp.AppendFormat("#text: {0}", Value ?? "");
            return tmp.ToString();
        }
        public MyXmlNodeType NodeType;
        public string LocalName;
        public string Value;
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public string GetAttribute(string nam)
        {
            if (Attributes.ContainsKey(nam)) 
                return Attributes[nam];
            return null;
        }
        public bool IsEmptyElement;
    }
}