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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	sealed class Types : ReferenceCacheBase<IIdentity, TypeInfo>, ITypes
	{
		readonly static Dictionary<IIdentity, TypeInfo> Aliased = WellKnownAliases.Default
		                                                                          .Select(x => x.Key)
		                                                                          .YieldMetadata()
		                                                                          .ToDictionary(Identities.Default.Get);

		public static Types Default { get; } = new Types();

		Types()
			: this(HasAliasSpecification.Default, TypeFormatter.Default, TypeLoader.Default, AssemblyTypePartitions.Default) {}

		readonly IDictionary<IIdentity, TypeInfo> _aliased;

		readonly ITypes _known, _partitions;

		public Types(ISpecification<TypeInfo> specification, ITypeFormatter formatter, params ITypePartitions[] partitions)
			: this(Aliased, new IdentityPartitionedTypes(specification, formatter), new PartitionedTypes(partitions)) {}

		public Types(IDictionary<IIdentity, TypeInfo> aliased, ITypes known, ITypes partitions)
		{
			_aliased = aliased;
			_known = known;
			_partitions = partitions;
		}

		protected override TypeInfo Create(IIdentity parameter)
			=> _aliased.Get(parameter) ?? _known.Get(parameter) ?? _partitions.Get(parameter);
	}
}