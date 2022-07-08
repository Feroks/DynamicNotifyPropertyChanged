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

		/// <param name="name">Name of the property.</param>
		/// <param name="type">Type of the property.</param>
		/// <param name="raisePropertyChanged">True, if <see cref="BaseNotifyPropertyChangedClass.PropertyChanged"/> should be raised.</param>
		/// <param name="raisePropertyChanging">True, if <see cref="BaseNotifyPropertyChangedClass.PropertyChanging"/> should be raised.</param>
		public DynamicProperty(string name, Type type, bool raisePropertyChanged, bool raisePropertyChanging)
		{
			Name = name;
			Type = type;
			RaisePropertyChanged = raisePropertyChanged;
			RaisePropertyChanging = raisePropertyChanging;
		}

		/// <summary>
		/// Get Name of the property.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Get <see cref="Type"/> of the property.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Get if <see cref="BaseNotifyPropertyChangedClass.PropertyChanged"/> should be triggered after property value is changed.
		/// </summary>
		public bool RaisePropertyChanged { get; set; } = true;

		/// <summary>
		/// Get if <see cref="BaseNotifyPropertyChangedClass.PropertyChanging"/> should be triggered before property value is changed.
		/// </summary>
		public bool RaisePropertyChanging { get; set; } = true;
	}
}
