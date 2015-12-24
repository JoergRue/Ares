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
using NetFwTypeLib;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ares.WinSecurity
{
    class WinSecurityException : Exception
    {
        public WinSecurityException(String message)
            : base(message)
        { }

        public WinSecurityException(String message, Exception inner)
            : base(message, inner)
        { }
    }

    class WinSecurityHelpers
    {

        #region Interop
        internal enum HTTP_SERVICE_CONFIG_QUERY_TYPE
        {
            HttpServiceConfigQueryExact = 0,
            HttpServiceConfigQueryNext,
            HttpServiceConfigQueryMax
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct HTTP_SERVICE_CONFIG_URLACL_KEY
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pUrlPrefix;

            public HTTP_SERVICE_CONFIG_URLACL_KEY(string urlPrefix)
            {
                pUrlPrefix = urlPrefix;
            }
        }

        internal enum HTTP_SERVICE_CONFIG_ID
        {
            HttpServiceConfigIPListenList = 0,
            HttpServiceConfigSSLCertInfo,
            HttpServiceConfigUrlAclInfo,
            HttpServiceConfigMax
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SERVICE_CONFIG_URLACL_QUERY
        {
            public HTTP_SERVICE_CONFIG_QUERY_TYPE QueryDesc;
            public HTTP_SERVICE_CONFIG_URLACL_KEY KeyDesc;
            public uint dwToken;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SERVICE_CONFIG_URLACL_SET
        {
            public HTTP_SERVICE_CONFIG_URLACL_KEY KeyDesc;
            public HTTP_SERVICE_CONFIG_URLACL_PARAM ParamDesc;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct HTTP_SERVICE_CONFIG_URLACL_PARAM
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pStringSecurityDescriptor;

            public HTTP_SERVICE_CONFIG_URLACL_PARAM(string securityDescriptor)
            {
                pStringSecurityDescriptor = securityDescriptor;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct HTTPAPI_VERSION
        {
            public ushort HttpApiMajorVersion;
            public ushort HttpApiMinorVersion;

            public HTTPAPI_VERSION(ushort majorVersion, ushort minorVersion)
            {
                HttpApiMajorVersion = majorVersion;
                HttpApiMinorVersion = minorVersion;
            }
        }

        [DllImport("httpapi.dll", SetLastError = true)]
        internal static extern uint HttpQueryServiceConfiguration(
             IntPtr ServiceIntPtr,
             HTTP_SERVICE_CONFIG_ID ConfigId,
             IntPtr pInputConfigInfo,
             int InputConfigInfoLength,
             IntPtr pOutputConfigInfo,
             int OutputConfigInfoLength,
             [Optional()]
             out int pReturnLength,
             IntPtr pOverlapped);

        [DllImport("httpapi.dll", SetLastError = true)]
        internal static extern uint HttpInitialize(
             HTTPAPI_VERSION Version,
             uint Flags,
             IntPtr pReserved);

        internal const uint ERROR_NO_MORE_ITEMS = 259;
        internal const uint ERROR_INSUFFICIENT_BUFFER = 122;
        internal const uint NO_ERROR = 0;
        internal const uint HTTP_INITIALIZE_CONFIG = 2;

        #endregion

        static WinSecurityHelpers()
        {
            HTTPAPI_VERSION version = new HTTPAPI_VERSION(1, 0);
            HttpInitialize(version, HTTP_INITIALIZE_CONFIG, IntPtr.Zero);
        }

        public static bool FirewallEnabled
        {
            get
            {
                try
                {
                    Type fwManagerType = Type.GetTypeFromProgID("HNetCfg.FwMgr");
                    INetFwMgr fwManager = (INetFwMgr)Activator.CreateInstance(fwManagerType);
                    return fwManager.LocalPolicy.CurrentProfile.FirewallEnabled;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private static INetFwPolicy2 GetFirewallPolicy()
        {
            Type fwPolicyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(fwPolicyType);
            return firewallPolicy;
        }

        private static INetFwRule FindFirewallRule(INetFwPolicy2 firewallPolicy, String ruleName)
        {
            foreach (INetFwRule rule in firewallPolicy.Rules)
            {
                if (rule.Name == ruleName)
                    return rule;
            }
            return null;
        }

        public static void RemoveFirewallRule(String ruleName)
        {
            try
            {
                INetFwPolicy2 firewallPolicy = GetFirewallPolicy();
                INetFwRule fwRule = FindFirewallRule(firewallPolicy, ruleName);
                if (fwRule != null)
                {
                    firewallPolicy.Rules.Remove(ruleName);
                }
            }
            catch (Exception ex)
            {
                throw new WinSecurityException("Error removing firewall rule", ex);
            }
        }

        public static void AddFirewallRule(String ruleName, int newPort)
        {
            try
            {
                INetFwPolicy2 firewallPolicy = GetFirewallPolicy();
                INetFwRule fwRule = FindFirewallRule(firewallPolicy, ruleName);
                if (fwRule != null)
                {
                    fwRule.LocalPorts = newPort.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    Type fwRuleType = Type.GetTypeFromProgID("HNetCfg.FWRule");
                    fwRule = (INetFwRule)Activator.CreateInstance(fwRuleType);
                    fwRule.Name = ruleName;
                    fwRule.Description = "Allow TCP Port for Web Controller access to Ares Player";
                    fwRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                    fwRule.LocalPorts = newPort.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    fwRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                    fwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    fwRule.Enabled = true;
                    firewallPolicy.Rules.Add(fwRule);
                }
            }
            catch (Exception ex)
            {
                throw new WinSecurityException("Error changing firewall rule", ex);
            }
        }

        public static void AddUrlPrefix(String urlPrefix)
        {
            String args = String.Format("http add urlacl url={0} user={1}\\{2} listen=yes", urlPrefix, Environment.UserDomainName, Environment.UserName);
            RunNetSh(args);
        }

        public static void RemoveUrlPrefix(String urlPrefix)
        {
            String args = String.Format("http delete urlacl url={0}", urlPrefix);
            RunNetSh(args);
        }

        private static void RunNetSh(String args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
                //psi.Verb = "runas";
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = true;

                var res = Process.Start(psi);
                res.WaitForExit();
                if (res.ExitCode != 0)
                {
                    throw new WinSecurityException("Error running netsh: error code " + res.ExitCode);
                }
            }
            catch (WinSecurityException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new WinSecurityException("Error running netsh", ex);
            }
        }

        public static bool FindUrlPrefix(String urlPrefix)
        {
            HTTP_SERVICE_CONFIG_URLACL_QUERY query = new HTTP_SERVICE_CONFIG_URLACL_QUERY();
            query.QueryDesc = HTTP_SERVICE_CONFIG_QUERY_TYPE.HttpServiceConfigQueryNext;

            IntPtr pQuery = Marshal.AllocHGlobal(Marshal.SizeOf(query));

            try
            {
                uint retval = NO_ERROR;
                for (query.dwToken = 0; ; query.dwToken++)
                {
                    Marshal.StructureToPtr(query, pQuery, false);

                    try
                    {
                        int returnSize = 0;

                        // Get Size
                        retval = HttpQueryServiceConfiguration(IntPtr.Zero, HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo, 
                            pQuery, Marshal.SizeOf(query), IntPtr.Zero, 0, 
                            out returnSize, IntPtr.Zero);

                        if (retval == ERROR_NO_MORE_ITEMS)
                        {
                            break;
                        }

                        if (retval != ERROR_INSUFFICIENT_BUFFER)
                        {
                            throw new WinSecurityException("HttpQueryServiceConfiguration returned unexpected error code " + retval);
                        }

                        IntPtr pConfig = Marshal.AllocHGlobal((IntPtr)returnSize);

                        string foundPrefix;
                        try
                        {
                            retval = HttpQueryServiceConfiguration(IntPtr.Zero, HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo, 
                                pQuery, Marshal.SizeOf(query), pConfig, returnSize, out returnSize, IntPtr.Zero);
                            HTTP_SERVICE_CONFIG_URLACL_SET config = (HTTP_SERVICE_CONFIG_URLACL_SET)Marshal.PtrToStructure(pConfig, typeof(HTTP_SERVICE_CONFIG_URLACL_SET));
                            foundPrefix = config.KeyDesc.pUrlPrefix;
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(pConfig);
                        }

                        if (foundPrefix == urlPrefix)
                        {
                            return true;
                        }
                    }
                    finally
                    {
                        Marshal.DestroyStructure(pQuery, typeof(HTTP_SERVICE_CONFIG_URLACL_QUERY));
                    }
                }

                if (retval != ERROR_NO_MORE_ITEMS)
                {
                    throw new WinSecurityException("HttpQueryServiceConfiguration returned unexpected error code " + retval);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pQuery);
            }

            return false;
        }
    }
}
