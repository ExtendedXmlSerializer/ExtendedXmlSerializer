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

using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.Legacy
{
	sealed class LegacyMemberOption : ConverterOptionBase<IMemberElement>
	{
		readonly IInternalExtendedXmlConfiguration _config;
		readonly IConverterOption _option;

		public LegacyMemberOption(IInternalExtendedXmlConfiguration config, IConverterOption option)
		{
			_config = config;
			_option = option;
		}

		protected override IConverter Create(IMemberElement parameter)
		{
			var result = _option.Get(parameter);
			if (result != null)
			{
				var configuration = _config.GetTypeConfiguration(parameter.Metadata.DeclaringType);
				var propertyConfiguration = configuration?.GetPropertyConfiguration(parameter.Metadata.Name);
				if (propertyConfiguration != null && propertyConfiguration.IsEncrypt)
				{
					var algorithm = _config.EncryptionAlgorithm;
					if (algorithm != null)
					{
						return new EncryptedMemberConverter(algorithm, result);
					}
				}
			}

			return result;
		}
	}
}