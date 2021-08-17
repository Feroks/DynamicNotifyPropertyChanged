using FluentAssertions;
using System;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class DynamicPropertyComparerCompareShould
	{
		[Theory]
		[InlineData("abc", "abd", -1)]
		[InlineData("abc", "aac", 1)]
		public void CompareByNameFirst(string name1, string name2, int expectedResult)
		{
			var fixture = new DynamicPropertyComparer();

			var property1 = new DynamicProperty(name1, typeof(int));
			var property2 = new DynamicProperty(name2, typeof(int));

			var value = fixture.Compare(property1, property2);

			value
				.Should()
				.Be(expectedResult);
		}

		[Theory]
		[InlineData(typeof(int), typeof(string), -10)]
		[InlineData(typeof(string), typeof(int), 10)]
		[InlineData(typeof(int), typeof(int), 0)]
		public void CompareByTypeNameAsFallback(Type type1, Type type2, int expectedResult)
		{
			var fixture = new DynamicPropertyComparer();

			var property1 = new DynamicProperty("Property", type1);
			var property2 = new DynamicProperty("Property", type2);

			var value = fixture.Compare(property1, property2);

			value
				.Should()
				.Be(expectedResult);
		}
	}
}
