/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;
using System.Xml;

namespace Pullenti.Unitext.Internal.Word
{
    /// <summary>
    /// Это блок данных
    /// </summary>
    class DocxPart
    {
        public string Name;
        public XmlNode Xml;
        public byte[] Data;
        public bool IsName(string name)
        {
            if (string.Compare(name, Name, true) == 0) 
                return true;
            if (Name.StartsWith("/")) 
            {
                if (string.Compare(name, Name.Substring(1), true) == 0) 
                    return true;
            }
            return false;
        }
        public bool IsNameStarts(string name)
        {
            if (Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)) 
                return true;
            return false;
        }
        public Pullenti.Unitext.Internal.Misc.MyZipEntry ZipEntry;
        public override string ToString()
        {
            return Name;
        }
        public XmlNode GetXmlNode(bool correct = false)
        {
            if (Xml != null) 
                return Xml;
            if (Data != null) 
                return null;
            XmlDocument xml = new XmlDocument();
            byte[] dat = ZipEntry.GetData();
            if (dat == null) 
                return null;
            using (MemoryStream documentXml = new MemoryStream(dat)) 
            {
                xml.PreserveWhitespace = true;
                xml.Load(documentXml);
                return xml.DocumentElement;
            }
        }
        static string _getTagName(string xml, int i)
        {
            for (int j = i; j < xml.Length; j++) 
            {
                if (!char.IsLetter(xml[j]) && xml[j] != ':') 
                {
                    if (j == i) 
                        return null;
                    return xml.Substring(i, j - i);
                }
            }
            return null;
        }
        public byte[] GetBytes()
        {
            if (Data != null) 
                return Data;
            if (Xml != null) 
                return null;
            try 
            {
                return ZipEntry.GetData();
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
    }
}