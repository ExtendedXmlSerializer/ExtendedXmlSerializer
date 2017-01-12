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

using ExtendedXmlSerialization.Conversion.Members;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    sealed class LegacyMemberFactory : IMemberFactory
    {
        private readonly ISerializationToolsFactory _tools;
        private readonly IMemberFactory _factory;

        public LegacyMemberFactory(ISerializationToolsFactory tools, IMemberFactory factory)
        {
            _tools = tools;
            _factory = factory;
        }

        public IMemberConverter Get(IMemberElement parameter)
        {
            var result = _factory.Get(parameter);
            var converter = result as IAssignableMemberConverter;
            if (converter != null)
            {
                var configuration = _tools.GetConfiguration(parameter.Metadata.DeclaringType);
                if (configuration != null)
                {
                    if (configuration.CheckPropertyEncryption(parameter.Metadata.Name))
                    {
                        var algorithm = _tools.EncryptionAlgorithm;
                        if (algorithm != null)
                        {
                            return new EncryptedMemberConverter(algorithm, converter);
                        }
                    }
                }
            }
            return result;
        }
    }
}