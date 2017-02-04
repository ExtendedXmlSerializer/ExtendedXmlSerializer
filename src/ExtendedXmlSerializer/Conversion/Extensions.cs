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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	public static class Extensions
	{
		public static ImmutableArray<string> ToStringArray(this string target, params char[] delimiters) =>
			target.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToImmutableArray();

		/*public static TypeInfo ToTypeInfo(this MemberInfo @this) => @this as TypeInfo ?? @this.DeclaringType.GetTypeInfo();

		public static object AnnotationAll(this XElement @this, Type type) 
			=> @this.Annotation(type) ?? @this.Parent?.AnnotationAll(type);*/

		public static TypeInfo AccountForNullable(this TypeInfo @this)
			=> Nullable.GetUnderlyingType(@this.AsType())?.GetTypeInfo() ?? @this;


		/*public static T Activate<T>(this IActivators @this, Type type) => (T) @this.Get(type).Invoke();*/

		/*public static IElement Load(this IElements @this, IContainer container, TypeInfo instanceType)
			=> container.Exact(instanceType) ? container.Element : @this.Get(instanceType);*/

		// public static bool Exact(this IElement @this, object instance) => Exact(@this, instance.GetType());

		// public static bool Exact(this IElement @this, Type type) => type == @this.Classification.AsType();

		/*public static IElement Actual(this IElements @this, IElement definition, object instance)
			=> Actual(@this, definition.Classification.AsType(), instance);*/

		/*public static IXmlElement Actual(this IEmitters @this, Type definition, object instance)
		{
			var type = instance.GetType();
			var result = definition != type ? (IXmlElement)@this.Get(type.GetTypeInfo()) : null;
			return result;
		}*/
	}
}