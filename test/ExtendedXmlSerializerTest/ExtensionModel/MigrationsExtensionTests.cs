﻿// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.Test.Support;
using Xunit;

namespace ExtendedXmlSerialization.Test.ExtensionModel
{
	public class MigrationsExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var migrations = new Dictionary<TypeInfo, IExtendedXmlTypeMigrator>
			                 {
				                 {
					                 typeof(Subject).GetTypeInfo(),
					                 new Migrations {new PropertyMigration("OldPropertyName", nameof(Subject.PropertyName))}
				                 }
			                 };
			var support =
				new SerializationSupport(new ExtendedXmlConfiguration().Extended(new MigrationsExtension(migrations)).Create());
			var instance = new Subject {PropertyName = "Hello World!"};
			support.Assert(instance,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><MigrationsExtensionTests-Subject xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:version=""1"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><PropertyName>Hello World!</PropertyName></MigrationsExtensionTests-Subject>");

			var data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><MigrationsExtensionTests-Subject xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><OldPropertyName>Hello World from Old Property!</OldPropertyName></MigrationsExtensionTests-Subject>";
			var migrated = support.Deserialize<Subject>(data);
			Assert.NotNull(migrated);
			Assert.Equal("Hello World from Old Property!", migrated.PropertyName);
		}

		class Subject
		{
			public string PropertyName { get; set; }
		}
	}
}