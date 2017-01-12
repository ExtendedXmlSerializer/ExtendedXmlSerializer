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

using System.Globalization;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    sealed class LegacyInstanceTypeConverter : TypeConverter
    {
        public LegacyInstanceTypeConverter(ISerializationToolsFactory tools, IConverter converter)
            : this(tools, new ConvertMembers(tools).Get(converter)) {}

        LegacyInstanceTypeConverter(ISerializationToolsFactory tools, IInstanceMembers members)
            : this(
                new LegacyInstanceBodyReader(tools, members),
                new LegacyTypeEmittingWriter(new Writer(tools, new InstanceBodyWriter(members)))) {}

        public LegacyInstanceTypeConverter(IInstanceMembers members)
            : this(new InstanceBodyReader(members), new TypeEmittingWriter(new InstanceBodyWriter(members))) {}

        LegacyInstanceTypeConverter(IReader reader, IWriter writer)
            : base(IsActivatedTypeSpecification.Default, reader, writer) {}

        private sealed class Writer : DecoratedWriter
        {
            private readonly ISerializationToolsFactory _tools;

            public Writer(ISerializationToolsFactory tools, IWriter writer) : base(writer)
            {
                _tools = tools;
            }

            public override void Write(IWriteContext context, object instance)
            {
                var type = instance.GetType();
                var configuration = _tools.GetConfiguration(type);
                if (configuration != null)
                {
                    if (configuration.IsObjectReference)
                    {
                        var references = context.Get<WriteReferences>();

                        var objectId = configuration.GetObjectId(instance);

                        var item = !(context.Current is IMemberElement);
                        if (item && references.Reserved.Contains(instance))
                        {
                            references.Reserved.Remove(instance);
                        }
                        else if (references.Contains(instance) || references.Reserved.Contains(instance))
                        {
                            context.Write(ReferenceProperty.Default, objectId);
                            return;
                        }

                        context.Write(IdentifierProperty.Default, objectId);
                        references.Add(instance);
                    }

                    if (configuration.Version > 0)
                    {
                        context.Write(VersionProperty.Default,
                                      configuration.Version.ToString(CultureInfo.InvariantCulture));
                    }
                }

                base.Write(context, instance);
            }
        }
    }
}