﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Omex.Extensions.Logging.UnitTests
{
	[TestClass]
	public class ServiceCollectionTests
	{
		[TestMethod]
		public void AddOmexServiceContextShouldRegisterServiceContext()
		{
			IServiceCollection collection = new ServiceCollection()
				.AddOmexServiceContext<MockServiceContext>();

			ValidateTypeRegistration<IServiceContext>(collection);
		}


		[TestMethod]
		public void AddOmexServiceContextShouldOverrrideContextType()
		{
			IServiceCollection collection = new ServiceCollection()
				.AddOmexServiceContext<MockServiceContext>()
				.AddOmexLogging();

			IServiceContext context = ValidateTypeRegistration<IServiceContext>(collection);

			Assert.IsInstanceOfType(context, typeof(MockServiceContext), "Call of AddOmexServiceContext before AddOmexLogging should override IServiceCollection implementation");
		}


		[TestMethod]
		public void AddOmexLoggerToServiceCollectionShouldBeResolvable()
		{
			IServiceCollection collection = new ServiceCollection().AddOmexLogging();
			ValidateTypeRegistration<ILogger<ServiceCollectionTests>>(collection);
		}


		[TestMethod]
		public void AddOmexLoggerToLogBuilderShouldBeResolvable()
		{
			ILoggingBuilder builder = new MockLoggingBuilder().AddOmexLogging();
			ValidateTypeRegistration<ILogger<ServiceCollectionTests>>(builder.Services);
		}


		private T ValidateTypeRegistration<T>(IServiceCollection collection)
			where T : class
		{
			T obj = collection
				.AddOmexLogging()
				.BuildServiceProvider(new ServiceProviderOptions
				{
					ValidateOnBuild = true,
					ValidateScopes = true
				}).GetService<T>();

			Assert.IsNotNull(obj);

			return obj;
		}


		private class MockLoggingBuilder : ILoggingBuilder
		{
			public IServiceCollection Services { get; } = new ServiceCollection();
		}


		private class MockServiceContext : IServiceContext
		{
			public Guid GetPartitionId() => Guid.Empty;

			public long GetReplicaOrInstanceId() => 0;
		}
	}
}
