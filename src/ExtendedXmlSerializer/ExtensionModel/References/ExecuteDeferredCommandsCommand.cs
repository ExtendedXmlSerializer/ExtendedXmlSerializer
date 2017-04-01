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

using System.Linq;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ExecuteDeferredCommandsCommand<T> : ICommand<IXmlReader> where T : ICommand<IXmlReader>
	{
		public static ExecuteDeferredCommandsCommand<T> Default { get; } = new ExecuteDeferredCommandsCommand<T>();
		ExecuteDeferredCommandsCommand() : this(ReaderContexts.Default, DeferredCommands.Default) {}

		readonly IReaderContexts _contexts;
		readonly IDeferredCommands _commands;

		public ExecuteDeferredCommandsCommand(IReaderContexts contexts, IDeferredCommands commands)
		{
			_contexts = contexts;
			_commands = commands;
		}

		public void Execute(IXmlReader parameter)
		{
			var commands = _commands.Get(_contexts.Get(parameter));
			var membered = commands.OfType<T>().ToArray();
			foreach (var command in membered)
			{
				command.Execute(parameter);
				commands.Remove(command);
			}
		}
	}
}