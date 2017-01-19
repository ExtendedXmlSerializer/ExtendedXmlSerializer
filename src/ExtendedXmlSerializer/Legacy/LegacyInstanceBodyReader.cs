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
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Legacy
{
	sealed class LegacyInstanceBodyReader : InstanceBodyReader
	{
		readonly IInternalExtendedXmlConfiguration _config;

		public LegacyInstanceBodyReader(IInternalExtendedXmlConfiguration config, IReader reader)
			: base(reader)
		{
			_config = config;
		}

		protected override object Activate(IReadContext context)
		{
			var result = base.Activate(context);
			var type = context.Element.Classification;
			var configuration = _config.GetTypeConfiguration(type.AsType());
			if (configuration != null && configuration.IsObjectReference)
			{
				var prefix = type.FullName + Defaults.Underscore;
				var refId = context[ReferenceProperty.Default];
				var references = context.Get<ReadReferences>();
				if (!string.IsNullOrEmpty(refId))
				{
					var key = prefix + refId;
					if (references.ContainsKey(key))
					{
						return references[key];
					}
					references.Add(key, result);
				}
				else
				{
					var objectId = context[IdentifierProperty.Default];
					if (!string.IsNullOrEmpty(objectId))
					{
						var key = prefix + objectId;
						if (references.ContainsKey(key))
						{
							return references[key];
						}
						references.Add(key, result);
					}
				}
			}
			return result;
		}
	}
}