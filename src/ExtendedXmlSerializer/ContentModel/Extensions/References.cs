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
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Extensions
{
	class References : DecoratedReader, IReferences
	{
		readonly static TypeInfo Reference = typeof(ReferenceIdentity).GetTypeInfo();

		readonly IEntities _entities;

		readonly IParameterizedSource<IXmlReader, IDictionary<ReferenceIdentity, object>> _identities =
			new ReferenceCache<IXmlReader, IDictionary<ReferenceIdentity, object>>(
				_ => new Dictionary<ReferenceIdentity, object>());

		public References(IReader reader, IEntities entities) : base(reader)
		{
			_entities = entities;
		}

/*
		ReferenceIdentity? GetReference(IXmlReader parameter)
		{
			if (parameter.Attributes.Contains(ReferenceProperty.Default))
			{
				return new ReferenceIdentity(Reference, ReferenceProperty.Default.Get(parameter));
			}

			if (parameter.Attributes.Contains(EntityProperty.Default))
			{
				var type = parameter.Classification;
				var reference = new ReferenceIdentity(type, _entities.Get(type).Get(EntityProperty.Default.Get(parameter)));
				return reference;
			}
			return null;
		}
*/

		static ReferenceIdentity? GetIdentity(IXmlReader reader)
		{
			if (reader.Attributes.Contains(IdentityProperty.Default))
			{
				return new ReferenceIdentity(Reference, IdentityProperty.Default.Get(reader));
			}
			return null;
		}

		ReferenceIdentity? GetIdentity(object instance)
		{
			var typeInfo = instance.GetType().GetTypeInfo();
			var entity = _entities.Get(typeInfo);
			if (entity != null)
			{
				return new ReferenceIdentity(Reference, entity.Get(instance));
			}
			return null;
		}

		public sealed override object Get(IXmlReader parameter)
		{
			/*var reference = GetReference(parameter);
			if (reference != null)
			{
				var identities = _identities.Get(parameter);
				var current = identities.Get(reference.Value);
				if (current == null)
				{
					var value = base.Get(parameter);
					identities.Add(reference.Value, value);
				}
				return identity;
			}*/


			var declared = GetIdentity(parameter);
			var result = base.Get(parameter);

			var identity = declared ?? GetIdentity(result);
			if (identity != null)
			{
				var identities = _identities.Get(parameter);
				identities.Add(identity.Value, result);
			}


			return result;
		}
	}
}