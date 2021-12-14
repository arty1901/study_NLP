/*
 * SDK Pullenti Lingvo, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Pullenti.Ner.Address.Internal
{
    static class AddressDefineHelper
    {
        public static Pullenti.Ner.Token TryDefine(List<AddressItemToken> li, Pullenti.Ner.Token t, Pullenti.Ner.Core.AnalyzerData ad)
        {
            if (li == null || li.Count == 0) 
                return null;
            if ((li.Count == 1 && li[0].Typ == AddressItemType.Street && (li[0].Referent as Pullenti.Ner.Address.StreetReferent).Kind == Pullenti.Ner.Address.StreetKind.Railway) && (li[0].Referent as Pullenti.Ner.Address.StreetReferent).Number == null) 
                return null;
            if ((li.Count > 3 && li[0].Typ == AddressItemType.City && li[1].Typ == AddressItemType.Street) && li[2].Typ == AddressItemType.City && li[3].Typ == AddressItemType.Street) 
            {
                if ((li[1].Referent as Pullenti.Ner.Address.StreetReferent).Kind == Pullenti.Ner.Address.StreetKind.Railway || (li[1].Referent as Pullenti.Ner.Address.StreetReferent).Kind == Pullenti.Ner.Address.StreetKind.Road) 
                {
                    Pullenti.Ner.Geo.GeoReferent geo = li[2].Referent as Pullenti.Ner.Geo.GeoReferent;
                    if (geo != null && geo.Higher == null && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(li[0].Referent as Pullenti.Ner.Geo.GeoReferent, geo)) 
                    {
                        geo.Higher = li[0].Referent as Pullenti.Ner.Geo.GeoReferent;
                        li[2] = li[2].Clone();
                        li[2].BeginToken = li[0].BeginToken;
                        li.RemoveRange(0, 2);
                    }
                }
            }
            if ((li.Count > 2 && li[0].Typ == AddressItemType.City && li[1].Typ == AddressItemType.Street) && li[2].Typ == AddressItemType.City && (li[1].Referent is Pullenti.Ner.Address.StreetReferent)) 
            {
                Pullenti.Ner.Geo.GeoReferent geo = li[2].Referent as Pullenti.Ner.Geo.GeoReferent;
                if ((geo.IsTerritory && (li[1].Referent as Pullenti.Ner.Address.StreetReferent).Kind == Pullenti.Ner.Address.StreetKind.Area && geo.Higher == null) && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(li[0].Referent as Pullenti.Ner.Geo.GeoReferent, geo)) 
                {
                    geo.Higher = li[0].Referent as Pullenti.Ner.Geo.GeoReferent;
                    Pullenti.Ner.Token b = li[1].BeginToken;
                    Pullenti.Ner.Token e = li[1].EndToken;
                    li[1].BeginToken = li[2].BeginToken;
                    li[1].EndToken = li[2].EndToken;
                    li[2].BeginToken = b;
                    li[2].EndToken = e;
                    AddressItemToken ll = li[1];
                    li[1] = li[2];
                    li[2] = ll;
                }
            }
            Pullenti.Ner.Address.AddressReferent addr = new Pullenti.Ner.Address.AddressReferent();
            List<AddressItemToken> streets = new List<AddressItemToken>();
            int i;
            int j;
            AddressItemToken metro = null;
            Pullenti.Ner.Address.AddressDetailType detTyp = Pullenti.Ner.Address.AddressDetailType.Undefined;
            int detParam = 0;
            List<Pullenti.Ner.Geo.GeoReferent> geos = null;
            bool err = false;
            AddressItemToken nearCity = null;
            for (i = 0; i < li.Count; i++) 
            {
                if ((li[i].Typ == AddressItemType.Detail && li[i].DetailType == Pullenti.Ner.Address.AddressDetailType.Cross && ((i + 2) < li.Count)) && li[i + 1].Typ == AddressItemType.Street && li[i + 2].Typ == AddressItemType.Street) 
                {
                    detTyp = Pullenti.Ner.Address.AddressDetailType.Cross;
                    streets.Add(li[i + 1]);
                    streets.Add(li[i + 2]);
                    li[i + 1].EndToken = li[i + 2].EndToken;
                    li[i].Tag = addr;
                    li[i + 1].Tag = addr;
                    li.RemoveAt(i + 2);
                    break;
                }
                else if (li[i].Typ == AddressItemType.Street) 
                {
                    if (((li[i].RefToken != null && !li[i].RefTokenIsGsk)) && streets.Count == 0) 
                    {
                        if (i > 0 && li[i].IsNewlineBefore) 
                            err = true;
                        else if ((i + 1) == li.Count) 
                            err = detTyp == Pullenti.Ner.Address.AddressDetailType.Undefined && detParam == 0 && nearCity == null;
                        else if (((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.Number) 
                            err = true;
                        if (err && geos != null) 
                        {
                            for (int ii = i - 1; ii >= 0; ii--) 
                            {
                                if (li[ii].Typ == AddressItemType.Zip || li[ii].Typ == AddressItemType.Prefix) 
                                    err = false;
                            }
                        }
                        if (err) 
                            break;
                    }
                    li[i].Tag = addr;
                    streets.Add(li[i]);
                    if (((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.Street) 
                    {
                    }
                    else 
                        break;
                }
                else if (li[i].Typ == AddressItemType.City || li[i].Typ == AddressItemType.Region) 
                {
                    if (geos == null) 
                        geos = new List<Pullenti.Ner.Geo.GeoReferent>();
                    geos.Insert(0, li[i].Referent as Pullenti.Ner.Geo.GeoReferent);
                    if (li[i].DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && detTyp == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                    {
                        if (li[i].Typ == AddressItemType.City && li[i].DetailType == Pullenti.Ner.Address.AddressDetailType.Near && li[i].DetailMeters == 0) 
                            nearCity = li[i];
                        else 
                            detTyp = li[i].DetailType;
                    }
                    if (li[i].DetailMeters > 0 && detParam == 0) 
                        detParam = li[i].DetailMeters;
                }
                else if (li[i].Typ == AddressItemType.Detail) 
                {
                    if (li[i].DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && detTyp == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                    {
                        if (li[i].DetailType == Pullenti.Ner.Address.AddressDetailType.Near && ((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.City) 
                        {
                            nearCity = li[i + 1];
                            li[i].Tag = addr;
                            i++;
                        }
                        else 
                        {
                            detTyp = li[i].DetailType;
                            if (li[i].DetailMeters > 0) 
                                detParam = li[i].DetailMeters;
                        }
                    }
                    li[i].Tag = addr;
                }
            }
            if (i >= li.Count && metro == null && detTyp == Pullenti.Ner.Address.AddressDetailType.Undefined) 
            {
                for (i = 0; i < li.Count; i++) 
                {
                    bool cit = false;
                    if (li[i].Typ == AddressItemType.City) 
                        cit = true;
                    else if (li[i].Typ == AddressItemType.Region) 
                    {
                        foreach (Pullenti.Ner.Slot s in li[i].Referent.Slots) 
                        {
                            if (s.TypeName == Pullenti.Ner.Geo.GeoReferent.ATTR_TYPE) 
                            {
                                string ss = s.Value as string;
                                if (ss.Contains("посел") || ss.Contains("сельск") || ss.Contains("почтовое отделение")) 
                                    cit = true;
                            }
                        }
                    }
                    if (cit) 
                    {
                        if (((i + 1) < li.Count) && (((((li[i + 1].Typ == AddressItemType.House || li[i + 1].Typ == AddressItemType.Block || li[i + 1].Typ == AddressItemType.Plot) || li[i + 1].Typ == AddressItemType.Field || li[i + 1].Typ == AddressItemType.Building) || li[i + 1].Typ == AddressItemType.Corpus || li[i + 1].Typ == AddressItemType.PostOfficeBox) || li[i + 1].Typ == AddressItemType.CSP))) 
                            break;
                        if (((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.Number) 
                        {
                            if (li[i].EndToken.Next.IsComma) 
                            {
                                if ((li[i].Referent is Pullenti.Ner.Geo.GeoReferent) && !(li[i].Referent as Pullenti.Ner.Geo.GeoReferent).IsBigCity && (li[i].Referent as Pullenti.Ner.Geo.GeoReferent).IsCity) 
                                {
                                    li[i + 1].Typ = AddressItemType.House;
                                    break;
                                }
                            }
                        }
                        if (li[0].Typ == AddressItemType.Zip || li[0].Typ == AddressItemType.Prefix) 
                            break;
                        continue;
                    }
                    if (li[i].Typ == AddressItemType.Region) 
                    {
                        if ((li[i].Referent is Pullenti.Ner.Geo.GeoReferent) && (li[i].Referent as Pullenti.Ner.Geo.GeoReferent).Higher != null && (li[i].Referent as Pullenti.Ner.Geo.GeoReferent).Higher.IsCity) 
                        {
                            if (((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.House) 
                                break;
                        }
                    }
                }
                if (i >= li.Count) 
                    return null;
            }
            if (err) 
                return null;
            int i0 = i;
            if (i > 0 && li[i - 1].Typ == AddressItemType.House && li[i - 1].IsDigit) 
            {
                addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_HOUSE, li[i - 1].Value, false, 0).Tag = li[i - 1];
                li[i - 1].Tag = addr;
            }
            else if ((i > 0 && li[i - 1].Typ == AddressItemType.Kilometer && li[i - 1].IsDigit) && (i < li.Count) && li[i].IsStreetRoad) 
            {
                addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_KILOMETER, li[i - 1].Value, false, 0).Tag = li[i - 1];
                li[i - 1].Tag = addr;
            }
            else 
            {
                if (i >= li.Count) 
                    i = -1;
                for (i = 0; i < li.Count; i++) 
                {
                    if (li[i].Tag != null) 
                        continue;
                    if (li[i].Typ == AddressItemType.House) 
                    {
                        if (addr.House != null) 
                            break;
                        if (li[i].Value != null) 
                        {
                            addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_HOUSE, li[i].Value, false, 0).Tag = li[i];
                            if (li[i].HouseType != Pullenti.Ner.Address.AddressHouseType.Undefined) 
                                addr.HouseType = li[i].HouseType;
                        }
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Kilometer && li[i].IsDigit && (((i0 < li.Count) && li[i0].IsStreetRoad))) 
                    {
                        if (addr.Kilometer != null) 
                            break;
                        Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_KILOMETER, li[i].Value, false, 0);
                        if (s != null) 
                            s.Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Plot) 
                    {
                        if (addr.Plot != null) 
                            break;
                        Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_PLOT, li[i].Value, false, 0);
                        if (s != null) 
                            s.Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Field) 
                    {
                        if (addr.Field != null) 
                            break;
                        Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_FIELD, li[i].Value, false, 0);
                        if (s != null) 
                            s.Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Box && li[i].IsDigit) 
                    {
                        if (addr.Box != null) 
                            break;
                        Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_BOX, li[i].Value, false, 0);
                        if (s != null) 
                            s.Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Block && li[i].IsDigit) 
                    {
                        if (addr.Block != null) 
                            break;
                        Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_BLOCK, li[i].Value, false, 0);
                        if (s != null) 
                            s.Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Corpus) 
                    {
                        if (addr.Corpus != null) 
                            break;
                        if (li[i].Value != null) 
                        {
                            Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_CORPUS, li[i].Value, false, 0);
                            if (s != null) 
                                s.Tag = li[i];
                        }
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Building) 
                    {
                        if (addr.Building != null) 
                            break;
                        if (li[i].Value != null) 
                        {
                            Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_BUILDING, li[i].Value, false, 0);
                            if (s != null) 
                                s.Tag = li[i];
                            if (li[i].BuildingType != Pullenti.Ner.Address.AddressBuildingType.Undefined) 
                                addr.BuildingType = li[i].BuildingType;
                        }
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Floor && li[i].IsDigit) 
                    {
                        if (addr.Floor != null) 
                            break;
                        Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_FLOOR, li[i].Value, false, 0);
                        if (s != null) 
                            s.Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Potch && li[i].IsDigit) 
                    {
                        if (addr.Potch != null) 
                            break;
                        Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_PORCH, li[i].Value, false, 0);
                        if (s != null) 
                            s.Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Flat) 
                    {
                        if (addr.Flat != null) 
                            break;
                        if (li[i].Value != null) 
                            addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_FLAT, li[i].Value, false, 0).Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Office && li[i].IsDigit) 
                    {
                        if (addr.Office != null) 
                            break;
                        Pullenti.Ner.Slot s = addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_OFFICE, li[i].Value, false, 0);
                        if (s != null) 
                            s.Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.CorpusOrFlat && ((li[i].IsDigit || li[i].Value == null))) 
                    {
                        for (j = i + 1; j < li.Count; j++) 
                        {
                            if (li[j].IsDigit) 
                            {
                                if (((li[j].Typ == AddressItemType.Flat || li[j].Typ == AddressItemType.CorpusOrFlat || li[j].Typ == AddressItemType.Office) || li[j].Typ == AddressItemType.Floor || li[j].Typ == AddressItemType.Potch) || li[j].Typ == AddressItemType.PostOfficeBox || li[j].Typ == AddressItemType.Building) 
                                    break;
                            }
                        }
                        if (li[i].Value != null) 
                        {
                            if ((j < li.Count) && addr.Corpus == null) 
                                addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_CORPUS, li[i].Value, false, 0).Tag = li[i];
                            else if (addr.Corpus != null) 
                                addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_FLAT, li[i].Value, false, 0).Tag = li[i];
                            else 
                                addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_CORPUSORFLAT, li[i].Value, false, 0).Tag = li[i];
                        }
                        li[i].Tag = addr;
                    }
                    else if ((!li[i].IsNewlineBefore && li[i].Typ == AddressItemType.Number && li[i].IsDigit) && li[i - 1].Typ == AddressItemType.Street) 
                    {
                        int v = 0;
                        if (!int.TryParse(li[i].Value, out v)) 
                        {
                            if (!int.TryParse(li[i].Value.Substring(0, li[i].Value.Length - 1), out v)) 
                            {
                                if (!li[i].Value.Contains("/")) 
                                    break;
                            }
                        }
                        if (v > 400) 
                            break;
                        addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_HOUSE, li[i].Value, false, 0).Tag = li[i];
                        li[i].Tag = addr;
                        if (((i + 1) < li.Count) && ((li[i + 1].Typ == AddressItemType.Number || li[i + 1].Typ == AddressItemType.Flat)) && !li[i + 1].IsNewlineBefore) 
                        {
                            if (!int.TryParse(li[i + 1].Value, out v)) 
                                break;
                            if (v > 500) 
                                break;
                            i++;
                            if ((((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.Number && !li[i + 1].IsNewlineBefore) && (v < 5)) 
                            {
                                if (int.TryParse(li[i + 1].Value, out v)) 
                                {
                                    if (v < 500) 
                                    {
                                        addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_CORPUS, li[i].Value, false, 0).Tag = li[i];
                                        li[i].Tag = addr;
                                        i++;
                                    }
                                }
                            }
                            addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_FLAT, li[i].Value, false, 0).Tag = li[i];
                            li[i].Tag = addr;
                        }
                    }
                    else if (li[i].Typ == AddressItemType.City) 
                    {
                        if (geos == null) 
                            geos = new List<Pullenti.Ner.Geo.GeoReferent>();
                        if (li[i].IsNewlineBefore) 
                        {
                            if (geos.Count > 0) 
                            {
                                if ((i > 0 && li[i - 1].Typ != AddressItemType.City && li[i - 1].Typ != AddressItemType.Region) && li[i - 1].Typ != AddressItemType.Zip && li[i - 1].Typ != AddressItemType.Prefix) 
                                    break;
                            }
                            if (((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.Street && i > i0) 
                                break;
                        }
                        if (li[i].DetailType == Pullenti.Ner.Address.AddressDetailType.Near && li[i].DetailMeters == 0) 
                        {
                            nearCity = li[i];
                            li[i].Tag = addr;
                            continue;
                        }
                        int ii;
                        for (ii = 0; ii < geos.Count; ii++) 
                        {
                            if (geos[ii].IsCity) 
                                break;
                        }
                        if (ii >= geos.Count) 
                            geos.Add(li[i].Referent as Pullenti.Ner.Geo.GeoReferent);
                        else if (i > 0 && li[i].IsNewlineBefore && i > i0) 
                        {
                            int jj;
                            for (jj = 0; jj < i; jj++) 
                            {
                                if ((li[jj].Typ != AddressItemType.Prefix && li[jj].Typ != AddressItemType.Zip && li[jj].Typ != AddressItemType.Region) && li[jj].Typ != AddressItemType.Country && li[jj].Typ != AddressItemType.City) 
                                    break;
                            }
                            if (jj < i) 
                                break;
                        }
                        if (li[i].DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && detTyp == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                        {
                            detTyp = li[i].DetailType;
                            if (li[i].DetailMeters > 0) 
                                detParam = li[i].DetailMeters;
                        }
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.PostOfficeBox) 
                    {
                        if (addr.PostOfficeBox != null) 
                            break;
                        addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_POSTOFFICEBOX, li[i].Value ?? "", false, 0).Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.CSP) 
                    {
                        if (addr.CSP != null) 
                            break;
                        addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_CSP, li[i].Value, false, 0).Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Street) 
                    {
                        if (streets.Count > 1) 
                            break;
                        if (streets.Count > 0) 
                        {
                            if (li[i].IsNewlineBefore) 
                                break;
                            if (Pullenti.Ner.Core.MiscHelper.CanBeStartOfSentence(li[i].BeginToken)) 
                                break;
                        }
                        if (li[i].RefToken == null && i > 0 && li[i - 1].Typ != AddressItemType.Street) 
                            break;
                        streets.Add(li[i]);
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Detail) 
                    {
                        if ((i + 1) == li.Count && li[i].DetailType == Pullenti.Ner.Address.AddressDetailType.Near) 
                            break;
                        if (li[i].DetailType == Pullenti.Ner.Address.AddressDetailType.Near && ((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.City) 
                        {
                            nearCity = li[i + 1];
                            li[i].Tag = addr;
                            i++;
                        }
                        else if (li[i].DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && detTyp == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                        {
                            detTyp = li[i].DetailType;
                            if (li[i].DetailMeters > 0) 
                                detParam = li[i].DetailMeters;
                        }
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.BusinessCenter && li[i].RefToken != null) 
                    {
                        addr.AddExtReferent(li[i].RefToken);
                        addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_MISC, li[i].RefToken.Referent, false, 0).Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (i > i0) 
                        break;
                }
            }
            List<string> typs = new List<string>();
            foreach (Pullenti.Ner.Slot s in addr.Slots) 
            {
                if (!typs.Contains(s.TypeName)) 
                    typs.Add(s.TypeName);
            }
            if (streets.Count == 1 && !streets[0].IsDoubt && streets[0].RefToken == null) 
            {
            }
            else if (li.Count > 2 && li[0].Typ == AddressItemType.Zip && ((li[1].Typ == AddressItemType.Country || li[1].Typ == AddressItemType.Region))) 
            {
            }
            else if ((typs.Count + streets.Count) < 2) 
            {
                if (typs.Count > 0) 
                {
                    if ((((typs[0] != Pullenti.Ner.Address.AddressReferent.ATTR_STREET && typs[0] != Pullenti.Ner.Address.AddressReferent.ATTR_POSTOFFICEBOX && metro == null) && typs[0] != Pullenti.Ner.Address.AddressReferent.ATTR_HOUSE && typs[0] != Pullenti.Ner.Address.AddressReferent.ATTR_CORPUS) && typs[0] != Pullenti.Ner.Address.AddressReferent.ATTR_BUILDING && typs[0] != Pullenti.Ner.Address.AddressReferent.ATTR_PLOT) && typs[0] != Pullenti.Ner.Address.AddressReferent.ATTR_DETAIL && detTyp == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                        return null;
                }
                else if (streets.Count == 0 && detTyp == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                {
                    if (li[i - 1].Typ == AddressItemType.City && i > 2 && li[i - 2].Typ == AddressItemType.Zip) 
                    {
                    }
                    else 
                        return null;
                }
                else if ((i == li.Count && streets.Count == 1 && (streets[0].Referent is Pullenti.Ner.Address.StreetReferent)) && streets[0].Referent.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_TYP, "квартал", true) != null) 
                    return null;
                if (geos == null) 
                {
                    bool hasGeo = false;
                    for (Pullenti.Ner.Token tt = li[0].BeginToken.Previous; tt != null; tt = tt.Previous) 
                    {
                        if (tt.Morph.Class.IsPreposition || tt.IsComma) 
                            continue;
                        Pullenti.Ner.Referent r = tt.GetReferent();
                        if (r == null) 
                            break;
                        if (r.TypeName == "DATE" || r.TypeName == "DATERANGE") 
                            continue;
                        if (r is Pullenti.Ner.Geo.GeoReferent) 
                        {
                            if (!(r as Pullenti.Ner.Geo.GeoReferent).IsState) 
                            {
                                if (geos == null) 
                                    geos = new List<Pullenti.Ner.Geo.GeoReferent>();
                                geos.Add(r as Pullenti.Ner.Geo.GeoReferent);
                                hasGeo = true;
                            }
                        }
                        break;
                    }
                    if (!hasGeo) 
                        return null;
                }
            }
            for (i = 0; i < li.Count; i++) 
            {
                if (li[i].Typ == AddressItemType.Prefix) 
                    li[i].Tag = addr;
                else if (li[i].Tag == null) 
                {
                    if (li[i].IsNewlineBefore && i > i0) 
                    {
                        bool stop = false;
                        for (j = i + 1; j < li.Count; j++) 
                        {
                            if (li[j].Typ == AddressItemType.Street) 
                            {
                                stop = true;
                                break;
                            }
                        }
                        if (stop) 
                            break;
                    }
                    if (li[i].Typ == AddressItemType.Country || li[i].Typ == AddressItemType.Region || li[i].Typ == AddressItemType.City) 
                    {
                        if (geos == null) 
                            geos = new List<Pullenti.Ner.Geo.GeoReferent>();
                        if (!geos.Contains(li[i].Referent as Pullenti.Ner.Geo.GeoReferent)) 
                            geos.Add(li[i].Referent as Pullenti.Ner.Geo.GeoReferent);
                        if (li[i].Typ != AddressItemType.Country) 
                        {
                            if (li[i].DetailType != Pullenti.Ner.Address.AddressDetailType.Undefined && addr.Detail == Pullenti.Ner.Address.AddressDetailType.Undefined) 
                            {
                                addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_DETAIL, li[i].DetailType.ToString().ToUpper(), false, 0).Tag = li[i];
                                if (li[i].DetailMeters > 0) 
                                    addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_DETAILPARAM, string.Format("{0}м", li[i].DetailMeters), false, 0);
                            }
                        }
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Zip) 
                    {
                        if (addr.Zip != null) 
                            break;
                        addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_ZIP, li[i].Value, false, 0).Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.PostOfficeBox) 
                    {
                        if (addr.PostOfficeBox != null) 
                            break;
                        addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_POSTOFFICEBOX, li[i].Value, false, 0).Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.CSP) 
                    {
                        if (addr.CSP != null) 
                            break;
                        addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_CSP, li[i].Value, false, 0).Tag = li[i];
                        li[i].Tag = addr;
                    }
                    else if (li[i].Typ == AddressItemType.Number && li[i].IsDigit && li[i].Value.Length == 6) 
                    {
                        if (((i + 1) < li.Count) && li[i + 1].Typ == AddressItemType.City) 
                        {
                            if (addr.Zip != null) 
                                break;
                            addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_ZIP, li[i].Value, false, 0).Tag = li[i];
                            li[i].Tag = addr;
                        }
                    }
                    else 
                        break;
                }
            }
            Pullenti.Ner.Token t0 = null;
            Pullenti.Ner.Token t1 = null;
            for (i = 0; i < li.Count; i++) 
            {
                if (li[i].Tag != null) 
                {
                    t0 = li[i].BeginToken;
                    break;
                }
            }
            for (i = li.Count - 1; i >= 0; i--) 
            {
                if (li[i].Tag != null) 
                {
                    t1 = li[i].EndToken;
                    break;
                }
            }
            if (t0 == null || t1 == null) 
                return null;
            if (addr.Slots.Count == 0) 
            {
                int pureStreets = 0;
                int gsks = 0;
                foreach (AddressItemToken s in streets) 
                {
                    if (s.RefToken != null && s.RefTokenIsGsk) 
                        gsks++;
                    else if (s.RefToken == null) 
                        pureStreets++;
                }
                if ((pureStreets + gsks) == 0 && streets.Count > 0) 
                {
                    if (((detTyp != Pullenti.Ner.Address.AddressDetailType.Undefined || nearCity != null)) && geos != null) 
                    {
                    }
                    else 
                        addr = null;
                }
                else if (streets.Count < 2) 
                {
                    if ((streets.Count == 1 && geos != null && geos.Count > 0) && ((streets[0].RefToken == null || streets[0].RefTokenIsGsk))) 
                    {
                    }
                    else if (detTyp != Pullenti.Ner.Address.AddressDetailType.Undefined && geos != null && streets.Count == 0) 
                    {
                    }
                    else 
                        addr = null;
                }
            }
            if (addr != null && detTyp != Pullenti.Ner.Address.AddressDetailType.Undefined) 
            {
                addr.Detail = detTyp;
                if (detParam > 0) 
                    addr.AddSlot(Pullenti.Ner.Address.AddressReferent.ATTR_DETAILPARAM, string.Format("{0}м", detParam), false, 0);
            }
            if (geos == null && streets.Count > 0 && !streets[0].IsStreetRoad) 
            {
                int cou = 0;
                for (Pullenti.Ner.Token tt = t0.Previous; tt != null && (cou < 200); tt = tt.Previous,cou++) 
                {
                    if (tt.IsNewlineAfter) 
                        cou += 10;
                    Pullenti.Ner.Referent r = tt.GetReferent();
                    if ((r is Pullenti.Ner.Geo.GeoReferent) && !(r as Pullenti.Ner.Geo.GeoReferent).IsState) 
                    {
                        geos = new List<Pullenti.Ner.Geo.GeoReferent>();
                        geos.Add(r as Pullenti.Ner.Geo.GeoReferent);
                        break;
                    }
                    if (r is Pullenti.Ner.Address.StreetReferent) 
                    {
                        List<Pullenti.Ner.Geo.GeoReferent> ggg = (r as Pullenti.Ner.Address.StreetReferent).Geos;
                        if (ggg.Count > 0) 
                        {
                            geos = new List<Pullenti.Ner.Geo.GeoReferent>(ggg);
                            break;
                        }
                    }
                    if (r is Pullenti.Ner.Address.AddressReferent) 
                    {
                        List<Pullenti.Ner.Geo.GeoReferent> ggg = (r as Pullenti.Ner.Address.AddressReferent).Geos;
                        if (ggg.Count > 0) 
                        {
                            geos = new List<Pullenti.Ner.Geo.GeoReferent>(ggg);
                            break;
                        }
                    }
                }
            }
            Pullenti.Ner.ReferentToken terrRef = null;
            Pullenti.Ner.ReferentToken terRef0 = null;
            Pullenti.Ner.ReferentToken rt;
            Pullenti.Ner.Address.StreetReferent sr0 = null;
            for (int ii = 0; ii < streets.Count; ii++) 
            {
                AddressItemToken s = streets[ii];
                Pullenti.Ner.Address.StreetReferent sr = s.Referent as Pullenti.Ner.Address.StreetReferent;
                if ((sr == null && s.Referent != null && s.Referent.TypeName == "ORGANIZATION") && s.RefToken != null) 
                {
                    if (s.RefTokenIsGsk && addr == null) 
                        addr = new Pullenti.Ner.Address.AddressReferent();
                    if (addr != null) 
                    {
                        addr.AddReferent(s.Referent);
                        addr.AddExtReferent(s.RefToken);
                        terRef0 = s.RefToken;
                        if (geos == null || geos.Count == 0) 
                            continue;
                        int jj = li.IndexOf(s);
                        Pullenti.Ner.Token bt0 = s.RefToken.BeginToken;
                        Pullenti.Ner.Geo.GeoReferent geo0 = null;
                        if (jj > 0 && (li[jj - 1].Referent is Pullenti.Ner.Geo.GeoReferent) && ((li[jj - 1] != nearCity || (li[jj - 1].Referent as Pullenti.Ner.Geo.GeoReferent).Higher != null))) 
                        {
                            geo0 = li[jj - 1].Referent as Pullenti.Ner.Geo.GeoReferent;
                            bt0 = li[jj - 1].BeginToken;
                        }
                        else if (jj > 1 && (li[jj - 2].Referent is Pullenti.Ner.Geo.GeoReferent)) 
                        {
                            geo0 = li[jj - 2].Referent as Pullenti.Ner.Geo.GeoReferent;
                            bt0 = li[jj - 2].BeginToken;
                        }
                        else if (nearCity != null) 
                            geo0 = nearCity.Referent as Pullenti.Ner.Geo.GeoReferent;
                        if (geo0 != null && ((geo0.IsRegion || geo0.IsCity))) 
                        {
                            Pullenti.Ner.Geo.GeoReferent geo = new Pullenti.Ner.Geo.GeoReferent();
                            geo.AddTypTer(t0.Kit.BaseLanguage);
                            if (geo0.IsRegion) 
                                geo.AddTyp((t0.Kit.BaseLanguage.IsUa ? "населений пункт" : "населенный пункт"));
                            geo.AddOrgReferent(s.Referent);
                            if (s.RefToken != null) 
                                geo.AddExtReferent(s.RefToken);
                            if (nearCity != null && geo0 == nearCity.Referent) 
                                geo.Higher = geo0.Higher;
                            else 
                                geo.Higher = geo0;
                            Pullenti.Ner.Slot sl = addr.FindSlot(Pullenti.Ner.Address.AddressReferent.ATTR_GEO, geo0, true);
                            if (sl != null) 
                                addr.Slots.Remove(sl);
                            if ((((sl = addr.FindSlot(Pullenti.Ner.Address.AddressReferent.ATTR_STREET, s.Referent, true)))) != null) 
                                addr.Slots.Remove(sl);
                            geos.Remove(geo0);
                            if (nearCity != null && geos.Contains(nearCity.Referent as Pullenti.Ner.Geo.GeoReferent)) 
                                geos.Remove(nearCity.Referent as Pullenti.Ner.Geo.GeoReferent);
                            geos.Add(geo);
                            streets.RemoveAt(ii);
                            Pullenti.Ner.ReferentToken rtt = new Pullenti.Ner.ReferentToken(geo, bt0, s.RefToken.EndToken);
                            rtt.Data = t0.Kit.GetAnalyzerDataByAnalyzerName("GEO");
                            if (nearCity != null && (nearCity.Referent is Pullenti.Ner.Geo.GeoReferent)) 
                            {
                                geo.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_REF, nearCity.Referent, false, 0);
                                if (nearCity.EndChar > rtt.EndChar) 
                                    rtt.EndToken = nearCity.EndToken;
                                if (nearCity.BeginChar < rtt.BeginChar) 
                                    rtt.BeginToken = nearCity.BeginToken;
                                if ((nearCity.Referent as Pullenti.Ner.Geo.GeoReferent).Higher == null && geo0 != nearCity.Referent) 
                                    (nearCity.Referent as Pullenti.Ner.Geo.GeoReferent).Higher = geo0;
                            }
                            addr.AddExtReferent(rtt);
                            terrRef = rtt;
                            ii--;
                            continue;
                        }
                        if ((geo0 != null && geo0.IsTerritory && jj > 0) && li[jj - 1].Referent == geo0) 
                        {
                            geo0.AddSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_REF, s.Referent, false, 0);
                            geo0.AddExtReferent(s.RefToken);
                            Pullenti.Ner.ReferentToken rtt = new Pullenti.Ner.ReferentToken(geo0, li[jj - 1].BeginToken, s.RefToken.EndToken);
                            rtt.Data = t0.Kit.GetAnalyzerDataByAnalyzerName("GEO");
                            addr.AddExtReferent(rtt);
                            terrRef = rtt;
                            streets.RemoveAt(ii);
                            ii--;
                            continue;
                        }
                        foreach (Pullenti.Ner.Geo.GeoReferent gr in geos) 
                        {
                            if (s.Referent.FindSlot("GEO", gr, true) != null) 
                            {
                                geos.Remove(gr);
                                Pullenti.Ner.Slot sl = addr.FindSlot(Pullenti.Ner.Address.AddressReferent.ATTR_GEO, gr, true);
                                if (sl != null) 
                                    addr.Slots.Remove(sl);
                                break;
                            }
                        }
                    }
                    continue;
                }
                if (sr != null && terrRef != null) 
                {
                    sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_GEO, terrRef.Referent, false, 0);
                    sr.AddExtReferent(terrRef);
                    if (geos != null && geos.Contains(terrRef.Referent as Pullenti.Ner.Geo.GeoReferent)) 
                        geos.Remove(terrRef.Referent as Pullenti.Ner.Geo.GeoReferent);
                }
                if (geos != null && sr != null && ((sr.Geos.Count == 0 || ((sr.Geos.Count == 1 && sr.Geos[0].IsTerritory))))) 
                {
                    foreach (Pullenti.Ner.Geo.GeoReferent gr in geos) 
                    {
                        if (gr.IsCity || ((gr.Higher != null && gr.Higher.IsCity)) || gr.IsTerritory) 
                        {
                            sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_GEO, gr, true, 0);
                            if (li[0].Referent == gr) 
                                streets[0].BeginToken = li[0].BeginToken;
                            for (int jj = ii + 1; jj < streets.Count; jj++) 
                            {
                                if (streets[jj].Referent is Pullenti.Ner.Address.StreetReferent) 
                                    streets[jj].Referent.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_GEO, gr, false, 0);
                            }
                            geos.Remove(gr);
                            break;
                        }
                        else if (gr.IsRegion) 
                        {
                            bool ok = false;
                            if (sr.Kind == Pullenti.Ner.Address.StreetKind.Railway || sr.Kind == Pullenti.Ner.Address.StreetKind.Road || sr.Kind == Pullenti.Ner.Address.StreetKind.Area) 
                                ok = true;
                            else 
                                foreach (string v in gr.Typs) 
                                {
                                    if (v == "муниципальный округ" || v == "городской округ") 
                                        ok = true;
                                }
                            if (ok) 
                            {
                                if (li[0].Referent == gr) 
                                    streets[0].BeginToken = li[0].BeginToken;
                                sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_GEO, gr, true, 0);
                                geos.Remove(gr);
                                break;
                            }
                        }
                    }
                }
                if (sr != null && sr.Geos.Count == 0) 
                {
                    if (sr0 != null) 
                    {
                        foreach (Pullenti.Ner.Geo.GeoReferent g in sr0.Geos) 
                        {
                            sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_GEO, g, false, 0);
                        }
                    }
                    sr0 = sr;
                }
                if (s.Referent != null && s.Referent.FindSlot(Pullenti.Ner.Address.StreetReferent.ATTR_NAME, "НЕТ", true) != null) 
                {
                    foreach (Pullenti.Ner.Slot ss in s.Referent.Slots) 
                    {
                        if (ss.TypeName == Pullenti.Ner.Address.StreetReferent.ATTR_GEO) 
                            addr.AddReferent(ss.Value as Pullenti.Ner.Referent);
                    }
                }
                else 
                {
                    if (sr != null && ii > 0 && (streets[ii - 1].Referent is Pullenti.Ner.Address.StreetReferent)) 
                    {
                        Pullenti.Ner.Address.StreetKind ki = (streets[ii - 1].Referent as Pullenti.Ner.Address.StreetReferent).Kind;
                        if (ki != sr.Kind || ki == Pullenti.Ner.Address.StreetKind.Area) 
                        {
                            if (ki == Pullenti.Ner.Address.StreetKind.Area || ki == Pullenti.Ner.Address.StreetKind.Railway || ki == Pullenti.Ner.Address.StreetKind.Road) 
                            {
                                sr.Higher = streets[ii - 1].Referent as Pullenti.Ner.Address.StreetReferent;
                                sr.AddSlot(Pullenti.Ner.Address.StreetReferent.ATTR_GEO, null, true, 0);
                                if (addr != null) 
                                {
                                    Pullenti.Ner.Slot slo = addr.FindSlot(Pullenti.Ner.Address.AddressReferent.ATTR_STREET, null, true);
                                    if (slo != null) 
                                        addr.Slots.Remove(slo);
                                }
                                s.BeginToken = t0;
                            }
                        }
                    }
                    if (addr != null) 
                        addr.MoveExtReferent(s.Referent, null);
                    s.Referent = ad.RegisterReferent(s.Referent);
                    if (addr != null) 
                        addr.AddReferent(s.Referent);
                    for (Pullenti.Ner.Token tt = s.BeginToken.Previous; tt != null && tt.BeginChar >= t0.BeginChar; tt = tt.Previous) 
                    {
                        Pullenti.Ner.Geo.GeoReferent g = tt.GetReferent() as Pullenti.Ner.Geo.GeoReferent;
                        if (g == null || sr == null) 
                            continue;
                        foreach (Pullenti.Ner.Geo.GeoReferent gg in sr.Geos) 
                        {
                            if (gg.TopHigher == g.TopHigher) 
                                s.BeginToken = tt;
                        }
                    }
                    t = (rt = new Pullenti.Ner.ReferentToken(s.Referent, s.BeginToken, s.EndToken));
                    t.Kit.EmbedToken(rt);
                    if (s.BeginChar == t0.BeginChar) 
                        t0 = rt;
                    if (s.EndChar == t1.EndChar) 
                        t1 = rt;
                }
            }
            if (addr != null) 
            {
                bool ok = false;
                foreach (Pullenti.Ner.Slot s in addr.Slots) 
                {
                    if (s.TypeName != Pullenti.Ner.Address.AddressReferent.ATTR_DETAIL) 
                        ok = true;
                }
                if (!ok) 
                    addr = null;
            }
            if (addr == null) 
            {
                if (terrRef != null) 
                {
                    terrRef.Referent.AddExtReferent(terRef0);
                    terrRef.Referent = ad.RegisterReferent(terrRef.Referent);
                    t.Kit.EmbedToken(terrRef);
                    t = terrRef;
                }
                return t;
            }
            if (geos != null && geos.Count > 0) 
            {
                if ((geos.Count == 1 && geos[0].IsRegion && streets.Count == 1) && streets[0].RefToken != null) 
                {
                }
                if (streets.Count == 1 && streets[0].Referent != null) 
                {
                    foreach (Pullenti.Ner.Slot s in streets[0].Referent.Slots) 
                    {
                        if (s.TypeName == Pullenti.Ner.Address.StreetReferent.ATTR_GEO && (s.Value is Pullenti.Ner.Geo.GeoReferent)) 
                        {
                            int k = 0;
                            for (Pullenti.Ner.Geo.GeoReferent gg = s.Value as Pullenti.Ner.Geo.GeoReferent; gg != null && (k < 5); gg = gg.ParentReferent as Pullenti.Ner.Geo.GeoReferent,k++) 
                            {
                                for (int ii = geos.Count - 1; ii >= 0; ii--) 
                                {
                                    if (geos[ii] == gg) 
                                    {
                                        geos.RemoveAt(ii);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                while (geos.Count >= 2) 
                {
                    if (geos[1].Higher == null && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(geos[0], geos[1])) 
                    {
                        geos[1].Higher = geos[0];
                        geos.RemoveAt(0);
                    }
                    else if (geos[0].Higher == null && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(geos[1], geos[0])) 
                    {
                        geos[0].Higher = geos[1];
                        geos.RemoveAt(1);
                    }
                    else if (geos[1].Higher != null && geos[1].Higher.Higher == null && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(geos[0], geos[1].Higher)) 
                    {
                        geos[1].Higher.Higher = geos[0];
                        geos.RemoveAt(0);
                    }
                    else if (geos[0].Higher != null && geos[0].Higher.Higher == null && Pullenti.Ner.Geo.Internal.GeoOwnerHelper.CanBeHigher(geos[1], geos[0].Higher)) 
                    {
                        geos[0].Higher.Higher = geos[1];
                        geos.RemoveAt(1);
                    }
                    else 
                        break;
                }
                foreach (Pullenti.Ner.Geo.GeoReferent g in geos) 
                {
                    addr.AddReferent(g);
                }
            }
            bool ok1 = false;
            foreach (Pullenti.Ner.Slot s in addr.Slots) 
            {
                if (s.TypeName != Pullenti.Ner.Address.AddressReferent.ATTR_STREET) 
                {
                    ok1 = true;
                    break;
                }
            }
            if (!ok1) 
                return t;
            if (addr.House != null && addr.Corpus == null && addr.FindSlot(Pullenti.Ner.Address.AddressReferent.ATTR_STREET, null, true) == null) 
            {
                if (geos != null && geos.Count > 0 && geos[0].FindSlot(Pullenti.Ner.Geo.GeoReferent.ATTR_NAME, "ЗЕЛЕНОГРАД", true) != null) 
                {
                    addr.Corpus = addr.House;
                    addr.House = null;
                }
            }
            rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(addr), t0, t1);
            t.Kit.EmbedToken(rt);
            t = rt;
            if ((t.Next != null && ((t.Next.IsComma || t.Next.IsChar(';'))) && (t.Next.WhitespacesAfterCount < 2)) && (t.Next.Next is Pullenti.Ner.NumberToken)) 
            {
                AddressItemToken last = null;
                foreach (AddressItemToken ll in li) 
                {
                    if (ll.Tag != null) 
                        last = ll;
                }
                string attrName = null;
                if (last == null) 
                    return t;
                if (last.Typ == AddressItemType.House) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_HOUSE;
                else if (last.Typ == AddressItemType.Corpus) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_CORPUS;
                else if (last.Typ == AddressItemType.Building) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_BUILDING;
                else if (last.Typ == AddressItemType.Flat) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_FLAT;
                else if (last.Typ == AddressItemType.Plot) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_PLOT;
                else if (last.Typ == AddressItemType.Field) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_FIELD;
                else if (last.Typ == AddressItemType.Box) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_BOX;
                else if (last.Typ == AddressItemType.Potch) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_PORCH;
                else if (last.Typ == AddressItemType.Block) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_BLOCK;
                else if (last.Typ == AddressItemType.Office) 
                    attrName = Pullenti.Ner.Address.AddressReferent.ATTR_OFFICE;
                if (attrName != null) 
                {
                    for (t = t.Next.Next; t != null; t = t.Next) 
                    {
                        if (!(t is Pullenti.Ner.NumberToken)) 
                            break;
                        Pullenti.Ner.Address.AddressReferent addr1 = addr.Clone() as Pullenti.Ner.Address.AddressReferent;
                        addr1.Occurrence.Clear();
                        addr1.AddSlot(attrName, (t as Pullenti.Ner.NumberToken).Value.ToString(), true, 0);
                        rt = new Pullenti.Ner.ReferentToken(ad.RegisterReferent(addr1), t, t);
                        t.Kit.EmbedToken(rt);
                        t = rt;
                        if ((t.Next != null && ((t.Next.IsComma || t.Next.IsChar(';'))) && (t.Next.WhitespacesAfterCount < 2)) && (t.Next.Next is Pullenti.Ner.NumberToken)) 
                        {
                        }
                        else 
                            break;
                    }
                }
            }
            return t;
        }
    }
}