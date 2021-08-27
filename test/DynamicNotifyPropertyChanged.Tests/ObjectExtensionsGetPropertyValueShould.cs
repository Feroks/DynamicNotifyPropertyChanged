using FluentAssertions;
using System;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class ObjectExtensionsGetPropertyValueShould
	{
		private readonly TestClass _model = new()
		{
			Value = "My Value",
			NullableBool = false
		};

		[Fact]
		public void GetValueGeneric()
		{
			var value = _model.GetPropertyValue<string>(nameof(TestClass.Value));

			value
				.Should()
				.Be(_model.Value);
		}
		
		[Fact]
		public void CastValueToObject()
		{
			var value = _model.GetPropertyValue<bool?>(nameof(TestClass.NullableBool));

			value
				.Should()
				.Be(_model.NullableBool);
		}
		
		[Fact]
		public void ThrowExceptionIfPropertyNotFound()
		{
			Func<string?> func = () => _model.GetPropertyValue<string>("NotRealProperty");

			func
				.Should()
				.ThrowExactly<ArgumentException>();
		}

		[Fact]
		public void ThrowExceptionIfPropertyTypeMismatched()
		{
			Func<int> func = () => _model.GetPropertyValue<int>(nameof(TestClass.Value));

			func
				.Should()
				.ThrowExactly<InvalidCastException>();
		}
	}
}
