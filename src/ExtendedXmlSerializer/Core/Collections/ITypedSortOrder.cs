// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nag�rski
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

using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Collections
{
	interface ITypedSortOrder : ITypedTable<int> {}

	public interface IGrouping<T> : ICollection<T>
	{
		string Name { get; }
	}

	public interface IGroup<T> : IEnumerable<T>, IParameterizedSource<string, ICollection<T>> {}

	class Group<T> : DelegatedSource<string, ICollection<T>>, IGroup<T>
	{
		readonly IOrderedDictionary<string, ICollection<T>> _store;

		public Group() : this(new OrderedDictionary<string, ICollection<T>>()) {}

		public Group(IOrderedDictionary<string, ICollection<T>> store) : base(store.GetValue) => _store = store;


		public IEnumerator<T> GetEnumerator() => _store.SelectMany(x => x.Value)
		                                               .GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class Grouping<T> : List<T>, IGrouping<T>
	{
		public Grouping(string name) : this(name, Enumerable.Empty<T>()) {}

		public Grouping(string name, IEnumerable<T> collection) : base(collection)
		{
			Name = name;
		}

		public string Name { get; }
	}
}