//-----------------------------------------------------------------------
// <copyright file="UserPrincipal.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net;
using OGDotNet.Properties;

namespace OGDotNet.Mappedtypes.LiveData
{
    [DebuggerDisplay("UserPrincipal {_userName}/{_ipAddress}")]
    public class UserPrincipal
    {
        private readonly string _userName;
        private readonly string _ipAddress;

        public string UserName
        {
            get { return _userName; }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
        }

        public UserPrincipal(string userName, string ipAddress)
        {
            _userName = userName;
            _ipAddress = ipAddress;
        }

        public static UserPrincipal DefaultUser
        {
            get { return new UserPrincipal(Settings.Default.UserName, GetIP()); }
        }

        private static string GetIP()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                return ipaddress.ToString();
            }
            throw new ArgumentException();
        }
    }
}