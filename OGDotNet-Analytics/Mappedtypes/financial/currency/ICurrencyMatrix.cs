﻿//-----------------------------------------------------------------------
// <copyright file="ICurrencyMatrix.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using OGDotNet.Mappedtypes.Id;
using Currency = OGDotNet.Mappedtypes.Util.Money.Currency;

namespace OGDotNet.Mappedtypes.Financial.currency
{
    public interface ICurrencyMatrix : IUniqueIdentifiable
    {
        ICollection<Currency> SourceCurrencies { get; }

        IEnumerable<Currency> TargetCurrencies { get; }

        CurrencyMatrixValue GetConversion(Currency source, Currency target);    
    }
} 