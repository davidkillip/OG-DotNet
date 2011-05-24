//-----------------------------------------------------------------------
// <copyright file="ManageableUnstructuredMarketDataSnapshot.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Fudge;
using Fudge.Serialization;
using OGDotNet.Builders;
using OGDotNet.Mappedtypes.Core.marketdatasnapshot;
using OGDotNet.Mappedtypes.Master.marketdatasnapshot;
using OGDotNet.Model;
using OGDotNet.Model.Context.MarketDataSnapshot;
using OGDotNet.Model.Context.MarketDataSnapshot.Warnings;
using OGDotNet.Utils;

namespace OGDotNet.Mappedtypes.master.marketdatasnapshot
{
    public class ManageableUnstructuredMarketDataSnapshot : INotifyPropertyChanged, IUpdatableFrom<ManageableUnstructuredMarketDataSnapshot>
    {
        private readonly IDictionary<MarketDataValueSpecification, IDictionary<string, ValueSnapshot>> _values;

        public ManageableUnstructuredMarketDataSnapshot(IDictionary<MarketDataValueSpecification, IDictionary<string, ValueSnapshot>> values)
        {
            _values = values;
        }

        public IDictionary<MarketDataValueSpecification, IDictionary<string, ValueSnapshot>> Values
        {
            get { return _values; }
        }

        public bool HaveOverrides()
        {
            return _values.Any(m => m.Value.Any(v => v.Value.OverrideValue.HasValue));
        }

        public void RemoveAllOverrides()
        {
            foreach (var valueSnapshot in _values.Values.SelectMany(m => m.Values))
            {
                valueSnapshot.OverrideValue = null;
            }
        }

        public UpdateAction<ManageableUnstructuredMarketDataSnapshot> PrepareUpdateFrom(ManageableUnstructuredMarketDataSnapshot newSnapshot)
        {
            var currValues = GetUpdateDictionary(Values);
            var newValues = GetUpdateDictionary(newSnapshot.Values);

            return currValues.ProjectStructure(newValues,
                                     PrepareUpdateFrom,
                                     PrepareRemoveAction,
                                     PrepareAddAction
                ).Aggregate((a, b) => a.Concat(b));
        }

        private static Dictionary<MarketDataValueSpecification, IDictionary<string, ValueSnapshot>> GetUpdateDictionary(IDictionary<MarketDataValueSpecification, IDictionary<string, ValueSnapshot>> values)
        {
            return new Dictionary<MarketDataValueSpecification, IDictionary<string, ValueSnapshot>>(
                values,
                IgnoreVersionComparer.Instance
                );
        }
        private class IgnoreVersionComparer : IEqualityComparer<MarketDataValueSpecification>
        {
            public static readonly IEqualityComparer<MarketDataValueSpecification> Instance = new IgnoreVersionComparer();

            private IgnoreVersionComparer()
            {
            }

            public bool Equals(MarketDataValueSpecification x, MarketDataValueSpecification y)
            {
                return x.Type.Equals(y.Type)
                       &&
                       x.UniqueId.ToLatest().Equals(y.UniqueId.ToLatest()); //Ignore the version info
            }

            public int GetHashCode(MarketDataValueSpecification obj)
            {
                int result = obj.Type.GetHashCode();
                result = (result * 397) ^ obj.UniqueId.ToLatest().GetHashCode(); //Ignore the version info
                return result;
            }
        }

        private static UpdateAction<ManageableUnstructuredMarketDataSnapshot> PrepareAddAction(MarketDataValueSpecification marketDataValueSpecification, IDictionary<string, ValueSnapshot> valueSnapshots)
        {
            var clonedValues = Clone(valueSnapshots);
            return new UpdateAction<ManageableUnstructuredMarketDataSnapshot>(
                delegate(ManageableUnstructuredMarketDataSnapshot snap)
                {
                    snap.Values.Add(marketDataValueSpecification, Clone(clonedValues));
                    snap.InvokePropertyChanged("Values");
                }
                );
        }

        private static IDictionary<string, ValueSnapshot> Clone(IDictionary<string, ValueSnapshot> valueSnapshots)
        {
            return valueSnapshots.ToDictionary(k => k.Key, k => k.Value.Clone());
        }

        private static UpdateAction<ManageableUnstructuredMarketDataSnapshot> PrepareRemoveAction(MarketDataValueSpecification marketDataValueSpecification, IDictionary<string, ValueSnapshot> valueSnapshots)
        {
            return new UpdateAction<ManageableUnstructuredMarketDataSnapshot>(
                delegate(ManageableUnstructuredMarketDataSnapshot snap)
                {
                    snap.Values.Remove(marketDataValueSpecification);
                    snap.InvokePropertyChanged("Values");
                },
                OverriddenSecurityDisappearingWarning.Of(marketDataValueSpecification, valueSnapshots)
                );
        }

