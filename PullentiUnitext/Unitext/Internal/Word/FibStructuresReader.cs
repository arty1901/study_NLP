/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.IO;

namespace Pullenti.Unitext.Internal.Word
{
    static class FibStructuresReader
    {
        internal static Fib ReadFib(Stream s)
        {
            Fib fib = new Fib();
            fib.@base = ReadFibBase(s);
            if (fib.@base == null) 
                return null;
            fib.csw = BitConverter.ToUInt16(ReadUtils.ReadExact(s, ReadUtils.WordSize), 0);
            fib.fibRgW = ReadFibRgW97(s, fib.csw);
            fib.cslw = BitConverter.ToUInt16(ReadUtils.ReadExact(s, ReadUtils.WordSize), 0);
            fib.fibRgLw = ReadFibRgLw97(s, fib.cslw);
            fib.cbRgFcLcb = BitConverter.ToUInt16(ReadUtils.ReadExact(s, ReadUtils.WordSize), 0);
            fib.fibRgFcLcbBlob = ReadFibRgFcLcbBlob(s, fib.@base.nFib, fib.cbRgFcLcb);
            fib.cswNew = BitConverter.ToUInt16(ReadUtils.ReadExact(s, ReadUtils.WordSize), 0);
            fib.fibRgCswNew = ReadFibRgCswNew(s, fib.@base.nFib, fib.cswNew);
            return fib;
        }
        static FibRgCswNew ReadFibRgCswNew(Stream s, ushort nFib, ushort size)
        {
            FibRgCswNew2000 fibRgCswNew2000 = null;
            FibRgCswNew2007 fibRgCswNew2007 = null;
            switch (size) { 
            case 0:
                return new FibRgCswNew();
            case 2:
                fibRgCswNew2000 = new FibRgCswNew2000();
                break;
            case 5:
                fibRgCswNew2000 = (fibRgCswNew2007 = new FibRgCswNew2007());
                break;
            default: 
                fibRgCswNew2000 = (fibRgCswNew2007 = new FibRgCswNew2007());
                break;
            }
            byte[] data = ReadUtils.ReadExact(s, size * 2);
            fibRgCswNew2000.cQuickSavesNew = BitConverter.ToUInt16(data, 0);
            return fibRgCswNew2000;
        }
        static FibRgFcLcb ReadFibRgFcLcbBlob(Stream s, ushort nFib, ushort size)
        {
            FibRbFcLcb97 fibRbFcLcb97 = null;
            FibRbFcLcb2000 fibRbFcLcb2000 = null;
            FibRbFcLcb2002 fibRbFcLcb2002 = null;
            FibRbFcLcb2003 fibRbFcLcb2003 = null;
            FibRbFcLcb2007 fibRbFcLcb2007 = null;
            switch (size) { 
            case 0x005D:
                fibRbFcLcb97 = new FibRbFcLcb97();
                break;
            case 0x006C:
                fibRbFcLcb97 = (fibRbFcLcb2000 = new FibRbFcLcb2000());
                break;
            case 0x0088:
                fibRbFcLcb97 = (fibRbFcLcb2000 = (fibRbFcLcb2002 = new FibRbFcLcb2002()));
                break;
            case 0x00A4:
                fibRbFcLcb97 = (fibRbFcLcb2000 = (fibRbFcLcb2002 = (fibRbFcLcb2003 = new FibRbFcLcb2003())));
                break;
            case 0x00B7:
                fibRbFcLcb97 = (fibRbFcLcb2000 = (fibRbFcLcb2002 = (fibRbFcLcb2003 = (fibRbFcLcb2007 = new FibRbFcLcb2007()))));
                break;
            default: 
                if (size >= 0x00B7) 
                    fibRbFcLcb97 = (fibRbFcLcb2000 = (fibRbFcLcb2002 = (fibRbFcLcb2003 = (fibRbFcLcb2007 = new FibRbFcLcb2007()))));
                else 
                    fibRbFcLcb97 = new FibRbFcLcb97();
                break;
            }
            byte[] data = ReadUtils.ReadExact(s, size * 8);
            fibRbFcLcb97.fcStshfOrig = BitConverter.ToUInt32(data, 0);
            fibRbFcLcb97.lcbStshfOrig = BitConverter.ToUInt32(data, 4);
            fibRbFcLcb97.fcStshf = BitConverter.ToUInt32(data, 8);
            fibRbFcLcb97.lcbStshf = BitConverter.ToUInt32(data, 12);
            fibRbFcLcb97.fcPlcffndRef = BitConverter.ToUInt32(data, 16);
            fibRbFcLcb97.lcbPlcffndRef = BitConverter.ToUInt32(data, 20);
            fibRbFcLcb97.fcPlcffndTxt = BitConverter.ToUInt32(data, 24);
            fibRbFcLcb97.lcbPlcffndTxt = BitConverter.ToUInt32(data, 28);
            fibRbFcLcb97.fcPlcfandRef = BitConverter.ToUInt32(data, 32);
            fibRbFcLcb97.lcbPlcfandRef = BitConverter.ToUInt32(data, 36);
            fibRbFcLcb97.fcPlcfandTxt = BitConverter.ToUInt32(data, 40);
            fibRbFcLcb97.lcbPlcfandTxt = BitConverter.ToUInt32(data, 44);
            fibRbFcLcb97.fcPlcfSed = BitConverter.ToUInt32(data, 48);
            fibRbFcLcb97.lcbPlcfSed = BitConverter.ToUInt32(data, 52);
            fibRbFcLcb97.fcPlcPad = BitConverter.ToUInt32(data, 56);
            fibRbFcLcb97.lcbPlcPad = BitConverter.ToUInt32(data, 60);
            fibRbFcLcb97.fcPlcfPhe = BitConverter.ToUInt32(data, 64);
            fibRbFcLcb97.lcbPlcfPhe = BitConverter.ToUInt32(data, 68);
            fibRbFcLcb97.fcSttbfGlsy = BitConverter.ToUInt32(data, 72);
            fibRbFcLcb97.lcbSttbfGlsy = BitConverter.ToUInt32(data, 76);
            fibRbFcLcb97.fcPlcfGlsy = BitConverter.ToUInt32(data, 80);
            fibRbFcLcb97.lcbPlcfGlsy = BitConverter.ToUInt32(data, 84);
            fibRbFcLcb97.fcPlcfHdd = BitConverter.ToUInt32(data, 88);
            fibRbFcLcb97.lcbPlcfHdd = BitConverter.ToUInt32(data, 92);
            fibRbFcLcb97.fcPlcfBteChpx = BitConverter.ToUInt32(data, 96);
            fibRbFcLcb97.lcbPlcfBteChpx = BitConverter.ToUInt32(data, 100);
            fibRbFcLcb97.fcPlcfBtePapx = BitConverter.ToUInt32(data, 104);
            fibRbFcLcb97.lcbPlcfBtePapx = BitConverter.ToUInt32(data, 108);
            fibRbFcLcb97.fcPlcfSea = BitConverter.ToUInt32(data, 112);
            fibRbFcLcb97.lcbPlcfSea = BitConverter.ToUInt32(data, 116);
            fibRbFcLcb97.fcSttbfFfn = BitConverter.ToUInt32(data, 120);
            fibRbFcLcb97.lcbSttbfFfn = BitConverter.ToUInt32(data, 124);
            fibRbFcLcb97.fcPlcfFldMom = BitConverter.ToUInt32(data, 128);
            fibRbFcLcb97.lcbPlcfFldMom = BitConverter.ToUInt32(data, 132);
            fibRbFcLcb97.fcPlcfFldHdr = BitConverter.ToUInt32(data, 136);
            fibRbFcLcb97.lcbPlcfFldHdr = BitConverter.ToUInt32(data, 140);
            fibRbFcLcb97.fcPlcfFldFtn = BitConverter.ToUInt32(data, 144);
            fibRbFcLcb97.lcbPlcfFldFtn = BitConverter.ToUInt32(data, 148);
            fibRbFcLcb97.fcPlcfFldAtn = BitConverter.ToUInt32(data, 152);
            fibRbFcLcb97.lcbPlcfFldAtn = BitConverter.ToUInt32(data, 156);
            fibRbFcLcb97.fcPlcfFldMcr = BitConverter.ToUInt32(data, 160);
            fibRbFcLcb97.lcbPlcfFldMcr = BitConverter.ToUInt32(data, 164);
            fibRbFcLcb97.fcSttbfBkmk = BitConverter.ToUInt32(data, 168);
            fibRbFcLcb97.lcbSttbfBkmk = BitConverter.ToUInt32(data, 172);
            fibRbFcLcb97.fcPlcfBkf = BitConverter.ToUInt32(data, 176);
            fibRbFcLcb97.lcbPlcfBkf = BitConverter.ToUInt32(data, 180);
            fibRbFcLcb97.fcPlcfBkl = BitConverter.ToUInt32(data, 184);
            fibRbFcLcb97.lcbPlcfBkl = BitConverter.ToUInt32(data, 188);
            fibRbFcLcb97.fcCmds = BitConverter.ToUInt32(data, 192);
            fibRbFcLcb97.lcbCmds = BitConverter.ToUInt32(data, 196);
            fibRbFcLcb97.fcUnused1 = BitConverter.ToUInt32(data, 200);
            fibRbFcLcb97.lcbUnused1 = BitConverter.ToUInt32(data, 204);
            fibRbFcLcb97.fcSttbfMcr = BitConverter.ToUInt32(data, 208);
            fibRbFcLcb97.lcbSttbfMcr = BitConverter.ToUInt32(data, 212);
            fibRbFcLcb97.fcPrDrvr = BitConverter.ToUInt32(data, 216);
            fibRbFcLcb97.lcbPrDrvr = BitConverter.ToUInt32(data, 220);
            fibRbFcLcb97.fcPrEnvPort = BitConverter.ToUInt32(data, 224);
            fibRbFcLcb97.lcbPrEnvPort = BitConverter.ToUInt32(data, 228);
            fibRbFcLcb97.fcPrEnvLand = BitConverter.ToUInt32(data, 232);
            fibRbFcLcb97.lcbPrEnvLand = BitConverter.ToUInt32(data, 236);
            fibRbFcLcb97.fcWss = BitConverter.ToUInt32(data, 240);
            fibRbFcLcb97.lcbWss = BitConverter.ToUInt32(data, 244);
            fibRbFcLcb97.fcDop = BitConverter.ToUInt32(data, 248);
            fibRbFcLcb97.lcbDop = BitConverter.ToUInt32(data, 252);
            fibRbFcLcb97.fcSttbfAssoc = BitConverter.ToUInt32(data, 256);
            fibRbFcLcb97.lcbSttbfAssoc = BitConverter.ToUInt32(data, 260);
            fibRbFcLcb97.fcClx = BitConverter.ToUInt32(data, 264);
            fibRbFcLcb97.lcbClx = BitConverter.ToUInt32(data, 268);
            fibRbFcLcb97.fcPlcfPgdFtn = BitConverter.ToUInt32(data, 272);
            fibRbFcLcb97.lcbPlcfPgdFtn = BitConverter.ToUInt32(data, 276);
            fibRbFcLcb97.fcAutosaveSource = BitConverter.ToUInt32(data, 280);
            fibRbFcLcb97.lcbAutosaveSource = BitConverter.ToUInt32(data, 284);
            fibRbFcLcb97.fcGrpXstAtnOwners = BitConverter.ToUInt32(data, 288);
            fibRbFcLcb97.lcbGrpXstAtnOwners = BitConverter.ToUInt32(data, 292);
            fibRbFcLcb97.fcSttbfAtnBkmk = BitConverter.ToUInt32(data, 296);
            fibRbFcLcb97.lcbSttbfAtnBkmk = BitConverter.ToUInt32(data, 300);
            fibRbFcLcb97.fcUnused2 = BitConverter.ToUInt32(data, 304);
            fibRbFcLcb97.lcbUnused2 = BitConverter.ToUInt32(data, 308);
            fibRbFcLcb97.fcUnused3 = BitConverter.ToUInt32(data, 312);
            fibRbFcLcb97.lcbUnused3 = BitConverter.ToUInt32(data, 316);
            fibRbFcLcb97.fcPlcSpaMom = BitConverter.ToUInt32(data, 320);
            fibRbFcLcb97.lcbPlcSpaMom = BitConverter.ToUInt32(data, 324);
            fibRbFcLcb97.fcPlcSpaHdr = BitConverter.ToUInt32(data, 328);
            fibRbFcLcb97.lcbPlcSpaHdr = BitConverter.ToUInt32(data, 332);
            fibRbFcLcb97.fcPlcfAtnBkf = BitConverter.ToUInt32(data, 336);
            fibRbFcLcb97.lcbPlcfAtnBkf = BitConverter.ToUInt32(data, 340);
            fibRbFcLcb97.fcPlcfAtnBkl = BitConverter.ToUInt32(data, 344);
            fibRbFcLcb97.lcbPlcfAtnBkl = BitConverter.ToUInt32(data, 348);
            fibRbFcLcb97.fcPms = BitConverter.ToUInt32(data, 352);
            fibRbFcLcb97.lcbPms = BitConverter.ToUInt32(data, 356);
            fibRbFcLcb97.fcFormFldSttbs = BitConverter.ToUInt32(data, 360);
            fibRbFcLcb97.lcbFormFldSttbs = BitConverter.ToUInt32(data, 364);
            fibRbFcLcb97.fcPlcfendRef = BitConverter.ToUInt32(data, 368);
            fibRbFcLcb97.lcbPlcfendRef = BitConverter.ToUInt32(data, 372);
            fibRbFcLcb97.fcPlcfendTxt = BitConverter.ToUInt32(data, 376);
            fibRbFcLcb97.lcbPlcfendTxt = BitConverter.ToUInt32(data, 380);
            fibRbFcLcb97.fcPlcfFldEdn = BitConverter.ToUInt32(data, 384);
            fibRbFcLcb97.lcbPlcfFldEdn = BitConverter.ToUInt32(data, 388);
            fibRbFcLcb97.fcUnused4 = BitConverter.ToUInt32(data, 392);
            fibRbFcLcb97.lcbUnused4 = BitConverter.ToUInt32(data, 396);
            fibRbFcLcb97.fcDggInfo = BitConverter.ToUInt32(data, 400);
            fibRbFcLcb97.lcbDggInfo = BitConverter.ToUInt32(data, 404);
            fibRbFcLcb97.fcSttbfRMark = BitConverter.ToUInt32(data, 408);
            fibRbFcLcb97.lcbSttbfRMark = BitConverter.ToUInt32(data, 412);
            fibRbFcLcb97.fcSttbfCaption = BitConverter.ToUInt32(data, 416);
            fibRbFcLcb97.lcbSttbfCaption = BitConverter.ToUInt32(data, 420);
            fibRbFcLcb97.fcSttbfAutoCaption = BitConverter.ToUInt32(data, 424);
            fibRbFcLcb97.lcbSttbfAutoCaption = BitConverter.ToUInt32(data, 428);
            fibRbFcLcb97.fcPlcfWkb = BitConverter.ToUInt32(data, 432);
            fibRbFcLcb97.lcbPlcfWkb = BitConverter.ToUInt32(data, 436);
            fibRbFcLcb97.fcPlcfSpl = BitConverter.ToUInt32(data, 440);
            fibRbFcLcb97.lcbPlcfSpl = BitConverter.ToUInt32(data, 444);
            fibRbFcLcb97.fcPlcftxbxTxt = BitConverter.ToUInt32(data, 448);
            fibRbFcLcb97.lcbPlcftxbxTxt = BitConverter.ToUInt32(data, 452);
            fibRbFcLcb97.fcPlcfFldTxbx = BitConverter.ToUInt32(data, 456);
            fibRbFcLcb97.lcbPlcfFldTxbx = BitConverter.ToUInt32(data, 460);
            fibRbFcLcb97.fcPlcfHdrtxbxTxt = BitConverter.ToUInt32(data, 464);
            fibRbFcLcb97.lcbPlcfHdrtxbxTxt = BitConverter.ToUInt32(data, 468);
            fibRbFcLcb97.fcPlcffldHdrTxbx = BitConverter.ToUInt32(data, 472);
            fibRbFcLcb97.lcbPlcffldHdrTxbx = BitConverter.ToUInt32(data, 476);
            fibRbFcLcb97.fcStwUser = BitConverter.ToUInt32(data, 480);
            fibRbFcLcb97.lcbStwUser = BitConverter.ToUInt32(data, 484);
            fibRbFcLcb97.fcSttbTtmbd = BitConverter.ToUInt32(data, 488);
            fibRbFcLcb97.lcbSttbTtmbd = BitConverter.ToUInt32(data, 492);
            fibRbFcLcb97.fcCookieData = BitConverter.ToUInt32(data, 496);
            fibRbFcLcb97.lcbCookieData = BitConverter.ToUInt32(data, 500);
            fibRbFcLcb97.fcPgdMotherOldOld = BitConverter.ToUInt32(data, 504);
            fibRbFcLcb97.lcbPgdMotherOldOld = BitConverter.ToUInt32(data, 508);
            fibRbFcLcb97.fcBkdMotherOldOld = BitConverter.ToUInt32(data, 512);
            fibRbFcLcb97.lcbBkdMotherOldOld = BitConverter.ToUInt32(data, 516);
            fibRbFcLcb97.fcPgdFtnOldOld = BitConverter.ToUInt32(data, 520);
            fibRbFcLcb97.lcbPgdFtnOldOld = BitConverter.ToUInt32(data, 524);
            fibRbFcLcb97.fcBkdFtnOldOld = BitConverter.ToUInt32(data, 528);
            fibRbFcLcb97.lcbBkdFtnOldOld = BitConverter.ToUInt32(data, 532);
            fibRbFcLcb97.fcPgdEdnOldOld = BitConverter.ToUInt32(data, 536);
            fibRbFcLcb97.lcbPgdEdnOldOld = BitConverter.ToUInt32(data, 540);
            fibRbFcLcb97.fcBkdEdnOldOld = BitConverter.ToUInt32(data, 544);
            fibRbFcLcb97.lcbBkdEdnOldOld = BitConverter.ToUInt32(data, 548);
            fibRbFcLcb97.fcSttbfIntlFld = BitConverter.ToUInt32(data, 552);
            fibRbFcLcb97.lcbSttbfIntlFld = BitConverter.ToUInt32(data, 556);
            fibRbFcLcb97.fcRouteSlip = BitConverter.ToUInt32(data, 560);
            fibRbFcLcb97.lcbRouteSlip = BitConverter.ToUInt32(data, 564);
            fibRbFcLcb97.fcSttbSavedBy = BitConverter.ToUInt32(data, 568);
            fibRbFcLcb97.lcbSttbSavedBy = BitConverter.ToUInt32(data, 572);
            fibRbFcLcb97.fcSttbFnm = BitConverter.ToUInt32(data, 576);
            fibRbFcLcb97.lcbSttbFnm = BitConverter.ToUInt32(data, 580);
            fibRbFcLcb97.fcPlfLst = BitConverter.ToUInt32(data, 584);
            fibRbFcLcb97.lcbPlfLst = BitConverter.ToUInt32(data, 588);
            fibRbFcLcb97.fcPlfLfo = BitConverter.ToUInt32(data, 592);
            fibRbFcLcb97.lcbPlfLfo = BitConverter.ToUInt32(data, 596);
            fibRbFcLcb97.fcPlcfTxbxBkd = BitConverter.ToUInt32(data, 600);
            fibRbFcLcb97.lcbPlcfTxbxBkd = BitConverter.ToUInt32(data, 604);
            fibRbFcLcb97.fcPlcfTxbxHdrBkd = BitConverter.ToUInt32(data, 608);
            fibRbFcLcb97.lcbPlcfTxbxHdrBkd = BitConverter.ToUInt32(data, 612);
            fibRbFcLcb97.fcDocUndoWord9 = BitConverter.ToUInt32(data, 616);
            fibRbFcLcb97.lcbDocUndoWord9 = BitConverter.ToUInt32(data, 620);
            fibRbFcLcb97.fcRgbUse = BitConverter.ToUInt32(data, 624);
            fibRbFcLcb97.lcbRgbUse = BitConverter.ToUInt32(data, 628);
            fibRbFcLcb97.fcUsp = BitConverter.ToUInt32(data, 632);
            fibRbFcLcb97.lcbUsp = BitConverter.ToUInt32(data, 636);
            fibRbFcLcb97.fcUskf = BitConverter.ToUInt32(data, 640);
            fibRbFcLcb97.lcbUskf = BitConverter.ToUInt32(data, 644);
            fibRbFcLcb97.fcPlcupcRgbUse = BitConverter.ToUInt32(data, 648);
            fibRbFcLcb97.lcbPlcupcRgbUse = BitConverter.ToUInt32(data, 652);
            fibRbFcLcb97.fcPlcupcUsp = BitConverter.ToUInt32(data, 656);
            fibRbFcLcb97.lcbPlcupcUsp = BitConverter.ToUInt32(data, 660);
            fibRbFcLcb97.fcSttbGlsyStyle = BitConverter.ToUInt32(data, 664);
            fibRbFcLcb97.lcbSttbGlsyStyle = BitConverter.ToUInt32(data, 668);
            fibRbFcLcb97.fcPlgosl = BitConverter.ToUInt32(data, 672);
            fibRbFcLcb97.lcbPlgosl = BitConverter.ToUInt32(data, 676);
            fibRbFcLcb97.fcPlcocx = BitConverter.ToUInt32(data, 680);
            fibRbFcLcb97.lcbPlcocx = BitConverter.ToUInt32(data, 684);
            fibRbFcLcb97.fcPlcfBteLvc = BitConverter.ToUInt32(data, 688);
            fibRbFcLcb97.lcbPlcfBteLvc = BitConverter.ToUInt32(data, 692);
            fibRbFcLcb97.dwLowDateTime = BitConverter.ToUInt32(data, 696);
            fibRbFcLcb97.dwHighDateTime = BitConverter.ToUInt32(data, 700);
            fibRbFcLcb97.fcPlcfLvcPre10 = BitConverter.ToUInt32(data, 704);
            fibRbFcLcb97.lcbPlcfLvcPre10 = BitConverter.ToUInt32(data, 708);
            fibRbFcLcb97.fcPlcfAsumy = BitConverter.ToUInt32(data, 712);
            fibRbFcLcb97.lcbPlcfAsumy = BitConverter.ToUInt32(data, 716);
            fibRbFcLcb97.fcPlcfGram = BitConverter.ToUInt32(data, 720);
            fibRbFcLcb97.lcbPlcfGram = BitConverter.ToUInt32(data, 724);
            fibRbFcLcb97.fcSttbListNames = BitConverter.ToUInt32(data, 728);
            fibRbFcLcb97.lcbSttbListNames = BitConverter.ToUInt32(data, 732);
            fibRbFcLcb97.fcSttbfUssr = BitConverter.ToUInt32(data, 736);
            fibRbFcLcb97.lcbSttbfUssr = BitConverter.ToUInt32(data, 740);
            if (fibRbFcLcb2000 != null) 
            {
                fibRbFcLcb2000.fcPlcfTch = BitConverter.ToUInt32(data, 744);
                fibRbFcLcb2000.lcbPlcfTch = BitConverter.ToUInt32(data, 748);
                fibRbFcLcb2000.fcRmdThreading = BitConverter.ToUInt32(data, 752);
                fibRbFcLcb2000.lcbRmdThreading = BitConverter.ToUInt32(data, 756);
                fibRbFcLcb2000.fcMid = BitConverter.ToUInt32(data, 760);
                fibRbFcLcb2000.lcbMid = BitConverter.ToUInt32(data, 764);
                fibRbFcLcb2000.fcSttbRgtplc = BitConverter.ToUInt32(data, 768);
                fibRbFcLcb2000.lcbSttbRgtplc = BitConverter.ToUInt32(data, 772);
                fibRbFcLcb2000.fcMsoEnvelope = BitConverter.ToUInt32(data, 776);
                fibRbFcLcb2000.lcbMsoEnvelope = BitConverter.ToUInt32(data, 780);
                fibRbFcLcb2000.fcPlcfLad = BitConverter.ToUInt32(data, 784);
                fibRbFcLcb2000.lcbPlcfLad = BitConverter.ToUInt32(data, 788);
                fibRbFcLcb2000.fcRgDofr = BitConverter.ToUInt32(data, 792);
                fibRbFcLcb2000.lcbRgDofr = BitConverter.ToUInt32(data, 796);
                fibRbFcLcb2000.fcPlcosl = BitConverter.ToUInt32(data, 800);
                fibRbFcLcb2000.lcbPlcosl = BitConverter.ToUInt32(data, 804);
                fibRbFcLcb2000.fcPlcfCookieOld = BitConverter.ToUInt32(data, 808);
                fibRbFcLcb2000.lcbPlcfCookieOld = BitConverter.ToUInt32(data, 812);
                fibRbFcLcb2000.fcPgdMotherOld = BitConverter.ToUInt32(data, 816);
                fibRbFcLcb2000.lcbPgdMotherOld = BitConverter.ToUInt32(data, 820);
                fibRbFcLcb2000.fcBkdMotherOld = BitConverter.ToUInt32(data, 824);
                fibRbFcLcb2000.lcbBkdMotherOld = BitConverter.ToUInt32(data, 828);
                fibRbFcLcb2000.fcPgdFtnOld = BitConverter.ToUInt32(data, 832);
                fibRbFcLcb2000.lcbPgdFtnOld = BitConverter.ToUInt32(data, 836);
                fibRbFcLcb2000.fcBkdFtnOld = BitConverter.ToUInt32(data, 840);
                fibRbFcLcb2000.lcbBkdFtnOld = BitConverter.ToUInt32(data, 844);
                fibRbFcLcb2000.fcPgdEdnOld = BitConverter.ToUInt32(data, 848);
                fibRbFcLcb2000.lcbPgdEdnOld = BitConverter.ToUInt32(data, 852);
                fibRbFcLcb2000.fcBkdEdnOld = BitConverter.ToUInt32(data, 856);
                fibRbFcLcb2000.lcbBkdEdnOld = BitConverter.ToUInt32(data, 860);
            }
            if (fibRbFcLcb2002 != null) 
            {
                fibRbFcLcb2002.fcUnused10 = BitConverter.ToUInt32(data, 864);
                fibRbFcLcb2002.lcbUnused10 = BitConverter.ToUInt32(data, 868);
                fibRbFcLcb2002.fcPlcfPgp = BitConverter.ToUInt32(data, 872);
                fibRbFcLcb2002.lcbPlcfPgp = BitConverter.ToUInt32(data, 876);
                fibRbFcLcb2002.fcPlcfuim = BitConverter.ToUInt32(data, 880);
                fibRbFcLcb2002.lcbPlcfuim = BitConverter.ToUInt32(data, 884);
                fibRbFcLcb2002.fcPlfguidUim = BitConverter.ToUInt32(data, 888);
                fibRbFcLcb2002.lcbPlfguidUim = BitConverter.ToUInt32(data, 892);
                fibRbFcLcb2002.fcAtrdExtra = BitConverter.ToUInt32(data, 896);
                fibRbFcLcb2002.lcbAtrdExtra = BitConverter.ToUInt32(data, 900);
                fibRbFcLcb2002.fcPlrsid = BitConverter.ToUInt32(data, 904);
                fibRbFcLcb2002.lcbPlrsid = BitConverter.ToUInt32(data, 908);
                fibRbFcLcb2002.fcSttbfBkmkFactoid = BitConverter.ToUInt32(data, 912);
                fibRbFcLcb2002.lcbSttbfBkmkFactoid = BitConverter.ToUInt32(data, 916);
                fibRbFcLcb2002.fcPlcfBkfFactoid = BitConverter.ToUInt32(data, 920);
                fibRbFcLcb2002.lcbPlcfBkfFactoid = BitConverter.ToUInt32(data, 924);
                fibRbFcLcb2002.fcPlcfcookie = BitConverter.ToUInt32(data, 928);
                fibRbFcLcb2002.lcbPlcfcookie = BitConverter.ToUInt32(data, 932);
                fibRbFcLcb2002.fcPlcfBklFactoid = BitConverter.ToUInt32(data, 936);
                fibRbFcLcb2002.lcbPlcfBklFactoid = BitConverter.ToUInt32(data, 940);
                fibRbFcLcb2002.fcFactoidData = BitConverter.ToUInt32(data, 944);
                fibRbFcLcb2002.lcbFactoidData = BitConverter.ToUInt32(data, 948);
                fibRbFcLcb2002.fcDocUndo = BitConverter.ToUInt32(data, 952);
                fibRbFcLcb2002.lcbDocUndo = BitConverter.ToUInt32(data, 956);
                fibRbFcLcb2002.fcSttbfBkmkFcc = BitConverter.ToUInt32(data, 960);
                fibRbFcLcb2002.lcbSttbfBkmkFcc = BitConverter.ToUInt32(data, 964);
                fibRbFcLcb2002.fcPlcfBkfFcc = BitConverter.ToUInt32(data, 968);
                fibRbFcLcb2002.lcbPlcfBkfFcc = BitConverter.ToUInt32(data, 972);
                fibRbFcLcb2002.fcPlcfBklFcc = BitConverter.ToUInt32(data, 976);
                fibRbFcLcb2002.lcbPlcfBklFcc = BitConverter.ToUInt32(data, 980);
                fibRbFcLcb2002.fcSttbfbkmkBPRepairs = BitConverter.ToUInt32(data, 984);
                fibRbFcLcb2002.lcbSttbfbkmkBPRepairs = BitConverter.ToUInt32(data, 988);
                fibRbFcLcb2002.fcPlcfbkfBPRepairs = BitConverter.ToUInt32(data, 992);
                fibRbFcLcb2002.lcbPlcfbkfBPRepairs = BitConverter.ToUInt32(data, 996);
                fibRbFcLcb2002.fcPlcfbklBPRepairs = BitConverter.ToUInt32(data, 1000);
                fibRbFcLcb2002.lcbPlcfbklBPRepairs = BitConverter.ToUInt32(data, 1004);
                fibRbFcLcb2002.fcPmsNew = BitConverter.ToUInt32(data, 1008);
                fibRbFcLcb2002.lcbPmsNew = BitConverter.ToUInt32(data, 1012);
                fibRbFcLcb2002.fcODSO = BitConverter.ToUInt32(data, 1016);
                fibRbFcLcb2002.lcbODSO = BitConverter.ToUInt32(data, 1020);
                fibRbFcLcb2002.fcPlcfpmiOldXP = BitConverter.ToUInt32(data, 1024);
                fibRbFcLcb2002.lcbPlcfpmiOldXP = BitConverter.ToUInt32(data, 1028);
                fibRbFcLcb2002.fcPlcfpmiNewXP = BitConverter.ToUInt32(data, 1032);
                fibRbFcLcb2002.lcbPlcfpmiNewXP = BitConverter.ToUInt32(data, 1036);
                fibRbFcLcb2002.fcPlcfpmiMixedXP = BitConverter.ToUInt32(data, 1040);
                fibRbFcLcb2002.lcbPlcfpmiMixedXP = BitConverter.ToUInt32(data, 1044);
                fibRbFcLcb2002.fcUnused20 = BitConverter.ToUInt32(data, 1048);
                fibRbFcLcb2002.lcbUnused20 = BitConverter.ToUInt32(data, 1052);
                fibRbFcLcb2002.fcPlcffactoid = BitConverter.ToUInt32(data, 1056);
                fibRbFcLcb2002.lcbPlcffactoid = BitConverter.ToUInt32(data, 1060);
                fibRbFcLcb2002.fcPlcflvcOldXP = BitConverter.ToUInt32(data, 1064);
                fibRbFcLcb2002.lcbPlcflvcOldXP = BitConverter.ToUInt32(data, 1068);
                fibRbFcLcb2002.fcPlcflvcNewXP = BitConverter.ToUInt32(data, 1072);
                fibRbFcLcb2002.lcbPlcflvcNewXP = BitConverter.ToUInt32(data, 1076);
                fibRbFcLcb2002.fcPlcflvcMixedXP = BitConverter.ToUInt32(data, 1080);
                fibRbFcLcb2002.lcbPlcflvcMixedXP = BitConverter.ToUInt32(data, 1084);
            }
            if (fibRbFcLcb2003 != null) 
            {
                fibRbFcLcb2003.fcHplxsdr = BitConverter.ToUInt32(data, 1088);
                fibRbFcLcb2003.lcbHplxsdr = BitConverter.ToUInt32(data, 1092);
                fibRbFcLcb2003.fcSttbfBkmkSdt = BitConverter.ToUInt32(data, 1096);
                fibRbFcLcb2003.lcbSttbfBkmkSdt = BitConverter.ToUInt32(data, 1100);
                fibRbFcLcb2003.fcPlcfBkfSdt = BitConverter.ToUInt32(data, 1104);
                fibRbFcLcb2003.lcbPlcfBkfSdt = BitConverter.ToUInt32(data, 1108);
                fibRbFcLcb2003.fcPlcfBklSdt = BitConverter.ToUInt32(data, 1112);
                fibRbFcLcb2003.lcbPlcfBklSdt = BitConverter.ToUInt32(data, 1116);
                fibRbFcLcb2003.fcCustomXForm = BitConverter.ToUInt32(data, 1120);
                fibRbFcLcb2003.lcbCustomXForm = BitConverter.ToUInt32(data, 1124);
                fibRbFcLcb2003.fcSttbfBkmkProt = BitConverter.ToUInt32(data, 1128);
                fibRbFcLcb2003.lcbSttbfBkmkProt = BitConverter.ToUInt32(data, 1132);
                fibRbFcLcb2003.fcPlcfBkfProt = BitConverter.ToUInt32(data, 1136);
                fibRbFcLcb2003.lcbPlcfBkfProt = BitConverter.ToUInt32(data, 1140);
                fibRbFcLcb2003.fcPlcfBklProt = BitConverter.ToUInt32(data, 1144);
                fibRbFcLcb2003.lcbPlcfBklProt = BitConverter.ToUInt32(data, 1148);
                fibRbFcLcb2003.fcSttbProtUser = BitConverter.ToUInt32(data, 1152);
                fibRbFcLcb2003.lcbSttbProtUser = BitConverter.ToUInt32(data, 1156);
                fibRbFcLcb2003.fcUnused = BitConverter.ToUInt32(data, 1160);
                fibRbFcLcb2003.lcbUnused = BitConverter.ToUInt32(data, 1164);
                fibRbFcLcb2003.fcPlcfpmiOld = BitConverter.ToUInt32(data, 1168);
                fibRbFcLcb2003.lcbPlcfpmiOld = BitConverter.ToUInt32(data, 1172);
                fibRbFcLcb2003.fcPlcfpmiOldInline = BitConverter.ToUInt32(data, 1176);
                fibRbFcLcb2003.lcbPlcfpmiOldInline = BitConverter.ToUInt32(data, 1180);
                fibRbFcLcb2003.fcPlcfpmiNew = BitConverter.ToUInt32(data, 1184);
                fibRbFcLcb2003.lcbPlcfpmiNew = BitConverter.ToUInt32(data, 1188);
                fibRbFcLcb2003.fcPlcfpmiNewInline = BitConverter.ToUInt32(data, 1192);
                fibRbFcLcb2003.lcbPlcfpmiNewInline = BitConverter.ToUInt32(data, 1196);
                fibRbFcLcb2003.fcPlcflvcOld = BitConverter.ToUInt32(data, 1200);
                fibRbFcLcb2003.lcbPlcflvcOld = BitConverter.ToUInt32(data, 1204);
                fibRbFcLcb2003.fcPlcflvcOldInline = BitConverter.ToUInt32(data, 1208);
                fibRbFcLcb2003.lcbPlcflvcOldInline = BitConverter.ToUInt32(data, 1212);
                fibRbFcLcb2003.fcPlcflvcNew = BitConverter.ToUInt32(data, 1216);
                fibRbFcLcb2003.lcbPlcflvcNew = BitConverter.ToUInt32(data, 1220);
                fibRbFcLcb2003.fcPlcflvcNewInline = BitConverter.ToUInt32(data, 1224);
                fibRbFcLcb2003.lcbPlcflvcNewInline = BitConverter.ToUInt32(data, 1228);
                fibRbFcLcb2003.fcPgdMother = BitConverter.ToUInt32(data, 1232);
                fibRbFcLcb2003.lcbPgdMother = BitConverter.ToUInt32(data, 1236);
                fibRbFcLcb2003.fcBkdMother = BitConverter.ToUInt32(data, 1240);
                fibRbFcLcb2003.lcbBkdMother = BitConverter.ToUInt32(data, 1244);
                fibRbFcLcb2003.fcAfdMother = BitConverter.ToUInt32(data, 1248);
                fibRbFcLcb2003.lcbAfdMother = BitConverter.ToUInt32(data, 1252);
                fibRbFcLcb2003.fcPgdFtn = BitConverter.ToUInt32(data, 1256);
                fibRbFcLcb2003.lcbPgdFtn = BitConverter.ToUInt32(data, 1260);
                fibRbFcLcb2003.fcBkdFtn = BitConverter.ToUInt32(data, 1264);
                fibRbFcLcb2003.lcbBkdFtn = BitConverter.ToUInt32(data, 1268);
                fibRbFcLcb2003.fcAfdFtn = BitConverter.ToUInt32(data, 1272);
                fibRbFcLcb2003.lcbAfdFtn = BitConverter.ToUInt32(data, 1276);
                fibRbFcLcb2003.fcPgdEdn = BitConverter.ToUInt32(data, 1280);
                fibRbFcLcb2003.lcbPgdEdn = BitConverter.ToUInt32(data, 1284);
                fibRbFcLcb2003.fcBkdEdn = BitConverter.ToUInt32(data, 1288);
                fibRbFcLcb2003.lcbBkdEdn = BitConverter.ToUInt32(data, 1292);
                fibRbFcLcb2003.fcAfdEdn = BitConverter.ToUInt32(data, 1296);
                fibRbFcLcb2003.lcbAfdEdn = BitConverter.ToUInt32(data, 1300);
                fibRbFcLcb2003.fcAfd = BitConverter.ToUInt32(data, 1304);
                fibRbFcLcb2003.lcbAfd = BitConverter.ToUInt32(data, 1308);
            }
            if (fibRbFcLcb2007 != null) 
            {
                fibRbFcLcb2007.fcPlcfmthd = BitConverter.ToUInt32(data, 1312);
                fibRbFcLcb2007.lcbPlcfmthd = BitConverter.ToUInt32(data, 1316);
                fibRbFcLcb2007.fcSttbfBkmkMoveFrom = BitConverter.ToUInt32(data, 1320);
                fibRbFcLcb2007.lcbSttbfBkmkMoveFrom = BitConverter.ToUInt32(data, 1324);
                fibRbFcLcb2007.fcPlcfBkfMoveFrom = BitConverter.ToUInt32(data, 1328);
                fibRbFcLcb2007.lcbPlcfBkfMoveFrom = BitConverter.ToUInt32(data, 1332);
                fibRbFcLcb2007.fcPlcfBklMoveFrom = BitConverter.ToUInt32(data, 1336);
                fibRbFcLcb2007.lcbPlcfBklMoveFrom = BitConverter.ToUInt32(data, 1340);
                fibRbFcLcb2007.fcSttbfBkmkMoveTo = BitConverter.ToUInt32(data, 1344);
                fibRbFcLcb2007.lcbSttbfBkmkMoveTo = BitConverter.ToUInt32(data, 1348);
                fibRbFcLcb2007.fcPlcfBkfMoveTo = BitConverter.ToUInt32(data, 1352);
                fibRbFcLcb2007.lcbPlcfBkfMoveTo = BitConverter.ToUInt32(data, 1356);
                fibRbFcLcb2007.fcPlcfBklMoveTo = BitConverter.ToUInt32(data, 1360);
                fibRbFcLcb2007.lcbPlcfBklMoveTo = BitConverter.ToUInt32(data, 1364);
                fibRbFcLcb2007.fcUnused11 = BitConverter.ToUInt32(data, 1368);
                fibRbFcLcb2007.lcbUnused11 = BitConverter.ToUInt32(data, 1372);
                fibRbFcLcb2007.fcUnused22 = BitConverter.ToUInt32(data, 1376);
                fibRbFcLcb2007.lcbUnused22 = BitConverter.ToUInt32(data, 1380);
                fibRbFcLcb2007.fcUnused33 = BitConverter.ToUInt32(data, 1384);
                fibRbFcLcb2007.lcbUnused33 = BitConverter.ToUInt32(data, 1388);
                fibRbFcLcb2007.fcSttbfBkmkArto = BitConverter.ToUInt32(data, 1392);
                fibRbFcLcb2007.lcbSttbfBkmkArto = BitConverter.ToUInt32(data, 1396);
                fibRbFcLcb2007.fcPlcfBkfArto = BitConverter.ToUInt32(data, 1400);
                fibRbFcLcb2007.lcbPlcfBkfArto = BitConverter.ToUInt32(data, 1404);
                fibRbFcLcb2007.fcPlcfBklArto = BitConverter.ToUInt32(data, 1408);
                fibRbFcLcb2007.lcbPlcfBklArto = BitConverter.ToUInt32(data, 1412);
                fibRbFcLcb2007.fcArtoData = BitConverter.ToUInt32(data, 1416);
                fibRbFcLcb2007.lcbArtoData = BitConverter.ToUInt32(data, 1420);
                fibRbFcLcb2007.fcUnused44 = BitConverter.ToUInt32(data, 1424);
                fibRbFcLcb2007.lcbUnused44 = BitConverter.ToUInt32(data, 1428);
                fibRbFcLcb2007.fcUnused5 = BitConverter.ToUInt32(data, 1432);
                fibRbFcLcb2007.lcbUnused5 = BitConverter.ToUInt32(data, 1436);
                fibRbFcLcb2007.fcUnused6 = BitConverter.ToUInt32(data, 1440);
                fibRbFcLcb2007.lcbUnused6 = BitConverter.ToUInt32(data, 1444);
                fibRbFcLcb2007.fcOssTheme = BitConverter.ToUInt32(data, 1448);
                fibRbFcLcb2007.lcbOssTheme = BitConverter.ToUInt32(data, 1452);
                fibRbFcLcb2007.fcColorSchemeMapping = BitConverter.ToUInt32(data, 1456);
                fibRbFcLcb2007.lcbColorSchemeMapping = BitConverter.ToUInt32(data, 1460);
            }
            return fibRbFcLcb97;
        }
        static FibRgLw97 ReadFibRgLw97(Stream s, ushort size)
        {
            int FibRgLw97Size = 88;
            if ((size * 4) != FibRgLw97Size) 
                throw new Exception("Invalid FibRgLw97 size");
            byte[] data = ReadUtils.ReadExact(s, FibRgLw97Size);
            FibRgLw97 fibRgLw97 = new FibRgLw97();
            fibRgLw97.cbMac = BitConverter.ToUInt32(data, 0);
            fibRgLw97.ccpText = BitConverter.ToUInt32(data, 12);
            fibRgLw97.ccpFtn = BitConverter.ToUInt32(data, 16);
            fibRgLw97.ccpHdd = BitConverter.ToUInt32(data, 20);
            fibRgLw97.ccpAtn = BitConverter.ToUInt32(data, 28);
            fibRgLw97.ccpEdn = BitConverter.ToUInt32(data, 32);
            fibRgLw97.ccpTxbx = BitConverter.ToUInt32(data, 36);
            fibRgLw97.ccpHdrTxbx = BitConverter.ToUInt32(data, 40);
            return fibRgLw97;
        }
        static FibRgW97 ReadFibRgW97(Stream s, ushort size)
        {
            int FibRgW97Size = 28;
            if ((size * 2) < FibRgW97Size) 
                throw new Exception("Invalid FibRgW97 size");
            byte[] data = ReadUtils.ReadExact(s, FibRgW97Size);
            FibRgW97 fibRgW97 = new FibRgW97();
            fibRgW97.lidFE = BitConverter.ToUInt16(data, 26);
            return fibRgW97;
        }
        static FibBase ReadFibBase(Stream s)
        {
            uint WordFileSignature = (uint)0xA5EC;
            uint WordFileSignature6 = (uint)0xA5DC;
            byte[] data = ReadUtils.ReadExact(s, 32);
            FibBase fibBase = new FibBase();
            fibBase.wIdent = BitConverter.ToUInt16(data, 0);
            fibBase.nFib = BitConverter.ToUInt16(data, 2);
            if (fibBase.wIdent == WordFileSignature6) 
                return null;
            if (fibBase.wIdent != WordFileSignature) 
                throw new Exception("Invalid word file signature");
            fibBase.lid = BitConverter.ToUInt16(data, 6);
            fibBase.pnNext = BitConverter.ToUInt16(data, 8);
            ushort flags;
            flags = BitConverter.ToUInt16(data, 10);
            fibBase.fDot = ((flags & 0x0001)) != 0;
            fibBase.fGlsy = ((flags & 0x0002)) != 0;
            fibBase.fComplex = ((flags & 0x0004)) != 0;
            fibBase.fHasPic = ((flags & 0x0008)) != 0;
            fibBase.cQuickSaves = (byte)((((flags >> 4)) & 0x0F));
            fibBase.fEncrypted = ((flags & 0x0100)) != 0;
            fibBase.fWhichTblStm = ((flags & 0x0200)) != 0;
            fibBase.fReadOnlyRecommended = ((flags & 0x0400)) != 0;
            fibBase.fWriteReservation = ((flags & 0x0800)) != 0;
            fibBase.fExtChar = ((flags & 0x1000)) != 0;
            fibBase.fLoadOverride = ((flags & 0x2000)) != 0;
            fibBase.fFarEast = ((flags & 0x4000)) != 0;
            fibBase.fObfuscated = ((flags & 0x8000)) != 0;
            fibBase.lKey = BitConverter.ToUInt32(data, 14);
            fibBase.envr = data[18];
            flags = data[19];
            fibBase.fMac = ((flags & 0x0001)) != 0;
            fibBase.fEmptySpecial = ((flags & 0x0002)) != 0;
            fibBase.fLoadOverridePage = ((flags & 0x0004)) != 0;
            return fibBase;
        }
    }
}