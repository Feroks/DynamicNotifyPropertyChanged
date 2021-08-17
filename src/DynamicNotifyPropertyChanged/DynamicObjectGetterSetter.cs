using System;

namespace DynamicNotifyPropertyChanged
{
	internal class DynamicObjectGetterSetter
	{
		internal DynamicObjectGetterSetter(Lazy<Func<object, object?>> getter, Lazy<Action<object, object?>> setter)
		{
			Getter = getter;
			Setter = setter;
		}

		/// <summary>
		/// Get object property getter.
		/// </summary>
		internal Lazy<Func<object, object?>> Getter { get; }

		/// <summary>
		/// Get object property setter.
		/// </summary>
		internal Lazy<Action<object, object?>> Setter { get; }
	}
}
