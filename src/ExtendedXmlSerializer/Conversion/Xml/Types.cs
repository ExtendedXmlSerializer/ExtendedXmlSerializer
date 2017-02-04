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

using System.Collections.Immutable;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class Types : WeakCacheBase<XName, TypeInfo>, ITypes
	{
		public static Types Default { get; } = new Types();
		Types() : this(Elements.Defaults.Elements, TypeContexts.Default) {}

		readonly ImmutableArray<IXmlElement> _known;
		readonly ITypeContexts _sources;

		
		public Types(ImmutableArray<IXmlElement> known, ITypeContexts sources)
		{
			_known = known;
			_sources = sources;
		}

		protected override TypeInfo Create(XName parameter)
			=> Known(parameter) ?? _sources.Get(parameter.NamespaceName)?.Invoke(parameter.LocalName);

		TypeInfo Known(XName parameter)
		{
			var localName = parameter.LocalName;
			var ns = parameter.NamespaceName;
			var length = _known.Length;
			for (var i = 0; i < length; i++)
			{
				var name = _known[i];
				if (ns == name.Namespace && localName == name.DisplayName)
				{
					return name.Classification;
				}
			}
			return null;
		}
	}
}