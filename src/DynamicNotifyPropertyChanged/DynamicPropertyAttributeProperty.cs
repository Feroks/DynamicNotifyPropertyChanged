namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Class describing property on attribute.
	/// </summary>
	public class DynamicPropertyAttributeProperty
	{
		/// <param name="name">Name of the property.</param>
		/// <param name="value">Value of the property.</param>
		public DynamicPropertyAttributeProperty(string name, object value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Name of the property.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Value of the property.
		/// </summary>
		public object Value { get; }
	}
}
