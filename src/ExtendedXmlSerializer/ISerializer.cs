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

using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization
{
	public interface ISerializer
	{
		void Serialize(Stream stream, object instance);

		object Deserialize(Stream stream);
	}

	public class Serializer : ISerializer
	{
		public static Serializer Default { get; } = new Serializer();
		Serializer() : this(RootConverter.Default) {}
		readonly IConverter _converter;
		readonly IElements _elements;

		public Serializer(IConverter converter) : this(converter, Elements.Default) {}

		public Serializer(IConverter converter, IElements elements)
		{
			_converter = converter;
			_elements = elements;
		}

		public void Serialize(Stream stream, object instance)
		{
			using (var writer = XmlWriter.Create(stream))
			{
				_converter.Write(CreateWriteContext(writer, instance.GetType().GetTypeInfo()), instance);
			}
		}

		protected virtual IWriteContext CreateWriteContext(XmlWriter writer, TypeInfo type)
			=> new XmlWriteContext(writer, new Root(_elements.Build(type)));

		public object Deserialize(Stream stream)
		{
			var context = CreateContext(stream);
			var result = _converter.Read(context);
			return result;
		}

		protected virtual IReadContext CreateContext(Stream stream)
		{
			var text = new StreamReader(stream).ReadToEnd();
			var document = XDocument.Parse(text);
			var context = new XmlReadContext(document.Root);
			return context;
		}
	}
}