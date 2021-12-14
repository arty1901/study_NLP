/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pullenti.Unitext.Internal.Word
{
    // Word document model.
    // See specification for explanations.
    // [MS-DOC]: Word Binary File Format (.doc) Structure Specification
    // Release: Thursday, August 27, 2009
    class WordDocument
    {
        public StringBuilder Text = new StringBuilder();
        FileCharacterPosition[] fileCharacterPositions;
        List<Paragraph> m_Paragraphs = new List<Paragraph>();
        CharacterFormatting[] m_Formattings;
        Dictionary<ushort, StyleDefinition> m_StyleDefinitionsMap;
        Prl[] defaultPrls;
        Dictionary<int, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle> m_NumStyles = new Dictionary<int, Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle>();
        public string Comment;
        bool m_Error = false;
        public IEnumerable<Paragraph> Paragraphs
        {
            get
            {
                return m_Paragraphs;
            }
        }
        public IEnumerable<CharacterFormatting> Formattings
        {
            get
            {
                return m_Formattings;
            }
        }
        public IEnumerable<StyleDefinition> StyleDefinitions
        {
            get
            {
                return m_StyleDefinitionsMap.Values;
            }
        }
        public IDictionary<ushort, StyleDefinition> StyleDefinitionsMap
        {
            get
            {
                return m_StyleDefinitionsMap;
            }
        }
        public void Load(CompoundFileSystem system, Pullenti.Unitext.UnitextDocument doc = null)
        {
            string WordDocumentStreamName = "WordDocument";
            CompoundFileStorage wordDocumentStorage = system.GetRootStorage().FindStorage(ExtendedName.FromString(WordDocumentStreamName));
            using (Stream wordDocumentStream = wordDocumentStorage.CreateStream()) 
            {
                Fib fib;
                fib = FibStructuresReader.ReadFib(wordDocumentStream);
                if (fib != null) 
                {
                    string tableStreamName = GetTableStreamName(fib);
                    CompoundFileStorage tableStorage = system.GetRootStorage().FindStorage(ExtendedName.FromString(tableStreamName));
                    if (tableStorage != null) 
                    {
                        using (Stream tableStream = tableStorage.CreateStream()) 
                        {
                            this.LoadContent(wordDocumentStream, tableStream, fib, doc);
                        }
                    }
                }
                else 
                {
                    wordDocumentStream.Position = 0;
                    byte[] buf = new byte[(int)wordDocumentStream.Length];
                    int i = wordDocumentStream.Read(buf, 0, buf.Length);
                    if (i < 0x20) 
                        return;
                    Pullenti.Unitext.UnitextDocument ddd = MSOfficeHelper._uniFromWord6orEarly(buf);
                    if (ddd != null) 
                        doc.Content = ddd.Content;
                }
            }
        }
        static string GetTableStreamName(Fib fib)
        {
            return (fib.@base.fWhichTblStm ? "1Table" : "0Table");
        }
        Dictionary<int, Pullenti.Unitext.UnitextFootnote> m_Footnotes = new Dictionary<int, Pullenti.Unitext.UnitextFootnote>();
        Dictionary<int, Pullenti.Unitext.UnitextComment> m_Comments = new Dictionary<int, Pullenti.Unitext.UnitextComment>();
        Dictionary<int, string> m_SuperTexts = new Dictionary<int, string>();
        Dictionary<int, string> m_SubTexts = new Dictionary<int, string>();
        Dictionary<int, Pullenti.Unitext.UnitextContainer> m_Shapes = new Dictionary<int, Pullenti.Unitext.UnitextContainer>();
        byte[] m_Dels;
        private void LoadContent(Stream wordDocumentStream, Stream tableStream, Fib fib, Pullenti.Unitext.UnitextDocument doc)
        {
            FibRgLw97 fib97 = fib.fibRgLw as FibRgLw97;
            if (fib97 == null) 
                return;
            FibRbFcLcb97 fibLc97 = fib.fibRgFcLcbBlob as FibRbFcLcb97;
            if (fibLc97 == null) 
                return;
            int pos = (int)fibLc97.fcClx;
            tableStream.Position = pos;
            int iii = (int)fibLc97.lcbClx;
            Clx clx = BasicTypesReader.ReadClx(tableStream, iii);
            this.LoadCharacters(wordDocumentStream, clx);
            try 
            {
                uint plcfBtePapxOffset = ((FibRbFcLcb97)fib.fibRgFcLcbBlob).fcPlcfBtePapx;
                tableStream.Position = plcfBtePapxOffset;
                uint plcfBtePapxLength = ((FibRbFcLcb97)fib.fibRgFcLcbBlob).lcbPlcfBtePapx;
                PlcBtePapx plcfBtePapx = BasicTypesReader.ReadPlcfBtePapx(tableStream, plcfBtePapxLength);
                this.LoadParagraphs(wordDocumentStream, clx, plcfBtePapx);
                uint plcfBteChpxOffset = ((FibRbFcLcb97)fib.fibRgFcLcbBlob).fcPlcfBteChpx;
                tableStream.Position = plcfBteChpxOffset;
                uint plcfBteChpxLength = ((FibRbFcLcb97)fib.fibRgFcLcbBlob).lcbPlcfBteChpx;
                PlcBteChpx plcBteChpx = BasicTypesReader.ReadPlcBteChpx(tableStream, plcfBteChpxLength);
                this.LoadCharacterFormatting(wordDocumentStream, plcBteChpx);
                uint stshOffset = ((FibRbFcLcb97)fib.fibRgFcLcbBlob).fcStshf;
                tableStream.Position = stshOffset;
                uint stshLength = ((FibRbFcLcb97)fib.fibRgFcLcbBlob).lcbStshf;
                STSH stsh = BasicTypesReader.ReadStsh(tableStream, stshLength);
                this.LoadStsh(wordDocumentStream, stsh);
                tableStream.Position = fibLc97.fcPlfLfo;
                this.LoadLFOS(tableStream, (int)fibLc97.lcbPlfLfo);
                tableStream.Position = fibLc97.fcPlfLst;
                this.LoadLSTFs(tableStream, (int)fibLc97.lcbPlfLst);
            }
            catch(Exception ex) 
            {
            }
            if (doc != null) 
            {
                m_Dels = new byte[(int)Text.Length];
                if (m_Formattings != null) 
                {
                    foreach (CharacterFormatting f in m_Formattings) 
                    {
                        if (f.Length == 1) 
                        {
                            byte[] dat = f.GetProperty(SinglePropertyModifiers.sprmCSymbol);
                            if (dat != null && dat.Length == 4) 
                            {
                                uint nnn = (uint)dat[3];
                                nnn <<= 8;
                                nnn |= dat[2];
                                if (nnn > 0xF000) 
                                    nnn -= 0xF000;
                                if (f.Pos >= 0 && (f.Pos < Text.Length)) 
                                {
                                    char ch0 = Text[f.Pos];
                                    Text[f.Pos] = Pullenti.Unitext.Internal.Misc.WingdingsHelper.GetUnicode((int)nnn);
                                    if (Text[f.Pos] == ((char)0)) 
                                        Text[f.Pos] = (char)nnn;
                                }
                            }
                        }
                        StyleCollection sty = f.GetStyles(FormattingLevel.Character);
                        if (sty != null) 
                        {
                            if (sty.ToString().Contains("CFRMarkDel")) 
                            {
                                for (int ii = f.Pos; ii < (f.Pos + f.Length); ii++) 
                                {
                                    m_Dels[ii] = (byte)1;
                                }
                            }
                        }
                    }
                }
                string totalText = Text.ToString();
                Pullenti.Unitext.Internal.Uni.UnitextGen gen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                if (m_Paragraphs == null || m_Paragraphs.Count == 0) 
                {
                    gen.AppendText(totalText, false);
                    doc.Content = gen.Finish(true, null);
                    return;
                }
                if (m_Formattings != null) 
                {
                    for (int i = 0; i < m_Formattings.Length; i++) 
                    {
                        CharacterFormatting f = m_Formattings[i];
                        byte[] dat = f.GetProperty(SinglePropertyModifiers.sprmCIss);
                        if (f.Pos > (84223 - 10) && (f.Pos < (84223 + 10))) 
                        {
                        }
                        if ((dat != null && dat.Length == 1 && dat[0] == 1) && (f.Length < 10)) 
                        {
                            string tmp = totalText.Substring(f.Pos, f.Length);
                            if (!m_SuperTexts.ContainsKey(f.Pos)) 
                                m_SuperTexts.Add(f.Pos, tmp);
                        }
                        if ((dat != null && dat.Length == 1 && dat[0] == 2) && (f.Length < 10)) 
                        {
                            string tmp = totalText.Substring(f.Pos, f.Length);
                            if (!m_SubTexts.ContainsKey(f.Pos)) 
                                m_SubTexts.Add(f.Pos, tmp);
                        }
                    }
                }
                for (int k = 0; k < 2; k++) 
                {
                    tableStream.Position = (k == 0 ? fibLc97.fcPlcffndRef : fibLc97.fcPlcfendRef);
                    List<int> cps = BasicTypesReader.ReadCPs(tableStream, (int)((k == 0 ? fibLc97.lcbPlcffndRef : fibLc97.lcbPlcfendRef)));
                    tableStream.Position = (k == 0 ? fibLc97.fcPlcffndTxt : fibLc97.fcPlcfendTxt);
                    List<int> cps2 = BasicTypesReader.ReadCPs(tableStream, (int)((k == 0 ? fibLc97.lcbPlcffndTxt : fibLc97.lcbPlcfendTxt)));
                    if (cps.Count > 2 && gen != null) 
                    {
                        int pos0 = (int)fib97.ccpText;
                        int len = (int)fib97.ccpFtn;
                        if (k == 1) 
                        {
                            pos0 += ((int)((fib97.ccpFtn + fib97.ccpHdd + fib97.ccpAtn)));
                            len = (int)fib97.ccpEdn;
                        }
                        string ttt = totalText.Substring(pos0, len);
                        for (int i = 0; (i < (cps.Count - 2)) && (i < cps2.Count); i++) 
                        {
                            int cp = cps[i];
                            if ((cp < 0) || cp >= pos0) 
                                continue;
                            Pullenti.Unitext.Internal.Uni.UnitextGen ge = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            int max = ((i + 1) < (cps2.Count - 2) ? cps2[i + 1] : len);
                            this._createUni(ge, null, 0, pos0 + cps2[i], pos0 + max, totalText, 0);
                            if (!m_Footnotes.ContainsKey(cp)) 
                            {
                                Pullenti.Unitext.UnitextFootnote fn = new Pullenti.Unitext.UnitextFootnote() { IsEndnote = k == 1, Content = ge.Finish(true, null) };
                                m_Footnotes.Add(cp, fn);
                            }
                        }
                    }
                }
                tableStream.Position = fibLc97.fcPlcfandTxt;
                List<int> cpsat2 = BasicTypesReader.ReadCPs(tableStream, (int)fibLc97.lcbPlcfandTxt);
                tableStream.Position = fibLc97.fcPlcfandRef;
                List<int> cpsat = BasicTypesReader.ReadCPs(tableStream, cpsat2.Count * 4);
                if (cpsat.Count == cpsat2.Count) 
                {
                    int pos0 = (int)((fib97.ccpText + fib97.ccpFtn + fib97.ccpHdd));
                    int len = (int)fib97.ccpAtn;
                    string ttt = totalText.Substring(pos0, len);
                    for (int i = 0; i < (cpsat.Count - 2); i++) 
                    {
                        int cp = cpsat[i];
                        if ((cp < 0) || cp >= pos0) 
                            continue;
                        Pullenti.Unitext.Internal.Uni.UnitextGen ge = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        int max = ((i + 1) < (cpsat.Count - 2) ? cpsat2[i + 1] : len);
                        this._createUni(ge, null, 0, pos0 + cpsat2[i], pos0 + max, totalText, 0);
                        Pullenti.Unitext.UnitextItem cc = ge.Finish(true, null);
                        if (cc == null) 
                            continue;
                        StringBuilder tmp = new StringBuilder();
                        cc.GetPlaintext(tmp, null);
                        if (tmp.Length < 1) 
                            continue;
                        Pullenti.Unitext.UnitextComment cmt = new Pullenti.Unitext.UnitextComment() { Text = tmp.ToString() };
                        m_Comments.Add(cp, cmt);
                    }
                }
                if (fib97.ccpTxbx > 0) 
                {
                    int pos0 = (((int)fib97.ccpText) + ((int)fib97.ccpFtn) + ((int)fib97.ccpHdd)) + ((int)fib97.ccpAtn) + ((int)fib97.ccpEdn);
                    int len = (int)fib97.ccpTxbx;
                    string ttt = totalText.Substring(pos0, len);
                    if (fibLc97.lcbPlcfTxbxBkd > 0 && fibLc97.lcbPlcftxbxTxt > 0) 
                    {
                        tableStream.Position = fibLc97.fcPlcftxbxTxt;
                        int k2 = (int)((((fibLc97.lcbPlcftxbxTxt - 4)) / 26));
                        List<int> cps2 = BasicTypesReader.ReadCPs(tableStream, ((k2 + 1)) * 4);
                        List<Pullenti.Unitext.UnitextContainer> texts2 = new List<Pullenti.Unitext.UnitextContainer>();
                        for (int ii = 0; ii < (cps2.Count - 2); ii++) 
                        {
                            Pullenti.Unitext.Internal.Uni.UnitextGen ge = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            int max = ((ii + 1) < (cps2.Count - 2) ? cps2[ii + 1] : len);
                            this._createUni(ge, null, 0, pos0 + cps2[ii], pos0 + max, totalText, 0);
                            Pullenti.Unitext.UnitextItem cc = ge.Finish(true, null);
                            Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer() { Typ = Pullenti.Unitext.UnitextContainerType.Shape };
                            if (cc != null) 
                                cnt.Children.Add(cc);
                            texts2.Add(cnt);
                        }
                        List<int> lids = new List<int>();
                        for (int ii = 0; ii < k2; ii++) 
                        {
                            tableStream.Position += 14;
                            lids.Add(ReadUtils.ReadInt(tableStream));
                            tableStream.Position += 4;
                        }
                        tableStream.Position = fibLc97.fcPlcSpaMom;
                        int kk = (int)((((fibLc97.lcbPlcSpaMom - 4)) / 30));
                        List<int> cps3 = BasicTypesReader.ReadCPs(tableStream, ((kk + 1)) * 4);
                        List<int> lids2 = new List<int>();
                        for (int ii = 0; ii < kk; ii++) 
                        {
                            lids2.Add(ReadUtils.ReadInt(tableStream));
                            tableStream.Position += 22;
                        }
                        for (int ii = lids2.Count - 1; ii >= 0; ii--) 
                        {
                            int j = lids.IndexOf(lids2[ii]);
                            if (j >= 0) 
                            {
                                m_Shapes.Add(cps3[ii], texts2[j]);
                                texts2.RemoveAt(j);
                                lids.RemoveAt(j);
                                lids2.RemoveAt(ii);
                                cps3.RemoveAt(ii);
                            }
                        }
                        for (int ii = lids2.Count - 1; ii >= 0; ii--) 
                        {
                            Pullenti.Unitext.UnitextContainer cnt = new Pullenti.Unitext.UnitextContainer();
                            for (int j = lids.Count - 1; j >= 0; j--) 
                            {
                                if (lids[j] > lids2[ii]) 
                                {
                                    cnt.Children.Insert(0, texts2[j]);
                                    lids.RemoveAt(j);
                                    texts2.RemoveAt(j);
                                }
                            }
                            if (cnt.Children.Count > 0) 
                                m_Shapes.Add(cps3[ii], cnt);
                        }
                    }
                }
                this._createUni(gen, null, 0, 0, (int)fib97.ccpText, totalText, 0);
                doc.Content = gen.Finish(true, null);
                if (fibLc97.lcbPlcfHdd != 0) 
                {
                    tableStream.Position = fibLc97.fcPlcfHdd;
                    List<int> strs = BasicTypesReader.ReadCPs(tableStream, (int)fibLc97.lcbPlcfHdd);
                    if (strs.Count > 7) 
                    {
                        int pos0 = ((int)fib97.ccpText) + ((int)fib97.ccpFtn);
                        int len = (int)fib97.ccpHdd;
                        string sub = totalText.Substring(pos0, len);
                        Pullenti.Unitext.UnitextPagesection sect = new Pullenti.Unitext.UnitextPagesection();
                        for (int k = 0; k < 2; k++) 
                        {
                            Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            for (int ii = 0; ii < 2; ii++) 
                            {
                                int jj = 7 + ii;
                                if (k > 0) 
                                    jj += 2;
                                if ((jj < strs.Count) && (strs[jj] < strs[jj + 1])) 
                                    this._createUni(gg, null, 0, pos0 + strs[jj], (pos0 + strs[jj + 1]) - 1, totalText, 0);
                            }
                            if (k == 0) 
                                sect.Header = gg.Finish(true, null);
                            else 
                                sect.Footer = gg.Finish(true, null);
                        }
                        if (sect.Header != null || sect.Footer != null) 
                            doc.Sections.Add(sect);
                    }
                }
                return;
            }
        }
        void _createUni(Pullenti.Unitext.Internal.Uni.UnitextGen gen, Pullenti.Unitext.UnitextItem blk, int i0, int p0, int p1, string txt, int level)
        {
            if (level > 30) 
                return;
            if (i0 == 32) 
            {
            }
            StringBuilder tmp = new StringBuilder();
            for (int i = i0; i < m_Paragraphs.Count; i++) 
            {
                Paragraph p = m_Paragraphs[i];
                if (p.Pos == 496) 
                {
                }
                if (p.Pos >= p1) 
                    break;
                if ((p.Pos < p0) || (p.Pos + p.Length) > p1) 
                    continue;
                if (gen != null) 
                {
                }
                byte[] data;
                if (p.IsInTable && gen != null && blk == null) 
                {
                    Pullenti.Unitext.Internal.Uni.UnitextGenTable tab = new Pullenti.Unitext.Internal.Uni.UnitextGenTable();
                    List<Pullenti.Unitext.Internal.Uni.UniTextGenCell> ro = new List<Pullenti.Unitext.Internal.Uni.UniTextGenCell>();
                    List<int> allids = new List<int>();
                    List<int> hasHugePapxRows = new List<int>();
                    Pullenti.Unitext.Internal.Uni.UnitextGen cgen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    for (; i < m_Paragraphs.Count; i++) 
                    {
                        Paragraph pp = m_Paragraphs[i];
                        if (!pp.IsInTable && pp.IsTableCellEnd && pp.Length <= 2) 
                        {
                            if (ro.Count > 0) 
                            {
                                data = pp.GetProperty(SinglePropertyModifiers.sprmPHugePapx);
                                if (data != null) 
                                    hasHugePapxRows.Add(tab.Cells.Count);
                                tab.Cells.Add(ro);
                            }
                            ro = new List<Pullenti.Unitext.Internal.Uni.UniTextGenCell>();
                            continue;
                        }
                        if (!pp.IsInTable) 
                        {
                                {
                                    i--;
                                    break;
                                }
                        }
                        if (pp.TableDepth < p.TableDepth) 
                        {
                                {
                                    i--;
                                    break;
                                }
                        }
                        if (pp.TableDepth > p.TableDepth) 
                        {
                            int i1;
                            for (i1 = i + 1; i1 < m_Paragraphs.Count; i1++) 
                            {
                                if (pp.TableDepth > m_Paragraphs[i1].TableDepth && m_Paragraphs[i1].TableDepth == p.TableDepth) 
                                    break;
                            }
                            Pullenti.Unitext.Internal.Uni.UnitextGen gt = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                            this._createUni(cgen, null, i, pp.Pos, m_Paragraphs[i1 - 1].Pos + m_Paragraphs[i1 - 1].Length, txt, level + 1);
                            i = i1;
                            if (i1 < m_Paragraphs.Count) 
                                pp = m_Paragraphs[i1];
                        }
                        if (pp.IsTableRowEnd) 
                        {
                            data = pp.GetProperty(SinglePropertyModifiers.sprmTDefTable);
                            if (data != null && data.Length > 0) 
                            {
                                int cou = (int)data[0];
                                if (cou == ro.Count) 
                                {
                                    for (int ii = 1, cc = 0; (ii < data.Length) && (cc < ro.Count); ii += 2,cc++) 
                                    {
                                        short iid = (short)data[ii + 1];
                                        iid <<= 8;
                                        iid |= ((short)data[ii]);
                                        ro[cc].Tag = iid;
                                        if (!allids.Contains(iid)) 
                                            allids.Add(iid);
                                    }
                                }
                            }
                            if (ro.Count > 0) 
                                tab.Cells.Add(ro);
                            ro = new List<Pullenti.Unitext.Internal.Uni.UniTextGenCell>();
                            continue;
                        }
                        this._createUni(cgen, new Pullenti.Unitext.UnitextTable(), i, pp.Pos, pp.Pos + pp.Length, txt, level + 1);
                        if (pp.IsTableCellEnd) 
                        {
                            Pullenti.Unitext.Internal.Uni.UniTextGenCell cel = new Pullenti.Unitext.Internal.Uni.UniTextGenCell();
                            ro.Add(cel);
                            cel.Content = cgen.Finish(true, null);
                            cgen = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                        }
                    }
                    if (ro.Count > 0) 
                        tab.Cells.Add(ro);
                    for (int ii = 0; ii < tab.Cells.Count; ii++) 
                    {
                        bool hasColSpan = false;
                        for (int jj = 0; jj < tab.Cells[ii].Count; jj++) 
                        {
                            Pullenti.Unitext.Internal.Uni.UniTextGenCell cel = tab.Cells[ii][jj];
                            int cid = allids.IndexOf(cel.Tag);
                            if (cid < 0) 
                                continue;
                            if ((jj + 1) == tab.Cells[ii].Count) 
                            {
                                if (((cid + 1) < allids.Count) && !hasHugePapxRows.Contains(cid)) 
                                {
                                    hasColSpan = true;
                                    cel.ColSpan = allids.Count - cid;
                                }
                            }
                            else 
                            {
                                Pullenti.Unitext.Internal.Uni.UniTextGenCell cel2 = tab.Cells[ii][jj + 1];
                                int cid2 = allids.IndexOf(cel2.Tag);
                                if (cid2 < 0) 
                                    continue;
                                if ((cid + 1) < cid2) 
                                {
                                    hasColSpan = true;
                                    cel.ColSpan = cid2 - cid;
                                }
                            }
                        }
                        if (hasColSpan || !hasHugePapxRows.Contains(ii) || (ii + 1) == tab.Cells.Count) 
                            continue;
                        List<Pullenti.Unitext.Internal.Uni.UniTextGenCell> row0 = tab.Cells[ii];
                        List<Pullenti.Unitext.Internal.Uni.UniTextGenCell> row1 = tab.Cells[ii + 1];
                        if (row0.Count < row1.Count) 
                        {
                            int emp = 0;
                            foreach (Pullenti.Unitext.Internal.Uni.UniTextGenCell vv in row1) 
                            {
                                if (vv.Content == null) 
                                    emp++;
                            }
                            if ((emp + 1) == row0.Count) 
                            {
                                int fne = 0;
                                for (; fne < row1.Count; fne++) 
                                {
                                    if (row1[fne].Content != null) 
                                        break;
                                }
                                row0[fne].ColSpan = row1.Count - emp;
                            }
                            else 
                                tab.MayHasError = true;
                        }
                        else if (row0.Count > row1.Count) 
                            tab.MayHasError = true;
                    }
                    if (gen != null) 
                    {
                        Pullenti.Unitext.UnitextTable utab = tab.Convert();
                        if (utab != null) 
                        {
                            gen.Append(utab, null, -1, false);
                            gen.AppendNewline(false);
                        }
                    }
                    continue;
                }
                if (p.IsList && gen != null && !(blk is Pullenti.Unitext.UnitextList)) 
                {
                    data = p.GetProperty(SinglePropertyModifiers.sprmPIlfo);
                    Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle num = null;
                    if (data != null && data.Length > 1) 
                    {
                        int ilfo = (int)data[1];
                        ilfo <<= 8;
                        ilfo |= data[0];
                        ilfo--;
                        if (ilfo >= 0 && (ilfo < lfos.Count)) 
                            m_NumStyles.TryGetValue(lfos[ilfo].lsid, out num);
                    }
                    if (m_Dels[p.Pos] != 0) 
                        continue;
                    Pullenti.Unitext.Internal.Uni.UnitextGen gg = new Pullenti.Unitext.Internal.Uni.UnitextGen();
                    this._createUni(gg, new Pullenti.Unitext.UnitextList(), i, p.Pos, p.Pos + p.Length, txt, level + 1);
                    Pullenti.Unitext.UnitextItem gg1 = gg.Finish(num != null, null);
                    gen.Append(gg1, num, p.ListLevel, false);
                    continue;
                }
                if (gen == null) 
                    continue;
                tmp.Length = 0;
                tmp.Append(txt.Substring(p.Pos, p.Length));
                int cp = p.Pos;
                for (int ii = 0; ii < tmp.Length; ii++,cp++) 
                {
                    char ch = tmp[ii];
                    if (m_Dels[cp] != 0) 
                    {
                        if (ii > 0) 
                            gen.AppendText(tmp.ToString().Substring(0, ii), false);
                        for (; ii < tmp.Length; ii++,cp++) 
                        {
                            if (m_Dels[cp] == 0) 
                                break;
                        }
                        if (ii < tmp.Length) 
                            tmp.Remove(0, ii);
                        else 
                            tmp.Length = 0;
                        ii = -1;
                        cp--;
                        continue;
                    }
                    string sup = null;
                    bool isSub = false;
                    if (m_SuperTexts.TryGetValue(cp, out sup)) 
                    {
                    }
                    else if (m_SubTexts.TryGetValue(cp, out sup)) 
                        isSub = true;
                    if (sup != null) 
                    {
                        if (cp == 84223) 
                        {
                        }
                        if (ii > 0) 
                            gen.AppendText(tmp.ToString().Substring(0, ii), false);
                        string stxt = sup.Trim();
                        if (!string.IsNullOrEmpty(stxt)) 
                            gen.Append(new Pullenti.Unitext.UnitextPlaintext() { Text = stxt, Typ = (isSub ? Pullenti.Unitext.UnitextPlaintextType.Sub : Pullenti.Unitext.UnitextPlaintextType.Sup) }, null, -1, false);
                        else 
                        {
                        }
                        ii += sup.Length;
                        if (sup.Length > 0) 
                            cp += (sup.Length - 1);
                        if (ii < tmp.Length) 
                            tmp.Remove(0, ii);
                        else 
                            tmp.Length = 0;
                        ii = -1;
                        continue;
                    }
                    if (ch < 0x15) 
                    {
                    }
                    if (ch == 0x13) 
                    {
                        StringBuilder tmp2 = new StringBuilder();
                        StringBuilder tmp3 = null;
                        Pullenti.Unitext.UnitextContainer shape = null;
                        int lev = 1;
                        int ii0 = ii;
                        cp++;
                        ii++;
                        for (; ii < tmp.Length; ii++,cp++) 
                        {
                            ch = tmp[ii];
                            if (ch == 0x13) 
                                lev++;
                            else if (ch == 0x15) 
                            {
                                lev--;
                                if (lev <= 0) 
                                    break;
                            }
                            else if (ch == 0x14 && lev == 1) 
                            {
                                if (tmp3 == null) 
                                    tmp3 = new StringBuilder();
                            }
                                {
                                    tmp2.Append(ch);
                                    if (tmp3 != null) 
                                        tmp3.Append(ch);
                                }
                            if (m_Shapes.ContainsKey(cp)) 
                                shape = m_Shapes[cp];
                        }
                        string str0 = tmp2.ToString().TrimStart();
                        if (lev == 0 || ((lev == 1 && str0.StartsWith("TOC")))) 
                        {
                            string str = MSOfficeHelper.extractSpecText(str0);
                            if (ii0 > 0) 
                                gen.AppendText(tmp.ToString().Substring(0, ii0), false);
                            if (str != null) 
                            {
                                string hyperlink = null;
                                if (str0.StartsWith("HYPERLINK") && (str0.IndexOf("PAGEREF") < 0)) 
                                {
                                    int jj0 = str0.IndexOf('"');
                                    int jj1 = str0.IndexOf('"', jj0 + 1);
                                    if (jj0 > 0 && jj1 > (jj0 + 1)) 
                                        hyperlink = str0.Substring(jj0 + 1, jj1 - jj0 - 1);
                                }
                                if (hyperlink != null) 
                                {
                                    Pullenti.Unitext.UnitextHyperlink hr = new Pullenti.Unitext.UnitextHyperlink() { Href = hyperlink };
                                    hr.Content = new Pullenti.Unitext.UnitextPlaintext() { Text = str };
                                    gen.Append(hr, null, -1, false);
                                }
                                else 
                                    gen.AppendText(str, false);
                            }
                            else if (tmp3 != null) 
                            {
                                string tt = tmp3.ToString();
                                if (!gen.LastText.EndsWith(tt)) 
                                    gen.AppendText(tt, false);
                            }
                            if (shape != null) 
                                gen.Append(shape, null, -1, false);
                            if ((ii + 1) >= tmp.Length) 
                                tmp.Length = 0;
                            else 
                                tmp.Remove(0, ii + 1);
                            ii = -1;
                            continue;
                        }
                    }
                    if (m_Footnotes != null) 
                    {
                        if (m_Footnotes.ContainsKey(cp)) 
                        {
                            if (ch != 0x2) 
                                m_Footnotes[cp].CustomMark = string.Format("{0}", ch);
                            if (ii > 0) 
                                gen.AppendText(tmp.ToString().Substring(0, ii), false);
                            gen.Append(m_Footnotes[cp], null, -1, false);
                            tmp.Remove(0, ii + 1);
                            ii = -1;
                            continue;
                        }
                    }
                    if (ch == 0xB) 
                    {
                        if (ii > 0) 
                            gen.AppendText(tmp.ToString().Substring(0, ii), false);
                        gen.AppendNewline(false);
                        tmp.Remove(0, ii + 1);
                        ii = -1;
                        continue;
                    }
                    if (ch == 0xC) 
                    {
                        if (ii > 0) 
                            gen.AppendText(tmp.ToString().Substring(0, ii), false);
                        gen.AppendPagebreak();
                        tmp.Remove(0, ii + 1);
                        ii = -1;
                        continue;
                    }
                    if (ch == 0xD || ch == 0xA) 
                    {
                    }
                    if (ch == 5 && m_Comments != null && m_Comments.ContainsKey(cp)) 
                    {
                        if (ii > 0) 
                            gen.AppendText(tmp.ToString().Substring(0, ii), false);
                        gen.Append(m_Comments[cp], null, -1, false);
                        tmp.Remove(0, ii + 1);
                        ii = -1;
                        continue;
                    }
                    if (((ch == 7 || ch == 2 || ch == 5) || ch == 3 || ch == 4) || ch == 0x1F) 
                    {
                        tmp.Remove(ii, 1);
                        ii--;
                        continue;
                    }
                    if (ch == 1 || ch == 8) 
                    {
                        if (ii > 0) 
                            gen.AppendText(tmp.ToString().Substring(0, ii), false);
                        if (m_Shapes.ContainsKey(cp)) 
                            gen.Append(m_Shapes[cp], null, -1, false);
                        else 
                            gen.Append(new Pullenti.Unitext.UnitextImage(), null, -1, false);
                        tmp.Remove(0, ii + 1);
                        ii = -1;
                        continue;
                    }
                    if (ch != 6 && ch != 0xD && (((int)ch) < 0x20)) 
                    {
                        if (ii >= 0 && (ii < tmp.Length)) 
                            tmp[ii] = ' ';
                    }
                }
                if (tmp.Length > 0) 
                    gen.AppendText(tmp.ToString(), false);
            }
        }
        private void LoadStsh(Stream wordDocumentStream, STSH stsh)
        {
            m_StyleDefinitionsMap = new Dictionary<ushort, StyleDefinition>();
            for (int istd = 0; istd < stsh.rglpstd.Length; istd++) 
            {
                STD std = stsh.rglpstd[istd];
                if (std == null) 
                    continue;
                StyleDefinition def = new StyleDefinition(this, std);
                m_StyleDefinitionsMap.Add((ushort)istd, def);
            }
            List<Prl> defaultPrls = new List<Prl>();
            short ftcBi = stsh.stshi.ftcBi;
            defaultPrls.Add(new Prl(new Sprm(SinglePropertyModifiers.sprmCFtcBi), BitConverter.GetBytes(ftcBi)));
            short ftcAsci = stsh.stshi.stshif.ftcAsci;
            defaultPrls.Add(new Prl(new Sprm(SinglePropertyModifiers.sprmCRgFtc0), BitConverter.GetBytes(ftcAsci)));
            short ftcFE = stsh.stshi.stshif.ftcFE;
            defaultPrls.Add(new Prl(new Sprm(SinglePropertyModifiers.sprmCRgFtc1), BitConverter.GetBytes(ftcFE)));
            short ftcOther = stsh.stshi.stshif.ftcOther;
            defaultPrls.Add(new Prl(new Sprm(SinglePropertyModifiers.sprmCRgFtc2), BitConverter.GetBytes(ftcOther)));
            this.defaultPrls = defaultPrls.ToArray();
        }
        private void LoadCharacterFormatting(Stream wordDocumentStream, PlcBteChpx plcfBteChpx)
        {
            List<CharacterFormatting> formattings = new List<CharacterFormatting>();
            for (int i = 0; i < plcfBteChpx.aPnBteChpx.Length; i++) 
            {
                wordDocumentStream.Position = plcfBteChpx.aPnBteChpx[i] * 512;
                try 
                {
                    ChpxFkp papxFkp = BasicTypesReader.ReadChpxFkp(wordDocumentStream);
                    for (int j = 0; j < papxFkp.rgb.Length; j++) 
                    {
                        int startFc = (int)papxFkp.rgfc[j];
                        int endFc = (int)papxFkp.rgfc[j + 1];
                        FileCharacterPosition fc = _findPos123(fileCharacterPositions, startFc, endFc);
                        if (fc == null) 
                            continue;
                        if (startFc < fc.Offset) 
                            startFc = fc.Offset;
                        if (endFc > (fc.Offset + (fc.BytesPerCharacter * fc.Length))) 
                            endFc = fc.Offset + (fc.BytesPerCharacter * fc.Length);
                        CharacterFormatting para = new CharacterFormatting(this, ((startFc - fc.Offset)) / fc.BytesPerCharacter, ((endFc - startFc)) / fc.BytesPerCharacter, fc, papxFkp.rgb[j].Value);
                        formattings.Add(para);
                    }
                }
                catch(Exception ex306) 
                {
                }
            }
            for (int ii = 0; ii < formattings.Count; ii++) 
            {
                bool ch = false;
                for (int jj = 0; jj < (formattings.Count - 1); jj++) 
                {
                    if (formattings[jj].Offset > formattings[jj + 1].Offset) 
                    {
                        CharacterFormatting f = formattings[jj];
                        formattings[jj] = formattings[jj + 1];
                        formattings[jj + 1] = f;
                        ch = true;
                    }
                }
                if (!ch) 
                    break;
            }
            this.m_Formattings = formattings.ToArray();
        }
        static FileCharacterPosition _findPos123(FileCharacterPosition[] arr, int startFc, int endFc)
        {
            foreach (FileCharacterPosition f in arr) 
            {
                if (!((endFc <= f.Offset || ((f.Offset + (f.BytesPerCharacter * f.Length)) < startFc)))) 
                    return f;
            }
            return null;
        }
        private void LoadParagraphs(Stream wordDocumentStream, Clx clx, PlcBtePapx plcfBtePapx)
        {
            m_Paragraphs = new List<Paragraph>();
            for (int i = 0; i < plcfBtePapx.aPnBtePapx.Length; i++) 
            {
                wordDocumentStream.Position = plcfBtePapx.aPnBtePapx[i] * 512;
                bool er;
                PapxFkp papxFkp = BasicTypesReader.ReadPapxFkp(wordDocumentStream, out er);
                if (er) 
                    m_Error = true;
                for (int j = 0; j < papxFkp.rgbx.Length; j++) 
                {
                    int startFc = (int)papxFkp.rgfc[j];
                    int endFc = (int)papxFkp.rgfc[j + 1];
                    FileCharacterPosition fc = _findPos123(fileCharacterPositions, startFc, endFc);
                    if (fc == null) 
                        continue;
                    if (startFc < fc.Offset) 
                        startFc = fc.Offset;
                    if (endFc > (fc.Offset + (fc.BytesPerCharacter * fc.Length))) 
                        endFc = fc.Offset + (fc.BytesPerCharacter * fc.Length);
                    Paragraph para = new Paragraph(this, ((startFc - fc.Offset)) / fc.BytesPerCharacter, ((endFc - startFc)) / fc.BytesPerCharacter, fc, papxFkp.rgbx[j].Value);
                    m_Paragraphs.Add(para);
                }
            }
            for (int ii = 0; ii < m_Paragraphs.Count; ii++) 
            {
                bool ch = false;
                for (int jj = 0; jj < (m_Paragraphs.Count - 1); jj++) 
                {
                    if (m_Paragraphs[jj].CompareTo(m_Paragraphs[jj + 1]) > 0) 
                    {
                        Paragraph p = m_Paragraphs[jj];
                        m_Paragraphs[jj] = m_Paragraphs[jj + 1];
                        m_Paragraphs[jj + 1] = p;
                        ch = true;
                    }
                }
                if (!ch) 
                    break;
            }
            for (int i = 0; i < (m_Paragraphs.Count - 1); i++) 
            {
                if ((m_Paragraphs[i].Pos + m_Paragraphs[i].Length) < m_Paragraphs[i + 1].Pos) 
                    m_Paragraphs[i].Length = (int)((m_Paragraphs[i + 1].Pos - m_Paragraphs[i].Pos));
            }
        }
        private void LoadCharacters(Stream wordDocumentStream, Clx clx)
        {
            if (clx == null) 
                return;
            PlcPcd plcPcd = clx.Pcdt.PlcPcd;
            FileCharacterPosition[] fileCharacters = new FileCharacterPosition[(int)plcPcd.Pcds.Length];
            int position = 0;
            for (int i = 0; i < plcPcd.Pcds.Length; i++) 
            {
                int length = (int)((plcPcd.CPs[i + 1] - plcPcd.CPs[i]));
                int offset = plcPcd.Pcds[i].fc.fc;
                bool compressed = plcPcd.Pcds[i].fc.fCompressed;
                if (compressed) 
                {
                    char[] txt = new char[(int)length];
                    wordDocumentStream.Position = offset / 2;
                    byte[] data = ReadUtils.ReadExact(wordDocumentStream, length);
                    FcCompressedMapping.GetChars(data, 0, length, txt, 0);
                    this.Text.Append(txt);
                }
                else 
                {
                    wordDocumentStream.Position = offset;
                    byte[] data = ReadUtils.ReadExact(wordDocumentStream, length * 2);
                    string txt = Pullenti.Util.MiscHelper.DecodeStringUnicode(data, 0, -1);
                    this.Text.Append(txt);
                }
                FileCharacterPosition fc = new FileCharacterPosition();
                fc.Offset = (compressed ? offset / 2 : offset);
                fc.BytesPerCharacter = (compressed ? 1 : 2);
                fc.Length = length;
                fc.CharacterIndex = position;
                fc.Prls = this.ExpandPrm(plcPcd.Pcds[i].prm, clx);
                fileCharacters[i] = fc;
                position += length;
            }
            List<FileCharacterPosition> li = new List<FileCharacterPosition>(fileCharacters);
            for (int ii = 0; ii < li.Count; ii++) 
            {
                bool ch = false;
                for (int jj = 0; jj < (li.Count - 1); jj++) 
                {
                    if (li[jj].Offset > li[jj + 1].Offset) 
                    {
                        FileCharacterPosition f = li[jj];
                        li[jj] = li[jj + 1];
                        li[jj + 1] = f;
                        ch = true;
                    }
                }
                if (!ch) 
                    break;
            }
            fileCharacters = li.ToArray();
            this.fileCharacterPositions = fileCharacters;
        }
        private Prl[] ExpandPrm(Prm prm, Clx clx)
        {
            if (prm.fComplex) 
            {
                int index = (int)prm.igrpprl;
                return clx.RgPrc[index].GrpPrl;
            }
            else if (prm.prm != 0x0000) 
            {
                ushort sprm;
                if (!SinglePropertyModifiers.prm0Map.TryGetValue(prm.isprm, out sprm)) 
                    throw new Exception("Invalid Prm: isprm");
                byte value = prm.val;
                Prl prl = new Prl(new Sprm(sprm), new byte[] {value});
                return new Prl[] {prl};
            }
            else 
                return new Prl[(int)0];
        }
        public StyleCollection GetDefaults()
        {
            List<Prl[]> li = new List<Prl[]>();
            li.Add(defaultPrls);
            return new StyleCollection(li);
        }
        public StyleCollection GetStyle(int characterPosition, FormattingLevel level)
        {
            List<Prl[]> prls = new List<Prl[]>();
            if (level >= FormattingLevel.Character) 
            {
                foreach (CharacterFormatting formatting in m_Formattings) 
                {
                    if (formatting.FileCharacterPosition.Contains(characterPosition)) 
                    {
                        if (level >= FormattingLevel.CharacterStyle) 
                        {
                            StyleDefinition definition = formatting.Style;
                            if (definition != null) 
                                definition.ExpandStyles(prls);
                        }
                    }
                }
            }
            if (level >= FormattingLevel.Paragraph) 
            {
                foreach (Paragraph paragraph in m_Paragraphs) 
                {
                    if (paragraph.FileCharacterPosition.Contains(characterPosition)) 
                    {
                        if (level >= FormattingLevel.ParagraphStyle) 
                        {
                            StyleDefinition definition = paragraph.Style;
                            definition.ExpandStyles(prls);
                        }
                    }
                }
            }
            if (level >= FormattingLevel.Part) 
            {
                foreach (FileCharacterPosition fcp in fileCharacterPositions) 
                {
                    if (fcp.Contains(characterPosition)) 
                        prls.Add(fcp.Prls);
                }
            }
            if (level >= FormattingLevel.Global) 
                prls.Add(defaultPrls);
            return new StyleCollection(prls);
        }
        internal class FileCharacterPosition
        {
            internal int Offset;
            internal int Length;
            internal int CharacterIndex;
            internal int BytesPerCharacter;
            internal Pullenti.Unitext.Internal.Word.Prl[] Prls;
            public override string ToString()
            {
                StringBuilder res = new StringBuilder();
                res.AppendFormat("CharIndex:{0} Offset:{1} Len:{2}", CharacterIndex, Offset, Length);
                if (Prls != null) 
                {
                    foreach (Pullenti.Unitext.Internal.Word.Prl prl in Prls) 
                    {
                        res.AppendFormat(" {0}", Pullenti.Unitext.Internal.Word.SinglePropertyModifiers.GetSprmName(prl.sprm.sprm) ?? "?");
                    }
                }
                return res.ToString();
            }
            internal bool Contains(int position)
            {
                return CharacterIndex <= position && (position < (CharacterIndex + Length));
            }
        }

        class LFO
        {
            public int lsid;
            public int DatCount;
            public byte ibstFltAutoNum;
            List<Pullenti.Unitext.Internal.Word.WordDocument.LFOLVL> Lvls = new List<Pullenti.Unitext.Internal.Word.WordDocument.LFOLVL>();
        }

        class LFOData
        {
        }

        class LFOLVL
        {
            public int iStartAt;
        }

        List<LFO> lfos = new List<LFO>();
        void LoadLFOS(Stream wordDocumentStream, int length)
        {
            if (length < 10) 
                return;
            int cou = ReadUtils.ReadInt(wordDocumentStream);
            length -= 4;
            for (int i = 0; i < cou; i++) 
            {
                LFO lfo = new LFO();
                lfos.Add(lfo);
                lfo.lsid = ReadUtils.ReadInt(wordDocumentStream);
                length -= 4;
                wordDocumentStream.Position += 8;
                length -= 8;
                lfo.DatCount = (int)wordDocumentStream.ReadByte();
                length--;
                lfo.ibstFltAutoNum = (byte)wordDocumentStream.ReadByte();
                length--;
                wordDocumentStream.Position += 2;
                length -= 2;
            }
            for (int i = 0; i < cou; i++) 
            {
                if (length < 4) 
                    break;
                int cp = ReadUtils.ReadInt(wordDocumentStream);
                length -= 4;
                for (int j = 0; j < lfos[i].DatCount; j++) 
                {
                    if (length < 10) 
                        break;
                }
            }
        }
        class LSTF
        {
            public int lsid;
            public byte[] istds = new byte[(int)18];
            public bool fSimpleList;
            public bool fAutoNum;
            public List<Pullenti.Unitext.Internal.Word.WordDocument.LVL> lvls = new List<Pullenti.Unitext.Internal.Word.WordDocument.LVL>();
        }

        class LVL
        {
            public int iStartAt;
            public byte nfc;
            public byte[] rgbxchNums = new byte[(int)9];
            public byte ixchFollow;
            public int dxaIndentSav;
            public byte cbGrpprlChpx;
            public byte cbGrpprlPapx;
            public byte ilvlRestartLim;
            public string xstname;
        }

        List<LSTF> lstfs = new List<LSTF>();
        void LoadLSTFs(Stream wordDocumentStream, int length)
        {
            int cou = (int)ReadUtils.ReadShort(wordDocumentStream);
            length -= 2;
            for (int i = 0; i < cou; i++) 
            {
                LSTF lstf = new LSTF();
                lstfs.Add(lstf);
                lstf.lsid = ReadUtils.ReadInt(wordDocumentStream);
                length -= 4;
                Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle num = new Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle();
                num.Id = lstf.lsid.ToString();
                if (!m_NumStyles.ContainsKey(lstf.lsid)) 
                    m_NumStyles.Add(lstf.lsid, num);
                ReadUtils.ReadInt(wordDocumentStream);
                length -= 4;
                wordDocumentStream.Read(lstf.istds, 0, 18);
                length -= 18;
                byte b = (byte)wordDocumentStream.ReadByte();
                wordDocumentStream.ReadByte();
                length -= 2;
                if (((b & 1)) != 0) 
                    lstf.fSimpleList = true;
                if (((b & 4)) != 0) 
                    lstf.fAutoNum = true;
            }
            for (int i = 0; i < cou; i++) 
            {
                LSTF lstf = lstfs[i];
                Pullenti.Unitext.Internal.Uni.UnitextGenNumStyle num = m_NumStyles[lstf.lsid];
                for (int j = 0; j < ((lstf.fSimpleList ? 1 : 9)); j++) 
                {
                    LVL lvl = new LVL();
                    lstf.lvls.Add(lvl);
                    lvl.iStartAt = ReadUtils.ReadInt(wordDocumentStream);
                    length -= 4;
                    lvl.nfc = (byte)wordDocumentStream.ReadByte();
                    length--;
                    byte bb = (byte)wordDocumentStream.ReadByte();
                    length--;
                    wordDocumentStream.Read(lvl.rgbxchNums, 0, 9);
                    length -= 9;
                    lvl.ixchFollow = (byte)wordDocumentStream.ReadByte();
                    length--;
                    lvl.dxaIndentSav = ReadUtils.ReadInt(wordDocumentStream);
                    length -= 4;
                    ReadUtils.ReadInt(wordDocumentStream);
                    length -= 4;
                    lvl.cbGrpprlChpx = (byte)wordDocumentStream.ReadByte();
                    length--;
                    lvl.cbGrpprlPapx = (byte)wordDocumentStream.ReadByte();
                    length--;
                    lvl.ilvlRestartLim = (byte)wordDocumentStream.ReadByte();
                    length--;
                    wordDocumentStream.ReadByte();
                    length--;
                    wordDocumentStream.Position += (((int)lvl.cbGrpprlChpx) + ((int)lvl.cbGrpprlPapx));
                    length -= (((int)lvl.cbGrpprlChpx) + ((int)lvl.cbGrpprlPapx));
                    StringBuilder tmp = new StringBuilder();
                    int slen = (int)ReadUtils.ReadShort(wordDocumentStream);
                    length -= 2;
                    for (int k = 0; k < slen; k++) 
                    {
                        ushort cod = (ushort)ReadUtils.ReadShort(wordDocumentStream);
                        char ch = (char)cod;
                        if (cod >= 0x20) 
                        {
                            if (cod >= 0xF000) 
                            {
                                char chh = Pullenti.Unitext.Internal.Misc.WingdingsHelper.GetUnicode(cod - 0xF000);
                                if (chh != 0) 
                                    ch = chh;
                            }
                            tmp.Append(ch);
                        }
                        else 
                            tmp.AppendFormat("%{0}", (int)((ch + 1)));
                        length -= 2;
                    }
                    lvl.xstname = tmp.ToString();
                    Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel nl = new Pullenti.Unitext.Internal.Uni.UniTextGenNumLevel();
                    num.Levels.Add(nl);
                    nl.Format = lvl.xstname;
                    nl.Start = lvl.iStartAt;
                    string mnem;
                    if (m_NumTypesMap.TryGetValue(lvl.nfc, out mnem)) 
                        nl.Type = DocNumStyles.GetNumTyp(mnem);
                    else 
                        nl.Type = Pullenti.Unitext.Internal.Uni.UniTextGenNumType.Decimal;
                }
            }
        }
        static WordDocument()
        {
            m_NumTypesMap.Add(0x00, "decimal msonfcUCRoman");
            m_NumTypesMap.Add(0x01, "upperRoman");
            m_NumTypesMap.Add(0x02, "lowerRoman");
            m_NumTypesMap.Add(0x03, "upperLetter");
            m_NumTypesMap.Add(0x04, "lowerLetter");
            m_NumTypesMap.Add(0x05, "ordinal");
            m_NumTypesMap.Add(0x06, "cardinalText");
            m_NumTypesMap.Add(0x07, "ordinalText");
            m_NumTypesMap.Add(0x08, "hex");
            m_NumTypesMap.Add(0x09, "chicago");
            m_NumTypesMap.Add(0x0A, "ideographDigital");
            m_NumTypesMap.Add(0x0B, "japaneseCounting");
            m_NumTypesMap.Add(0x0C, "Aiueo");
            m_NumTypesMap.Add(0x0D, "Iroha");
            m_NumTypesMap.Add(0x0E, "decimalFullWidth");
            m_NumTypesMap.Add(0x0F, "decimalHalfWidth");
            m_NumTypesMap.Add(0x10, "japaneseLegal");
            m_NumTypesMap.Add(0x11, "japaneseDigitalTenThousand");
            m_NumTypesMap.Add(0x12, "decimalEnclosedCircle");
            m_NumTypesMap.Add(0x13, "decimalFullWidth2");
            m_NumTypesMap.Add(0x14, "aiueoFullWidth");
            m_NumTypesMap.Add(0x15, "irohaFullWidth");
            m_NumTypesMap.Add(0x16, "decimalZero");
            m_NumTypesMap.Add(0x17, "bullet");
            m_NumTypesMap.Add(0x18, "ganada");
            m_NumTypesMap.Add(0x19, "chosung");
            m_NumTypesMap.Add(0x1A, "decimalEnclosedFullstop");
            m_NumTypesMap.Add(0x1D, "ideographEnclosedCircle");
            m_NumTypesMap.Add(0x1E, "ideographTraditional");
            m_NumTypesMap.Add(0x1F, "ideographZodiac");
            m_NumTypesMap.Add(0x20, "ideographZodiacTraditional");
            m_NumTypesMap.Add(0x21, "taiwaneseCounting");
            m_NumTypesMap.Add(0x22, "ideographLegalTraditional");
            m_NumTypesMap.Add(0x23, "taiwaneseCountingThousand");
            m_NumTypesMap.Add(0x24, "taiwaneseDigital");
            m_NumTypesMap.Add(0x25, "chineseCounting");
            m_NumTypesMap.Add(0x26, "chineseLegalSimplified");
            m_NumTypesMap.Add(0x27, "chineseCountingThousand");
            m_NumTypesMap.Add(0x28, "decimal");
            m_NumTypesMap.Add(0x29, "koreanDigital");
            m_NumTypesMap.Add(0x2A, "koreanCounting");
            m_NumTypesMap.Add(0x2B, "koreanLegal");
            m_NumTypesMap.Add(0x2C, "koreanDigital2");
            m_NumTypesMap.Add(0x2D, "hebrew1");
            m_NumTypesMap.Add(0x2E, "arabicAlpha");
            m_NumTypesMap.Add(0x2F, "hebrew2");
            m_NumTypesMap.Add(0x30, "arabicAbjad");
            m_NumTypesMap.Add(0x31, "hindiVowels");
            m_NumTypesMap.Add(0x32, "hindiConsonants");
            m_NumTypesMap.Add(0x33, "hindiNumbers");
            m_NumTypesMap.Add(0x34, "hindiCounting");
            m_NumTypesMap.Add(0x35, "thaiLetters");
            m_NumTypesMap.Add(0x36, "thaiNumbers");
            m_NumTypesMap.Add(0x37, "thaiCounting");
            m_NumTypesMap.Add(0x38, "vietnameseCounting");
            m_NumTypesMap.Add(0x39, "numberInDash");
            m_NumTypesMap.Add(0x3A, "russianLower");
            m_NumTypesMap.Add(0x3B, "russianUpper");
        }
        public static Dictionary<byte, string> m_NumTypesMap = new Dictionary<byte, string>();
    }
}