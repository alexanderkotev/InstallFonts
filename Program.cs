using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FontsInstaller
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                // Relaunch the application with administrative rights
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Verb = "runas";  // This triggers the UAC prompt

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    // User clicked 'No' on UAC prompt or other error
                    MessageBox.Show("This application requires administrator privileges to perform certain actions.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                return;  // Exit the current instance of the application
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
