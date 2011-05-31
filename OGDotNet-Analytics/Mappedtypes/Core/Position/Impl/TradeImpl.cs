﻿//-----------------------------------------------------------------------
// <copyright file="TradeImpl.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Fudge.Serialization;
using OGDotNet.Builders;
using OGDotNet.Mappedtypes.Id;

namespace OGDotNet.Mappedtypes.Core.Position.Impl
{
    [FudgeSurrogate(typeof(TradeBuilder))]
    class TradeImpl : ITrade
    {
        private readonly UniqueIdentifier _uniqueId;
        private readonly UniqueIdentifier _parentPositionId;
        private readonly DateTimeOffset _tradeDate;
        private readonly IdentifierBundle _securityKey;

        public TradeImpl(UniqueIdentifier uniqueId, UniqueIdentifier parentPositionId, DateTimeOffset tradeDate, IdentifierBundle securityKey)
        {
            _uniqueId = uniqueId;
            _securityKey = securityKey;
            _tradeDate = tradeDate;
            _parentPositionId = parentPositionId;
        }

        public UniqueIdentifier ParentPositionId
        {
            get { return _parentPositionId; }
        }

        public DateTimeOffset TradeDate
        {
            get { return _tradeDate; }
        }

        public IdentifierBundle SecurityKey
        {
            get { return _securityKey; }
        }

        public UniqueIdentifier UniqueId
        {
            get { return _uniqueId; }
        }
    }
}
