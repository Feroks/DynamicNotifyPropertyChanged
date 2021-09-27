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
		/// <returns>Value of property on <paramref name="source"/>.</returns>
		public static T? GetPropertyValue<T>(this object source, string propertyName)
		{
			return (T?)source
				.GetType()
				.GetPropertyGetter(propertyName)(source);
		}

		/// <summary>
		/// Get value of <paramref name="propertyName"/> on <paramref name="source"/> using cached compiled lambda.
		/// </summary>
		/// <param name="source">Object to get property value from.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="value">Property value</param>
		/// <typeparam name="T"><see cref="Type"/> of <paramref name="source"/>.</typeparam>
		/// <returns>True, if <paramref name="propertyName"/> exists on <paramref name="source"/>.</returns>
		public static bool TryGetPropertyValue<T>(this object source, string propertyName, out T? value)
		{
			if (source.GetType().TryGetPropertyGetter(propertyName, out var getter))
			{
				value = (T?)getter(source);
				return true;
			}
			
			value = default;
			return false;
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
			source
				.GetType()
				.GetPropertySetter(propertyName)(source, value);
		}
		
		/// <summary>
		/// Set <paramref name="value"/> for property <paramref name="propertyName"/> on <paramref name="source"/>.
		/// </summary>
		/// <param name="source">Object whose property to change.</param>
		/// <param name="propertyName">Name of the property to change.</param>
		/// <param name="value">Value to set.</param>
		/// <typeparam name="T">Type of <paramref name="value"/>.</typeparam>
		/// <returns>True, if <paramref name="propertyName"/> exists on <paramref name="source"/>.</returns>
		public static bool TrySetPropertyValue<T>(this object source, string propertyName, T? value)
		{
			if (source.GetType().TryGetPropertySetter(propertyName, out var setter))
			{
				setter(source, value);
				return true;
			}

			return false;
		}
	}
}
