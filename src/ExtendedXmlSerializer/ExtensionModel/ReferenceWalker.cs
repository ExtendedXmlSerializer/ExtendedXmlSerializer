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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ReferenceWalker : ObjectWalkerBase<object, IEnumerable<object>>, ISource<IReadOnlyList<object>>
	{
		readonly IMembers _members;
		readonly Func<object, bool> _check;

		public ReferenceWalker(IMembers members, object root) : base(root)
		{
			_members = members;
			_check = Check;
		}

		protected sealed override IEnumerable<object> Select(object input)
		{
			var parameter = input.GetType().GetTypeInfo();
			var members = _members.Get(parameter);
			var length = members.Length;

			for (var i = 0; i < length; i++)
			{
				var member = members[i];
				var variable = member.Adapter as IVariableTypeMemberAdapter;
				if (variable != null)
				{
					var instance = member.Adapter.Get(input);
					if (_check(instance))
					{
						yield return instance;
					}
				}
			}

			var enumerable = input as IEnumerable;
			if (enumerable != null)
			{
				foreach (var item in enumerable.Cast<object>().Where(_check))
				{
					if (!Schedule(item))
					{
						yield return item;
					}
				}
			}
		}

		bool Check(object instance) => (!instance?.GetType().GetTypeInfo().IsValueType ?? false) && !Schedule(instance);

		public IReadOnlyList<object> Get() => this.SelectMany(x => x).Distinct().AsReadOnly();
	}
}