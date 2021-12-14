/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Xml;

namespace Pullenti.Util
{
    /// <summary>
    /// Объекты, реализующие данный интерфейс, сериализуются одинаково на всех 
    /// языках программирования. См. MiscHelper.SerializeToBin и MiscHelper.DeserializeFromBin.
    /// </summary>
    public interface IXmlReadWriteSupport
    {
        /// <summary>
        /// Сериализация в Xml
        /// </summary>
        /// <param name="tagName">имя тега</param>
        void WriteToXml(XmlWriter xml, string tagName);
        /// <summary>
        /// Десериализация из узла Xml
        /// </summary>
        /// <param name="xml">узел</param>
        void ReadFromXml(XmlNode xml);
    }
}