﻿//-----------------------------------------------------------------------
// <copyright file="UriEncoding.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using OGDotNet.Mappedtypes.Id;

namespace OGDotNet.Model.Resources
{
    public static class UriEncoding
    {
        public static string ToString(DateTimeOffset curveDate)
        {
            return curveDate.ToString("yyyy-MM-dd");
        }

        internal static Tuple<string, string>[] GetParameters(ExternalIdBundle bundle)
        {
            var ids = bundle.Identifiers.ToList();

            return ids.Select(s => new Tuple<string, string>("id", s.ToString())).ToArray();
        }
    }
}