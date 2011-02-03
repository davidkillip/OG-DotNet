﻿using System;
using System.Net;
using OGDotNet_Analytics.Properties;

namespace OGDotNet_Analytics.Mappedtypes.LiveData
{
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

        internal static UserPrincipal DefaultUser
        {
            get { return new UserPrincipal(Settings.Default.UserName, GetIP()); }
        }

        private static string GetIP()
        {
            String strHostName = Dns.GetHostName();
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                return ipaddress.ToString();
            }
            throw new ArgumentException();
        }
    }
}