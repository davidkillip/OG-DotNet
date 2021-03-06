//-----------------------------------------------------------------------
// <copyright file="RemoteEngineContextTests.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using OGDotNet.Model.Context;
using OGDotNet.Tests.Integration.Properties;
using Xunit;
using FactAttribute = OGDotNet.Tests.Integration.Xunit.Extensions.FactAttribute;

namespace OGDotNet.Tests.Integration.OGDotNet.Model.Context
{
    public class RemoteEngineContextTests
    {
        [Fact]
        public void CanCreate()
        {
            GetContext();
        }

        [Fact]
        public void RootUriAsExpected()
        {
            Assert.Equal(new Uri(Settings.Default.ServiceUri), GetContext().RootUri);
        }

        [Fact]
        public void CanGetAllServices()
        {
            var context = GetContext();
            var type = context.GetType();
            foreach (var prop in type.GetProperties())
            {
                var service = prop.GetGetMethod().Invoke(context, new object[] { });
                if (service == null)
                {
                    throw new Exception("Null service " + prop.Name);
                }
            }
            foreach (var method in type.GetMethods().Where(m => !m.IsSpecialName && m.GetParameters().Count() == 0 && m.DeclaringType != typeof(object)))
            {
                var result = method.Invoke(context, new object[] {});
                if (result == null)
                {
                    throw new Exception("Null service " + method.Name);
                }
                if (result is IDisposable)
                {
                    ((IDisposable) result).Dispose();
                }
            }
        }

        internal static RemoteEngineContext GetContext()
        {
            var remoteEngineContextFactory = RemoteEngineContextFactoryTests.GetContextFactory();
            return remoteEngineContextFactory.CreateRemoteEngineContext();
        }
    }
}
