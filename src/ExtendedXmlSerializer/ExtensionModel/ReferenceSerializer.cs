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

using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ReferenceSerializer : ISerializer
	{
		readonly IStoredEncounters _encounters;
		readonly IReferences _references;
		readonly ISerializer _serializer;

		public ReferenceSerializer(IStoredEncounters encounters, IReferences references, ISerializer serializer)
		{
			_encounters = encounters;
			_references = references;
			_serializer = serializer;
		}

		public void Write(IXmlWriter writer, object instance)
		{
			var context = _encounters.Get(writer).Get(instance);
			if (context != null)
			{
				var identifier = context.Value.Identifier;

				var first = context.Value.FirstEncounter;
				if (identifier.Entity != null)
				{
					if (!first)
					{
						EntityProperty.Default.Write(writer, identifier.Entity.Get(instance));
					}
				}
				else
				{
					var property = first ? (IProperty<uint>) IdentityProperty.Default : ReferenceProperty.Default;
					property.Write(writer, identifier.UniqueId);
				}

				if (!first)
				{
					return;
				}
			}
			_serializer.Write(writer, instance);
		}

		public object Get(IXmlReader parameter) => _references.Get(parameter);
	}
}