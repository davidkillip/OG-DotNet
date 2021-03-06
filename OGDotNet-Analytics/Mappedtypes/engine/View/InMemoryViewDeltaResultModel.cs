﻿//-----------------------------------------------------------------------
// <copyright file="InMemoryViewDeltaResultModel.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using OGDotNet.Mappedtypes.Id;

namespace OGDotNet.Mappedtypes.Engine.View
{
    public class InMemoryViewDeltaResultModel : InMemoryViewResultModelBase, IViewDeltaResultModel
    {
        private readonly DateTimeOffset _previousResultTimestamp;

        public InMemoryViewDeltaResultModel(UniqueId viewProcessId, UniqueId viewCycleId, DateTimeOffset inputDataTimestamp, DateTimeOffset resultTimestamp, IDictionary<string, ViewCalculationResultModel> configurationMap, DateTimeOffset previousResultTimestamp, TimeSpan calculationDuration) 
            : base(viewProcessId, viewCycleId, inputDataTimestamp, resultTimestamp, configurationMap, calculationDuration)
        {
            _previousResultTimestamp = previousResultTimestamp;
        }

        public DateTimeOffset PreviousResultTimestamp
        {
            get { return _previousResultTimestamp; }
        }
    }
}