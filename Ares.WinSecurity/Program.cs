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
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Text;

namespace Ares.WinSecurity
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Invalid number of arguments");
                return 1;
            }
            if (args[0] == "ChangePort")
            {
                return ChangePort(args);
            }
            else if (args[0] == "AddPort")
            {
                return AddPort(args);
            }
            else if (args[0] == "RemovePort")
            {
                return RemovePort(args);
            }
            else
            {
                Console.Error.WriteLine("Invalid request '" + args[0] + "'");
                return 2;
            }
        }

        private static readonly String FW_RULE_NAME = "Ares Web Controller to Player Tcp Port";

        private static int WrapExceptions(Func<int> f, String outFile)
        {
            try
            {
                try
                {
                    return f();
                }
                catch (WinSecurityException ex)
                {
                    if (!String.IsNullOrEmpty(outFile))
                    {
                        using (StreamWriter writer = new StreamWriter(outFile))
                        {
                            writer.WriteLine(ex.Message);
                            if (ex.InnerException != null)
                            {
                                writer.WriteLine(ex.InnerException.GetType().Name);
                                writer.WriteLine(ex.InnerException.Message);
                            }
                            writer.Flush();
                            return 5;
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine(ex.Message);
                        if (ex.InnerException != null)
                        {
                            Console.Error.WriteLine(ex.InnerException.GetType().Name);
                            Console.Error.WriteLine(ex.InnerException.Message);
                        }
                        return 5;
                    }
                }
            }
            catch (IOException)
            {
                return 6;
            }
        }

        private static int AddPort(string[] args)
        {
            if (args.Length > 3)
            {
                Console.Error.WriteLine("Invalid number of arguments");
                return 1;
            }
            int newPort;
            if (args.Length > 1)
            {
                if (!Int32.TryParse(args[1], out newPort))
                {
                    Console.Error.WriteLine("Invalid new port " + args[2]);
                    return 4;
                }
            }
            else
            {
                Ares.Settings.BasicSettings basicSettings = new Ares.Settings.BasicSettings();
                bool foundSettings = basicSettings.ReadFromFile();
                Ares.Settings.Settings.Instance.InitializeWithoutSharedMemory(foundSettings ? basicSettings.GetSettingsDir() : null);
                newPort = Ares.Settings.Settings.Instance.WebTcpPort;
            }
            return WrapExceptions(() =>
            {
                String newUrl = String.Format("http://+:{0}/", newPort);
                if (!WinSecurityHelpers.FindUrlPrefix(newUrl))
                {
                    WinSecurityHelpers.AddUrlPrefix(newUrl);
                }
                if (WinSecurityHelpers.FirewallEnabled)
                {
                    WinSecurityHelpers.AddFirewallRule(FW_RULE_NAME, newPort);
                }
                return 0;
            }, args.Length == 3 ? args[2] : String.Empty);
        }

        private static int ChangePort(string[] args)
        { 
            if (args.Length != 4)
            {
                Console.Error.WriteLine("Invalid number of arguments");
                return 1;
            }
            int oldPort, newPort;
            if (!Int32.TryParse(args[1], out oldPort))
            {
                Console.Error.WriteLine("Invalid old port " + args[1]);
                return 3;
            }
            if (!Int32.TryParse(args[2], out newPort))
            {
                Console.Error.WriteLine("Invalid new port " + args[2]);
                return 4;
            }
            return WrapExceptions(() =>
            {
                String oldUrl = String.Format("http://+:{0}/", oldPort);
                String newUrl = String.Format("http://+:{0}/", newPort);
                if (WinSecurityHelpers.FindUrlPrefix(oldUrl))
                {
                    WinSecurityHelpers.RemoveUrlPrefix(oldUrl);
                }
                if (!WinSecurityHelpers.FindUrlPrefix(newUrl))
                {
                    WinSecurityHelpers.AddUrlPrefix(newUrl);
                }
                if (WinSecurityHelpers.FirewallEnabled)
                {
                    WinSecurityHelpers.AddFirewallRule(FW_RULE_NAME, newPort);
                }
                return 0;
            }, args[3]);
        }

        private static int RemovePort(String[] args)
        {
            if (args.Length > 2)
            {
                Console.Error.WriteLine("Invalid number of arguments");
                return 1;
            }
            return WrapExceptions(() =>
            {
                Ares.Settings.BasicSettings basicSettings = new Ares.Settings.BasicSettings();
                bool foundSettings = basicSettings.ReadFromFile();
                Ares.Settings.Settings.Instance.InitializeWithoutSharedMemory(foundSettings ? basicSettings.GetSettingsDir() : null);
                String oldUrl = String.Format("http://+:{0}/", Ares.Settings.Settings.Instance.WebTcpPort);
                if (WinSecurityHelpers.FindUrlPrefix(oldUrl))
                {
                    WinSecurityHelpers.RemoveUrlPrefix(oldUrl);
                }
                if (WinSecurityHelpers.FirewallEnabled)
                {
                    WinSecurityHelpers.RemoveFirewallRule(FW_RULE_NAME);
                }
                return 0;
            }, args.Length == 2 ? args[1] : String.Empty);

        }
    }
}