        private static UpdateAction<ManageableUnstructuredMarketDataSnapshot> PrepareUpdateFrom(MarketDataValueSpecification currSpec, IDictionary<string, ValueSnapshot> currValues, MarketDataValueSpecification newSpec, IDictionary<string, ValueSnapshot> newValues)
        {
            var actions = currValues.ProjectStructure(newValues,
                                                (k, a, b) =>
                                                    {
                                                        var newMarketValue = b.MarketValue;
                                                        return new UpdateAction<ManageableUnstructuredMarketDataSnapshot>(delegate(ManageableUnstructuredMarketDataSnapshot s)
                                                                    {
                                                                        s._values[currSpec][k].MarketValue = newMarketValue;
                                                                    });
                                                    },
                                                (k, v) => PrepareRemoveAction(currSpec, k, v),
                                                (k, v) =>
                                                    {
                                                        var valueSnapshot =v.Clone();
                                                        return new UpdateAction<ManageableUnstructuredMarketDataSnapshot>(
                                                                delegate(ManageableUnstructuredMarketDataSnapshot s)
                                                                    {
                                                                        s._values[currSpec].Add(k, valueSnapshot.Clone());
                                                                        s.InvokePropertyChanged("Values");
                                                                    });
                                                    });

            UpdateAction<ManageableUnstructuredMarketDataSnapshot> ret = UpdateAction<ManageableUnstructuredMarketDataSnapshot>.Of(actions);

            if (!currSpec.Equals(newSpec))
            {//we need to update the key, since we used a non standard comparer
                ret = ret.Concat(
                    new UpdateAction<ManageableUnstructuredMarketDataSnapshot>(delegate(ManageableUnstructuredMarketDataSnapshot s)
                    {
                        var prevValue = s._values[currSpec];
                        s.Values[newSpec] = prevValue;
                        s.InvokePropertyChanged("Values");
                    })
                    );
            }

            return ret;
        }

        private static UpdateAction<ManageableUnstructuredMarketDataSnapshot> PrepareRemoveAction(MarketDataValueSpecification spec, string k, ValueSnapshot v)
        {
            Action<ManageableUnstructuredMarketDataSnapshot> updateAction = delegate(ManageableUnstructuredMarketDataSnapshot s)
                                      {
                                          if (! s._values[spec].Remove(k))
                                          {
                                            throw new InvalidOperationException("Unexpected missing key");   
                                          }
                                          s.InvokePropertyChanged("Values");
                                      };
            return new UpdateAction<ManageableUnstructuredMarketDataSnapshot>(updateAction, OverriddenValueDisappearingWarning.Of(spec, k, v));
        }

        public IEnumerator<KeyValuePair<MarketDataValueSpecification, IDictionary<string, ValueSnapshot>>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public static ManageableUnstructuredMarketDataSnapshot FromFudgeMsg(IFudgeFieldContainer ffc, IFudgeDeserializer deserializer)
        {
            var dictionary = new Dictionary<MarketDataValueSpecification, IDictionary<string, ValueSnapshot>>();
            var enumerable = ffc.GetAllByOrdinal(1).Select(deserializer.FromField<Entry>);
            foreach (var entry in enumerable)
            {
                IDictionary<string, ValueSnapshot> innerDict;
                if (! dictionary.TryGetValue(entry.ValueSpec, out innerDict))
                {
                    innerDict = new Dictionary<string, ValueSnapshot>();
                    dictionary[entry.ValueSpec] = innerDict;
                }
                innerDict.Add(entry.ValueName, entry.Value);
            }
            return new ManageableUnstructuredMarketDataSnapshot(dictionary);
        }

        public void ToFudgeMsg(IAppendingFudgeFieldContainer a, IFudgeSerializer s)
        {
            Type type = typeof(ManageableUnstructuredMarketDataSnapshot);
            s.WriteTypeHeader(a, type);
            foreach (var value in Values)
            {
                foreach (var valueSnapshot in value.Value)
                {
                    var openGammaFudgeContext = (OpenGammaFudgeContext) s.Context;
                    var newMessage = s.Context.NewMessage();
                    newMessage.Add("valueSpec", openGammaFudgeContext.GetSerializer().SerializeToMsg(value.Key));
                    newMessage.Add("valueName", valueSnapshot.Key);
                    newMessage.Add("value", openGammaFudgeContext.GetSerializer().SerializeToMsg(valueSnapshot.Value));
                    a.Add(1, newMessage);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(string propertyName)
        {
            InvokePropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        private void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        public class Entry
        {
            private readonly MarketDataValueSpecification valueSpec;
            private readonly string valueName;
            private readonly ValueSnapshot value;

            public Entry(MarketDataValueSpecification valueSpec, string valueName, ValueSnapshot value)
            {
                this.valueSpec = valueSpec;
                this.valueName = valueName;
                this.value = value;
            }

            public MarketDataValueSpecification ValueSpec
            {
                get { return valueSpec; }
            }

            public string ValueName
            {
                get { return valueName; }
            }

            public ValueSnapshot Value
            {
                get { return value; }
            }
        }

        public ManageableUnstructuredMarketDataSnapshot Clone()
        {
            return new ManageableUnstructuredMarketDataSnapshot(Values.ToDictionary(k => k.Key, k => Clone(k.Value)));
        }
    }
}