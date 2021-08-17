using FluentAssertions;
using System;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class ObjectExtensionsSetPropertyValueShould
	{
		private const string? NewValue = "New Value";
		private readonly TestClass _model = new();

		[Fact]
		public void SetValue()
		{
			_model.SetPropertyValue(nameof(TestClass.Value), NewValue);

			_model
				.Value
				.Should()
				.Be(NewValue);
		}

		[Fact]
		public void ThrowExceptionIfPropertyNotFound()
		{
			Action action = () => _model.SetPropertyValue("NotRealProperty", NewValue);
			
			action
				.Should()
				.ThrowExactly<ArgumentException>();
		}
		
		[Fact]
		public void ThrowExceptionIfPropertyTypeMismatched()
		{
			Action action = () => _model.SetPropertyValue(nameof(TestClass.Value), 1);
			
			action
				.Should()
				.ThrowExactly<InvalidCastException>();
		}
	}
}
