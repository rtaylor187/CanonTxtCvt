using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonTxtCvt
{
    class Convert
    {
        static MainForm form;
        static RichTextBox rTxtCtrl;

        public Convert(RichTextBox rtxt)
        {
            rTxtCtrl = rtxt;
        }

        public String LoadCanonFile(MainForm form, string filePath)
        {
            Convert.form = form;

            // Validiate file
            if (!File.Exists(filePath))
                return "Convert.LoadCanonFile() - ERROR - File doesn't exist.";

            // Canon WP file load -> rTxtCtrl
            //
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

            // 
            String hdrMisc = BitConverter.ToString(fileBytes, 0x80, 136);
            byte[] docHdrMisc = new byte[136];
            Array.Copy(fileBytes, 0x80, docHdrMisc, 0, 136);

            // TXT Document Data 
            //    fileBytes[0x264 : (txtlen - 0x108)]
            //
            byte[] docText = new byte[txtLen];
            Array.Copy(fileBytes, 0x108, docText, 0, txtLen - 0x88);

            return ParseText(docText, docHdrMisc);
        }

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
                                //  2 = 0x11 block - format: paragraph & page break
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
                                    // Seen @ end of pre-text 0x10 block - ignore
                                    break;

                                case 0x10:      // end of 0x10 block -> start
                                    state = 0;
                                    break;

                                case (byte)' ': // Type face change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO
                                    break;

                                case (byte)'0': // Type size change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO
                                    break;

                                case (byte)'2': // Type expand/condense change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO
                                    break;

                                case (byte)'@': // Underline change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO
                                    break;

                                case (byte)'B': // Bold change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO
                                    break;

                                case (byte)'D': // Italic change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO
                                    break;

                                case (byte)'F': // Outline change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO
                                    break;

                                case (byte)'H': // Super/Sub-script change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO
                                    break;

                                case (byte)'J': // Shading change
                                    fmVal = docText[pos++];
                                    toVal = docText[pos++];
                                    closer = docText[pos++];
                                    checkClose = true;

                                    // TODO - unsupported?
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
                                // TODO
                                break;

                            case 0x84:  // Paragraph - center justified
                                rTxtCtrl.Text += "\r\n";
                                // TODO
                                break;

                            case 0x85:  // Paragraph - right justified
                                rTxtCtrl.Text += "\r\n";
                                // TODO
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
                                rTxtCtrl.Text += Characters.GetChar(0x13, bytes, form);
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

            return "  - Done";
        }
    }
}
