using System;

namespace DynamicNotifyPropertyChanged
{
	/// <summary>
	/// Set of Extensions for <see cref="object"/>.
	/// </summary>
	public static class ObjectExtensions
	{
		/// <summary>
		/// Get value of <paramref name="propertyName"/> on <paramref name="source"/> using cached compiled lambda.
		/// </summary>
		/// <param name="source">Object to get property value from.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <typeparam name="T"><see cref="Type"/> of <paramref name="source"/>.</typeparam>
		/// <returns>Name of property on <paramref name="source"/>.</returns>
		public static T? GetPropertyValue<T>(this object source, string propertyName)
		{
			return (T?)TypeCache
				.GetObjectGetterSetter(source.GetType(), propertyName)
				.Value
				.Getter
				.Value(source);
		}

		/// <summary>
		/// Set <paramref name="value"/> for property <paramref name="propertyName"/> on <paramref name="source"/>.
		/// </summary>
		/// <param name="source">Object whose property to change.</param>
		/// <param name="propertyName">Name of the property to change.</param>
		/// <param name="value">Value to set.</param>
		/// <typeparam name="T">Type of <paramref name="value"/>.</typeparam>
		public static void SetPropertyValue<T>(this object source, string propertyName, T? value)
		{
			TypeCache
				.GetObjectGetterSetter(source.GetType(), propertyName)
				.Value
				.Setter
				.Value(source, value);
		}
	}
}
