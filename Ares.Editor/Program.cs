/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ares.Editor
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                string arg = Environment.GetCommandLineArgs()[1];
                string paramName = "Language=";
                if (arg.StartsWith(paramName))
                {
                    arg = arg.Substring(paramName.Length);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(arg);
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                using (Ares.Playing.BassInit bassInit = new Ares.Playing.BassInit(-1, s => MessageBox.Show(s, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Warning)))
                {
                    Application.Run(new MainForm());
                }
            }
			#if !MONO
            catch (Ares.Ipc.ApplicationAlreadyStartedException)
            {
            }
			#endif
            catch (Ares.Playing.BassInitException ex)
            {
                MessageBox.Show(ex.Message, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            WriteExceptionTrace(ex);
        }

        static void WriteExceptionTrace(Exception ex)
        {
            try
            {
                String folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                String path = System.IO.Path.Combine(folder, "Ares_Errors.log");
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true))
                {
                    writer.WriteLine(System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteLine(ex.GetType().Name + ": " + ex.Message);
                    writer.WriteLine("Stack Trace:");
                    writer.WriteLine(ex.StackTrace);
                    writer.WriteLine("--------------------------------------------------");
                    writer.Flush();
                }
            }
            catch (Exception)
            {
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            WriteExceptionTrace(e.Exception);
            Dialogs.ExceptionDialog dialog = new Dialogs.ExceptionDialog();
            dialog.SetException(e.Exception);
            if (dialog.ShowDialog() == DialogResult.Abort)
            {
                Application.Exit();
            }
        }
    }
}
