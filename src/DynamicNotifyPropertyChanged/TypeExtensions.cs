using System;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Set of extensions for <see cref="Type"/>.
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Get <see cref="Func{Object, Object}"/> that gets value of property called <see cref="propertyName"/> on passed object.
		/// </summary>
		/// <param name="source"><see cref="Type"/> containing property called <paramref name="propertyName"/>.</param>
		/// <param name="propertyName">Name fo the property.</param>
		/// <returns>Func that gets property value.</returns>
		public static Func<object, object?> GetPropertyGetter(this Type source, string propertyName)
		{
			return TypeCache
				.GetObjectGetterSetter(source, propertyName)
				.Value
				.Getter
				.Value;
		}

		/// <summary>
		/// Get <see cref="Func{Object, Object}"/> that sets value on property called <see cref="propertyName"/> on passed object.
		/// </summary>
		/// <param name="source"><see cref="Type"/> containing property called <paramref name="propertyName"/>.</param>
		/// <param name="propertyName">Name fo the property.</param>
		/// <returns>Action that sets property value.</returns>
		public static Action<object, object?> GetPropertySetter(this Type source, string propertyName)
		{
			return TypeCache
				.GetObjectGetterSetter(source, propertyName)
				.Value
				.Setter
				.Value;
		}
	}
}
