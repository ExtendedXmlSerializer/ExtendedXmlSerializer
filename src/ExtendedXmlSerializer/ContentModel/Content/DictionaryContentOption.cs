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

using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class DictionaryContentOption : ContentOptionBase
	{
		readonly static ILists Lists = new Lists(DictionaryAddDelegates.Default);

		readonly static AllSpecification<TypeInfo> Specification =
			new AllSpecification<TypeInfo>(IsActivatedTypeSpecification.Default, IsDictionaryTypeSpecification.Default);

		readonly IMembers _members;
		readonly IDictionaryItems _items;
		readonly IActivators _activators;

		public DictionaryContentOption(IMembers members, IMemberOption variable)
			: this(members, new DictionaryItems(variable), Activators.Default) {}

		public DictionaryContentOption(IMembers members, IDictionaryItems items, IActivators activators)
			: base(Specification)
		{
			_members = members;
			_items = items;
			_activators = activators;
		}

		public override ISerializer Get(TypeInfo parameter)
		{
			var item = _items.Get(parameter);
			var members = _members.Get(parameter);
			var activator = new DelegatedFixedActivator(_activators.Get(parameter.AsType()));

			var reader = new ActivatedContentsReader(
				activator,
				new MemberedCollectionContentsReader(members, item, Lists)
			);

			var writer = new MemberedCollectionWriter(new MemberWriter(members), new DictionaryEntryWriter(item));
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}