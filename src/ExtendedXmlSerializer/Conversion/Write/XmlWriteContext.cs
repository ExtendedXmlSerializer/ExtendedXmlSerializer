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
using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Write
{
    class XmlWriteContext : IWriteContext
    {
        private readonly INamespaces _namespaces;
        private readonly XmlWriter _writer;
        private readonly ImmutableArray<object> _services;
        private readonly IDisposable _finish;

        public XmlWriteContext(XmlWriter writer, IElement element, params object[] services)
            : this(Namespaces.Default, writer, element, services) {}

        protected XmlWriteContext(INamespaces namespaces, XmlWriter writer, IElement element, params object[] services)
            : this(namespaces, writer, element, services.ToImmutableArray()) {}

        XmlWriteContext(INamespaces namespaces, XmlWriter writer, IElement element, ImmutableArray<object> services)
        {
            Current = element;
            _namespaces = namespaces;
            _writer = writer;
            _services = services;
            _finish = new DelegatedDisposable(_writer.WriteEndElement);
        }

        public IElement Current { get; }

        public object GetService(Type serviceType)
        {
            var info = serviceType.GetTypeInfo();
            var length = _services.Length;
            for (int i = 0; i < length; i++)
            {
                var service = _services[i];
                if (info.IsInstanceOfType(service))
                {
                    return service;
                }
            }
            return null;
        }

        public IWriteContext Start(IElement element)
        {
            var ns = _namespaces.Get(element.ReferencedType);
            _writer.WriteStartElement(element.Name, ns);

            var result = new XmlWriteContext(_namespaces, _writer, element, _services);
            return result;
        }

        public void Write(string text) => _writer.WriteString(text);

        public void Write(IElement element, string value) => _writer.WriteAttributeString(element.Name, value);

        public virtual void Dispose() => _finish.Dispose();
    }
}