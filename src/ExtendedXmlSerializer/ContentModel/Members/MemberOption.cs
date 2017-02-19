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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class MemberOption : MemberOptionBase
	{
		readonly ISetterFactory _setter;

		public MemberOption(ISerialization serialization)
			: this(AssignableMemberSpecification.Default, serialization) {}

		public MemberOption(IMemberSpecification specification, ISerialization serialization)
			: this(specification, serialization, SetterFactory.Default) {}

		public MemberOption(IMemberSpecification specification, ISerialization serialization, ISetterFactory setter)
			: base(specification, serialization)
		{
			_setter = setter;
		}

		protected override IMember Create(ISpecification<object> emit, string displayName, TypeInfo classification,
		                                  Func<object, object> getter, ISerializer body, MemberInfo metadata)
			=> CreateMember(displayName, classification, _setter.Get(metadata), getter, body);

		protected virtual IMember CreateMember(string displayName, TypeInfo classification, Action<object, object> setter,
		                                       Func<object, object> getter, ISerializer body)
			=> new Member(AssignedSpecification.Default, displayName, getter, setter, body);
	}
}