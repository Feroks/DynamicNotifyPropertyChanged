using FluentAssertions;
using System;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class ObjectExtensionsTrySetPropertyValueShould
	{
		private const string? NewValue = "New Value";
		private readonly TestClass _model = new();

		[Fact]
		public void ReturnTrueIfPropertyFound()
		{
			var value = _model.TrySetPropertyValue(nameof(TestClass.Value), NewValue);

			value
				.Should()
				.BeTrue();
		}

		[Fact]
		public void ReturnFalseIfPropertyNotFound()
		{
			var value = _model.TrySetPropertyValue("NotRealProperty", NewValue);

			value
				.Should()
				.BeFalse();
		}

		[Fact]
		public void SetValue()
		{
			_model.TrySetPropertyValue(nameof(TestClass.Value), NewValue);

			_model
				.Value
				.Should()
				.Be(NewValue);
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
