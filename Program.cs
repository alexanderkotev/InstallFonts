using System.Runtime.InteropServices;
using System.IO.Compression;


namespace FontsInstaller
{
    class Program
    {
        public static int fontsInstalled = 0;
        static void Main()
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
            static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                         string lpFileName);

            Console.WriteLine("Paste fonts directory path: ");

            string folderPath = Console.ReadLine();

            try
            {
                DirectorySearch(folderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                DisplayInstalledFontsNumber(fontsInstalled);
            }

            static void DirectorySearch(string dir)
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
                                Console.WriteLine(fontFolderPath);
                                ZipFile.ExtractToDirectory(file, fontFolderPath);
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Archive extraction error: " + ex.Message);
                                Console.ResetColor();
                            }
                        }
                        if (Path.GetExtension(file) == ".ttf" || Path.GetExtension(file) == ".otf")
                        {
                            string fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                            string fontName = Path.GetFileName(file);
                            var newPath = Path.Combine(fontsFolder, fontName);
                            Console.WriteLine(newPath);
                            try
                            {
                                File.Copy(file, newPath);
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Move file error: " + ex.Message);
                                Console.ResetColor();
                            }
                            try
                            {
                                var result = AddFontResource(newPath);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Successfully added: " + fontName);
                                Console.ResetColor();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Add font error: " + ex.Message);
                            }
                            try
                            {
                                //Add font's record to the registry
                                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.
                                 CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts");
                                key.SetValue(Path.GetFileNameWithoutExtension(fontName) + " (TrueType)", fontName);

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(fontName + " Successfully added to the registry.");
                                Console.ResetColor();
                                fontsInstalled++;
                                key.Close();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Add registry item exception: " + ex.Message);
                                Console.ResetColor();
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
                    Console.WriteLine(ex.Message);
                }
            }

            static void DisplayInstalledFontsNumber(int numberofFontsInstaled)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = numberofFontsInstaled > 0 ? ConsoleColor.DarkGreen : ConsoleColor.Red;
                Console.WriteLine(numberofFontsInstaled + " fonts installed.");
                Console.ResetColor();
            }
            Console.ReadLine();
        }
    }
}


