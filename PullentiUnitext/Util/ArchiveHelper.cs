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
using System.IO.Compression;

namespace Pullenti.Util
{
    /// <summary>
    /// Работа с архивами и сжатием
    /// </summary>
    public static class ArchiveHelper
    {
        /// <summary>
        /// Заархивировать массив байт алгоритмом GZip
        /// </summary>
        /// <param name="dat">исходный массив</param>
        /// <return>заархивированый массив</return>
        public static byte[] CompressGZip(byte[] dat)
        {
            using (MemoryStream res = new MemoryStream()) 
            {
                using (MemoryStream input = new MemoryStream(dat)) 
                {
                    input.Position = 0;
                    using (GZipStream deflate = new GZipStream(res, CompressionMode.Compress)) 
                    {
                        input.WriteTo(deflate);
                        deflate.Flush();
                        deflate.Dispose();
                    }
                }
                return res.ToArray();
            }
        }
        /// <summary>
        /// Разархивировать байтовый массив алгоритмом GZip
        /// </summary>
        /// <param name="zip">архивированный массив</param>
        /// <return>результат</return>
        public static byte[] DecompressGZip(byte[] zip)
        {
            using (MemoryStream data = new MemoryStream(zip)) 
            {
                data.Position = 0;
                using (MemoryStream unzip = new MemoryStream()) 
                {
                    byte[] buf = new byte[(int)(zip.Length + zip.Length)];
                    using (GZipStream deflate = new GZipStream(data, CompressionMode.Decompress)) 
                    {
                        while (true) 
                        {
                            int i = -1;
                            try 
                            {
                                for (int ii = 0; ii < buf.Length; ii++) 
                                {
                                    buf[ii] = 0;
                                }
                                i = deflate.Read(buf, 0, buf.Length);
                            }
                            catch(Exception ex) 
                            {
                                for (i = buf.Length - 1; i >= 0; i--) 
                                {
                                    if (buf[i] != 0) 
                                    {
                                        unzip.Write(buf, 0, i + 1);
                                        break;
                                    }
                                }
                                break;
                            }
                            if (i < 1) 
                                break;
                            unzip.Write(buf, 0, i);
                        }
                    }
                    byte[] res = unzip.ToArray();
                    return res;
                }
            }
        }
        /// <summary>
        /// Заархивировать байтовый массив алгоритмом Zlib
        /// </summary>
        /// <param name="dat">исходный массив</param>
        /// <return>заархивированый массив</return>
        public static byte[] CompressZlib(byte[] dat)
        {
            using (MemoryStream res = new MemoryStream()) 
            {
                int header = ((8 + (((15 - 8)) << 4))) << 8;
                int level_flags = 3;
                header |= level_flags << 6;
                header += (31 - ((header % 31)));
                res.WriteByte((byte)((((header >> 8)) & 0xFF)));
                res.WriteByte((byte)((header & 0xFF)));
                using (MemoryStream input = new MemoryStream(dat)) 
                {
                    input.Position = 0;
                    using (DeflateStream deflate = new DeflateStream(res, CompressionMode.Compress)) 
                    {
                        input.WriteTo(deflate);
                        deflate.Flush();
                    }
                }
                byte[] bres = res.ToArray();
                Pullenti.Unitext.Internal.Zip.Adler32 adl = new Pullenti.Unitext.Internal.Zip.Adler32();
                adl.UpdateByBuf(dat);
                byte[] bres2 = new byte[(int)(bres.Length + 4)];
                int i;
                for (i = 0; i < bres.Length; i++) 
                {
                    bres2[i] = bres[i];
                }
                bres2[i] = (byte)((((adl.Value >> 24)) & 0xFF));
                bres2[i + 1] = (byte)((((adl.Value >> 16)) & 0xFF));
                bres2[i + 2] = (byte)((((adl.Value >> 8)) & 0xFF));
                bres2[i + 3] = (byte)((adl.Value & 0xFF));
                return bres2;
            }
        }
        /// <summary>
        /// Разархивировать байтовый массив алгоритмом Zlib.
        /// </summary>
        /// <param name="zip">архивированный массив</param>
        /// <return>результат</return>
        public static byte[] DecompressZlib(byte[] zip)
        {
            if (zip == null || (zip.Length < 4)) 
                return null;
            int pos0 = 2;
            int CMF = (int)zip[0];
            int FLG = (int)zip[1];
            int CM = CMF & 0xF;
            int CINFO = ((CMF >> 4)) & 0xF;
            if (CM != 8 || CINFO > 7) 
                return null;
            if ((((((CMF * 256) + FLG)) % 31)) != 0) 
                return null;
            if (((((FLG >> 5)) & 1)) != 0) 
                pos0 += 4;
            byte[] zip1 = new byte[(int)(zip.Length - pos0 - 4)];
            for (int i = 0; i < zip1.Length; i++) 
            {
                zip1[i] = zip[i + pos0];
            }
            zip = zip1;
            using (MemoryStream unzip = new MemoryStream()) 
            {
                byte[] buf = new byte[(int)(zip.Length + zip.Length)];
                using (MemoryStream data = new MemoryStream(zip)) 
                {
                    data.Position = 0;
                    using (DeflateStream deflate = new DeflateStream(data, CompressionMode.Decompress)) 
                    {
                        while (true) 
                        {
                            int i = -1;
                            try 
                            {
                                for (int ii = 0; ii < buf.Length; ii++) 
                                {
                                    buf[ii] = 0;
                                }
                                i = deflate.Read(buf, 0, buf.Length);
                            }
                            catch(Exception ex) 
                            {
                                for (i = buf.Length - 1; i >= 0; i--) 
                                {
                                    if (buf[i] != 0) 
                                    {
                                        unzip.Write(buf, 0, i + 1);
                                        break;
                                    }
                                }
                                break;
                            }
                            if (i < 1) 
                                break;
                            unzip.Write(buf, 0, i);
                        }
                    }
                    byte[] res = unzip.ToArray();
                    return res;
                }
            }
        }
        /// <summary>
        /// Заархивировать байтовый массив алгоритмом Deflate (без Zlib-заголовка)
        /// </summary>
        /// <param name="dat">исходный массив</param>
        /// <return>заархивированый массив</return>
        public static byte[] CompressDeflate(byte[] dat)
        {
            byte[] zip = null;
            using (MemoryStream res = new MemoryStream()) 
            {
                using (MemoryStream input = new MemoryStream(dat)) 
                {
                    input.Position = 0;
                    using (DeflateStream deflate = new DeflateStream(res, CompressionMode.Compress)) 
                    {
                        input.WriteTo(deflate);
                        deflate.Flush();
                    }
                }
                zip = res.ToArray();
            }
            return zip;
        }
        /// <summary>
        /// Разархивировать байтовый массив алгоритмом Deflate (без Zlib-заголовка)
        /// </summary>
        /// <param name="zip">архивированный массив</param>
        /// <return>результат</return>
        public static byte[] DecompressDeflate(byte[] zip)
        {
            if (zip == null || (zip.Length < 1)) 
                return null;
            using (MemoryStream unzip = new MemoryStream()) 
            {
                byte[] buf = new byte[(int)(zip.Length + zip.Length)];
                using (MemoryStream data = new MemoryStream(zip)) 
                {
                    data.Position = 0;
                    using (DeflateStream deflate = new DeflateStream(data, CompressionMode.Decompress)) 
                    {
                        while (true) 
                        {
                            int i = deflate.Read(buf, 0, buf.Length);
                            if (i < 1) 
                                break;
                            unzip.Write(buf, 0, i);
                        }
                    }
                    return unzip.ToArray();
                }
            }
        }
        /// <summary>
        /// Создать zip в потоке
        /// </summary>
        /// <param name="res">результирующий поток</param>
        /// <param name="files">файлы для архивирования</param>
        public static void CreateZipStream(Stream res, List<FileInArchive> files)
        {
            List<MemoryStream> streams = new List<MemoryStream>();
            try 
            {
                using (Pullenti.Unitext.Internal.Zip.ZipFile zip = Pullenti.Unitext.Internal.Zip.ZipFile.CreateStream(res)) 
                {
                    zip.BeginUpdate0();
                    foreach (FileInArchive f in files) 
                    {
                        MemoryStream mem = new MemoryStream(f.Content);
                        zip.AddStream(mem, f.Key);
                        streams.Add(mem);
                    }
                    zip.CommitUpdate();
                }
            }
            catch(Exception ex) 
            {
                throw new Exception("ZIP Error: " + ex, ex);
            }
            foreach (MemoryStream m in streams) 
            {
                m.Dispose();
            }
        }
        /// <summary>
        /// Создать zip-файл
        /// </summary>
        /// <param name="resName">результирующее имя архивного файла</param>
        /// <param name="files">файлы для архивирования</param>
        public static void CreateZipFile(string resName, List<string> files)
        {
            try 
            {
                using (Pullenti.Unitext.Internal.Zip.ZipFile zip = Pullenti.Unitext.Internal.Zip.ZipFile.CreateFile(resName)) 
                {
                    zip.BeginUpdate0();
                    foreach (string f in files) 
                    {
                        zip.Add(f, Path.GetFileName(f));
                    }
                    zip.CommitUpdate();
                }
            }
            catch(Exception ex) 
            {
                throw new Exception("ZIP Error: " + ex, ex);
            }
        }
        /// <summary>
        /// Разархивировать файлы из ZIP-архива
        /// </summary>
        /// <param name="zipFile">имя архива</param>
        /// <param name="outDir">результирующая директория</param>
        /// <return>список полных имён разархивированных файлов</return>
        public static List<string> UnzipFiles(string zipFile, string outDir)
        {
            List<string> files = new List<string>();
            using (Pullenti.Unitext.Internal.Misc.MyZipFile zip = new Pullenti.Unitext.Internal.Misc.MyZipFile(zipFile, null)) 
            {
                foreach (Pullenti.Unitext.Internal.Misc.MyZipEntry e in zip.Entries) 
                {
                    if (e.IsDirectory || e.Encrypted || e.UncompressDataSize == 0) 
                        continue;
                    byte[] dat = e.GetData();
                    if (dat == null) 
                        continue;
                    string fname = Path.Combine(outDir, Path.GetFileName(e.Name));
                    files.Add(fname);
                    File.WriteAllBytes(fname, dat);
                }
            }
            return files;
        }
        /// <summary>
        /// Получить список файлов, содержащихся в архиве (архив может быть ZIP, RAR, TAR и некоторые другие)
        /// </summary>
        /// <param name="fileName">имя архива</param>
        /// <param name="content">содержимое файла (если null, то fileName - ссылка на локальный файл)</param>
        /// <return>список файлов в виде словаря {имя файла, размер файла} или null, если не поддержано</return>
        public static Dictionary<string, int> GetFileNamesFromArchive(string fileName, byte[] content = null)
        {
            Pullenti.Unitext.FileFormat frm = FileFormatsHelper.AnalizeFileFormat(fileName, content);
            Dictionary<string, int> res = new Dictionary<string, int>();
            if (frm == Pullenti.Unitext.FileFormat.Zip || frm == Pullenti.Unitext.FileFormat.Docx) 
            {
                using (Pullenti.Unitext.Internal.Misc.MyZipFile zip = new Pullenti.Unitext.Internal.Misc.MyZipFile(fileName, content)) 
                {
                    foreach (Pullenti.Unitext.Internal.Misc.MyZipEntry e in zip.Entries) 
                    {
                        if (e.IsDirectory || e.Encrypted || e.UncompressDataSize == 0) 
                            continue;
                        if (!res.ContainsKey(e.Name)) 
                            res.Add(e.Name, e.CompressDataSize);
                    }
                }
                return res;
            }
            try 
            {
                if (Pullenti.Unitext.UnitextService.Extension != null) 
                    return Pullenti.Unitext.UnitextService.Extension.GetFileNamesFromArchive(fileName, content);
            }
            catch(Exception ex) 
            {
            }
            return null;
        }
        /// <summary>
        /// Разархивировать файл архива (архив может быть ZIP, RAR, TAR и некоторые другие). 
        /// Сами файлы на диск не записываются - всё в памяти.
        /// </summary>
        /// <param name="fileName">имя архива</param>
        /// <param name="content">содержимое файла (если null, то fileName - ссылка на локальный файл)</param>
        /// <return>список файлов FileInArchive или null, если не поддержано</return>
        public static List<FileInArchive> GetFilesFromArchive(string fileName, byte[] content = null)
        {
            Pullenti.Unitext.FileFormat frm = FileFormatsHelper.AnalizeFileFormat(fileName, content);
            List<FileInArchive> res = new List<FileInArchive>();
            if (frm == Pullenti.Unitext.FileFormat.Zip || frm == Pullenti.Unitext.FileFormat.Docx) 
            {
                using (Pullenti.Unitext.Internal.Misc.MyZipFile zip = new Pullenti.Unitext.Internal.Misc.MyZipFile(fileName, content)) 
                {
                    foreach (Pullenti.Unitext.Internal.Misc.MyZipEntry e in zip.Entries) 
                    {
                        if (e.IsDirectory || e.Encrypted || e.UncompressDataSize == 0) 
                            continue;
                        byte[] dat = e.GetData();
                        res.Add(new FileInArchive() { Key = e.Name, Content = dat });
                    }
                }
                return res;
            }
            try 
            {
                if (Pullenti.Unitext.UnitextService.Extension != null) 
                    return Pullenti.Unitext.UnitextService.Extension.GetFilesFromArchive(fileName, content);
            }
            catch(Exception ex) 
            {
            }
            return null;
        }
        /// <summary>
        /// Извлечь файл из архива (архив может быть ZIP, RAR, TAR и некоторые другие)
        /// </summary>
        /// <param name="fileName">имя архива</param>
        /// <param name="content">содержимое файла (если null, то fileName - ссылка на локальный файл)</param>
        /// <param name="internalName">внутреннее имя (то, которое возвращалось функцией GetFileNamesFromArchive)</param>
        /// <return>разархивированный байтовый поток</return>
        public static byte[] GetFileFromArchive(string fileName, byte[] content, string internalName)
        {
            Pullenti.Unitext.FileFormat frm = FileFormatsHelper.AnalizeFileFormat(fileName, content);
            if (frm == Pullenti.Unitext.FileFormat.Zip || frm == Pullenti.Unitext.FileFormat.Docx) 
            {
                using (Pullenti.Unitext.Internal.Misc.MyZipFile zip = new Pullenti.Unitext.Internal.Misc.MyZipFile(fileName, content)) 
                {
                    foreach (Pullenti.Unitext.Internal.Misc.MyZipEntry e in zip.Entries) 
                    {
                        if (e.IsDirectory || e.Encrypted || e.UncompressDataSize == 0) 
                            continue;
                        if (e.Name != internalName) 
                            continue;
                        return e.GetData();
                    }
                }
            }
            try 
            {
                if (Pullenti.Unitext.UnitextService.Extension != null) 
                    return Pullenti.Unitext.UnitextService.Extension.GetFileFromArchive(fileName, content, internalName);
            }
            catch(Exception ex) 
            {
            }
            return null;
        }
    }
}