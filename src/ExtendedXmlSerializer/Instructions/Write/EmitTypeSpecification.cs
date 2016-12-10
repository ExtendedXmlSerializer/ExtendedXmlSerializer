﻿// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.Instructions.Write
{
    class EmitTypeSpecification : ISpecification<IWritingContext>
    {
        public static EmitTypeSpecification Default { get; } = new EmitTypeSpecification();
        EmitTypeSpecification() {}

        public bool IsSatisfiedBy(IWritingContext parameter)
        {
            if (parameter.Current.Instance != parameter.Current.Root)
            {
                var context = parameter.GetMemberContext().GetValueOrDefault();
                switch (context.State)
                {
                    case ProcessState.MemberValue:
                        var member = context.Member.GetValueOrDefault();
                        var result = member.IsWritable && member.Value.GetType() != member.MemberType;
                        return result;
                }
                var emit = parameter.GetArrayContext() == null && parameter.GetDictionaryContext() == null;
                return emit;
            }
            return false;
        }
    }
}