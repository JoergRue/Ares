/*
 Copyright (c) 2015 [Martin Ried]
 
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ares.Editor.Plugins
{
    /// <summary>
    /// The PluginManager scans the "plugins" directory for DLLs.
    /// 
    /// It can then provide a list of all Types from those Assemblies implementing a specific Interface.
    /// In addition, an instance of each plugin type can be created.
    /// </summary>
    public class PluginManager
    {

        /// <summary>
        /// List of all Assemblies found in the "plugins" directory.
        /// </summary>
        private ICollection<Assembly> assemblies = null;

        /// <summary>
        /// Cached list of plugin instances.
        /// </summary>
        private Dictionary<Type, object> pluginInstances = new Dictionary<Type, object>();

        public PluginManager()
        {
            string pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(),"plugins");

            string[] pluginDlls = null;
            if (Directory.Exists(pluginDirectory))
            {
                pluginDlls = Directory.GetFiles(pluginDirectory, "*.dll");
            } 
            else
            {
                pluginDlls = new string[0];
            }

            this.assemblies = new List<Assembly>(pluginDlls.Length);
            foreach (string dllFile in pluginDlls)
            {
                AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                Assembly assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }
        }

        /// <summary>
        /// Find all types from the loaded plugin assemblies that conform to the given interface T
        /// </summary>
        /// <typeparam name="T">Desired plugin interface type</typeparam>
        /// <returns></returns>
        public ICollection<Type> GetPluginTypes<T>()
        {
            ICollection<Type> pluginTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if (type.GetInterface(typeof(T).FullName) != null)
                            {
                                pluginTypes.Add(type);
                            }
                        }
                    }
                }
            }

            return pluginTypes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Desired plugin interface type</typeparam>
        /// <returns></returns>
        public ICollection<T> GetPluginInstances<T>()
        {
            ICollection<Type> pluginTypes = GetPluginTypes<T>();
            ICollection<T> relevantPluginInstances = new List<T>();

            foreach (Type pluginType in pluginTypes)
            {
                if (this.pluginInstances[pluginType] != null)
                {
                    this.pluginInstances.Add(pluginType,(T)Activator.CreateInstance(pluginType));
                }

                relevantPluginInstances.Add((T)this.pluginInstances[pluginType]);
            }

            return relevantPluginInstances;
        }

    }
}
