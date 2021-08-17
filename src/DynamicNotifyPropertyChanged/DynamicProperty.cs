using System;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Class that describes property used for creating class dynamically.
	/// </summary>
	public class DynamicProperty
	{
		/// <param name="name">Name of the property.</param>
		/// <param name="type">Type of the property.</param>
		public DynamicProperty(string name, Type type)
		{
			Name = name;
			Type = type;
		}

		/// <summary>
		/// Get Name of the property.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Get <see cref="Type"/> of the property.
		/// </summary>
		public Type Type { get; }
	}
}
