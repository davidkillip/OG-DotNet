﻿//-----------------------------------------------------------------------
// <copyright file="ViewCycleState.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

namespace OGDotNet.Mappedtypes.engine.View.calc
{
    public enum ViewCycleState
    {
        AwaitingExecution,
        Executing,
        ExecutionInterrupted,
        Executed,
        Destroyed
    }
}