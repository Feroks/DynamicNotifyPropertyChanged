using FluentAssertions;
using System;
using Xunit;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class ObjectExtensionsTryGetPropertyValueShould
	{
		private readonly TestClass _model = new()
		{
			Value = "My Value",
			NullableBool = false
		};

		[Fact]
		public void ReturnTrueIfPropertyFound()
		{
			var value = _model.TryGetPropertyValue<string>(nameof(TestClass.Value), out _);

			value
				.Should()
				.BeTrue();
		}

		[Fact]
		public void ReturnFalseIfPropertyNotFound()
		{
			var value = _model.TryGetPropertyValue<string>("NotRealProperty", out _);

			value
				.Should()
				.BeFalse();
		}

		[Fact]
		public void GetValueGeneric()
		{
			_model.TryGetPropertyValue<string>(nameof(TestClass.Value), out var value);

			value
				.Should()
				.Be(_model.Value);
		}

		[Fact]
		public void CastValueToObject()
		{
			_model.TryGetPropertyValue<bool?>(nameof(TestClass.NullableBool), out var value);

			value
				.Should()
				.Be(_model.NullableBool);
		}

		[Fact]
		public void ThrowExceptionIfPropertyTypeMismatched()
		{
			Func<bool> func = () => _model.TryGetPropertyValue<int>(nameof(TestClass.Value), out var value);

			func
				.Should()
				.ThrowExactly<InvalidCastException>();
		}
	}
}
