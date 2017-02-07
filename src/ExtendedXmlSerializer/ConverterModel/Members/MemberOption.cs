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
using ExtendedXmlSerialization.ConverterModel.Elements;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Members
{
	class MemberOption : MemberOptionBase
	{
		readonly ISpecification<TypeInfo> _specification;

		readonly ISetterFactory _setter;

		public MemberOption(IContainers containers)
			: this(FixedTypeSpecification.Default, containers, MemberAliasProvider.Default, SetterFactory.Default) {}

		public MemberOption(ISpecification<TypeInfo> specification, IContainers containers, IAliasProvider alias,
		                    ISetterFactory setter)
			: base(new DelegatedSpecification<MemberInformation>(x => x.Assignable), containers, alias)
		{
			_specification = specification;
			_setter = setter;
		}

		protected override IMember Create(string displayName, TypeInfo classification, Func<object, object> getter,
		                                  IConverter body, MemberInfo metadata)
			=> CreateMember(displayName, classification, _setter.Get(metadata), getter, body);

		protected virtual IMember CreateMember(string displayName, TypeInfo classification, Action<object, object> setter,
		                                       Func<object, object> getter, IConverter body)
		{
			var result = _specification.IsSatisfiedBy(classification)
				? new Member(displayName, getter, setter, body)
				: (IMember)new VariableTypeMember(displayName, classification, getter, setter, body);
			return result;
		}
	}
}