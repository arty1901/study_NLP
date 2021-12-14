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

namespace Pullenti.Unitext.Internal.Rtf
{
    /// <summary>
    /// Простейший элемент структуры файла RTF
    /// </summary>
    class RtfItem
    {
        public RftItemTyp Typ = RftItemTyp.Undefined;
        public string Text;
        public byte[] Codes;
        public int Level;
        public override string ToString()
        {
            if (Typ == RftItemTyp.BracketOpen) 
                return string.Format("{0}: {{", Level);
            if (Typ == RftItemTyp.BracketClose) 
                return string.Format("{0}: }}", Level);
            if (Typ == RftItemTyp.Command) 
                return string.Format("{0}: \\{1}", Level, Text ?? "");
            if (Typ == RftItemTyp.Image) 
                return string.Format("{0}: Image {1}: Len={2}", Level, Text ?? "", Codes.Length);
            if (Typ == RftItemTyp.Text) 
            {
                if (Text != null) 
                    return string.Format("{0}: [{1}] '{2}'", Level, Text.Length, (Text.Length > 200 ? Text.Substring(0, 200) : Text));
                if (Codes != null) 
                {
                    StringBuilder tmp = new StringBuilder();
                    tmp.AppendFormat("{0}: [{1}]", Level, Codes.Length);
                    foreach (byte c in Codes) 
                    {
                        tmp.AppendFormat("'{0}' ", ((int)c).ToString("X02"));
                        if (tmp.Length > 200) 
                        {
                            tmp.Append("...");
                            break;
                        }
                    }
                    return tmp.ToString();
                }
            }
            return "?";
        }
        class RftCommand
        {
            public string Command;
            public byte CharCode;
            public char PureChar;
            public byte[] PictContent;
            public int PicWidth;
            public int PicHeight;
            RftCommand m_TempCmd;
            public bool Parse(Stream stream)
            {
                Command = null;
                CharCode = 0;
                PureChar = (char)0;
                PictContent = null;
                int i = stream.ReadByte();
                if (i < 0) 
                    return false;
                char ch = (char)i;
                if (ch == '\'') 
                {
                    byte[] buf = new byte[(int)2];
                    if (stream.Read(buf, 0, 2) != 2) 
                        return false;
                    for (i = 0; i < 2; i++) 
                    {
                        ch = (char)buf[i];
                        if (ch >= '0' && ch <= '9') 
                            buf[i] = (byte)((ch - '0'));
                        else if (ch >= 'a' && ch <= 'f') 
                            buf[i] = (byte)(((ch - 'a') + 10));
                        else if (ch >= 'A' && ch <= 'F') 
                            buf[i] = (byte)(((ch - 'A') + 10));
                        else 
                            return false;
                    }
                    CharCode = (byte)(((buf[0] << 4) | buf[1]));
                    return true;
                }
                if (ch == 'u') 
                {
                    int cod = 0;
                    i = stream.ReadByte();
                    if (i < 0) 
                        return false;
                    char ch1 = (char)i;
                    if (char.IsDigit(ch1)) 
                    {
                        cod = (int)((ch1 - '0'));
                        while (true) 
                        {
                            i = stream.ReadByte();
                            if (i < 0) 
                                break;
                            ch1 = (char)i;
                            if (char.IsDigit(ch1)) 
                                cod = (cod * 10) + ((int)((ch1 - '0')));
                            else 
                            {
                                if (ch1 == '\\') 
                                    stream.Position--;
                                break;
                            }
                        }
                        PureChar = (char)cod;
                        if (m_TempCmd == null) 
                            m_TempCmd = new RftCommand();
                        if (ch1 == '\\') 
                        {
                            long pos = stream.Position;
                            stream.Position++;
                            m_TempCmd.Parse(stream);
                            if (m_TempCmd.CharCode != 0) 
                            {
                            }
                            else 
                                stream.Position = pos;
                        }
                        return true;
                    }
                    else 
                        stream.Position--;
                }
                if ("{}\\".IndexOf(ch) >= 0) 
                {
                    PureChar = ch;
                    return true;
                }
                if (ch == 0xD || ch == 0xA) 
                {
                    PureChar = '\n';
                    return true;
                }
                StringBuilder cmd = new StringBuilder();
                cmd.Append(ch);
                if (ch != '*') 
                {
                    while (true) 
                    {
                        if (cmd.Length == 1) 
                        {
                            if (ch == '~' || ch == '_') 
                                break;
                        }
                        i = stream.ReadByte();
                        if (i < 0) 
                            break;
                        ch = (char)i;
                        if ((ch == '\\' || ch == '}' || ch == '{') || i >= 0x80) 
                        {
                            stream.Position--;
                            break;
                        }
                        if (char.IsWhiteSpace(ch)) 
                            break;
                        cmd.Append(ch);
                    }
                }
                Command = cmd.ToString();
                if (Command == "tab") 
                    PureChar = '\t';
                else if (Command == "emdash") 
                    PureChar = (char)0x2014;
                else if (Command == "endash") 
                    PureChar = (char)0x2013;
                else if (Command == "emspace" || Command == "enspace" || Command == "~") 
                    PureChar = ' ';
                else if (Command == "bullet") 
                    PureChar = (char)0x2022;
                else if (Command == "lquote") 
                    PureChar = (char)0x2018;
                else if (Command == "rquote") 
                    PureChar = (char)0x2019;
                else if (Command == "ldblquote") 
                    PureChar = (char)0x201C;
                else if (Command == "rdblquote") 
                    PureChar = (char)0x201D;
                else if (Command == "lquote") 
                    PureChar = (char)0x2018;
                else if (Command == "_") 
                    PureChar = '-';
                else if (Command.Length > 2 && Command[0] == 'u' && char.IsDigit(Command[1])) 
                {
                    if (int.TryParse(Command.Substring(1), out i)) 
                    {
                        PureChar = (char)i;
                        return true;
                    }
                }
                if ((Command.StartsWith("colortbl") || Command.StartsWith("info") || Command.StartsWith("object")) || Command.StartsWith("pict") || Command.StartsWith("themedata")) 
                {
                    int lev = 0;
                    bool isPict = Command.StartsWith("pict") || Command.StartsWith("object");
                    RftCommand pictComm = new RftCommand();
                    StringBuilder hexVal = new StringBuilder();
                    int picW = 0;
                    int picGoalW = 0;
                    int picH = 0;
                    int picGoalH = 0;
                    int scalx = 100;
                    int scaly = 100;
                    while (true) 
                    {
                        i = stream.ReadByte();
                        if (i < 0) 
                            break;
                        ch = (char)i;
                        if (ch == '}') 
                        {
                            if (lev == 0) 
                            {
                                stream.Position--;
                                break;
                            }
                            else 
                                lev--;
                        }
                        else if (ch == '{') 
                            lev++;
                        else if (ch == '\\') 
                        {
                            if (pictComm.Parse(stream)) 
                            {
                                if (pictComm.Command != null) 
                                {
                                    if (pictComm.Command.StartsWith("picwgoal")) 
                                        int.TryParse(pictComm.Command.Substring(8), out picGoalW);
                                    else if (pictComm.Command.StartsWith("pichgoal")) 
                                        int.TryParse(pictComm.Command.Substring(8), out picGoalH);
                                    else if (pictComm.Command.StartsWith("picw") || pictComm.Command.StartsWith("objw")) 
                                        int.TryParse(pictComm.Command.Substring(4), out picW);
                                    else if (pictComm.Command.StartsWith("pich") || pictComm.Command.StartsWith("objh")) 
                                        int.TryParse(pictComm.Command.Substring(4), out picH);
                                    else if (pictComm.Command.StartsWith("picscalex")) 
                                        int.TryParse(pictComm.Command.Substring(9), out scalx);
                                    else if (pictComm.Command.StartsWith("picscaley")) 
                                        int.TryParse(pictComm.Command.Substring(9), out scaly);
                                    else if (pictComm.Command.StartsWith("bin")) 
                                    {
                                        int blen;
                                        if (int.TryParse(pictComm.Command.Substring(3), out blen)) 
                                        {
                                            if ((stream.Position + blen) <= stream.Length) 
                                            {
                                                if (isPict) 
                                                {
                                                    PictContent = new byte[(int)blen];
                                                    stream.Read(PictContent, 0, blen);
                                                }
                                                else 
                                                    stream.Position += blen;
                                            }
                                        }
                                    }
                                    if (isPict && pictComm.PictContent != null) 
                                        PictContent = pictComm.PictContent;
                                }
                            }
                        }
                        else if (lev == 0 && isPict && char.IsLetterOrDigit(ch)) 
                            hexVal.Append(ch);
                    }
                    if (isPict && hexVal.Length > 10 && PictContent == null) 
                    {
                        PictContent = new byte[(int)(hexVal.Length / 2)];
                        string str = hexVal.ToString();
                        for (int ii = 0, p = 0; ii < (hexVal.Length - 1); ii += 2,p++) 
                        {
                            byte bb = (byte)0;
                            for (int jj = 0; jj < 2; jj++) 
                            {
                                char chh = str[ii + jj];
                                int v = 0;
                                if (chh >= '0' && chh <= '9') 
                                    v = chh - '0';
                                else if (chh >= 'a' && chh <= 'f') 
                                    v = 10 + ((int)((chh - 'a')));
                                else if (chh >= 'A' && chh <= 'F') 
                                    v = 10 + ((int)((chh - 'A')));
                                else 
                                    break;
                                bb = (byte)(((bb << 4) | v));
                            }
                            PictContent[p] = bb;
                        }
                    }
                    if (isPict && ((picW > 0 || picGoalW > 0)) && ((picH > 0 || picGoalH > 0))) 
                    {
                        if (picGoalW <= 0 || picGoalH <= 0) 
                        {
                            PicWidth = (int)((0.75 * PicWidth));
                            PicHeight = (int)((0.75 * PicHeight));
                        }
                        else 
                        {
                            PicWidth = picGoalW / 20;
                            PicHeight = picGoalH / 20;
                        }
                        if (scalx > 0) 
                            PicWidth = (PicWidth * scalx) / 100;
                        if (scaly > 0) 
                            PicHeight = (PicHeight * scaly) / 100;
                    }
                    return true;
                }
                return true;
            }
        }

