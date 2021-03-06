﻿//-----------------------------------------------------------------------
// <copyright file="AbstractDocument.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Fudge;
using Fudge.Types;
using OGDotNet.Mappedtypes.Id;
using OGDotNet.Utils;

namespace OGDotNet.Mappedtypes.Master
{
    public abstract class AbstractDocument : IUniqueIdentifiable
    {
        private readonly DateTimeOffset _versionFromInstant;
        private readonly DateTimeOffset _versionToInstant;
        private readonly DateTimeOffset _correctionFromInstant;
        private readonly DateTimeOffset _correctionToInstant;

        protected AbstractDocument() : this(default(DateTimeOffset), default(DateTimeOffset), default(DateTimeOffset), default(DateTimeOffset))
        {
        }

        protected AbstractDocument(DateTimeOffset versionFromInstant, DateTimeOffset versionToInstant, DateTimeOffset correctionFromInstant, DateTimeOffset correctionToInstant)
        {
            _versionFromInstant = versionFromInstant;
            _versionToInstant = versionToInstant;
            _correctionFromInstant = correctionFromInstant;
            _correctionToInstant = correctionToInstant;
        }

        public DateTimeOffset VersionFromInstant
        {
            get { return _versionFromInstant; }
        }

        public DateTimeOffset VersionToInstant
        {
            get { return _versionToInstant; }
        }

        public DateTimeOffset CorrectionFromInstant
        {
            get { return _correctionFromInstant; }
        }

        public DateTimeOffset CorrectionToInstant
        {
            get { return _correctionToInstant; }
        }

        public static DateTimeOffset GetDocumentValues(IFudgeFieldContainer ffc, out DateTimeOffset versionToInstant, out DateTimeOffset correctionFromInstant, out DateTimeOffset correctionToInstant)
        {
            var versionFromInstant = ffc.GetValue<FudgeDateTime>("versionFromInstant").ToDateTimeOffsetWithDefault();
            correctionFromInstant = ffc.GetValue<FudgeDateTime>("correctionFromInstant").ToDateTimeOffsetWithDefault();
            versionToInstant = ffc.GetValue<FudgeDateTime>("versionToInstant").ToDateTimeOffsetWithDefault();
            correctionToInstant = ffc.GetValue<FudgeDateTime>("correctionToInstant").ToDateTimeOffsetWithDefault();
            return versionFromInstant;
        }

        public void WriteDocumentFields(IAppendingFudgeFieldContainer a)
        {
            AddDateTimeOffsetWithDefault(a, "versionFromInstant", VersionFromInstant);
            AddDateTimeOffsetWithDefault(a, "correctionFromInstant", CorrectionFromInstant);
            
            AddDateTimeOffsetWithDefault(a, "versionToInstant", VersionToInstant);
            AddDateTimeOffsetWithDefault(a, "correctionToInstant", CorrectionToInstant);
        }

        private static void AddDateTimeOffsetWithDefault(IAppendingFudgeFieldContainer a, string fieldName, DateTimeOffset value)
        {
            if (value != default(DateTimeOffset))
            {
                a.Add(fieldName, new FudgeDateTime(value));
            }
        }

        public abstract UniqueId UniqueId { get; set; }
    }
}
