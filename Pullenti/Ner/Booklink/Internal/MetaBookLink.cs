/*
 * SDK Pullenti Lingvo, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software.
 * This class is generated using the converter Unisharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Ner.Booklink.Internal
{
    class MetaBookLink : Pullenti.Ner.Metadata.ReferentClass
    {
        public static void Initialize()
        {
            GlobalMeta = new MetaBookLink();
            GlobalMeta.AddFeature(Pullenti.Ner.Booklink.BookLinkReferent.ATTR_AUTHOR, "Автор", 0, 0);
            GlobalMeta.AddFeature(Pullenti.Ner.Booklink.BookLinkReferent.ATTR_NAME, "Наименование", 1, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Booklink.BookLinkReferent.ATTR_TYPE, "Тип", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Booklink.BookLinkReferent.ATTR_YEAR, "Год", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Booklink.BookLinkReferent.ATTR_GEO, "География", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Booklink.BookLinkReferent.ATTR_LANG, "Язык", 0, 1);
            GlobalMeta.AddFeature(Pullenti.Ner.Booklink.BookLinkReferent.ATTR_URL, "URL", 0, 0);
            GlobalMeta.AddFeature(Pullenti.Ner.Booklink.BookLinkReferent.ATTR_MISC, "Разное", 0, 0);
        }
        public override string Name
        {
            get
            {
                return Pullenti.Ner.Booklink.BookLinkReferent.OBJ_TYPENAME;
            }
        }
        public override string Caption
        {
            get
            {
                return "Ссылка на внешний источник";
            }
        }
        public static string ImageId = "booklink";
        public override string GetImageId(Pullenti.Ner.Referent obj = null)
        {
            return ImageId;
        }
        internal static MetaBookLink GlobalMeta;
    }
}