﻿//-----------------------------------------------------------------------
// <copyright file="ICounterparty.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------
using OGDotNet.Mappedtypes.Id;

namespace OGDotNet.Mappedtypes.Core.Position
{
    public interface ICounterparty
    {
        ExternalId ExternalId { get; }
    }
}
