/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.CmdLinePlayer
{
    class Program
    {
        static int Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

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
			
            Console.WriteLine(String.Format(StringResources.AresPlayer, (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString()));
            Console.WriteLine();

            int res = 0;
            try
            {
                PlayerOptions options = new PlayerOptions();
                if (options.Parse(args))
                {
                    int outputDevice = options.OutputDevice == 0 ? -1 : options.OutputDevice;
                    using (Ares.Playing.BassInit bassInit = new Ares.Playing.BassInit(outputDevice, s => Console.WriteLine(s)))
                    {
                        {
                            Player player = new Player(bassInit);
                            res = player.Run(options);
                        }
                    }
                }
                else
                {
                    res = 2;
                }
            }
            catch (Ares.Playing.BassInitException ex)
            {
                Console.WriteLine(ex.Message);
                res = 1;                
            }
            return res;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
				Console.WriteLine("Unhandled Exception!");
                Console.WriteLine(ex.GetType() + ": " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                WriteExceptionTrace(ex);
            }
			else
			{
				Console.WriteLine("Unhandled Exception!");
			}
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
    }
}
