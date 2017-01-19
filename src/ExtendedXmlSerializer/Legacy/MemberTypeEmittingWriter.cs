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
using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Write;

namespace ExtendedXmlSerialization.Legacy
{
	class MemberTypeEmittingWriter : TypeEmittingWriterBase
	{
		readonly static TypeInfo
			TypeObject = typeof(object).GetTypeInfo();

		public MemberTypeEmittingWriter(IWriter writer) : base(writer) {}

		protected override bool Emit(IWriteContext context, object instance, TypeInfo type)
		{
			var declaredType = ((XmlWriteContext) context).Parent.Container.GetDeclaredType(context.Container);

			var primitive = declaredType.IsValueType || declaredType.IsPrimitive ||
			                Type.GetTypeCode(declaredType.AsType()) != TypeCode.Object;
			var result = Equals(declaredType, TypeObject) ||
			             !primitive && (!Equals(declaredType, type) || CheckInstance(context, instance));
			return result;
		}


		protected virtual bool CheckInstance(IWriteContext context, object instance)
		{
			return !(instance is IEnumerable);
		}
	}
}