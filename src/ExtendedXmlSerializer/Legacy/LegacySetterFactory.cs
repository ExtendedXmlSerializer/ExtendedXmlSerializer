// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
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

using System;
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.Legacy
{
	class LegacySetterFactory : ISetterFactory
	{
		readonly IInternalExtendedXmlConfiguration _config;
		readonly ISetterFactory _factory;

		public LegacySetterFactory(IInternalExtendedXmlConfiguration config, ISetterFactory factory)
		{
			_config = config;
			_factory = factory;
		}

		public Action<object, object> Get(MemberInfo parameter)
		{
			var result = _factory.Get(parameter);
			var configuration = _config.GetTypeConfiguration(parameter.DeclaringType);
			var propertyConfig = configuration?.GetPropertyConfiguration(parameter.Name);
			if (propertyConfig != null && propertyConfig.IsEncrypt)
			{
				var algorithm = _config.EncryptionAlgorithm;
				if (algorithm != null)
				{
					return new Decrypt(algorithm, result).Assign;
				}
			}

			return result;
		}

		sealed class Decrypt
		{
			readonly IPropertyEncryption _encryption;
			readonly Action<object, object> _source;

			public Decrypt(IPropertyEncryption encryption, Action<object, object> source)
			{
				_encryption = encryption;
				_source = source;
			}

			public void Assign(object instance, object value)
			{
				var text = _encryption.Decrypt(value as string ?? value.ToString());
				_source(instance, text);
			}
		}
	}
}