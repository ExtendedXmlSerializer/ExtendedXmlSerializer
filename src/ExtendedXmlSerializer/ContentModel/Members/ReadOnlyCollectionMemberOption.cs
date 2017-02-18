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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class ReadOnlyCollectionMemberOption : MemberOptionBase
	{
		readonly static MemberTypeSpecification Specification =
			new MemberTypeSpecification(IsCollectionTypeSpecification.Default);

		readonly IAddDelegates _add;

		public ReadOnlyCollectionMemberOption(ISerializers serializers) : this(serializers, AddDelegates.Default) {}

		public ReadOnlyCollectionMemberOption(ISerializers serializers, IAddDelegates add) : base(Specification, serializers)
		{
			_add = add;
		}

		protected override IMember Create(ISpecification<object> emit, string displayName, TypeInfo classification,
		                                  Func<object, object> getter, ISerializer body, MemberInfo metadata)
		{
			var add = _add.Get(classification);
			var result = add != null ? new ReadOnlyCollectionMember(emit, displayName, getter, add, body) : null;
			return result;
		}

		class ReadOnlyCollectionMember : Member
		{
			public ReadOnlyCollectionMember(ISpecification<object> emit, string displayName, Func<object, object> getter,
			                                Action<object, object> add,
			                                ISerializer context)
				: base(new AllSpecification<object>(ContainsItemsSpecification.Default, emit), displayName, getter, add, context) {}

			public override void Assign(object instance, object value)
			{
				var collection = Get(instance);
				foreach (var element in (IEnumerable) value)
				{
					base.Assign(collection, element);
				}
			}
		}
	}
}