using FluentAssertions;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class DynamicNotifyPropertyChangedClassFactoryCreateTypeInstanceShould
	{
		[Fact]
		public void CreateInstanceSuccessfully()
		{
			var obj = DynamicNotifyPropertyChangedClassFactory.CreateTypeInstance(typeof(object));

			obj
				.Should()
				.NotBeNull();
		}
		
		[Fact]
		public void CreateUniqueInstances()
		{
			var obj1 = DynamicNotifyPropertyChangedClassFactory.CreateTypeInstance(typeof(object));
			var obj2 = DynamicNotifyPropertyChangedClassFactory.CreateTypeInstance(typeof(object));

			obj1
				.Should()
				.NotBe(obj2);
		}
	}
}
