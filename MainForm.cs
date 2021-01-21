using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CanonTxtCvt
{
    public partial class MainForm : Form
    {
        Convert wpCvt = null;       // Canon WP conversion routines object
        String currentPath = null;  // Current Canon .TXT file path
        String appName = null;

        public MainForm()
        {
            InitializeComponent();

            wpCvt = new Convert(rtxt: rtxtDoc);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Setup rich text control
            rtxtDoc.WordWrap = true;

            // Save app title string
            appName = this.Text;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox dlg = new AboutBox();
            dlg.ShowDialog();
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

            // Scroll to end of log
            txtLog.Select(txtLog.TextLength, 0);
            txtLog.ScrollToCaret();
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

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Select file
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                //openFileDialog.RestoreDirectory = true;
                openFileDialog.Filter = "TXT files (*.TXT)|*.TXT|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    currentPath = openFileDialog.FileName;
                }
                else
                    return;
            }
 
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;
            btnClear.Enabled = false;

            Log("Converting file: " + currentPath);

            // Load file -> rtxDoc
            String res = wpCvt.LoadCanonFile(this, currentPath);
            if (!String.IsNullOrEmpty(res))
                Log(res);

            UpdateTitleBar();

            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
            btnClear.Enabled = true;
        }

        /// <summary>
        /// Update title bar with name of current documet
        /// </summary>
        private void UpdateTitleBar()
        {
            string fileName = Path.GetFileName(currentPath);

            this.Text = appName + " - " + fileName;
        }

        private void saveRTFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(currentPath))
            {
                MessageBox.Show("No document loaded.");
                return;
            }

            // Default save path = input file w/.rtf extension
            String savePath = Path.ChangeExtension(currentPath, "rtf");

            rtxtDoc.SaveFile(savePath);
        }

        /// <summary>
        /// Prompt for filename and let user save (still RTF)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(currentPath))
            {
                MessageBox.Show("No document loaded.");
                return;
            }

            // Default save path = input file w/.rtf extension
            String savePath = Path.ChangeExtension(currentPath, "rtf");

            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "RTF files (*.RTF)|*.RTF";
                fileDialog.FilterIndex = 1;
                fileDialog.FileName = Path.GetFileName(savePath);
                fileDialog.DefaultExt = "rtf";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    currentPath = fileDialog.FileName;
                    rtxtDoc.SaveFile(currentPath);
                    UpdateTitleBar();
                }
            }
        }

        private void convertDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Select directory of files
            String dir = @"C:\Users\rtaylor\Desktop\Canon WP Diskettes\Diskettes\Bishop Nominations\Files";
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    dir = fbd.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            Log("Converting directory: " + dir);

            // Loop through .txt files in directory
            string[] fileEntries = Directory.GetFiles(dir, "*.txt");
            foreach (string filePath in fileEntries)
            {
                currentPath = filePath;

                Log("Converting file: " + currentPath);

                // Load/convert file -> rtxDoc
                String res = wpCvt.LoadCanonFile(this, currentPath);
                if (!String.IsNullOrEmpty(res))
                    Log(res);

                UpdateTitleBar();

                String savePath = Path.ChangeExtension(currentPath, "rtf");
                rtxtDoc.SaveFile(savePath);
            }

        }
    }
}
