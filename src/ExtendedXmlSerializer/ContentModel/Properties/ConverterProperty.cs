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

using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	class StringProperty : DelegatedProperty<string>
	{
		public StringProperty(IIdentity identity) : this(Self<string>.Default.Get, identity) {}

		public StringProperty(Func<string, string> alter, IIdentity identity) : base(alter, alter, identity) {}
	}

	class ConverterProperty<T> : DelegatedProperty<T>
	{
		public ConverterProperty(IConverter<T> converter, IIdentity identity)
			: this(converter.Parse, converter.Format, identity) {}

		public ConverterProperty(Func<string, T> parser, Func<T, string> formatter, IIdentity identity) : base(parser, formatter, identity) {}
	}
}