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
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class XmlWriter : IWriter
	{
		readonly System.Xml.XmlWriter _writer;
		readonly IDisposable _finish;

		public XmlWriter(System.Xml.XmlWriter writer) : this(writer, new DelegatedDisposable(writer.WriteEndElement)) {}

		public XmlWriter(System.Xml.XmlWriter writer, IDisposable finish)
		{
			_writer = writer;
			_finish = finish;
		}

		public IDisposable Emit(IElement element)
		{
			var xml = element as IXmlElement;
			if (xml != null)
			{
				_writer.WriteStartElement(element.DisplayName, xml.Namespace);
			}
			else
			{
				_writer.WriteStartElement(element.DisplayName);
			}

			var writable = element as IXmlWritable;
			writable?.Execute(_writer);

			return _finish;
		}

		public void Write(string text) => _writer.WriteString(text);
		public void Render(IRender render, object instance)
		{
			var xml = render as IRenderXml;
			xml?.Render(_writer, instance);
		}
	}
}