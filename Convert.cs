using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonTxtCvt
{
    enum Face
    {
        Courier = 1,    // monospace (default)
        Script = 2,     // script
        Swiss = 3,      // Helvetica
        Dutch = 4,      // Times Roman
        Humanist = 5    // Calibri? Carlito? Corbel?
    }
    enum Compress
    {
        Normal = 0,
        Extended = 1,
        Condensed = 2
    }

    enum Baseline
    {
        Regular = 0,
        Superscript = 1,
        Subscript = 2
    }

    enum Shading
    {
        None = 1,
        Shading1 = 2,
        Shading2 = 3,
        Shading3 = 4,
        Shading4 = 5,
        Shading5 = 6,
    }

    enum ParagraphJust
    {
        Left = 0,
        Center,
        Right,
        Indent,
        Full
    }


    class StyleInfo
    {
        public Face typeFace;
        public ushort typeSize;
        public bool isUnderline;
        public bool isBold;
        public bool isItalic;

        public ParagraphJust justification;

        public bool isOutline;
        public Shading shade;
        public Compress typeCompress;

        public Baseline typeBaseline;
        public float lineSpacing;      // 0.75, 1.0, 1.5, 2.0, 2.5 or 3.0


        /// <summary>
        /// Default settings constructor
        /// </summary>
        public StyleInfo()
        {
            typeFace = Face.Courier;
            typeSize = 12;
            typeCompress = Compress.Normal;
            typeBaseline = Baseline.Regular;
            isUnderline = false;
            isBold = false;
            isItalic = false;
            isOutline = false;
            shade = Shading.None;
            justification = ParagraphJust.Left;
            lineSpacing = 1.0f;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="style"></param>
        public StyleInfo(StyleInfo style)
        {
            typeFace = style.typeFace;
            typeSize = style.typeSize;
            typeCompress = style.typeCompress;
            typeBaseline = style.typeBaseline;
            isUnderline = style.isUnderline;
            isBold = style.isBold;
            isItalic = style.isItalic;
            isOutline = style.isOutline;
            shade = style.shade;
            justification = style.justification;
            lineSpacing = style.lineSpacing;
        }

        public FontStyle getFontStyle()
        {
            FontStyle style = FontStyle.Regular;
            if (isBold) style |= FontStyle.Bold;
            if (isItalic) style |= FontStyle.Italic;
            if (isUnderline) style |= FontStyle.Underline;

            return style;
        }

        public String getFontName()
        {
            string fontName;
            switch (typeFace)
            {
                default:
                case Face.Courier: fontName = "Courier New"; break;

                case Face.Script: fontName = "Segoe Script"; break;
                case Face.Swiss: fontName = "Arial"; break;
                case Face.Dutch: fontName = "Times New Roman"; break;
                case Face.Humanist: fontName = "Carlito"; break;
            }

            return fontName;
        }
    }

    class Convert
    {
        MainForm form;
        RichTextBox rTxtCtrl;

        SortedList<int, StyleInfo> styleSet = new SortedList<int, StyleInfo>();

        public Convert(RichTextBox rtxt)
        {
            rTxtCtrl = rtxt;
        }

        /// <summary>
        /// Load Canon WP file -> rTxtCtrl
        /// </summary>
        /// <param name="form"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public String LoadCanonFile(MainForm form, string filePath)
        {
            this.form = form;

            // Validiate file
            if (!File.Exists(filePath))
                return "Convert.LoadCanonFile() - ERROR - File doesn't exist.";

            // Reset rich-text control
            rTxtCtrl.Clear();

            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Canon File Header - common to all StarWriter files
            //
            //  00      short       zeros1      0x0000
            //  02      byte[5]     canon       "Canon"
            //  07      byte[4]     str1        "ETW1"
            //  0b      short       zeroes2     0x0000
            //  0d      byte[24]    str2/3/4    "V021    CBS1A01 TLCS-900"
            //  25      int         dataLen     0xXXXXXXXX
            //
            short zeros1 = BitConverter.ToInt16(fileBytes, 0x00);
            short zeros2 = BitConverter.ToInt16(fileBytes, 0x0b);
            int dataLen = BitConverter.ToInt32(fileBytes, 0x25);

            String canon = Encoding.UTF8.GetString(fileBytes, 2, 5);
            String str1 = Encoding.UTF8.GetString(fileBytes, 7, 4);
            String str2 = Encoding.UTF8.GetString(fileBytes, 0x0d, 4);
            String str3 = Encoding.UTF8.GetString(fileBytes, 0x15, 7);
            String str4 = Encoding.UTF8.GetString(fileBytes, 0x1d, 8);

            // TXT Document Header
            //
            //   29     byte            val1        0x9F
            //   2A     byte[12]        zeros3      
            //   36     char[8]         str5        "VS180   "
            //   3E     byte[3]         zeros4      0x000000
            //   41     char[8]         fileName    "????????"
            //   49     char[3]         fileExt     "TXT"
            //   4C     char[10]        fileDate    "??????????"
            //   56     char[36]        descrip     "???..."
            //   7A     short           val2        0x5b17          - magic for "text document"?
            //   7C     int             txtLen      0xXXXXXXXX
            //   80     byte[136]       docHdrMisc  unknown header data
            //  108     byte[txtLen-136]  document data (starts w/0x12)

            byte val1 = fileBytes[0x29];

            byte[] zeros3 = new byte[12];
            Array.Copy(fileBytes, 0x2A, zeros3, 0, 12);

            String str5 = Encoding.UTF8.GetString(fileBytes, 0x36, 8);
            String fileName = Encoding.UTF8.GetString(fileBytes, 0x41, 8);
            String fileExt = Encoding.UTF8.GetString(fileBytes, 0x49, 3);
            String fileDate = Encoding.UTF8.GetString(fileBytes, 0x4C, 10);
            String descrip = Encoding.UTF8.GetString(fileBytes, 0x56, 36);

            short val2 = BitConverter.ToInt16(fileBytes, 0x7A);
            int txtLen = BitConverter.ToInt32(fileBytes, 0x7C);

            String hdrMisc = BitConverter.ToString(fileBytes, 0x80, 136);
            byte[] docHdrMisc = new byte[136];
            Array.Copy(fileBytes, 0x80, docHdrMisc, 0, 136);

            // TXT Document Data 
            //    fileBytes[0x264 : (txtlen - 0x108)]
            //
            byte[] docText = new byte[txtLen];
            Array.Copy(fileBytes, 0x108, docText, 0, txtLen - 0x88);

            // start w/default style
            styleSet.Clear();
            styleSet.Add(0, new StyleInfo());

            return ParseText(docText, docHdrMisc);
        }

        //-----------------------------------------------------------
        // Styles list management
        //

        private void setTypeface(Face face)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.typeFace = face;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.typeFace = face;
            }
        }

        private void setTypeSize(ushort points)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.typeSize = points;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.typeSize = points;
            }
        }

        private void setTypeCompress(Compress comp)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.typeCompress = comp;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.typeCompress = comp;
            }
        }

        private void setTypeBaseline(Baseline baseline)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.typeBaseline = baseline;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.typeBaseline = baseline;
            }
        }

        private void setBold(bool bold)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.isBold = bold;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.isBold = bold;
            }
        }

        private void setItalic(bool italic)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.isItalic = italic;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.isItalic = italic;
            }
        }

        private void setUnderline(bool underline)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.isUnderline = underline;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.isUnderline = underline;
            }
        }

        private void setOutline(bool outline)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.isOutline = outline;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.isOutline = outline;
            }
        }

        private void setShade(Shading shade)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.shade = shade;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.shade = shade;
            }
        }

        private void setJustify(ParagraphJust justify)
        {
            if (justify != styleSet.Last().Value.justification && rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.justification = justify;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.justification = justify;
            }
        }

        private void setLineSpace(float lineSpace)
        {
            if (rTxtCtrl.TextLength > styleSet.Last().Key)
            {
                StyleInfo newStyle = new StyleInfo(styleSet.Last().Value);
                newStyle.lineSpacing = lineSpace;
                styleSet.Add(rTxtCtrl.TextLength, newStyle);
            }
            else
            {
                styleSet.Last().Value.lineSpacing = lineSpace;
            }
        }

        private void ApplyStyles()
        {
            int startPos = 0;
            StyleInfo lastStyle = styleSet.First().Value;

            bool first = true;
            foreach (int endPos in styleSet.Keys)
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                // Apply style to previous segment
                rTxtCtrl.Select(startPos, endPos - startPos);

                StyleRange(startPos, endPos, lastStyle);

                // Set for next segment
                startPos = endPos;
                lastStyle = styleSet[endPos];
            }

            // Apply style to last segment
            StyleRange(startPos, rTxtCtrl.TextLength, lastStyle);

            // Reset insertion point to start
            rTxtCtrl.Select(0, 0);
        }

        private void StyleRange(int startPos, int endPos, StyleInfo style)
        {
            rTxtCtrl.Select(startPos, endPos - startPos);

            System.Drawing.Font newFont = new System.Drawing.Font(style.getFontName(), style.typeSize, style.getFontStyle());
            rTxtCtrl.SelectionFont = newFont;

            HorizontalAlignment hAlign = HorizontalAlignment.Left;
            if (style.justification == ParagraphJust.Center)
                hAlign = HorizontalAlignment.Center;
            else if (style.justification == ParagraphJust.Right)
                hAlign = HorizontalAlignment.Right;
            rTxtCtrl.SelectionAlignment = hAlign;
        }


        //-----------------------------------------------------------
        // Main document parse routine
        //

        /// <summary>
        /// Parse formatted text of body to RichTextBox control
        /// </summary>
        /// <param name="docText"></param>
        /// <returns></returns>
        public String ParseText(byte[] docText, byte[] docHdr)
        {
                                // -2 = EOF
                                // -1 = error
            short state = 0;    //  0 = start / between blocks
                                //  1 = 0x10 block - format: styles
                                //  2 = 0x11 block - format: tab, paragraph & page break
                                //  3 = 0x12 block - format: line spacing, justification (TODO - structure unknown)
                                //  4 = 0x13 block - text: base code page
                                //  5 = 0x14 block - text: symbol code page #1
                                //  6 = 0x15 block - text: symbol code page #2

            byte ch = 0;        // current char
            byte lastCh;        // last char

            int pos = 0;
            while (pos < docText.Length && state >= 0)
            {
                lastCh = ch;
                ch = docText[pos++];

                rTxtCtrl.SelectionStart = rTxtCtrl.TextLength;

                switch (state)
                {
                    case 0:     // start
                        switch (ch)
                        {
                            case 0x03:  // end of file
                                state = -2;
                                break;

                            case 0x10:  state = 1;  break;
                            case 0x11:  state = 2;  break;
                            case 0x12:  state = 3;  break;
                            case 0x13:  state = 4;  break;
                            case 0x14:  state = 5;  break;
                            case 0x15:  state = 6;  break;

                            default:
                                form.Log("Unknown block byte @ pos = 0x" + pos.ToString("X2") + ":  0x" + ch.ToString("X2"));
                                break;
                        }
                        break;

                    case 1:     // 0x10 block - format style
                        {
                            byte fmVal = 0, toVal = 0, closer = 0;
                            bool checkClose = false;

                            switch (ch)
                            {
                                case 0x02:
                                    // Seen @ end of pre-text 0x10 block - ignore...
                                    break;

                                case 0x10:      // end of 0x10 block -> start
                                    state = 0;
                                    break;

                                case (byte)' ': // Type face change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setTypeface((Face)toVal);
                                    break;

                                case (byte)'0': // Type size change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setTypeSize(toVal);
                                    break;

                                case (byte)'2': // Type expand/condense change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setTypeCompress((Compress)toVal);
                                    break;

                                case (byte)'@': // Underline change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setUnderline(toVal == 1);
                                    break;

                                case (byte)'B': // Bold change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setBold(toVal == 1);
                                     break;

                                case (byte)'D': // Italic change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setItalic(toVal == 1);
                                    break;

                                case (byte)'F': // Outline change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setOutline(toVal == 1);
                                    break;

                                case (byte)'H': // Super/Sub-script change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setTypeBaseline((Baseline)toVal);
                                    break;

                                case (byte)'J': // Shading change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    setShade((Shading)toVal);
                                    break;
                            }

                            if (checkClose && closer != (ch + 1))
                            {
                                // Log unexpected 0x10 format change pair
                                form.Log("ParseText() - unexpected 0x10 pair: \'" + (char)ch + "\'/\'" + (char)closer);
                            }
                        }
                        break;

                    case 2:     // 0x11 block - paragraph/page break/tab/?
                        switch (ch)
                        {
                            case 0x03:      // end of file
                                state = -2;     
                                break;
                            case 0x11:      // end of block -> start
                                state = 0;  
                                break;

                            case 0x22:  // Page break
                                        // TODO?
                                rTxtCtrl.Text += "\r\n----------------------------- Page Break -------------------------------\r\n";
                                break;

                            case 0x80:  // TAB
                                rTxtCtrl.Text += "\t";
                                // TODO
                                break;

                            case 0x81:  // Paragraph - left justified
                                rTxtCtrl.Text += "\r\n";
                                setJustify(ParagraphJust.Left);
                                break;

                            case 0x84:  // Paragraph - center justified
                                //rTxtCtrl.Text += "\r\n";
                                setJustify(ParagraphJust.Center);
                                break;

                            case 0x85:  // Paragraph - right justified
                                //rTxtCtrl.Text += "\r\n";
                                setJustify(ParagraphJust.Right);
                                break;

                            default:
                                // TODO:  Unknown format byte - log
                                break;
                        }
                        break;

                    case 3:     // 0x12 block - format change (TODO: structure unknown)
                        switch (ch)
                        {
                            case 0x12:      // end of block
                                state = 0;  
                                break;

                            default:
                                // TODO:  gather bytes for analysis
                                break;
                        }
                        break;

                    case 4:     // 0x13 block - text: base code page
                        switch (ch)
                        {
                            case 0x03:
                                state = -2;
                                break;

                            case 0x04:  // 4/x/y/5 -> special char
                                {
                                    byte[] pair = { docText[pos++], docText[pos++] };
 
                                    byte end5 = docText[pos++];
                                    // TODO: Validate end5 == 0x05

                                    rTxtCtrl.Text += Characters.GetChar(0x13, pair, form);
                                }
                                break;

                            case 0x13:      // end of block
                                state = 0;  
                                break;

                            default:
                                // Text character
                                byte[] bytes = { ch };
                                //rTxtCtrl.Text += Characters.GetChar(0x13, bytes, form);
                                rTxtCtrl.AppendText(Characters.GetChar(0x13, bytes, form));                          
                                break;
                        }
                        break;

                    case 5:     // 0x14 block - text: symbol code page #1
                        switch (ch)
                        {
                            case 0x14:      // end of block
                                state = 0;
                                break;

                            default:
                                // Text character
                                byte[] bytes = { ch };
                                rTxtCtrl.Text += Characters.GetChar(0x14, bytes, form);
                                break;
                        }
                        break;

                    case 6:     // 0x15 block - text: symbol code page #2
                        switch (ch)
                        {
                            case 0x15:      // end of block
                                state = 0;
                                break;

                            default:
                                // Text character
                                byte[] bytes = { ch };
                                rTxtCtrl.Text += Characters.GetChar(0x15, bytes, form);
                                break;
                        }
                        break;
                }
            }

            // TODO:  Apply styles to format text
            ApplyStyles();

            // TODO:  Add annotations ?

            return "  - Done";
        }
    }
}
