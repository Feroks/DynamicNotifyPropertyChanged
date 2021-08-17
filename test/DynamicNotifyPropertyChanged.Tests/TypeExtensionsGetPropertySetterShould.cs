using FluentAssertions;
using System;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class TypeExtensionsGetPropertySetterShould
	{
		[Fact]
		public void ReturnCachedGetter()
		{
			var type = typeof(TestClass);

			var factory1 = type.GetPropertySetter(nameof(TestClass.Value));
			var factory2 = type.GetPropertySetter(nameof(TestClass.Value));

			factory1
				.Should()
				.Be(factory2);
		}

		[Fact]
		public void ThrowExceptionIfPropertyNotFound()
		{
			Func<Action<object, object?>> func = () => typeof(TestClass).GetPropertySetter("NotRealProperty");

			func
				.Should()
				.ThrowExactly<ArgumentException>();
		}
	}
}
