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
using System.Collections;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Write;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    class TypeEmittingWriter : TypeEmittingWriterBase
    {
        public TypeEmittingWriter(IWriter writer) : base(writer) {}

        protected override bool Emit(IWriteContext context, object instance, Type type)
        {
            if (!(context.Current is IMemberElement))
            {
                var declared = (context.Current as IDeclaredTypeElement)?.DeclaredType;
                var result = declared == null || declared.Type != type || CheckInstance(context, instance);
                return result;
            }
            return false;
        }

        protected virtual bool CheckInstance(IWriteContext context, object instance) => !(instance is IEnumerable);
    }
}