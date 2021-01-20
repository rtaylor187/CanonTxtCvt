using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonTxtCvt
{
    public partial class MainForm : Form
    {
        Convert wpCvt = null;       // Canon WP conversion routines object
        String currentFile = null;  // Current Canon .TXT file path

        public MainForm()
        {
            InitializeComponent();

            wpCvt = new Convert(rtxt: rtxtDoc);

            // Setup rich text control
            rtxtDoc.WordWrap = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO:  Show About dialog box
        }


        //-------------------------------------------------------
        // Logging

        /// <summary>
        /// Add line of text to log
        /// </summary>
        /// <param name="logText"></param>
        public void Log(String logText)
        {
            if (txtLog.TextLength != 0)
                txtLog.Text += Environment.NewLine;

            txtLog.Text += logText;
        }

        public void HexLog(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null)
            {
                Log("<null>");
                return;
            }

            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }

            Log(result.ToString());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }


        //-------------------------------------------------------
        // Operations

        private void convertFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Select file
            var filePath = string.Empty;

            // static name for testing
            //currentFile = @"C:\Users\rtaylor\Desktop\Canon WP Diskettes\Test Diskette 3\Files\Symbols.txt";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                //openFileDialog.RestoreDirectory = true;
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    currentFile = openFileDialog.FileName;
                }
            }
 
            Log("Converting file: " + currentFile);

            // Load file -> rtxDoc
            String res = wpCvt.LoadCanonFile(this, currentFile);
            if (!String.IsNullOrEmpty(res))
                Log(res);
        }

        private void convertDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Select directory
            String dir = @"C:\Users\rtaylor\Desktop\Canon WP Diskettes\Diskettes\Bishop Nominations\Files";

            Log("Converting directory: " + dir);

            // TODO:  Validate that directory is Canon StarLink WP diskette

            // TODO:  Loop through .txt files in directory
            string[] fileEntries = Directory.GetFiles(dir, "*.txt");
            foreach (string filePath in fileEntries)
            {
                currentFile = filePath;

                Log("Converting file: " + currentFile);

                // Convert file -> rtxDoc
                String res = wpCvt.LoadCanonFile(this, currentFile);
                if (!String.IsNullOrEmpty(res))
                    Log(res);

                // TODO:  Save rtxDoc -> .RTF
            }
        }
    }
}
