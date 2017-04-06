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
using System.Xml.Linq;
using ExtendedXmlSerializer.ContentModel.Xml.Namespacing;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class XmlWriterSupport : IXmlWriterSupport
	{
		readonly static Func<INamespaces> Namespaces = Activators.Default.New<Namespaces>;
		readonly static string Xmlns = XNamespace.Xmlns.NamespaceName;


		readonly System.Xml.XmlWriter _writer;
		readonly Source _source;

		public XmlWriterSupport(System.Xml.XmlWriter writer) : this(writer, Namespaces) {}

		public XmlWriterSupport(System.Xml.XmlWriter writer, Func<INamespaces> namespaces)
			: this(writer, new Source(namespaces)) {}

		XmlWriterSupport(System.Xml.XmlWriter writer, Source source)
		{
			_writer = writer;
			_source = source;
		}

		public string Get(string identifier) => _writer.LookupPrefix(identifier ?? string.Empty) ?? Create(identifier);

		string Create(string identifier)
		{
			var source = _source;
			_writer.WriteAttributeString(source.Get().Get(identifier).Name, Xmlns, identifier);
			return _writer.LookupPrefix(identifier);
		}

		struct Source : ISource<INamespaces>
		{
			readonly Func<INamespaces> _source;

			INamespaces _field;

			public Source(Func<INamespaces> source, INamespaces field = null)
			{
				_source = source;
				_field = field;
			}

			public INamespaces Get() => _field ?? (_field = _source());
		}
	}
}