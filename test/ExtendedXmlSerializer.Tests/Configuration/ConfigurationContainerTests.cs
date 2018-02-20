// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nag�rski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Content.Registration;
using ExtendedXmlSerializer.ExtensionModel.Encryption;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using ExtendedXmlSerializer.Tests.TestObject;
using JetBrains.Annotations;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.Configuration
{
	public class ConfigurationContainerTests
	{
		const string Testclass = "UpdatedTestClassName", MemberName = "UpdatedMemberName";

		static IConfigurationElement Configure(Action<IConfigurationElement> configure)
		{
			var result = new ConfigurationContainer();
			configure(result);
			return result;
		}

		[Fact]
		public void ConfigureType()
		{
			var config = Configure(cfg => cfg.ConfigureType<TestClassPrimitiveTypes>());

			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.NotNull(configType);
			Assert.Same(configType, config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes)));

			Assert.NotNull(config.GetTypeConfiguration(typeof(TestClassPrimitiveTypesNullable)));
		}

		[Fact]
		public void ConfigureNameForType()
		{
			var configuration = new ConfigurationContainer();
			var name = configuration.Type<SimpleTestSubject>().Name(Testclass);

			Assert.Equal(RegisteredNamesProperty.Default.Get(name), Testclass);
			Assert.False(RegisteredNamesProperty.Default.IsSatisfiedBy(configuration.Type<TestClassPrimitiveTypesNullable>()));

			var support = new SerializationSupport(configuration);
			var expected = new SimpleTestSubject {BasicProperty = "Hello World!"};
			var actual = support.Assert(expected,
										@"<?xml version=""1.0"" encoding=""utf-8""?><UpdatedTestClassName xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests""><BasicProperty>Hello World!</BasicProperty></UpdatedTestClassName>");
			Assert.Equal(expected.BasicProperty, actual.BasicProperty);
		}

		[Fact]
		public void ConfigureEntityType()
		{
			var configuration = new ConfigurationContainer();

			Assert.Null(configuration.Extensions.FirstOf<ReferencesExtension>());
			configuration.EnableReferences();
			var configType = configuration.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			var extension = configuration.Extensions.FirstOf<ReferencesExtension>();
			Assert.NotNull(extension);
			Assert.Null(extension.Get(configType.Type()));
		}

		[Fact]
		public void ConfigureMigrationForType()
		{
			var configuration = Configure(cfg => cfg.ConfigureType<TestClassPrimitiveTypes>().AddMigration(xml => { }));
			var type = configuration.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.Single(configuration.Extend<MigrationsExtension>().Get(type.Type()));
		}

		[Fact]
		public void ConfigureCustomSerializerForType()
		{
			var serializers = RegisteredContentSerializers<TestClassPrimitiveTypes>.Default;
			var config = Configure(cfg =>
								   {
									   var t = cfg.ConfigureType<TestClassPrimitiveTypes>();
									   Assert.False(serializers.IsSatisfiedBy(t.Get()));
									   t.CustomSerializer((writer, types) => { }, element => null);
								   });
			var type = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.True(serializers.IsSatisfiedBy(type.Get()));
		}

		[Fact]
		public void ConfigureProperty()
		{
			var config = Configure(cfg => cfg.ConfigureType<TestClassPrimitiveTypes>().Member(p => p.PropChar));
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.NotNull(configType.Member("PropChar"));
			Assert.Null(configType.Member("TheNewPropertyThatDoesNotExist"));
		}

		[Fact]
		public void ConfigurePropertyAsId()
		{
			var configuration =
				Configure(cfg => cfg.ConfigureType<TestClassPrimitiveTypes>().EnableReferences(p => p.PropChar));
			var configType = configuration.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			var property = configType.Member("PropChar");
			Assert.NotNull(property);

			var extension = configuration.Extensions.FirstOf<ReferencesExtension>();
			Assert.NotNull(extension);
			Assert.Same(property.Member(), extension.Get(configType.Type()));
		}

		[Fact]
		public void ConfigureNameForProperty()
		{
			var configuration =
				Configure(cfg => cfg.ConfigureType<SimpleTestSubject>().Member(p => p.BasicProperty).Name(MemberName));

			var member =
				configuration.GetTypeConfiguration(typeof(SimpleTestSubject)).Member(nameof(SimpleTestSubject.BasicProperty));

			Assert.Equal(RegisteredNamesProperty.Default.Get(member), MemberName);

			var support = new SerializationSupport(configuration);
			var instance = new SimpleTestSubject {BasicProperty = "Hello World!  Testing Member."};
			support.Assert(instance,
						   @"<?xml version=""1.0"" encoding=""utf-8""?><ConfigurationContainerTests-SimpleTestSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests""><UpdatedMemberName>Hello World!  Testing Member.</UpdatedMemberName></ConfigurationContainerTests-SimpleTestSubject>");
		}

		[Fact]
		public void ConfigureOrderForProperty()
		{
			var order = 0;
			var configuration = Configure(cfg =>
											  cfg.ConfigureType<SimpleOrderedTestSubject>().Member(p => p.Property2).Order(order));
			var member =
				configuration.GetTypeConfiguration(typeof(SimpleOrderedTestSubject))
							 .Member(nameof(SimpleOrderedTestSubject.Property2));
			Assert.Equal(configuration.Extensions.FirstOf<MemberOrderingExtension>().Get(member.Member()), order);

			var instance = new SimpleOrderedTestSubject {Property2 = "World!", Property1 = "Hello"};

			new SerializationSupport().Assert(instance,
											  @"<?xml version=""1.0"" encoding=""utf-8""?><ConfigurationContainerTests-SimpleOrderedTestSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests""><Property1>Hello</Property1><Property2>World!</Property2></ConfigurationContainerTests-SimpleOrderedTestSubject>");

			new SerializationSupport(configuration).Assert(instance,
														   @"<?xml version=""1.0"" encoding=""utf-8""?><ConfigurationContainerTests-SimpleOrderedTestSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests""><Property2>World!</Property2><Property1>Hello</Property1></ConfigurationContainerTests-SimpleOrderedTestSubject>");
		}

		[Fact]
		public void ConfigurePropertyAsAttribute()
		{
			var configuration =
				Configure(cfg => { cfg.ConfigureType<SimpleTestSubject>().Member(p => p.BasicProperty).Attribute(); });
			var member = configuration.GetTypeConfiguration(typeof(SimpleTestSubject))
				.Member(nameof(SimpleTestSubject.BasicProperty));
			Assert.True(configuration.Extend<MemberFormatExtension>().Registered.Contains(member.Member()));

			var instance = new SimpleTestSubject {BasicProperty = "Hello World as Attribute!"};
			new SerializationSupport(configuration).Assert(instance,
														   @"<?xml version=""1.0"" encoding=""utf-8""?><ConfigurationContainerTests-SimpleTestSubject BasicProperty=""Hello World as Attribute!"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests"" />");
		}

		[Fact]
		public void ConfigureEncrypt()
		{
			var configuration = new ConfigurationContainer();
			Assert.Null(configuration.Extensions.FirstOf<EncryptionExtension>());
			configuration.UseEncryptionAlgorithm()
			             .ConfigureType<TestClassWithEncryptedData>()
			             .Member(p => p.Password, x => x.Encrypt())
			             .Member(p => p.Salary)
			             .Encrypt();

			var extension = configuration.Extensions.FirstOf<EncryptionExtension>();
			Assert.NotNull(extension);
			var type = configuration.GetTypeConfiguration(typeof(TestClassWithEncryptedData));
			Assert.NotNull(type);

			var member = type.Member(nameof(TestClassWithEncryptedData.Salary));
			var property = member.Member();
			Assert.NotNull(property);
			Assert.Contains(property, extension.Registered);

			const int salary = 6776;
			var instance = new TestClassWithEncryptedData { Salary = salary };
			var support = new SerializationSupport(configuration);
			var actual = support.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithEncryptedData xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Salary>Njc3Ng==</Salary></TestClassWithEncryptedData>");
			Assert.Equal(salary, actual.Salary);
		}

		[Fact]
		public void ConfigureEncryptDifferenetOrder()
		{
			var container = new ConfigurationContainer();
			container.UseAutoFormatting();


			Assert.Null(container.Extensions.FirstOf<EncryptionExtension>());
			container.ConfigureType<TestClassWithEncryptedData>()
			      .Member(p => p.Password, x => x.Encrypt())
			      .Member(p => p.Salary)
			      .Encrypt();

			container.UseEncryptionAlgorithm();

			var extension = container.Extensions.FirstOf<EncryptionExtension>();
			Assert.NotNull(extension);
			var type = container.GetTypeConfiguration(typeof(TestClassWithEncryptedData));
			Assert.NotNull(type);

			var member = type.Member(nameof(TestClassWithEncryptedData.Salary));
			var property = member.Member();
			Assert.NotNull(property);
			Assert.Contains(property, extension.Registered);

			const int salary = 6776;
			var instance = new TestClassWithEncryptedData { Salary = salary };
			var actual = new SerializationSupport(container).Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithEncryptedData Salary=""Njc3Ng=="" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests"" />");
			Assert.Equal(salary, actual.Salary);
		}

		class SimpleTestSubject
		{
			public string BasicProperty { get; set; }
		}

		class SimpleOrderedTestSubject
		{
			public string Property1 { [UsedImplicitly] get; set; }
			public string Property2 { get; set; }
		}
	}
}