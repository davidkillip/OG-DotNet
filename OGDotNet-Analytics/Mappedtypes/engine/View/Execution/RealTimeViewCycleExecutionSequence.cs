﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealTimeViewCycleExecutionSequence.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//   Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using Fudge;
using Fudge.Serialization;
using OGDotNet.Builders;

namespace OGDotNet.Mappedtypes.engine.View.Execution
{
    public class RealTimeViewCycleExecutionSequence : IViewCycleExecutionSequence
    {
        public bool IsEmpty
        {
            get { return false; }
        }

        public ViewCycleExecutionOptions Next
        {
            get { return new ViewCycleExecutionOptions(DateTimeOffset.Now, DateTimeOffset.Now); }
        }

        public static RealTimeViewCycleExecutionSequence FromFudgeMsg(IFudgeFieldContainer ffc, IFudgeDeserializer deserializer)
        {
            return new RealTimeViewCycleExecutionSequence();
        }

        public void ToFudgeMsg(IAppendingFudgeFieldContainer a, IFudgeSerializer s)
        {
            s.WriteTypeHeader(a, typeof(RealTimeViewCycleExecutionSequence));
        }
    }
}