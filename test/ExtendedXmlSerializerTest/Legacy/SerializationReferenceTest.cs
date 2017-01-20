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

using System.Collections.Generic;
using ExtendedXmlSerialization.Legacy;
using ExtendedXmlSerialization.Test.Legacy.TestObjectConfigs;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test.Legacy
{
	public class SerializationReferenceTest : BaseTest
	{
		public SerializationReferenceTest()
		{
			/*Serializer = new ExtendedXmlSerializer(cfg =>
			                                       {
				                                       //cfg.ConfigureType<TestClassConcreteReference>().Property(p => p.Id).ObjectReference();
				                                       cfg.ConfigureType<TestClassReference>().Property(p => p.Id).ObjectReference();
				                                       cfg.ConfigureType<IReference>().Property(p => p.Id).ObjectReference();
			                                       });*/
			Serializer.SerializationToolsFactory = new SimpleSerializationToolsFactory()
			                                       {
				                                       Configurations =
					                                       new List<IExtendedXmlSerializerConfig>
					                                       {
						                                       new TestClassReferenceConfig(),
						                                       new InterfaceReferenceConfig()
					                                       }
			                                       };
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationRefernece()
		{
			var obj = new TestClassReference();
			obj.Id = 1;
			obj.CyclicReference = obj;
			obj.ObjectA = new TestClassReference {Id = 2};
			obj.ReferenceToObjectA = obj.ObjectA;
			obj.Lists = new List<IReference>
			            {
				            new TestClassReference {Id = 3},
				            new TestClassReference {Id = 4}
			            };

			CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Legacy.Resources.TestClassReference.xml", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationListWithReference()
		{
			var obj = new TestClassReferenceWithList();
			obj.Parent = new TestClassReference {Id = 1};
			var other = new TestClassReference {Id = 2, ObjectA = obj.Parent, ReferenceToObjectA = obj.Parent};

			obj.All = new List<IReference>
			          {
				          new TestClassReference {Id = 3, ObjectA = obj.Parent, ReferenceToObjectA = obj.Parent},
				          new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other},
				          other,
				          obj.Parent
			          };

			CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Legacy.Resources.TestClassReferenceWithList.xml", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationDictionaryWithReference()
		{
			var obj = new TestClassReferenceWithDictionary();
			obj.Parent = new TestClassReference {Id = 1};
			var other = new TestClassReference {Id = 2, ObjectA = obj.Parent, ReferenceToObjectA = obj.Parent};

			obj.All = new Dictionary<int, IReference>
			          {
				          {3, new TestClassReference {Id = 3, ObjectA = obj.Parent, ReferenceToObjectA = obj.Parent}},
				          {4, new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other}},
				          {2, other},
				          {1, obj.Parent}
			          };

			CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Legacy.Resources.TestClassReferenceWithDictionary.xml", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationListOfInterfaceReference()
		{
			var parent = new TestClassReference {Id = 1};
			var other = new TestClassReference {Id = 2, ObjectA = parent, ReferenceToObjectA = parent};

			var obj = new List<IReference>();
			obj.Add(new TestClassReference {Id = 3, ObjectA = parent, ReferenceToObjectA = parent});
			obj.Add(new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other});
			obj.Add(other);
			obj.Add(parent);

			CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Legacy.Resources.ListOfInterfaceReference.xml", obj);
		}
	}
}