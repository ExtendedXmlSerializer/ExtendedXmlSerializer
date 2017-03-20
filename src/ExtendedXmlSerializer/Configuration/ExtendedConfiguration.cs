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

using System.Linq;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;

namespace ExtendedXmlSerializer.Configuration
{
	public class ExtendedConfiguration : KeyedByTypeCollection<ISerializerExtension>, IConfiguration
	{
		readonly static ServicesFactory ServicesFactory = ServicesFactory.Default;

		readonly IServicesFactory _source;

		public ExtendedConfiguration() : this(DefaultExtensions.Default.ToArray()) {}

		public ExtendedConfiguration(params ISerializerExtension[] extensions) : this(ServicesFactory, extensions) {}

		public ExtendedConfiguration(IServicesFactory source, params ISerializerExtension[] extensions) : base(extensions)
		{
			_source = source;
		}

		public IExtendedXmlSerializer Create()
		{
			using (var services = _source.Get(this))
			{
				var result = services.Get<IExtendedXmlSerializer>();
				return result;
			}
		}

		T IConfiguration.Find<T>() => Find<T>();
	}
}