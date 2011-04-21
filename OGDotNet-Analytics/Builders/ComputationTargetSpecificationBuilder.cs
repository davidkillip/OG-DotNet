//-----------------------------------------------------------------------
// <copyright file="ComputationTargetSpecificationBuilder.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Fudge;
using Fudge.Serialization;
using OGDotNet.Mappedtypes.engine;
using OGDotNet.Mappedtypes.Id;

namespace OGDotNet.Builders
{
    internal class ComputationTargetSpecificationBuilder : BuilderBase<ComputationTargetSpecification>
    {
        public ComputationTargetSpecificationBuilder(FudgeContext context, Type type) : base(context, type)
        {
        }

        public override ComputationTargetSpecification DeserializeImpl(IFudgeFieldContainer msg, IFudgeDeserializer deserializer)
        {
            ComputationTargetType? type = null;
            UniqueIdentifier uid = null;

            foreach (var field in msg)
            {
                switch (field.Name)
                {
                    case "computationTargetType":
                        type = EnumBuilder<ComputationTargetType>.Parse((string) field.Value);
                        break;
                    case "computationTargetIdentifier":
                        uid = UniqueIdentifier.Parse((string)field.Value);
                        break;
                    default:
                        break;
                }
            }
            return new ComputationTargetSpecification(type.Value, uid);
        }

        public static void AddMessageFields(IFudgeSerializer fudgeSerializer, IAppendingFudgeFieldContainer msg, ComputationTargetSpecification @object)
        {
            msg.Add("computationTargetType", EnumBuilder<ComputationTargetType>.GetJavaName(@object.Type));
            UniqueIdentifier uid = @object.Uid;
            if (uid != null)
            {
                fudgeSerializer.WriteInline(msg, "computationTargetIdentifier", uid);
            }
        }
    }
}