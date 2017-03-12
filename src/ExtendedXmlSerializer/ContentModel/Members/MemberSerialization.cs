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

using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	sealed class MemberSerialization : TableSource<string, IMemberSerializer>, IMemberSerialization
	{
		readonly IRuntimeMemberList _serializers;
		readonly ImmutableArray<IMemberSerializer> _all;

		public MemberSerialization(IRuntimeMemberList serializers, IDictionary<string, IMemberSerializer> store,
		                           ImmutableArray<IMemberSerializer> all) : base(store)
		{
			_serializers = serializers;
			_all = all;
		}

		public ImmutableArray<IMemberSerializer> Get(object parameter) => _serializers.Get(parameter);


		public ImmutableArray<IMemberSerializer> Get() => _all;
	}
}