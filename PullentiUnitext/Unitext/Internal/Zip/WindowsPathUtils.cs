/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;

namespace Pullenti.Unitext.Internal.Zip
{
    // WindowsPathUtils provides simple utilities for handling windows paths.
    abstract class WindowsPathUtils
    {
        internal WindowsPathUtils()
        {
        }
        public static string DropPathRoot(string path)
        {
            string result = path;
            if ((path != null) && (path.Length > 0)) 
            {
                if ((path[0] == '\\') || (path[0] == '/')) 
                {
                    if ((path.Length > 1) && (((path[1] == '\\') || (path[1] == '/')))) 
                    {
                        int index = 2;
                        int elements = 2;
                        while ((index <= path.Length) && (((((path[index] != '\\') && (path[index] != '/'))) || ((--elements) > 0)))) 
                        {
                            index++;
                        }
                        index++;
                        if (index < path.Length) 
                            result = path.Substring(index);
                        else 
                            result = "";
                    }
                }
                else if ((path.Length > 1) && (path[1] == ':')) 
                {
                    int dropCount = 2;
                    if ((path.Length > 2) && (((path[2] == '\\') || (path[2] == '/')))) 
                        dropCount = 3;
                    result = result.Substring(dropCount);
                }
            }
            return result;
        }
    }
}