﻿//-----------------------------------------------------------------------
// <copyright file="MarketDataSnapshotProcessor.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using OGDotNet.Mappedtypes.Core.marketdatasnapshot;
using OGDotNet.Mappedtypes.engine.view;
using OGDotNet.Mappedtypes.financial.analytics.ircurve;
using OGDotNet.Mappedtypes.financial.model.interestrate.curve;
using OGDotNet.Mappedtypes.Master.marketdatasnapshot;
using OGDotNet.Mappedtypes.Master.MarketDataSnapshot;
using OGDotNet.Model.Context.MarketDataSnapshot;
using OGDotNet.Model.Resources;
using OGDotNet.Utils;

namespace OGDotNet.Model.Context
{
    /// <summary>
    /// <para>This class handles creating and mutating snapshots based on Views and live data</para>
    /// <list type="table">
    /// <item>TODO: this implementation probably shouldn't be client side</item>
    /// </list>
    /// </summary>
    public class MarketDataSnapshotProcessor : DisposableBase
    {
        private readonly ManageableMarketDataSnapshot _snapshot;
        private readonly RawMarketDataSnapper _rawMarketDataSnapper;
        private readonly RemoteMarketDataSnapshotMaster _marketDataSnapshotMaster; //TODO should be the user master
        private readonly LiveDataStream _liveDataStream;

        internal static MarketDataSnapshotProcessor Create(RemoteEngineContext context, ViewDefinition definition, CancellationToken ct)
        {
            var liveDataStream = new LiveDataStream(definition.Name, context);
            var rawMarketDataSnapper = new RawMarketDataSnapper(context, definition);
            var snapshot = liveDataStream.GetNewSnapshotForUpdate(ct);
            
            return new MarketDataSnapshotProcessor(snapshot, rawMarketDataSnapper, context, liveDataStream);
        }

        internal MarketDataSnapshotProcessor(RemoteEngineContext remoteEngineContext, ManageableMarketDataSnapshot snapshot)
            : this(snapshot, new RawMarketDataSnapper(remoteEngineContext, remoteEngineContext.ViewProcessor.ViewDefinitionRepository.GetViewDefinition(snapshot.BasisViewName)), remoteEngineContext, new LiveDataStream(snapshot.BasisViewName, remoteEngineContext))
        {
        }

        private MarketDataSnapshotProcessor(ManageableMarketDataSnapshot snapshot, RawMarketDataSnapper rawMarketDataSnapper, RemoteEngineContext remoteEngineContext, LiveDataStream liveDataStream)
        {
            _snapshot = snapshot;
            _rawMarketDataSnapper = rawMarketDataSnapper;
            _marketDataSnapshotMaster = remoteEngineContext.MarketDataSnapshotMaster;
            _liveDataStream = liveDataStream;
        }

        public ManageableMarketDataSnapshot Snapshot
        {
            get
            {
                CheckDisposed(); 
                return _snapshot;
            }
        }

        public UpdateAction<ManageableMarketDataSnapshot> PrepareUpdate(CancellationToken ct = default(CancellationToken))
        {
            return Snapshot.PrepareUpdateFrom(GetNewSnapshotForUpdate(ct));
        }

        public ManageableMarketDataSnapshot GetNewSnapshotForUpdate(CancellationToken ct = default(CancellationToken))
        {
            return _liveDataStream.GetNewSnapshotForUpdate(ct);
        }

        public LiveDataStream LiveDataStream
        {
            get { return _liveDataStream; }
        }

        public Dictionary<YieldCurveKey, Tuple<YieldCurve, InterpolatedYieldCurveSpecificationWithSecurities>> GetYieldCurves(CancellationToken ct = default(CancellationToken))
        {
            CheckDisposed();

            var shallowClone = new ManageableMarketDataSnapshot(_snapshot.BasisViewName, _snapshot.GlobalValues, _snapshot.YieldCurves, _snapshot.UniqueId)
                                   {
                                       Name = typeof(MarketDataSnapshotProcessor).Name + " Temp",
                                       UniqueId = null
                                   };
            var snapshot = _marketDataSnapshotMaster.Add(new MarketDataSnapshotDocument(null, shallowClone));
            try
            {
                return _rawMarketDataSnapper.GetYieldCurves(snapshot.UniqueId, ct);
            }
            finally
            {
                _marketDataSnapshotMaster.Remove(snapshot.UniqueId);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _liveDataStream.Dispose();
            }
        }
    }
}
