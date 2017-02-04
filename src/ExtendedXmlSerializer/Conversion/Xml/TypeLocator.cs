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
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	class TypeLocator : ITypeLocator
	{
		public static TypeLocator Default { get; } = new TypeLocator();
		TypeLocator() : this(Types.Default) {}

		readonly ITypes _types;

		public TypeLocator(ITypes types)
		{
			_types = types;
		}

		public TypeInfo Get(System.Xml.XmlReader parameter)
		{
			switch (parameter.MoveToContent())
			{
				case XmlNodeType.Element:
					var name = XName.Get(parameter.LocalName, parameter.NamespaceURI);
					var result = _types.Get(name);
					return result;
			}

			throw new InvalidOperationException($"Could not locate the type from the current Xml reader '{parameter}.'");
		}
	}
}