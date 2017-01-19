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

using System.Collections;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Legacy
{
	sealed class EnumerableReferenceWriter : DecoratedWriter
	{
		readonly IInternalExtendedXmlConfiguration _config;

		public EnumerableReferenceWriter(IInternalExtendedXmlConfiguration config, IWriter writer) : base(writer)
		{
			_config = config;
		}

		public override void Write(IWriteContext context, object instance)
		{
			var element = ((ICollectionElement) context.Element).Item.Classification.AsType();
			var configuration = _config.GetTypeConfiguration(element);
			if (configuration?.IsObjectReference ?? false)
			{
				var references = context.Get<WriteReferences>();
				foreach (var item in (IEnumerable) instance)
				{
					if (!references.Contains(item) && !references.Reserved.Contains(item))
					{
						references.Reserved.Add(item);
					}
				}
			}

			base.Write(context, instance);
		}
	}
}