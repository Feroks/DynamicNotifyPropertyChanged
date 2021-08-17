using FluentAssertions;
using System;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class TypeExtensionsGetPropertyGetterShould
	{
		[Fact]
		public void ReturnCachedGetter()
		{
			var type = typeof(TestClass);

			var factory1 = type.GetPropertyGetter(nameof(TestClass.Value));
			var factory2 = type.GetPropertyGetter(nameof(TestClass.Value));

			factory1
				.Should()
				.Be(factory2);
		}

		[Fact]
		public void ThrowExceptionIfPropertyNotFound()
		{
			Func<Func<object, object?>> func = () => typeof(TestClass).GetPropertyGetter("NotRealProperty");

			func
				.Should()
				.ThrowExactly<ArgumentException>();
		}
	}
}
