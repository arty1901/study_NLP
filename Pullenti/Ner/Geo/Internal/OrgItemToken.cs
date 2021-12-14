/*
 * SDK Pullenti Lingvo, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Ner.Geo.Internal
{
    public class OrgItemToken : Pullenti.Ner.ReferentToken
    {
        public OrgItemToken(Pullenti.Ner.Referent r, Pullenti.Ner.Token b, Pullenti.Ner.Token e) : base(r, b, e, null)
        {
        }
        public bool HasTerrKeyword;
        public bool IsGsk
        {
            get
            {
                foreach (Pullenti.Ner.Slot s in Referent.Slots) 
                {
                    if (s.TypeName == "TYPE" && (s.Value is string)) 
                    {
                        string ty = s.Value as string;
                        if ((((ty.Contains("товарищество") || ty.Contains("кооператив") || ty.Contains("коллектив")) || ty.Contains("партнерство") || ty.Contains("объединение")) || ty.Contains("бизнес") || ty.Contains("станция")) || ty.Contains("аэропорт")) 
                            return true;
                    }
                    else if (s.TypeName == "NAME" && (s.Value is string)) 
                    {
                        string nam = s.Value as string;
                        if (nam.EndsWith("ГЭС") || nam.EndsWith("АЭС") || nam.EndsWith("ТЭС")) 
                            return true;
                    }
                }
                return false;
            }
        }
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            if (HasTerrKeyword) 
                tmp.Append("Terr ");
            if (IsGsk) 
                tmp.Append("Gsk ");
            tmp.Append(Referent.ToString());
            return tmp.ToString();
        }
        public static bool SpeedRegime = false;
        internal static void PrepareAllData(Pullenti.Ner.Token t0)
        {
            if (!SpeedRegime) 
                return;
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t0);
            if (ad == null) 
                return;
            for (Pullenti.Ner.Token t = t0; t != null; t = t.Next) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                OrgItemToken org = TryParse(t);
                if (org != null) 
                {
                    if (d == null) 
                        d = new GeoTokenData(t);
                    d.Org = org;
                }
            }
        }
        public static OrgItemToken TryParse(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken)) 
                return null;
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad != null && SpeedRegime && ((ad.ORegime || ad.AllRegime))) 
            {
                GeoTokenData d = t.Tag as GeoTokenData;
                if (d != null) 
                    return d.Org;
                if (t.Tag != null) 
                    return null;
            }
            if (ad != null) 
            {
                if (ad.OLevel > 0) 
                    return null;
                ad.OLevel++;
            }
            OrgItemToken res = _TryParse(t, false, 0);
            if (ad != null) 
                ad.OLevel--;
            return res;
        }
        static OrgItemToken _TryParse(Pullenti.Ner.Token t, bool afterTerr, int lev)
        {
            if (lev > 3 || t == null) 
                return null;
            if ((t.IsValue("ТЕРРИТОРИЯ", "ТЕРИТОРІЯ") || t.IsValue("САД", null) || t.IsValue("ГРАНИЦА", null)) || ((t.IsValue("В", null) && t.Next != null && t.Next.IsValue("ГРАНИЦА", null)))) 
            {
                Pullenti.Ner.Token tt2 = t.Next;
                if (tt2 != null && ((tt2.IsValue("ВЛАДЕНИЕ", null) || tt2.IsValue("ГРАНИЦА", null)))) 
                    tt2 = tt2.Next;
                bool br = false;
                if (Pullenti.Ner.Core.BracketHelper.IsBracket(tt2, true)) 
                {
                    br = true;
                    tt2 = tt2.Next;
                }
                if (tt2 == null || lev > 3) 
                    return null;
                OrgItemToken re2 = _TryParse(tt2, true, lev + 1);
                if (re2 != null) 
                {
                    re2.BeginToken = t;
                    if (br && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(re2.EndToken.Next, false, null, false)) 
                        re2.EndToken = re2.EndToken.Next;
                    re2.HasTerrKeyword = true;
                    return re2;
                }
            }
            bool doubt = false;
            Pullenti.Ner.MetaToken tokTyp = _tryParseTyp(t, afterTerr, out doubt, 0);
            NameToken nam = null;
            if (tokTyp == null) 
            {
                bool ok = false;
                if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(t, true, false)) 
                    ok = true;
                else if ((t is Pullenti.Ner.TextToken) && !t.Chars.IsAllLower && t.LengthChar > 1) 
                    ok = true;
                if (!ok) 
                    return null;
                if ((t.LengthChar > 5 && !t.Chars.IsAllUpper && !t.Chars.IsAllLower) && !t.Chars.IsCapitalUpper) 
                {
                    string namm = (t as Pullenti.Ner.TextToken).GetSourceText();
                    if (char.IsUpper(namm[0]) && char.IsUpper(namm[1])) 
                    {
                        for (int i = 0; i < namm.Length; i++) 
                        {
                            if (char.IsLower(namm[i]) && i > 2) 
                            {
                                string abbr = namm.Substring(0, i - 1);
                                Pullenti.Ner.Core.Termin te = new Pullenti.Ner.Core.Termin(abbr) { Acronym = abbr };
                                List<Pullenti.Ner.Core.Termin> li = m_OrgOntology.FindTerminsByTermin(te);
                                if (li != null && li.Count > 0) 
                                {
                                    nam = new NameToken(t, t);
                                    nam.Name = (t as Pullenti.Ner.TextToken).Term.Substring(i - 1);
                                    tokTyp = new Pullenti.Ner.MetaToken(t, t) { Tag = new List<string>() };
                                    (tokTyp.Tag as List<string>).Add(li[0].CanonicText.ToLower());
                                    (tokTyp.Tag as List<string>).Add(abbr);
                                    nam.TryAttachNumber();
                                    break;
                                }
                            }
                        }
                    }
                }
                if (nam == null) 
                {
                    nam = NameToken.TryParse(t, NameTokenType.Org, 0);
                    if (nam == null || nam.Name == null) 
                        return null;
                    tokTyp = _tryParseTyp(nam.EndToken.Next, afterTerr, out doubt, 0);
                    if (tokTyp != null) 
                    {
                        if (nam.BeginToken == nam.EndToken) 
                        {
                            Pullenti.Morph.MorphClass mc = nam.GetMorphClassInDictionary();
                            if (mc.IsConjunction || mc.IsPreposition || mc.IsPronoun) 
                                return null;
                        }
                        nam.EndToken = tokTyp.EndToken;
                    }
                    else 
                    {
                        bool dou = false;
                        if (nam.Name.EndsWith("ПЛАЗА") || nam.Name.StartsWith("БИЗНЕС")) 
                        {
                        }
                        else if (nam.BeginToken == nam.EndToken) 
                            return null;
                        else if ((((tokTyp = _tryParseTyp(nam.EndToken, false, out dou, lev + 1)))) == null) 
                            return null;
                        else if (nam.Morph.Case.IsGenitive && !nam.Morph.Case.IsNominative) 
                            nam.Name = Pullenti.Ner.Core.MiscHelper.GetTextValueOfMetaToken(nam, Pullenti.Ner.Core.GetTextAttr.FirstNounGroupToNominativeSingle).Replace("-", " ");
                        if (tokTyp == null) 
                        {
                            tokTyp = new Pullenti.Ner.MetaToken(t, t);
                            tokTyp.Tag = new List<string>();
                            (tokTyp.Tag as List<string>).Add("бизнес центр");
                            (tokTyp.Tag as List<string>).Add("БЦ");
                        }
                        nam.IsDoubt = false;
                    }
                }
            }
            else 
            {
                if (tokTyp.WhitespacesAfterCount > 3) 
                    return null;
                if (Pullenti.Ner.Core.BracketHelper.CanBeStartOfSequence(tokTyp.EndToken.Next, true, false)) 
                {
                    bool doubt2 = false;
                    Pullenti.Ner.MetaToken tokTyp2 = _tryParseTyp(tokTyp.EndToken.Next.Next, afterTerr, out doubt2, 0);
                    if (tokTyp2 != null && !doubt2) 
                    {
                        doubt = false;
                        nam = NameToken.TryParse(tokTyp2.EndToken.Next, NameTokenType.Org, 0);
                        if (nam != null && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(nam.EndToken.Next, false, null, false)) 
                        {
                            (tokTyp.Tag as List<string>).AddRange(tokTyp2.Tag as List<string>);
                            nam.EndToken = nam.EndToken.Next;
                        }
                        else if (nam != null && Pullenti.Ner.Core.BracketHelper.CanBeEndOfSequence(nam.EndToken, false, null, false)) 
                            (tokTyp.Tag as List<string>).AddRange(tokTyp2.Tag as List<string>);
                        else 
                            nam = null;
                    }
                }
            }
            if (nam == null) 
                nam = NameToken.TryParse(tokTyp.EndToken.Next, NameTokenType.Org, 0);
            if (nam == null) 
                return null;
            if (doubt && nam.IsDoubt) 
                return null;
            if ((tokTyp.LengthChar < 3) && nam.Name == null && nam.Pref == null) 
                return null;
            Pullenti.Ner.Referent org = t.Kit.CreateReferent("ORGANIZATION");
            OrgItemToken res = new OrgItemToken(org, t, nam.EndToken);
            res.Data = t.Kit.GetAnalyzerDataByAnalyzerName("ORGANIZATION");
            res.HasTerrKeyword = afterTerr;
            foreach (string ty in tokTyp.Tag as List<string>) 
            {
                org.AddSlot("TYPE", ty, false, 0);
            }
            bool ignoreNext = false;
            if ((res.WhitespacesAfterCount < 3) && res.EndToken.Next != null && res.EndToken.Next.IsValue("ТЕРРИТОРИЯ", null)) 
            {
                if (_TryParse(res.EndToken.Next.Next, true, lev + 1) == null) 
                {
                    res.EndToken = res.EndToken.Next;
                    ignoreNext = true;
                }
            }
            if (res.WhitespacesAfterCount < 3) 
            {
                Pullenti.Ner.Token tt = res.EndToken.Next;
                OrgItemToken next = _TryParse(tt, false, lev + 1);
                if (next != null) 
                {
                    if (next.IsGsk) 
                        next = null;
                    else 
                        res.EndToken = next.EndToken;
                    ignoreNext = true;
                }
                else 
                {
                    if (tt != null && tt.IsValue("ПРИ", null)) 
                        tt = tt.Next;
                    Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("ORGANIZATION", tt, null);
                    if (rt != null) 
                    {
                        res.EndToken = rt.EndToken;
                        TerrItemToken ter = TerrItemToken.TryParse(res.EndToken.Next, null);
                        if (ter != null && ter.OntoItem != null) 
                            res.EndToken = ter.EndToken;
                        ignoreNext = true;
                    }
                }
            }
            string suffName = null;
            if (!ignoreNext && (res.WhitespacesAfterCount < 2)) 
            {
                bool doubt2 = false;
                Pullenti.Ner.MetaToken tokTyp2 = _tryParseTyp(res.EndToken.Next, true, out doubt2, 0);
                if (tokTyp2 != null) 
                {
                    res.EndToken = tokTyp2.EndToken;
                    if (doubt2 && nam.Name != null) 
                        suffName = (tokTyp2.Tag as List<string>)[0];
                    else 
                        foreach (string ty in tokTyp2.Tag as List<string>) 
                        {
                            org.AddSlot("TYPE", ty, false, 0);
                        }
                    if (nam.Number == null) 
                    {
                        NameToken nam2 = NameToken.TryParse(res.EndToken.Next, NameTokenType.Org, 0);
                        if ((nam2 != null && nam2.Number != null && nam2.Name == null) && nam2.Pref == null) 
                        {
                            nam.Number = nam2.Number;
                            res.EndToken = nam2.EndToken;
                        }
                    }
                }
            }
            if (nam.Name != null) 
            {
                if (nam.Pref != null) 
                {
                    org.AddSlot("NAME", string.Format("{0} {1}", nam.Pref, nam.Name), false, 0);
                    if (suffName != null) 
                        org.AddSlot("NAME", string.Format("{0} {1} {2}", nam.Pref, nam.Name, suffName), false, 0);
                }
                else 
                {
                    org.AddSlot("NAME", nam.Name, false, 0);
                    if (suffName != null) 
                        org.AddSlot("NAME", string.Format("{0} {1}", nam.Name, suffName), false, 0);
                }
            }
            else if (nam.Pref != null) 
                org.AddSlot("NAME", nam.Pref, false, 0);
            else if (nam.Number != null && (res.WhitespacesAfterCount < 2)) 
            {
                NameToken nam2 = NameToken.TryParse(res.EndToken.Next, NameTokenType.Org, 0);
                if (nam2 != null && nam2.Name != null && nam2.Number == null) 
                {
                    res.EndToken = nam2.EndToken;
                    org.AddSlot("NAME", nam2.Name, false, 0);
                }
            }
            if (nam.Number != null) 
                org.AddSlot("NUMBER", nam.Number, false, 0);
            return res;
        }
        static Pullenti.Ner.MetaToken _tryParseTyp(Pullenti.Ner.Token t, bool afterTerr, out bool doubt, int lev = 0)
        {
            doubt = false;
            if (t == null) 
                return null;
            if (t.IsValue("СП", null) && !afterTerr) 
                return null;
            if (t.IsValue("КП", null)) 
                return null;
            Pullenti.Ner.Token t1 = null;
            List<string> typs = null;
            Pullenti.Ner.Core.TerminToken tok = m_OrgOntology.TryParse(t, Pullenti.Ner.Core.TerminParseAttr.No);
            if (tok != null) 
            {
                t1 = tok.EndToken;
                typs = new List<string>();
                typs.Add(tok.Termin.CanonicText.ToLower());
                if (tok.Termin.Acronym != null) 
                    typs.Add(tok.Termin.Acronym);
            }
            else 
            {
                Pullenti.Ner.ReferentToken rtok = t.Kit.ProcessReferent("ORGANIZATION", t, "MINTYPE");
                if (rtok != null) 
                {
                    t1 = rtok.EndToken;
                    typs = rtok.Referent.GetStringValues("TYPE");
                }
            }
            if (((t1 == null && (t is Pullenti.Ner.TextToken) && t.LengthChar >= 2) && t.LengthChar <= 4 && t.Chars.IsAllUpper) && t.Chars.IsCyrillicLetter) 
            {
                if (Pullenti.Ner.Address.Internal.AddressItemToken.TryParsePureItem(t, null) != null) 
                    return null;
                typs = new List<string>();
                typs.Add((t as Pullenti.Ner.TextToken).Term);
                t1 = t;
                doubt = true;
            }
            if (t1 == null) 
                return null;
            Pullenti.Ner.MetaToken res = new Pullenti.Ner.MetaToken(t, t1) { Tag = typs };
            if ((lev < 2) && (res.WhitespacesAfterCount < 3)) 
            {
                bool doubt2 = false;
                Pullenti.Ner.MetaToken next = _tryParseTyp(res.EndToken.Next, true, out doubt2, lev + 1);
                if (next != null && !next.BeginToken.Chars.IsAllLower) 
                {
                    NameToken nam = NameToken.TryParse(next.EndToken.Next, NameTokenType.Org, 0);
                    if (nam == null || next.WhitespacesAfterCount > 3) 
                        next = null;
                    else if ((nam.Number != null && nam.Name == null && next.LengthChar > 2) && doubt2) 
                        next = null;
                }
                if (next != null) 
                {
                    if (!doubt2) 
                        doubt = false;
                    foreach (string ty in next.Tag as List<string>) 
                    {
                        if (!typs.Contains(ty)) 
                            typs.Add(ty);
                    }
                    res.EndToken = next.EndToken;
                }
            }
            return res;
        }
        public static Pullenti.Ner.Address.Internal.StreetItemToken TryParseRailway(Pullenti.Ner.Token t)
        {
            if (!(t is Pullenti.Ner.TextToken) || !t.Chars.IsLetter) 
                return null;
            if (t.IsValue("ДОРОГА", null) && (t.WhitespacesAfterCount < 3)) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken next = TryParseRailway(t.Next);
                if (next != null) 
                {
                    next.BeginToken = t;
                    return next;
                }
            }
            GeoAnalyzerData ad = Pullenti.Ner.Geo.GeoAnalyzer.GetData(t);
            if (ad == null) 
                return null;
            if (ad.OLevel > 0) 
                return null;
            ad.OLevel++;
            Pullenti.Ner.Address.Internal.StreetItemToken res = _tryParseRailway(t);
            ad.OLevel--;
            return res;
        }
        static Pullenti.Ner.ReferentToken _tryParseRailwayOrg(Pullenti.Ner.Token t)
        {
            if (t == null) 
                return null;
            int cou = 0;
            bool ok = false;
            for (Pullenti.Ner.Token tt = t; tt != null && (cou < 4); tt = tt.Next,cou++) 
            {
                if (tt is Pullenti.Ner.TextToken) 
                {
                    string val = (tt as Pullenti.Ner.TextToken).Term;
                    if (val == "Ж" || val.StartsWith("ЖЕЛЕЗ") || val.EndsWith("ЖД")) 
                        ok = true;
                }
            }
            if (!ok) 
                return null;
            Pullenti.Ner.ReferentToken rt = t.Kit.ProcessReferent("ORGANIZATION", t, null);
            if (rt == null) 
                return null;
            foreach (string ty in rt.Referent.GetStringValues("TYPE")) 
            {
                if (ty.EndsWith("дорога")) 
                    return rt;
            }
            return null;
        }
        static Pullenti.Ner.Address.Internal.StreetItemToken _tryParseRailway(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.ReferentToken rt0 = _tryParseRailwayOrg(t);
            if (rt0 != null) 
            {
                Pullenti.Ner.Address.Internal.StreetItemToken res = new Pullenti.Ner.Address.Internal.StreetItemToken(t, rt0.EndToken) { Typ = Pullenti.Ner.Address.Internal.StreetItemType.Fix, IsRailway = true };
                res.Value = rt0.Referent.GetStringValue("NAME");
                t = res.EndToken.Next;
                if (t != null && t.IsComma) 
                    t = t.Next;
                Pullenti.Ner.Address.Internal.StreetItemToken next = _tryParseRzdDir(t);
                if (next != null) 
                {
                    res.EndToken = next.EndToken;
                    res.Value = string.Format("{0} {1}", res.Value, next.Value);
                }
                else if ((t is Pullenti.Ner.TextToken) && t.Morph.Class.IsAdjective && !t.Chars.IsAllLower) 
                {
                    bool ok = false;
                    if (t.IsNewlineAfter || t.Next == null) 
                        ok = true;
                    else if (t.Next.IsCharOf(".,")) 
                        ok = true;
                    else if (Pullenti.Ner.Address.Internal.AddressItemToken.CheckHouseAfter(t.Next, false, false) || Pullenti.Ner.Address.Internal.AddressItemToken.CheckKmAfter(t.Next)) 
                        ok = true;
                    if (ok) 
                    {
                        res.Value = string.Format("{0} {1} НАПРАВЛЕНИЕ", res.Value, (t as Pullenti.Ner.TextToken).Term);
                        res.EndToken = t;
                    }
                }
                if (res.Value == "РОССИЙСКИЕ ЖЕЛЕЗНЫЕ ДОРОГИ") 
                    res.NounIsDoubtCoef = 2;
                return res;
            }
            Pullenti.Ner.Address.Internal.StreetItemToken dir = _tryParseRzdDir(t);
            if (dir != null && dir.NounIsDoubtCoef == 0) 
                return dir;
            return null;
        }
        static Pullenti.Ner.Address.Internal.StreetItemToken _tryParseRzdDir(Pullenti.Ner.Token t)
        {
            Pullenti.Ner.Token napr = null;
            Pullenti.Ner.Token tt0 = null;
            Pullenti.Ner.Token tt1 = null;
            string val = null;
            for (Pullenti.Ner.Token tt = t; tt != null; tt = tt.Next) 
            {
                if (tt.IsCharOf(",.")) 
                    continue;
                if (tt.IsNewlineBefore) 
                    break;
                if (tt.IsValue("НАПРАВЛЕНИЕ", null)) 
                {
                    napr = tt;
                    continue;
                }
                if (tt.IsValue("НАПР", null)) 
                {
                    if (tt.Next != null && tt.Next.IsChar('.')) 
                        tt = tt.Next;
                    napr = tt;
                    continue;
                }
                Pullenti.Ner.Core.NounPhraseToken npt = Pullenti.Ner.Core.NounPhraseHelper.TryParse(tt, Pullenti.Ner.Core.NounPhraseParseAttr.No, 0, null);
                if (npt != null && npt.Adjectives.Count > 0 && npt.Noun.IsValue("КОЛЬЦО", null)) 
                {
                    tt0 = tt;
                    tt1 = npt.EndToken;
                    val = npt.GetNormalCaseText(null, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Undefined, false);
                    break;
                }
                if ((tt is Pullenti.Ner.TextToken) && ((!tt.Chars.IsAllLower || napr != null)) && ((tt.Morph.Gender & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    tt0 = (tt1 = tt);
                    continue;
                }
                if ((((tt is Pullenti.Ner.TextToken) && ((!tt.Chars.IsAllLower || napr != null)) && tt.Next != null) && tt.Next.IsHiphen && (tt.Next.Next is Pullenti.Ner.TextToken)) && ((tt.Next.Next.Morph.Gender & Pullenti.Morph.MorphGender.Neuter)) != Pullenti.Morph.MorphGender.Undefined) 
                {
                    tt0 = tt;
                    tt = tt.Next.Next;
                    tt1 = tt;
                    continue;
                }
                break;
            }
            if (tt0 == null) 
                return null;
            Pullenti.Ner.Address.Internal.StreetItemToken res = new Pullenti.Ner.Address.Internal.StreetItemToken(tt0, tt1) { Typ = Pullenti.Ner.Address.Internal.StreetItemType.Fix, IsRailway = true, NounIsDoubtCoef = 1 };
            if (val != null) 
                res.Value = val;
            else 
            {
                res.Value = tt1.GetNormalCaseText(Pullenti.Morph.MorphClass.Adjective, Pullenti.Morph.MorphNumber.Singular, Pullenti.Morph.MorphGender.Neuter, false);
                if (tt0 != tt1) 
                    res.Value = string.Format("{0} {1}", (tt0 as Pullenti.Ner.TextToken).Term, res.Value);
                res.Value += " НАПРАВЛЕНИЕ";
            }
            if (napr != null && napr.EndChar > res.EndChar) 
                res.EndToken = napr;
            t = res.EndToken.Next;
            if (t != null && t.IsComma) 
                t = t.Next;
            if (t != null) 
            {
                Pullenti.Ner.ReferentToken rt0 = _tryParseRailwayOrg(t);
                if (rt0 != null) 
                {
                    res.Value = string.Format("{0} {1}", rt0.Referent.GetStringValue("NAME"), res.Value);
                    res.EndToken = rt0.EndToken;
                    res.NounIsDoubtCoef = 0;
                }
            }
            return res;
        }
        public static void Initialize()
        {
            NameToken.Initialize();
            m_OrgOntology = new Pullenti.Ner.Core.TerminCollection();
            Pullenti.Ner.Core.Termin t = new Pullenti.Ner.Core.Termin("САДОВОЕ ТОВАРИЩЕСТВО") { Acronym = "СТ" };
            t.AddVariant("САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            t.Acronym = "СТ";
            t.AddAbridge("С/ТОВ");
            t.AddAbridge("ПК СТ");
            t.AddAbridge("САД.ТОВ.");
            t.AddAbridge("САДОВ.ТОВ.");
            t.AddAbridge("С/Т");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ ТОВАРИЩЕСТВО");
            t.AddAbridge("Д/Т");
            t.AddAbridge("ДАЧ/Т");
            t.Acronym = "ДТ";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВЫЙ КООПЕРАТИВ");
            t.AddAbridge("С/К");
            t.Acronym = "СК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ");
            t.AddVariant("ПОТРЕБКООПЕРАТИВ", false);
            t.Acronym = "ПК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ДАЧНОЕ ТОВАРИЩЕСТВО");
            t.AddVariant("САДОВОЕ ДАЧНОЕ ТОВАРИЩЕСТВО", false);
            t.Acronym = "СДТ";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ");
            t.Acronym = "ДНО";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО");
            t.Acronym = "ДНП";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО");
            t.Acronym = "ДНТ";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНЫЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ");
            t.Acronym = "ДПК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ");
            t.AddVariant("ДАЧНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ", false);
            t.Acronym = "ДСК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ");
            t.Acronym = "СПК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО");
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            t.Acronym = "СНТ";
            t.AcronymCanBeLower = true;
            t.AddAbridge("САДОВОЕ НЕКОМ-Е ТОВАРИЩЕСТВО");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ") { Acronym = "СНО", AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО") { Acronym = "СНП", AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "СНТ", AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ОГОРОДНИЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "СОТ", AcronymCanBeLower = true };
            t.AddVariant("САДОВОЕ ОГОРОДНИЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДАЧНОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ДНТ", AcronymCanBeLower = true };
            t.AddVariant("ДАЧНО НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "НСТ", AcronymCanBeLower = true };
            t.AddVariant("НЕКОММЕРЧЕСКОЕ САДОВОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБЪЕДИНЕННОЕ НЕКОММЕРЧЕСКОЕ САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ОНСТ", AcronymCanBeLower = true };
            t.AddVariant("ОБЪЕДИНЕННОЕ НЕКОММЕРЧЕСКОЕ САДОВОЕ ТОВАРИЩЕСТВО", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКАЯ ПОТРЕБИТЕЛЬСКАЯ КООПЕРАЦИЯ") { Acronym = "СПК", AcronymCanBeLower = true };
            t.AddVariant("САДОВАЯ ПОТРЕБИТЕЛЬСКАЯ КООПЕРАЦИЯ", false);
            t.AddVariant("САДОВОДЧЕСКИЙ ПОТРЕБИТЕЛЬНЫЙ КООПЕРАТИВ", false);
            t.AddVariant("САДОВОДЧЕСКИЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ", false);
            m_OrgOntology.Add(t);
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ДАЧНО СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ") { Acronym = "ДСПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНО ПРОИЗВОДСТВЕННЫЙ КООПЕРАТИВ") { Acronym = "ЖСПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ЖСК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ЖИЛИЩНЫЙ СТРОИТЕЛЬНЫЙ КООПЕРАТИВ ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "ЖСКИЗ", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ОБЪЕДИНЕНИЕ") { Acronym = "ОНО", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО") { Acronym = "ОНП", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО") { Acronym = "ОНТ", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ОГОРОДНИЧЕСКИЙ ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ОПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "СТСН", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("САДОВОДЧЕСКОЕ ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "ТСН", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО СОБСТВЕННИКОВ ЖИЛЬЯ") { Acronym = "ТСЖ", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("САДОВЫЕ ЗЕМЕЛЬНЫЕ УЧАСТКИ") { Acronym = "СЗУ", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ТОВАРИЩЕСТВО ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "ТИЗ", AcronymCanBeLower = true });
            t = new Pullenti.Ner.Core.Termin("КОЛЛЕКТИВ ИНДИВИДУАЛЬНЫХ ЗАСТРОЙЩИКОВ") { Acronym = "КИЗ", AcronymCanBeLower = true };
            t.AddVariant("КИЗК", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("САДОВОЕ НЕКОММЕРЧЕСКОЕ ТОВАРИЩЕСТВО СОБСТВЕННИКОВ НЕДВИЖИМОСТИ") { Acronym = "СНТСН", AcronymCanBeLower = true };
            t.AddVariant("СНТ СН", false);
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ОБЪЕДИНЕНИЕ");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СОВМЕСТНОЕ ПРЕДПРИЯТИЕ");
            t.Acronym = "СП";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("НЕКОММЕРЧЕСКОЕ ПАРТНЕРСТВО");
            t.Acronym = "НП";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("АВТОМОБИЛЬНЫЙ КООПЕРАТИВ");
            t.AddAbridge("А/К");
            t.Acronym = "АК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ГАРАЖНЫЙ КООПЕРАТИВ");
            t.AddAbridge("Г/К");
            t.AddAbridge("ГР.КОП.");
            t.AddAbridge("ГАР.КОП.");
            t.Acronym = "ГК";
            t.AcronymCanBeLower = true;
            m_OrgOntology.Add(t);
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ГСК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО ЭКСПЛУАТАЦИОННЫЙ КООПЕРАТИВ") { Acronym = "ГЭК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНО ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ГПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ ГАРАЖНО СТРОИТЕЛЬНЫЙ КООПЕРАТИВ") { Acronym = "ПГСК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ГАРАЖНЫЙ СТРОИТЕЛЬНО ПОТРЕБИТЕЛЬСКИЙ КООПЕРАТИВ") { Acronym = "ГСПК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ПОТРЕБИТЕЛЬСКИЙ ГАРАЖНЫЙ КООПЕРАТИВ") { Acronym = "ПГК", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ИНДИВИДУАЛЬНОЕ ЖИЛИЩНОЕ СТРОИТЕЛЬСТВО") { Acronym = "ИЖС", AcronymCanBeLower = true });
            m_OrgOntology.Add(new Pullenti.Ner.Core.Termin("ЖИВОТНОВОДЧЕСКАЯ ТОЧКА"));
            t = new Pullenti.Ner.Core.Termin("САНАТОРИЙ");
            t.AddAbridge("САН.");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ДОМ ОТДЫХА");
            t.AddAbridge("Д/О");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БАЗА ОТДЫХА");
            t.AddAbridge("Б/О");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("СОВХОЗ");
            t.AddAbridge("С-ЗА");
            t.AddAbridge("С/ЗА");
            t.AddAbridge("С/З");
            t.AddAbridge("СХ.");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("ПИОНЕРСКИЙ ЛАГЕРЬ");
            t.AddAbridge("П/Л");
            t.AddAbridge("П.Л.");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КУРОРТ");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("КОЛЛЕКТИВ ИНДИВИДУАЛЬНЫХ ВЛАДЕЛЬЦЕВ");
            m_OrgOntology.Add(t);
            t = new Pullenti.Ner.Core.Termin("БИЗНЕС ЦЕНТР");
            t.Acronym = "БЦ";
            t.AddVariant("БІЗНЕС ЦЕНТР", false);
            m_OrgOntology.Add(t);
        }
        internal static Pullenti.Ner.Core.TerminCollection m_OrgOntology;
    }
}