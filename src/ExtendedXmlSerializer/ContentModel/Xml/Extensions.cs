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

using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	public static class Extensions
	{
		public static TypeInfo Classification(this IXmlReader @this) => TypeExtractor.Default.Get(@this);

		public static void Property<T>(this IXmlWriter @this, IProperty<T> property, T instance)
			=> @this.Attribute(property.Name, property.Format(instance));

		public static T Property<T>(this IXmlReader @this, IProperty<T> property) => property.Parse(@this[property.Name]);

		public static TypeInfo Property(this IXmlReader @this, IQualifiedNameProperty property)
		{
			var data = @this[property.Name];
			var result = data != null ? @this.Get(property.Parse(data)) : null;
			return result;
		}

		public static TypeInfo Property(this IXmlReader @this, IQualifiedNameArgumentsProperty property)
		{
			var data = @this[property.Name];
			var result = data != null ? @this.Get(new QualifiedName(@this.Name.LocalName, @this.Name.NamespaceName, property.Parse(data).ToImmutableArray)) : null;
			return result;
		}
	}
}