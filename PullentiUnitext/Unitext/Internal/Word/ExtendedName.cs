/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Unitext.Internal.Word
{
    // Provides name class for Compound File Storage.
    class ExtendedName
    {
        char[] m_Name;
        public char[] Name
        {
            get
            {
                return m_Name;
            }
        }
        public ExtendedName(string sname, char[] name, int offset = 0, int count = -1)
        {
            if (sname != null) 
                this.m_Name = sname.ToCharArray();
            else 
            {
                if (name == null) 
                    throw new ArgumentNullException("name");
                if (count < 0) 
                    count = name.Length;
                this.m_Name = new char[(int)count];
                Array.Copy(name, offset, this.m_Name, 0, count);
            }
        }
        public override string ToString()
        {
            return new string(m_Name);
        }
        public char[] ToCharArray()
        {
            return (char[])m_Name.Clone();
        }
        public string ToEscapedString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in m_Name) 
            {
                if ((ch < ' ') || ch == '\\') 
                    sb.Append("\\u").Append(((int)ch).ToString("X4"));
                else 
                    sb.Append(ch);
            }
            return sb.ToString();
        }
        public static ExtendedName FromString(string name)
        {
            return new ExtendedName(name, null, 0, -1);
        }
        public static ExtendedName FromEscapedString(string name)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (i < name.Length) 
            {
                if (name[i] != '\\') 
                    sb.Append(name[i]);
                else 
                {
                    if ((i + 6) > name.Length || name[i + 1] != 'u') 
                        throw new FormatException("Invalid escaped string format");
                    ushort code = ushort.Parse(name.Substring(i + 2, 4));
                    sb.Append((char)code);
                }
            }
            return new ExtendedName(sb.ToString(), null, 0, -1);
        }
        public override int GetHashCode()
        {
            int hash = m_Name.Length;
            for (int i = 0; i < m_Name.Length; i++) 
            {
                hash ^= m_Name[i];
            }
            return hash;
        }
        public override bool Equals(object obj)
        {
            return this == ((ExtendedName)obj);
        }
        public static bool operator ==(ExtendedName n1, ExtendedName n2)
        {
            if (n1.m_Name.Length != n2.m_Name.Length) 
                return false;
            for (int i = 0; i < n1.m_Name.Length; i++) 
            {
                if (n1.m_Name[i] != n2.m_Name[i]) 
                    return false;
            }
            return true;
        }
        public static bool operator !=(ExtendedName n1, ExtendedName n2)
        {
            return !(n1 == n2);
        }
    }
}