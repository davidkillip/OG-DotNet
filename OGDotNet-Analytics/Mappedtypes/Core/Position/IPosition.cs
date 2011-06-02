//-----------------------------------------------------------------------
// <copyright file="IPosition.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Fudge.Serialization;
using OGDotNet.Builders;
using OGDotNet.Mappedtypes.Id;

namespace OGDotNet.Mappedtypes.Core.Position
{
    [FudgeSurrogate(typeof(PositionBuilder))]
    public interface IPosition : IPositionOrTrade
    {
        IEnumerable<ITrade> Trades { get; }
    }
}