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
            if (args.Length != 4)
            {
                Console.Error.WriteLine("Invalid number of arguments");
                return 1;
            }
            if (args[0] != "ChangePort")
            {
                Console.Error.WriteLine("Invalid request '" + args[0] + "'");
                return 2;
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
            try
            {
                String oldUrl = String.Format("http://+:{0}/", oldPort);
                String newUrl = String.Format("http://+:{0}/", newPort);
                try
                {
                    if (WinSecurityHelpers.FindUrlPrefix(oldUrl))
                    {
                        WinSecurityHelpers.RemoveUrlPrefix(oldUrl);
                    }
                    if (!WinSecurityHelpers.FindUrlPrefix(newUrl))
                    {
                        WinSecurityHelpers.AddUrlPrefix(newUrl);
                    }
                    return 0;
                }
                catch (WinSecurityException ex)
                {
                    using (StreamWriter writer = new StreamWriter(args[3]))
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
            }
            catch (IOException)
            {
                return 6;
            }
        }
    }
}
