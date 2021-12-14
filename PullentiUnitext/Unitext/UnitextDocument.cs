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
using System.Xml;

namespace Pullenti.Unitext
{
    /// <summary>
    /// Unitext - документ
    /// </summary>
    public class UnitextDocument : UnitextItem
    {
        /// <summary>
        /// Содержимое (тело) структурно-текстового представления (СТП)
        /// </summary>
        public UnitextItem Content;
        /// <summary>
        /// Внутренние документы (например, для архива его разархивированные файлы или 
        /// вложения для писем)
        /// </summary>
        public List<UnitextDocument> InnerDocuments = new List<UnitextDocument>();
        /// <summary>
        /// Последовательность страниц UnilayPage с расположенными на них прямоугольниками с текстами и картинками. 
        /// Тексто-графическое представление (ТГП) - для форматов PDF, DjVu
        /// </summary>
        public List<UnilayPage> Pages = new List<UnilayPage>();
        /// <summary>
        /// Стили текста и абзацев
        /// </summary>
        public List<UnitextStyle> Styles = new List<UnitextStyle>();
        /// <summary>
        /// Информация о страницах и колонтитулах
        /// </summary>
        public List<UnitextPagesection> Sections = new List<UnitextPagesection>();
        /// <summary>
        /// Формат исходного документа
        /// </summary>
        public FileFormat SourceFormat;
        /// <summary>
        /// Имя исходного файла (если есть)
        /// </summary>
        public string SourceFileName;
        /// <summary>
        /// Страницы исходного файла (это когда файл PDF разбивается на несколько документов)
        /// </summary>
        public string SourceFilePages;
        /// <summary>
        /// Сообщение об ошибке, формируемое при обработке исходного файла (например, что файл зашифрован)
        /// </summary>
        public string ErrorMessage;
        /// <summary>
        /// Некоторые дополнительные атрибуты (зависят от формата)
        /// </summary>
        public Dictionary<string, string> Attrs = new Dictionary<string, string>();
        /// <summary>
        /// Если документ получен функцией CreateFromText, то это входной текст, 
        /// причём никак не корректированный. Функция GetPlaintext возвращает его независимо 
        /// от настроек параметров генерации.
        /// </summary>
        public string SourcePlainText;
        public override string ToString()
        {
            string res = string.Format("{0}{1}{2}", SourceFormat, (SourceFileName == null ? "" : " :"), SourceFileName ?? "");
            if (SourceFilePages != null) 
                res = string.Format("{0} (стр.{1})", res, SourceFilePages);
            return res;
        }
        public override UnitextItem Clone()
        {
            UnitextDocument res = new UnitextDocument();
            res._cloneFrom(this);
            foreach (UnitextStyle s in Styles) 
            {
                res.Styles.Add(s.Clone(false));
            }
            if (Content != null) 
            {
                res.Content = Content.Clone();
                if (Content.m_StyledFrag != null) 
                    res.Content.m_StyledFrag = Content.m_StyledFrag.Clone();
            }
            foreach (UnitextDocument d in InnerDocuments) 
            {
                res.InnerDocuments.Add(d.Clone() as UnitextDocument);
            }
            foreach (UnitextPagesection s in Sections) 
            {
                res.Sections.Add(s.Clone() as UnitextPagesection);
            }
            res.Pages = new List<UnilayPage>(Pages);
            res.SourceFormat = SourceFormat;
            res.SourceFileName = SourceFileName;
            res.SourceFilePages = SourceFilePages;
            res.SourceInfo = SourceInfo;
            res.ErrorMessage = ErrorMessage;
            res.SourcePlainText = SourcePlainText;
            res.DefaultGetHtmlParam = DefaultGetHtmlParam;
            foreach (KeyValuePair<string, string> a in Attrs) 
            {
                res.Attrs.Add(a.Key, a.Value);
            }
            this.RefreshParents();
            return res;
        }
        public override bool IsInline
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        /// <summary>
        /// Сгенерировать плоский текст
        /// </summary>
        /// <param name="res">куда записать</param>
        /// <param name="pars">параметры генерации</param>
        public override void GetPlaintext(StringBuilder res, GetPlaintextParam pars = null)
        {
            if (SourcePlainText != null) 
            {
                res.Append(SourcePlainText);
                return;
            }
            if (pars == null) 
                pars = new GetPlaintextParam() { SetPositions = true };
            if (pars.SetPositions) 
                BeginChar = res.Length;
            if (Content != null) 
            {
                Pullenti.Unitext.Internal.Uni.ChangeTextPosInfo ii = null;
                if (Content.m_StyledFrag != null) 
                    ii = new Pullenti.Unitext.Internal.Uni.ChangeTextPosInfo(Content.m_StyledFrag, Content);
                Content.GetPlaintext(res, pars);
                if (ii != null) 
                    ii.Restore(res.Length);
            }
            if (InnerDocuments.Count > 0 && pars.UseInnerDocuments) 
            {
                foreach (UnitextDocument d in InnerDocuments) 
                {
                    if (res.Length > 0) 
                    {
                        if (pars.NewLine != null) 
                        {
                            res.Append(pars.NewLine);
                            res.Append(pars.NewLine);
                        }
                        if (pars.PageBreak != null) 
                            res.Append(pars.PageBreak);
                    }
                    d.GetPlaintext(res, pars);
                }
            }
            if (pars.SetPositions) 
            {
                EndChar = res.Length - 1;
                if (Sections.Count == 1) 
                    Sections[0].EndChar = EndChar;
                else if (Sections.Count > 1 && Content != null) 
                {
                    List<UnitextItem> its = new List<UnitextItem>();
                    this.GetAllItems(its, 0);
                    foreach (UnitextItem it in its) 
                    {
                        UnitextPagesection ps = it.PageSection;
                        if (ps == null) 
                            continue;
                        if (it.BeginChar > 0 && ps.BeginChar == 0) 
                            ps.BeginChar = it.BeginChar;
                        if (it.EndChar > ps.EndChar) 
                            ps.EndChar = it.EndChar;
                    }
                }
            }
        }
        /// <summary>
        /// Сгенерировать плоский текст
        /// </summary>
        /// <param name="pars">параметры генерации</param>
        /// <return>результат</return>
        public override string GetPlaintextString(GetPlaintextParam pars = null)
        {
            StringBuilder res = new StringBuilder();
            this.GetPlaintext(res, pars);
            if (res.Length == 0) 
                return null;
            return res.ToString();
        }
        // Параметры генерации Html по умолчанию
        public GetHtmlParam DefaultGetHtmlParam;
        /// <summary>
        /// Сгенерировать HTML
        /// </summary>
        /// <param name="res">куда записать</param>
        /// <param name="par">параметры генерации</param>
        public override void GetHtml(StringBuilder res, GetHtmlParam par = null)
        {
            if (par == null) 
                par = DefaultGetHtmlParam;
            if (par == null) 
                par = new GetHtmlParam();
            if (!par.CallBefore(this, res)) 
                return;
            if (par.OutHtmlAndBodyTags) 
            {
                res.Append("<html><meta charset=\"utf-8\"/>");
                if (!string.IsNullOrEmpty(par.Title)) 
                {
                    res.Append("\r\n<title>");
                    Pullenti.Util.MiscHelper.CorrectHtmlValue(res, par.Title, false, false);
                    res.Append("</title>");
                }
                res.Append("<body>");
            }
            if (Id != null) 
                res.AppendFormat("<a name=\"{0}\"> </a>", Id);
            if (ErrorMessage != null) 
            {
                res.AppendFormat("\r\n<div style=\"border:2pt solid red\">");
                Pullenti.Util.MiscHelper.CorrectHtmlValue(res, ErrorMessage, false, false);
                res.AppendFormat("</div>");
            }
            if (Content != null) 
            {
                Content.GetHtml(res, par);
                par._outFootnotes(res);
                par._outEndnotes(res);
            }
            if (par == null || par.UseInnerDocuments) 
            {
                foreach (UnitextDocument d in InnerDocuments) 
                {
                    res.AppendFormat("\r\n<HR/><H2>Внутренний документ {0}</H1>", d.SourceFileName ?? "");
                    d.GetHtml(res, par);
                }
            }
            par.CallAfter(this, res);
            if (par.OutHtmlAndBodyTags) 
                res.Append("\r\n</body></html>");
        }
        /// <summary>
        /// Сгенерировать HTML
        /// </summary>
        /// <param name="par">параметры генерации</param>
        /// <return>результат</return>
        public override string GetHtmlString(GetHtmlParam par = null)
        {
            StringBuilder res = new StringBuilder();
            this.GetHtml(res, par);
            return res.ToString();
        }
        /// <summary>
        /// Сериализовать в XML. Потом можно восстановить фукнцией FromXml().
        /// </summary>
        /// <param name="xml">куда сериализовать</param>
        public override void GetXml(XmlWriter xml)
        {
            xml.WriteStartElement("doc");
            if (SourceFormat != FileFormat.Unknown) 
                xml.WriteAttributeString("format", SourceFormat.ToString().ToLower());
            if (SourceFileName != null) 
                xml.WriteAttributeString("filename", Pullenti.Util.MiscHelper.CorrectXmlValue(Path.GetFileName(SourceFileName)));
            if (SourceFilePages != null) 
                xml.WriteAttributeString("filepages", SourceFilePages);
            if (ErrorMessage != null) 
                xml.WriteAttributeString("error", Pullenti.Util.MiscHelper.CorrectXmlValue(ErrorMessage));
            foreach (KeyValuePair<string, string> a in Attrs) 
            {
                xml.WriteStartElement("attr");
                xml.WriteAttributeString("name", a.Key);
                xml.WriteAttributeString("val", Pullenti.Util.MiscHelper.CorrectXmlValue(a.Value) ?? "");
                xml.WriteEndElement();
            }
            if (SourcePlainText != null) 
                xml.WriteElementString("sourcetext", Convert.ToBase64String(Pullenti.Util.MiscHelper.EncodeStringUtf8(SourcePlainText, false)));
            if (Styles.Count > 0) 
            {
                xml.WriteStartElement("styles");
                foreach (UnitextStyle s in Styles) 
                {
                    s.GetXml(xml);
                }
                xml.WriteEndElement();
            }
            foreach (UnitextPagesection s in Sections) 
            {
                s.GetXml(xml);
            }
            if (Content != null) 
            {
                xml.WriteStartElement("content");
                Content.GetXml(xml);
                xml.WriteEndElement();
                if (Content.m_StyledFrag != null) 
                    Content.m_StyledFrag.GetXml(xml, null, false);
            }
            foreach (UnitextDocument d in InnerDocuments) 
            {
                d.GetXml(xml);
            }
            xml.WriteEndElement();
        }
        /// <summary>
        /// Десериализовать из XML, полученный функцией GetXml().
        /// </summary>
        /// <param name="xml">корневой узел, куда сериализовали</param>
        public override void FromXml(XmlNode xml)
        {
            if (xml.Attributes != null) 
            {
                foreach (XmlAttribute a in xml.Attributes) 
                {
                    if (a.LocalName == "format") 
                    {
                        try 
                        {
                            SourceFormat = (FileFormat)Enum.Parse(typeof(FileFormat), a.Value, true);
                        }
                        catch(Exception ex324) 
                        {
                        }
                    }
                    else if (a.LocalName == "filename") 
                        SourceFileName = a.Value;
                    else if (a.LocalName == "filepages") 
                        SourceFilePages = a.Value;
                    else if (ErrorMessage != null) 
                        ErrorMessage = a.Value;
                }
            }
            foreach (XmlNode x in xml.ChildNodes) 
            {
                if (x.LocalName == "content") 
                {
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        Content = Pullenti.Unitext.Internal.Uni.UnitextHelper.CreateItem(xx);
                        Content.Parent = this;
                        break;
                    }
                }
                else if (x.LocalName == "sourcetext") 
                {
                    try 
                    {
                        SourcePlainText = Pullenti.Util.MiscHelper.DecodeStringUtf8(Convert.FromBase64String(x.InnerText), 0, -1);
                    }
                    catch(Exception ex) 
                    {
                    }
                }
                else if (x.LocalName == "section") 
                {
                    UnitextPagesection s = new UnitextPagesection();
                    s.FromXml(x);
                    Sections.Add(s);
                }
                else if (x.LocalName == "doc") 
                {
                    UnitextDocument d = new UnitextDocument();
                    d.FromXml(x);
                    InnerDocuments.Add(d);
                }
                else if (x.LocalName == "attr" && x.Attributes != null) 
                {
                    try 
                    {
                        Attrs.Add(x.Attributes["name"].Value, x.Attributes["val"].Value);
                    }
                    catch(Exception ex325) 
                    {
                    }
                }
                else if (x.LocalName == "styles") 
                {
                    Styles.Clear();
                    foreach (XmlNode xx in x.ChildNodes) 
                    {
                        UnitextStyle sty = new UnitextStyle();
                        sty.FromXml(xx);
                        Styles.Add(sty);
                    }
                }
                else if (x.LocalName == "stylefrag" && Content != null) 
                {
                    Content.m_StyledFrag = new UnitextStyledFragment();
                    Content.m_StyledFrag.Doc = this;
                    Content.m_StyledFrag.FromXml(x);
                }
            }
            this.GetAllItems(null, 0);
        }
        public override UnitextItem Optimize(bool isContent, CreateDocumentParam pars)
        {
            if (Content != null) 
                Content = Content.Optimize(true, pars);
            foreach (UnitextPagesection s in Sections) 
            {
                if (s.Header != null) 
                    s.Header = s.Header.Optimize(true, pars);
                if (s.Footer != null) 
                    s.Footer = s.Footer.Optimize(true, pars);
            }
            foreach (UnitextDocument d in InnerDocuments) 
            {
                d.Optimize(false, pars);
            }
            return this;
        }
        // Обновить ссылки на родительские элементы
        public void RefreshParents()
        {
            this.GetAllItems(null, 0);
        }
        /// <summary>
        /// После OCR-распознавания обновить СТП (Content) на основе нового ТГП (Pages)
        /// </summary>
        public void RefreshContentByPages()
        {
            if (Pages.Count == 0) 
                return;
            Pullenti.Unitext.Internal.Uni.UnilayoutHelper.CreateContentFromPages(this, true);
            Pullenti.Unitext.Internal.Uni.UnitextCorrHelper.RemoveFalseNewLines(this, true);
            this.RefreshParents();
        }
        /// <summary>
        /// Получить список всех элементов (включая и сам документ как элемент). 
        /// Порядок последовательный, как они входят в дерево и в какой последовательности генерируется плоский текст. 
        /// Колонтитулы не включаются.
        /// </summary>
        public List<UnitextItem> ContentItems
        {
            get
            {
                List<UnitextItem> res = new List<UnitextItem>();
                this.GetAllItems(res, 0);
                return res;
            }
        }
        public override void GetAllItems(List<UnitextItem> res, int lev = 0)
        {
            if (res != null) 
                res.Add(this);
            if (Content != null) 
            {
                Content.Parent = this;
                Content.GetAllItems(res, lev + 1);
            }
            foreach (UnitextDocument d in InnerDocuments) 
            {
                d.Parent = this;
                d.GetAllItems(res, lev + 1);
            }
            List<UnitextItem> tmp = null;
            if (res != null) 
                tmp = new List<UnitextItem>();
            foreach (UnitextPagesection s in Sections) 
            {
                s.Parent = this;
                if (s.Header != null) 
                {
                    s.Header.GetAllItems(tmp, 0);
                    s.Header.Parent = s;
                }
                if (s.Footer != null) 
                {
                    s.Footer.GetAllItems(tmp, 0);
                    s.Footer.Parent = s;
                }
            }
            if (lev == 0 && res != null) 
            {
                foreach (UnitextItem it in res) 
                {
                    if (it.Parent != null && it.PageSectionId != null) 
                    {
                        if (it.PageSection == it.Parent.PageSection) 
                            it.PageSectionId = null;
                    }
                }
            }
            if (tmp != null) 
            {
                foreach (UnitextItem it in tmp) 
                {
                    if (it.Parent != null && it.PageSectionId != null) 
                    {
                        if (it.PageSection == it.PageSection.PageSection) 
                            it.PageSectionId = null;
                    }
                }
            }
        }
        // Откорректировать документ
        public bool Correct(CorrectDocumentParam corrPars)
        {
            if (corrPars == null) 
                corrPars = new CorrectDocumentParam();
            bool ret = false;
            if (corrPars.RestoreTables) 
            {
                if (Pullenti.Unitext.Internal.Uni.UnitextCorrHelper.RestoreTables(this)) 
                    ret = true;
            }
            if (corrPars.RemovePageBreakNumeration) 
            {
                FileFormatClass fty = Pullenti.Util.FileFormatsHelper.GetFormatClass(SourceFormat);
                if (SourceFormat == FileFormat.Txt || fty == FileFormatClass.Pagelayout || fty == FileFormatClass.Image) 
                    Pullenti.Unitext.Internal.Uni.UnitextCorrHelper.RemovePageBreakNumeration(this);
            }
            if (corrPars.RemoveFalseNewLines) 
                Pullenti.Unitext.Internal.Uni.UnitextCorrHelper.RemoveFalseNewLines(this, corrPars.ReplaceNbspBySpace);
            if (corrPars.RestoreTextFootnotes) 
                Pullenti.Unitext.Internal.Uni.UnitextCorrHelper.RestoreTextFootnotes(this);
            if (corrPars.OptimizeStructure) 
                this.Optimize(false, new CreateDocumentParam());
            return ret;
        }
        public override UnitextStyledFragment GetStyledFragment(int textPos = -1)
        {
            if (Content != null) 
            {
                if (Content.m_StyledFrag == null) 
                    return null;
                return Content.GetStyledFragment(textPos);
            }
            return null;
        }
        /// <summary>
        /// Встроить контейнер в дерево элементов. 
        /// ВНИМАНИЕ! Встраивание возможно только после вызова GetPlaintext(), 
        /// когда значения BeginChar и EndChar установлены у всех элементов, и встраивание происходит относительно этих значений. 
        /// Идентификатор у встраиваемого контейнера устанавливать самим, если нужно потом производить к нему навигацию в Html.
        /// </summary>
        /// <param name="cnt">встраиваемый контейнер (любой наследный от контейнера класс) с установленными значениями BeginChar и EndChar</param>
        /// <param name="plaintext">сгенерированный плоский текст, относительно которого идёт позиционирование</param>
        /// <param name="parent">возможный контейнерный блок (если известен), может быть null</param>
        /// <return>получилось ли встроить</return>
        public bool Implantate(UnitextContainer cnt, string plaintext, UnitextDocblock parent = null)
        {
            if (Content == null) 
                return false;
            if (parent != null) 
            {
                Pullenti.Unitext.Internal.Uni.UnitextImplantateHelper.Implantate(parent, cnt, plaintext, null);
                return cnt.Parent != null;
            }
            UnitextItem res = Pullenti.Unitext.Internal.Uni.UnitextImplantateHelper.Implantate(Content, cnt, plaintext, null);
            if (res == null) 
                return false;
            Content = res;
            return cnt.Parent != null;
        }
        /// <summary>
        /// Встроить гиперссылку в дерево элементов. 
        /// ВНИМАНИЕ! Встраивание возможно только после вызова GetPlaintext(), 
        /// когда значения BeginChar и EndChar установлены у всех элементов, и встраивание происходит относительно этих значений. 
        /// Идентификатор у встраиваемого контейнера устанавливать самим, если нужно потом производить к нему навигацию в Html.
        /// </summary>
        /// <param name="hl">встраиваемая гиперссылка с установленными значениями BeginChar и EndChar</param>
        /// <param name="plaintext">сгенерированный плоский текст, относительно которого идёт позиционирование</param>
        /// <param name="parent">возможный контейнерный блок (если известен), может быть null</param>
        /// <return>получилось ли встроить</return>
        public bool ImplantateHyperlink(UnitextHyperlink hl, string plaintext, UnitextDocblock parent = null)
        {
            if (Content == null) 
                return false;
            UnitextContainer cnt = new UnitextContainer();
            cnt.BeginChar = hl.BeginChar;
            cnt.EndChar = hl.EndChar;
            if (!this.Implantate(cnt, plaintext, parent)) 
                return false;
            if (cnt.Parent == null) 
                return false;
            if (!cnt.Parent.ReplaceChild(cnt, hl)) 
                return false;
            hl.Content = cnt.Optimize(true, null);
            if (hl.Content != null) 
                hl.Content.Parent = hl;
            return true;
        }
        /// <summary>
        /// Встроить в дерево структурирующий блок UnitextDocblock. 
        /// Его идентификатор Id нужно устанавливать самим, если нужно.
        /// </summary>
        /// <param name="beginHead">начальная позиция заголовка блока</param>
        /// <param name="beginBody">начальная позиция тела блока</param>
        /// <param name="beginTail">начальная позиция подписи (если = beginBody, то отсутствует)</param>
        /// <param name="beginAppendix">начальная позиция приложения (если = end, то отсутствует)</param>
        /// <param name="end">конечная позиция</param>
        /// <param name="plaintext">сгенерированный плоский текст, относительно которого идёт позиционирование</param>
        /// <param name="parent">возможный вышележащий блок</param>
        /// <return>в случае успеха вернёт указатель встроенный блок, null - если не получилось</return>
        public UnitextDocblock ImplantateBlock(int beginHead, int beginBody, int beginTail, int beginAppendix, int end, string plaintext, UnitextDocblock parent = null)
        {
            UnitextDocblock res = null;
            if ((parent != null && parent.Appendix != null && beginHead >= parent.Appendix.BeginChar) && end <= parent.Appendix.EndChar) 
            {
                UnitextItem cnt = Pullenti.Unitext.Internal.Uni.UnitextImplantateHelper.ImplantateBlock(parent.Appendix, beginHead, beginBody, beginTail, beginAppendix, end, plaintext, null, ref res);
                if (cnt != null) 
                    parent.Appendix = cnt;
            }
            else if ((parent != null && parent.Body != null && beginHead >= parent.BeginChar) && end <= parent.EndChar) 
            {
                UnitextItem cnt = Pullenti.Unitext.Internal.Uni.UnitextImplantateHelper.ImplantateBlock(parent.Body, beginHead, beginBody, beginTail, beginAppendix, end, plaintext, null, ref res);
                if (cnt != null) 
                    parent.Body = cnt;
            }
            else 
            {
                UnitextItem cnt = Pullenti.Unitext.Internal.Uni.UnitextImplantateHelper.ImplantateBlock(Content, beginHead, beginBody, beginTail, beginAppendix, end, plaintext, null, ref res);
                if (cnt != null) 
                    Content = cnt;
            }
            return res;
        }
        /// <summary>
        /// Удалить элементы определённого типа
        /// </summary>
        /// <param name="typ">тип элементов</param>
        /// <param name="cntTyp">для контейнеров можно задать дополнительный тип</param>
        /// <return>количество удалённых элементов</return>
        public int RemoveItems(Type typ, UnitextContainerType cntTyp = UnitextContainerType.Undefined)
        {
            if (Content == null) 
                return 0;
            int cou = 0;
            Content = Pullenti.Unitext.Internal.Uni.UnitextImplantateHelper.Clear(Content, null, typ, cntTyp, null, ref cou);
            return cou;
        }
        /// <summary>
        /// Удалить элемент по его идентификатору Id. 
        /// Сейчас работает только для тех элементов, которые были встроены через Implantate
        /// </summary>
        /// <param name="id">идентификатор удаляемого элемента</param>
        /// <return>удалось ли удалить</return>
        public bool RemoveItemById(string id)
        {
            if (Content == null) 
                return false;
            int cou = 0;
            Content = Pullenti.Unitext.Internal.Uni.UnitextImplantateHelper.Clear(Content, id, null, UnitextContainerType.Undefined, null, ref cou);
            return cou > 0;
        }
        /// <summary>
        /// Удалить все гиперссылки, сделав их обычными текстами
        /// </summary>
        /// <param name="subString">если задан, то url должен содержать эту подстроку. Подстрок м.б. несколько, разделитель - точка с запятой.</param>
        /// <return>количество удалённых гиперссылок</return>
        public int RemoveAllHyperlinks(string subString = null)
        {
            List<UnitextItem> its = new List<UnitextItem>();
            this.GetAllItems(its, 0);
            int change = 0;
            List<string> substrs = new List<string>();
            if (subString != null) 
                substrs.AddRange(subString.ToUpper().Split(';'));
            foreach (UnitextItem it in its) 
            {
                UnitextHyperlink hl = it as UnitextHyperlink;
                if (hl == null) 
                    continue;
                if (substrs.Count > 0) 
                {
                    if (hl.Href == null) 
                    {
                        if (!substrs.Contains("")) 
                            continue;
                    }
                    else 
                    {
                        int k;
                        for (k = 0; k < substrs.Count; k++) 
                        {
                            if (!string.IsNullOrEmpty(substrs[k]) && hl.Href.ToUpper().Contains(substrs[k])) 
                                break;
                        }
                        if (k >= substrs.Count) 
                        {
                            if ((hl.Href.IndexOf(' ') < 0) && hl.Href.IndexOf('.') > 0) 
                                continue;
                        }
                    }
                }
                if (hl.Parent != null && hl.Content != null) 
                {
                    if (hl.Parent.ReplaceChild(hl, hl.Content)) 
                        change++;
                }
            }
            if (change > 0) 
                this.Optimize(false, new CreateDocumentParam());
            return change;
        }
        /// <summary>
        /// Сгенерировать внутренние идентификаторы у элементов. 
        /// Если у элемента Id установлен, то он не меняется. 
        /// У колонтитулов Id не устанавливается. По умолчанию, Id генерируются при создании документа.
        /// </summary>
        public void GenerateIds()
        {
            Dictionary<string, int> ids = new Dictionary<string, int>();
            List<UnitextItem> all = new List<UnitextItem>();
            this.GetAllItems(all, 0);
            foreach (UnitextItem it in all) 
            {
                if (it.InnerTag != null && it.Id != null && it.Id.StartsWith(it.InnerTag)) 
                {
                    int n;
                    if (int.TryParse(it.Id.Substring(it.InnerTag.Length), out n)) 
                    {
                        if (!ids.ContainsKey(it.InnerTag)) 
                            ids.Add(it.InnerTag, n);
                        else if (ids[it.InnerTag] < n) 
                            ids[it.InnerTag] = n;
                    }
                }
            }
            foreach (UnitextItem it in all) 
            {
                if (it.InnerTag != null && it != this && it.Id == null) 
                {
                    int id;
                    if (!ids.TryGetValue(it.InnerTag, out id)) 
                        ids.Add(it.InnerTag, (id = 1));
                    else 
                        ids[it.InnerTag] = ++id;
                    it.Id = string.Format("{0}{1}", it.InnerTag, id);
                }
            }
        }
        /// <summary>
        /// Объединить содержимое с содержимым другого документа
        /// </summary>
        /// <param name="doc">другой документ</param>
        public void MergeWith(UnitextDocument doc)
        {
            UnitextContainer cnt = Content as UnitextContainer;
            UnitextContainer cnt2 = doc.Content as UnitextContainer;
            if (cnt == null) 
            {
                cnt = new UnitextContainer();
                if (Content != null) 
                    cnt.Children.Add(Content);
                Content = cnt;
            }
            cnt.Children.Add(new UnitextPagebreak());
            if (cnt2 != null) 
                cnt.Children.AddRange(cnt2.Children);
            else if (doc.Content != null) 
                cnt.Children.Add(doc.Content);
            Sections.AddRange(doc.Sections);
        }
        /// <summary>
        /// Преобразовать в байтовый поток (со сжатием). Восстанавливать потом функцией Deserialize().
        /// </summary>
        /// <return>результат</return>
        public byte[] Serialize()
        {
            StringBuilder tmp = new StringBuilder();
            using (XmlWriter xml = XmlWriter.Create(tmp, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = false })) 
            {
                this.GetXml(xml);
            }
            string str = tmp.ToString();
            byte[] dat = Pullenti.Util.MiscHelper.EncodeStringUtf8(str, false);
            return Pullenti.Util.ArchiveHelper.CompressGZip(dat);
        }
        /// <summary>
        /// Восстановить документ из байтового потока, полученного функцией Serialize(). 
        /// Если что не так, то выдаст Exception.
        /// </summary>
        /// <param name="dat">массив байт</param>
        public void Deserialize(byte[] dat)
        {
            if (dat == null || dat.Length == 0) 
                return;
            dat = Pullenti.Util.ArchiveHelper.DecompressGZip(dat);
            string txt = Pullenti.Util.MiscHelper.DecodeStringUtf8(dat, 0, -1);
            if (string.IsNullOrEmpty(txt)) 
                return;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(txt);
            this.FromXml(xml.DocumentElement);
        }
        internal override string InnerTag
        {
            get
            {
                return "doc";
            }
        }
        /// <summary>
        /// Найти элемент по его идентификатору
        /// </summary>
        /// <param name="id">идентификатор элемента</param>
        /// <return>результат или null</return>
        public override UnitextItem FindById(string id)
        {
            if (Id == id) 
                return this;
            UnitextItem res;
            if (Content != null) 
            {
                if ((((res = Content.FindById(id)))) != null) 
                    return res;
            }
            foreach (UnitextDocument doc in InnerDocuments) 
            {
                if ((((res = doc.FindById(id)))) != null) 
                    return res;
            }
            return null;
        }
    }
}