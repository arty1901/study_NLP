/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Text;

namespace Pullenti.Unitext.Internal.Zip
{
    /// <summary>
    /// ZipNameTransform transforms names as per the Zip file naming convention.
    /// </summary>
    class ZipNameTransform : INameTransform
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ZipNameTransform"></see>
        /// </summary>
        /// <param name="trimPrefix">The string to trim from the front of paths if found.</param>
        public ZipNameTransform(string trimPrefix = null)
        {
            TrimPrefix = trimPrefix;
        }
        /// <summary>
        /// Static constructor.
        /// </summary>
        static ZipNameTransform()
        {
            char[] invalidPathChars;
            invalidPathChars = new char[(int)0];
            int howMany = invalidPathChars.Length + 2;
            InvalidEntryCharsRelaxed = new char[(int)howMany];
            Array.Copy(invalidPathChars, 0, InvalidEntryCharsRelaxed, 0, invalidPathChars.Length);
            InvalidEntryCharsRelaxed[howMany - 1] = '*';
            InvalidEntryCharsRelaxed[howMany - 2] = '?';
            howMany = invalidPathChars.Length + 4;
            InvalidEntryChars = new char[(int)howMany];
            Array.Copy(invalidPathChars, 0, InvalidEntryChars, 0, invalidPathChars.Length);
            InvalidEntryChars[howMany - 1] = ':';
            InvalidEntryChars[howMany - 2] = '\\';
            InvalidEntryChars[howMany - 3] = '*';
            InvalidEntryChars[howMany - 4] = '?';
        }
        /// <summary>
        /// Transform a windows directory name according to the Zip file naming conventions.
        /// </summary>
        /// <param name="name">The directory name to transform.</param>
        /// <return>The transformed name.</return>
        public string TransformDirectory(string name)
        {
            name = this.TransformFile(name);
            if (name.Length > 0) 
            {
                if (!name.EndsWith("/")) 
                    name += "/";
            }
            else 
                throw new Exception("Cannot have an empty directory name");
            return name;
        }
        /// <summary>
        /// Transform a windows file name according to the Zip file naming conventions.
        /// </summary>
        /// <param name="name">The file name to transform.</param>
        /// <return>The transformed name.</return>
        public string TransformFile(string name)
        {
            if (name != null) 
            {
                string lowerName = name.ToLower();
                if ((trimPrefix_ != null) && (lowerName.IndexOf(trimPrefix_) == 0)) 
                    name = name.Substring(trimPrefix_.Length);
                name = name.Replace("\\", "/");
                while ((name.Length > 0) && (name[0] == '/')) 
                {
                    name = name.Substring(1);
                }
                while ((name.Length > 0) && (name[name.Length - 1] == '/')) 
                {
                    name = name.Substring(0, name.Length - 1);
                }
                int index = name.IndexOf("//");
                while (index >= 0) 
                {
                    name = name.Substring(0, index) + name.Substring(index + 1);
                    index = name.IndexOf("//");
                }
                name = MakeValidName(name, '_');
            }
            else 
                name = string.Empty;
            return name;
        }
        /// <summary>
        /// Get/set the path prefix to be trimmed from paths if present.
        /// </summary>
        public string TrimPrefix
        {
            get
            {
                return trimPrefix_;
            }
            set
            {
                trimPrefix_ = value;
                if (trimPrefix_ != null) 
                    trimPrefix_ = trimPrefix_.ToLower();
            }
        }
        /// <summary>
        /// Force a name to be valid by replacing invalid characters with a fixed value
        /// </summary>
        /// <param name="name">The name to force valid</param>
        /// <param name="replacement">The replacement character to use.</param>
        /// <return>Returns a valid name</return>
        static string MakeValidName(string name, char replacement)
        {
            int index = name.IndexOfAny(InvalidEntryChars);
            if (index >= 0) 
            {
                StringBuilder builder = new StringBuilder(name);
                while (index >= 0) 
                {
                    builder[index] = replacement;
                    if (index >= name.Length) 
                        index = -1;
                    else 
                        index = name.IndexOfAny(InvalidEntryChars, index + 1);
                }
                name = builder.ToString();
            }
            if (name.Length > 0xffff) 
                throw new Exception("PathTooLong");
            return name;
        }
        /// <summary>
        /// Test a name to see if it is a valid name for a zip entry.
        /// </summary>
        /// <param name="name">The name to test.</param>
        /// <param name="relaxed">If true checking is relaxed about windows file names and absolute paths.</param>
        /// <return>Returns true if the name is a valid zip name; false otherwise.</return>
        public static bool IsValidNameEx(string name, bool relaxed)
        {
            bool result = name != null;
            if (result) 
            {
                if (relaxed) 
                    result = name.IndexOfAny(InvalidEntryCharsRelaxed) < 0;
                else 
                    result = ((name.IndexOfAny(InvalidEntryChars) < 0)) && (name.IndexOf('/') != 0);
            }
            return result;
        }
        /// <summary>
        /// Test a name to see if it is a valid name for a zip entry.
        /// </summary>
        /// <param name="name">The name to test.</param>
        /// <return>Returns true if the name is a valid zip name; false otherwise.</return>
        public static bool IsValidName(string name)
        {
            bool result = (name != null) && ((name.IndexOfAny(InvalidEntryChars) < 0)) && (name.IndexOf('/') != 0);
            return result;
        }
        string trimPrefix_;
        static char[] InvalidEntryChars;
        static char[] InvalidEntryCharsRelaxed;
    }
}