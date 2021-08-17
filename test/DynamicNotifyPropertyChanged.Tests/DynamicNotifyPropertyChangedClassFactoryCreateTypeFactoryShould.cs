using FluentAssertions;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class DynamicNotifyPropertyChangedClassFactoryCreateTypeFactoryShould
	{
		[Fact]
		public void CreateFactorySuccessfully()
		{
			var factory = DynamicNotifyPropertyChangedClassFactory.CreateTypeFactory(typeof(object));

			factory
				.Should()
				.NotBeNull();
		}
		
		[Fact]
		public void ReturnCachedFactory()
		{
			var factory1 = DynamicNotifyPropertyChangedClassFactory.CreateTypeFactory(typeof(object));
			var factory2 = DynamicNotifyPropertyChangedClassFactory.CreateTypeFactory(typeof(object));

			factory1
				.Should()
				.BeSameAs(factory2);
		}
	}
}
