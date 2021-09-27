using FluentAssertions;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class TypeExtensionsTryGetPropertyGetterShould
	{
		[Fact]
		public void ReturnTrueIfPropertyFound()
		{
			var type = typeof(TestClass);

			var factory1 = type.TryGetPropertyGetter(nameof(TestClass.Value), out _);

			factory1
				.Should()
				.BeTrue();
		}
		
		[Fact]
		public void ReturnFalseIfPropertyNotFound()
		{
			var type = typeof(TestClass);

			var factory1 = type.TryGetPropertyGetter("NotRealProperty", out _);

			factory1
				.Should()
				.BeFalse();
		}
		
		[Fact]
		public void ReturnCachedGetter()
		{
			var type = typeof(TestClass);

			type.TryGetPropertyGetter(nameof(TestClass.Value), out var getter1);
			type.TryGetPropertyGetter(nameof(TestClass.Value), out var getter2);

			getter1
				.Should()
				.Be(getter2);
		}
	}
}
