//-----------------------------------------------------------------------
// <copyright file="OpenGammaFudgeContext.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Castle.Core;
using Castle.Core.Logging;
using Fudge;
using Fudge.Serialization;
using Fudge.Serialization.Reflection;
using Fudge.Types;
using OGDotNet.Utils;
using Currency = OGDotNet.Mappedtypes.Core.Common.Currency;

namespace OGDotNet.Model
{
    [Singleton]
    public class OpenGammaFudgeContext : FudgeContext
    {
        //TODO inject this
        private readonly ILogger _logger = NullLogger.Instance;

        //DOTNET-12
        private static readonly Type[] ForcedReferences = new[] { typeof(Apache.NMS.ActiveMQ.ConnectionFactory) };

        private readonly MemoizingFudgeSurrogateSelector _fudgeSurrogateSelector;

        public OpenGammaFudgeContext()
        {
            SetProperty(ContextProperties.TypeMappingStrategyProperty, new JavaTypeMappingStrategy("OGDotNet.Mappedtypes", "com.opengamma"));
            SetProperty(ContextProperties.FieldNameConventionProperty, FudgeFieldNameConvention.CamelCase);
            AddSecondaryTypes(TypeDictionary);
            _fudgeSurrogateSelector = new MemoizingFudgeSurrogateSelector(this);

            _logger.Debug("Forced references to {0} types", ForcedReferences.Length);
        }

        private void AddSecondaryTypes(FudgeTypeDictionary typeDictionary)
        {
            typeDictionary.AddType(new SecondaryFieldType<Currency, string>(StringFieldType.Instance, Currency.Create, c => c.ISOCode));
        }

        public FudgeSerializer GetSerializer()
        {
            //We can't cache at the SerializationTypeMap level
            // a. It's not thread safe
            // b. We shouldn't artificially inflate the type indexes
            // see 3c356af3736508f1c2a18146a28af6dca4bf8525
            return new FudgeSerializer(this, new SerializationTypeMap(this, _fudgeSurrogateSelector));
        }


        /// <remarks>
        /// NOTE: There's all sorts of bugs with this if the context changes.
        ///         We're largely saved by the fact that we only cache Surrogates we found, not the failure to find one
        /// </remarks>
        private class MemoizingFudgeSurrogateSelector : FudgeSurrogateSelector
        {
            readonly Memoizer<Type, FudgeFieldNameConvention, IFudgeSerializationSurrogate> _memoizer;

            public MemoizingFudgeSurrogateSelector(FudgeContext context)
                : base(context)
            {
                _memoizer = new Memoizer<Type, FudgeFieldNameConvention, IFudgeSerializationSurrogate>(GetSurrogateImpl);
            }

            public override IFudgeSerializationSurrogate GetSurrogate(Type type, FudgeFieldNameConvention fieldNameConvention)
            {
                var fudgeSerializationSurrogate = _memoizer.Get(type, fieldNameConvention);
                Debug.Assert(fudgeSerializationSurrogate != null, "Returning a null surrogate may well expose our bugs");
                return fudgeSerializationSurrogate;
            }

            private IFudgeSerializationSurrogate GetSurrogateImpl(Type type, FudgeFieldNameConvention fieldNameConvention)
            {
                return base.GetSurrogate(type, fieldNameConvention);
            }

        }
    }
}