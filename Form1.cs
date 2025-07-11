using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace FontsInstaller
{
    public partial class MainForm : Form
    {
        public static int FontsInstalled = 0;

        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        private static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        public MainForm()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderDialog.SelectedPath;
                    try
                    {
                        LogMessage("Searching for font files in: " + folderPath, MessageType.Success);

                        DirectorySearch(folderPath);
                    }
                    catch (Exception ex)
                    {
                        LogMessage("Error: " + ex.Message, MessageType.Error);
                    }
                    finally
                    {
                        DisplayInstalledFontsNumber(FontsInstalled);
                    }
                }
            }
        }

        private void DirectorySearch(string dir)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file) == ".zip")
                    {
                        try
                        {
                            string[] fontFolderParts = { dir, Path.GetFileNameWithoutExtension(file) };
                            var fontFolderPath = Path.Combine(fontFolderParts);

                            LogMessage("Extracting fonts from: " + file + " to: " + fontFolderPath, MessageType.Success);

                            ZipFile.ExtractToDirectory(file, fontFolderPath);
                        }
                        catch (Exception ex)
                        {
                            LogMessage("Archive extraction error: " + ex.Message, MessageType.Error);
                        }
                    }

                    if (Path.GetExtension(file) == ".ttf" || Path.GetExtension(file) == ".otf")
                    {
                        string fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                        string fontName = Path.GetFileName(file);
                        var newPath = Path.Combine(fontsFolder, fontName);

                        LogMessage("Copying font file: " + file + " to: " + newPath, MessageType.Success);

                        try
                        {
                            File.Copy(file, newPath);
                        }
                        catch (Exception ex)
                        {
                            LogMessage("Move file error: " + ex.Message, MessageType.Error);
                        }

                        try
                        {
                            var result = AddFontResource(newPath);

                            LogMessage("Successfully added: " + fontName, MessageType.Success);
                        }
                        catch (Exception ex)
                        {
                            LogMessage("Add font error: " + ex.Message, MessageType.Error);
                        }

                        try
                        {
                            // Add font's record to the registry
                            var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", true);
                            key.SetValue(Path.GetFileNameWithoutExtension(fontName) + " (TrueType)", fontName);

                            LogMessage(fontName + " successfully added to the registry.", MessageType.Success);
                            FontsInstalled++;
                            key.Close();
                        }
                        catch (Exception ex)
                        {
                            LogMessage("Add registry item exception: " + ex.Message, MessageType.Error);
                        }
                    }
                }

                foreach (string directory in Directory.GetDirectories(dir))
                {
                    DirectorySearch(directory);
                }
            }
            catch (Exception ex)
            {
                LogMessage("Error: " + ex.Message, MessageType.Error);
            }
        }

        private void DisplayInstalledFontsNumber(int numberOfFontsInstalled)
        {
            LogMessage("", MessageType.Final);
            LogMessage(numberOfFontsInstalled + " fonts installed.", MessageType.Final);
            UpdateLabelText(numberOfFontsInstalled + " fonts installed. Select another folder if you wish to install more. ");
        }

        private void LogMessage(string message, MessageType messageType)
        {
            // Assuming richTextBoxLog is the name of your RichTextBox control
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;

            switch (messageType)
            {
                case MessageType.Success:
                    richTextBox1.SelectionColor = Color.Green;
                    break;
                case MessageType.Error:
                    richTextBox1.SelectionColor = Color.Red;
                    break;
                case MessageType.Final:
                    richTextBox1.SelectionColor = Color.DarkGreen;
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                    richTextBox1.SelectionIndent = 25;
                    richTextBox1.SelectionHangingIndent = -25;
                    break;
                default:
                    richTextBox1.SelectionColor = richTextBox1.ForeColor; // Use default color for other types
                    break;
            }

            richTextBox1.AppendText(message + Environment.NewLine);
            // Reset formatting to default after appending the message
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
            richTextBox1.SelectionIndent = 0;
            richTextBox1.SelectionRightIndent = 0;
        }

        public enum MessageType
        {
            Success,
            Error,
            Final
        }

        private void UpdateLabelText(string newLabelText)
        {
            // Assuming label1 is the name of your Label control
            label1.Text = newLabelText;
        }
    }
}
