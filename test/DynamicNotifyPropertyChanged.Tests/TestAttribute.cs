using System;

namespace DynamicNotifyPropertyChanged.Tests
{
	public class TestAttribute : Attribute
	{
		public TestAttribute()
		{
		}

		public TestAttribute(string value)
		{
			Value = value;
		}

		public string? Value { get; set; }
	}
}
