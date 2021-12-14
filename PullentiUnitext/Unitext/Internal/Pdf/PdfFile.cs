/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Unitext.Internal.Pdf
{
    public class PdfFile : PdfObject, IDisposable
    {
        public PdfDictionary RootObject = null;
        public PdfDictionary Info = null;
        public PdfDictionary Encrypt = null;
        public List<PdfDictionary> Pages = new List<PdfDictionary>();
        public List<PdfDictionary> AllResources = new List<PdfDictionary>();
        public List<PdfObject> Objs = null;
        public void Open(string fileName, byte[] fileContent = null)
        {
            RootObject = null;
            Info = null;
            if (m_Stream != null) 
                m_Stream.Dispose();
            m_Stream = new PdfStream(fileName, fileContent);
            m_Head = m_Stream.ReadHead();
            int hpos = (int)m_Stream.Position;
            if (this._readXRefs()) 
            {
            }
            else 
            {
                int i;
                PdfObject rootObj = null;
                PdfObject infoObj = null;
                m_Stream.Position = hpos;
                while (true) 
                {
                    i = m_Stream.PeekSolidByte();
                    if (i < 0) 
                        break;
                    byte ch = (byte)i;
                    int p0 = (int)m_Stream.Position;
                    int id = 0;
                    ushort v = (ushort)0;
                    bool isObj = false;
                    string str;
                    PdfObject obj;
                    if (ch >= '0' && ch <= '9') 
                        isObj = m_Stream.TryReadIdAndVersion(false, out id, out v);
                    if (isObj) 
                    {
                        obj = null;
                        try 
                        {
                            obj = m_Stream.ParseObject(this, false);
                        }
                        catch(Exception ex) 
                        {
                        }
                        if (obj == null) 
                        {
                            m_Stream.Position++;
                            continue;
                        }
                        obj.Id = id;
                        obj.Version = v;
                        obj.SourceFile = this;
                        this._addObj(id, obj);
                        str = m_Stream.ReadWord(false);
                        continue;
                    }
                    str = m_Stream.ReadWord(false);
                    if (str == "xref") 
                    {
                        int i1 = 0;
                        str = m_Stream.ReadWord(false);
                        if (int.TryParse(str, out i1)) 
                        {
                            int i2 = 0;
                            str = m_Stream.ReadWord(false);
                            if (int.TryParse(str, out i2)) 
                            {
                                if (i2 > i1) 
                                    m_Stream.Position += (((i2 - i1)) * 20);
                            }
                        }
                        while (true) 
                        {
                            str = m_Stream.ReadWord(false);
                            if (string.IsNullOrEmpty(str) || str == "trailer") 
                                break;
                        }
                    }
                    if (str == "trailer") 
                    {
                        if ((((i = m_Stream.PeekSolidByte()))) < 0) 
                            break;
                        ch = (byte)i;
                        if (ch != '<') 
                            continue;
                        m_Stream.Position++;
                        if ((((i = m_Stream.ReadByte()))) < 0) 
                            break;
                        ch = (byte)i;
                        if (ch != '<') 
                            break;
                        PdfDictionary trailer = new PdfDictionary();
                        trailer.SourceFile = this;
                        trailer.PostParse(m_Stream);
                        if (rootObj == null) 
                            rootObj = trailer.GetObject("Root", true);
                        if (infoObj == null) 
                            infoObj = trailer.GetObject("Info", true);
                        if (trailer.GetObject("Encrypt", true) != null) 
                            Encrypt = trailer.GetDictionary("Encrypt", null);
                        continue;
                    }
                    int pos = (int)m_Stream.Position;
                    obj = m_Stream.ParseObject(this, false);
                    if (pos == ((int)m_Stream.Position)) 
                        m_Stream.Position++;
                }
                if (rootObj != null) 
                    RootObject = this.GetObject(rootObj.Id) as PdfDictionary;
                if (infoObj != null) 
                    Info = this.GetObject(infoObj.Id) as PdfDictionary;
            }
            for (int i = 1; i < m_IndirectObjs.Count; i++) 
            {
                if (m_IndirectObjs[i] is PdfDictionary) 
                {
                    if ((m_IndirectObjs[i] as PdfDictionary).IsTypeItem("ObjStm")) 
                    {
                        if (!this._readObjStm(m_IndirectObjs[i] as PdfDictionary)) 
                        {
                            if (this._checkEncrypt()) 
                                return;
                        }
                    }
                }
            }
            bool isLinearized = false;
            if (RootObject == null) 
            {
                for (int i = 1; i < m_IndirectObjs.Count; i++) 
                {
                    if (m_IndirectObjs[i] is PdfDictionary) 
                    {
                        if ((m_IndirectObjs[i] as PdfDictionary).GetObject("Linearized", false) != null) 
                        {
                            isLinearized = true;
                            break;
                        }
                    }
                }
            }
            this._readAllResources();
            if (isLinearized) 
            {
                PdfDictionary firstPage = null;
                int pos0 = m_Stream.Length;
                for (int i = 1; i < m_IndirectObjs.Count; i++) 
                {
                    if (m_IndirectObjs[i] is PdfDictionary) 
                    {
                        PdfDictionary dic = m_IndirectObjs[i] as PdfDictionary;
                        if (dic.IsTypeItem("Page")) 
                        {
                            Pages.Add(dic);
                            if (dic.m_FilePos > 0 && (dic.m_FilePos < pos0)) 
                            {
                                firstPage = dic;
                                pos0 = dic.m_FilePos;
                            }
                        }
                        else if (dic.IsTypeItem("Catalog")) 
                            RootObject = dic;
                        else if (dic.IsTypeItem("Info")) 
                            Info = dic;
                    }
                }
                if (firstPage != null && Pages[0] != firstPage) 
                {
                    Pages.Remove(firstPage);
                    Pages.Insert(0, firstPage);
                }
                return;
            }
            if (RootObject == null) 
            {
                for (int i = 1; i < m_IndirectObjs.Count; i++) 
                {
                    if (m_IndirectObjs[i] is PdfDictionary) 
                    {
                        if ((m_IndirectObjs[i] as PdfDictionary).IsTypeItem("Catalog")) 
                        {
                            RootObject = m_IndirectObjs[i] as PdfDictionary;
                            break;
                        }
                    }
                }
            }
            if (Info == null) 
            {
                for (int i = 1; i < m_IndirectObjs.Count; i++) 
                {
                    if (m_IndirectObjs[i] is PdfDictionary) 
                    {
                        if ((m_IndirectObjs[i] as PdfDictionary).IsTypeItem("Info")) 
                        {
                            Info = m_IndirectObjs[i] as PdfDictionary;
                            break;
                        }
                    }
                }
            }
            if (RootObject == null) 
            {
                for (int i = 1; i < m_IndirectObjs.Count; i++) 
                {
                    if (m_IndirectObjs[i] is PdfDictionary) 
                    {
                        if ((m_IndirectObjs[i] as PdfDictionary).IsTypeItem("XRef")) 
                        {
                            Objs = new List<PdfObject>();
                            foreach (PdfObject v in m_IndirectObjs) 
                            {
                                if (v != null) 
                                    Objs.Add(v);
                            }
                            break;
                        }
                    }
                }
            }
            if (RootObject != null) 
                RootObject.GetAllPages(Pages);
        }
        void _readAllResources()
        {
            for (int i = 1; i < m_IndirectObjs.Count; i++) 
            {
                if (m_IndirectObjs[i] is PdfDictionary) 
                {
                    PdfDictionary dic = m_IndirectObjs[i] as PdfDictionary;
                    string sub = dic.GetStringItem("Subtype");
                    if (sub == "Image" || sub == "Font") 
                        AllResources.Add(dic);
                }
            }
        }
        bool _checkEncrypt()
        {
            for (int i = 1; i < m_IndirectObjs.Count; i++) 
            {
                if (m_IndirectObjs[i] is PdfDictionary) 
                {
                    PdfDictionary dic = m_IndirectObjs[i] as PdfDictionary;
                    if (dic.IsTypeItem("Encrypt")) 
                    {
                        Encrypt = dic;
                        return true;
                    }
                    Encrypt = dic.GetDictionary("Encrypt", null);
                    if (Encrypt != null) 
                        return true;
                }
            }
            return false;
        }
        public void Dispose()
        {
            if (m_Stream != null) 
                m_Stream.Dispose();
            m_Stream = null;
        }
        public int MaxObjsCount
        {
            get
            {
                return m_IndirectObjs.Count;
            }
        }
        public PdfObject GetObject(int id)
        {
            if ((id < 1) || id >= m_IndirectObjs.Count) 
                return null;
            if (m_IndirectObjs[id] != null) 
                return m_IndirectObjs[id];
            if (m_IndirectObjDisp[id] == 0) 
                return null;
            m_Stream.Position = m_IndirectObjDisp[id];
            int uid = 0;
            ushort v = (ushort)0;
            if (!m_Stream.TryReadIdAndVersion(false, out uid, out v)) 
                return null;
            if (uid != id) 
                return null;
            PdfObject obj = null;
            try 
            {
                obj = m_Stream.ParseObject(this, false);
            }
            catch(Exception ex) 
            {
            }
            if (obj == null) 
                return null;
            obj.Id = id;
            obj.Version = v;
            m_IndirectObjs[id] = obj;
            return obj;
        }
        PdfStream m_Stream = null;
        byte[] m_Head;
        List<PdfObject> m_IndirectObjs = new List<PdfObject>();
        List<int> m_IndirectObjDisp = new List<int>();
        void _addObj(int id, PdfObject obj)
        {
            while (id >= m_IndirectObjs.Count) 
            {
                m_IndirectObjs.Add(null);
                m_IndirectObjDisp.Add(0);
            }
            m_IndirectObjs[id] = obj;
        }
        void _addIndir(int id, int disp)
        {
            while (id >= m_IndirectObjs.Count) 
            {
                m_IndirectObjs.Add(null);
                m_IndirectObjDisp.Add(0);
            }
            m_IndirectObjDisp[id] = disp;
        }
        bool _readXRefs()
        {
            if (m_Stream.Length < 100) 
                return false;
            m_Stream.Position = m_Stream.Length - 100;
            string str;
            while ((((str = m_Stream.ReadWord(true)))) != null) 
            {
                if (str == "startxref") 
                    break;
            }
            if (str == null) 
                return false;
            str = m_Stream.ReadWord(true);
            int disp;
            if (!int.TryParse(str, out disp)) 
                return false;
            PdfObject infObj = null;
            while (true) 
            {
                if ((disp < 0) || disp >= m_Stream.Length) 
                    return false;
                m_Stream.Position = disp;
                str = m_Stream.ReadWord(false);
                if (str != "xref") 
                    return false;
                while (true) 
                {
                    int id0;
                    int count;
                    int i;
                    if (!int.TryParse((str = m_Stream.ReadWord(true)), out id0)) 
                        break;
                    if (!int.TryParse(m_Stream.ReadWord(true), out count)) 
                        break;
                    for (i = 0; i < count; i++,id0++) 
                    {
                        string w = m_Stream.ReadWord(true);
                        if (!int.TryParse(w, out disp)) 
                            break;
                        m_Stream.ReadWord(true);
                        str = m_Stream.ReadWord(true);
                        if (str == null || str.Length != 1) 
                            break;
                        if (str == "n") 
                            this._addIndir(id0, disp);
                    }
                    if (i < count) 
                        break;
                }
                if (str != "trailer") 
                    return false;
                if (m_Stream.ReadWord(true) != "<") 
                    return false;
                if (m_Stream.ReadWord(true) != "<") 
                    return false;
                PdfDictionary trailer = new PdfDictionary();
                trailer.SourceFile = this;
                trailer.PostParse(m_Stream);
                if (trailer.GetObject("Encrypt", false) != null) 
                    Encrypt = trailer.GetDictionary("Encrypt", null);
                if (RootObject == null) 
                {
                    PdfObject oo = trailer.GetObject("Root", false);
                    if (oo != null) 
                        RootObject = this.GetObject(oo.Id) as PdfDictionary;
                }
                if (infObj == null) 
                    infObj = trailer.GetObject("Info", true);
                double d = (double)0;
                PdfIntValue prev = trailer.GetObject("Prev", false) as PdfIntValue;
                if (prev == null) 
                    break;
                disp = prev.Val;
            }
            if (infObj != null) 
                Info = this.GetObject(infObj.Id) as PdfDictionary;
            return RootObject != null;
        }
        bool _readObjStm(PdfDictionary dic)
        {
            byte[] dat = dic.ExtractData();
            if (dat == null || (dat.Length < 1)) 
                return false;
            int count = (int)dic.GetIntItem("N");
            if (count < 1) 
                return false;
            using (PdfStream pstr = new PdfStream(null, dat)) 
            {
                int[] ids = new int[(int)count];
                int[] disps = new int[(int)count];
                for (int i = 0; i < count; i++) 
                {
                    PdfIntValue id = pstr.ParseObject(this, false) as PdfIntValue;
                    if (id == null) 
                        break;
                    PdfIntValue disp = pstr.ParseObject(this, false) as PdfIntValue;
                    if (disp == null) 
                        break;
                    ids[i] = (int)id.Val;
                    disps[i] = (int)disp.Val;
                }
                int n0 = (int)pstr.Position;
                for (int i = 0; i < count; i++) 
                {
                    pstr.Position = n0 + disps[i];
                    PdfObject obj = pstr.ParseObject(this, false);
                    if (obj != null) 
                    {
                        obj.Id = ids[i];
                        obj.SourceFile = this;
                        this._addObj(obj.Id, obj);
                        if (obj is PdfDictionary) 
                        {
                            if ((obj as PdfDictionary).IsTypeItem("ObjStm")) 
                            {
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}