﻿using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue407Tests
	{
		[Fact]
		public void Verify()
		{
			var type = new PathogenType{ Code = 'f', Description = "Fungus" };
			var instance = new List<object>
			{
				new LookupTable<PathogenDto>
				{
					Models = new List<PathogenDto>
					{
						new Pathogen{ PathogenType = type },
						new Pathogen{ PathogenType = type }
					}
				},
				new LookupTable<PathogenTypeDto>
				{
					Models = new List<PathogenTypeDto> { type }
				}
			};

			var serializer = new ConfigurationContainer().EnableReferences().ForTesting();
			var cycled = serializer.Cycle(instance);
			var expected = cycled[1].To<LookupTable<PathogenTypeDto>>().Models.Only();

			var table = cycled[0].To<LookupTable<PathogenDto>>();
			table.Models[0].PathogenType.Should()
			     .BeSameAs(expected);
			table.Models[1].PathogenType.Should()
			     .BeSameAs(expected);

		}

		sealed class LookupTable<T>
		{
			public List<T> Models { get; set; } = new List<T>();
		}

		class PathogenDto
		{
			public PathogenTypeDto PathogenType { get; set; }
		}

		sealed class Pathogen : PathogenDto {}

		class PathogenTypeDto {}

		sealed class PathogenType : PathogenTypeDto
		{
			public char Code { get; set; }

			public string Description { get; set; }
		}
	}
}