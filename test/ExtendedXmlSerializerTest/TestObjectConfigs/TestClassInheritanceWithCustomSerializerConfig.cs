﻿// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Test.TestObject;

namespace ExtendedXmlSerialization.Test.TestObjectConfigs
{
    public class TestClassInheritanceWithCustomSerializerBaseConfig: CustomExtendedXmlSerializerConfig<TestClassInheritanceWithCustomSerializerBase>
    {
        public TestClassInheritanceWithCustomSerializerBaseConfig()
        {
            CustomSerializer(Serializer, Deserialize);
        }

        private TestClassInheritanceWithCustomSerializerBase Deserialize(XElement xElement)
        {
            return new TestClassInheritanceWithCustomSerializerBase()
            {
                FirstProperty = xElement.Attribute("First").Value
            };
        }

        private void Serializer(XmlWriter xmlWriter, TestClassInheritanceWithCustomSerializerBase obj)
        {
            xmlWriter.WriteAttributeString("First" , obj.FirstProperty);
        }
    }

    public class TestClassInheritanceWithCustomSerializerAConfig : CustomExtendedXmlSerializerConfig<TestClassInheritanceWithCustomSerializerA>
    {
        public TestClassInheritanceWithCustomSerializerAConfig()
        {
            CustomSerializer(Serializer, Deserialize);
        }

        private TestClassInheritanceWithCustomSerializerA Deserialize(XElement xElement)
        {
            return new TestClassInheritanceWithCustomSerializerA()
            {
                FirstProperty = xElement.Attribute("First").Value,
                SecondProperty = xElement.Attribute("Second").Value
            };
        }

        private void Serializer(XmlWriter xmlWriter, TestClassInheritanceWithCustomSerializerA obj)
        {
            xmlWriter.WriteAttributeString("First", obj.FirstProperty);
            xmlWriter.WriteAttributeString("Second", obj.SecondProperty);
        }
    }

    public class TestClassInheritanceWithCustomSerializerBConfig : CustomExtendedXmlSerializerConfig<TestClassInheritanceWithCustomSerializerB>
    {
        public TestClassInheritanceWithCustomSerializerBConfig()
        {
            CustomSerializer(Serializer, Deserialize);
        }

        private TestClassInheritanceWithCustomSerializerB Deserialize(XElement xElement)
        {
            return new TestClassInheritanceWithCustomSerializerB()
            {
                FirstProperty = xElement.Attribute("First").Value,
                ThirdProperty = xElement.Attribute("Third").Value
            };
        }

        private void Serializer(XmlWriter xmlWriter, TestClassInheritanceWithCustomSerializerB obj)
        {
            xmlWriter.WriteAttributeString("First", obj.FirstProperty);
            xmlWriter.WriteAttributeString("Third", obj.ThirdProperty);
        }
    }
}