        public static List<RtfItem> ParseList(Stream stream)
        {
            List<RtfItem> res = new List<RtfItem>();
            StringBuilder txtBuf = new StringBuilder();
            List<byte> codBuf = new List<byte>();
            RftCommand command = new RftCommand();
            while (stream.Position < stream.Length) 
            {
                int i = stream.ReadByte();
                if (i < 0) 
                    break;
                char ch = (char)i;
                RtfItem newItem = null;
                byte newCode = (byte)0;
                char newChar = (char)0;
                if (ch == '{') 
                    newItem = new RtfItem() { Typ = RftItemTyp.BracketOpen };
                else if (ch == '}') 
                    newItem = new RtfItem() { Typ = RftItemTyp.BracketClose };
                else if (ch == '\\') 
                {
                    if (!command.Parse(stream)) 
                        continue;
                    if (command.CharCode != 0) 
                        newCode = command.CharCode;
                    else if (command.PureChar != 0) 
                        newChar = command.PureChar;
                    else if (command.PictContent != null) 
                        newItem = new RtfItemImage() { Typ = RftItemTyp.Image, Width = command.PicWidth, Height = command.PicHeight, Text = command.Command, Codes = command.PictContent };
                    else if (command.Command != null) 
                        newItem = new RtfItem() { Typ = RftItemTyp.Command, Text = command.Command };
                }
                else 
                {
                    if (ch == 0xD || ch == 0xA) 
                        continue;
                    if (((int)ch) < 0x80) 
                        newChar = ch;
                    else 
                        newCode = (byte)ch;
                }
                if (newItem != null || newCode != 0) 
                {
                    if (txtBuf.Length > 0) 
                    {
                        res.Add(new RtfItem() { Typ = RftItemTyp.Text, Text = txtBuf.ToString() });
                        txtBuf.Length = 0;
                    }
                }
                if (newItem != null || newChar != 0) 
                {
                    if (codBuf.Count > 0) 
                    {
                        res.Add(new RtfItem() { Typ = RftItemTyp.Text, Codes = codBuf.ToArray() });
                        codBuf.Clear();
                    }
                }
                if (newItem != null) 
                    res.Add(newItem);
                if (newCode != 0) 
                    codBuf.Add(newCode);
                if (newChar != 0) 
                    txtBuf.Append(newChar);
            }
            if (txtBuf.Length > 0) 
            {
                res.Add(new RtfItem() { Typ = RftItemTyp.Text, Text = txtBuf.ToString() });
                txtBuf.Length = 0;
            }
            if (codBuf.Count > 0) 
            {
                res.Add(new RtfItem() { Typ = RftItemTyp.Text, Codes = codBuf.ToArray() });
                codBuf.Clear();
            }
            int lev = 0;
            foreach (RtfItem it in res) 
            {
                it.Level = lev;
                if (it.Typ == RftItemTyp.BracketOpen) 
                    lev++;
                else if (it.Typ == RftItemTyp.BracketClose) 
                {
                    lev--;
                    it.Level = lev;
                }
            }
            return res;
        }
    }
}