﻿//-----------------------------------------------------------------------
// <copyright file="ViewComputationResultModelBuilder.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Fudge;
using Fudge.Serialization;
using OGDotNet.Mappedtypes.engine.value;
using OGDotNet.Mappedtypes.engine.View;
using OGDotNet.Mappedtypes.Id;

namespace OGDotNet.Builders.ViewResultModel
{
    internal class InMemoryViewComputationResultModelBuilder : InMemoryViewResultModelBuilderBase<InMemoryViewComputationResultModel>
    {
        public InMemoryViewComputationResultModelBuilder(FudgeContext context, Type type)
            : base(context, type)
        {
        }

        protected override InMemoryViewComputationResultModel BuildObject(IFudgeFieldContainer msg, IFudgeDeserializer deserializer, Dictionary<string, ViewCalculationResultModel> configurationMap, UniqueIdentifier viewProcessId, UniqueIdentifier viewCycleId, DateTimeOffset inputDataTimestamp, DateTimeOffset resultTimestamp)
        {
            var liveDataMsg = msg.GetMessage("liveData");
            List<ComputedValue> liveData = liveDataMsg == null ? null : liveDataMsg.GetAllByOrdinal(1).Select(deserializer.FromField<ComputedValue>).ToList();
            return new InMemoryViewComputationResultModel(viewProcessId, viewCycleId, inputDataTimestamp, resultTimestamp, configurationMap, liveData);
        }
    }
}